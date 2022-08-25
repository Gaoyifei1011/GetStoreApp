using GetStoreApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Download
{
    public interface IDownloadDBService
    {
        Task InitializeDownloadDBAsync();

        Task<bool> AddDataAsync(DownloadModel downloadItem);

        Task<bool> UpdateFlagAsync(DownloadModel downloadItem);

        Task<List<DownloadModel>> QueryDownloadDataAsync(int downloadFlag);

        Task DeleteDownloadDataAsync(DownloadModel downloadItem);

        Task<bool> ClearDownloadDataAsync();
    }
}
