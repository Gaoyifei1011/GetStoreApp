using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.DataType.Events;
using GetStoreApp.Extensions.Messaging;
using GetStoreApp.Models.Controls.Download;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings.Advanced;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.Common;
using GetStoreApp.UI.Dialogs.Download;
using GetStoreApp.ViewModels.Base;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Management.Deployment;
using Windows.Storage;
using Windows.System;

namespace GetStoreApp.ViewModels.Controls.Download
{
    /// <summary>
    /// 下载页面：已下载完成用户控件视图模型
    /// </summary>
    public sealed class CompletedViewModel : ViewModelBase
    {
        // 临界区资源访问互斥锁
        private readonly object CompletedDataListLock = new object();

        public ObservableCollection<CompletedModel> CompletedDataList { get; } = new ObservableCollection<CompletedModel>();

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

            foreach (CompletedModel completedItem in CompletedDataList.Where(item => item.IsSelected is true))
            {
                SelectedCompletedDataList.Add(new BackgroundModel
                {
                    DownloadKey = completedItem.DownloadKey,
                    IsInstalling = completedItem.IsInstalling
                });
            }

            // 没有选中任何内容时显示空提示对话框
            if (SelectedCompletedDataList.Count is 0)
            {
                await new SelectEmptyPromptDialog().ShowAsync();
                return;
            }

            // 当前任务正在安装时，不进行其他任何操作
            if (SelectedCompletedDataList.Exists(item => item.IsInstalling is true))
            {
                await new InstallingNotifyDialog().ShowAsync();
                return;
            }

            // 删除时显示删除确认对话框
            ContentDialogResult result = await new DeletePromptDialog(DeleteArgs.Download).ShowAsync();

            if (result is ContentDialogResult.Primary)
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

            foreach (CompletedModel completedItem in CompletedDataList.Where(item => item.IsSelected is true))
            {
                SelectedCompletedDataList.Add(new BackgroundModel
                {
                    DownloadKey = completedItem.DownloadKey,
                    FilePath = completedItem.FilePath,
                    IsInstalling = completedItem.IsInstalling,
                });
            }

            // 没有选中任何内容时显示空提示对话框
            if (SelectedCompletedDataList.Count is 0)
            {
                await new SelectEmptyPromptDialog().ShowAsync();
                return;
            }

            // 当前任务正在安装时，不进行其他任何操作
            if (SelectedCompletedDataList.Exists(item => item.IsInstalling is true))
            {
                await new InstallingNotifyDialog().ShowAsync();
                return;
            }

            // 删除时显示删除确认对话框
            ContentDialogResult result = await new DeletePromptDialog(DeleteArgs.DownloadWithFile).ShowAsync();

