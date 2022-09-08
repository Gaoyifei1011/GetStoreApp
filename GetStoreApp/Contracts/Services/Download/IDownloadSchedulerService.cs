using GetStoreApp.Extensions.Collection;
using GetStoreApp.Models;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Download
{
    public interface IDownloadSchedulerService
    {
        NotifyList<DownloadModel> DownloadingList { get; }

        NotifyList<DownloadModel> WaitingList { get; }

        Task InitializeDownloadMonitorAsync();

        Task CloseDownloadMonitorAsync();

        Task<int> AddTaskAsync(string fileName, string fileLink, string fileSHA1);

        Task<bool> ContinueTaskAsync(DownloadModel downloadItem);

        Task<bool> PauseTaskAsync(DownloadModel downloadItem);

        Task<bool> DeleteTaskAsync(DownloadModel downloadItem);
    }
}
