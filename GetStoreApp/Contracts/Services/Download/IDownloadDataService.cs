using GetStoreApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Download
{
    public interface IDownloadDataService
    {
        Task InitializeDownloadDataAsync();

        Task<bool> CheckDuplicatedDataAsync(string downloadKey);

        Task<bool> AddDataAsync(DownloadModel downloadItem);

        Task UpdateDataAsync(DownloadModel downloadItem);

        Task<Tuple<List<DownloadModel>, bool>> QueryAllDownloadDataAsync();

        Task DeleteDownloadDataAsync(DownloadModel downloadItem);

        Task<bool> ClearDownloadDataAsync();
    }
}