            if (result is ContentDialogResult.Primary)
            {
                IsSelectMode = false;

                foreach (BackgroundModel completedItem in SelectedCompletedDataList)
                {
                    // 删除文件
                    try
                    {
                        StorageFile CompletedDownloadFile = await StorageFile.GetFileFromPathAsync(completedItem.FilePath);
                        await CompletedDownloadFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                    }
                    finally { }

                    // 删除记录
                    try
                    {
                        bool DeleteResult = await DownloadDBService.DeleteAsync(completedItem.DownloadKey);

                        if (DeleteResult)
                        {
                            lock (CompletedDataListLock)
                            {
                                CompletedDataList.Remove(CompletedDataList.First(item => item.DownloadKey == item.DownloadKey));
                            }
                        }
                    }
                    finally { }
                }
            }
        });

        // 退出多选模式
        public IRelayCommand CancelCommand => new RelayCommand(() =>
        {
            IsSelectMode = false;
        });

        // 安装应用
        public IRelayCommand InstallCommand => new RelayCommand<CompletedModel>(async (completedItem) =>
        {
            // 使用应用安装程序安装
            if (!string.IsNullOrEmpty(completedItem.FilePath))
            {
                try
                {
                    StorageFile CompletedFile = await StorageFile.GetFileFromPathAsync(completedItem.FilePath);

                    if (InstallModeService.InstallMode.InternalName == InstallModeService.InstallModeList[0].InternalName)
                    {
                        await Launcher.LaunchFileAsync(CompletedFile);
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
                            AppNotificationService.Show("InstallApp", "Successfully", CompletedFile.Name);
                        }
                        // 安装失败显示失败信息
                        catch (Exception e)
                        {
                            CompletedDataList[InstallIndex].InstallError = true;
                            // 显示安装失败通知
                            AppNotificationService.Show("InstallApp", "TBPF_ERROR", CompletedFile.Name, e.Message);
                        }
                        // 恢复原来的安装信息显示（并延缓当前安装信息显示时间1秒）
                        finally
                        {
                            await Task.Delay(1000);
                            CompletedDataList[InstallIndex].IsInstalling = false;
                            CompletedDataList[InstallIndex].InstallError = false;
                        }
                    }
                }
                catch (Exception)
                {
                    return;
                }
            }
        });

        // 打开当前项目存储的文件夹
        public IRelayCommand OpenItemFolderCommand => new RelayCommand<string>(async (filePath) =>
        {
            if (filePath is not null)
            {
                filePath = filePath.Replace(@"\\", @"\");

                // 定位文件，若定位失败，则仅启动资源管理器并打开桌面目录
                if (!string.IsNullOrEmpty(filePath))
                {
                    try
                    {
                        StorageFile file = await StorageFile.GetFileFromPathAsync(filePath);
                        StorageFolder folder = await file.GetParentAsync();
                        FolderLauncherOptions options = new FolderLauncherOptions();
                        options.ItemsToSelect.Add(file);
                        await Launcher.LaunchFolderAsync(folder, options);
                    }
                    catch (Exception)
                    {
                        await Launcher.LaunchFolderPathAsync(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                    }
                }
                else
                {
                    await Launcher.LaunchFolderPathAsync(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                }
            }
        });

        // 删除当前任务
        public IRelayCommand DeleteCommand => new RelayCommand<CompletedModel>(async (completedItem) =>
        {
            if (completedItem is not null)
            {
                if (completedItem.IsInstalling is true)
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
                if (completedItem.IsInstalling is true)
                {
                    await new InstallingNotifyDialog().ShowAsync();
                    return;
                }

                // 删除文件
                try
                {
                    StorageFile CompletedDownloadFile = await StorageFile.GetFileFromPathAsync(completedItem.FilePath);
                    await CompletedDownloadFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                }
                finally { }

                // 删除记录
                try
                {
                    bool DeleteResult = await DownloadDBService.DeleteAsync(completedItem.DownloadKey);

                    if (DeleteResult)
                    {
                        lock (CompletedDataListLock)
                        {
                            CompletedDataList.Remove(completedItem);
                        }
                    }
                }
                finally { }
            }
        });

        // 查看文件信息
        public IRelayCommand FileInformationCommand => new RelayCommand<CompletedModel>(async (completedItem) =>
        {
            await new FileInformationDialog(completedItem).ShowAsync();
        });

        public CompletedViewModel()
        {
            Messenger.Default.Register<int>(this, MessageToken.PivotSelection, async (pivotSelectionMessage) =>
            {
                // 切换到已完成页面时，更新当前页面的数据
                if (pivotSelectionMessage is 2)
                {
                    await GetCompletedDataListAsync();
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
            CompletedModel completedItem = (CompletedModel)args.ClickedItem;
            int ClickedIndex = CompletedDataList.IndexOf(completedItem);

            lock (CompletedDataListLock)
            {
                CompletedDataList[ClickedIndex].IsSelected = !CompletedDataList[ClickedIndex].IsSelected;
            }
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
        private void OnDownloadingListItemsChanged(object sender, ItemsChangedEventArgs<BackgroundModel> args)
        {
            if (args.RemovedItems.Any(item => item.DownloadFlag is 4))
            {
                Program.ApplicationRoot.MainWindow.DispatcherQueue.TryEnqueue(async () =>
                {
                    foreach (BackgroundModel backgroundItem in args.RemovedItems)
                    {
                        if (backgroundItem.DownloadFlag is 4)
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
