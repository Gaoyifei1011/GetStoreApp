using GetStoreAppInstaller.Extensions.DataType.Enums;
using GetStoreAppInstaller.Extensions.DataType.Methods;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GetStoreAppInstaller.Extensions.PriExtract
{
    public sealed class DecisionInfoSection
    {
        public uint SectionQualifier { get; private set; }

        public uint Flags { get; private set; }

        public uint SectionFlags { get; private set; }

        public uint SectionLength { get; private set; }

        public IReadOnlyList<Decision> DecisionsList { get; private set; }

        public IReadOnlyList<QualifierSet> QualifierSetsList { get; private set; }

        public IReadOnlyList<Qualifier> QualifiersList { get; private set; }

        public DecisionInfoSection(string sectionIdentifier, BinaryReader binaryReader)
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

            ushort numDistinctQualifiers = binaryReader.ReadUInt16();
            ushort numQualifiers = binaryReader.ReadUInt16();
            ushort numQualifierSets = binaryReader.ReadUInt16();
            ushort numDecisions = binaryReader.ReadUInt16();
            ushort numIndexTableEntries = binaryReader.ReadUInt16();
            ushort totalDataLength = binaryReader.ReadUInt16();

            List<DecisionInfo> decisionInfosList = new(numDecisions);
            for (int i = 0; i < numDecisions; i++)
            {
                ushort firstQualifierSetIndexIndex = binaryReader.ReadUInt16();
                ushort numQualifierSetsInDecision = binaryReader.ReadUInt16();
                decisionInfosList.Add(new DecisionInfo()
                {
                    FirstQualifierSetIndexIndex = firstQualifierSetIndexIndex,
                    NumQualifierSetsInDecision = numQualifierSetsInDecision,
                });
            }

            List<QualifierSetInfo> qualifierSetInfosList = new(numQualifierSets);
            for (int i = 0; i < numQualifierSets; i++)
            {
                ushort firstQualifierIndexIndex = binaryReader.ReadUInt16();
                ushort numQualifiersInSet = binaryReader.ReadUInt16();
                qualifierSetInfosList.Add(new QualifierSetInfo()
                {
                    FirstQualifierIndexIndex = firstQualifierIndexIndex,
                    NumQualifiersInSet = numQualifiersInSet,
                });
            }

            List<QualifierInfo> qualifierInfosList = new(numQualifiers);
            for (int i = 0; i < numQualifiers; i++)
            {
                ushort index = binaryReader.ReadUInt16();
                ushort priority = binaryReader.ReadUInt16();
                ushort fallbackScore = binaryReader.ReadUInt16();
                binaryReader.ExpectUInt16(0);
                qualifierInfosList.Add(new QualifierInfo()
                {
                    Index = index,
                    Priority = priority,
                    FallbackScore = fallbackScore
                });
            }

            List<DistinctQualifierInfo> distinctQualifierInfosList = new(numDistinctQualifiers);
            for (int i = 0; i < numDistinctQualifiers; i++)
            {
                binaryReader.ReadUInt16();
                QualifierType qualifierType = (QualifierType)binaryReader.ReadUInt16();
                binaryReader.ReadUInt16();
                binaryReader.ReadUInt16();
                uint operandValueOffset = binaryReader.ReadUInt32();
                distinctQualifierInfosList.Add(new DistinctQualifierInfo()
                {
                    QualifierType = qualifierType,
                    OperandValueOffset = operandValueOffset,
                });
            }

            ushort[] indexTableArray = new ushort[numIndexTableEntries];

            for (int i = 0; i < numIndexTableEntries; i++)
            {
                indexTableArray[i] = binaryReader.ReadUInt16();
            }

            long dataStartOffset = binaryReader.BaseStream.Position;

            List<Qualifier> qualifiersList = new(numQualifiers);

            for (int i = 0; i < numQualifiers; i++)
            {
                DistinctQualifierInfo distinctQualifierInfo = distinctQualifierInfosList[qualifierInfosList[i].Index];

                binaryReader.BaseStream.Seek(dataStartOffset + distinctQualifierInfo.OperandValueOffset * 2, SeekOrigin.Begin);

                string value = binaryReader.ReadNullTerminatedString(Encoding.Unicode);

                qualifiersList.Add(new Qualifier()
                {
                    Index = (ushort)i,
                    Type = distinctQualifierInfo.QualifierType,
                    Priority = qualifierInfosList[i].Priority,
                    FallbackScore = qualifierInfosList[i].FallbackScore,
                    Value = value
                });
            }

            QualifiersList = qualifiersList;

            List<QualifierSet> qualifierSetsList = new(numQualifierSets);

            for (int i = 0; i < numQualifierSets; i++)
            {
                List<Qualifier> qualifiersInSet = new(qualifierSetInfosList[i].NumQualifiersInSet);

                for (int j = 0; j < qualifierSetInfosList[i].NumQualifiersInSet; j++)
                {
                    qualifiersInSet.Add(qualifiersList[indexTableArray[qualifierSetInfosList[i].FirstQualifierIndexIndex + j]]);
                }

                qualifierSetsList.Add(new QualifierSet()
                {
                    Index = (ushort)i,
                    QualifiersList = qualifiersInSet
                });
            }

            QualifierSetsList = qualifierSetsList;

            List<Decision> decisionsList = new(numDecisions);

            for (int i = 0; i < numDecisions; i++)
            {
                List<QualifierSet> qualifierSetsInDecision = new(decisionInfosList[i].NumQualifierSetsInDecision);

                for (int j = 0; j < decisionInfosList[i].NumQualifierSetsInDecision; j++)
                {
                    qualifierSetsInDecision.Add(qualifierSetsList[indexTableArray[decisionInfosList[i].FirstQualifierSetIndexIndex + j]]);
                }

                decisionsList.Add(new Decision()
                {
                    Index = (ushort)i,
                    QualifierSetsList = qualifierSetsInDecision
                });
            }

            DecisionsList = decisionsList;
        }
    }
}
