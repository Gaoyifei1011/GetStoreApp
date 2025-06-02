using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models;
using GetStoreApp.Services.Download;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Settings;
using GetStoreApp.UI.TeachingTips;
using GetStoreApp.Views.Dialogs;
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
using Microsoft.Windows.Management.Deployment;
using Windows.Management.Deployment;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;
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
        private readonly string CompletedCountInfoString = ResourceService.GetLocalized("Completed/CompletedCountInfo");
        private readonly string FileShareString = ResourceService.GetLocalized("Completed/FileShare");
        private readonly string InstallFailed1String = ResourceService.GetLocalized("Completed/InstallFailed1");
        private readonly string InstallFailed2String = ResourceService.GetLocalized("Completed/InstallFailed2");
        private readonly string InstallFailed3String = ResourceService.GetLocalized("Completed/InstallFailed3");
        private readonly string InstallSuccessfullyString = ResourceService.GetLocalized("Completed/InstallSuccessfully");
        private readonly string UnknownString = ResourceService.GetLocalized("Completed/Unknown");
        private bool isInitialized;
        private PackageDeploymentManager packageDeploymentManager;

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
                    packageDeploymentManager = PackageDeploymentManager.GetDefault();

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
                            TotalSize = downloadSchedulerItem.TotalSize,
                            IsNotOperated = true,
                            IsSelected = false,
                            IsSelectMode = false
                        });
                    }
                }

                await Task.Run(() =>
                {
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
            if (args.Parameter is CompletedModel completed)
            {
                if (completed.IsInstalling)
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.InstallingNotify));
                }
                else
                {
                    completed.IsNotOperated = false;
                    await Task.Run(() =>
                    {
                        DownloadStorageService.DeleteDownloadData(completed.DownloadKey);
                    });
                }
            }
        }

        /// <summary>
        /// 删除当前任务（包括文件）
        /// </summary>
        private async void OnDeleteWithFileExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is CompletedModel completed)
            {
                if (completed.IsInstalling)
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.InstallingNotify));
                }
                else
                {
                    completed.IsNotOperated = false;
                    await Task.Run(() =>
                    {
                        // 删除文件
                        try
                        {
                            if (File.Exists(completed.FilePath))
                            {
                                File.Delete(completed.FilePath);
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(CompletedPage), nameof(OnDeleteWithFileExecuteRequested), 1, e);
                        }

                        DownloadStorageService.DeleteDownloadData(completed.DownloadKey);
                    });
                }
            }
        }

        /// <summary>
        /// 查看文件信息
        /// </summary>
        private async void OnFileInformationExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is CompletedModel completed && File.Exists(completed.FilePath))
            {
                await MainWindow.Current.ShowDialogAsync(new FileInformationDialog(completed));
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
            if (args.Parameter is CompletedModel completed && File.Exists(completed.FilePath))
            {
                // 普通应用：直接安装
                if (completed.FilePath.EndsWith(".exe") || completed.FileName.EndsWith(".msi"))
                {
                    await Task.Run(() =>
                    {
                        Shell32Library.ShellExecute(IntPtr.Zero, "open", completed.FilePath, string.Empty, null, WindowShowStyle.SW_SHOWNORMAL);
                    });
                }
                // 商店打包应用：使用应用安装程序安装或直接安装
                else
                {
                    StorageFile completedFile = await Task.Run(async () =>
                    {
                        try
                        {
                            return await StorageFile.GetFileFromPathAsync(completed.FilePath);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(CompletedPage), nameof(OnInstallExecuteRequested), 1, e);
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
                                completed.IsInstalling = true;

                                (bool result, PackageDeploymentResult packageDeploymentResult, Exception exception) = await Task.Run(async () =>
                                {
                                    try
                                    {
                                        Microsoft.Windows.Management.Deployment.AddPackageOptions addPackageOptions = new()
                                        {
                                            AllowUnsigned = AppInstallService.AllowUnsignedPackageValue,
                                            ForceAppShutdown = AppInstallService.ForceAppShutdownValue,
                                            ForceTargetAppShutdown = AppInstallService.ForceTargetAppShutdownValue
                                        };

                                        // 安装目标应用，并获取安装进度
                                        IAsyncOperationWithProgress<PackageDeploymentResult, PackageDeploymentProgress> installPackageWithProgress = packageDeploymentManager.AddPackageByUriAsync(new Uri(completed.FilePath), addPackageOptions);

                                        // 更新安装进度
                                        installPackageWithProgress.Progress = (result, progress) => OnPackageInstallProgress(result, progress, completed);
                                        return ValueTuple.Create<bool, PackageDeploymentResult, Exception>(true, await installPackageWithProgress, null);
                                    }
                                    // 安装失败显示失败信息
                                    catch (Exception e)
                                    {
                                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(CompletedPage), nameof(OnInstallExecuteRequested), 2, e);
                                        return ValueTuple.Create<bool, PackageDeploymentResult, Exception>(false, null, e);
                                    }
                                });

                                if (result && packageDeploymentResult is not null)
                                {
                                    // 安装成功
                                    if (packageDeploymentResult.Status is PackageDeploymentStatus.CompletedSuccess)
                                    {
                                        await Task.Run(() =>
                                        {
                                            // 显示安装成功通知
                                            AppNotificationBuilder appNotificationBuilder = new();
                                            appNotificationBuilder.AddArgument("action", "OpenApp");
                                            appNotificationBuilder.AddText(string.Format(InstallSuccessfullyString, completedFile.Name));
                                            ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                                        });
                                    }
                                    // 安装失败
                                    else
                                    {
                                        completed.InstallFailed = true;

                                        await Task.Run(() =>
                                        {
                                            string errorCode = packageDeploymentResult.ExtendedError is not null ? Convert.ToString(packageDeploymentResult.ExtendedError.HResult) : UnknownString;
                                            string errorMessage = packageDeploymentResult.ErrorText;

                                            // 显示安装失败通知
                                            AppNotificationBuilder appNotificationBuilder = new();
                                            appNotificationBuilder.AddArgument("action", "OpenApp");
                                            appNotificationBuilder.AddText(string.Format(InstallFailed1String, completedFile.Name));
                                            appNotificationBuilder.AddText(string.Format(InstallFailed2String, errorCode));
                                            appNotificationBuilder.AddText(string.Format(InstallFailed3String, errorMessage));
                                            ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                                        });
                                    }
                                }
                                else
                                {
                                    completed.InstallFailed = true;

                                    await Task.Run(() =>
                                    {
                                        string errorMessage = exception is not null ? exception.Message : UnknownString;

                                        // 显示安装失败通知
                                        AppNotificationBuilder appNotificationBuilder = new();
                                        appNotificationBuilder.AddArgument("action", "OpenApp");
                                        appNotificationBuilder.AddText(string.Format(InstallFailed1String, completedFile.Name));
                                        appNotificationBuilder.AddText(string.Format(InstallFailed3String, errorMessage));
                                        ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                                    });
                                }

                                // 恢复原来的安装信息显示（并延缓当前安装信息显示时间0.5秒）
                                await Task.Delay(500);
                                completed.IsInstalling = false;
                                completed.InstallFailed = false;
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(CompletedPage), nameof(OnInstallExecuteRequested), 3, e);
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
                                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(CompletedPage), nameof(OnOpenItemFolderExecuteRequested), 1, e);
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
            if (args.Parameter is CompletedModel completed && File.Exists(completed.FilePath))
            {
                try
                {
                    DataTransferManager dataTransferManager = DataTransferManagerInterop.GetForWindow(Win32Interop.GetWindowFromWindowId(MainWindow.Current.AppWindow.Id));

                    dataTransferManager.DataRequested += async (sender, args) =>
                    {
                        DataRequestDeferral deferral = args.Request.GetDeferral();

                        args.Request.Data.Properties.Title = FileShareString;
                        args.Request.Data.SetStorageItems(new List<StorageFile>() { await StorageFile.GetFileFromPathAsync(completed.FilePath) });
                        deferral.Complete();
                    };

                    DataTransferManagerInterop.ShowShareUIForWindow(Win32Interop.GetWindowFromWindowId(MainWindow.Current.AppWindow.Id));
                }
                catch (Exception e)
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.ShareFailed, false, 1));
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(CompletedPage), nameof(OnShareFileExecuteRequested), 1, e);
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
        private void OnOpenFolderClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(DownloadOptionsService.OpenFolderAsync);
        }

        /// <summary>
        /// 了解应用具体的使用说明
        /// </summary>
        private async void OnUseInstructionClicked(object sender, RoutedEventArgs args)
        {
            if (MainWindow.Current.GetFrameContent() is DownloadPage downloadPage)
            {
                await Task.Delay(300);
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
                                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(CompletedPage), nameof(OnDeleteSelectedWithFileClicked), 1, e);
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

                        args.Request.Data.Properties.Title = FileShareString;

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
                                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(CompletedPage), nameof(OnShareSelectedFileClicked), 1, e);
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
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(CompletedPage), nameof(OnShareSelectedFileClicked), 2, e);
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
            if (args.ClickedItem is CompletedModel completed)
            {
                completed.IsSelected = !completed.IsSelected;
            }
        }

        #region 第三部分：已下载完成页面——自定义事件

        /// <summary>
        /// 应用程序退出时触发的事件
        /// </summary>
        private void OnApplicationExit()
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
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(CompletedPage), nameof(OnApplicationExit), 1, e);
            }
        }

        /// <summary>
        /// 添加已下载完成任务
        /// </summary>
        private void OnStorageDataAdded(DownloadSchedulerModel downloadScheduler)
        {
            DispatcherQueue.TryEnqueue(async () =>
            {
                CompletedCollection.Add(new CompletedModel()
                {
                    IconImage = await GetFileIconImageAsync(downloadScheduler.FilePath),
                    DownloadKey = downloadScheduler.DownloadKey,
                    FileName = downloadScheduler.FileName,
                    FilePath = downloadScheduler.FilePath,
                    TotalSize = downloadScheduler.TotalSize,
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
        private void OnPackageInstallProgress(IAsyncOperationWithProgress<PackageDeploymentResult, PackageDeploymentProgress> result, PackageDeploymentProgress progress, CompletedModel completed)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                for (int index = 0; index < CompletedCollection.Count; index++)
                {
                    if (Equals(CompletedCollection[index].DownloadKey, completed.DownloadKey))
                    {
                        CompletedCollection[index].InstallValue = progress.Progress * 100;
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
            return completedKind is not CompletedResultKind.Loading;
        }
    }
}
