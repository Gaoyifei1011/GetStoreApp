using GetStoreApp.Extensions.DataType.Enums;
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
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.AppNotifications.Builder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;
using Windows.Management.Deployment;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.System;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 下载已完成页面
    /// </summary>
    public sealed partial class CompletedPage : Page, INotifyPropertyChanged
    {
        private readonly string CompletedCountInfo = ResourceService.GetLocalized("Download/CompletedCountInfo");
        private bool isInitialized;
        private PackageManager packageManager;

        private CompletedResultKind _completedResultKind = CompletedResultKind.Loading;

        public CompletedResultKind CompletedResultKind
        {
            get { return _completedResultKind; }

            set
            {
                if (!Equals(_completedResultKind, value))
                {
                    _completedResultKind = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CompletedResultKind)));
                }
            }
        }

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

        public CompletedPage()
        {
            InitializeComponent();
        }

        #region 重载父类事件

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (!isInitialized)
            {
                isInitialized = true;
                List<DownloadSchedulerModel> downloadStorageList = await Task.Run(() =>
                {
                    packageManager = new();

                    DownloadStorageService.DownloadStorageSemaphoreSlim?.Wait();
                    return DownloadStorageService.GetDownloadData();
                });

                if (downloadStorageList is not null)
                {
                    foreach (DownloadSchedulerModel downloadSchedulerItem in downloadStorageList)
                    {
                        CompletedCollection.Add(new CompletedModel()
                        {
                            IconImage = await GetFileIconImageAsync(downloadSchedulerItem.FilePath),
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
                }

                await Task.Run(() =>
                {
                    packageManager = new();

                    GlobalNotificationService.ApplicationExit += OnApplicationExit;
                    DownloadStorageService.StorageDataAdded += OnStorageDataAdded;
                    DownloadStorageService.StorageDataDeleted += OnStorageDataDeleted;
                    DownloadStorageService.StorageDataCleared += OnStorageDataCleared;

                    DownloadStorageService.DownloadStorageSemaphoreSlim?.Release();
                });

                CompletedResultKind = CompletedCollection.Count is 0 ? CompletedResultKind.Empty : CompletedResultKind.Successfully;
            }
        }

        #endregion 重载父类事件

        #region 第一部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 删除当前任务
        /// </summary>
        private async void OnDeleteExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is CompletedModel completedItem)
            {
                if (completedItem.IsInstalling)
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.InstallingNotify));
                }
                else
                {
                    completedItem.IsNotOperated = false;
                    await Task.Run(() =>
                    {
                        DownloadStorageService.DeleteDownloadData(completedItem.DownloadKey);
                    });
                }
            }
        }

        /// <summary>
        /// 删除当前任务（包括文件）
        /// </summary>
        private async void OnDeleteWithFileExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is CompletedModel completedItem)
            {
                if (completedItem.IsInstalling)
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.InstallingNotify));
                }
                else
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

        /// <summary>
        /// 查看文件信息
        /// </summary>
        private async void OnFileInformationExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is CompletedModel completedItem && File.Exists(completedItem.FilePath))
            {
                await MainWindow.Current.ShowDialogAsync(new FileInformationDialog(completedItem));
            }
            else
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.FileLost));
            }
        }

        /// <summary>
        /// 安装应用
        /// </summary>
        private async void OnInstallExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is CompletedModel completedItem && File.Exists(completedItem.FilePath))
            {
                // 普通应用：直接安装
                if (completedItem.FilePath.EndsWith(".exe") || completedItem.FileName.EndsWith(".msi"))
                {
                    await Task.Run(() =>
                    {
                        Shell32Library.ShellExecute(IntPtr.Zero, "open", completedItem.FilePath, string.Empty, null, WindowShowStyle.SW_SHOWNORMAL);
                    });
                }
                // 商店打包应用：使用应用安装程序安装或直接安装
                else
                {
                    StorageFile completedFile = await Task.Run(async () =>
                    {
                        try
                        {
                            return await StorageFile.GetFileFromPathAsync(completedItem.FilePath);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LoggingLevel.Error, "Get File from path failed", e);
                            return null;
                        }
                    });

                    if (completedFile is not null)
                    {
                        try
                        {
                            // 使用应用安装程序安装
                            if (Equals(InstallModeService.InstallMode, InstallModeService.InstallModeList[0]))
                            {
                                await Task.Run(async () =>
                                {
                                    await Launcher.LaunchFileAsync(completedFile);
                                });
                            }
                            // 直接安装
                            else if (Equals(InstallModeService.InstallMode, InstallModeService.InstallModeList[1]))
                            {
                                // 标记安装状态
                                completedItem.IsInstalling = true;

                                (bool result, DeploymentResult deploymentResult, Exception exception) = await Task.Run(async () =>
                                {
                                    try
                                    {
                                        AddPackageOptions addPackageOptions = new()
                                        {
                                            AllowUnsigned = AppInstallService.AllowUnsignedPackageValue,
                                            ForceAppShutdown = AppInstallService.ForceAppShutdownValue,
                                            ForceTargetAppShutdown = AppInstallService.ForceTargetAppShutdownValue
                                        };

                                        // 安装目标应用，并获取安装进度
                                        IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> installPackageWithProgress = packageManager.AddPackageByUriAsync(new Uri(completedItem.FilePath), addPackageOptions);

                                        // 更新安装进度
                                        installPackageWithProgress.Progress = (result, progress) => OnPackageInstallProgress(result, progress, completedItem);
                                        return ValueTuple.Create<bool, DeploymentResult, Exception>(true, await installPackageWithProgress, null);
                                    }
                                    // 安装失败显示失败信息
                                    catch (Exception e)
                                    {
                                        LogService.WriteLog(LoggingLevel.Warning, "Install apps failed.", e);
                                        return ValueTuple.Create<bool, DeploymentResult, Exception>(false, null, e);
                                    }
                                });

                                if (result && deploymentResult is not null)
                                {
                                    // 安装成功
                                    if (deploymentResult.ExtendedErrorCode is null)
                                    {
                                        await Task.Run(() =>
                                        {
                                            // 显示安装成功通知
                                            AppNotificationBuilder appNotificationBuilder = new();
                                            appNotificationBuilder.AddArgument("action", "OpenApp");
                                            appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/InstallSuccessfully"), completedFile.Name));
                                            ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                                        });
                                    }
                                    // 安装失败
                                    else
                                    {
                                        completedItem.InstallError = true;

                                        await Task.Run(() =>
                                        {
                                            // 显示安装失败通知
                                            AppNotificationBuilder appNotificationBuilder = new();
                                            appNotificationBuilder.AddArgument("action", "OpenApp");
                                            appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/InstallFailed1"), completedFile.Name));
                                            appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/InstallFailed2"), deploymentResult.ExtendedErrorCode.Message));
                                            ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                                        });
                                    }
                                }
                                else
                                {
                                    completedItem.InstallError = true;

                                    await Task.Run(() =>
                                    {
                                        // 显示安装失败通知
                                        AppNotificationBuilder appNotificationBuilder = new();
                                        appNotificationBuilder.AddArgument("action", "OpenApp");
                                        appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/InstallFailed1"), completedFile.Name));
                                        appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/InstallFailed2"), exception.Message));
                                        ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                                    });
                                }

                                // 恢复原来的安装信息显示（并延缓当前安装信息显示时间0.5秒）
                                await Task.Delay(500);
                                completedItem.IsInstalling = false;
                                completedItem.InstallError = false;
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LoggingLevel.Warning, "Install apps failed.", e);
                            return;
                        }
                    }
                }
            }
            else
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.FileLost));
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
                    DataTransferManager dataTransferManager = DataTransferManagerInterop.GetForWindow(Win32Interop.GetWindowFromWindowId(MainWindow.Current.AppWindow.Id));

                    dataTransferManager.DataRequested += async (sender, args) =>
                    {
                        DataRequestDeferral deferral = args.Request.GetDeferral();

                        args.Request.Data.Properties.Title = ResourceService.GetLocalized("Download/ShareFileTitle");
                        args.Request.Data.SetStorageItems(new List<StorageFile>() { await StorageFile.GetFileFromPathAsync(completedItem.FilePath) });
                        deferral.Complete();
                    };

                    DataTransferManagerInterop.ShowShareUIForWindow(Win32Interop.GetWindowFromWindowId(MainWindow.Current.AppWindow.Id));
                }
                catch (Exception e)
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.ShareFailed, false, 1));
                    LogService.WriteLog(LoggingLevel.Warning, "Share file failed.", e);
                }
            }
            else
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.FileLost));
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
        /// 了解应用具体的使用说明
        /// </summary>
        private void OnUseInstructionClicked(object sender, RoutedEventArgs args)
        {
            if (MainWindow.Current.Content is DownloadPage downloadPage)
            {
                downloadPage.ShowUseInstruction();
            }
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

        #endregion 第二部分：已下载完成控件——挂载的事件

        /// <summary>
        /// 删除选中的任务
        /// </summary>
        private async void OnDeleteSelectedClicked(object sender, RoutedEventArgs args)
        {
            List<CompletedModel> selectedCompletedDataList = [];

            foreach (CompletedModel completedItem in CompletedCollection)
            {
                if (completedItem.IsSelected)
                {
                    selectedCompletedDataList.Add(completedItem);
                }
            }

            // 没有选中任何内容时显示空提示对话框
            if (selectedCompletedDataList.Count is 0)
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.SelectEmpty));
                return;
            }

            // 当前任务正在安装时，不进行其他任何操作
            if (selectedCompletedDataList.Exists(item => item.IsInstalling))
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.InstallingNotify));
                return;
            }

            // 删除时显示删除确认对话框
            ContentDialogResult result = await MainWindow.Current.ShowDialogAsync(new DeletePromptDialog(DeleteKind.Download));

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

                CompletedResultKind = CompletedCollection.Count is 0 ? CompletedResultKind.Empty : CompletedResultKind.Successfully;
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
                if (completedItem.IsSelected)
                {
                    selectedCompletedDataList.Add(completedItem);
                }
            }

            // 没有选中任何内容时显示空提示对话框
            if (selectedCompletedDataList.Count is 0)
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.SelectEmpty));
                return;
            }

            // 当前任务正在安装时，不进行其他任何操作
            if (selectedCompletedDataList.Exists(item => item.IsInstalling))
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.SelectEmpty));
                return;
            }

            // 删除时显示删除确认对话框
            ContentDialogResult result = await MainWindow.Current.ShowDialogAsync(new DeletePromptDialog(DeleteKind.Download));

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

                CompletedResultKind = CompletedCollection.Count is 0 ? CompletedResultKind.Empty : CompletedResultKind.Successfully;
            }
        }

        /// <summary>
        /// 分享选中的文件
        /// </summary>
        private async void OnShareSelectedFileClicked(object sender, RoutedEventArgs args)
        {
            List<CompletedModel> selectedCompletedDataList = [];

            foreach (CompletedModel completedItem in CompletedCollection)
            {
                if (completedItem.IsSelected)
                {
                    selectedCompletedDataList.Add(completedItem);
                }
            }

            // 没有选中任何内容时显示空提示对话框
            if (selectedCompletedDataList.Count is 0)
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.SelectEmpty));
                return;
            }
            else
            {
                try
                {
                    DataTransferManager dataTransferManager = DataTransferManagerInterop.GetForWindow(Win32Interop.GetWindowFromWindowId(MainWindow.Current.AppWindow.Id));

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

                    DataTransferManagerInterop.ShowShareUIForWindow(Win32Interop.GetWindowFromWindowId(MainWindow.Current.AppWindow.Id));
                }
                catch (Exception e)
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.ShareFailed, true, selectedCompletedDataList.Count));
                    LogService.WriteLog(LoggingLevel.Warning, "Share selected files failed.", e);
                }
            }
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
        private void OnItemClick(object sender, ItemClickEventArgs args)
        {
            if (args.ClickedItem is CompletedModel completedItem)
            {
                completedItem.IsSelected = !completedItem.IsSelected;
            }
        }

        #region 第三部分：已下载完成页面——自定义事件

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
            DispatcherQueue.TryEnqueue(async () =>
            {
                CompletedCollection.Add(new CompletedModel()
                {
                    IconImage = await GetFileIconImageAsync(downloadSchedulerItem.FilePath),
                    DownloadKey = downloadSchedulerItem.DownloadKey,
                    FileName = downloadSchedulerItem.FileName,
                    FilePath = downloadSchedulerItem.FilePath,
                    FileLink = downloadSchedulerItem.FileLink,
                    TotalSize = downloadSchedulerItem.TotalSize,
                    IsNotOperated = true,
                    IsSelected = false,
                    IsSelectMode = false
                });

                CompletedResultKind = CompletedCollection.Count is 0 ? CompletedResultKind.Empty : CompletedResultKind.Successfully;
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
                    if (Equals(completedItem.DownloadKey, downloadKey))
                    {
                        CompletedCollection.Remove(completedItem);
                        break;
                    }
                }

                CompletedResultKind = CompletedCollection.Count is 0 ? CompletedResultKind.Empty : CompletedResultKind.Successfully;
            });
        }

        /// <summary>
        /// 清空已下载完成任务
        /// </summary>
        private void OnStorageDataCleared()
        {
            DispatcherQueue.TryEnqueue(CompletedCollection.Clear);
        }

        /// <summary>
        /// 应用安装状态发生改变时触发的事件
        /// </summary>
        private void OnPackageInstallProgress(IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> result, DeploymentProgress progress, CompletedModel completedItem)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                for (int index = 0; index < CompletedCollection.Count; index++)
                {
                    if (Equals(CompletedCollection[index].DownloadKey, completedItem.DownloadKey))
                    {
                        CompletedCollection[index].InstallValue = progress.percentage;
                        break;
                    }
                }
            });
        }

        #endregion 第三部分：已下载完成页面——自定义事件

        /// <summary>
        /// 获取文件缩略图
        /// </summary>
        private async Task<BitmapImage> GetFileIconImageAsync(string filePath)
        {
            if (File.Exists(filePath))
            {
                StorageItemThumbnail storageItemThumbnail = await Task.Run(async () =>
                {
                    try
                    {
                        StorageFile storageFile = await StorageFile.GetFileFromPathAsync(filePath);
                        return await storageFile.GetThumbnailAsync(ThumbnailMode.SingleItem, 32, ThumbnailOptions.UseCurrentScale);
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                });

                if (storageItemThumbnail is not null)
                {
                    BitmapImage bitmapImage = new();
                    await bitmapImage.SetSourceAsync(storageItemThumbnail);
                    storageItemThumbnail.Dispose();
                    return bitmapImage;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取加载下载已完成文件是否成功
        /// </summary>
        private Visibility GetCompletedSuccessfullyState(CompletedResultKind completedResultKind, bool isSuccessfully)
        {
            return isSuccessfully ? completedResultKind is CompletedResultKind.Successfully ? Visibility.Visible : Visibility.Collapsed : completedResultKind is CompletedResultKind.Successfully ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// 检查加载下载已完成文件是否成功
        /// </summary>
        private Visibility CheckCompletedState(CompletedResultKind completedResultKind, CompletedResultKind comparedCompletedResultKind)
        {
            return Equals(completedResultKind, comparedCompletedResultKind) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取是否正在加载中
        /// </summary>

        private bool GetIsLoading(CompletedResultKind completedKind)
        {
            return !Equals(completedKind, CompletedResultKind.Loading);
        }
    }
}
