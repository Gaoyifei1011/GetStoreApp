using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI;
using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Extensions.Enum;
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
using Windows.Management.Deployment;
using Windows.Storage;

namespace GetStoreApp.ViewModels.Controls.Download
{
    public class CompletedViewModel : ObservableRecipient
    {
        // 临界区资源访问互斥锁
        private readonly object CompletedDataListLock = new object();

        // 获取UI主线程
        private DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        private IDownloadDBService DownloadDBService { get; } = IOCHelper.GetService<IDownloadDBService>();

        private IAppNotificationService AppNotificationService { get; } = IOCHelper.GetService<IAppNotificationService>();

        private IDownloadSchedulerService DownloadSchedulerService { get; } = IOCHelper.GetService<IDownloadSchedulerService>();

        private IDownloadOptionsService DownloadOptionsService { get; } = IOCHelper.GetService<IDownloadOptionsService>();

        private IInstallModeService InstallModeService { get; } = IOCHelper.GetService<IInstallModeService>();

        public ObservableCollection<CompletedModel> CompletedDataList { get; } = new ObservableCollection<CompletedModel>();

        private bool _isSelectMode = false;

        public bool IsSelectMode
        {
            get { return _isSelectMode; }

            set { SetProperty(ref _isSelectMode, value); }
        }

        // 打开默认保存的文件夹
        public IRelayCommand OpenFolderCommand => new RelayCommand(async () =>
        {
            await DownloadOptionsService.OpenFolderAsync(DownloadOptionsService.DownloadFolder);
        });

        // 进入多选模式
        public IRelayCommand SelectCommand => new RelayCommand(() =>
        {
            lock (CompletedDataListLock)
            {
                foreach (CompletedModel completedItem in CompletedDataList)
                {
                    completedItem.IsSelected = false;
                }
            }

            IsSelectMode = true;
        });

        // 全部选择
        public IRelayCommand SelectAllCommand => new RelayCommand(() =>
        {
            lock (CompletedDataListLock)
            {
                foreach (CompletedModel completedItem in CompletedDataList)
                {
                    completedItem.IsSelected = true;
                }
            }
        });

        // 全部不选
        public IRelayCommand SelectNoneCommand => new RelayCommand(() =>
        {
            lock (CompletedDataListLock)
            {
                foreach (CompletedModel completedItem in CompletedDataList)
                {
                    completedItem.IsSelected = false;
                }
            }
        });

