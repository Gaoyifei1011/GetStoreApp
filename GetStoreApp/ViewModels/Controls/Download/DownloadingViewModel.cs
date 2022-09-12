using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI;
using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Helpers;
using GetStoreApp.Messages;
using GetStoreApp.Models.Download;
using GetStoreApp.UI.Dialogs;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
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
        private readonly object IsUpdatingNowLock = new object();

        // 标志信息是否在更新中
        private bool IsUpdatingNow = false;

        private IDownloadSchedulerService DownloadSchedulerService { get; } = IOCHelper.GetService<IDownloadSchedulerService>();

        private IDownloadOptionsService DownloadOptionsService { get; } = IOCHelper.GetService<IDownloadOptionsService>();

        private DispatcherTimer DownloadingTimer { get; } = new DispatcherTimer();

        private DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        public ObservableCollection<DownloadingModel> DownloadingDataList { get; } = new ObservableCollection<DownloadingModel>();

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
            // 有信息在更新时，等待操作
            while (IsUpdatingNow)
            {
                await Task.Delay(300);
                continue;
            }

            // 当有信息处于更新状态中时，暂停其他操作
            lock (IsUpdatingNowLock)
            {
                IsUpdatingNow = true;
            }

            List<DownloadingModel> SelectedDownloadingDataList = DownloadingDataList.Where(item => item.IsSelected == true).ToList();

            foreach (DownloadingModel downloadingItem in SelectedDownloadingDataList)
            {
                bool PauseResult = await DownloadSchedulerService.PauseTaskAsync(downloadingItem.DownloadKey, downloadingItem.GID, downloadingItem.DownloadFlag);
            }

            // 信息更新完毕时，允许其他操作开始执行
            lock (IsUpdatingNowLock)
            {
                IsUpdatingNow = false;
            }
        });

        // 进入多选模式
        public IAsyncRelayCommand SelectCommand => new AsyncRelayCommand(async () =>
        {
            foreach (DownloadingModel downloadingItem in DownloadingDataList)
            {
                downloadingItem.IsSelected = false;
            }

            IsSelectMode = true;
            await Task.CompletedTask;
        });

        // 全部选择
        public IAsyncRelayCommand SelectAllCommand => new AsyncRelayCommand(async () =>
        {
            foreach (DownloadingModel downloadingItem in DownloadingDataList)
            {
                downloadingItem.IsSelected = true;
            }

            await Task.CompletedTask;
        });

        // 全部不选
        public IAsyncRelayCommand SelectNoneCommand => new AsyncRelayCommand(async () =>
        {
            foreach (DownloadingModel downloadingItem in DownloadingDataList)
            {
                downloadingItem.IsSelected = false;
            }

            await Task.CompletedTask;
        });

        // 删除选中的任务
        public IAsyncRelayCommand DeleteSelectedCommand => new AsyncRelayCommand(async () =>
        {
            // 有信息在更新时，等待操作
            while (IsUpdatingNow)
            {
                await Task.Delay(300);
                continue;
            }

            // 当有信息处于更新状态中时，暂停其他操作
            lock (IsUpdatingNowLock)
            {
                IsUpdatingNow = true;
            }

            List<DownloadingModel> SelectedDownloadingDataList = DownloadingDataList.Where(item => item.IsSelected == true).ToList();

            // 没有选中任何内容时显示空提示对话框
            if (SelectedDownloadingDataList.Count == 0)
            {
                await new SelectEmptyPromptDialog().ShowAsync();
                return;
            }

            IsSelectMode = false;

            foreach (DownloadingModel downloadingItem in SelectedDownloadingDataList)
            {
                bool DeleteResult = await DownloadSchedulerService.DeleteTaskAsync(downloadingItem.DownloadKey, downloadingItem.GID, downloadingItem.DownloadFlag);
            }

            // 信息更新完毕时，允许其他操作开始执行
            lock (IsUpdatingNowLock)
            {
                IsUpdatingNow = false;
            }
        });

        // 退出多选模式
        public IAsyncRelayCommand CancelCommand => new AsyncRelayCommand(async () =>
        {
            IsSelectMode = false;
            await Task.CompletedTask;
        });

        // 在多选模式下点击项目选择相应的条目
        public IAsyncRelayCommand ItemClickCommand => new AsyncRelayCommand<ItemClickEventArgs>(async (param) =>
        {
            DownloadingModel downloadingItem = (DownloadingModel)param.ClickedItem;
            int ClickedIndex = DownloadingDataList.IndexOf(downloadingItem);

            DownloadingDataList[ClickedIndex].IsSelected = !DownloadingDataList[ClickedIndex].IsSelected;

            await Task.CompletedTask;
        });

        // 暂停下载当前任务
        public IAsyncRelayCommand PauseCommand => new AsyncRelayCommand<DownloadingModel>(async (param) =>
        {
            // 有信息在更新时，等待操作
            while (IsUpdatingNow)
            {
                await Task.Delay(300);
                continue;
            }

            // 当有信息处于更新状态中时，暂停其他操作
            lock (IsUpdatingNowLock)
            {
                IsUpdatingNow = true;
            }

            bool PauseResult = await DownloadSchedulerService.PauseTaskAsync(param.DownloadKey, param.GID, param.DownloadFlag);

            // 信息更新完毕时，允许其他操作开始执行
            lock (IsUpdatingNowLock)
            {
                IsUpdatingNow = false;
            }
        });

        // 删除当前任务
        public IAsyncRelayCommand DeleteCommand => new AsyncRelayCommand<DownloadingModel>(async (param) =>
        {
            // 有信息在更新时，等待操作
            while (IsUpdatingNow)
            {
                await Task.Delay(300);
                continue;
            }

            // 当有信息处于更新状态中时，暂停其他操作
            lock (IsUpdatingNowLock)
            {
                IsUpdatingNow = true;
            }

            bool DeleteResult = await DownloadSchedulerService.DeleteTaskAsync(param.DownloadKey, param.GID, param.DownloadFlag);

            // 信息更新完毕时，允许其他操作开始执行
            lock (IsUpdatingNowLock)
            {
                IsUpdatingNow = false;
            }
        });

        public DownloadingViewModel()
        {
            // Tick 超过计时器间隔时发生
            DownloadingTimer.Tick += DownloadInfoTimerTick;
            // Interval 获取或设置计时器刻度之间的时间段
            DownloadingTimer.Interval = new TimeSpan(0, 0, 1);

            // 订阅事件
            DownloadSchedulerService.DownloadingList.ItemsChanged += DownloadingListItemsChanged;
            DownloadSchedulerService.WaitingList.ItemsChanged += WaitingListItemsChanged;

            WeakReferenceMessenger.Default.Register<DownloadingViewModel, PivotSelectionMessage>(this, async (downloadingViewModel, pivotSelectionMessage) =>
            {

                // 切换到下载中页面时，开启监控。并更新当前页面的数据
                if (pivotSelectionMessage.Value == 0)
                {
                    lock (IsUpdatingNowLock)
                    {
                        IsUpdatingNow = true;
                    }

                    DownloadingDataList.Clear();

                    foreach (BackgroundModel item in DownloadSchedulerService.DownloadingList)
                    {
                        DownloadingDataList.Add(new DownloadingModel
                        {
                            DownloadKey = item.DownloadKey,
                            FileName = item.FileName,
                            FileLink = item.FileLink,
                            FilePath = item.FilePath,
                            FileSHA1 = item.FileSHA1,
                            TotalSize = item.TotalSize,
                            DownloadFlag = item.DownloadFlag
                        });
                    }
                    foreach (BackgroundModel downloadItem in DownloadSchedulerService.WaitingList)
                    {
                        DownloadingDataList.Add(new DownloadingModel
                        {
                            DownloadKey = downloadItem.DownloadKey,
                            FileName = downloadItem.FileName,
                            FileLink = downloadItem.FileLink,
                            FilePath = downloadItem.FilePath,
                            FileSHA1 = downloadItem.FileSHA1,
                            TotalSize = downloadItem.TotalSize,
                            DownloadFlag = downloadItem.DownloadFlag
                        });
                    }

                    lock (IsUpdatingNowLock)
                    {
                        IsUpdatingNow = false;
                    }

                    DownloadingTimer.Start();
                }

                // 从下载页面离开时，取消订阅所有事件。并注销所有消息服务
                else if (pivotSelectionMessage.Value == -1)
                {
                    if (DownloadingTimer.IsEnabled)
                    {
                        DownloadingTimer.Stop();
                    }

                    // 取消订阅所有事件
                    DownloadingTimer.Tick -= DownloadInfoTimerTick;

                    DownloadSchedulerService.DownloadingList.ItemsChanged -= DownloadingListItemsChanged;
                    DownloadSchedulerService.WaitingList.ItemsChanged -= WaitingListItemsChanged;

                    // 关闭消息服务
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
        /// 计时器：获取正在下载中文件的下载进度
        /// </summary>
        private void DownloadInfoTimerTick(object sender, object e)
        {
            // 有信息在更新时，不再操作，等待下一秒尝试更新内容
            if (IsUpdatingNow)
            {
                return;
            }

            // 当有信息处于更新状态中时，暂停其他操作
            lock (IsUpdatingNowLock)
            {
                IsUpdatingNow = true;
            }

            List<BackgroundModel> DownloadingList = DownloadSchedulerService.DownloadingList;

            foreach (BackgroundModel backgroundItem in DownloadingList)
            {
                int index = DownloadingDataList.IndexOf(DownloadingDataList.First(item => item.DownloadKey == backgroundItem.DownloadKey));
                DownloadingDataList[index].GID = backgroundItem.GID;
                DownloadingDataList[index].FinishedSize = backgroundItem.FinishedSize;
                DownloadingDataList[index].TotalSize = backgroundItem.TotalSize;
                DownloadingDataList[index].CurrentSpeed = backgroundItem.CurrentSpeed;
            }

            // 信息更新完毕时，允许其他操作开始执行
            lock (IsUpdatingNowLock)
            {
                IsUpdatingNow = false;
            }
        }

        /// <summary>
        /// 订阅事件，下载中列表内容发生改变时通知UI更改
        /// </summary>
        private async void DownloadingListItemsChanged(object sender, Extensions.Event.ItemsChangedEventArgs<BackgroundModel> args)
        {
            // 有信息在更新时，等待操作
            while (IsUpdatingNow)
            {
                await Task.Delay(300);
                continue;
            }

            // 当有信息处于更新状态中时，暂停其他操作
            lock (IsUpdatingNowLock)
            {
                IsUpdatingNow = true;
            }

            // 下载中列表添加项目时，更新UI
            if (args.AddedItems.Count > 0)
            {
                await dispatcherQueue.EnqueueAsync(() =>
                {
                    foreach (BackgroundModel downloadItem in args.AddedItems)
                    {
                        DownloadingDataList.Insert(DownloadingDataList.Count(item => item.DownloadFlag == 3),
                            new DownloadingModel
                            {
                                DownloadKey = downloadItem.DownloadKey,
                                FileName = downloadItem.FileName,
                                FileLink = downloadItem.FileLink,
                                FilePath = downloadItem.FilePath,
                                FileSHA1 = downloadItem.FileSHA1,
                                TotalSize = downloadItem.TotalSize,
                                DownloadFlag = downloadItem.DownloadFlag
                            });
                    }
                });
            }

            // 下载中列表删除项目时，更新UI
            if (args.RemovedItems.Count > 0)
            {
                await dispatcherQueue.EnqueueAsync(() =>
                {
                    foreach (BackgroundModel backgroundItem in args.RemovedItems)
                    {
                        DownloadingDataList.Remove(DownloadingDataList.First(item => item.DownloadKey == backgroundItem.DownloadKey));
                    }
                });
            }

            // 信息更新完毕时，允许其他操作开始执行
            lock (IsUpdatingNowLock)
            {
                IsUpdatingNow = false;
            }
        }

        /// <summary>
        /// 订阅事件，等待列表内容发生改变时通知UI更改
        /// </summary>
        private async void WaitingListItemsChanged(object sender, Extensions.Event.ItemsChangedEventArgs<BackgroundModel> args)
        {
            // 有信息在更新时，等待操作
            while (IsUpdatingNow)
            {
                await Task.Delay(300);
                continue;
            }

            // 当有信息处于更新状态中时，暂停其他操作
            lock (IsUpdatingNowLock)
            {
                IsUpdatingNow = true;
            }

            // 等待列表添加项目时，更新UI
            if (args.AddedItems.Count > 0)
            {
                await dispatcherQueue.EnqueueAsync(() =>
                {
                    foreach (BackgroundModel item in args.AddedItems)
                    {
                        DownloadingDataList.Add(new DownloadingModel
                        {
                            DownloadKey = item.DownloadKey,
                            FileName = item.FileName,
                            FileLink = item.FileLink,
                            FilePath = item.FilePath,
                            FileSHA1 = item.FileSHA1,
                            TotalSize = item.TotalSize,
                            DownloadFlag = item.DownloadFlag
                        });
                    }
                });
            }

            // 等待列表删除项目时，更新UI
            if (args.RemovedItems.Count > 0)
            {
                await dispatcherQueue.EnqueueAsync(() =>
                {
                    foreach (BackgroundModel backgroundItem in args.RemovedItems)
                    {
                        DownloadingDataList.Remove(DownloadingDataList.First(item => item.DownloadKey == backgroundItem.DownloadKey));
                    }
                });
            }

            // 信息更新完毕时，允许其他操作开始执行
            lock (IsUpdatingNowLock)
            {
                IsUpdatingNow = false;
            }
        }
    }
}
