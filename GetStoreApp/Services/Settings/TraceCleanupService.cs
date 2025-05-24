using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Download;
using GetStoreApp.Services.History;
using Windows.UI.Notifications;

namespace GetStoreApp.Services.Settings
{
    /// <summary>
    /// 痕迹清理服务
    /// </summary>
    public static class TraceCleanupService
    {
        /// <summary>
        /// 根据传入的清理选项清理应用痕迹
        /// </summary>
        public static bool CleanAppTraceAsync(CleanKind cleanupArgs)
        {
            return cleanupArgs switch
            {
                CleanKind.ActionCenter => ClearActionCenter(),
                CleanKind.Download => DownloadStorageService.ClearDownloadData(),
                CleanKind.History => HistoryStorageService.ClearData(),
                CleanKind.LocalFile => IOHelper.CleanFolder(DownloadOptionsService.DefaultDownloadFolder),
                _ => true,
            };
        }

        /// <summary>
        /// 清理操作中心的通知内容
        /// </summary>
        private static bool ClearActionCenter()
        {
            ToastNotificationManager.History.Clear();
            return true;
        }
    }
}
