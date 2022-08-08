using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace GetStoreApp.Contracts.Services.Settings
{
    public interface IDownloadOptionsService
    {
        StorageFolder DownloadFolder { get; set; }

        int DownloadItem { get; set; }

        bool DownloadNotification { get; set; }

        List<int> DownloadItemList { get; set; }

        Task InitializeAsync();

        Task OpenFolderAsync(StorageFolder folder);

        Task SetFolderAsync(StorageFolder folder);

        Task SetItemValueAsync(int itemValue);

        Task SetNotificationAsync(bool notification);
    }
}
