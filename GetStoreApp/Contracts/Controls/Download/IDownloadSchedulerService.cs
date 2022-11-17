using GetStoreApp.Extensions.DataType.Collections;
using GetStoreApp.Models.Controls.Download;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Controls.Download
{
    public interface IDownloadSchedulerService
    {
        NotifyList<BackgroundModel> DownloadingList { get; }

        NotifyList<BackgroundModel> WaitingList { get; }

        Task InitializeDownloadSchedulerAsync();

        Task CloseDownloadSchedulerAsync();

        Task<bool> AddTaskAsync(BackgroundModel backgroundItem, string operation);

        Task<bool> ContinueTaskAsync(BackgroundModel downloadItem);

        Task<bool> PauseTaskAsync(string downloadKey, string gID, int downloadFlag);

        Task PauseAllTaskAsync();

        Task<bool> DeleteTaskAsync(string downloadKey, string gID, int downloadFlag);
    }
}
