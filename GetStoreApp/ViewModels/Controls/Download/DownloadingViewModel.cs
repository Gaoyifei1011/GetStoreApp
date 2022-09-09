using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Helpers;
using GetStoreApp.Messages;
using GetStoreApp.Models.Download;
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
            List<DownloadingModel> SelectedDownloadingDataList = DownloadingDataList.Where(item => item.IsSelected == true).ToList();

            foreach (DownloadingModel downloadingItem in SelectedDownloadingDataList)
            {
                bool PauseResult = await DownloadSchedulerService.PauseTaskAsync(new BackgroundModel
                {
                    DownloadKey = downloadingItem.DownloadKey,
                    FileName = downloadingItem.FileName,
                    FileLink = downloadingItem.FileLink,
                    FilePath = downloadingItem.FilePath,
                    FileSHA1 = downloadingItem.FileSHA1,
                    TotalSize = downloadingItem.TotalSize,
                    DownloadFlag = downloadingItem.DownloadFlag
                });

                if (PauseResult)
                {
                    lock (DownloadingDataListLock)
                    {
                        DownloadingDataList.Remove(DownloadingDataList.First(item => item.DownloadKey == downloadingItem.DownloadKey));
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
                foreach (DownloadingModel downloadingItem in DownloadingDataList)
                {
                    downloadingItem.IsSelected = false;
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
                foreach (DownloadingModel downloadingItem in DownloadingDataList)
                {
                    downloadingItem.IsSelected = true;
                }
            }

            await Task.CompletedTask;
        });

        // 全部不选
        public IAsyncRelayCommand SelectNoneCommand => new AsyncRelayCommand(async () =>
        {
            lock (DownloadingDataListLock)
            {
                foreach (DownloadingModel downloadingItem in DownloadingDataList)
                {
                    downloadingItem.IsSelected = false;
                }
            }

            await Task.CompletedTask;
        });

        // 删除选中的任务
        public IAsyncRelayCommand DeleteSelectedCommand => new AsyncRelayCommand(async () =>
        {
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
                bool DeleteResult = await DownloadSchedulerService.DeleteTaskAsync(new BackgroundModel
                {
                    DownloadKey = downloadingItem.DownloadKey,
                    FileName = downloadingItem.FileName,
                    FileLink = downloadingItem.FileLink,
                    FilePath = downloadingItem.FilePath,
                    FileSHA1 = downloadingItem.FileSHA1,
                    TotalSize = downloadingItem.TotalSize,
                    DownloadFlag = downloadingItem.DownloadFlag
                });

                if (DeleteResult)
                {
                    lock (DownloadingDataListLock)
                    {
                        DownloadingDataList.Remove(DownloadingDataList.First(item => item.DownloadKey == downloadingItem.DownloadKey));
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
        public IAsyncRelayCommand PauseCommand => new AsyncRelayCommand<DownloadingModel>(async (param) =>
        {
            bool PauseResult = await DownloadSchedulerService.PauseTaskAsync(new BackgroundModel
            {
                DownloadKey = param.DownloadKey,
                FileName = param.FileName,
                FileLink = param.FileLink,
                FilePath = param.FilePath,
                FileSHA1 = param.FileSHA1,
                TotalSize = param.TotalSize,
                DownloadFlag = param.DownloadFlag
            });

            if (PauseResult)
            {
                lock (DownloadingDataListLock)
                {
                    DownloadingDataList.Remove(DownloadingDataList.First(item => item.DownloadKey == param.DownloadKey));
                }
            }
        });

        // 删除当前任务
        public IAsyncRelayCommand DeleteCommand => new AsyncRelayCommand<DownloadingModel>(async (param) =>
        {
            bool DeleteResult = await DownloadSchedulerService.DeleteTaskAsync(new BackgroundModel
            {
                DownloadKey = param.DownloadKey,
                FileName = param.FileName,
                FileLink = param.FileLink,
                FilePath = param.FilePath,
                FileSHA1 = param.FileSHA1,
                TotalSize = param.TotalSize,
                DownloadFlag = param.DownloadFlag
            });

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

            // 订阅事件
            DownloadSchedulerService.DownloadingList.ItemsChanged += DownloadingListItemsChanged;
            DownloadSchedulerService.WaitingList.ItemsChanged += WaitingListItemsChanged;

            WeakReferenceMessenger.Default.Register<DownloadingViewModel, PivotSelectionMessage>(this, async (downloadingViewModel, pivotSelectionMessage) =>
            {
                // 切换到下载中页面时，开启监控。并更新当前页面的数据
                if (pivotSelectionMessage.Value == 0)
                {
                    lock (DownloadingDataListLock)
                    {
                        DownloadingDataList.Clear();
                    }

                    lock (DownloadingDataListLock)
                    {
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
        /// 订阅事件，下载中列表内容发生改变时通知UI更改
        /// </summary>
        private void WaitingListItemsChanged(object sender, Extensions.Event.ItemsChangedEventArgs<BackgroundModel> args)
        {
            // 等待列表添加项目时，更新UI
            if (args.AddedItems.Count > 0)
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
            }

            // 等待列表删除项目时，更新UI
            if (args.RemovedItems.Count > 0)
            {
                foreach (BackgroundModel downloadItem in args.RemovedItems)
                {
                    DownloadingDataList.Remove(DownloadingDataList.First(item => item.DownloadKey == downloadItem.DownloadKey));
                }
            }
        }

        /// <summary>
        /// 订阅事件，等待列表内容发生改变时通知UI更改
        /// </summary>
        private void DownloadingListItemsChanged(object sender, Extensions.Event.ItemsChangedEventArgs<BackgroundModel> args)
        {
            // 下载中列表添加项目时，更新UI
            if (args.AddedItems.Count > 0)
            {
                lock (DownloadingDataListLock)
                {
                    foreach (BackgroundModel downloadItem in args.AddedItems)
                    {
                        DownloadingDataList.Insert(DownloadingDataList.Count(item => item.DownloadFlag == 3), new DownloadingModel
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
                }
            }

            // 下载中列表删除项目时，更新UI
            if (args.RemovedItems.Count > 0)
            {
                lock (DownloadingDataListLock)
                {
                    foreach (BackgroundModel item in args.RemovedItems)
                    {
                        DownloadingDataList.Remove(DownloadingDataList.First(item => item.DownloadKey == item.DownloadKey));
                    }
                }
            }
        }

        /// <summary>
        /// 计时器：获取正在下载中文件的下载进度
        /// </summary>
        private void DownloadInfoTimerTick(object sender, object e)
        {
            List<BackgroundModel> DownloadingList = DownloadSchedulerService.DownloadingList;

            lock (DownloadingDataListLock)
            {
                for (int i = 0; i < DownloadingList.Count; i++)
                {
                    if (DownloadingDataList[i].DownloadKey == DownloadingList[i].DownloadKey)
                    {
                        DownloadingDataList[i].FinishedSize = DownloadingList[i].FinishedSize;
                    }
                }
            }
        }
    }
}
