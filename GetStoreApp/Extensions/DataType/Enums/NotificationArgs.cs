namespace GetStoreApp.Extensions.DataType.Enums
{
    /// <summary>
    /// 应用通知参数
    /// </summary>
    public enum NotificationArgs
    {
        DownloadAborted = 0,
        InstallApp = 1,
        DownloadCompleted = 2,
        UWPUnInstallSuccessfully = 3,
        UWPUnInstallFailed = 4,
        WinGetInstallSuccessfully = 5,
        WinGetInstallFailed = 6,
        WinGetUnInstallSuccessfully = 7,
        WinGetUnInstallFailed = 8,
        WinGetUpgradeSuccessfully = 9,
        WinGetUpgradeFailed = 10
    }
}
