using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.DataType.Events;
using GetStoreApp.Models.Controls.Download;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.Common;
using GetStoreApp.ViewModels.Base;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Controls.Download
{
    /// <summary>
    /// 下载页面：正在下载完成用户控件视图模型
    /// </summary>
    public sealed class DownloadingViewModel : ViewModelBase
    {
        // 临界区资源访问互斥锁
        private readonly object DownloadingNowLock = new object();

        // 标志信息是否在更新中
        private bool IsUpdatingNow = false;

        // 标志信息是否已经初始化完成
        private bool IsInitializeFinished = false;

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

        // 暂停下载当前任务
        public XamlUICommand PauseCommand { get; } = new XamlUICommand();

        // 删除当前任务
        public XamlUICommand DeleteCommand { get; } = new XamlUICommand();

        /// <summary>
        /// 打开默认保存的文件夹
        /// </summary>
        public async void OnOpenFolderClicked(object sender, RoutedEventArgs args)
        {
            await DownloadOptionsService.OpenFolderAsync(DownloadOptionsService.DownloadFolder);
        }

        /// <summary>
        /// 暂停下载全部任务
        /// </summary>
        public async void OnPauseAllClicked(object sender, RoutedEventArgs args)
        {
            // 有信息在更新时，等待操作
            while (IsUpdatingNow || !IsInitializeFinished) await Task.Delay(50);
            lock (DownloadingNowLock) IsUpdatingNow = true;

            // 暂停下载所有任务
            await DownloadSchedulerService.PauseAllTaskAsync();

            // 信息更新完毕时，允许其他操作开始执行
            lock (DownloadingNowLock) IsUpdatingNow = false;
        }

        /// <summary>
        /// 进入多选模式
        /// </summary>
        public async void OnSelectClicked(object sender, RoutedEventArgs args)
        {
            // 有信息在更新时，等待操作
            while (IsUpdatingNow || !IsInitializeFinished) await Task.Delay(50);
            lock (DownloadingNowLock) IsUpdatingNow = true;

            foreach (DownloadingModel downloadingItem in DownloadingDataList)
            {
                downloadingItem.IsSelected = false;
            }

            IsSelectMode = true;

            // 信息更新完毕时，允许其他操作开始执行
            lock (DownloadingNowLock) IsUpdatingNow = false;
        }

        /// <summary>
        /// 全部选择
        /// </summary>
        public async void OnSelectAllClicked(object sender, RoutedEventArgs args)
        {
            // 有信息在更新时，等待操作
            while (IsUpdatingNow || !IsInitializeFinished) await Task.Delay(50);
            lock (DownloadingNowLock) IsUpdatingNow = true;

            foreach (DownloadingModel downloadingItem in DownloadingDataList)
            {
                downloadingItem.IsSelected = true;
            }

            // 信息更新完毕时，允许其他操作开始执行
            lock (DownloadingNowLock) IsUpdatingNow = false;
        }

        /// <summary>
        /// 全部不选
        /// </summary>
        public async void OnSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            // 有信息在更新时，等待操作
            while (IsUpdatingNow || !IsInitializeFinished) await Task.Delay(50);
            lock (DownloadingNowLock) IsUpdatingNow = true;

            foreach (DownloadingModel downloadingItem in DownloadingDataList)
            {
                downloadingItem.IsSelected = false;
            }

            // 信息更新完毕时，允许其他操作开始执行
            lock (DownloadingNowLock) IsUpdatingNow = false;
        }

        /// <summary>
        /// 删除选中的任务
        /// </summary>
        public async void OnDeleteSelectedClicked(object sender, RoutedEventArgs args)
        {
            // 有信息在更新时，等待操作
            while (IsUpdatingNow || !IsInitializeFinished) await Task.Delay(50);
            lock (DownloadingNowLock) IsUpdatingNow = true;

            List<DownloadingModel> SelectedDownloadingDataList = DownloadingDataList.Where(item => item.IsSelected is true).ToList();

            // 没有选中任何内容时显示空提示对话框
            if (SelectedDownloadingDataList.Count is 0)
            {
                await new SelectEmptyPromptDialog().ShowAsync();
                return;
            }

            IsSelectMode = false;

            SelectedDownloadingDataList.ForEach(async downloadingItem =>
            {
                bool DeleteResult = await DownloadSchedulerService.DeleteTaskAsync(downloadingItem.DownloadKey, downloadingItem.GID, downloadingItem.DownloadFlag);

                if (DeleteResult)
                {
                    // 删除下载文件
                    try
                    {
                        if (File.Exists(downloadingItem.FilePath))
                        {
                            File.Delete(downloadingItem.FilePath);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LogType.WARNING, "Delete downloading file failed.", e);
                    }

                    // 删除Aria2后缀下载信息记录文件
                    try
                    {
                        if (File.Exists(string.Format("{0}.{1}", downloadingItem.FilePath, "aria2")))
                        {
                            File.Delete(string.Format("{0}.{1}", downloadingItem.FilePath, "aria2"));
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LogType.WARNING, "Delete downloading information file failed.", e);
                    }
                }
            });

            // 信息更新完毕时，允许其他操作开始执行
            lock (DownloadingNowLock) IsUpdatingNow = false;
        }

        public void OnCancelClicked(object sender, RoutedEventArgs args)
        {
            IsSelectMode = false;
        }

        /// <summary>
        /// 在多选模式下点击项目选择相应的条目
        /// </summary>
        public async void OnItemClicked(object sender, ItemClickEventArgs args)
        {
            // 有信息在更新时，等待操作
            while (IsUpdatingNow || !IsInitializeFinished) await Task.Delay(50);
            lock (DownloadingNowLock) IsUpdatingNow = true;

            DownloadingModel downloadingItem = (DownloadingModel)args.ClickedItem;
            int ClickedIndex = DownloadingDataList.IndexOf(downloadingItem);

            if (ClickedIndex >= 0 && ClickedIndex < DownloadingDataList.Count)
            {
                DownloadingDataList[ClickedIndex].IsSelected = !DownloadingDataList[ClickedIndex].IsSelected;
            }

            // 信息更新完毕时，允许其他操作开始执行
            lock (DownloadingNowLock) IsUpdatingNow = false;
        }

        public DownloadingViewModel()
        {
            // Tick 超过计时器间隔时发生
            DownloadingTimer.Tick += DownloadInfoTimerTick;
            // Interval 获取或设置计时器刻度之间的时间段
            DownloadingTimer.Interval = new TimeSpan(0, 0, 1);

            PauseCommand.ExecuteRequested += async (sender, args) =>
            {
                DownloadingModel downloadingItem = args.Parameter as DownloadingModel;
                if (downloadingItem is not null)
                {
                    // 有信息在更新时，等待操作
                    while (IsUpdatingNow || !IsInitializeFinished) await Task.Delay(50);
                    lock (DownloadingNowLock) IsUpdatingNow = true;

                    bool PauseResult = await DownloadSchedulerService.PauseTaskAsync(downloadingItem.DownloadKey, downloadingItem.GID, downloadingItem.DownloadFlag);

                    // 信息更新完毕时，允许其他操作开始执行
                    lock (DownloadingNowLock) IsUpdatingNow = false;
                }
            };

            DeleteCommand.ExecuteRequested += async (sender, args) =>
            {
                DownloadingModel downloadingItem = args.Parameter as DownloadingModel;
                if (downloadingItem is not null)
                {
                    // 有信息在更新时，等待操作
                    while (IsUpdatingNow) await Task.Delay(50);
                    lock (DownloadingNowLock) IsUpdatingNow = true;

                    bool DeleteResult = await DownloadSchedulerService.DeleteTaskAsync(downloadingItem.DownloadKey, downloadingItem.GID, downloadingItem.DownloadFlag);

                    if (DeleteResult)
                    {
                        // 删除下载文件
                        try
                        {
                            if (File.Exists(downloadingItem.FilePath))
                            {
                                File.Delete(downloadingItem.FilePath);
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LogType.WARNING, "Delete downloading file failed.", e);
                        }

                        // 删除Aria2后缀下载信息记录文件
                        try
                        {
                            if (File.Exists(string.Format("{0}.{1}", downloadingItem.FilePath, "aria2")))
                            {
                                File.Delete(string.Format("{0}.{1}", downloadingItem.FilePath, "aria2"));
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LogType.WARNING, "Delete downloading information file failed.", e);
                        }
                    }

                    // 信息更新完毕时，允许其他操作开始执行
                    lock (DownloadingNowLock) IsUpdatingNow = false;
                }
            };

            // 订阅事件
            DownloadSchedulerService.DownloadingList.ItemsChanged += OnDownloadingListItemsChanged;
            DownloadSchedulerService.WaitingList.ItemsChanged += OnWaitingListItemsChanged;
        }

        /// <summary>
        /// 开始运行下载计时器，并获取下载任务信息
        /// </summary>
        public void StartDownloadingTimer()
        {
            GetDownloadingDataList();
            DownloadingTimer.Start();
        }

        /// <summary>
        /// 停止运行下载计时器
        /// </summary>
        public void StopDownloadingTimer(bool needUnRegister)
        {
            if (DownloadingTimer.IsEnabled)
            {
                DownloadingTimer.Stop();
            }

            if (needUnRegister)
            {
                // 取消订阅所有事件
                DownloadingTimer.Tick -= DownloadInfoTimerTick;

                DownloadSchedulerService.DownloadingList.ItemsChanged -= OnDownloadingListItemsChanged;
                DownloadSchedulerService.WaitingList.ItemsChanged -= OnWaitingListItemsChanged;
            }
        }

        /// <summary>
        /// 从下载调度服务中获取正在下载和等待下载的数据
        /// </summary>
        private void GetDownloadingDataList()
        {
            // 有信息在更新时，等待操作
            lock (DownloadingNowLock) IsUpdatingNow = true;

            DownloadingDataList.Clear();

            DownloadSchedulerService.DownloadingList.ForEach(downloadItem =>
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
            });

            DownloadSchedulerService.WaitingList.ForEach(downloadItem =>
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
            });

            // 有信息在更新时，等待操作
            lock (DownloadingNowLock)
            {
                IsInitializeFinished = true;
                IsUpdatingNow = false;
            }
        }

        /// <summary>
        /// 计时器：获取正在下载中文件的下载进度
        /// </summary>
        private void DownloadInfoTimerTick(object sender, object args)
        {
            // 有信息在更新时，不再操作，等待下一秒尝试更新内容
            if (!IsInitializeFinished)
            {
                return;
            }

            // 当有信息处于更新状态中时，暂停其他操作
            lock (DownloadingNowLock) IsUpdatingNow = true;

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
                catch (Exception e)
                {
                    LogService.WriteLog(LogType.WARNING, "Update downloading progress failed.", e);
                    continue;
                }
            }

            // 信息更新完毕时，允许其他操作开始执行
            lock (DownloadingNowLock) IsUpdatingNow = false;
        }

        /// <summary>
        /// 订阅事件，下载中列表内容发生改变时通知UI更改
        /// </summary>
        private async void OnDownloadingListItemsChanged(object sender, ItemsChangedEventArgs<BackgroundModel> args)
        {
            // 有信息在更新时，等待操作
            while (IsUpdatingNow || !IsInitializeFinished) await Task.Delay(50);
            lock (DownloadingNowLock) IsUpdatingNow = true;

            // 下载中列表添加项目时，更新UI
            if (args.AddedItems.Count > 0)
            {
                Program.ApplicationRoot.MainWindow.DispatcherQueue.TryEnqueue(() =>
                {
                    foreach (BackgroundModel downloadItem in args.AddedItems)
                    {
                        DownloadingDataList.Insert(DownloadingDataList.Count(item => item.DownloadFlag is 3),
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
                Program.ApplicationRoot.MainWindow.DispatcherQueue.TryEnqueue(() =>
                {
                    foreach (BackgroundModel backgroundItem in args.RemovedItems)
                    {
                        try
                        {
                            DownloadingDataList.Remove(DownloadingDataList.First(item => item.DownloadKey == backgroundItem.DownloadKey && item.DownloadFlag is 3));
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LogType.WARNING, "Downloading list remove items failed", e);
                            continue;
                        }
                    }
                });
            }

            // 信息更新完毕时，允许其他操作开始执行
            lock (DownloadingNowLock) IsUpdatingNow = false;
        }

        /// <summary>
        /// 订阅事件，等待列表内容发生改变时通知UI更改
        /// </summary>
        private async void OnWaitingListItemsChanged(object sender, ItemsChangedEventArgs<BackgroundModel> args)
        {
            // 有信息在更新时，等待操作
            while (IsUpdatingNow || !IsInitializeFinished) await Task.Delay(50);
            lock (DownloadingNowLock) IsUpdatingNow = true;

            // 等待列表添加项目时，更新UI
            if (args.AddedItems.Count > 0)
            {
                Program.ApplicationRoot.MainWindow.DispatcherQueue.TryEnqueue(() =>
                {
                    foreach (BackgroundModel downloadItem in args.AddedItems)
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
                });
            }

            // 等待列表删除项目时，更新UI
            if (args.RemovedItems.Count > 0)
            {
                Program.ApplicationRoot.MainWindow.DispatcherQueue.TryEnqueue(() =>
                {
                    foreach (BackgroundModel backgroundItem in args.RemovedItems)
                    {
                        try
                        {
                            DownloadingDataList.Remove(DownloadingDataList.First(item => item.DownloadKey == backgroundItem.DownloadKey && item.DownloadFlag is 1));
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LogType.WARNING, "Waiting list remove items failed", e);
                            continue;
                        }
                    }
                });
            }

            // 信息更新完毕时，允许其他操作开始执行
            lock (DownloadingNowLock) IsUpdatingNow = false;
        }
    }
}
