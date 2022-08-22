using GetStoreApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Download
{
    public interface IDownloadDataService
    {
        Task<bool> CheckDuplicatedDataAsync(string downloadKey);

        Task<bool> AddDataAsync(DownloadModel download);

        Task UpdateDataAsync(DownloadModel download);

        Task<Tuple<List<DownloadModel>, bool>> QueryAllDownloadDataAsync();

        Task DeleteDownloadDataAsync(List<DownloadModel> selectedDownloadDataList);

        Task<bool> ClearDownloadDataAsync();
    }
}
