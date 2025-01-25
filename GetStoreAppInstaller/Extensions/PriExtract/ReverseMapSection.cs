using GetStoreAppInstaller.Extensions.DataType.Methods;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GetStoreAppInstaller.Extensions.PriExtract
{
    public sealed class ReverseMapSection
    {
        public string SectionIdentifier { get; private set; }

        public uint SectionQualifier { get; private set; }

        public uint Flags { get; private set; }

        public uint SectionFlags { get; private set; }

        public uint SectionLength { get; private set; }

        public uint[] Mapping { get; private set; }

        public IReadOnlyList<ResourceMapScopeAndItem> ScopesList { get; private set; }

        public IReadOnlyList<ResourceMapScopeAndItem> ItemsList { get; private set; }

        public ReverseMapSection(string sectionIdentifier, BinaryReader binaryReader)
        {
            SectionIdentifier = sectionIdentifier;

            if (new string(binaryReader.ReadChars(16)) != SectionIdentifier)
            {
                throw new InvalidDataException("Unexpected section identifier.");
            }

            SectionQualifier = binaryReader.ReadUInt32();
            Flags = binaryReader.ReadUInt16();
            SectionFlags = binaryReader.ReadUInt16();
            SectionLength = binaryReader.ReadUInt32();
            binaryReader.ExpectUInt32(0);

            binaryReader.BaseStream.Seek(SectionLength - 16 - 24, SeekOrigin.Current);

            binaryReader.ExpectUInt32(0xDEF5FADE);
            binaryReader.ExpectUInt32(SectionLength);

            binaryReader.BaseStream.Seek(-8 - (SectionLength - 16 - 24), SeekOrigin.Current);

            using SubStream subStream = new(binaryReader.BaseStream, binaryReader.BaseStream.Position, (int)SectionLength - 16 - 24);
            using BinaryReader subBinaryReader = new(subStream, Encoding.ASCII);

            uint numItems = binaryReader.ReadUInt32();
            binaryReader.ExpectUInt32((uint)(binaryReader.BaseStream.Length - 8));

            uint[] mapping = new uint[numItems];
            for (int index = 0; index < numItems; index++)
            {
                mapping[index] = binaryReader.ReadUInt32();
            }

            Mapping = mapping;

            ushort maxFullPathLength = binaryReader.ReadUInt16();
            binaryReader.ExpectUInt16(0);
            uint numEntries = binaryReader.ReadUInt32();
            uint numScopes = binaryReader.ReadUInt32();
            binaryReader.ExpectUInt32(numItems);
            uint unicodeDataLength = binaryReader.ReadUInt32();
            binaryReader.ReadUInt32();

            List<Tuple<ushort, ushort, uint, uint, ushort>> scopeAndItemInfoList = [];

            for (int i = 0; i < numScopes + numItems; i++)
            {
                ushort parent = binaryReader.ReadUInt16();
                ushort fullPathLength = binaryReader.ReadUInt16();
                uint hashCode = binaryReader.ReadUInt32();
                uint nameOffset = binaryReader.ReadUInt16() | (((hashCode >> 24) & 0xF) << 16);
                ushort index = binaryReader.ReadUInt16();
                scopeAndItemInfoList.Add(Tuple.Create(parent, fullPathLength, hashCode, nameOffset, index));
            }

            List<Tuple<ushort, ushort, ushort>> scopeExInfo = [];

            for (int i = 0; i < numScopes; i++)
            {
                ushort scopeIndex = binaryReader.ReadUInt16();
                ushort childCount = binaryReader.ReadUInt16();
                ushort firstChildIndex = binaryReader.ReadUInt16();
                binaryReader.ExpectUInt16(0);
                scopeExInfo.Add(Tuple.Create(scopeIndex, childCount, firstChildIndex));
            }

            ushort[] itemIndexPropertyToIndex = new ushort[numItems];
            for (int i = 0; i < numItems; i++)
            {
                itemIndexPropertyToIndex[i] = binaryReader.ReadUInt16();
            }

            long unicodeDataOffset = binaryReader.BaseStream.Position;
            long asciiDataOffset = binaryReader.BaseStream.Position + unicodeDataLength * 2;

            ResourceMapScopeAndItem[] scopesArray = new ResourceMapScopeAndItem[numScopes];
            ResourceMapScopeAndItem[] itemsArray = new ResourceMapScopeAndItem[numItems];

            for (int i = 0; i < numScopes + numItems; i++)
            {
                bool nameInAscii = (scopeAndItemInfoList[i].Item3 & 0x20000000) is not 0;
                long pos = (nameInAscii ? asciiDataOffset : unicodeDataOffset) + (scopeAndItemInfoList[i].Item4 * (nameInAscii ? 1 : 2));
                binaryReader.BaseStream.Seek(pos, SeekOrigin.Begin);

                string name = string.Empty;

                if (scopeAndItemInfoList[i].Item2 is not 0)
                {
                    name = binaryReader.ReadNullTerminatedString(nameInAscii ? Encoding.ASCII : Encoding.Unicode);
                }

                ushort index = scopeAndItemInfoList[i].Item5;
                bool isScope = (scopeAndItemInfoList[i].Item3 & 0x10000000) is not 0;

                if (isScope)
                {
                    if (scopesArray[index] is not null)
                    {
                        throw new InvalidDataException();
                    }

                    scopesArray[index] = new ResourceMapScopeAndItem()
                    {
                        Index = index,
                        Parent = null,
                        Name = name
                    };
                }
                else
                {
                    if (itemsArray[index] is not null)
                    {
                        throw new InvalidDataException();
                    }

                    itemsArray[index] = new ResourceMapScopeAndItem()
                    {
                        Index = index,
                        Parent = null,
                        Name = name
                    };
                }
            }

            for (int i = 0; i < numScopes + numItems; i++)
            {
                ushort index = scopeAndItemInfoList[i].Item5;
                bool isScope = (scopeAndItemInfoList[i].Item3 & 0x10000000) is not 0;
                ushort parent = scopeAndItemInfoList[i].Item1;
                parent = scopeAndItemInfoList[parent].Item5;

                if (parent is not 0xFFFF)
                {
                    if (isScope)
                    {
                        if (parent != index)
                        {
                            scopesArray[index].Parent = scopesArray[parent];
                        }
                    }
                    else
                    {
                        itemsArray[index].Parent = scopesArray[parent];
                    }
                }
            }

            for (int i = 0; i < numScopes; i++)
            {
                ResourceMapScopeAndItem[] childrenArray = new ResourceMapScopeAndItem[scopeExInfo[i].Item2];

                for (int j = 0; j < childrenArray.Length; j++)
                {
                    Tuple<ushort, ushort, uint, uint, ushort> saiInfo = scopeAndItemInfoList[scopeExInfo[i].Item3 + j];

                    bool isScope = (saiInfo.Item3 & 0x10000000) is not 0;

                    childrenArray[j] = isScope ? scopesArray[saiInfo.Item5] : itemsArray[saiInfo.Item5];
                }

                scopesArray[i].Children = childrenArray;
            }

            ScopesList = scopesArray;
            ItemsList = itemsArray;
        }
    }
}
