namespace GetStoreApp.Extensions.DataType.Enums
{
    /// <summary>
    /// 应用通知参数
    /// </summary>
    public enum NotificationKind
    {
        AppUpdate = 0,
        DownloadAborted = 1,
        InstallApp = 2,
        DownloadCompleted = 3,
        PinToTaskbarTip = 4,
        RunAsAdministrator = 5,
        UWPUnInstallSuccessfully = 6,
        UWPUnInstallFailed = 7,
        WinGetInstallSuccessfully = 8,
        WinGetInstallFailed = 9,
        WinGetUnInstallSuccessfully = 10,
        WinGetUnInstallFailed = 11,
        WinGetUpgradeSuccessfully = 12,
        WinGetUpgradeFailed = 13
    }
}
