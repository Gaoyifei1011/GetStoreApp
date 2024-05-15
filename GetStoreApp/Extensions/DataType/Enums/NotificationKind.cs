namespace GetStoreApp.Extensions.DataType.Enums
{
    /// <summary>
    /// 应用通知参数
    /// </summary>
    public enum NotificationKind
    {
        AppUpdate = 0,
        InstallApp = 1,
        DownloadCompleted = 2,
        RunAsAdministrator = 3,
        NetworkError = 4,
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
