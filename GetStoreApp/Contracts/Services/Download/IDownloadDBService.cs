using GetStoreApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Download
{
    public interface IDownloadDBService
    {
        Task InitializeDownloadDBAsync();

        Task<bool> AddAsync(DownloadModel downloadItem);

        Task<bool> UpdateFlagAsync(DownloadModel downloadItem);

        Task<List<DownloadModel>> QueryAsync(int downloadFlag);

        Task<bool> DeleteAsync(DownloadModel downloadItem);

        Task<bool> DeleteSelectedAsync(List<DownloadModel> selectedDownloadDataList);

        Task<bool> ClearAsync();
    }
}
