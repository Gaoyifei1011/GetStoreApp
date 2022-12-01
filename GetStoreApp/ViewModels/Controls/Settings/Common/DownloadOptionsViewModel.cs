using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Settings.Common;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Window;
using GetStoreApp.UI.Dialogs.Settings;
using GetStoreApp.ViewModels.Base;
using GetStoreApp.Views.Pages;
using GetStoreApp.WindowsAPI.Dialogs;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using Windows.Storage;
using WinRT.Interop;

namespace GetStoreApp.ViewModels.Controls.Settings.Common
{
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
            App.NavigationArgs = AppNaviagtionArgs.SettingsHelp;
            NavigationService.NavigateTo(typeof(AboutPage));
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
            StorageFolder Folder;

            while (true)
            {
                // 选择文件夹
                FolderPickerDialog dialog = new FolderPickerDialog()
                {
                    Title = ResourceService.GetLocalized("/Settings/SelectFolder"),
                    Path = DownloadFolder.Path
                };

                bool Result = dialog.ShowDialog(WindowNative.GetWindowHandle(App.MainWindow));

                if (Result && !string.IsNullOrEmpty(dialog.Path))
                {
                    Folder = await StorageFolder.GetFolderFromPathAsync(dialog.Path);

                    bool CheckResult = IOHelper.GetFolderAuthorization(Folder, FileSystemRights.Write);

                    if (CheckResult)
                    {
                        ContentDialogResult result = await new FolderAccessFailedDialog().ShowAsync();

                        if (result == ContentDialogResult.Primary)
                        {
                            continue;
                        }
                        else if (result == ContentDialogResult.Secondary)
                        {
                            DownloadFolder = DownloadOptionsService.DefaultFolder;
                            await DownloadOptionsService.SetFolderAsync(DownloadFolder);
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        DownloadFolder = Folder;
                        await DownloadOptionsService.SetFolderAsync(DownloadFolder);
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        });

        /// <summary>
        /// 修改同时下载文件数
        /// </summary>
        public IRelayCommand DownloadItemSelectCommand => new RelayCommand<string>(async (downloadItemIndex) =>
        {
            DownloadItem = Convert.ToInt32(downloadItemIndex);
            await DownloadOptionsService.SetItemAsync(DownloadItem);
        });

        /// <summary>
        /// 修改下载文件的方式
        /// </summary>
        public IRelayCommand DownloadModeSelectCommand => new RelayCommand<string>(async (downloadModeIndex) =>
        {
            DownloadMode = DownloadModeList[Convert.ToInt32(downloadModeIndex)];
            await DownloadOptionsService.SetModeAsync(DownloadMode);
        });
    }
}
