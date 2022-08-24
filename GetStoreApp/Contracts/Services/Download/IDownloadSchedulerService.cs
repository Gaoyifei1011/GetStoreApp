using GetStoreApp.Models;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Download
{
    public interface IDownloadSchedulerService
    {
        Task InitializeDownloadMonitorAsync();

        Task CloseDownloadMonitorAsync();

        Task AddTaskAsync(DownloadModel downloadItem);

        Task ContinueTaskAsync(DownloadModel downloadItem);

        Task PauseTaskAsync(DownloadModel downloadItem);

        Task DeleteTaskAsync(DownloadModel downloadItem);
    }
}
