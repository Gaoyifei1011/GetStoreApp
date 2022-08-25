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

        Task AddTaskAsync(DownloadModel downloadItem);

        Task ContinueTaskAsync(DownloadModel downloadItem);

        Task PauseTaskAsync(DownloadModel downloadItem);

        Task DeleteTaskAsync(DownloadModel downloadItem);

        Task ContinueAllTaskAsync(List<DownloadModel> downloadItemList);

        Task PauseAllTaskAsync(List<DownloadModel> downloadItemList);

        Task DeleteSelectedTaskAsync(List<DownloadModel> downloadItemList);
    }
}
