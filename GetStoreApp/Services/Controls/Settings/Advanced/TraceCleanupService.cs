using GetStoreApp.Contracts.Services.Controls.Download;
using GetStoreApp.Contracts.Services.Controls.History;
using GetStoreApp.Contracts.Services.Controls.Settings.Advanced;
using GetStoreApp.Contracts.Services.Controls.Settings.Common;
using GetStoreApp.Extensions.DataType.Enum;
using GetStoreApp.Helpers.Root;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Controls.Settings.Advanced
{
    public class TraceCleanupService : ITraceCleanupService
    {
        private IHistoryDBService HistoryDBService { get; } = IOCHelper.GetService<IHistoryDBService>();

        private IDownloadDBService DownloadDBService { get; } = IOCHelper.GetService<IDownloadDBService>();

        private IDownloadOptionsService DownloadOptionsService { get; } = IOCHelper.GetService<IDownloadOptionsService>();

        /// <summary>
        /// 根据传入的清理选项清理应用痕迹
        /// </summary>
        public async Task<bool> CleanAppTraceAsync(CleanArgs cleanupArgs)
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
