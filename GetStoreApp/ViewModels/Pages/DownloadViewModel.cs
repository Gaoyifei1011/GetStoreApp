using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.Contracts.ViewModels;
using GetStoreApp.Helpers;
using GetStoreApp.Models;
using GetStoreApp.UI.Dialogs;
using Microsoft.UI.Xaml;
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
    public class DownloadViewModel : ObservableRecipient, INavigationAware
    {
        private StorageFolder DownloadFolder { get; }

        private int DownloadItem { get; }

        private bool DownloadNotification { get; }

        private DispatcherTimer DownloadInfoTimer { get; } = new DispatcherTimer();

        private IAria2Service Aria2Service { get; } = IOCHelper.GetService<IAria2Service>();

        private IDownloadDataService DownloadDataService { get; } = IOCHelper.GetService<IDownloadDataService>();

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

        public ObservableCollection<DownloadModel> DownloadDataList { get; } = new ObservableCollection<DownloadModel>();

        public IAsyncRelayCommand DownloadOptionsCommand => new AsyncRelayCommand(async () =>
        {
            NavigationService.NavigateTo(typeof(SettingsViewModel).FullName, null, new DrillInNavigationTransitionInfo());
            await Task.CompletedTask;
        });

        public IAsyncRelayCommand OpenFolderCommand => new AsyncRelayCommand(async () =>
        {
            await DownloadOptionsService.OpenFolderAsync(DownloadFolder);
        });

        public IAsyncRelayCommand ContinueAllCommand { get; }

        public IAsyncRelayCommand PauseAllCommand { get; }

        public IAsyncRelayCommand SelectCommand => new AsyncRelayCommand(async () =>
        {
            await SelectNoneAsync();
            IsSelectMode = true;
        });

        public IAsyncRelayCommand SelectAllCommand => new AsyncRelayCommand(async () =>
        {
            foreach (DownloadModel downloadItem in DownloadDataList)
            {
                downloadItem.IsSelected = true;
            }
            await Task.CompletedTask;
        });

        public IAsyncRelayCommand SelectNoneCommand => new AsyncRelayCommand(SelectNoneAsync);

        public IAsyncRelayCommand DeleteCommand => new AsyncRelayCommand(DeleteAsync);

        public IAsyncRelayCommand DeleteWithFileCommand { get; }

        public IAsyncRelayCommand CancelCommand => new AsyncRelayCommand(async () =>
        {
            IsSelectMode = false;
            await Task.CompletedTask;
        });

        public IAsyncRelayCommand ContinueDownloadCommand { get; }

        public IAsyncRelayCommand OpenItemFolderCommand => new AsyncRelayCommand<string>(async (param) =>
        {
            await DownloadOptionsService.OpenFolderAsync(await StorageFolder.GetFolderFromPathAsync(param));
        });

        public IAsyncRelayCommand PauseDownloadCommand { get; }

        public IAsyncRelayCommand DeleteTaskCommand { get; }

        public DownloadViewModel()
        {
            DownloadFolder = DownloadOptionsService.DownloadFolder;
            DownloadItem = DownloadOptionsService.DownloadItem;
            DownloadNotification = DownloadOptionsService.DownloadNotification;
        }

        // 导航到下载记录页面时，下载记录数据列表初始化，从数据库中存储的列表中加载，并获取Aria2下载状态信息
        public async void OnNavigatedTo(object parameter)
        {
            // Tick 超过计时器间隔时发生
            DownloadInfoTimer.Tick += DownloadInfoTimerTick;
            // Interval 获取或设置计时器刻度之间的时间段
            DownloadInfoTimer.Interval = new TimeSpan(0, 0, 1);

            // 从数据库中获取所有下载记录
            await GetDownloadDataListAsync();

            DownloadInfoTimer.Start();
        }

        /// <summary>
        /// 计时器：获取正在下载中的文件信息
        /// </summary>
        private void DownloadInfoTimerTick(object sender, object e)
        {
        }

        public void OnNavigatedFrom()
        {
            DownloadInfoTimer.Stop();
            DownloadInfoTimer.Tick -= DownloadInfoTimerTick;
        }

        private async Task DeleteAsync()
        {
            List<DownloadModel> SelectedDownloadDataList = DownloadDataList.Where(item => item.IsSelected == true).ToList();

            List<DownloadModel> DownloadingList = SelectedDownloadDataList.Where(item => item.DownloadFlag == 1).ToList();

            // 没有选中任何内容时显示空提示对话框
            if (SelectedDownloadDataList.Count == 0)
            {
                await new SelectEmptyPromptDialog().ShowAsync();
                return;
            }

            // 删除时显示删除确认对话框
            ContentDialogResult result = await new DeletePromptDialog().ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                IsSelectMode = false;

                await DownloadDataService.DeleteDownloadDataAsync(SelectedDownloadDataList);

                // 如果有正在下载的服务，从下载列表中删除
                if (DownloadingList.Any())
                {
                    foreach (DownloadModel downloadItem in DownloadingList)
                    {
                        //Aria2Service.DeleteSelectedAsync();
                    }
                }
                await GetDownloadDataListAsync();
            }
        }

        /// <summary>
        /// 从数据库中加载数据
        /// </summary>
        private async Task GetDownloadDataListAsync()
        {
            Tuple<List<DownloadModel>, bool> DownloadData = await DownloadDataService.QueryAllDownloadDataAsync();

            List<DownloadModel> DownloadRawList = DownloadData.Item1;

            IsDownloadEmpty = DownloadData.Item2;

            try
            {
                ConvertRawListToDisplayList(ref DownloadRawList);
            }
            catch (Exception)
            {
                ConvertRawListToDisplayList(ref DownloadRawList);
            }
        }

        /// <summary>
        /// 将原始数据转换为在UI界面上呈现出来的数据
        /// </summary>
        private void ConvertRawListToDisplayList(ref List<DownloadModel> downloadRawList)
        {
            DownloadDataList.Clear();

            foreach (DownloadModel downloadRawData in downloadRawList)
            {
                downloadRawData.IsSelected = false;
                DownloadDataList.Add(downloadRawData);
            }
        }

        /// <summary>
        /// 点击全部不选按钮时，让复选框的下载记录全部不选
        /// </summary>
        private async Task SelectNoneAsync()
        {
            foreach (DownloadModel downloadItem in DownloadDataList)
            {
                downloadItem.IsSelected = false;
            }

            await Task.CompletedTask;
        }
    }
}
