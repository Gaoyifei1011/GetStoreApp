using GetStoreApp.Models;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Download
{
    public interface IDownloadMonitorService
    {
        Task InitializeDownloadMonitorAsync();

        Task CloseDownloadMonitorAsync();

        Task AddTaskAsync(DownloadModel download);

        Task DeleteTaskAsync(DownloadModel download);
    }
}
