using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class DownloadOptionsViewModel : ObservableRecipient
    {
        private StorageFolder _downloadFolder = ApplicationData.Current.LocalCacheFolder;

        public StorageFolder DownloadFolder
        {
            get { return _downloadFolder; }

            set { SetProperty(ref _downloadFolder, value); }
        }

        private int _downloadItem = 1;

        public int DownloadItem
        {
            get { return _downloadItem; }

            set { SetProperty(ref _downloadItem, value); }
        }

        private bool _isnotificationOpened = true;

        public bool IsNotificationOpened
        {
            get { return _isnotificationOpened; }

            set { SetProperty(ref _isnotificationOpened, value); }
        }

        public List<int> DownloadItemList { get; set; } = new List<int>() { 1, 2, 3 };

        public IAsyncRelayCommand OpenFolderCommand { get; set; }

        public IAsyncRelayCommand ChangeFolderCommand { get; set; }

        public IAsyncRelayCommand NotificationStateCommand { get; set; }

        public DownloadOptionsViewModel()
        {
            OpenFolderCommand = new AsyncRelayCommand(async () =>
            {
                await Windows.System.Launcher.LaunchFolderAsync(DownloadFolder);
            });

            ChangeFolderCommand = new AsyncRelayCommand(ChangeFolderAsync);
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
            }
        }
    }
}
