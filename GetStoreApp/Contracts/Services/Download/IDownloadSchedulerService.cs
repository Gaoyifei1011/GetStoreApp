using GetStoreApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Download
{
    public interface IDownloadSchedulerService
    {
        List<DownloadModel> GetDownloadingList();

        List<DownloadModel> GetWaitingList();

        Task InitializeDownloadMonitorAsync();

        Task CloseDownloadMonitorAsync();

        Task<bool> AddTaskAsync(DownloadModel downloadItem);

        Task<bool> ContinueTaskAsync(DownloadModel downloadItem);

        Task<bool> PauseTaskAsync(DownloadModel downloadItem);

        Task<bool> DeleteTaskAsync(DownloadModel downloadItem);
    }
}
