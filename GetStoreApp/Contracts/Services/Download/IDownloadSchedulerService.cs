using GetStoreApp.Extensions.Collection;
using GetStoreApp.Models.Download;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Download
{
    public interface IDownloadSchedulerService
    {
        NotifyList<BackgroundModel> DownloadingList { get; }

        NotifyList<BackgroundModel> WaitingList { get; }

        Task InitializeDownloadMonitorAsync();

        Task CloseDownloadMonitorAsync();

        Task<int> AddTaskAsync(string fileName, string fileLink, string fileSHA1);

        Task<bool> ContinueTaskAsync(BackgroundModel downloadItem);

        Task<bool> PauseTaskAsync(string downloadKey, string gID, int downloadFlag);

        Task<bool> DeleteTaskAsync(string downloadKey, string gID, int downloadFlag);
    }
}
