namespace GetStoreApp.Extensions.DataType.Enums
{
    /// <summary>
    /// 应用通知参数
    /// </summary>
    public enum NotificationKind
    {
        DownloadAborted = 0,
        InstallApp = 1,
        DownloadCompleted = 2,
        RunAsAdministrator = 3,
        UWPUnInstallSuccessfully = 4,
        UWPUnInstallFailed = 5,
        WinGetInstallSuccessfully = 6,
        WinGetInstallFailed = 7,
        WinGetUnInstallSuccessfully = 8,
        WinGetUnInstallFailed = 9,
        WinGetUpgradeSuccessfully = 10,
        WinGetUpgradeFailed = 11
    }
}
