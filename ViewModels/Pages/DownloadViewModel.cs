using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.Helpers;
using GetStoreApp.Models;
using GetStoreApp.UI.Dialogs;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace GetStoreApp.ViewModels.Pages
{
    public class DownloadViewModel : ObservableRecipient
    {
        private StorageFolder DownloadFolder;
        private int DownloadItem;
        private bool DownloadNotification;

        private IDownloadOptionsService DownloadOptionsService { get; } = IOCHelper.GetService<IDownloadOptionsService>();

        private INavigationService NavigationService { get; } = IOCHelper.GetService<INavigationService>();

        private bool _isSelectMode = false;

        public bool IsSelectMode
        {
            get { return _isSelectMode; }

            set { SetProperty(ref _isSelectMode, value); }
        }

        private bool _isDownloadEmpty = true;

        public bool IsDownloadEmpty
        {
            get { return _isDownloadEmpty; }

            set { SetProperty(ref _isDownloadEmpty, value); }
        }

        public ObservableCollection<DownloadModel> DownloadDataList { get; set; } = new ObservableCollection<DownloadModel>();

        public IAsyncRelayCommand LoadCommand { get; set; }

        public IAsyncRelayCommand DownloadOptionsCommand { get; set; }

        public IAsyncRelayCommand OpenFolderCommand { get; set; }

        public IAsyncRelayCommand ContinueAllCommand { get; set; }

        public IAsyncRelayCommand PauseAllCommand { get; set; }

        public IAsyncRelayCommand SelectCommand { get; set; }

        public IAsyncRelayCommand SelectAllCommand { get; set; }

        public IAsyncRelayCommand SelectNoneCommand { get; set; }

        public IAsyncRelayCommand DeleteCommand { get; set; }

        public IAsyncRelayCommand DeleteWithFileCommand { get; set; }

        public IAsyncRelayCommand CancelCommand { get; set; }

        public IAsyncRelayCommand ContinueDownloadCommand { get; set; }

        public IAsyncRelayCommand OpenItemFolderCommand { get; set; }

        public IAsyncRelayCommand PauseDownloadCommand { get; set; }

        public IAsyncRelayCommand DeleteTaskCommand { get; set; }

        public DownloadViewModel()
        {
            DownloadFolder = DownloadOptionsService.DownloadFolder;
            DownloadItem = DownloadOptionsService.DownloadItem;
            DownloadNotification = DownloadOptionsService.DownloadNotification;

            // 下载记录数据列表初始化，从数据库中存储的列表中加载
            LoadCommand = new AsyncRelayCommand(GetDownloadDataListAsync);

            DownloadOptionsCommand = new AsyncRelayCommand(async () =>
            {
                NavigationService.NavigateTo(typeof(SettingsViewModel).FullName, null, new DrillInNavigationTransitionInfo());
                await Task.CompletedTask;
            });

            OpenFolderCommand = new AsyncRelayCommand(async () =>
            {
                await DownloadOptionsService.OpenFolderAsync(DownloadFolder);
            });

            SelectCommand = new AsyncRelayCommand(async () =>
            {
                await SelectNoneAsync();
                IsSelectMode = true;
            });

            SelectAllCommand = new AsyncRelayCommand(async () =>
            {
                foreach (var item in DownloadDataList) item.IsSelected = true;
                await Task.CompletedTask;
            });

            SelectNoneCommand = new AsyncRelayCommand(SelectNoneAsync);

            DeleteCommand = new AsyncRelayCommand(DeleteAsync);

            CancelCommand = new AsyncRelayCommand(async () =>
            {
                IsSelectMode = false;
                await Task.CompletedTask;
            });

            OpenItemFolderCommand = new AsyncRelayCommand<string>(async (param) =>
            {
                await DownloadOptionsService.OpenFolderAsync(await StorageFolder.GetFolderFromPathAsync(param));
            });

            TestListView();
        }

        private async Task DeleteAsync()
        {
            List<DownloadModel> SelectedDownloadDataList = DownloadDataList.Where(item => item.IsSelected == true).ToList();

            // 没有选中任何内容时显示空提示对话框
            if (SelectedDownloadDataList.Count == 0)
            {
                await ShowSelectEmptyPromptDialogAsync();
                return;
            }

            // 删除时显示删除确认对话框
            ContentDialogResult result = await ShowDeletePromptDialogAsync();

            if (result == ContentDialogResult.Primary)
            {
                IsSelectMode = false;

                await GetDownloadDataListAsync();
            }
        }

        /// <summary>
        /// 从数据库中加载数据
        /// </summary>
        private async Task GetDownloadDataListAsync()
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// 点击全部不选按钮时，让复选框的下载记录全部不选
        /// </summary>
        private async Task SelectNoneAsync()
        {
            foreach (var item in DownloadDataList) item.IsSelected = false;
            await Task.CompletedTask;
        }

        /// <summary>
        /// 选中内容为空时，显示提示对话框
        /// </summary>
        private async Task ShowSelectEmptyPromptDialogAsync()
        {
            SelectEmptyPromptDialog dialog = new SelectEmptyPromptDialog();
            dialog.XamlRoot = App.MainWindow.Content.XamlRoot;
            await dialog.ShowAsync();
        }

        /// <summary>
        /// 删除选中的条目时，显示删除提示对话框
        /// </summary>
        private async Task<ContentDialogResult> ShowDeletePromptDialogAsync()
        {
            DeletePromptDialog dialog = new DeletePromptDialog();
            dialog.XamlRoot = App.MainWindow.Content.XamlRoot;
            return await dialog.ShowAsync();
        }

        // 测试下载列表
        private void TestListView()
        {
            IsDownloadEmpty = false;

            DownloadDataList.Add(new DownloadModel()
            {
                IsSelected = false,
                CreateTimeStamp = 01,
                DownloadKey = "DownloadKey",
                FileName = "00.AppxBundle",
                FileLink = "https://01.url",
                FilePath = ApplicationData.Current.LocalCacheFolder.Path,
                FileSHA1 = "0123456789",
                FileSize = "100MB",
                DownloadFlag = 0,
                DownloadedFinishedSize = 30,
                DownloadProgress = 10,
                DownloadSpeed = 3
            });

            DownloadDataList.Add(new DownloadModel()
            {
                IsSelected = false,
                CreateTimeStamp = 01,
                DownloadKey = "DownloadKey",
                FileName = "01.AppxBundle",
                FileLink = "https://01.url",
                FilePath = ApplicationData.Current.LocalCacheFolder.Path,
                FileSHA1 = "0123456789",
                FileSize = "200MB",
                DownloadFlag = 1,
                DownloadedFinishedSize = 120,
                DownloadProgress = 60,
                DownloadSpeed = 3
            });

            DownloadDataList.Add(new DownloadModel()
            {
                IsSelected = false,
                CreateTimeStamp = 01,
                DownloadKey = "DownloadKey",
                FileName = "02.AppxBundle",
                FileLink = "https://01.url",
                FilePath = ApplicationData.Current.LocalCacheFolder.Path,
                FileSHA1 = "0123456789",
                FileSize = "300MB",
                DownloadFlag = 2,
                DownloadedFinishedSize = 150,
                DownloadProgress = 50,
                DownloadSpeed = 1
            });

            DownloadDataList.Add(new DownloadModel()
            {
                IsSelected = false,
                CreateTimeStamp = 01,
                DownloadKey = "DownloadKey",
                FileName = "03.AppxBundle",
                FileLink = "https://01.url",
                FilePath = ApplicationData.Current.LocalCacheFolder.Path,
                FileSHA1 = "0123456789",
                FileSize = "400MB",
                DownloadFlag = 3,
                DownloadedFinishedSize = 240,
                DownloadProgress = 60,
                DownloadSpeed = 2
            });
        }
    }
}
