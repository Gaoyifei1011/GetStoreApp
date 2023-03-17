using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.DataType.Events;
using GetStoreApp.Extensions.Messaging;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Download;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.Services.Controls.Settings.Experiment;
using GetStoreApp.UI.Dialogs.Common;
using GetStoreApp.UI.Notifications;
using GetStoreApp.ViewModels.Base;
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

        // 打开默认保存的文件夹
        public IRelayCommand OpenFolderCommand => new RelayCommand(async () =>
        {
            await DownloadOptionsService.OpenFolderAsync(DownloadOptionsService.DownloadFolder);
        });

        // 继续下载全部任务
        public IRelayCommand ContinueAllCommand => new RelayCommand(async () =>
        {
            // 查看是否开启了网络监控服务
            if (NetWorkMonitorService.NetWorkMonitorValue)
            {
                NetWorkStatus NetStatus = NetWorkHelper.GetNetWorkStatus();

                // 网络处于未连接状态，不再进行下载，显示通知
                if (NetStatus is NetWorkStatus.None || NetStatus is NetWorkStatus.Unknown)
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
                    catch (Exception)
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }
            }

            lock (UnfinishedDataListLock) isUpdatingNow = false;
        });

        // 进入多选模式
        public IRelayCommand SelectCommand => new RelayCommand(async () =>
        {
            while (isUpdatingNow) await Task.Delay(50);
            lock (UnfinishedDataListLock) isUpdatingNow = true;

            foreach (UnfinishedModel unfinishedItem in UnfinishedDataList)
            {
                unfinishedItem.IsSelected = false;
            }

            IsSelectMode = true;
            lock (UnfinishedDataListLock) isUpdatingNow = false;
        });

        // 全部选择
        public IRelayCommand SelectAllCommand => new RelayCommand(async () =>
        {
            while (isUpdatingNow) await Task.Delay(50);
            lock (UnfinishedDataListLock) isUpdatingNow = true;

            foreach (UnfinishedModel unfinishedItem in UnfinishedDataList)
            {
                unfinishedItem.IsSelected = true;
            }

            lock (UnfinishedDataListLock) isUpdatingNow = false;
        });

        // 全部不选
        public IRelayCommand SelectNoneCommand => new RelayCommand(async () =>
        {
            while (isUpdatingNow) await Task.Delay(50);
            lock (UnfinishedDataListLock) isUpdatingNow = true;

            foreach (UnfinishedModel unfinishedItem in UnfinishedDataList)
            {
                unfinishedItem.IsSelected = false;
            }

            lock (UnfinishedDataListLock) isUpdatingNow = false;
        });

        // 删除选中的任务
        public IRelayCommand DeleteSelectedCommand => new RelayCommand(async () =>
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
                catch (Exception) { }

                // 删除Aria2后缀下载信息记录文件
                try
                {
                    if (File.Exists(string.Format("{0}.{1}", backgroundItem.FilePath, "aria2")))
                    {
                        File.Delete(string.Format("{0}.{1}", backgroundItem.FilePath, "aria2"));
                    }
                }
                catch (Exception) { }

                // 删除记录
                try
                {
                    bool DeleteResult = await DownloadXmlService.DeleteAsync(backgroundItem.DownloadKey);

                    if (DeleteResult)
                    {
                        UnfinishedDataList.Remove(UnfinishedDataList.First(item => item.DownloadKey == item.DownloadKey));
                    }
                }
                catch (Exception) { }
            }

            lock (UnfinishedDataListLock) isUpdatingNow = false;
        });

        // 退出多选模式
        public IRelayCommand CancelCommand => new RelayCommand(() =>
        {
            IsSelectMode = false;
        });

        // 继续下载当前任务
        public IRelayCommand ContinueCommand => new RelayCommand<UnfinishedModel>(async (unfinishedItem) =>
        {
            // 查看是否开启了网络监控服务
            if (NetWorkMonitorService.NetWorkMonitorValue)
            {
                NetWorkStatus NetStatus = NetWorkHelper.GetNetWorkStatus();

                // 网络处于未连接状态，不再进行下载，显示通知
                if (NetStatus is NetWorkStatus.None || NetStatus is NetWorkStatus.Unknown)
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
        });

        // 删除当前任务
        public IRelayCommand DeleteCommand => new RelayCommand<UnfinishedModel>(async (unfinishedItem) =>
        {
            // 删除下载文件
            try
            {
                if (File.Exists(unfinishedItem.FilePath))
                {
                    File.Delete(unfinishedItem.FilePath);
                }
            }
            catch (Exception) { }

            // 删除Aria2后缀下载信息记录文件
            try
            {
                if (File.Exists(string.Format("{0}.{1}", unfinishedItem.FilePath, "aria2")))
                {
                    File.Delete(string.Format("{0}.{1}", unfinishedItem.FilePath, "aria2"));
                }
            }
            catch (Exception) { }

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
            catch (Exception) { }

            lock (UnfinishedDataListLock) isUpdatingNow = false;
        });

        public UnfinishedViewModel()
        {
            Messenger.Default.Register<int>(this, MessageToken.PivotSelection, async (pivotSelectionMessage) =>
            {
                // 切换到已完成页面时，更新当前页面的数据
                if (pivotSelectionMessage is 1)
                {
                    await GetUnfinishedDataListAsync();
                }

                // 从下载页面离开时，关闭所有事件。
                else if (pivotSelectionMessage is -1)
                {
                    // 取消订阅所有事件
                    DownloadSchedulerService.DownloadingList.ItemsChanged -= OnDownloadingListItemsChanged;
                }
            });

            // 订阅事件
            DownloadSchedulerService.DownloadingList.ItemsChanged += OnDownloadingListItemsChanged;
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
        private void OnDownloadingListItemsChanged(object sender, ItemsChangedEventArgs<BackgroundModel> args)
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
        private async Task GetUnfinishedDataListAsync()
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
