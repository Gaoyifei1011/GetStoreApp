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
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;

namespace GetStoreApp.UI.Controls.Download
{
    /// <summary>
    /// 下载页面：未下载完成控件
    /// </summary>
    public sealed partial class UnfinishedControl : Grid, INotifyPropertyChanged
    {
        private readonly object unfinishedLock = new object();

        private bool isUpdatingNow = false;

        public int SelectedIndex { get; set; } = 0;

        private bool _isSelectMode = false;

        public bool IsSelectMode
        {
            get { return _isSelectMode; }

            set
            {
                if (!Equals(_isSelectMode, value))
                {
                    _isSelectMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelectMode)));
                }
            }
        }

        public ObservableCollection<UnfinishedModel> UnfinishedCollection { get; } = new ObservableCollection<UnfinishedModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        public UnfinishedControl()
        {
            InitializeComponent();
        }

        #region 第一部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 继续下载当前任务
        /// </summary>
        private void OnContinueExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            UnfinishedModel unfinishedItem = args.Parameter as UnfinishedModel;
            if (unfinishedItem is not null)
            {
                if (unfinishedItem.DownloadFlag is 2)
                {
                    Task.Run(async () =>
                    {
                        bool continueResult = await DownloadSchedulerService.ContinueTaskAsync(new BackgroundModel
                        {
                            DownloadKey = unfinishedItem.DownloadKey,
                            FileName = unfinishedItem.FileName,
                            FileLink = unfinishedItem.FileLink,
                            FilePath = unfinishedItem.FilePath,
                            TotalSize = unfinishedItem.TotalSize
                        });

                        if (continueResult)
                        {
                            DispatcherQueue.TryEnqueue(async () =>
                            {
                                while (isUpdatingNow) await Task.Delay(50);
                                lock (unfinishedLock) isUpdatingNow = true;

                                try
                                {
                                    UnfinishedCollection.Remove(unfinishedItem);
                                }
                                catch (Exception e)
                                {
                                    LogService.WriteLog(LoggingLevel.Warning, "Unfinished list remove items failed", e);
                                }

                                lock (unfinishedLock) isUpdatingNow = false;
                            });
                        }
                    });
                }
            }
        }

        /// <summary>
        /// 删除当前任务
        /// </summary>
        private void OnDeleteExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            UnfinishedModel unfinishedItem = args.Parameter as UnfinishedModel;
            if (unfinishedItem is not null)
            {
                Task.Run(async () =>
                {
                    // 删除下载文件
                    try
                    {
                        if (File.Exists(unfinishedItem.FilePath))
                        {
                            File.Delete(unfinishedItem.FilePath);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Warning, "Delete unfinished download file failed.", e);
                    }

                    // 删除Aria2后缀下载信息记录文件
                    try
                    {
                        if (File.Exists(string.Format("{0}.{1}", unfinishedItem.FilePath, "aria2")))
                        {
                            File.Delete(string.Format("{0}.{1}", unfinishedItem.FilePath, "aria2"));
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Warning, "Delete unfinished download information file failed.", e);
                    }

                    // 删除记录

                    bool deleteResult = await DownloadXmlService.DeleteAsync(unfinishedItem.DownloadKey);

                    if (deleteResult)
                    {
                        DispatcherQueue.TryEnqueue(async () =>
                        {
                            while (isUpdatingNow) await Task.Delay(50);
                            lock (unfinishedLock) isUpdatingNow = true;

                            try
                            {
                                UnfinishedCollection.Remove(unfinishedItem);
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(LoggingLevel.Warning, "Unfinished list remove items failed", e);
                            }

                            lock (unfinishedLock) isUpdatingNow = false;
                        });
                    }
                });
            }
        }

        #endregion 第一部分：XamlUICommand 命令调用时挂载的事件

        #region 第二部分：未下载完成控件——挂载的事件

        /// <summary>
        /// 打开默认保存的文件夹
        /// </summary>
        private async void OnOpenFolderClicked(object sender, RoutedEventArgs args)
        {
            await DownloadOptionsService.OpenFolderAsync(DownloadOptionsService.DownloadFolder);
        }

        /// <summary>
        /// 继续下载全部任务
        /// </summary>
        private async void OnContinueAllClicked(object sender, RoutedEventArgs args)
        {
            List<BackgroundModel> pauseList = new List<BackgroundModel>();

            foreach (UnfinishedModel unfinishedItem in UnfinishedCollection)
            {
                if (unfinishedItem.DownloadFlag is 2)
                {
                    pauseList.Add(new BackgroundModel
                    {
                        DownloadKey = unfinishedItem.DownloadKey,
                        FileName = unfinishedItem.FileName,
                        FileLink = unfinishedItem.FileLink,
                        FilePath = unfinishedItem.FilePath,
                        DownloadFlag = unfinishedItem.DownloadFlag,
                        TotalSize = unfinishedItem.TotalSize,
                    });
                }
            }

            await Task.Run(async () =>
            {
                foreach (BackgroundModel unfinishedItem in pauseList)
                {
                    bool continueResult = await DownloadSchedulerService.ContinueTaskAsync(unfinishedItem);

                    if (continueResult)
                    {
                        try
                        {
                            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
                            DispatcherQueue.TryEnqueue(async () =>
                            {
                                for (int index = 0; index < UnfinishedCollection.Count; index++)
                                {
                                    if (UnfinishedCollection[index].DownloadKey.Equals(unfinishedItem.DownloadKey, StringComparison.OrdinalIgnoreCase))
                                    {
                                        while (isUpdatingNow) await Task.Delay(50);
                                        lock (unfinishedLock) isUpdatingNow = true;

                                        try
                                        {
                                            UnfinishedCollection.RemoveAt(index);
                                        }
                                        catch (Exception e)
                                        {
                                            LogService.WriteLog(LoggingLevel.Warning, "Unfinished list remove items failed", e);
                                        }

                                        lock (unfinishedLock) isUpdatingNow = false;
                                        break;
                                    }
                                }

                                autoResetEvent.Set();
                            });

                            autoResetEvent.WaitOne();
                            autoResetEvent.Dispose();
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LoggingLevel.Warning, "Unfinished download list remove oldItem failed.", e);
                            continue;
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 进入多选模式
        /// </summary>
        private async void OnSelectClicked(object sender, RoutedEventArgs args)
        {
            while (isUpdatingNow) await Task.Delay(50);
            lock (unfinishedLock) isUpdatingNow = true;

            foreach (UnfinishedModel unfinishedItem in UnfinishedCollection)
            {
                unfinishedItem.IsSelectMode = true;
                unfinishedItem.IsSelected = false;
            }

            IsSelectMode = true;
            lock (unfinishedLock) isUpdatingNow = false;
        }

        /// <summary>
        /// 全部选择
        /// </summary>
        private async void OnSelectAllClicked(object sender, RoutedEventArgs args)
        {
            while (isUpdatingNow) await Task.Delay(50);
            lock (unfinishedLock) isUpdatingNow = true;

            foreach (UnfinishedModel unfinishedItem in UnfinishedCollection)
            {
                unfinishedItem.IsSelected = true;
            }

            lock (unfinishedLock) isUpdatingNow = false;
        }

        /// <summary>
        /// 全部不选
        /// </summary>
        private async void OnSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            while (isUpdatingNow) await Task.Delay(50);
            lock (unfinishedLock) isUpdatingNow = true;

            foreach (UnfinishedModel unfinishedItem in UnfinishedCollection)
            {
                unfinishedItem.IsSelected = false;
            }

            lock (unfinishedLock) isUpdatingNow = false;
        }

        /// <summary>
        /// 删除选中的任务
        /// </summary>
        private async void OnDeleteSelectedClicked(object sender, RoutedEventArgs args)
        {
            List<BackgroundModel> selectedUnfinishedDataList = new List<BackgroundModel>();

            foreach (UnfinishedModel unfinishedItem in UnfinishedCollection)
            {
                if (unfinishedItem.IsSelected is true)
                {
                    selectedUnfinishedDataList.Add(new BackgroundModel
                    {
                        DownloadKey = unfinishedItem.DownloadKey,
                        FilePath = unfinishedItem.FilePath
                    });
                }
            }

            // 没有选中任何内容时显示空提示对话框
            if (selectedUnfinishedDataList.Count is 0)
            {
                await ContentDialogHelper.ShowAsync(new SelectEmptyPromptDialog(), this);
                return;
            }

            IsSelectMode = false;

            while (isUpdatingNow) await Task.Delay(50);
            lock (unfinishedLock) isUpdatingNow = true;

            foreach (UnfinishedModel unfinishedItem in UnfinishedCollection)
            {
                unfinishedItem.IsSelectMode = false;
            }

            lock (unfinishedLock) isUpdatingNow = false;

            await Task.Run(async () =>
            {
                foreach (BackgroundModel backgroundItem in selectedUnfinishedDataList)
                {
                    // 删除下载文件
                    try
                    {
                        if (File.Exists(backgroundItem.FilePath))
                        {
                            File.Delete(backgroundItem.FilePath);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Warning, "Delete unfinished download list  file failed.", e);
                    }

                    // 删除Aria2后缀下载信息记录文件
                    try
                    {
                        if (File.Exists(string.Format("{0}.{1}", backgroundItem.FilePath, "aria2")))
                        {
                            File.Delete(string.Format("{0}.{1}", backgroundItem.FilePath, "aria2"));
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Warning, "Delete unfinished download information file failed.", e);
                    }

                    // 删除记录
                    try
                    {
                        bool deleteResult = await DownloadXmlService.DeleteAsync(backgroundItem.DownloadKey);

                        if (deleteResult)
                        {
                            DispatcherQueue.TryEnqueue(async () =>
                            {
                                while (isUpdatingNow) await Task.Delay(50);
                                lock (unfinishedLock) isUpdatingNow = true;

                                for (int index = 0; index < UnfinishedCollection.Count; index++)
                                {
                                    if (UnfinishedCollection[index].DownloadKey.Equals(backgroundItem.DownloadKey))
                                    {
                                        try
                                        {
                                            UnfinishedCollection.RemoveAt(index);
                                        }
                                        catch (Exception e)
                                        {
                                            LogService.WriteLog(LoggingLevel.Warning, "Unfinished list remove items failed", e);
                                        }
                                    }
                                }

                                lock (unfinishedLock) isUpdatingNow = false;
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Warning, "Delete unfinished download record failed.", e);
                    }
                }
            });
        }

        /// <summary>
        /// 退出多选模式
        /// </summary>
        private void OnCancelClicked(object sender, RoutedEventArgs args)
        {
            IsSelectMode = false;
            foreach (UnfinishedModel unfinishedItem in UnfinishedCollection)
            {
                unfinishedItem.IsSelectMode = false;
            }
        }

        /// <summary>
        /// 在多选模式下点击项目选择相应的条目
        /// </summary>
        private async void OnItemClicked(object sender, ItemClickEventArgs args)
        {
            UnfinishedModel resultItem = (UnfinishedModel)args.ClickedItem;
            int ClickedIndex = UnfinishedCollection.IndexOf(resultItem);

            while (isUpdatingNow) await Task.Delay(50);
            lock (unfinishedLock) isUpdatingNow = true;

            if (ClickedIndex is not >= 0 && ClickedIndex < UnfinishedCollection.Count)
            {
                UnfinishedCollection[ClickedIndex].IsSelected = !UnfinishedCollection[ClickedIndex].IsSelected;
            }

            lock (unfinishedLock) isUpdatingNow = false;
        }

        #endregion 第二部分：未下载完成控件——挂载的事件

        #region 第三部分：自定义事件

        /// <summary>
        /// 订阅事件，下载中列表内容有暂停下载或下载失败的项目时通知UI更改
        /// </summary>
        public void OnDownloadingListItemsChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (SelectedIndex is 1)
            {
                if (args.Action is NotifyCollectionChangedAction.Remove)
                {
                    foreach (object oldItem in args.OldItems)
                    {
                        BackgroundModel backgroundItem = oldItem as BackgroundModel;
                        if (backgroundItem is not null)
                        {
                            if (backgroundItem.DownloadFlag is 0 || backgroundItem.DownloadFlag is 2)
                            {
                                DispatcherQueue.TryEnqueue(async () =>
                                {
                                    while (isUpdatingNow) await Task.Delay(50);
                                    lock (unfinishedLock) isUpdatingNow = true;

                                    BackgroundModel item = await DownloadXmlService.QueryWithKeyAsync(backgroundItem.DownloadKey);

                                    UnfinishedCollection.Add(new UnfinishedModel
                                    {
                                        DownloadKey = item.DownloadKey,
                                        FileName = item.FileName,
                                        FilePath = item.FilePath,
                                        FileLink = item.FileLink,
                                        TotalSize = item.TotalSize,
                                        DownloadFlag = item.DownloadFlag
                                    });

                                    lock (unfinishedLock) isUpdatingNow = false;
                                });
                            }
                        }
                    }
                }
            }
        }

        #endregion 第三部分：自定义事件

        /// <summary>
        /// 本地化未下载完成数量统计信息
        /// </summary>
        private string LocalizeUnfinishedCountInfo(int count)
        {
            if (count is 0)
            {
                return ResourceService.GetLocalized("Download/UnfinishedEmpty");
            }
            else
            {
                return string.Format(ResourceService.GetLocalized("Download/UnfinishedCountInfo"), count);
            }
        }

        /// <summary>
        /// 从数据库中加载未下载完成和下载失败的数据
        /// </summary>
        public async Task GetUnfinishedDataListAsync()
        {
            List<BackgroundModel> failureDownloadRawList = await DownloadXmlService.QueryWithFlagAsync(0);
            List<BackgroundModel> pauseDownloadRawList = await DownloadXmlService.QueryWithFlagAsync(2);

            while (isUpdatingNow) await Task.Delay(50);
            lock (unfinishedLock) isUpdatingNow = true;

            UnfinishedCollection.Clear();

            pauseDownloadRawList.ForEach(downloadItem =>
            {
                UnfinishedCollection.Add(new UnfinishedModel
                {
                    DownloadKey = downloadItem.DownloadKey,
                    FileName = downloadItem.FileName,
                    FileLink = downloadItem.FileLink,
                    FilePath = downloadItem.FilePath,
                    TotalSize = downloadItem.TotalSize,
                    DownloadFlag = downloadItem.DownloadFlag
                });
            });

            failureDownloadRawList.ForEach(async downloadItem =>
            {
                UnfinishedCollection.Add(new UnfinishedModel
                {
                    DownloadKey = downloadItem.DownloadKey,
                    FileName = downloadItem.FileName,
                    FileLink = downloadItem.FileLink,
                    FilePath = downloadItem.FilePath,
                    TotalSize = downloadItem.TotalSize,
                    DownloadFlag = downloadItem.DownloadFlag
                });
                await Task.Delay(1);
            });

            lock (unfinishedLock) isUpdatingNow = false;
        }
    }
}
