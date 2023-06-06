using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.History;
using GetStoreApp.Services.Controls.Settings.Common;
using System;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace GetStoreApp.Services.Controls.Settings.Advanced
{
    /// <summary>
    /// 痕迹清理服务
    /// </summary>
    public static class TraceCleanupService
    {
        /// <summary>
        /// 根据传入的清理选项清理应用痕迹
        /// </summary>
        public static async Task<bool> CleanAppTraceAsync(CleanArgs cleanupArgs)
        {
            return cleanupArgs switch
            {
                CleanArgs.ActionCenter => ClearActionCenter(),
                CleanArgs.Download => await DownloadXmlService.ClearAsync(),
                CleanArgs.History => await HistoryXmlService.ClearAsync(),
                CleanArgs.JumpList => await CleanTaskbarJumpListAsync(),
                CleanArgs.LocalFile => IOHelper.CleanFolder(DownloadOptionsService.DefaultFolder),
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

        /// <summary>
        /// 清理任务栏右键跳转列表历史记录内容
        /// </summary>
        private static async Task<bool> CleanTaskbarJumpListAsync()
        {
            if (Program.ApplicationRoot.TaskbarJumpList is not null)
            {
                Program.ApplicationRoot.TaskbarJumpList.Items.Clear();
                await Program.ApplicationRoot.TaskbarJumpList.SaveAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
