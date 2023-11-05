using GetStoreApp.Helpers.Controls.Extensions;
using GetStoreApp.Models.Controls.Download;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.Common;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;

namespace GetStoreApp.UI.Controls.Download
{
    /// <summary>
    /// 下载页面：正在下载中控件
    /// </summary>
    public sealed partial class DownloadingControl : Grid, INotifyPropertyChanged
    {
        private readonly object DownloadingLock = new object();

        private bool isUpdatingNow = false;

        private bool IsInitializeFinished = false;

        public int SelectedIndex { get; set; } = 0;

        public DispatcherTimer DownloadingTimer { get; } = new DispatcherTimer();

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

        private ObservableCollection<DownloadingModel> DownloadingCollection { get; } = new ObservableCollection<DownloadingModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        public DownloadingControl()
        {
            InitializeComponent();
            DownloadingTimer.Interval = new TimeSpan(0, 0, 1);
        }

        #region 第一部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 删除当前任务
        /// </summary>
        private async void OnDeleteExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            DownloadingModel downloadingItem = args.Parameter as DownloadingModel;
            if (downloadingItem is not null)
            {
                // 有信息在更新时，等待操作
                while (isUpdatingNow) await Task.Delay(50);
                lock (DownloadingLock) isUpdatingNow = true;

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
                        LogService.WriteLog(LoggingLevel.Warning, "Delete downloading file failed.", e);
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
                        LogService.WriteLog(LoggingLevel.Warning, "Delete downloading information file failed.", e);
                    }
                }

