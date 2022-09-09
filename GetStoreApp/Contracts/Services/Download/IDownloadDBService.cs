using GetStoreApp.Models.Download;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Download
{
    public interface IDownloadDBService
    {
        Task InitializeDownloadDBAsync();

        Task<bool> AddAsync(BackgroundModel downloadItem);

        Task<bool> UpdateFlagAsync(BackgroundModel downloadItem);

        Task<List<BackgroundModel>> QueryAsync(int downloadFlag);

        Task<bool> CheckDuplicatedAsync(string downloadKey);

        Task<bool> DeleteAsync(string downloadKey);

        Task<bool> DeleteSelectedAsync(List<string> selectedDownloadKeyList);

        Task<bool> ClearAsync();
    }
}
