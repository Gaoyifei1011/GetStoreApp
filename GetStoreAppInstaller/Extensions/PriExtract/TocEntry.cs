namespace GetStoreAppInstaller.Extensions.PriExtract
{
    public sealed class TocEntry
    {
        public string SectionIdentifier { get; set; }

        public ushort Flags { get; set; }

        public ushort SectionFlags { get; set; }

        public uint SectionQualifier { get; set; }

        public uint SectionOffset { get; set; }

        public uint SectionLength { get; set; }
    }
}
