using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.History;
using GetStoreApp.Services.Controls.Settings.Common;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Controls.Settings.Advanced
{
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
                case CleanArgs.Download: return await DownloadDBService.ClearAsync();
                case CleanArgs.LocalFile: return IOHelper.CleanFolder(DownloadOptionsService.DefaultFolder);
                case CleanArgs.WebCache: return true;
                default: return true;
            }
        }
    }
}
