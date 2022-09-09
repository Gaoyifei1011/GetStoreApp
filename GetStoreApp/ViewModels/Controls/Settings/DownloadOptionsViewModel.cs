using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Helpers;
using GetStoreApp.Models.Settings;
using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class DownloadOptionsViewModel : ObservableRecipient
    {
        private IDownloadOptionsService DownloadOptionsService { get; } = IOCHelper.GetService<IDownloadOptionsService>();

        public List<int> DownloadItemList => DownloadOptionsService.DownloadItemList;

        public List<DownloadModeModel> DownloadModeList => DownloadOptionsService.DownloadModeList;

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

        private DownloadModeModel _downloadMode;

        public DownloadModeModel DownloadMode
        {
            get { return _downloadMode; }

            set { SetProperty(ref _downloadMode, value); }
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

        // 修改下载目录
        public IAsyncRelayCommand ChangeFolderCommand => new AsyncRelayCommand(async () =>
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
        });

        // 修改同时下载文件数
        public IAsyncRelayCommand DownloadItemCommand => new AsyncRelayCommand(async () =>
        {
            await DownloadOptionsService.SetItemAsync(DownloadItem);
        });

        // 修改下载文件的方式
        public IAsyncRelayCommand DownloadModeCommand => new AsyncRelayCommand(async () =>
        {
            await DownloadOptionsService.SetModeAsync(DownloadMode);
        });

        public DownloadOptionsViewModel()
        {
            DownloadFolder = DownloadOptionsService.DownloadFolder;

            DownloadItem = DownloadOptionsService.DownloadItem;

            DownloadMode = DownloadOptionsService.DownloadMode;
        }
    }
}
