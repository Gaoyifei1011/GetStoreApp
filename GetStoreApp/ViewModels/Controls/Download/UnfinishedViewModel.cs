using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.DataType.Events;
using GetStoreApp.Models.Controls.Download;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.Services.Controls.Settings.Experiment;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.Common;
using GetStoreApp.UI.Notifications;
using GetStoreApp.ViewModels.Base;
using GetStoreApp.WindowsAPI.PInvoke.WinINet;
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
    /// 下载页面：未下载完成用户控件视图模型
    /// </summary>
    public sealed class UnfinishedViewModel : ViewModelBase
    {
        // 临界区资源访问互斥锁
        private readonly object UnfinishedDataListLock = new object();

        private bool isUpdatingNow = false;

        public ObservableCollection<UnfinishedModel> UnfinishedDataList { get; } = new ObservableCollection<UnfinishedModel>();

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

        // 继续下载当前任务
        public XamlUICommand ContinueCommand { get; } = new XamlUICommand();

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
        /// 继续下载全部任务
        /// </summary>
        public async void OnContinueAllClicked(object sender, RoutedEventArgs args)
        {
            // 查看是否开启了网络监控服务
            if (NetWorkMonitorService.NetWorkMonitorValue)
            {
                // 网络处于未连接状态，不再进行下载，显示通知
                INTERNET_CONNECTION_FLAGS flags = INTERNET_CONNECTION_FLAGS.INTERNET_CONNECTION_OFFLINE;
                if (!WinINetLibrary.InternetGetConnectedState(ref flags, 0))
                {
                    new NetWorkErrorNotification().Show();
                    return;
                }
            }

            List<BackgroundModel> PauseList = new List<BackgroundModel>();

            foreach (UnfinishedModel unfinishedItem in UnfinishedDataList.Where(item => item.DownloadFlag is 2))
            {
                PauseList.Add(new BackgroundModel
                {
                    DownloadKey = unfinishedItem.DownloadKey,
                    FileName = unfinishedItem.FileName,
                    FileLink = unfinishedItem.FileLink,
                    FilePath = unfinishedItem.FilePath,
                    FileSHA1 = unfinishedItem.FileSHA1,
                    DownloadFlag = unfinishedItem.DownloadFlag,
                    TotalSize = unfinishedItem.TotalSize,
                });
            }

            while (isUpdatingNow) await Task.Delay(50);
            lock (UnfinishedDataListLock) isUpdatingNow = true;

            foreach (BackgroundModel unfinishedItem in PauseList)
            {
                bool ContinueResult = await DownloadSchedulerService.ContinueTaskAsync(unfinishedItem);

                if (ContinueResult)
                {
                    try
                    {
                        UnfinishedDataList.Remove(UnfinishedDataList.First(item => item.DownloadKey == item.DownloadKey));
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LogType.WARNING, "Unfinished download list remove item failed.", e);
                        continue;
                    }
                }
                else
                {
                    continue;
                }
            }

            lock (UnfinishedDataListLock) isUpdatingNow = false;
        }

        /// <summary>
        /// 进入多选模式
        /// </summary>
        public async void OnSelectClicked(object sender, RoutedEventArgs args)
        {
            while (isUpdatingNow) await Task.Delay(50);
            lock (UnfinishedDataListLock) isUpdatingNow = true;

            foreach (UnfinishedModel unfinishedItem in UnfinishedDataList)
            {
                unfinishedItem.IsSelectMode = true;
                unfinishedItem.IsSelected = false;
            }

            IsSelectMode = true;
            lock (UnfinishedDataListLock) isUpdatingNow = false;
        }

        /// <summary>
        /// 全部选择
        /// </summary>
        public async void OnSelectAllClicked(object sender, RoutedEventArgs args)
        {
            while (isUpdatingNow) await Task.Delay(50);
            lock (UnfinishedDataListLock) isUpdatingNow = true;

            foreach (UnfinishedModel unfinishedItem in UnfinishedDataList)
            {
                unfinishedItem.IsSelected = true;
            }

            lock (UnfinishedDataListLock) isUpdatingNow = false;
        }

        /// <summary>
        /// 全部不选
        /// </summary>
        public async void OnSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            while (isUpdatingNow) await Task.Delay(50);
            lock (UnfinishedDataListLock) isUpdatingNow = true;

            foreach (UnfinishedModel unfinishedItem in UnfinishedDataList)
            {
                unfinishedItem.IsSelected = false;
            }

            lock (UnfinishedDataListLock) isUpdatingNow = false;
        }

        /// <summary>
        /// 删除选中的任务
        /// </summary>
        public async void OnDeleteSelectedClicked(object sender, RoutedEventArgs args)
        {
            List<BackgroundModel> SelectedUnfinishedDataList = new List<BackgroundModel>();

            foreach (UnfinishedModel unfinishedItem in UnfinishedDataList.Where(item => item.IsSelected is true))
            {
                SelectedUnfinishedDataList.Add(new BackgroundModel
                {
                    DownloadKey = unfinishedItem.DownloadKey,
                    FilePath = unfinishedItem.FilePath
                });
            }

            // 没有选中任何内容时显示空提示对话框
            if (SelectedUnfinishedDataList.Count is 0)
            {
                await new SelectEmptyPromptDialog().ShowAsync();
                return;
            }

            IsSelectMode = false;

            while (isUpdatingNow) await Task.Delay(50);
            lock (UnfinishedDataListLock) isUpdatingNow = true;

            foreach (UnfinishedModel unfinishedItem in UnfinishedDataList)
            {
                unfinishedItem.IsSelectMode = false;
            }

            foreach (BackgroundModel backgroundItem in SelectedUnfinishedDataList)
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
                    LogService.WriteLog(LogType.WARNING, "Delete unfinished download list  file failed.", e);
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
                    LogService.WriteLog(LogType.WARNING, "Delete unfinished download information file failed.", e);
                }

                // 删除记录
                try
                {
                    bool DeleteResult = await DownloadXmlService.DeleteAsync(backgroundItem.DownloadKey);

                    if (DeleteResult)
                    {
                        UnfinishedDataList.Remove(UnfinishedDataList.First(item => item.DownloadKey == item.DownloadKey));
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LogType.WARNING, "Delete unfinished download record failed.", e);
                }
            }

            lock (UnfinishedDataListLock) isUpdatingNow = false;
        }

        /// <summary>
        /// 退出多选模式
        /// </summary>
        public void OnCancelClicked(object sender, RoutedEventArgs args)
        {
            IsSelectMode = false;
            foreach (UnfinishedModel unfinishedItem in UnfinishedDataList)
            {
                unfinishedItem.IsSelectMode = false;
            }
        }

        public UnfinishedViewModel()
        {
            ContinueCommand.ExecuteRequested += async (sender, args) =>
            {
                UnfinishedModel unfinishedItem = args.Parameter as UnfinishedModel;
                if (unfinishedItem is not null)
                {
                    // 查看是否开启了网络监控服务
                    if (NetWorkMonitorService.NetWorkMonitorValue)
                    {
                        // 网络处于未连接状态，不再进行下载，显示通知
                        INTERNET_CONNECTION_FLAGS flags = INTERNET_CONNECTION_FLAGS.INTERNET_CONNECTION_OFFLINE;
                        if (!WinINetLibrary.InternetGetConnectedState(ref flags, 0))
                        {
                            new NetWorkErrorNotification().Show();
                            return;
                        }
                    }

                    if (unfinishedItem.DownloadFlag is 2)
                    {
                        bool ContinueResult = await DownloadSchedulerService.ContinueTaskAsync(new BackgroundModel
                        {
                            DownloadKey = unfinishedItem.DownloadKey,
                            FileName = unfinishedItem.FileName,
                            FileLink = unfinishedItem.FileLink,
                            FilePath = unfinishedItem.FilePath,
                            FileSHA1 = unfinishedItem.FileSHA1,
                            TotalSize = unfinishedItem.TotalSize
                        });

                        if (ContinueResult)
                        {
                            while (isUpdatingNow) await Task.Delay(50);
                            lock (UnfinishedDataListLock) isUpdatingNow = true;

                            UnfinishedDataList.Remove(unfinishedItem);

                            lock (UnfinishedDataListLock) isUpdatingNow = false;
                        }
                    }
                }
            };

            DeleteCommand.ExecuteRequested += async (sender, args) =>
            {
                UnfinishedModel unfinishedItem = args.Parameter as UnfinishedModel;
                if (unfinishedItem is not null)
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
                        LogService.WriteLog(LogType.WARNING, "Delete unfinished download file failed.", e);
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
                        LogService.WriteLog(LogType.WARNING, "Delete unfinished download information file failed.", e);
                    }

                    // 删除记录
                    while (isUpdatingNow) await Task.Delay(50);
                    lock (UnfinishedDataListLock) isUpdatingNow = true;

                    try
                    {
                        bool DeleteResult = await DownloadXmlService.DeleteAsync(unfinishedItem.DownloadKey);

                        if (DeleteResult)
                        {
                            UnfinishedDataList.Remove(unfinishedItem);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LogType.WARNING, "Delete unfinished download record failed.", e);
                    }

                    lock (UnfinishedDataListLock) isUpdatingNow = false;
                }
            };

            // 订阅事件
            DownloadSchedulerService.DownloadingList.ItemsChanged += OnDownloadingListItemsChanged;
        }

        /// <summary>
        /// 在多选模式下点击项目选择相应的条目
        /// </summary>
        public async void OnItemClicked(object sender, ItemClickEventArgs args)
        {
            UnfinishedModel resultItem = (UnfinishedModel)args.ClickedItem;
            int ClickedIndex = UnfinishedDataList.IndexOf(resultItem);

            while (isUpdatingNow) await Task.Delay(50);
            lock (UnfinishedDataListLock) isUpdatingNow = true;

            if (ClickedIndex is not >= 0 && ClickedIndex < UnfinishedDataList.Count)
            {
                UnfinishedDataList[ClickedIndex].IsSelected = !UnfinishedDataList[ClickedIndex].IsSelected;
            }

            lock (UnfinishedDataListLock) isUpdatingNow = false;
        }

        /// <summary>
        /// 订阅事件，下载中列表内容有暂停下载或下载失败的项目时通知UI更改
        /// </summary>
        public void OnDownloadingListItemsChanged(object sender, ItemsChangedEventArgs<BackgroundModel> args)
        {
            if (args.RemovedItems.Any(item => item.DownloadFlag is 0 || item.DownloadFlag is 2))
            {
                Program.ApplicationRoot.MainWindow.DispatcherQueue.TryEnqueue(async () =>
                {
                    while (isUpdatingNow) await Task.Delay(50);
                    lock (UnfinishedDataListLock) isUpdatingNow = true;

                    foreach (BackgroundModel backgroundItem in args.RemovedItems)
                    {
                        if (backgroundItem.DownloadFlag is 0 || backgroundItem.DownloadFlag is 2)
                        {
                            BackgroundModel item = await DownloadXmlService.QueryWithKeyAsync(backgroundItem.DownloadKey);

                            UnfinishedDataList.Add(new UnfinishedModel
                            {
                                DownloadKey = item.DownloadKey,
                                FileName = item.FileName,
                                FilePath = item.FilePath,
                                FileLink = item.FileLink,
                                FileSHA1 = item.FileSHA1,
                                TotalSize = item.TotalSize,
                                DownloadFlag = item.DownloadFlag
                            });
                        }
                    }

                    lock (UnfinishedDataListLock) isUpdatingNow = false;
                });
            }
        }

        /// <summary>
        /// 从数据库中加载未下载完成和下载失败的数据
        /// </summary>
        public async Task GetUnfinishedDataListAsync()
        {
            List<BackgroundModel> FailureDownloadRawList = await DownloadXmlService.QueryWithFlagAsync(0);

            List<BackgroundModel> PauseDownloadRawList = await DownloadXmlService.QueryWithFlagAsync(2);

            while (isUpdatingNow) await Task.Delay(50);
            lock (UnfinishedDataListLock) isUpdatingNow = true;

            UnfinishedDataList.Clear();

            PauseDownloadRawList.ForEach(downloadItem =>
                {
                    UnfinishedDataList.Add(new UnfinishedModel
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

            FailureDownloadRawList.ForEach(downloadItem =>
            {
                UnfinishedDataList.Add(new UnfinishedModel
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

            lock (UnfinishedDataListLock) isUpdatingNow = false;
        }
    }
}
