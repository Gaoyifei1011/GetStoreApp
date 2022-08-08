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

        private bool _downloadNotification;

        public bool DownloadNotification
        {
            get { return _downloadNotification; }

            set { SetProperty(ref _downloadNotification, value); }
        }

        public List<int> DownloadItemList { get; set; }

        public IAsyncRelayCommand OpenFolderCommand { get; set; }

        public IAsyncRelayCommand ChangeFolderCommand { get; set; }

        public IAsyncRelayCommand DownloadItemCommand { get; set; }

        public IAsyncRelayCommand NotificationStateCommand { get; set; }

        public DownloadOptionsViewModel()
        {
            DownloadItemList = DownloadOptionsService.DownloadItemList;

            DownloadFolder = DownloadOptionsService.DownloadFolder;

            DownloadItem = DownloadOptionsService.DownloadItem;

            DownloadNotification = DownloadOptionsService.DownloadNotification;

            OpenFolderCommand = new AsyncRelayCommand(async () =>
            {
                await DownloadOptionsService.OpenFolderAsync(DownloadFolder);
            });

            ChangeFolderCommand = new AsyncRelayCommand(ChangeFolderAsync);

            DownloadItemCommand = new AsyncRelayCommand(async () =>
            {
                await DownloadOptionsService.SetItemValueAsync(DownloadItem);
            });

            NotificationStateCommand = new AsyncRelayCommand<bool>(async (param) =>
            {
                DownloadNotification = param;
                await DownloadOptionsService.SetNotificationAsync(param);
            });
        }

        /// <summary>
        /// 更改下载文件目录
        /// </summary>
        private async Task ChangeFolderAsync()
        {
            FolderPicker folderPicker = new FolderPicker();

            IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);

            folderPicker.SuggestedStartLocation = PickerLocationId.Desktop;

            StorageFolder Folder = await folderPicker.PickSingleFolderAsync();

            if (Folder != null)
            {
                DownloadFolder = Folder;
                await DownloadOptionsService.SetFolderAsync(DownloadFolder);
            }
        }
    }
}
