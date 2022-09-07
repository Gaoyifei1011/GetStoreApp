using GetStoreApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Download
{
    public interface IDownloadSchedulerService
    {
        List<DownloadModel> DownloadingList { get; }

        List<DownloadModel> WaitingList { get; }

        Task InitializeDownloadMonitorAsync();

        Task CloseDownloadMonitorAsync();

        Task<int> AddTaskAsync(string fileName, string fileLink, string fileSHA1);

        Task<bool> ContinueTaskAsync(DownloadModel downloadItem);

        Task<bool> PauseTaskAsync(DownloadModel downloadItem);

        Task<bool> DeleteTaskAsync(DownloadModel downloadItem);
    }
}
