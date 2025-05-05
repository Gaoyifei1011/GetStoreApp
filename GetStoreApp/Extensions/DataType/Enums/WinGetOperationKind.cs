namespace GetStoreApp.Extensions.DataType.Enums
{
    /// <summary>
    /// WinGet 操作枚举值
    /// </summary>
    public enum WinGetOperationKind
    {
        SearchDownloadCopy = 0,
        SearchInstallCopy = 1,
        SearchRepairCopy = 2,
        SearchDownload = 3,
        SearchInstall = 4,
        SearchRepair = 5,
        SearchDownloadCommand = 6,
        SearchInstallCommand = 7,
        SearchRepairCommand = 8,
        Uninstall = 9,
        Upgrade = 10,
        VersionInfo = 11
    }
}
