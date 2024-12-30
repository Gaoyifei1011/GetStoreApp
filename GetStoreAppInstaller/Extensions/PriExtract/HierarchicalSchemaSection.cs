using GetStoreAppInstaller.Extensions.DataType.Methods;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GetStoreAppInstaller.Extensions.PriExtract
{
    public sealed class HierarchicalSchemaSection
    {
        public uint SectionQualifier { get; private set; }

        public uint Flags { get; private set; }

        public uint SectionFlags { get; private set; }

        public uint SectionLength { get; private set; }

        public HierarchicalSchemaVersion Version { get; private set; }
        public string UniqueName { get; private set; }
        public string Name { get; private set; }
        public IReadOnlyList<ResourceMapScopeAndItem> ScopesList { get; private set; }
        public IReadOnlyList<ResourceMapScopeAndItem> ItemsList { get; private set; }

        public HierarchicalSchemaSection(string sectionIdentifier, BinaryReader binaryReader, bool extendedVersion)
        {
            if (new string(binaryReader.ReadChars(16)) != sectionIdentifier)
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

            if (binaryReader.BaseStream.Length is 0)
            {
                Version = null;
                UniqueName = null;
                Name = null;
                ScopesList = [];
                ItemsList = [];
            }

            binaryReader.ExpectUInt16(1);
            ushort uniqueNameLength = binaryReader.ReadUInt16();
            ushort nameLength = binaryReader.ReadUInt16();
            binaryReader.ExpectUInt16(0);

            bool extendedHNames = false;

            if (extendedVersion)
            {
                extendedHNames = new string(binaryReader.ReadChars(16)) switch
                {
                    "[def_hnamesx]  \0" => true,
                    "[def_hnames]   \0" => false,
                    _ => throw new InvalidDataException(),
                };
            }

            // Hierarchical Schema 版本信息
            ushort majorVersion = binaryReader.ReadUInt16();
            ushort minorVersion = binaryReader.ReadUInt16();
            binaryReader.ExpectUInt32(0);
            uint checksum = binaryReader.ReadUInt32();
            uint numScopes = binaryReader.ReadUInt32();
            uint numItems = binaryReader.ReadUInt32();

            Version = new HierarchicalSchemaVersion()
            {
                MajorVersion = majorVersion,
                MinorVersion = minorVersion,
                Checksum = checksum,
                NumScopes = numScopes,
                NumItems = numItems
            };

            UniqueName = binaryReader.ReadNullTerminatedString(Encoding.Unicode);
            Name = binaryReader.ReadNullTerminatedString(Encoding.Unicode);

            if (UniqueName.Length != uniqueNameLength - 1 || Name.Length != nameLength - 1)
            {
                throw new InvalidDataException();
            }

            binaryReader.ExpectUInt16(0);
            ushort maxFullPathLength = binaryReader.ReadUInt16();
            binaryReader.ExpectUInt16(0);
            binaryReader.ExpectUInt32(numScopes + numItems);
            binaryReader.ExpectUInt32(numScopes);
            binaryReader.ExpectUInt32(numItems);
            uint unicodeDataLength = binaryReader.ReadUInt32();
            binaryReader.ReadUInt32();

            if (extendedHNames)
            {
                binaryReader.ReadUInt32();
            }

            List<ScopeAndItemInfo> scopeAndItemInfosList = new((int)(numScopes + numItems));

            for (int i = 0; i < numScopes + numItems; i++)
            {
                ushort parent = binaryReader.ReadUInt16();
                ushort fullPathLength = binaryReader.ReadUInt16();
                char uppercaseFirstChar = (char)binaryReader.ReadUInt16();
                byte nameLength2 = binaryReader.ReadByte();
                byte flags = binaryReader.ReadByte();
                uint nameOffset = binaryReader.ReadUInt16() | (uint)((flags & 0xF) << 16);
                ushort index = binaryReader.ReadUInt16();
                bool isScope = (flags & 0x10) is not 0;
                bool nameInAscii = (flags & 0x20) is not 0;
                scopeAndItemInfosList.Add(new ScopeAndItemInfo()
                {
                    Parent = parent,
                    FullPathLength = fullPathLength,
                    IsScope = isScope,
                    NameInAscii = nameInAscii,
                    NameOffset = nameOffset,
                    Index = index,
                });
            }

            List<ScopeExInfo> scopeExInfosList = new((int)numScopes);

            for (int i = 0; i < numScopes; i++)
            {
                ushort scopeIndex = binaryReader.ReadUInt16();
                ushort childCount = binaryReader.ReadUInt16();
                ushort firstChildIndex = binaryReader.ReadUInt16();
                binaryReader.ExpectUInt16(0);
                scopeExInfosList.Add(new ScopeExInfo()
                {
                    ScopeIndex = scopeIndex,
                    ChildCount = childCount,
                    FirstChildIndex = firstChildIndex
                });
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
                long pos = scopeAndItemInfosList[i].NameInAscii
                    ? asciiDataOffset + scopeAndItemInfosList[i].NameOffset
                    : unicodeDataOffset + scopeAndItemInfosList[i].NameOffset * 2;

                binaryReader.BaseStream.Seek(pos, SeekOrigin.Begin);

                string name = scopeAndItemInfosList[i].FullPathLength is not 0
                    ? binaryReader.ReadNullTerminatedString(scopeAndItemInfosList[i].NameInAscii ? Encoding.ASCII : Encoding.Unicode)
                    : string.Empty;

                ushort index = scopeAndItemInfosList[i].Index;

                if (scopeAndItemInfosList[i].IsScope)
                {
                    if (scopesArray[index] is not null)
                    {
                        throw new InvalidDataException();
                    }

                    scopesArray[index] = new ResourceMapScopeAndItem()
                    {
                        Index = index,
                        Parent = null,
                        Name = name,
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
                        Name = name,
                    };
                }
            }

            for (int i = 0; i < numScopes + numItems; i++)
            {
                ushort index = scopeAndItemInfosList[i].Index;

                ushort parent = scopeAndItemInfosList[scopeAndItemInfosList[i].Parent].Index;

                if (parent is not 0xFFFF)
                {
                    if (scopeAndItemInfosList[i].IsScope)
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
                List<ResourceMapScopeAndItem> children = new(scopeExInfosList[i].ChildCount);

                for (int j = 0; j < scopeExInfosList[i].ChildCount; j++)
                {
                    ScopeAndItemInfo saiInfo = scopeAndItemInfosList[scopeExInfosList[i].FirstChildIndex + j];

                    if (saiInfo.IsScope)
                    {
                        children.Add(scopesArray[saiInfo.Index]);
                    }
                    else
                    {
                        children.Add(itemsArray[saiInfo.Index]);
                    }
                }

                scopesArray[i].Children = children;
            }

            ScopesList = scopesArray;
            ItemsList = itemsArray;
        }
    }
}
