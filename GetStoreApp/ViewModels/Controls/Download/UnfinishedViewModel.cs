using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI;
using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Extensions.Enum;
using GetStoreApp.Extensions.Event;
using GetStoreApp.Helpers;
using GetStoreApp.Messages;
using GetStoreApp.Models.Download;
using GetStoreApp.Models.Notification;
using GetStoreApp.UI.Dialogs;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Controls.Download
{
    public class UnfinishedViewModel : ObservableRecipient
    {
        // 临界区资源访问互斥锁
        private readonly object UnfinishedDataListLock = new object();

        private DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        private IDownloadDBService DownloadDBService { get; } = IOCHelper.GetService<IDownloadDBService>();

        private IDownloadSchedulerService DownloadSchedulerService { get; } = IOCHelper.GetService<IDownloadSchedulerService>();

        private IDownloadOptionsService DownloadOptionsService { get; } = IOCHelper.GetService<IDownloadOptionsService>();

        public ObservableCollection<UnfinishedModel> UnfinishedDataList { get; } = new ObservableCollection<UnfinishedModel>();

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

        // 继续下载全部任务
        public IAsyncRelayCommand ContinueAllCommand => new AsyncRelayCommand(async () =>
        {
            NetWorkStatus NetStatus = NetWorkHelper.GetNetWorkStatus();

            // 网络处于未连接状态，不再进行下载，显示通知
            if (NetStatus == NetWorkStatus.None || NetStatus == NetWorkStatus.Unknown)
            {
                WeakReferenceMessenger.Default.Send(new InAppNotificationMessage(new InAppNotificationModel
                {
                    NotificationContent = "NetWorkError",
                }));
                return;
            }

            List<BackgroundModel> PauseList = new List<BackgroundModel>();

            foreach (UnfinishedModel unfinishedItem in UnfinishedDataList.Where(item => item.DownloadFlag == 2))
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
        public IAsyncRelayCommand SelectCommand => new AsyncRelayCommand(async () =>
        {
            lock (UnfinishedDataListLock)
            {
                foreach (UnfinishedModel unfinishedItem in UnfinishedDataList)
                {
                    unfinishedItem.IsSelected = false;
                }
            }

            IsSelectMode = true;
            await Task.CompletedTask;
        });

        // 全部选择
        public IAsyncRelayCommand SelectAllCommand => new AsyncRelayCommand(async () =>
        {
            lock (UnfinishedDataListLock)
            {
                foreach (UnfinishedModel unfinishedItem in UnfinishedDataList)
                {
                    unfinishedItem.IsSelected = true;
                }
            }

            await Task.CompletedTask;
        });

        // 全部不选
        public IAsyncRelayCommand SelectNoneCommand => new AsyncRelayCommand(async () =>
        {
            lock (UnfinishedDataListLock)
            {
                foreach (UnfinishedModel unfinishedItem in UnfinishedDataList)
                {
                    unfinishedItem.IsSelected = false;
                }
            }

            await Task.CompletedTask;
        });

        // 删除选中的任务
        public IAsyncRelayCommand DeleteSelectedCommand => new AsyncRelayCommand(async () =>
        {
            List<BackgroundModel> SelectedUnfinishedDataList = new List<BackgroundModel>();

            foreach (UnfinishedModel unfinishedItem in UnfinishedDataList.Where(item => item.IsSelected == true))
            {
                SelectedUnfinishedDataList.Add(new BackgroundModel
                {
                    DownloadKey = unfinishedItem.DownloadKey,
                    FilePath = unfinishedItem.FilePath
                });
            }

            // 没有选中任何内容时显示空提示对话框
            if (SelectedUnfinishedDataList.Count == 0)
            {
                await new SelectEmptyPromptDialog().ShowAsync();
                return;
            }

            IsSelectMode = false;

            foreach (BackgroundModel item in SelectedUnfinishedDataList)
            {
                try
                {
                    // 删除文件
                    string tempFilePath = item.FilePath;
                    string tempFileAria2Path = string.Format("{0}.{1}", item.FilePath, "Aria2");

                    if (File.Exists(tempFilePath))
                    {
                        File.Delete(tempFilePath);
                    }

                    if (File.Exists(tempFileAria2Path))
                    {
                        File.Delete(tempFileAria2Path);
                    }

                    // 删除记录
                    bool DeleteResult = await DownloadDBService.DeleteAsync(item.DownloadKey);

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
        public IAsyncRelayCommand CancelCommand => new AsyncRelayCommand(async () =>
        {
            IsSelectMode = false;
            await Task.CompletedTask;
        });

        // 在多选模式下点击项目选择相应的条目
        public IAsyncRelayCommand ItemClickCommand => new AsyncRelayCommand<ItemClickEventArgs>(async (param) =>
        {
            UnfinishedModel resultItem = (UnfinishedModel)param.ClickedItem;
            int ClickedIndex = UnfinishedDataList.IndexOf(resultItem);

            UnfinishedDataList[ClickedIndex].IsSelected = !UnfinishedDataList[ClickedIndex].IsSelected;

            await Task.CompletedTask;
        });

        // 继续下载当前任务
        public IAsyncRelayCommand ContinueCommand => new AsyncRelayCommand<UnfinishedModel>(async (param) =>
        {
            NetWorkStatus NetStatus = NetWorkHelper.GetNetWorkStatus();

            // 网络处于未连接状态，不再进行下载，显示通知
            if (NetStatus == NetWorkStatus.None || NetStatus == NetWorkStatus.Unknown)
            {
                WeakReferenceMessenger.Default.Send(new InAppNotificationMessage(new InAppNotificationModel
                {
                    NotificationContent = "NetWorkError",
                }));
                return;
            }

            if (param.DownloadFlag == 2)
            {
                bool ContinueResult = await DownloadSchedulerService.ContinueTaskAsync(new BackgroundModel
                {
                    DownloadKey = param.DownloadKey,
                    FileName = param.FileName,
                    FileLink = param.FileLink,
                    FilePath = param.FilePath,
                    FileSHA1 = param.FileSHA1,
                    TotalSize = param.TotalSize
                });

                if (ContinueResult)
                {
                    lock (UnfinishedDataList)
                    {
                        UnfinishedDataList.Remove(param);
                    }
                }
            }
        });

        // 删除当前任务
        public IAsyncRelayCommand DeleteCommand => new AsyncRelayCommand<UnfinishedModel>(async (param) =>
        {
            bool DeleteResult = await DownloadDBService.DeleteAsync(param.DownloadKey);

            if (DeleteResult)
            {
                lock (UnfinishedDataListLock)
                {
                    UnfinishedDataList.Remove(param);
                }
            }
        });

        public UnfinishedViewModel()
        {
            WeakReferenceMessenger.Default.Register<UnfinishedViewModel, PivotSelectionMessage>(this, async (unfinishedViewModel, pivotSelectionMessage) =>
            {
                // 切换到已完成页面时，更新当前页面的数据
                if (pivotSelectionMessage.Value == 1)
                {
                    await GetUnfinishedDataListAsync();
                }

                // 从下载页面离开时，关闭所有事件。并注销所有消息服务
                else if (pivotSelectionMessage.Value == -1)
                {
                    // 取消订阅所有事件
                    DownloadSchedulerService.DownloadingList.ItemsChanged -= DownloadingListItemsChanged;

                    Messenger.UnregisterAll(this);
                }

                await Task.CompletedTask;
            });

            // 订阅事件
            DownloadSchedulerService.DownloadingList.ItemsChanged += DownloadingListItemsChanged;
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

        /// <summary>
        /// 订阅事件，下载中列表内容有暂停下载或下载失败的项目时通知UI更改
        /// </summary>
        private async void DownloadingListItemsChanged(object sender, ItemsChangedEventArgs<BackgroundModel> args)
        {
            if (args.RemovedItems.Any(item => item.DownloadFlag == 0 || item.DownloadFlag == 2))
            {
                await dispatcherQueue.EnqueueAsync(async () =>
                {
                    foreach (BackgroundModel backgroundItem in args.RemovedItems)
                    {
                        if (backgroundItem.DownloadFlag == 0 || backgroundItem.DownloadFlag == 2)
                        {
                            BackgroundModel item = await DownloadDBService.QueryWithKeyAsync(backgroundItem.DownloadKey);

                            lock (UnfinishedDataListLock)
                            {
                                UnfinishedDataList.Insert(0, new UnfinishedModel
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
    }
}
