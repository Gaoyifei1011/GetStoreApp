using GetStoreApp.Contracts.Controls.Download;
using GetStoreApp.Contracts.Controls.History;
using GetStoreApp.Contracts.Controls.Settings.Advanced;
using GetStoreApp.Contracts.Controls.Settings.Common;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Controls.Settings.Advanced
{
    public class TraceCleanupService : ITraceCleanupService
    {
        private IHistoryDBService HistoryDBService { get; } = ContainerHelper.GetInstance<IHistoryDBService>();

        private IDownloadDBService DownloadDBService { get; } = ContainerHelper.GetInstance<IDownloadDBService>();

        private IDownloadOptionsService DownloadOptionsService { get; } = ContainerHelper.GetInstance<IDownloadOptionsService>();

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
