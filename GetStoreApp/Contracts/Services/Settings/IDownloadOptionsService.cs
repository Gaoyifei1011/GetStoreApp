using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace GetStoreApp.Contracts.Services.Settings
{
    public interface IDownloadOptionsService
    {
        StorageFolder DefaultFolder { get; }

        StorageFolder DownloadFolder { get; set; }

        int DownloadItem { get; set; }

        bool DownloadNotification { get; set; }

        List<int> DownloadItemList { get; }

        Task InitializeAsync();

        Task OpenFolderAsync(StorageFolder folder);

        Task CreateFolderAsync(string folderPath);

        Task SetFolderAsync(StorageFolder folder);

        Task SetItemValueAsync(int itemValue);

        Task SetNotificationAsync(bool notification);
    }
}
