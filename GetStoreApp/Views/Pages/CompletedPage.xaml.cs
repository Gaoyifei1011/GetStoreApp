using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models;
using GetStoreApp.Services.Download;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Settings;
using GetStoreApp.Views.Dialogs;
using GetStoreApp.Views.NotificationTips;
using GetStoreApp.Views.Windows;
using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.AppNotifications.Builder;
using Microsoft.Windows.Management.Deployment;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices.Marshalling;
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
    internal sealed partial class CompletedPage : Page, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string CompletedCountInfoString = ResourceService.GetLocalized("Completed/CompletedCountInfo");
        private readonly string FileShareString = ResourceService.GetLocalized("Completed/FileShare");
        private readonly string InstallProgressString = ResourceService.GetLocalized("Completed/InstallProgress");
        private readonly string InstallFailedString = ResourceService.GetLocalized("Completed/InstallFailed");
        private readonly string InstallFailed1String = ResourceService.GetLocalized("Completed/InstallFailed1");
        private readonly string InstallFailed2String = ResourceService.GetLocalized("Completed/InstallFailed2");
        private readonly string InstallFailed3String = ResourceService.GetLocalized("Completed/InstallFailed3");
        private readonly string PrepareInstallString = ResourceService.GetLocalized("Completed/PrepareInstall");
        private readonly string InstallSuccessfullyString = ResourceService.GetLocalized("Completed/InstallSuccessfully");
        private readonly string InstallSuccessfully1String = ResourceService.GetLocalized("Completed/InstallSuccessfully1");
        private readonly string NotAvailableString = ResourceService.GetLocalized("Completed/NotAvailable");
        private readonly string WaitInstallString = ResourceService.GetLocalized("Completed/WaitInstall");
        private bool isInitialized;
        private PackageDeploymentManager packageDeploymentManager;

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、集合与事件

        private CompletedResultKind _completedResultKind;

        private CompletedResultKind CompletedResultKind
        {
            get { return _completedResultKind; }

            set
            {
                if (!Equals(_completedResultKind, value))
                {
                    _completedResultKind = value;
                    PropertyChanged?.Invoke(this, new(nameof(CompletedResultKind)));
                }
            }
        }

        private ListViewSelectionMode _selectionMode;

        private ListViewSelectionMode SelectionMode
        {
            get { return _selectionMode; }

            set
            {
                if (!Equals(_selectionMode, value))
                {
                    _selectionMode = value;
                    PropertyChanged?.Invoke(this, new(nameof(SelectionMode)));
                }
            }
        }

        private ObservableCollection<CompletedModel> CompletedCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、集合与事件

        #region 第三部分：构造函数

        internal CompletedPage()
        {
            InitializeComponent();
        }

        #endregion 第三部分：构造函数

        #region 第四部分：父类虚方法重写

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (!isInitialized)
            {
                isInitialized = true;
                await InitializeDataAsync();
                await MountDownloadEventAsync();
                CompletedResultKind = CompletedCollection.Count is 0 ? CompletedResultKind.Empty : CompletedResultKind.Successfully;
            }
        }

        #endregion 第四部分：父类虚方法重写

        #region 第五部分：命令调用处理

        /// <summary>
        /// 复制到剪贴板
        /// </summary>
        private async void OnCopyEexeuteReqeusted(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is CompletedModel completed && File.Exists(completed.FilePath))
            {
                try
                {
                    List<StorageFile> fileList = await GetStorageFileListAsync([completed.FilePath]);
                    bool copyResult = CopyPasteHelper.CopyFileToClipBoard(fileList);
                    await MainWindow.Current.ShowNotificationAsync(new CopyPasteMainNotificationTip(copyResult));
                }
                catch (Exception e)
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ShareFailed, false, 1));
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(CompletedPage), nameof(OnCopyEexeuteReqeusted), 1, e);
                }
            }
        }

        /// <summary>
        /// 删除当前任务
        /// </summary>
        private async void OnDeleteExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is CompletedModel completed)
            {
                if (completed.IsInstalling)
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.InstallingNotify));
                }
                else
                {
                    DeleteFileDialog deleteFileDialog = new();
                    ContentDialogResult contentDialogResult = await MainWindow.Current.ShowDialogAsync(deleteFileDialog);

                    if (contentDialogResult is ContentDialogResult.Primary)
                    {
                        // 同时删除文件
                        await DeleteDownloaodFileAsync([completed], deleteFileDialog.DeleteFileSameTime);
                    }
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
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.FileLost));
            }
        }

        /// <summary>
        /// 安装应用
        /// </summary>
        private async void OnInstallExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is CompletedModel completed && File.Exists(completed.FilePath))
            {
                InstallAppsKind installAppsKind = GetInstallAppsKind(completed.FileName);

                // 普通应用：直接安装 或 商店打包应用：使用应用安装程序安装
                if (installAppsKind is InstallAppsKind.NonPackagedApp || installAppsKind is InstallAppsKind.PackagedAppViaAppInstaller)
                {
                    await InstallAppAsync(installAppsKind, completed);
                }
                // 直接安装
                else if (installAppsKind is InstallAppsKind.PackagedAppDirectlyInstall)
                {
                    try
                    {
                        // 标记安装状态
                        completed.IsInstalling = true;
                        completed.InstallProgressValue = 0;
                        completed.InstallStateString = PrepareInstallString;
                        (bool result, PackageDeploymentResult packageDeploymentResult, Exception exception)? installResult = await InstallAppAsync(installAppsKind, completed);

                        if (installResult.HasValue && installResult.Value.result && installResult.Value.packageDeploymentResult is not null)
                        {
                            // 安装成功
                            if (installResult.Value.packageDeploymentResult.Status is PackageDeploymentStatus.CompletedSuccess)
                            {
                                completed.InstallProgressValue = 100;
                                completed.IsInstallWaiting = false;
                                completed.InstallFailed = false;
                                completed.InstallStateString = InstallSuccessfullyString;
                                ShowInstallAppsResultNotification(completed.FileName, installResult.Value.result, installResult.Value.packageDeploymentResult, installResult.Value.exception);
                            }
                            // 安装失败
                            else if (installResult.Value.packageDeploymentResult.Status is PackageDeploymentStatus.CompletedFailure)
                            {
                                completed.InstallProgressValue = 100;
                                completed.IsInstallWaiting = false;
                                completed.InstallFailed = true;
                                completed.InstallStateString = InstallFailedString;
                                ShowInstallAppsResultNotification(completed.FileName, installResult.Value.result, installResult.Value.packageDeploymentResult, installResult.Value.exception);
                            }
                        }
                        else
                        {
                            completed.InstallProgressValue = 100;
                            completed.IsInstallWaiting = false;
                            completed.InstallFailed = true;
                            completed.InstallStateString = InstallFailedString;
                            ShowInstallAppsResultNotification(completed.FileName, installResult.Value.result, installResult.Value.packageDeploymentResult, installResult.Value.exception);
                        }

                        // 恢复原来的安装信息显示（并延缓当前安装信息显示时间0.5秒）
                        await Task.Delay(500);
                        completed.IsInstalling = false;
                        completed.InstallFailed = false;
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(CompletedPage), nameof(OnInstallExecuteRequested), 1, e);
                        return;
                    }
                }
            }
            else
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.FileLost));
            }
        }

        /// <summary>
        /// 打开当前项目存储的文件夹
        /// </summary>
        private void OnOpenFolderExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string filePath)
            {
                OpenFolder(filePath);
            }
        }

        /// <summary>
        /// 显示分享面板
        /// </summary>
        private async void OnShowShareUIExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is CompletedModel completed && File.Exists(completed.FilePath))
            {
                if (DataTransferManager.IsSupported())
                {
                    try
                    {
                        List<StorageFile> fileList = [await StorageFile.GetFileFromPathAsync(completed.FilePath)];
                        ShowShareUI(fileList);
                    }
                    catch (Exception e)
                    {
                        await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ShareFailed, false, 1));
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(CompletedPage), nameof(OnShowShareUIExecuteRequested), 1, e);
                    }
                }
            }
            else
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.FileLost));
            }
        }

        #endregion 第五部分：命令调用处理

        #region 第六部分：挂载事件处理

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
        private void OnUseInstructionClicked(object sender, RoutedEventArgs args)
        {
            if (MainWindow.Current.GetFrameContent() is DownloadPage downloadPage)
            {
                downloadPage.ShowUseInstruction();
            }
        }

        /// <summary>
        /// 进入多选模式
        /// </summary>
        private void OnSelectClicked(object sender, RoutedEventArgs args)
        {
            SelectionMode = ListViewSelectionMode.Multiple;
            foreach (CompletedModel completedItem in CompletedCollection)
            {
                completedItem.SelectionMode = ListViewSelectionMode.Multiple;
            }
        }

        /// <summary>
        /// 全选
        /// </summary>
        private void OnSelectAllClicked(object sender, RoutedEventArgs args)
        {
            CompletedListView.SelectAll();
        }

        /// <summary>
        /// 全部不选
        /// </summary>
        private void OnSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            CompletedListView.DeselectRange(new(0, (uint)CompletedListView.Items.Count));
        }

        /// <summary>
        /// 全部反选
        /// </summary>
        private void OnSelectReverseClicked(object sender, RoutedEventArgs args)
        {
            List<object> selectedItemsList = [.. CompletedListView.SelectedItems];

            foreach (object item in CompletedListView.Items)
            {
                if (selectedItemsList.Contains(item))
                {
                    CompletedListView.SelectedItems.Remove(item);
                }
                else
                {
                    CompletedListView.SelectedItems.Add(item);
                }
            }
        }

        /// <summary>
        /// 删除选中的任务
        /// </summary>
        private async void OnDeleteSelectedClicked(object sender, RoutedEventArgs args)
        {
            List<CompletedModel> selectedCompletedDataList = GetSelectedItemsList(CompletedListView.SelectedItems);

            // 没有选中任何内容时显示空提示对话框
            if (selectedCompletedDataList.Count is 0)
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.SelectEmpty));
                return;
            }

            // 当前任务正在安装时，不进行其他任何操作
            if (selectedCompletedDataList.Exists(item => item.IsInstalling))
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.InstallingNotify));
                return;
            }

            // 删除时显示删除确认对话框
            DeleteFileDialog deleteFileDialog = new();
            ContentDialogResult contentDialogResult = await MainWindow.Current.ShowDialogAsync(deleteFileDialog);

            if (contentDialogResult is ContentDialogResult.Primary)
            {
                SelectionMode = ListViewSelectionMode.None;
                foreach (CompletedModel completedItem in CompletedCollection)
                {
                    completedItem.SelectionMode = ListViewSelectionMode.None;
                }

                List<string> selectedFileList = [];

                foreach (CompletedModel completedItem in selectedCompletedDataList)
                {
                    selectedFileList.Add(completedItem.FilePath);
                }

                await DeleteDownloaodFileAsync(selectedCompletedDataList, deleteFileDialog.DeleteFileSameTime);
                CompletedResultKind = CompletedCollection.Count is 0 ? CompletedResultKind.Empty : CompletedResultKind.Successfully;
            }
        }

        /// <summary>
        /// 显示分享面板
        /// </summary>
        private async void OnShowShareUIClicked(object sender, RoutedEventArgs args)
        {
            List<CompletedModel> selectedCompletedDataList = GetSelectedItemsList(CompletedListView.SelectedItems);

            // 没有选中任何内容时显示空提示对话框
            if (selectedCompletedDataList.Count is 0)
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.SelectEmpty));
                return;
            }
            else
            {
                try
                {
                    List<string> selectedFileList = [];
                    foreach (CompletedModel completedItem in selectedCompletedDataList)
                    {
                        selectedFileList.Add(completedItem.FilePath);
                    }

                    List<StorageFile> selectedStorageFileList = await GetStorageFileListAsync(selectedFileList);
                    ShowShareUI(selectedStorageFileList);
                }
                catch (Exception e)
                {
                    await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.ShareFailed, true, selectedCompletedDataList.Count));
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(CompletedPage), nameof(OnShowShareUIClicked), 2, e);
                }
            }
        }

        /// <summary>
        /// 复制到剪贴板
        /// </summary>
        private async void OnCopyClicked(object sender, RoutedEventArgs args)
        {
            List<CompletedModel> selectedCompletedDataList = GetSelectedItemsList(CompletedListView.SelectedItems);

            // 没有选中任何内容时显示空提示对话框
            if (selectedCompletedDataList.Count is 0)
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultNotificationTip(OperationKind.SelectEmpty));
                return;
            }
            else
            {
                List<string> selectedFileList = [];
                foreach (CompletedModel completedItem in selectedCompletedDataList)
                {
                    selectedFileList.Add(completedItem.FilePath);
                }

                List<StorageFile> selectedStorageFileList = await GetStorageFileListAsync(selectedFileList);
                bool copyResult = CopyPasteHelper.CopyFileToClipBoard(selectedStorageFileList);
                await MainWindow.Current.ShowNotificationAsync(new CopyPasteMainNotificationTip(copyResult));
            }
        }

        /// <summary>
        /// 退出多选模式
        /// </summary>
        private void OnCancelClicked(object sender, RoutedEventArgs args)
        {
            SelectionMode = ListViewSelectionMode.None;
            foreach (CompletedModel completedItem in CompletedCollection)
            {
                completedItem.SelectionMode = ListViewSelectionMode.None;
            }
        }

        /// <summary>
        /// 在共享操作启动时发生的事件
        /// </summary>
        private void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args, List<StorageFile> fileList)
        {
            DataRequestDeferral dataRequestDeferral = args.Request.GetDeferral();

            try
            {
                args.Request.Data.Properties.Title = FileShareString;
                args.Request.Data.SetStorageItems(fileList);
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
            }
            finally
            {
                dataRequestDeferral.Complete();
            }
        }

        /// <summary>
        /// 应用程序退出时触发的事件
        /// </summary>
        private void OnApplicationExit()
        {
            DismountDownloadEvent();
        }

        /// <summary>
        /// 添加已下载完成任务
        /// </summary>
        private void OnStorageDataAdded(DownloadSchedulerModel downloadScheduler)
        {
            DispatcherQueue.TryEnqueue(async () =>
            {
                CompletedCollection.Add(new()
                {
                    SelectionMode = SelectionMode,
                    IconImage = await GetFileIconImageAsync(downloadScheduler.FilePath),
                    DownloadKey = downloadScheduler.DownloadKey,
                    FileName = downloadScheduler.FileName,
                    FilePath = downloadScheduler.FilePath,
                    TotalSize = downloadScheduler.TotalSize,
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
                    if (string.Equals(completedItem.DownloadKey, downloadKey))
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
                if (progress.Status is PackageDeploymentProgressStatus.Queued)
                {
                    completed.IsInstalling = true;
                    completed.IsInstallWaiting = true;
                    completed.InstallStateString = WaitInstallString;
                    completed.InstallProgressValue = Convert.ToInt32(progress.Progress * 100);
                    completed.InstallStateString = string.Format(InstallProgressString, progress.Progress * 100);
                }
                else if (progress.Status is PackageDeploymentProgressStatus.InProgress)
                {
                    completed.IsInstalling = true;
                    completed.IsInstallWaiting = false;
                    completed.InstallProgressValue = Convert.ToInt32(progress.Progress * 100);
                    completed.InstallStateString = string.Format(InstallProgressString, progress.Progress * 100);
                }
            });
        }

        #endregion 第六部分：挂载事件处理

        #region 第七部分：数据操作与业务逻辑

        /// <summary>
        /// 初始化数据
        /// </summary>
        private async Task InitializeDataAsync()
        {
            List<DownloadSchedulerModel> downloadStorageList = await GetDownloadStorageListAsync();

            if (downloadStorageList is not null)
            {
                foreach (DownloadSchedulerModel downloadSchedulerItem in downloadStorageList)
                {
                    CompletedCollection.Add(new()
                    {
                        SelectionMode = SelectionMode,
                        IconImage = await GetFileIconImageAsync(downloadSchedulerItem.FilePath),
                        DownloadKey = downloadSchedulerItem.DownloadKey,
                        FileName = downloadSchedulerItem.FileName,
                        FilePath = downloadSchedulerItem.FilePath,
                        TotalSize = downloadSchedulerItem.TotalSize
                    });
                }
            }
        }

        /// <summary>
        /// 获取下载存储数据
        /// </summary>
        private async Task<List<DownloadSchedulerModel>> GetDownloadStorageListAsync()
        {
            return await Task.Run(() =>
            {
                packageDeploymentManager = PackageDeploymentManager.GetDefault();
                DownloadStorageService.DownloadStorageSemaphoreSlim?.Wait();
                return DownloadStorageService.GetDownloadData();
            });
        }

        /// <summary>
        /// 获取选中项
        /// </summary>
        private List<CompletedModel> GetSelectedItemsList(IList<object> selectedItemsList)
        {
            List<CompletedModel> selectedCompletedDataList = [];

            foreach (object completedItemObj in selectedItemsList)
            {
                if (completedItemObj is CompletedModel completedItem)
                {
                    selectedCompletedDataList.Add(completedItem);
                }
            }

            return selectedCompletedDataList;
        }

        /// <summary>
        /// 挂载与下载相关的事件
        /// </summary>
        private async Task MountDownloadEventAsync()
        {
            await Task.Run(() =>
            {
                GlobalNotificationService.ApplicationExit += OnApplicationExit;
                DownloadStorageService.StorageDataAdded += OnStorageDataAdded;
                DownloadStorageService.StorageDataDeleted += OnStorageDataDeleted;
                DownloadStorageService.StorageDataCleared += OnStorageDataCleared;
                DownloadStorageService.DownloadStorageSemaphoreSlim?.Release();
            });
        }

        /// <summary>
        /// 卸载与下载相关的事件
        /// </summary>
        private void DismountDownloadEvent()
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
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(CompletedPage), nameof(DismountDownloadEvent), 1, e);
            }
        }

        /// <summary>
        /// 删除下载记录和文件
        /// </summary>
        private async Task DeleteDownloaodFileAsync(List<CompletedModel> selectedCompletedDataList, bool deleteFile)
        {
            if (selectedCompletedDataList is not null)
            {
                await Task.Run(() =>
                {
                    if (deleteFile)
                    {
                        foreach (CompletedModel completedItem in selectedCompletedDataList)
                        {
                            if (!File.Exists(completedItem.FilePath) || DeleteFileHelper.DeleteFileToRecycleBin(completedItem.FilePath))
                            {
                                DownloadStorageService.DeleteDownloadData(completedItem.DownloadKey);
                            }
                        }
                    }
                    else
                    {
                        foreach (CompletedModel completedItem in selectedCompletedDataList)
                        {
                            DownloadStorageService.DeleteDownloadData(completedItem.DownloadKey);
                        }
                    }
                });
            }
        }

        /// <summary>
        /// 获取应用安装类型
        /// </summary>
        private InstallAppsKind GetInstallAppsKind(string fileName)
        {
            InstallAppsKind installAppsKind = InstallAppsKind.None;
            if (!string.IsNullOrEmpty(fileName))
            {
                try
                {
                    string extension = Path.GetExtension(fileName);

                    // 普通应用：直接安装
                    if (extension.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) || extension.EndsWith(".msi", StringComparison.OrdinalIgnoreCase))
                    {
                        installAppsKind = InstallAppsKind.NonPackagedApp;
                    }
                    // 商店打包应用：使用应用安装程序安装或直接安装
                    else if (extension.EndsWith("appx", StringComparison.OrdinalIgnoreCase) || extension.EndsWith("appxbundle", StringComparison.OrdinalIgnoreCase) || extension.EndsWith("msix", StringComparison.OrdinalIgnoreCase) || extension.EndsWith("msixbundle", StringComparison.OrdinalIgnoreCase) || extension.EndsWith("appinstaller", StringComparison.OrdinalIgnoreCase))
                    {
                        // 使用应用安装程序安装
                        if (string.Equals(InstallModeService.InstallMode, InstallModeService.InstallModeList[0]))
                        {
                            installAppsKind = InstallAppsKind.PackagedAppViaAppInstaller;
                        }
                        // 直接安装
                        else if (string.Equals(InstallModeService.InstallMode, InstallModeService.InstallModeList[1]))
                        {
                            installAppsKind = InstallAppsKind.PackagedAppDirectlyInstall;
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(CompletedPage), nameof(GetInstallAppsKind), 1, e);
                }
            }
            return installAppsKind;
        }

        /// <summary>
        /// 安装应用
        /// </summary>
        private async Task<(bool, PackageDeploymentResult, Exception)?> InstallAppAsync(InstallAppsKind installAppsKind, CompletedModel completed)
        {
            if (completed is not null)
            {
                // 普通应用：直接安装
                if (installAppsKind is InstallAppsKind.NonPackagedApp)
                {
                    await Task.Run(() =>
                    {
                        Shell32Library.ShellExecute(nint.Zero, "open", completed.FilePath, string.Empty, null, WindowShowStyle.SW_SHOWNORMAL);
                    });
                    return null;
                }
                // 商店打包应用：使用应用安装程序安装或直接安装
                else if (installAppsKind is InstallAppsKind.PackagedAppViaAppInstaller || installAppsKind is InstallAppsKind.PackagedAppDirectlyInstall)
                {
                    StorageFile completedFile = await Task.Run(async () =>
                    {
                        try
                        {
                            return await StorageFile.GetFileFromPathAsync(completed.FilePath);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(CompletedPage), nameof(InstallAppAsync), 1, e);
                            return null;
                        }
                    });

                    // 使用应用安装程序安装
                    if (installAppsKind is InstallAppsKind.PackagedAppViaAppInstaller)
                    {
                        await Task.Run(async () =>
                        {
                            await Launcher.LaunchFileAsync(completedFile);
                        });
                        return null;
                    }
                    else if (installAppsKind is InstallAppsKind.PackagedAppDirectlyInstall)
                    {
                        (bool, PackageDeploymentResult, Exception) installResult = await Task.Run(async () =>
                        {
                            try
                            {
                                AddPackageOptions addPackageOptions = new()
                                {
                                    AllowUnsigned = AppInstallService.AllowUnsignedPackage,
                                    ForceAppShutdown = AppInstallService.ForceAppShutdown,
                                    ForceTargetAppShutdown = AppInstallService.ForceTargetAppShutdown,
                                    TargetVolume = PackageVolume.GetDefault()
                                };

                                // 安装目标应用，并获取安装进度
                                IAsyncOperationWithProgress<PackageDeploymentResult, PackageDeploymentProgress> installPackageWithProgress = packageDeploymentManager.AddPackageByUriAsync(new(completed.FilePath), addPackageOptions);

                                // 更新安装进度
                                installPackageWithProgress.Progress = (result, progress) => OnPackageInstallProgress(result, progress, completed);
                                return ValueTuple.Create<bool, PackageDeploymentResult, Exception>(true, await installPackageWithProgress, null);
                            }
                            // 安装失败显示失败信息
                            catch (Exception e)
                            {
                                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(CompletedPage), nameof(InstallAppAsync), 2, e);
                                return ValueTuple.Create<bool, PackageDeploymentResult, Exception>(false, null, e);
                            }
                        });
                        return installResult;
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
            else
            {
                return default;
            }
        }

        /// <summary>
        /// 显示安装应用结果通知
        /// </summary>
        private void ShowInstallAppsResultNotification(string fileName, bool result, PackageDeploymentResult packageDeploymentResult, Exception exception)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                Task.Run(() =>
                {
                    if (result && packageDeploymentResult is not null)
                    {
                        if (packageDeploymentResult.Status is PackageDeploymentStatus.CompletedSuccess)
                        {
                            // 显示安装成功通知
                            AppNotificationBuilder appNotificationBuilder = new();
                            appNotificationBuilder.AddArgument("action", "OpenApp");
                            appNotificationBuilder.AddText(string.Format(InstallSuccessfully1String, fileName));
                            ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                        }
                        else if (packageDeploymentResult.Status is PackageDeploymentStatus.CompletedFailure)
                        {
                            string errorCode = packageDeploymentResult.ExtendedError is not null ? string.Format("0x{0:X8}", packageDeploymentResult.ExtendedError.HResult) : NotAvailableString;
                            string errorMessage = packageDeploymentResult.ErrorText;

                            // 显示安装失败通知
                            AppNotificationBuilder appNotificationBuilder = new();
                            appNotificationBuilder.AddArgument("action", "OpenApp");
                            appNotificationBuilder.AddText(string.Format(InstallFailed1String, fileName));
                            appNotificationBuilder.AddText(string.Format(InstallFailed2String, errorCode));
                            appNotificationBuilder.AddText(string.Format(InstallFailed3String, errorMessage));
                            ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                        }
                    }
                    else
                    {
                        string errorCode = exception is not null ? string.Format("0x{0:X8}", exception.HResult) : NotAvailableString;
                        string errorMessage = exception is not null ? exception.Message : NotAvailableString;

                        // 显示安装失败通知
                        AppNotificationBuilder appNotificationBuilder = new();
                        appNotificationBuilder.AddArgument("action", "OpenApp");
                        appNotificationBuilder.AddText(string.Format(InstallFailed1String, fileName));
                        appNotificationBuilder.AddText(string.Format(InstallFailed2String, errorCode));
                        appNotificationBuilder.AddText(string.Format(InstallFailed3String, errorMessage));
                        ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                    }
                });
            }
        }

        /// <summary>
        /// 打开当前项目存储的文件夹
        /// </summary>
        private void OpenFolder(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                Task.Run(async () =>
                {
                    try
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
                                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(CompletedPage), nameof(OpenFolder), 1, e);
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
                            await Launcher.LaunchFolderPathAsync(DownloadOptionsService.DownloadFolder);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(CompletedPage), nameof(OpenFolder), 2, e);
                    }
                });
            }
        }

        /// <summary>
        /// 显示分享面板
        /// </summary>
        private void ShowShareUI(List<StorageFile> fileList)
        {
            if (fileList is not null)
            {
                DataTransferManager dataTransferManager = DataTransferManagerInterop.GetForWindow(Win32Interop.GetWindowFromWindowId(MainWindow.Current.AppWindow.Id));
                dataTransferManager.DataRequested += (sender, args) => OnDataRequested(sender, args, fileList);
                DataTransferManagerInterop.ShowShareUIForWindow(Win32Interop.GetWindowFromWindowId(MainWindow.Current.AppWindow.Id));
            }
        }

        /// <summary>
        /// 获取文件列表
        /// </summary>
        private async Task<List<StorageFile>> GetStorageFileListAsync(List<string> fileList)
        {
            if (fileList is not null)
            {
                List<StorageFile> storageFileList = [];
                foreach (string file in fileList)
                {
                    try
                    {
                        if (File.Exists(file))
                        {
                            storageFileList.Add(await StorageFile.GetFileFromPathAsync(file));
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(CompletedPage), nameof(GetStorageFileListAsync), 1, e);
                        continue;
                    }
                }
                return storageFileList;
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// 获取文件缩略图
        /// </summary>
        private async Task<ImageSource> GetFileIconImageAsync(string filePath)
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
                    bitmapImage.SetSource(storageItemThumbnail);
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
        private Visibility GetCompletedSuccessfullyVisibility(CompletedResultKind completedResultKind, bool isSuccessfully)
        {
            return isSuccessfully ? completedResultKind is CompletedResultKind.Successfully ? Visibility.Visible : Visibility.Collapsed : completedResultKind is CompletedResultKind.Successfully ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// 检查加载下载已完成文件是否成功
        /// </summary>
        private Visibility CheckCompletedResultKindVisibility(CompletedResultKind completedResultKind, CompletedResultKind comparedCompletedResultKind)
        {
            return Equals(completedResultKind, comparedCompletedResultKind) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取是否正在加载中
        /// </summary>

        private bool GetIsLoading(CompletedResultKind completedResultKind)
        {
            return completedResultKind is not CompletedResultKind.Loading;
        }
    }

        #endregion 第七部分：数据操作与业务逻辑
}
