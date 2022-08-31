using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Helpers;
using GetStoreApp.Messages;
using GetStoreApp.Models;
using GetStoreApp.UI.Dialogs;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Controls.Download
{
    public class DownloadingViewModel : ObservableRecipient
    {
        // 临界区资源访问互斥锁
        private readonly object DownloadingDataListLock = new object();

        private IDownloadSchedulerService DownloadSchedulerService { get; } = IOCHelper.GetService<IDownloadSchedulerService>();

        private IDownloadOptionsService DownloadOptionsService { get; } = IOCHelper.GetService<IDownloadOptionsService>();

        private DispatcherTimer DownloadingTimer { get; } = new DispatcherTimer();

        public ObservableCollection<DownloadModel> DownloadingDataList { get; } = new ObservableCollection<DownloadModel>();

        private bool _isSelectMode = false;

        public bool IsSelectMode
        {
            get { return _isSelectMode; }

            set { SetProperty(ref _isSelectMode, value); }
        }

        // 打开默认保存的文件夹
        public IAsyncRelayCommand OpenFolderCommand => new AsyncRelayCommand(async () =>
        {
            await DownloadOptionsService.OpenFolderAsync(DownloadOptionsService.DownloadFolder);
        });

        // 暂停下载全部任务
        public IAsyncRelayCommand PauseAllCommand => new AsyncRelayCommand(async () =>
        {
            List<DownloadModel> SelectedDownloadingDataList = DownloadingDataList.Where(item => item.IsSelected == true).ToList();

            foreach (DownloadModel downloadItem in SelectedDownloadingDataList)
            {
                bool PauseResult = await DownloadSchedulerService.PauseTaskAsync(downloadItem);

                if (PauseResult)
                {
                    lock (DownloadingDataListLock)
                    {
                        DownloadingDataList.Remove(DownloadingDataList.First(item => item.DownloadKey == downloadItem.DownloadKey));
                    }
                }
                else
                {
                    continue;
                }
            }
        });

        // 进入多选模式
        public IAsyncRelayCommand SelectCommand => new AsyncRelayCommand(async () =>
        {
            lock (DownloadingDataListLock)
            {
                foreach (DownloadModel downloadItem in DownloadingDataList)
                {
                    downloadItem.IsSelected = false;
                }
            }

            IsSelectMode = true;
            await Task.CompletedTask;
        });

        // 全部选择
        public IAsyncRelayCommand SelectAllCommand => new AsyncRelayCommand(async () =>
        {
            lock (DownloadingDataListLock)
            {
                foreach (DownloadModel downloadItem in DownloadingDataList)
                {
                    downloadItem.IsSelected = true;
                }
            }

            await Task.CompletedTask;
        });

        // 全部不选
        public IAsyncRelayCommand SelectNoneCommand => new AsyncRelayCommand(async () =>
        {
            lock (DownloadingDataListLock)
            {
                foreach (DownloadModel downloadItem in DownloadingDataList)
                {
                    downloadItem.IsSelected = false;
                }
            }

            await Task.CompletedTask;
        });

        // 删除选中的任务
        public IAsyncRelayCommand DeleteSelectedCommand => new AsyncRelayCommand(async () =>
        {
            List<DownloadModel> SelectedDownloadingDataList = DownloadingDataList.Where(item => item.IsSelected == true).ToList();

            // 没有选中任何内容时显示空提示对话框
            if (SelectedDownloadingDataList.Count == 0)
            {
                await new SelectEmptyPromptDialog().ShowAsync();
                return;
            }

            IsSelectMode = false;

            foreach (DownloadModel downloadItem in SelectedDownloadingDataList)
            {
                bool DeleteResult = await DownloadSchedulerService.DeleteTaskAsync(downloadItem);

                if (DeleteResult)
                {
                    lock (DownloadingDataListLock)
                    {
                        DownloadingDataList.Remove(DownloadingDataList.First(item => item.DownloadKey == downloadItem.DownloadKey));
                    }
                }
                else
                {
                    continue;
                }
            }
        });

        // 退出多选模式
        public IAsyncRelayCommand CancelCommand => new AsyncRelayCommand(async () =>
        {
            IsSelectMode = false;
            await Task.CompletedTask;
        });

        // 暂停下载当前任务
        public IAsyncRelayCommand PauseCommand => new AsyncRelayCommand<DownloadModel>(async (param) =>
        {
            bool PauseResult = await DownloadSchedulerService.PauseTaskAsync(param);

            if (PauseResult)
            {
                lock (DownloadingDataListLock)
                {
                    DownloadingDataList.Remove(DownloadingDataList.First(item => item.DownloadKey == param.DownloadKey));
                }
            }
        });

        // 删除当前任务
        public IAsyncRelayCommand DeleteCommand => new AsyncRelayCommand<DownloadModel>(async (param) =>
        {
            bool DeleteResult = await DownloadSchedulerService.DeleteTaskAsync(param);

            if (DeleteResult)
            {
                lock (DownloadingDataListLock)
                {
                    DownloadingDataList.Remove(DownloadingDataList.First(item => item.DownloadKey == param.DownloadKey));
                }
            }
        });

        public DownloadingViewModel()
        {
            // Tick 超过计时器间隔时发生
            DownloadingTimer.Tick += DownloadInfoTimerTick;
            // Interval 获取或设置计时器刻度之间的时间段
            DownloadingTimer.Interval = new TimeSpan(0, 0, 1);

            WeakReferenceMessenger.Default.Register<DownloadingViewModel, PivotSelectionMessage>(this, async (downloadingViewModel, pivotSelectionMessage) =>
            {
                // 切换到下载中页面时，开启监控。并更新当前页面的数据
                if (pivotSelectionMessage.Value == 0)
                {
                    lock (DownloadingDataListLock)
                    {
                        DownloadingDataList.Clear();
                    }

                    List<DownloadModel> DownloadingList = DownloadSchedulerService.GetDownloadingList();
                    List<DownloadModel> WaitingList = DownloadSchedulerService.GetWaitingList();

                    lock (DownloadingDataListLock)
                    {
                        foreach (DownloadModel downloadItem in DownloadingList)
                        {
                            DownloadingDataList.Add(downloadItem);
                        }
                        foreach (DownloadModel downloadItem in WaitingList)
                        {
                            DownloadingDataList.Add(downloadItem);
                        }
                    }

                    DownloadingTimer.Start();
                }

                // 从下载页面离开时，关闭所有事件。并注销所有消息服务
                else if (pivotSelectionMessage.Value == -1)
                {
                    if (DownloadingTimer.IsEnabled)
                    {
                        DownloadingTimer.Stop();
                    }
                    DownloadingTimer.Tick -= DownloadInfoTimerTick;
                    Messenger.UnregisterAll(this);
                }

                // 切换到其他页面时，关闭监控
                else
                {
                    DownloadingTimer.Stop();
                }

                await Task.CompletedTask;
            });
        }

        /// <summary>
        /// 计时器：获取正在下载中的文件信息
        /// </summary>
        private void DownloadInfoTimerTick(object sender, object e)
        {
        }
    }
}
