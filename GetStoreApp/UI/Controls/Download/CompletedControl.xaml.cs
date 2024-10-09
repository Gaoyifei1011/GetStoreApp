using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Controls.Extensions;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Download;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.Common;
using GetStoreApp.UI.Dialogs.Download;
using GetStoreApp.UI.TeachingTips;
using GetStoreApp.Views.Windows;
using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation.Diagnostics;
using Windows.Management.Deployment;
using Windows.Storage;
using Windows.System;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.UI.Controls.Download
{
    /// <summary>
    /// 下载页面：已下载完成控件
    /// </summary>
    public sealed partial class CompletedControl : Grid, INotifyPropertyChanged
    {
        private readonly PackageManager packageManager = new();

        private string CompletedCountInfo { get; } = ResourceService.GetLocalized("Download/CompletedCountInfo");

        private bool _isSelectMode;

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

        private ObservableCollection<CompletedModel> CompletedCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public CompletedControl()
        {
            InitializeComponent();

            Task.Run(() =>
            {
                GlobalNotificationService.ApplicationExit += OnApplicationExit;
                DownloadStorageService.DownloadStorageSemaphoreSlim?.Wait();
                List<DownloadSchedulerModel> downloadStorageList = DownloadStorageService.GetDownloadData();
                AutoResetEvent autoResetEvent = new(false);

                DispatcherQueue.TryEnqueue(() =>
                {
                    foreach (DownloadSchedulerModel downloadSchedulerItem in downloadStorageList)
                    {
                        CompletedCollection.Add(new CompletedModel()
                        {
                            DownloadKey = downloadSchedulerItem.DownloadKey,
                            FileName = downloadSchedulerItem.FileName,
                            FilePath = downloadSchedulerItem.FilePath,
                            FileLink = downloadSchedulerItem.FileLink,
                            TotalSize = downloadSchedulerItem.TotalSize,
                            IsNotOperated = true,
                            IsSelected = false,
                            IsSelectMode = false
                        });
                    }

                    autoResetEvent.Set();
                });

                autoResetEvent.WaitOne();
                autoResetEvent.Dispose();

                DownloadStorageService.StorageDataAdded += OnStorageDataAdded;
                DownloadStorageService.StorageDataDeleted += OnStorageDataDeleted;
                DownloadStorageService.StorageDataCleared += OnStorageDataCleared;

                DownloadStorageService.DownloadStorageSemaphoreSlim?.Release();
            });
        }

        #region 第一部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 删除当前任务
        /// </summary>
        private async void OnDeleteExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is CompletedModel completedItem)
            {
                if (completedItem.IsInstalling is true)
                {
                    await ContentDialogHelper.ShowAsync(new InstallingNotifyDialog(), this);
                    return;
                }

                foreach (CompletedModel item in CompletedCollection)
                {
                    if (item.DownloadKey.Equals(item.DownloadKey))
                    {
                        item.IsNotOperated = false;
                        break;
                    }
                }

                await Task.Run(() =>
                {
                    DownloadStorageService.DeleteDownloadData(completedItem.DownloadKey);
                });
            }
        }

        /// <summary>
        /// 删除当前任务（包括文件）
        /// </summary>
        private async void OnDeleteWithFileExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is CompletedModel completedItem)
            {
                if (completedItem.IsInstalling is true)
                {
                    await ContentDialogHelper.ShowAsync(new InstallingNotifyDialog(), this);
                    return;
                }

                foreach (CompletedModel item in CompletedCollection)
                {
                    if (item.DownloadKey.Equals(item.DownloadKey))
                    {
                        item.IsNotOperated = false;
                        break;
                    }
                }

                await Task.Run(() =>
                {
                    // 删除文件
                    try
                    {
                        if (File.Exists(completedItem.FilePath))
                        {
                            File.Delete(completedItem.FilePath);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Warning, "Delete completed download file failed.", e);
                    }

                    DownloadStorageService.DeleteDownloadData(completedItem.DownloadKey);
                });
            }
        }

        /// <summary>
        /// 查看文件信息
        /// </summary>
        private async void OnFileInformationExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is CompletedModel completedItem && File.Exists(completedItem.FilePath))
            {
                await ContentDialogHelper.ShowAsync(new FileInformationDialog(completedItem), this);
            }
            else
            {
                await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.FileLost));
            }
        }

        /// <summary>
        /// 安装应用
        /// </summary>
        private async void OnInstallExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is CompletedModel completedItem && File.Exists(completedItem.FilePath))
            {
                await Task.Run(async () =>
                {
                    // 普通应用：直接安装
                    if (completedItem.FilePath.EndsWith(".exe") || completedItem.FileName.EndsWith(".msi"))
                    {
                        Shell32Library.ShellExecute(IntPtr.Zero, "open", completedItem.FilePath, string.Empty, null, WindowShowStyle.SW_SHOWNORMAL);
                    }
                    // 商店打包应用：使用应用安装程序安装或直接安装
                    else
                    {
                        try
                        {
                            StorageFile completedFile = await StorageFile.GetFileFromPathAsync(completedItem.FilePath);

                            if (InstallModeService.InstallMode.Equals(InstallModeService.InstallModeList[0]))
                            {
                                await Launcher.LaunchFileAsync(completedFile);
                            }

                            // 直接安装
                            else if (InstallModeService.InstallMode.Equals(InstallModeService.InstallModeList[1]))
                            {
                                // 标记安装状态
                                try
                                {
                                    AutoResetEvent autoResetEvent = new(false);
                                    DispatcherQueue.TryEnqueue(() =>
                                    {
                                        for (int index = 0; index < CompletedCollection.Count; index++)
                                        {
                                            if (CompletedCollection[index].DownloadKey.Equals(completedItem.DownloadKey))
                                            {
                                                CompletedCollection[index].IsInstalling = true;
                                                break;
                                            }
                                        }

                                        autoResetEvent.Set();
                                    });

                                    autoResetEvent.WaitOne();
                                    autoResetEvent.Dispose();

                                    // 更新安装进度
                                    Progress<DeploymentProgress> progressCallBack = new((installProgress) =>
                                    {
                                        DispatcherQueue.TryEnqueue(() =>
                                        {
                                            for (int index = 0; index < CompletedCollection.Count; index++)
                                            {
                                                if (CompletedCollection[index].DownloadKey.Equals(completedItem.DownloadKey))
                                                {
                                                    CompletedCollection[index].InstallValue = installProgress.percentage;
                                                    break;
                                                };
                                            }
                                        });
                                    });

                                    try
                                    {
                                        // 安装目标应用，并获取安装进度
                                        DeploymentResult installResult = await packageManager.AddPackageAsync(new Uri(completedItem.FilePath), null, DeploymentOptions.None).AsTask(progressCallBack);
                                        // 显示安装成功通知
                                        ToastNotificationService.Show(NotificationKind.InstallApp, "Successfully", completedFile.Name);
                                    }
                                    // 安装失败显示失败信息
                                    catch (Exception e)
                                    {
                                        DispatcherQueue.TryEnqueue(() =>
                                        {
                                            for (int index = 0; index < CompletedCollection.Count; index++)
                                            {
                                                if (CompletedCollection[index].DownloadKey.Equals(completedItem.DownloadKey))
                                                {
                                                    CompletedCollection[index].InstallError = true;
                                                }
                                            }
                                        });

                                        // 显示安装失败通知
                                        ToastNotificationService.Show(NotificationKind.InstallApp, "Error", completedFile.Name, e.Message);
                                    }
                                    // 恢复原来的安装信息显示（并延缓当前安装信息显示时间0.5秒）
                                    finally
                                    {
                                        await Task.Delay(500);
                                        DispatcherQueue.TryEnqueue(() =>
                                        {
                                            for (int index = 0; index < CompletedCollection.Count; index++)
                                            {
                                                if (CompletedCollection[index].DownloadKey.Equals(completedItem.DownloadKey))
                                                {
                                                    CompletedCollection[index].IsInstalling = false;
                                                    CompletedCollection[index].InstallError = false;
                                                    break;
                                                }
                                            }
                                        });
                                    }
                                }
                                catch (Exception e)
                                {
                                    LogService.WriteLog(LoggingLevel.Warning, "Install apps failed.", e);
                                    return;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LoggingLevel.Warning, "Install apps failed.", e);
                            return;
                        }
                    }
                });
            }
            else
            {
                await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.FileLost));
            }
        }

        /// <summary>
        /// 打开当前项目存储的文件夹
        /// </summary>
        private void OnOpenItemFolderExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string filePath)
            {
                Task.Run(async () =>
                {
                    if (File.Exists(filePath))
                    {
                        // 定位文件，若定位失败，则仅启动资源管理器并打开桌面目录
                        if (!string.IsNullOrEmpty(filePath))
                        {
                            try
                            {
                                StorageFile file = await StorageFile.GetFileFromPathAsync(filePath);
                                StorageFolder folder = await file.GetParentAsync();
                                FolderLauncherOptions options = new();
                                options.ItemsToSelect.Add(file);
                                await Launcher.LaunchFolderAsync(folder, options);
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(LoggingLevel.Warning, "Completed download completedItem folder located failed.", e);
                                await Launcher.LaunchFolderPathAsync(InfoHelper.UserDataPath.Desktop);
                            }
                        }
                        else
                        {
                            await Launcher.LaunchFolderPathAsync(InfoHelper.UserDataPath.Desktop);
                        }
                    }
                    else
                    {
                        await Launcher.LaunchFolderAsync(DownloadOptionsService.DownloadFolder);
                    }
                });
            }
        }

        /// <summary>
        /// 共享文件
        /// </summary>
        private async void OnShareFileExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is CompletedModel completedItem && File.Exists(completedItem.FilePath))
            {
                try
                {
                    DataTransferManager dataTransferManager = DataTransferManagerInterop.GetForWindow((IntPtr)MainWindow.Current.AppWindow.Id.Value);

                    dataTransferManager.DataRequested += async (sender, args) =>
                    {
                        DataRequestDeferral deferral = args.Request.GetDeferral();

                        args.Request.Data.Properties.Title = ResourceService.GetLocalized("Download/ShareFileTitle");
                        args.Request.Data.SetStorageItems(new List<StorageFile>() { await StorageFile.GetFileFromPathAsync(completedItem.FilePath) });
                        deferral.Complete();
                    };

                    DataTransferManagerInterop.ShowShareUIForWindow((IntPtr)MainWindow.Current.AppWindow.Id.Value);
                }
                catch (Exception e)
                {
                    await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.ShareFailed, false, 1));
                    LogService.WriteLog(LoggingLevel.Warning, "Share file failed.", e);
                }
            }
            else
            {
                await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.FileLost));
            }
        }

        #endregion 第一部分：XamlUICommand 命令调用时挂载的事件

        #region 第二部分：已下载完成控件——挂载的事件

        /// <summary>
        /// 打开默认保存的文件夹
        /// </summary>
        private async void OnOpenFolderClicked(object sender, RoutedEventArgs args)
        {
            await DownloadOptionsService.OpenFolderAsync(DownloadOptionsService.DownloadFolder);
        }

        /// <summary>
        /// 进入多选模式
        /// </summary>
        private void OnSelectClicked(object sender, RoutedEventArgs args)
        {
            foreach (CompletedModel completedItem in CompletedCollection)
            {
                completedItem.IsSelectMode = true;
                completedItem.IsSelected = false;
            }

            IsSelectMode = true;
        }

        /// <summary>
        /// 全部选择
        /// </summary>
        private void OnSelectAllClicked(object sender, RoutedEventArgs args)
        {
            foreach (CompletedModel completedItem in CompletedCollection)
            {
                completedItem.IsSelected = true;
            }
        }

        /// <summary>
        ///  全部不选
        /// </summary>
        private void OnSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            foreach (CompletedModel completedItem in CompletedCollection)
            {
                completedItem.IsSelected = false;
            }
        }

        /// <summary>
        /// 删除选中的任务
        /// </summary>
        private async void OnDeleteSelectedClicked(object sender, RoutedEventArgs args)
        {
            List<CompletedModel> selectedCompletedDataList = [];

            foreach (CompletedModel completedItem in CompletedCollection)
            {
                if (completedItem.IsSelected is true)
                {
                    selectedCompletedDataList.Add(completedItem);
                }
            }

            // 没有选中任何内容时显示空提示对话框
            if (selectedCompletedDataList.Count is 0)
            {
                await ContentDialogHelper.ShowAsync(new SelectEmptyPromptDialog(), this);
                return;
            }

            // 当前任务正在安装时，不进行其他任何操作
            if (selectedCompletedDataList.Exists(item => item.IsInstalling is true))
            {
                await ContentDialogHelper.ShowAsync(new InstallingNotifyDialog(), this);
                return;
            }

            // 删除时显示删除确认对话框
            ContentDialogResult result = await ContentDialogHelper.ShowAsync(new DeletePromptDialog(DeleteKind.Download), this);

            if (result is ContentDialogResult.Primary)
            {
                IsSelectMode = false;

                for (int index = CompletedCollection.Count - 1; index >= 0; index--)
                {
                    CompletedModel completedItem = CompletedCollection[index];
                    completedItem.IsSelectMode = false;

                    if (completedItem.IsSelected)
                    {
                        completedItem.IsNotOperated = false;
                        await Task.Run(() =>
                        {
                            DownloadStorageService.DeleteDownloadData(completedItem.DownloadKey);
                        });
                    }
                }
            }
        }

        /// <summary>
        /// 删除选中的任务（包括文件）
        /// </summary>
        private async void OnDeleteSelectedWithFileClicked(object sender, RoutedEventArgs args)
        {
            List<CompletedModel> selectedCompletedDataList = [];

            foreach (CompletedModel completedItem in CompletedCollection)
            {
                if (completedItem.IsSelected is true)
                {
                    selectedCompletedDataList.Add(completedItem);
                }
            }

            // 没有选中任何内容时显示空提示对话框
            if (selectedCompletedDataList.Count is 0)
            {
                await ContentDialogHelper.ShowAsync(new SelectEmptyPromptDialog(), this);
                return;
            }

            // 当前任务正在安装时，不进行其他任何操作
            if (selectedCompletedDataList.Exists(item => item.IsInstalling is true))
            {
                await ContentDialogHelper.ShowAsync(new InstallingNotifyDialog(), this);
                return;
            }

            // 删除时显示删除确认对话框
            ContentDialogResult result = await ContentDialogHelper.ShowAsync(new DeletePromptDialog(DeleteKind.Download), this);

            if (result is ContentDialogResult.Primary)
            {
                IsSelectMode = false;

                for (int index = CompletedCollection.Count - 1; index >= 0; index--)
                {
                    CompletedModel completedItem = CompletedCollection[index];
                    completedItem.IsSelectMode = false;

                    if (completedItem.IsSelected)
                    {
                        completedItem.IsNotOperated = false;
                        await Task.Run(() =>
                        {
                            // 删除文件
                            try
                            {
                                if (File.Exists(completedItem.FilePath))
                                {
                                    File.Delete(completedItem.FilePath);
                                }
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(LoggingLevel.Warning, "Delete completed download file failed.", e);
                            }

                            DownloadStorageService.DeleteDownloadData(completedItem.DownloadKey);
                        });
                    }
                }
            }
        }

        /// <summary>
        /// 分享选中的文件
        /// </summary>
        private void OnShareSelectedFileClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                List<CompletedModel> selectedCompletedDataList = [];

                foreach (CompletedModel completedItem in CompletedCollection)
                {
                    if (completedItem.IsSelected is true)
                    {
                        selectedCompletedDataList.Add(completedItem);
                    }
                }

                // 没有选中任何内容时显示空提示对话框
                if (selectedCompletedDataList.Count is 0)
                {
                    DispatcherQueue.TryEnqueue(async () =>
                    {
                        await ContentDialogHelper.ShowAsync(new SelectEmptyPromptDialog(), this);
                    });
                    return;
                }

                DispatcherQueue.TryEnqueue(async () =>
                {
                    try
                    {
                        DataTransferManager dataTransferManager = DataTransferManagerInterop.GetForWindow((IntPtr)MainWindow.Current.AppWindow.Id.Value);

                        dataTransferManager.DataRequested += async (sender, args) =>
                        {
                            DataRequestDeferral deferral = args.Request.GetDeferral();

                            args.Request.Data.Properties.Title = ResourceService.GetLocalized("Download/ShareFileTitle");

                            List<StorageFile> selectedFileList = [];
                            foreach (CompletedModel completedItem in selectedCompletedDataList)
                            {
                                try
                                {
                                    if (File.Exists(completedItem.FilePath))
                                    {
                                        selectedFileList.Add(await StorageFile.GetFileFromPathAsync(completedItem.FilePath));
                                    }
                                }
                                catch (Exception e)
                                {
                                    LogService.WriteLog(LoggingLevel.Warning, string.Format("Read file {0} failed", completedItem.FilePath), e);
                                    continue;
                                }
                            }
                            args.Request.Data.SetStorageItems(selectedFileList);
                            deferral.Complete();
                        };

                        DataTransferManagerInterop.ShowShareUIForWindow((IntPtr)MainWindow.Current.AppWindow.Id.Value);
                    }
                    catch (Exception e)
                    {
                        await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.ShareFailed, true, selectedCompletedDataList.Count));
                        LogService.WriteLog(LoggingLevel.Warning, "Share selected files failed.", e);
                    }
                });
            });
        }

        /// <summary>
        /// 退出多选模式
        /// </summary>
        private void OnCancelClicked(object sender, RoutedEventArgs args)
        {
            IsSelectMode = false;

            foreach (CompletedModel completedItem in CompletedCollection)
            {
                completedItem.IsSelectMode = false;
            }
        }

        /// <summary>
        /// 在多选模式下点击项目选择相应的条目
        /// </summary>
        private void OnItemInvoked(object sender, ItemClickEventArgs args)
        {
            if (args.ClickedItem is CompletedModel completedItem)
            {
                int clickedIndex = CompletedCollection.IndexOf(completedItem);

                if (clickedIndex >= 0 && clickedIndex < CompletedCollection.Count)
                {
                    CompletedCollection[clickedIndex].IsSelected = !CompletedCollection[clickedIndex].IsSelected;
                }
            }
        }

        #endregion 第二部分：已下载完成控件——挂载的事件

        #region 第三部分：自定义事件

        /// <summary>
        /// 应用程序退出时触发的事件
        /// </summary>
        private void OnApplicationExit(object sender, EventArgs args)
        {
            try
            {
                GlobalNotificationService.ApplicationExit -= OnApplicationExit;
                DownloadStorageService.StorageDataAdded -= OnStorageDataAdded;
                DownloadStorageService.StorageDataDeleted -= OnStorageDataDeleted;
                DownloadStorageService.StorageDataCleared -= OnStorageDataCleared;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Unregister download storage event failed", e);
            }
        }

        /// <summary>
        /// 添加已下载完成任务
        /// </summary>
        private void OnStorageDataAdded(DownloadSchedulerModel downloadSchedulerItem)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                CompletedCollection.Add(new CompletedModel()
                {
                    DownloadKey = downloadSchedulerItem.DownloadKey,
                    FileName = downloadSchedulerItem.FileName,
                    FilePath = downloadSchedulerItem.FilePath,
                    FileLink = downloadSchedulerItem.FileLink,
                    TotalSize = downloadSchedulerItem.TotalSize,
                    IsNotOperated = true,
                    IsSelected = false,
                    IsSelectMode = false
                });
            });
        }

        /// <summary>
        /// 删除已下载完成任务
        /// </summary>
        private void OnStorageDataDeleted(string downloadKey)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                foreach (CompletedModel completedItem in CompletedCollection)
                {
                    if (completedItem.DownloadKey.Equals(downloadKey))
                    {
                        CompletedCollection.Remove(completedItem);
                        break;
                    }
                }
            });
        }

        /// <summary>
        /// 清空已下载完成任务
        /// </summary>
        private void OnStorageDataCleared()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                CompletedCollection.Clear();
            });
        }

        #endregion 第三部分：自定义事件
    }
}
