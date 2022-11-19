using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.DataType.Events;
using GetStoreApp.Extensions.Messaging;
using GetStoreApp.Models.Controls.Download;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.UI.Dialogs.Common;
using GetStoreApp.ViewModels.Base;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Controls.Download
{
    public sealed class DownloadingViewModel : ViewModelBase
    {
        // 临界区资源访问互斥锁
        private readonly object IsUpdatingNowLock = new object();

        // 标志信息是否在更新中
        private bool IsUpdatingNow = false;

        // 标志信息是否已经初始化完成
        private bool IsInitializeFinished = false;

        private DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        private DispatcherTimer DownloadingTimer { get; } = new DispatcherTimer();

        public ObservableCollection<DownloadingModel> DownloadingDataList { get; } = new ObservableCollection<DownloadingModel>();

        private bool _isSelectMode = false;

        public bool IsSelectMode
        {
            get { return _isSelectMode; }

            set
            {
                _isSelectMode = value;
                OnPropertyChanged();
            }
        }

        // 打开默认保存的文件夹
        public IRelayCommand OpenFolderCommand => new RelayCommand(async () =>
        {
            await DownloadOptionsService.OpenFolderAsync(DownloadOptionsService.DownloadFolder);
        });

        // 暂停下载全部任务
        public IRelayCommand PauseAllCommand => new RelayCommand(async () =>
        {
            // 有信息在更新时，等待操作
            while (IsUpdatingNow && IsInitializeFinished)
            {
                await Task.Delay(300);
                continue;
            }

            // 当有信息处于更新状态中时，暂停其他操作
            lock (IsUpdatingNowLock)
            {
                IsUpdatingNow = true;
            }

            // 暂停下载所有任务
            await DownloadSchedulerService.PauseAllTaskAsync();

            // 信息更新完毕时，允许其他操作开始执行
            lock (IsUpdatingNowLock)
            {
                IsUpdatingNow = false;
            }
        });

        // 进入多选模式
        public IRelayCommand SelectCommand => new RelayCommand(async () =>
        {
            while (IsUpdatingNow && IsInitializeFinished)
            {
                await Task.Delay(300);
                continue;
            }

            lock (IsUpdatingNowLock)
            {
                IsUpdatingNow = true;
            }

            foreach (DownloadingModel downloadingItem in DownloadingDataList)
            {
                downloadingItem.IsSelected = false;
            }

            IsSelectMode = true;

            lock (IsUpdatingNowLock)
            {
                IsUpdatingNow = false;
            }
        });

        // 全部选择
        public IRelayCommand SelectAllCommand => new RelayCommand(async () =>
        {
            while (IsUpdatingNow && IsInitializeFinished)
            {
                await Task.Delay(300);
                continue;
            }

            lock (IsUpdatingNowLock)
            {
                IsUpdatingNow = true;
            }

            foreach (DownloadingModel downloadingItem in DownloadingDataList)
            {
                downloadingItem.IsSelected = true;
            }

            lock (IsUpdatingNowLock)
            {
                IsUpdatingNow = false;
            }
        });

        // 全部不选
        public IRelayCommand SelectNoneCommand => new RelayCommand(async () =>
        {
            while (IsUpdatingNow && IsInitializeFinished)
            {
                await Task.Delay(300);
                continue;
            }

            lock (IsUpdatingNowLock)
            {
                IsUpdatingNow = true;
            }

            foreach (DownloadingModel downloadingItem in DownloadingDataList)
            {
                downloadingItem.IsSelected = false;
            }

            lock (IsUpdatingNowLock)
            {
                IsUpdatingNow = false;
            }
        });

        // 删除选中的任务
        public IRelayCommand DeleteSelectedCommand => new RelayCommand(async () =>
        {
            // 有信息在更新时，等待操作
            while (IsUpdatingNow && IsInitializeFinished)
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

                if (DeleteResult)
                {
                    // 删除文件
                    string tempFilePath = downloadingItem.FilePath;
                    string tempFileAria2Path = string.Format("{0}.{1}", downloadingItem.FilePath, "aria2");

                    if (File.Exists(tempFilePath))
                    {
                        File.Delete(tempFilePath);
                    }

                    if (File.Exists(tempFileAria2Path))
                    {
                        File.Delete(tempFileAria2Path);
                    }
                }
            }

            // 信息更新完毕时，允许其他操作开始执行
            lock (IsUpdatingNowLock)
            {
                IsUpdatingNow = false;
            }
        });

        // 退出多选模式
        public IRelayCommand CancelCommand => new RelayCommand(() =>
        {
            IsSelectMode = false;
        });

        // 暂停下载当前任务
        public IRelayCommand PauseCommand => new RelayCommand<DownloadingModel>(async (downloadingItem) =>
        {
            // 有信息在更新时，等待操作
            while (IsUpdatingNow && IsInitializeFinished)
            {
                await Task.Delay(300);
                continue;
            }

            // 当有信息处于更新状态中时，暂停其他操作
            lock (IsUpdatingNowLock)
            {
                IsUpdatingNow = true;
            }

            bool PauseResult = await DownloadSchedulerService.PauseTaskAsync(downloadingItem.DownloadKey, downloadingItem.GID, downloadingItem.DownloadFlag);

            // 信息更新完毕时，允许其他操作开始执行
            lock (IsUpdatingNowLock)
            {
                IsUpdatingNow = false;
            }
        });

        // 删除当前任务
        public IRelayCommand DeleteCommand => new RelayCommand<DownloadingModel>(async (downloadingItem) =>
        {
            // 有信息在更新时，等待操作
            while (IsUpdatingNow && IsInitializeFinished)
            {
                await Task.Delay(300);
                continue;
            }

            // 当有信息处于更新状态中时，暂停其他操作
            lock (IsUpdatingNowLock)
            {
                IsUpdatingNow = true;
            }

            bool DeleteResult = await DownloadSchedulerService.DeleteTaskAsync(downloadingItem.DownloadKey, downloadingItem.GID, downloadingItem.DownloadFlag);

            if (DeleteResult)
            {
                // 删除文件
                string tempFilePath = downloadingItem.FilePath;
                string tempFileAria2Path = string.Format("{0}.{1}", downloadingItem.FilePath, "aria2");

                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }

                if (File.Exists(tempFileAria2Path))
                {
                    File.Delete(tempFileAria2Path);
                }
            }

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

            Messenger.Default.Register<int>(this, MessageToken.PivotSelection, async (pivotSelectionMessage) =>
            {
                // 切换到下载中页面时，开启监控。并更新当前页面的数据
                if (pivotSelectionMessage == 0)
                {
                    await GetDownloadingDataListAsync();

                    DownloadingTimer.Start();
                }

                // 从下载页面离开时，取消订阅所有事件。并注销所有消息服务
                else if (pivotSelectionMessage == -1)
                {
                    if (DownloadingTimer.IsEnabled)
                    {
                        DownloadingTimer.Stop();
                    }

                    // 取消订阅所有事件
                    DownloadingTimer.Tick -= DownloadInfoTimerTick;

                    DownloadSchedulerService.DownloadingList.ItemsChanged -= OnDownloadingListItemsChanged;
                    DownloadSchedulerService.WaitingList.ItemsChanged -= OnWaitingListItemsChanged;

                    // 关闭消息服务
                    Messenger.Default.Unregister(this);
                }

                // 切换到其他页面时，关闭监控
                else
                {
                    DownloadingTimer.Stop();
                }
            });

            // 订阅事件
            DownloadSchedulerService.DownloadingList.ItemsChanged += OnDownloadingListItemsChanged;
            DownloadSchedulerService.WaitingList.ItemsChanged += OnWaitingListItemsChanged;
        }

        /// <summary>
        /// 页面被卸载时，关闭消息服务
        /// </summary>
        public void OnUnloaded(object sender, RoutedEventArgs args)
        {
            Messenger.Default.Unregister(this);
        }

        /// <summary>
        /// 在多选模式下点击项目选择相应的条目
        /// </summary>
        public async void OnItemClick(object sender, ItemClickEventArgs args)
        {
            while (IsUpdatingNow && IsInitializeFinished)
            {
                await Task.Delay(300);
                continue;
            }

            lock (IsUpdatingNowLock)
            {
                IsUpdatingNow = true;
            }

            DownloadingModel downloadingItem = (DownloadingModel)args.ClickedItem;
            int ClickedIndex = DownloadingDataList.IndexOf(downloadingItem);

            DownloadingDataList[ClickedIndex].IsSelected = !DownloadingDataList[ClickedIndex].IsSelected;

            lock (IsUpdatingNowLock)
            {
                IsUpdatingNow = false;
            }
        }

        /// <summary>
        /// 从下载调度服务中获取正在下载和等待下载的数据
        /// </summary>
        private async Task GetDownloadingDataListAsync()
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
                IsInitializeFinished = true;
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// 计时器：获取正在下载中文件的下载进度
        /// </summary>
        private void DownloadInfoTimerTick(object sender, object e)
        {
            // 有信息在更新时，不再操作，等待下一秒尝试更新内容
            if (IsUpdatingNow && IsInitializeFinished)
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
                try
                {
                    int index = DownloadingDataList.IndexOf(DownloadingDataList.First(item => item.DownloadKey == backgroundItem.DownloadKey));
                    DownloadingDataList[index].GID = backgroundItem.GID;
                    DownloadingDataList[index].FinishedSize = backgroundItem.FinishedSize;
                    DownloadingDataList[index].TotalSize = backgroundItem.TotalSize;
                    DownloadingDataList[index].CurrentSpeed = backgroundItem.CurrentSpeed;
                }
                catch (Exception)
                {
                    continue;
                }
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
        private async void OnDownloadingListItemsChanged(object sender, ItemsChangedEventArgs<BackgroundModel> args)
        {
            // 有信息在更新时，等待操作
            while (IsUpdatingNow && IsInitializeFinished)
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
                dispatcherQueue.TryEnqueue(() =>
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
                dispatcherQueue.TryEnqueue(() =>
                {
                    foreach (BackgroundModel backgroundItem in args.RemovedItems)
                    {
                        try
                        {
                            DownloadingDataList.Remove(DownloadingDataList.First(item => item.DownloadKey == backgroundItem.DownloadKey && item.DownloadFlag == 3));
                        }
                        catch (Exception)
                        {
                            continue;
                        }
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
        private async void OnWaitingListItemsChanged(object sender, ItemsChangedEventArgs<BackgroundModel> args)
        {
            // 有信息在更新时，等待操作
            while (IsUpdatingNow && IsInitializeFinished)
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
                dispatcherQueue.TryEnqueue(() =>
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
                dispatcherQueue.TryEnqueue(() =>
                {
                    foreach (BackgroundModel backgroundItem in args.RemovedItems)
                    {
                        try
                        {
                            DownloadingDataList.Remove(DownloadingDataList.First(item => item.DownloadKey == backgroundItem.DownloadKey && item.DownloadFlag == 1));
                        }
                        catch (Exception)
                        {
                            continue;
                        }
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
