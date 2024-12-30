using GetStoreAppInstaller.Extensions.DataType.Methods;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GetStoreAppInstaller.Extensions.PriExtract
{
    public sealed class DataItemSection
    {
        public uint SectionQualifier { get; private set; }

        public uint Flags { get; private set; }

        public uint SectionFlags { get; private set; }

        public uint SectionLength { get; private set; }

        public IReadOnlyList<ByteSpan> DataItemsList { get; private set; }

        public DataItemSection(string sectionIdentifier, BinaryReader binaryReader)
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

            binaryReader.ExpectUInt32(0);
            ushort numStrings = binaryReader.ReadUInt16();
            ushort numBlobs = binaryReader.ReadUInt16();
            uint totalDataLength = binaryReader.ReadUInt32();

            List<ByteSpan> dataItemsList = new(numStrings + numBlobs);

            long dataStartOffset = binaryReader.BaseStream.Position +
                numStrings * 2 * sizeof(ushort) + numBlobs * 2 * sizeof(uint);

            for (int i = 0; i < numStrings; i++)
            {
                ushort stringOffset = binaryReader.ReadUInt16();
                ushort stringLength = binaryReader.ReadUInt16();
                dataItemsList.Add(new ByteSpan()
                {
                    Offset = sectionPosition + dataStartOffset + stringOffset,
                    Length = stringLength
                });
            }

            for (int i = 0; i < numBlobs; i++)
            {
                uint blobOffset = binaryReader.ReadUInt32();
                uint blobLength = binaryReader.ReadUInt32();
                dataItemsList.Add(new ByteSpan()
                {
                    Offset = sectionPosition + dataStartOffset + blobOffset,
                    Length = blobLength
                });
            }

            DataItemsList = dataItemsList;
        }
    }
}
