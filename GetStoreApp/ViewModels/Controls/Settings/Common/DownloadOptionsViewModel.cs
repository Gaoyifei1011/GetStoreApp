using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Controls.Settings.Common;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Contracts.Services.Window;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Settings.Common;
using GetStoreApp.UI.Dialogs.ContentDialogs.Settings;
using GetStoreApp.Views.Pages;
using GetStoreAppWindowsAPI.Dialogs;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using Windows.Storage;
using WinUIEx;

namespace GetStoreApp.ViewModels.Controls.Settings.Common
{
    public class DownloadOptionsViewModel : ObservableRecipient
    {
        private IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        private IDownloadOptionsService DownloadOptionsService { get; } = ContainerHelper.GetInstance<IDownloadOptionsService>();

        private INavigationService NavigationService { get; } = ContainerHelper.GetInstance<INavigationService>();

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

        // 下载管理说明
        public IRelayCommand DownloadInstructionCommand => new RelayCommand(() =>
        {
            App.NavigationArgs = AppNaviagtionArgs.SettingsHelp;
            NavigationService.NavigateTo(typeof(SettingsPage));
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

                bool Result = dialog.ShowDialog(WindowExtensions.GetWindowHandle(App.MainWindow));

                if (Result && !string.IsNullOrEmpty(dialog.Path))
                {
                    Folder = await StorageFolder.GetFolderFromPathAsync(dialog.Path);

                    bool CheckResult = IOHelper.GetFolderAuthorization(Folder, FileSystemRights.Write);

                    if (CheckResult)
                    {
                        DownloadFolder = Folder;
                        await DownloadOptionsService.SetFolderAsync(DownloadFolder);
                        break;
                    }
                    else
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
                }
                else
                {
                    break;
                }
            }
        });

        // 修改同时下载文件数
        public IRelayCommand DownloadItemCommand => new RelayCommand(async () =>
        {
            await DownloadOptionsService.SetItemAsync(DownloadItem);
        });

        // 修改下载文件的方式
        public IRelayCommand DownloadModeCommand => new RelayCommand(async () =>
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