        // 删除选中的任务
        public IRelayCommand DeleteSelectedCommand => new RelayCommand(async () =>
        {
            List<BackgroundModel> SelectedCompletedDataList = new List<BackgroundModel>();

            foreach (CompletedModel completedItem in CompletedDataList.Where(item => item.IsSelected == true))
            {
                SelectedCompletedDataList.Add(new BackgroundModel
                {
                    DownloadKey = completedItem.DownloadKey,
                    IsInstalling = completedItem.IsInstalling
                });
            }

            // 没有选中任何内容时显示空提示对话框
            if (SelectedCompletedDataList.Count == 0)
            {
                await new SelectEmptyPromptDialog().ShowAsync();
                return;
            }

            // 当前任务正在安装时，不进行其他任何操作
            if (SelectedCompletedDataList.Exists(item => item.IsInstalling == true))
            {
                await new InstallingNotifyDialog().ShowAsync();
                return;
            }

            // 删除时显示删除确认对话框
            ContentDialogResult result = await new DeletePromptDialog(DeletePrompt.Download).ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                IsSelectMode = false;

                bool DeleteSelectedResult = await DownloadDBService.DeleteSelectedAsync(SelectedCompletedDataList);

                lock (CompletedDataListLock)
                {
                    foreach (BackgroundModel backgroundItem in SelectedCompletedDataList)
                    {
                        try
                        {
                            CompletedDataList.Remove(CompletedDataList.First(item => item.DownloadKey == backgroundItem.DownloadKey));
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
        public IRelayCommand DeleteSelectedWithFileCommand => new RelayCommand(async () =>
        {
            List<BackgroundModel> SelectedCompletedDataList = new List<BackgroundModel>();

            foreach (CompletedModel completedItem in CompletedDataList.Where(item => item.IsSelected == true))
            {
                SelectedCompletedDataList.Add(new BackgroundModel
                {
                    DownloadKey = completedItem.DownloadKey,
                    FilePath = completedItem.FilePath,
                    IsInstalling = completedItem.IsInstalling,
                });
            }

            // 没有选中任何内容时显示空提示对话框
            if (SelectedCompletedDataList.Count == 0)
            {
                await new SelectEmptyPromptDialog().ShowAsync();
                return;
            }

            // 当前任务正在安装时，不进行其他任何操作
            if (SelectedCompletedDataList.Exists(item => item.IsInstalling == true))
            {
                await new InstallingNotifyDialog().ShowAsync();
                return;
            }

            // 删除时显示删除确认对话框
            ContentDialogResult result = await new DeletePromptDialog(DeletePrompt.DownloadWithFile).ShowAsync();

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
        public IRelayCommand CancelCommand => new RelayCommand(() =>
        {
            IsSelectMode = false;
        });

        // 在多选模式下点击项目选择相应的条目
        public IRelayCommand ItemClickCommand => new RelayCommand<ItemClickEventArgs>((args) =>
        {
            CompletedModel completedItem = (CompletedModel)args.ClickedItem;
            int ClickedIndex = CompletedDataList.IndexOf(completedItem);

            lock (CompletedDataListLock)
            {
                CompletedDataList[ClickedIndex].IsSelected = !CompletedDataList[ClickedIndex].IsSelected;
            }
        });

        // 安装应用
        public IRelayCommand InstallCommand => new RelayCommand<CompletedModel>(async (completedItem) =>
        {
            // 使用应用安装程序安装
            if (!string.IsNullOrEmpty(completedItem.FilePath) && File.Exists(completedItem.FilePath))
            {
                if (InstallModeService.InstallMode.InternalName == InstallModeService.InstallModeList[0].InternalName)
                {
                    ProcessStartInfo Info = new ProcessStartInfo();
                    Info.FileName = completedItem.FilePath;
                    Info.UseShellExecute = true;

                    Process.Start(Info);
                }

                // 直接安装
                else if (InstallModeService.InstallMode.InternalName == InstallModeService.InstallModeList[1].InternalName)
                {
                    // 标记安装状态
                    int InstallIndex = CompletedDataList.IndexOf(CompletedDataList.First(item => item.DownloadKey == completedItem.DownloadKey));
                    CompletedDataList[InstallIndex].IsInstalling = true;

                    PackageManager packageManager = new PackageManager();

                    // 更新安装进度
                    Progress<DeploymentProgress> progressCallBack = new Progress<DeploymentProgress>((installProgress) =>
                    {
                        CompletedDataList[InstallIndex].InstallValue = installProgress.percentage;
                    });

                    try
                    {
                        // 安装目标应用
                        DeploymentResult InstallResult = await packageManager.AddPackageAsync(new Uri(completedItem.FilePath), null, DeploymentOptions.None).AsTask(progressCallBack);
                        // 显示安装成功通知
                        AppNotificationService.Show("InstallApp", "Successfully", Path.GetFileName(completedItem.FilePath));
                    }
                    // 安装失败显示失败信息
                    catch (Exception e)
                    {
                        CompletedDataList[InstallIndex].InstallError = true;
                        // 显示安装失败通知
                        AppNotificationService.Show("InstallApp", "Error", Path.GetFileName(completedItem.FilePath), e.Message);
                    }
                    // 恢复原来的安装信息显示（并延缓当前安装信息显示时间3秒）
                    finally
                    {
                        await Task.Delay(3000);
                        CompletedDataList[InstallIndex].IsInstalling = false;
                        CompletedDataList[InstallIndex].InstallError = false;
                    }
                }
            }
        });

        // 打开当前项目存储的文件夹
        public IRelayCommand OpenItemFolderCommand => new RelayCommand<string>(async (filePath) =>
        {
            if (filePath is not null)
            {
                filePath = filePath.Replace(@"\\", @"\");

                // 判断文件是否存在，文件存在则寻找对应的文件，不存在打开对应的目录；若目录不存在，则仅启动Explorer.exe进程，打开资源管理器的默认文件夹
                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    await DownloadOptionsService.OpenItemFolderAsync(filePath);
                }
                else
                {
                    string FileFolderPath = Path.GetDirectoryName(filePath);

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
        public IRelayCommand DeleteCommand => new RelayCommand<CompletedModel>(async (completedItem) =>
        {
            if (completedItem is not null)
            {
                if (completedItem.IsInstalling == true)
                {
                    await new InstallingNotifyDialog().ShowAsync();
                    return;
                }

                bool DeleteResult = await DownloadDBService.DeleteAsync(completedItem.DownloadKey);

                if (DeleteResult)
                {
                    lock (CompletedDataListLock)
                    {
                        CompletedDataList.Remove(completedItem);
                    }
                }
            }
        });

        // 删除当前任务
        public IRelayCommand DeleteWithFileCommand => new RelayCommand<CompletedModel>(async (completedItem) =>
        {
            if (completedItem is not null)
            {
                if (completedItem.IsInstalling == true)
                {
                    await new InstallingNotifyDialog().ShowAsync();
                    return;
                }

                try
                {
                    if (File.Exists(completedItem.FilePath))
                    {
                        File.Delete(completedItem.FilePath);
                    }

                    bool DeleteResult = await DownloadDBService.DeleteAsync(completedItem.DownloadKey);

                    if (DeleteResult)
                    {
                        lock (CompletedDataListLock)
                        {
                            CompletedDataList.Remove(completedItem);
                        }
                    }
                }
                catch (Exception)
                {
                    return;
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
                                CompletedDataList.Add(new CompletedModel
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
