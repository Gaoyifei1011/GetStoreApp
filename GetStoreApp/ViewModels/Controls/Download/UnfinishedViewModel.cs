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
    public sealed class UnfinishedViewModel : ViewModelBase
    {
        // 临界区资源访问互斥锁
        private readonly object UnfinishedDataListLock = new object();

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

            foreach (BackgroundModel item in PauseList)
            {
                bool ContinueResult = await DownloadSchedulerService.ContinueTaskAsync(item);

                if (ContinueResult)
                {
                    lock (UnfinishedDataListLock)
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
                }
                else
                {
                    continue;
                }
            }
        });

        // 进入多选模式
        public IRelayCommand SelectCommand => new RelayCommand(() =>
        {
            lock (UnfinishedDataListLock)
            {
                foreach (UnfinishedModel unfinishedItem in UnfinishedDataList)
                {
                    unfinishedItem.IsSelected = false;
                }
            }

            IsSelectMode = true;
        });

        // 全部选择
        public IRelayCommand SelectAllCommand => new RelayCommand(() =>
        {
            lock (UnfinishedDataListLock)
            {
                foreach (UnfinishedModel unfinishedItem in UnfinishedDataList)
                {
                    unfinishedItem.IsSelected = true;
                }
            }
        });

        // 全部不选
        public IRelayCommand SelectNoneCommand => new RelayCommand(() =>
        {
            lock (UnfinishedDataListLock)
            {
                foreach (UnfinishedModel unfinishedItem in UnfinishedDataList)
                {
                    unfinishedItem.IsSelected = false;
                }
            }
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
                if (!Program.ApplicationRoot.IsDialogOpening)
                {
                    Program.ApplicationRoot.IsDialogOpening = true;
                    await new SelectEmptyPromptDialog().ShowAsync();
                    Program.ApplicationRoot.IsDialogOpening = false;
                }
                return;
            }

            IsSelectMode = false;

            foreach (BackgroundModel backgroundItem in SelectedUnfinishedDataList)
            {
                try
                {
                    // 删除文件
                    string tempFilePath = backgroundItem.FilePath;
                    string tempFileAria2Path = string.Format("{0}.{1}", backgroundItem.FilePath, "aria2");

                    if (File.Exists(tempFilePath))
                    {
                        File.Delete(tempFilePath);
                    }

                    if (File.Exists(tempFileAria2Path))
                    {
                        File.Delete(tempFileAria2Path);
                    }

                    // 删除记录
                    bool DeleteResult = await DownloadDBService.DeleteAsync(backgroundItem.DownloadKey);

                    if (DeleteResult)
                    {
                        lock (UnfinishedDataListLock)
                        {
                            UnfinishedDataList.Remove(UnfinishedDataList.First(item => item.DownloadKey == item.DownloadKey));
                        }
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }
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
                    lock (UnfinishedDataList)
                    {
                        UnfinishedDataList.Remove(unfinishedItem);
                    }
                }
            }
        });

        // 删除当前任务
        public IRelayCommand DeleteCommand => new RelayCommand<UnfinishedModel>(async (unfinishedItem) =>
        {
            try
            {
                // 删除文件
                string tempFilePath = unfinishedItem.FilePath;
                string tempFileAria2Path = string.Format("{0}.{1}", unfinishedItem.FilePath, "aria2");

                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }

                if (File.Exists(tempFileAria2Path))
                {
                    File.Delete(tempFileAria2Path);
                }

                bool DeleteResult = await DownloadDBService.DeleteAsync(unfinishedItem.DownloadKey);

                if (DeleteResult)
                {
                    lock (UnfinishedDataListLock)
                    {
                        UnfinishedDataList.Remove(unfinishedItem);
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
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
        public void OnItemClick(object sender, ItemClickEventArgs args)
        {
            UnfinishedModel resultItem = (UnfinishedModel)args.ClickedItem;
            int ClickedIndex = UnfinishedDataList.IndexOf(resultItem);

            lock (UnfinishedDataListLock)
            {
                UnfinishedDataList[ClickedIndex].IsSelected = !UnfinishedDataList[ClickedIndex].IsSelected;
            }
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
                    foreach (BackgroundModel backgroundItem in args.RemovedItems)
                    {
                        if (backgroundItem.DownloadFlag is 0 || backgroundItem.DownloadFlag is 2)
                        {
                            BackgroundModel item = await DownloadDBService.QueryWithKeyAsync(backgroundItem.DownloadKey);

                            lock (UnfinishedDataListLock)
                            {
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
                    }
                });
            }
        }

        /// <summary>
        /// 从数据库中加载未下载完成和下载失败的数据
        /// </summary>
        private async Task GetUnfinishedDataListAsync()
        {
            List<BackgroundModel> FailureDownloadRawList = await DownloadDBService.QueryWithFlagAsync(0);

            List<BackgroundModel> PauseDownloadRawList = await DownloadDBService.QueryWithFlagAsync(2);

            lock (UnfinishedDataListLock)
            {
                UnfinishedDataList.Clear();
            }

            lock (UnfinishedDataListLock)
            {
                foreach (BackgroundModel downloadItem in PauseDownloadRawList)
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
                }

                foreach (BackgroundModel downloadItem in FailureDownloadRawList)
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
                }
            }
        }
    }
}
