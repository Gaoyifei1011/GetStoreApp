using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.History;
using GetStoreApp.Services.Controls.Settings.Common;
using System;
using System.Threading.Tasks;

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
            switch (cleanupArgs)
            {
                case CleanArgs.History: return await HistoryDBService.ClearAsync();
                case CleanArgs.JumpList: return await CleanTaskbarJumpListAsync();
                case CleanArgs.Download: return await DownloadDBService.ClearAsync();
                case CleanArgs.LocalFile: return IOHelper.CleanFolder(DownloadOptionsService.DefaultFolder);
                default: return true;
            }
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
