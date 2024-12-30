namespace GetStoreAppInstaller.Extensions.PriExtract
{
    public sealed class FolderInfo
    {
        public ushort ParentFolder { get; set; }

        public ushort NumFoldersInFolder { get; set; }

        public ushort FirstFolderInFolder { get; set; }

        public ushort NumFilesInFolder { get; set; }

        public ushort FirstFileInFolder { get; set; }

        public ushort FolderNameLength { get; set; }

        public ushort FullPathLength { get; set; }

        public uint FolderNameOffset { get; set; }
    }
}
