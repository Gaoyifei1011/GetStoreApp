using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models.Controls.Settings.Common;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Window;
using GetStoreApp.ViewModels.Base;
using GetStoreApp.Views.Pages;
using GetStoreApp.WindowsAPI.Dialogs;
using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace GetStoreApp.ViewModels.Controls.Settings.Common
{
    /// <summary>
    /// 设置页面：下载管理设置用户控件视图模型
    /// </summary>
    public sealed class DownloadOptionsViewModel : ViewModelBase
    {
        public List<DownloadModeModel> DownloadModeList { get; } = DownloadOptionsService.DownloadModeList;

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

        private DownloadModeModel _downloadMode = DownloadOptionsService.DownloadMode;

        public DownloadModeModel DownloadMode
        {
            get { return _downloadMode; }

            set
            {
                _downloadMode = value;
                OnPropertyChanged();
            }
        }

        // 下载管理说明
        public IRelayCommand DownloadInstructionCommand => new RelayCommand(() =>
        {
            NavigationService.NavigateTo(typeof(AboutPage), AppNaviagtionArgs.SettingsHelp);
        });

        // 打开文件存放目录
        public IRelayCommand OpenFolderCommand => new RelayCommand(async () =>
        {
            await DownloadOptionsService.OpenFolderAsync(DownloadFolder);
        });

        // 使用默认目录
        public IRelayCommand UseDefaultFolderCommand => new RelayCommand(async () =>
        {
            DownloadFolder = DownloadOptionsService.DefaultFolder;
            await DownloadOptionsService.SetFolderAsync(DownloadOptionsService.DefaultFolder);
        });

        // 修改下载目录
        public IRelayCommand ChangeFolderCommand => new RelayCommand(async () =>
        {
            if (Shell32Library.IsUserAnAdmin())
            {
                FolderPickerDialog dialog = new FolderPickerDialog()
                {
                    Title = ResourceService.GetLocalized("Settings/SelectFolder"),
                    Path = DownloadFolder.Path
                };

                bool Result = dialog.ShowDialog(Program.ApplicationRoot.MainWindow.GetMainWindowHandle());

                if (Result)
                {
                    DownloadFolder = await StorageFolder.GetFolderFromPathAsync(dialog.Path);
                    await DownloadOptionsService.SetFolderAsync(DownloadFolder);
                }
            }
            else
            {
                try
                {
                    FolderPicker folderPicker = new FolderPicker();
                    folderPicker.SuggestedStartLocation = PickerLocationId.Downloads;
                    WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, Program.ApplicationRoot.MainWindow.GetMainWindowHandle());
                    StorageFolder folder = await folderPicker.PickSingleFolderAsync();

                    if (folder is not null)
                    {
                        DownloadFolder = folder;
                        await DownloadOptionsService.SetFolderAsync(folder);
                    }
                }
                catch (Exception) { }
            }
        });

        // 修改同时下载文件数
        public IRelayCommand DownloadItemSelectCommand => new RelayCommand<string>(async (downloadItemIndex) =>
        {
            DownloadItem = Convert.ToInt32(downloadItemIndex);
            await DownloadOptionsService.SetItemAsync(DownloadItem);
        });

        // 修改下载文件的方式
        public IRelayCommand DownloadModeSelectCommand => new RelayCommand<string>(async (downloadModeIndex) =>
        {
            DownloadMode = DownloadModeList[Convert.ToInt32(downloadModeIndex)];
            await DownloadOptionsService.SetModeAsync(DownloadMode);
        });
    }
}
