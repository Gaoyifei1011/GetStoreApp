using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Settings;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.UI.Notifications;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace GetStoreApp.UI.Controls.Settings.Common
{
    /// <summary>
    /// 设置页面：下载管理设置控件
    /// </summary>
    public sealed partial class DownloadOptionsControl : Expander, INotifyPropertyChanged
    {
        private StorageFolder _downloadFolder = DownloadOptionsService.DownloadFolder;

        public StorageFolder DownloadFolder
        {
            get { return _downloadFolder; }

            set
            {
                _downloadFolder = value;
                OnPropertyChanged();
            }
        }

        private int _downloadItem = DownloadOptionsService.DownloadItem;

        public int DownloadItem
        {
            get { return _downloadItem; }

            set
            {
                _downloadItem = value;
                OnPropertyChanged();
            }
        }

        private GroupOptionsModel _downloadMode = DownloadOptionsService.DownloadMode;

        public GroupOptionsModel DownloadMode
        {
            get { return _downloadMode; }

            set
            {
                _downloadMode = value;
                OnPropertyChanged();
            }
        }

        public List<GroupOptionsModel> DownloadModeList { get; } = DownloadOptionsService.DownloadModeList;

        public event PropertyChangedEventHandler PropertyChanged;

        public DownloadOptionsControl()
        {
            InitializeComponent();
        }

        public bool IsDownloadItemChecked(int selectedIndex, int index)
        {
            return selectedIndex == index;
        }

        public bool IsDownloadModeChecked(GroupOptionsModel selectedMember, GroupOptionsModel comparedMember)
        {
            return selectedMember.SelectedValue == comparedMember.SelectedValue;
        }

        /// <summary>
        /// 打开文件存放目录
        /// </summary>
        public async void OnOpenFolderClicked(object sender, RoutedEventArgs args)
        {
            await DownloadOptionsService.OpenFolderAsync(DownloadFolder);
        }

        /// <summary>
        /// 修改下载目录
        /// </summary>
        public async void OnChangeFolderClicked(object sender, RoutedEventArgs args)
        {
            MenuFlyoutItem item = sender as MenuFlyoutItem;
            if (item.Tag is not null)
            {
                switch ((string)item.Tag)
                {
                    case "AppCache":
                        {
                            DownloadFolder = DownloadOptionsService.AppCacheFolder;
                            DownloadOptionsService.SetFolder(DownloadFolder);
                            break;
                        }
                    case "Download":
                        {
                            DownloadFolder = await StorageFolder.GetFolderFromPathAsync(InfoHelper.UserDataPath.Downloads);
                            DownloadOptionsService.SetFolder(DownloadFolder);
                            break;
                        }
                    case "Desktop":
                        {
                            DownloadFolder = await StorageFolder.GetFolderFromPathAsync(InfoHelper.UserDataPath.Desktop);
                            DownloadOptionsService.SetFolder(DownloadFolder);
                            break;
                        }
                    case "Custom":
                        {
                            try
                            {
                                FolderPicker folderPicker = new FolderPicker();
                                InitializeWithWindow.Initialize(folderPicker, Program.ApplicationRoot.MainWindow.Handle);
                                folderPicker.SuggestedStartLocation = PickerLocationId.Downloads;

                                StorageFolder downloadFolder = await folderPicker.PickSingleFolderAsync();

                                if (downloadFolder is not null)
                                {
                                    DownloadFolder = downloadFolder;
                                    DownloadOptionsService.SetFolder(downloadFolder);
                                }
                            }
                            catch (Exception)
                            {
                                new FolderPickerNotification(this).Show();
                            }
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 修改同时下载文件数
        /// </summary>
        public void OnDownloadItemSelectClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.Tag is not null)
            {
                DownloadItem = Convert.ToInt32(item.Tag);
                DownloadOptionsService.SetItem(DownloadItem);
            }
        }

        /// <summary>
        /// 修改下载文件的方式
        /// </summary>
        public void OnDownloadModeSelectClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.Tag is not null)
            {
                DownloadMode = DownloadModeList[Convert.ToInt32(item.Tag)];
                DownloadOptionsService.SetMode(DownloadMode);
            }
        }

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
