namespace GetStoreAppInstaller.Extensions.PriExtract
{
    public sealed class ScopeAndItemInfo
    {
        public ushort Parent { get; set; }

        public ushort FullPathLength { get; set; }

        public bool IsScope { get; set; }

        public bool NameInAscii { get; set; }

        public uint NameOffset { get; set; }

        public ushort Index { get; set; }
    }
}
