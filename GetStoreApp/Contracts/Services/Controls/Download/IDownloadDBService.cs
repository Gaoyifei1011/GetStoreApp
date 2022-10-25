using GetStoreApp.Extensions.DataType.Enum;
using GetStoreApp.Models.Controls.Download;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Controls.Download
{
    public interface IDownloadDBService
    {
        Task InitializeDownloadDBAsync();

        Task<bool> AddAsync(BackgroundModel downloadItem);

        Task<bool> UpdateFlagAsync(string downloadKey, int downloadFlag);

        Task<bool> UpdateFileSizeAsync(string downloadKey, double fileSize);

        Task<List<BackgroundModel>> QueryWithFlagAsync(int downloadFlag);

        Task<BackgroundModel> QueryWithKeyAsync(string downloadKey);

        Task<DuplicatedDataInfoArgs> CheckDuplicatedAsync(string downloadKey);

        Task<bool> DeleteAsync(string downloadKey);

        Task<bool> DeleteSelectedAsync(List<BackgroundModel> selectedDownloadList);

        Task<bool> ClearAsync();
    }
}
