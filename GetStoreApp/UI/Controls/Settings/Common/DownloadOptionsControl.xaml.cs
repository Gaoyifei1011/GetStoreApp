using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models.Controls.Settings.Common;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Window;
using GetStoreApp.Views.Pages;
using GetStoreApp.WindowsAPI.Dialogs;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.Storage;
using WinRT;

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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DownloadFolder)));
            }
        }

        private int _downloadItem = DownloadOptionsService.DownloadItem;

        public int DownloadItem
        {
            get { return _downloadItem; }

            set
            {
                _downloadItem = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DownloadItem)));
            }
        }

        private DownloadModeModel _downloadMode = DownloadOptionsService.DownloadMode;

        public DownloadModeModel DownloadMode
        {
            get { return _downloadMode; }

            set
            {
                _downloadMode = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DownloadMode)));
            }
        }

        public List<DownloadModeModel> DownloadModeList { get; } = DownloadOptionsService.DownloadModeList;

        public event PropertyChangedEventHandler PropertyChanged;

        public DownloadOptionsControl()
        {
            InitializeComponent();
        }

        public bool IsDownloadItemChecked(int selectedIndex, int index)
        {
            return selectedIndex == index;
        }

        public bool IsDownloadModeChecked(string selectedInternalName, string internalName)
        {
            return selectedInternalName == internalName;
        }

        /// <summary>
        /// 下载管理说明
        /// </summary>
        public void OnDownloadInstructionClicked(object sender, RoutedEventArgs args)
        {
            NavigationService.NavigateTo(typeof(AboutPage), AppNaviagtionArgs.SettingsHelp);
        }

        /// <summary>
        /// 打开文件存放目录
        /// </summary>
        public async void OnOpenFolderClicked(object sender, RoutedEventArgs args)
        {
            await DownloadOptionsService.OpenFolderAsync(DownloadFolder);
        }

        /// <summary>
        /// 使用默认目录
        /// </summary>
        public async void OnUseDefaultFolderClicked(object sender, RoutedEventArgs args)
        {
            DownloadFolder = DownloadOptionsService.DefaultFolder;
            await DownloadOptionsService.SetFolderAsync(DownloadOptionsService.DefaultFolder);
        }

        /// <summary>
        /// 修改下载目录
        /// </summary>
        public async void OnChangeFolderClicked(object sender, RoutedEventArgs args)
        {
            FolderPickerDialog dialog = new FolderPickerDialog()
            {
                Title = ResourceService.GetLocalized("Settings/SelectFolder"),
                Path = DownloadFolder.Path
            };

            bool Result = dialog.ShowDialog(Program.ApplicationRoot.MainWindow.Handle);

            if (Result)
            {
                DownloadFolder = await StorageFolder.GetFolderFromPathAsync(dialog.Path);
                await DownloadOptionsService.SetFolderAsync(DownloadFolder);
            }
        }

        /// <summary>
        /// 修改同时下载文件数
        /// </summary>
        public async void OnDownloadItemSelectClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem item = sender.As<ToggleMenuFlyoutItem>();
            if (item.Tag is not null)
            {
                DownloadItem = Convert.ToInt32(item.Tag);
                await DownloadOptionsService.SetItemAsync(DownloadItem);
            }
        }

        /// <summary>
        /// 修改下载文件的方式
        /// </summary>
        public async void OnDownloadModeSelectClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem item = sender.As<ToggleMenuFlyoutItem>();
            if (item.Tag is not null)
            {
                DownloadMode = DownloadModeList[Convert.ToInt32(item.Tag)];
                await DownloadOptionsService.SetModeAsync(DownloadMode);
            }
        }
    }
}
