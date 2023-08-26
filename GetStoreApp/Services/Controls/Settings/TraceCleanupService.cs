using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.History;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace GetStoreApp.Services.Controls.Settings
{
    /// <summary>
    /// 痕迹清理服务
    /// </summary>
    public static class TraceCleanupService
    {
        /// <summary>
        /// 根据传入的清理选项清理应用痕迹
        /// </summary>
        public static async Task<bool> CleanAppTraceAsync(CleaKind cleanupArgs)
        {
            return cleanupArgs switch
            {
                CleaKind.ActionCenter => ClearActionCenter(),
                CleaKind.Download => await DownloadXmlService.ClearAsync(),
                CleaKind.History => await HistoryXmlService.ClearAsync(),
                CleaKind.LocalFile => IOHelper.CleanFolder(DownloadOptionsService.AppCacheFolder),
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
