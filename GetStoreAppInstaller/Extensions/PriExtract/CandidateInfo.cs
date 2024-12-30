using GetStoreAppInstaller.Extensions.DataType.Enums;

namespace GetStoreAppInstaller.Extensions.PriExtract
{
    public sealed class CandidateInfo
    {
        public byte Type { get; set; }

        public ResourceValueType ResourceValueType { get; set; }

        public ushort SourceFileIndex { get; set; }

        public ushort DataItemIndex { get; set; }
        public ushort DataItemSection { get; set; }

        public ushort DataLength { get; set; }

        public uint DataOffset { get; set; }
    }
}
