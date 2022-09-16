using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI;
using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Extensions.Event;
using GetStoreApp.Helpers;
using GetStoreApp.Messages;
using GetStoreApp.Models.Download;
using GetStoreApp.UI.Dialogs;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace GetStoreApp.ViewModels.Controls.Download
{
    public class CompletedViewModel : ObservableRecipient
    {
        // 临界区资源访问互斥锁
        private readonly object CompletedDataListLock = new object();

        private DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        private IDownloadDBService DownloadDBService { get; } = IOCHelper.GetService<IDownloadDBService>();

        private IDownloadSchedulerService DownloadSchedulerService { get; } = IOCHelper.GetService<IDownloadSchedulerService>();

        private IDownloadOptionsService DownloadOptionsService { get; } = IOCHelper.GetService<IDownloadOptionsService>();

        public ObservableCollection<CompletedModel> CompletedDataList { get; } = new ObservableCollection<CompletedModel>();

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

        // 进入多选模式
        public IAsyncRelayCommand SelectCommand => new AsyncRelayCommand(async () =>
        {
            lock (CompletedDataListLock)
            {
                foreach (CompletedModel completedItem in CompletedDataList)
                {
                    completedItem.IsSelected = false;
                }
            }

            IsSelectMode = true;
            await Task.CompletedTask;
        });

        // 全部选择
        public IAsyncRelayCommand SelectAllCommand => new AsyncRelayCommand(async () =>
        {
            lock (CompletedDataListLock)
            {
                foreach (CompletedModel completedItem in CompletedDataList)
                {
                    completedItem.IsSelected = true;
                }
            }

            await Task.CompletedTask;
        });

        // 全部不选
        public IAsyncRelayCommand SelectNoneCommand => new AsyncRelayCommand(async () =>
        {
            lock (CompletedDataListLock)
            {
                foreach (CompletedModel completedItem in CompletedDataList)
                {
                    completedItem.IsSelected = false;
                }
            }

            await Task.CompletedTask;
        });

        // 删除选中的任务
        public IAsyncRelayCommand DeleteRecordCommand => new AsyncRelayCommand(async () =>
        {
            List<string> SelectedDownloadKeyList = new List<string>();

            foreach (CompletedModel completedItem in CompletedDataList.Where(item => item.IsSelected == true))
            {
                SelectedDownloadKeyList.Add(completedItem.DownloadKey);
            }

            // 没有选中任何内容时显示空提示对话框
            if (SelectedDownloadKeyList.Count == 0)
            {
                await new SelectEmptyPromptDialog().ShowAsync();
                return;
            }

            // 删除时显示删除确认对话框
            ContentDialogResult result = await new DeletePromptDialog().ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                IsSelectMode = false;

                bool DeleteSelectedResult = await DownloadDBService.DeleteSelectedAsync(SelectedDownloadKeyList);

                lock (CompletedDataListLock)
                {
                    foreach (string downloadKey in SelectedDownloadKeyList)
                    {
                        try
                        {
                            CompletedDataList.Remove(CompletedDataList.First(item => item.DownloadKey == downloadKey));
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                }
            }
        });

        // 删除选中的任务（包括文件）
        public IAsyncRelayCommand DeleteRecordWithFileCommand => new AsyncRelayCommand(async () =>
        {
            List<BackgroundModel> SelectedCompletedDataList = new List<BackgroundModel>();

            foreach (CompletedModel completedItem in CompletedDataList.Where(item => item.IsSelected == true))
            {
                SelectedCompletedDataList.Add(new BackgroundModel
                {
                    DownloadKey = completedItem.DownloadKey,
                    FilePath = completedItem.FilePath
                });
            }

            // 没有选中任何内容时显示空提示对话框
            if (SelectedCompletedDataList.Count == 0)
            {
                await new SelectEmptyPromptDialog().ShowAsync();
                return;
            }

            // 删除时显示删除确认对话框
            ContentDialogResult result = await new DeletePromptDialog("DeleteWithFile").ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                IsSelectMode = false;

                foreach (BackgroundModel item in SelectedCompletedDataList)
                {
                    // 删除文件
                    try
                    {
                        if (File.Exists(item.FilePath))
                        {
                            File.Delete(item.FilePath);
                        }

                        // 删除记录
                        bool DeleteResult = await DownloadDBService.DeleteAsync(item.DownloadKey);

                        if (DeleteResult)
                        {
                            lock (CompletedDataListLock)
                            {
                                CompletedDataList.Remove(CompletedDataList.First(item => item.DownloadKey == item.DownloadKey));
                            }
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
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
            CompletedModel completedItem = (CompletedModel)param.ClickedItem;
            int ClickedIndex = CompletedDataList.IndexOf(completedItem);

            CompletedDataList[ClickedIndex].IsSelected = !CompletedDataList[ClickedIndex].IsSelected;

            await Task.CompletedTask;
        });

        // 安装应用
        public IAsyncRelayCommand InstallCommand => new AsyncRelayCommand<string>(async (param) =>
        {
            if (param is not null)
            {
                // TODO:添加安装操作
                await Task.CompletedTask;
            }
        });

        // 打开当前项目存储的文件夹
        public IAsyncRelayCommand OpenItemFolderCommand => new AsyncRelayCommand<string>(async (param) =>
        {
            if (param is not null)
            {
                param = param.Replace(@"\\", @"\");

                // 判断文件是否存在，文件存在则寻找对应的文件，不存在打开对应的目录；若目录不存在，则仅启动Explorer.exe进程，打开资源管理器的默认文件夹
                if (!string.IsNullOrEmpty(param) && File.Exists(param))
                {
                    await DownloadOptionsService.OpenItemFolderAsync(param);
                }
                else
                {
                    string FileFolderPath = Path.GetDirectoryName(param);

                    if (Directory.Exists(FileFolderPath))
                    {
                        await Windows.System.Launcher.LaunchFolderAsync(await StorageFolder.GetFolderFromPathAsync(FileFolderPath));
                    }
                    else
                    {
                        Process.Start("explorer.exe");
                    }
                }
            }
        });

        // 删除当前任务
        public IAsyncRelayCommand DeleteCommand => new AsyncRelayCommand<CompletedModel>(async (param) =>
        {
            if (param is not null)
            {
                bool DeleteResult = await DownloadDBService.DeleteAsync(param.DownloadKey);

                if (DeleteResult)
                {
                    lock (CompletedDataListLock)
                    {
                        CompletedDataList.Remove(param);
                    }
                }
            }
        });

        public CompletedViewModel()
        {
            WeakReferenceMessenger.Default.Register<CompletedViewModel, PivotSelectionMessage>(this, async (completedViewModel, pivotSelectionMessage) =>
            {
                // 切换到已完成页面时，更新当前页面的数据
                if (pivotSelectionMessage.Value == 2)
                {
                    await GetCompletedDataListAsync();
                }

                // 从下载页面离开时，关闭所有事件。并注销所有消息服务
                else if (pivotSelectionMessage.Value == -1)
                {
                    // 取消订阅所有事件
                    DownloadSchedulerService.DownloadingList.ItemsChanged -= DownloadingListItemsChanged;

                    // 关闭消息服务
                    Messenger.UnregisterAll(this);
                }
                await Task.CompletedTask;
            });

            // 订阅事件
            DownloadSchedulerService.DownloadingList.ItemsChanged += DownloadingListItemsChanged;
        }

        /// <summary>
        /// 从数据库中加载已下载完成的数据
        /// </summary>
        private async Task GetCompletedDataListAsync()
        {
            List<BackgroundModel> DownloadRawList = await DownloadDBService.QueryWithFlagAsync(4);

            lock (CompletedDataListLock)
            {
                CompletedDataList.Clear();
            }

            lock (CompletedDataListLock)
            {
                foreach (BackgroundModel downloadRawData in DownloadRawList)
                {
                    CompletedDataList.Add(new CompletedModel
                    {
                        DownloadKey = downloadRawData.DownloadKey,
                        FileName = downloadRawData.FileName,
                        FileLink = downloadRawData.FileLink,
                        FilePath = downloadRawData.FilePath,
                        FileSHA1 = downloadRawData.FileSHA1,
                        TotalSize = downloadRawData.TotalSize,
                        DownloadFlag = downloadRawData.DownloadFlag
                    });
                }
            }
        }

        /// <summary>
        /// 订阅事件，下载中列表内容有完成项目时通知UI更改
        /// </summary>
        private async void DownloadingListItemsChanged(object sender, ItemsChangedEventArgs<BackgroundModel> args)
        {
            if (args.RemovedItems.Any(item => item.DownloadFlag == 4))
            {
                await dispatcherQueue.EnqueueAsync(async () =>
                {
                    foreach (BackgroundModel backgroundItem in args.RemovedItems)
                    {
                        if (backgroundItem.DownloadFlag == 4)
                        {
                            BackgroundModel item = await DownloadDBService.QueryWithKeyAsync(backgroundItem.DownloadKey);

                            lock (CompletedDataListLock)
                            {
                                CompletedDataList.Insert(0, new CompletedModel
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
                    }
                });
            }
        }
    }
}