                // 信息更新完毕时，允许其他操作开始执行
                lock (DownloadingLock) isUpdatingNow = false;
            }
        }

        /// <summary>
        /// 暂停下载当前任务
        /// </summary>
        private async void OnPauseExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            DownloadingModel downloadingItem = args.Parameter as DownloadingModel;
            if (downloadingItem is not null)
            {
                // 有信息在更新时，等待操作
                while (isUpdatingNow || !IsInitializeFinished) await Task.Delay(50);
                lock (DownloadingLock) isUpdatingNow = true;

                await DownloadSchedulerService.PauseTaskAsync(downloadingItem.DownloadKey, downloadingItem.GID, downloadingItem.DownloadFlag);

                // 信息更新完毕时，允许其他操作开始执行
                lock (DownloadingLock) isUpdatingNow = false;
            }
        }

        #endregion 第一部分：XamlUICommand 命令调用时挂载的事件

        #region 第二部分：正在下载中控件——挂载的事件

        /// <summary>
        /// 打开默认保存的文件夹
        /// </summary>
        private async void OnOpenFolderClicked(object sender, RoutedEventArgs args)
        {
            await DownloadOptionsService.OpenFolderAsync(DownloadOptionsService.DownloadFolder);
        }

        /// <summary>
        /// 暂停下载全部任务
        /// </summary>
        private async void OnPauseAllClicked(object sender, RoutedEventArgs args)
        {
            // 有信息在更新时，等待操作
            while (isUpdatingNow || !IsInitializeFinished) await Task.Delay(50);
            lock (DownloadingLock) isUpdatingNow = true;

            // 暂停下载所有任务
            await DownloadSchedulerService.PauseAllTaskAsync();

            // 信息更新完毕时，允许其他操作开始执行
            lock (DownloadingLock) isUpdatingNow = false;
        }

        /// <summary>
        /// 进入多选模式
        /// </summary>
        private async void OnSelectClicked(object sender, RoutedEventArgs args)
        {
            // 有信息在更新时，等待操作
            while (isUpdatingNow || !IsInitializeFinished) await Task.Delay(50);
            lock (DownloadingLock) isUpdatingNow = true;

            foreach (DownloadingModel downloadingItem in DownloadingCollection)
            {
                downloadingItem.IsSelectMode = true;
                downloadingItem.IsSelected = false;
            }

            IsSelectMode = true;

            // 信息更新完毕时，允许其他操作开始执行
            lock (DownloadingLock) isUpdatingNow = false;
        }

        /// <summary>
        /// 全部选择
        /// </summary>
        private async void OnSelectAllClicked(object sender, RoutedEventArgs args)
        {
            // 有信息在更新时，等待操作
            while (isUpdatingNow || !IsInitializeFinished) await Task.Delay(50);
            lock (DownloadingLock) isUpdatingNow = true;

            foreach (DownloadingModel downloadingItem in DownloadingCollection)
            {
                downloadingItem.IsSelected = true;
            }

            // 信息更新完毕时，允许其他操作开始执行
            lock (DownloadingLock) isUpdatingNow = false;
        }

        /// <summary>
        /// 全部不选
        /// </summary>
        private async void OnSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            // 有信息在更新时，等待操作
            while (isUpdatingNow || !IsInitializeFinished) await Task.Delay(50);
            lock (DownloadingLock) isUpdatingNow = true;

            foreach (DownloadingModel downloadingItem in DownloadingCollection)
            {
                downloadingItem.IsSelected = false;
            }

            // 信息更新完毕时，允许其他操作开始执行
            lock (DownloadingLock) isUpdatingNow = false;
        }

        /// <summary>
        /// 删除选中的任务
        /// </summary>
        private async void OnDeleteSelectedClicked(object sender, RoutedEventArgs args)
        {
            // 有信息在更新时，等待操作
            while (isUpdatingNow || !IsInitializeFinished) await Task.Delay(50);
            lock (DownloadingLock) isUpdatingNow = true;

            List<DownloadingModel> selectedDownloadingDataList = DownloadingCollection.Where(item => item.IsSelected is true).ToList();

            // 没有选中任何内容时显示空提示对话框
            if (selectedDownloadingDataList.Count is 0)
            {
                await ContentDialogHelper.ShowAsync(new SelectEmptyPromptDialog(), this);
                return;
            }

            IsSelectMode = false;

            foreach (DownloadingModel downloadingItem in DownloadingCollection)
            {
                downloadingItem.IsSelectMode = false;
            }

            selectedDownloadingDataList.ForEach(async downloadingItem =>
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
                        LogService.WriteLog(LoggingLevel.Warning, "Delete downloading file failed.", e);
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
                        LogService.WriteLog(LoggingLevel.Warning, "Delete downloading information file failed.", e);
                    }
                }
            });

            // 信息更新完毕时，允许其他操作开始执行
            lock (DownloadingLock) isUpdatingNow = false;
        }

        private void OnCancelClicked(object sender, RoutedEventArgs args)
        {
            IsSelectMode = false;
            foreach (DownloadingModel downloadingItem in DownloadingCollection)
            {
                downloadingItem.IsSelectMode = false;
            }
        }

        /// <summary>
        /// 在多选模式下点击项目选择相应的条目
        /// </summary>
        private async void OnItemClicked(object sender, ItemClickEventArgs args)
        {
            // 有信息在更新时，等待操作
            while (isUpdatingNow || !IsInitializeFinished) await Task.Delay(50);
            lock (DownloadingLock) isUpdatingNow = true;

            DownloadingModel downloadingItem = (DownloadingModel)args.ClickedItem;
            int ClickedIndex = DownloadingCollection.IndexOf(downloadingItem);

            if (ClickedIndex >= 0 && ClickedIndex < DownloadingCollection.Count)
            {
                DownloadingCollection[ClickedIndex].IsSelected = !DownloadingCollection[ClickedIndex].IsSelected;
            }

            // 信息更新完毕时，允许其他操作开始执行
            lock (DownloadingLock) isUpdatingNow = false;
        }

        #endregion 第二部分：正在下载中控件——挂载的事件

        #region 第三部分：自定义事件

        /// <summary>
        /// 计时器：获取正在下载中文件的下载进度
        /// </summary>
        public void DownloadInfoTimerTick(object sender, object args)
        {
            // 有信息在更新时，不再操作，等待下一秒尝试更新内容
            if (!IsInitializeFinished)
            {
                return;
            }

            // 当有信息处于更新状态中时，暂停其他操作
            lock (DownloadingLock) isUpdatingNow = true;

            ObservableCollection<BackgroundModel> downloadingList = DownloadSchedulerService.DownloadingCollection;

            foreach (BackgroundModel backgroundItem in downloadingList)
            {
                try
                {
                    int index = DownloadingCollection.IndexOf(DownloadingCollection.First(item => item.DownloadKey == backgroundItem.DownloadKey));
                    DownloadingCollection[index].GID = backgroundItem.GID;
                    DownloadingCollection[index].FinishedSize = backgroundItem.FinishedSize;
                    DownloadingCollection[index].TotalSize = backgroundItem.TotalSize;
                    DownloadingCollection[index].CurrentSpeed = backgroundItem.CurrentSpeed;
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Warning, "Update downloading progress failed.", e);
                    continue;
                }
            }

            // 信息更新完毕时，允许其他操作开始执行
            lock (DownloadingLock) isUpdatingNow = false;
        }

        /// <summary>
        /// 订阅事件，下载中列表内容发生改变时通知UI更改
        /// </summary>
        public void OnDownloadingListItemsChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (SelectedIndex is 0)
            {
                // 下载中列表添加项目时，更新UI
                if (args.Action is NotifyCollectionChangedAction.Add)
                {
                    DispatcherQueue.TryEnqueue(async () =>
                    {
                        // 有信息在更新时，等待操作
                        while (isUpdatingNow || !IsInitializeFinished) await Task.Delay(50);
                        lock (DownloadingLock) isUpdatingNow = true;

                        foreach (object newItem in args.NewItems)
                        {
                            BackgroundModel downloadItem = newItem as BackgroundModel;
                            if (downloadItem is not null)
                            {
                                DownloadingCollection.Insert(DownloadingCollection.Count(item => item.DownloadFlag is 3),
                                    new DownloadingModel
                                    {
                                        DownloadKey = downloadItem.DownloadKey,
                                        FileName = downloadItem.FileName,
                                        FileLink = downloadItem.FileLink,
                                        FilePath = downloadItem.FilePath,
                                        TotalSize = downloadItem.TotalSize,
                                        DownloadFlag = downloadItem.DownloadFlag
                                    });
                            }
                        }

                        lock (DownloadingLock) isUpdatingNow = false;
                    });
                }

                // 下载中列表删除项目时，更新UI
                if (args.Action is NotifyCollectionChangedAction.Remove)
                {
                    DispatcherQueue.TryEnqueue(async () =>
                    {
                        // 有信息在更新时，等待操作
                        while (isUpdatingNow || !IsInitializeFinished) await Task.Delay(50);
                        lock (DownloadingLock) isUpdatingNow = true;

                        foreach (object oldItem in args.OldItems)
                        {
                            BackgroundModel downloadItem = oldItem as BackgroundModel;
                            if (downloadItem is not null)
                            {
                                try
                                {
                                    DownloadingCollection.Remove(DownloadingCollection.First(item => item.DownloadKey == downloadItem.DownloadKey && item.DownloadFlag is 3));
                                }
                                catch (Exception e)
                                {
                                    LogService.WriteLog(LoggingLevel.Warning, "Downloading list remove items failed", e);
                                    continue;
                                }
                            }
                        }

                        lock (DownloadingLock) isUpdatingNow = false;
                    });
                }
            }
        }

        /// <summary>
        /// 订阅事件，等待列表内容发生改变时通知UI更改
        /// </summary>
        public void OnWaitingListItemsChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (SelectedIndex is 0)
            {
                // 等待列表添加项目时，更新UI
                if (args.Action is NotifyCollectionChangedAction.Add)
                {
                    DispatcherQueue.TryEnqueue(async () =>
                    {
                        // 有信息在更新时，等待操作
                        while (isUpdatingNow || !IsInitializeFinished) await Task.Delay(50);
                        lock (DownloadingLock) isUpdatingNow = true;

                        foreach (object newItem in args.NewItems)
                        {
                            BackgroundModel downloadItem = newItem as BackgroundModel;
                            if (downloadItem is not null)
                            {
                                DownloadingCollection.Add(new DownloadingModel
                                {
                                    DownloadKey = downloadItem.DownloadKey,
                                    FileName = downloadItem.FileName,
                                    FileLink = downloadItem.FileLink,
                                    FilePath = downloadItem.FilePath,
                                    TotalSize = downloadItem.TotalSize,
                                    DownloadFlag = downloadItem.DownloadFlag
                                });
                            }
                        }

                        lock (DownloadingLock) isUpdatingNow = false;
                    });
                }

                // 等待列表添加项目时，更新UI
                if (args.Action is NotifyCollectionChangedAction.Remove)
                {
                    DispatcherQueue.TryEnqueue(async () =>
                    {
                        // 有信息在更新时，等待操作
                        while (isUpdatingNow || !IsInitializeFinished) await Task.Delay(50);
                        lock (DownloadingLock) isUpdatingNow = true;

                        foreach (object oldItem in args.OldItems)
                        {
                            BackgroundModel downloadItem = oldItem as BackgroundModel;
                            if (downloadItem is not null)
                            {
                                try
                                {
                                    DownloadingCollection.Remove(DownloadingCollection.First(item => item.DownloadKey == downloadItem.DownloadKey && item.DownloadFlag is 1));
                                }
                                catch (Exception e)
                                {
                                    LogService.WriteLog(LoggingLevel.Warning, "Waiting list remove items failed", e);
                                    continue;
                                }
                            }
                        }

                        lock (DownloadingLock) isUpdatingNow = false;
                    });
                }
            }
        }

        #endregion 第三部分：自定义事件

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 本地化正在下载完成数量统计信息
        /// </summary>
        private string LocalizeDownloadingCountInfo(int count)
        {
            if (count is 0)
            {
                return ResourceService.GetLocalized("Download/DownloadingEmpty");
            }
            else
            {
                return string.Format(ResourceService.GetLocalized("Download/DownloadingCountInfo"), count);
            }
        }

        /// <summary>
        /// 开始运行下载计时器，并获取下载任务信息
        /// </summary>
        public async Task StartDownloadingTimerAsync()
        {
            await GetDownloadingDataListAsync();
            DownloadingTimer.Start();
        }

        /// <summary>
        /// 停止运行下载计时器
        /// </summary>
        public void StopDownloadingTimer()
        {
            if (DownloadingTimer.IsEnabled)
            {
                DownloadingTimer.Stop();
            }
        }

        /// <summary>
        /// 从下载调度服务中获取正在下载和等待下载的数据
        /// </summary>
        private async Task GetDownloadingDataListAsync()
        {
            // 有信息在更新时，等待操作
            lock (DownloadingLock) isUpdatingNow = true;

            DownloadingCollection.Clear();

            ObservableCollection<BackgroundModel> downloadingList = DownloadSchedulerService.DownloadingCollection;

            foreach (BackgroundModel downloadItem in downloadingList)
            {
                DownloadingCollection.Add(new DownloadingModel
                {
                    DownloadKey = downloadItem.DownloadKey,
                    FileName = downloadItem.FileName,
                    FileLink = downloadItem.FileLink,
                    FilePath = downloadItem.FilePath,
                    TotalSize = downloadItem.TotalSize,
                    DownloadFlag = downloadItem.DownloadFlag
                });
                await Task.Delay(1);
            }

            foreach (BackgroundModel downloadItem in DownloadSchedulerService.WaitingCollection)
            {
                DownloadingCollection.Add(new DownloadingModel
                {
                    DownloadKey = downloadItem.DownloadKey,
                    FileName = downloadItem.FileName,
                    FileLink = downloadItem.FileLink,
                    FilePath = downloadItem.FilePath,
                    TotalSize = downloadItem.TotalSize,
                    DownloadFlag = downloadItem.DownloadFlag
                });
                await Task.Delay(1);
            }

            // 有信息在更新时，等待操作
            lock (DownloadingLock)
            {
                IsInitializeFinished = true;
                isUpdatingNow = false;
            }
        }
    }
}
