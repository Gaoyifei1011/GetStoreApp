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
        PinToTaskbarTip = 3,
        RunAsAdministrator = 4,
        UWPUnInstallSuccessfully = 5,
        UWPUnInstallFailed = 6,
        WinGetInstallSuccessfully = 7,
        WinGetInstallFailed = 8,
        WinGetUnInstallSuccessfully = 9,
        WinGetUnInstallFailed = 10,
        WinGetUpgradeSuccessfully = 11,
        WinGetUpgradeFailed = 12
    }
}
