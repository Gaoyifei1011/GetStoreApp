using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class DownloadOptionsViewModel : ObservableRecipient
    {
        private IDownloadOptionsService DownloadOptionsService { get; } = IOCHelper.GetService<IDownloadOptionsService>();

        public List<int> DownloadItemList => DownloadOptionsService.DownloadItemList;

        private StorageFolder _downloadFolder;

        public StorageFolder DownloadFolder
        {
            get { return _downloadFolder; }

            set { SetProperty(ref _downloadFolder, value); }
        }

        private int _downloadItem;

        public int DownloadItem
        {
            get { return _downloadItem; }

            set { SetProperty(ref _downloadItem, value); }
        }

        public IAsyncRelayCommand OpenFolderCommand => new AsyncRelayCommand(async () =>
        {
            await DownloadOptionsService.OpenFolderAsync(DownloadFolder);
        });

        // 使用默认目录
        public IAsyncRelayCommand UseDefaultFolderCommand => new AsyncRelayCommand(async () =>
        {
            DownloadFolder = DownloadOptionsService.DefaultFolder;
            await DownloadOptionsService.SetFolderAsync(DownloadOptionsService.DefaultFolder);
        });

        public IAsyncRelayCommand ChangeFolderCommand => new AsyncRelayCommand(ChangeFolderAsync);

        public IAsyncRelayCommand DownloadItemCommand => new AsyncRelayCommand(async () =>
        {
            await DownloadOptionsService.SetItemValueAsync(DownloadItem);
        });

        public DownloadOptionsViewModel()
        {
            DownloadFolder = DownloadOptionsService.DownloadFolder;

            DownloadItem = DownloadOptionsService.DownloadItem;
        }

        /// <summary>
        /// 更改下载文件目录
        /// </summary>
        private async Task ChangeFolderAsync()
        {
            await DownloadOptionsService.CreateFolderAsync(DownloadOptionsService.DefaultFolder.Path);

            FolderPicker folderPicker = new FolderPicker();

            IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);

            folderPicker.SuggestedStartLocation = PickerLocationId.Downloads;

            StorageFolder Folder = await folderPicker.PickSingleFolderAsync();

            if (Folder != null)
            {
                DownloadFolder = Folder;
                await DownloadOptionsService.SetFolderAsync(DownloadFolder);
            }
        }
    }
}
