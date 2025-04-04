using GetStoreAppInstaller.Extensions.DataType.Enums;
using GetStoreAppInstaller.Extensions.DataType.Methods;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GetStoreAppInstaller.Extensions.PriExtract
{
    public sealed class ResourceMapSection
    {
        public uint SectionQualifier { get; private set; }

        public uint Flags { get; private set; }

        public uint SectionFlags { get; private set; }

        public uint SectionLength { get; private set; }

        public HierarchicalSchemaReference HierarchicalSchemaReference { get; private set; }
        public int SchemaSectionIndex { get; private set; }

        public int DecisionInfoSectionIndex { get; private set; }

        public IReadOnlyDictionary<ushort, CandidateSet> CandidateSetsDict { get; private set; }

        public ResourceMapSection(string sectionIdentifier, BinaryReader binaryReader, bool version2, ref object[] sectionList)
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

            long sectionPosition = (binaryReader.BaseStream as SubStream)?.SubStreamPosition ?? 0;

            ushort environmentReferencesLength = binaryReader.ReadUInt16();
            ushort numEnvironmentReferences = binaryReader.ReadUInt16();

            if (version2)
            {
                if (environmentReferencesLength is not 0 || numEnvironmentReferences is not 0)
                {
                    throw new InvalidDataException();
                }
            }
            else
            {
                if (environmentReferencesLength is 0 || numEnvironmentReferences is 0)
                {
                    throw new InvalidDataException();
                }
            }

            SchemaSectionIndex = binaryReader.ReadUInt16();
            ushort hierarchicalSchemaReferenceLength = binaryReader.ReadUInt16();
            DecisionInfoSectionIndex = binaryReader.ReadUInt16();
            ushort resourceValueTypeTableSize = binaryReader.ReadUInt16();
            ushort ItemToItemInfoGroupCount = binaryReader.ReadUInt16();
            ushort itemInfoGroupCount = binaryReader.ReadUInt16();
            uint itemInfoCount = binaryReader.ReadUInt32();
            uint numCandidates = binaryReader.ReadUInt32();
            uint dataLength = binaryReader.ReadUInt32();
            uint largeTableLength = binaryReader.ReadUInt32();

            if (sectionList[DecisionInfoSectionIndex] is null)
            {
                return;
            }

            byte[] environmentReferencesDataArray = binaryReader.ReadBytes(environmentReferencesLength);
            byte[] schemaReferenceDataArray = binaryReader.ReadBytes(hierarchicalSchemaReferenceLength);

            if (schemaReferenceDataArray.Length is not 0)
                using (BinaryReader r = new(new MemoryStream(schemaReferenceDataArray, false)))
                {
                    ushort majorVersion = r.ReadUInt16();
                    ushort minorVersion = r.ReadUInt16();
                    r.ExpectUInt32(0);
                    uint checksum = r.ReadUInt32();
                    uint numScopes = r.ReadUInt32();
                    uint numItems = r.ReadUInt32();

                    HierarchicalSchemaVersion hierarchicalSchemaVersion = new()
                    {
                        MajorVersion = majorVersion,
                        MinorVersion = minorVersion,
                        Checksum = checksum,
                        NumScopes = numScopes,
                    };

                    ushort stringDataLength = r.ReadUInt16();
                    r.ExpectUInt16(0);
                    uint unknown1 = r.ReadUInt32();
                    uint unknown2 = r.ReadUInt32();
                    string uniqueName = r.ReadNullTerminatedString(Encoding.Unicode);

                    if (uniqueName.Length != stringDataLength - 1)
                        throw new InvalidDataException();

                    HierarchicalSchemaReference = new HierarchicalSchemaReference()
                    {
                        Version = hierarchicalSchemaVersion,
                        UniqueName = uniqueName,
                        Unknown1 = unknown1,
                        Unknown2 = unknown2,
                    };
                }

            List<ResourceValueType> resourceValueTypeTableList = new(resourceValueTypeTableSize);

            for (int i = 0; i < resourceValueTypeTableSize; i++)
            {
                binaryReader.ExpectUInt32(4);
                ResourceValueType resourceValueType = (ResourceValueType)binaryReader.ReadUInt32();
                resourceValueTypeTableList.Add(resourceValueType);
            }

            List<ItemToItemInfoGroup> itemToItemInfoGroupsList = [];

            for (int i = 0; i < ItemToItemInfoGroupCount; i++)
            {
                ushort firstItem = binaryReader.ReadUInt16();
                ushort itemInfoGroup = binaryReader.ReadUInt16();

                itemToItemInfoGroupsList.Add(new ItemToItemInfoGroup()
                {
                    FirstItem = firstItem,
                    ItemInfoGroup = itemInfoGroup
                });
            }

            List<ItemInfoGroup> itemInfoGroupsList = [];
            for (int i = 0; i < itemInfoGroupCount; i++)
            {
                ushort groupSize = binaryReader.ReadUInt16();
                ushort firstItemInfo = binaryReader.ReadUInt16();
                itemInfoGroupsList.Add(new ItemInfoGroup()
                {
                    FirstItemInfo = firstItemInfo,
                    GroupSize = groupSize
                });
            }

            List<ItemInfo> itemInfosList = [];
            for (int i = 0; i < itemInfoCount; i++)
            {
                ushort decision = binaryReader.ReadUInt16();
                ushort firstCandidate = binaryReader.ReadUInt16();
                itemInfosList.Add(new ItemInfo()
                {
                    Decision = decision,
                    FirstCandidate = firstCandidate,
                });
            }

            byte[] largeTable = binaryReader.ReadBytes((int)largeTableLength);

            if (largeTable.Length is not 0)
            {
                using BinaryReader r = new(new MemoryStream(largeTable, false));
                uint ItemToItemInfoGroupCountLarge = r.ReadUInt32();
                uint itemInfoGroupCountLarge = r.ReadUInt32();
                uint itemInfoCountLarge = r.ReadUInt32();

                for (int i = 0; i < ItemToItemInfoGroupCountLarge; i++)
                {
                    uint firstItem = r.ReadUInt32();
                    uint itemInfoGroup = r.ReadUInt32();
                    itemToItemInfoGroupsList.Add(new ItemToItemInfoGroup()
                    {
                        FirstItem = firstItem,
                        ItemInfoGroup = itemInfoGroup
                    });
                }

                for (int i = 0; i < itemInfoGroupCountLarge; i++)
                {
                    uint groupSize = r.ReadUInt32();
                    uint firstItemInfo = r.ReadUInt32();
                    itemInfoGroupsList.Add(new ItemInfoGroup()
                    {
                        FirstItemInfo = firstItemInfo,
                        GroupSize = groupSize
                    });
                }

                for (int i = 0; i < itemInfoCountLarge; i++)
                {
                    uint decision = r.ReadUInt32();
                    uint firstCandidate = r.ReadUInt32();
                    itemInfosList.Add(new ItemInfo()
                    {
                        Decision = decision,
                        FirstCandidate = firstCandidate,
                    });
                }

                if (r.BaseStream.Position != r.BaseStream.Length)
                {
                    throw new InvalidDataException();
                }
            }

            List<CandidateInfo> candidateInfos = new((int)numCandidates);
            for (int i = 0; i < numCandidates; i++)
            {
                byte type = binaryReader.ReadByte();

                if (type is 0x01)
                {
                    ResourceValueType resourceValueType = resourceValueTypeTableList[binaryReader.ReadByte()];
                    ushort sourceFileIndex = binaryReader.ReadUInt16();
                    ushort dataItemIndex = binaryReader.ReadUInt16();
                    ushort dataItemSection = binaryReader.ReadUInt16();
                    candidateInfos.Add(new CandidateInfo()
                    {
                        Type = 0x01,
                        ResourceValueType = resourceValueType,
                        SourceFileIndex = sourceFileIndex,
                        DataItemIndex = dataItemIndex,
                        DataItemSection = dataItemSection,
                        DataLength = 0,
                        DataOffset = 0
                    });
                }
                else if (type is 0x00)
                {
                    ResourceValueType resourceValueType = resourceValueTypeTableList[binaryReader.ReadByte()];
                    ushort length = binaryReader.ReadUInt16();
                    uint stringOffset = binaryReader.ReadUInt32();
                    candidateInfos.Add(new CandidateInfo()
                    {
                        Type = 0x00,
                        ResourceValueType = resourceValueType,
                        SourceFileIndex = 0,
                        DataItemIndex = 0,
                        DataItemSection = 0,
                        DataLength = length,
                        DataOffset = stringOffset
                    });
                }
                else
                {
                    throw new InvalidDataException();
                }
            }

            long stringDataStartOffset = binaryReader.BaseStream.Position;

            Dictionary<ushort, CandidateSet> candidateSetsList = [];

            for (int itemToItemInfoGroupIndex = 0; itemToItemInfoGroupIndex < itemToItemInfoGroupsList.Count; itemToItemInfoGroupIndex++)
            {
                ItemToItemInfoGroup itemToItemInfoGroup = itemToItemInfoGroupsList[itemToItemInfoGroupIndex];

                ItemInfoGroup itemInfoGroup;

                if (itemToItemInfoGroup.ItemInfoGroup < itemInfoGroupsList.Count)
                {
                    itemInfoGroup = itemInfoGroupsList[(int)itemToItemInfoGroup.ItemInfoGroup];
                }
                else
                {
                    itemInfoGroup = new ItemInfoGroup()
                    {
                        GroupSize = 1,
                        FirstItemInfo = (uint)(itemToItemInfoGroup.ItemInfoGroup - itemInfoGroupsList.Count)
                    };
                }

                for (uint itemInfoIndex = itemInfoGroup.FirstItemInfo; itemInfoIndex < itemInfoGroup.FirstItemInfo + itemInfoGroup.GroupSize; itemInfoIndex++)
                {
                    ItemInfo itemInfo = itemInfosList[(int)itemInfoIndex];

                    ushort decisionIndex = (ushort)itemInfo.Decision;

                    Decision decision = (sectionList[DecisionInfoSectionIndex] as DecisionInfoSection)?.DecisionsList[decisionIndex];

                    List<Candidate> candidatesList = new(decision.QualifierSetsList.Count);

                    for (int i = 0; i < decision.QualifierSetsList.Count; i++)
                    {
                        CandidateInfo candidateInfo = candidateInfos[(int)itemInfo.FirstCandidate + i];

                        if (candidateInfo.Type is 0x01)
                        {
                            int? sourceFile = candidateInfo.SourceFileIndex is 0 ? null : candidateInfo.SourceFileIndex - 1;

                            candidatesList.Add(new Candidate()
                            {
                                QualifierSet = decision.QualifierSetsList[i].Index,
                                Type = candidateInfo.ResourceValueType,
                                SourceFileIndex = sourceFile,
                                DataItemSectionAndIndex = ValueTuple.Create(candidateInfo.DataItemSection, candidateInfo.DataItemIndex),
                                Data = null
                            });
                        }
                        else if (candidateInfo.Type is 0x00)
                        {
                            ByteSpan data = new()
                            {
                                Offset = sectionPosition + stringDataStartOffset + candidateInfo.DataOffset,
                                Length = candidateInfo.DataLength
                            };

                            candidatesList.Add(new Candidate()
                            {
                                QualifierSet = decision.QualifierSetsList[i].Index,
                                Type = candidateInfo.ResourceValueType,
                                SourceFileIndex = null,
                                DataItemSectionAndIndex = default,
                                Data = data
                            });
                        }
                    }

                    ushort resourceMapItemIndex = (ushort)(itemToItemInfoGroup.FirstItem + (itemInfoIndex - itemInfoGroup.FirstItemInfo));

                    CandidateSet candidateSet = new()
                    {
                        ResourceMapSectionAndIndex = ValueTuple.Create(SchemaSectionIndex, (int)resourceMapItemIndex),
                        DecisionIndex = decisionIndex,
                        CandidatesList = candidatesList
                    };

                    candidateSetsList.Add(resourceMapItemIndex, candidateSet);
                }
            }

            CandidateSetsDict = candidateSetsList;
        }
    }
}
