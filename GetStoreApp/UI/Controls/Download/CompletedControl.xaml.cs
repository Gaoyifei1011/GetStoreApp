using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Controls.Extensions;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Download;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.Common;
using GetStoreApp.UI.Dialogs.Download;
using GetStoreApp.UI.Notifications;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation.Diagnostics;
using Windows.Management.Deployment;
using Windows.Storage;
using Windows.System;

namespace GetStoreApp.UI.Controls.Download
{
    /// <summary>
    /// 下载页面：已下载完成控件
    /// </summary>
    public sealed partial class CompletedControl : Grid, INotifyPropertyChanged
    {
        private readonly object CompletedLock = new object();

        private bool isUpdatingNow = false;

        public int SelectedIndex { get; set; } = 0;

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

        private ObservableCollection<CompletedModel> CompletedCollection { get; } = new ObservableCollection<CompletedModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        public CompletedControl()
        {
            InitializeComponent();

            // 订阅事件
            DownloadSchedulerService.DownloadingCollection.CollectionChanged += OnDownloadingListItemsChanged;
        }

        #region 第一部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 删除当前任务
        /// </summary>
        private async void OnDeleteExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            CompletedModel completedItem = args.Parameter as CompletedModel;
            if (completedItem is not null)
            {
                if (completedItem.IsInstalling is true)
                {
                    await ContentDialogHelper.ShowAsync(new InstallingNotifyDialog(), this);
                    return;
                }

                bool DeleteResult = await DownloadXmlService.DeleteAsync(completedItem.DownloadKey);

                if (DeleteResult)
                {
                    while (isUpdatingNow) await Task.Delay(50);
                    lock (CompletedLock) isUpdatingNow = true;

                    try
                    {
                        CompletedCollection.Remove(completedItem);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Warning, "Delete completed download record failed.", e);
                    }

                    lock (CompletedLock) isUpdatingNow = false;
                }
            }
        }

        /// <summary>
        /// 删除当前任务（包括文件）
        /// </summary>
        private async void OnDeleteWithFileExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            CompletedModel completedItem = args.Parameter as CompletedModel;
            if (completedItem is not null)
            {
                if (completedItem.IsInstalling is true)
                {
                    await ContentDialogHelper.ShowAsync(new InstallingNotifyDialog(), this);
                    return;
                }

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

                bool DeleteResult = await DownloadXmlService.DeleteAsync(completedItem.DownloadKey);

                // 删除记录
                if (DeleteResult)
                {
                    while (isUpdatingNow) await Task.Delay(50);
                    lock (CompletedLock) isUpdatingNow = true;

                    try
                    {
                        CompletedCollection.Remove(completedItem);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Warning, "Delete completed download record failed.", e);
                    }

                    lock (CompletedLock) isUpdatingNow = false;
                }
            }
        }

        /// <summary>
        /// 查看文件信息
        /// </summary>
        private async void OnFileInformationExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            CompletedModel completedItem = args.Parameter as CompletedModel;

            if (completedItem is not null)
            {
                await ContentDialogHelper.ShowAsync(new FileInformationDialog(completedItem), this);
            }
        }

        /// <summary>
        /// 安装应用
        /// </summary>
        private async void OnInstallExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            CompletedModel completedItem = args.Parameter as CompletedModel;
            if (completedItem is not null)
            {
                // 使用应用安装程序安装
                if (!string.IsNullOrEmpty(completedItem.FilePath))
                {
                    try
                    {
                        StorageFile CompletedFile = await StorageFile.GetFileFromPathAsync(completedItem.FilePath);

                        if (InstallModeService.InstallMode.Value.Equals(InstallModeService.InstallModeList[0].Value))
                        {
                            await Launcher.LaunchFileAsync(CompletedFile);
                        }

                        // 直接安装
                        else if (InstallModeService.InstallMode.Value.Equals(InstallModeService.InstallModeList[1].Value))
                        {
                            // 标记安装状态
                            try
                            {
                                int InstallIndex = CompletedCollection.IndexOf(CompletedCollection.First(item => item.DownloadKey == completedItem.DownloadKey));

                                if (InstallIndex is not -1 && InstallIndex < CompletedCollection.Count)
                                {
                                    CompletedCollection[InstallIndex].IsInstalling = true;

                                    PackageManager packageManager = new PackageManager();

                                    // 更新安装进度
                                    Progress<DeploymentProgress> progressCallBack = new Progress<DeploymentProgress>((installProgress) =>
                                    {
                                        CompletedCollection[InstallIndex].InstallValue = installProgress.percentage;
                                    });

                                    try
                                    {
                                        // 安装目标应用
                                        DeploymentResult InstallResult = await packageManager.AddPackageAsync(new Uri(completedItem.FilePath), null, DeploymentOptions.None).AsTask(progressCallBack);
                                        // 显示安装成功通知
                                        ToastNotificationService.Show(NotificationKind.InstallApp, "Successfully", CompletedFile.Name);
                                    }
                                    // 安装失败显示失败信息
                                    catch (Exception e)
                                    {
                                        CompletedCollection[InstallIndex].InstallError = true;
                                        // 显示安装失败通知
                                        ToastNotificationService.Show(NotificationKind.InstallApp, "Error", CompletedFile.Name, e.Message);
                                    }
                                    // 恢复原来的安装信息显示（并延缓当前安装信息显示时间0.5秒）
                                    finally
                                    {
                                        await Task.Delay(500);
                                        CompletedCollection[InstallIndex].IsInstalling = false;
                                        CompletedCollection[InstallIndex].InstallError = false;
                                    }
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
            }
        }

        /// <summary>
        /// 打开当前项目存储的文件夹
        /// </summary>
        private async void OnOpenItemFolderExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            string filePath = args.Parameter as string;
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
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Warning, "Completed download item folder located failed.", e);
                        await Launcher.LaunchFolderPathAsync(InfoHelper.UserDataPath.Desktop);
                    }
                }
                else
                {
                    await Launcher.LaunchFolderPathAsync(InfoHelper.UserDataPath.Desktop);
                }
            }
        }

        /// <summary>
        /// 共享文件
        /// </summary>
        private void OnShareFileExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            CompletedModel completedItem = args.Parameter as CompletedModel;

            if (completedItem is not null)
            {
                try
                {
                    if (RuntimeHelper.IsElevated)
                    {
                        Task.Run(async () =>
                        {
                            List<StorageFile> selectedFileList = new List<StorageFile>() { await StorageFile.GetFileFromPathAsync(completedItem.FilePath) };
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                CopyPasteHelper.CopyFilesToClipBoard(selectedFileList);
                                new DataCopyNotification(this, DataCopyKind.ShareFile, false).Show();
                            });
                        });
                    }
                    else
                    {
                        DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();

                        dataTransferManager.DataRequested += async (sender, args) =>
                        {
                            DataRequestDeferral deferral = args.Request.GetDeferral();

                            args.Request.Data.Properties.Title = string.Format(ResourceService.GetLocalized("Download/ShareFileTitle"));
                            args.Request.Data.SetStorageItems(new List<StorageFile>() { await StorageFile.GetFileFromPathAsync(completedItem.FilePath) });
                            deferral.Complete();
                        };

                        ShareUIOptions options = new ShareUIOptions();
                        options.Theme = (ShareUITheme)ActualTheme;
                        DataTransferManager.ShowShareUI(options);
                    }
                }
                catch (Exception e)
                {
                    new ShareFailedNotification(this, false).Show();
                    LogService.WriteLog(LoggingLevel.Warning, "Share file failed.", e);
                }
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
        private async void OnSelectClicked(object sender, RoutedEventArgs args)
        {
            while (isUpdatingNow) await Task.Delay(50);
            lock (CompletedLock) isUpdatingNow = true;

            foreach (CompletedModel completedItem in CompletedCollection)
            {
                completedItem.IsSelectMode = true;
                completedItem.IsSelected = false;
            }

            IsSelectMode = true;
            lock (CompletedLock) isUpdatingNow = false;
        }

        /// <summary>
        /// 全部选择
        /// </summary>
        private async void OnSelectAllClicked(object sender, RoutedEventArgs args)
        {
            while (isUpdatingNow) await Task.Delay(50);
            lock (CompletedLock) isUpdatingNow = true;

            foreach (CompletedModel completedItem in CompletedCollection)
            {
                completedItem.IsSelected = true;
            }

            lock (CompletedLock) isUpdatingNow = false;
        }

        /// <summary>
        ///  全部不选
        /// </summary>
        private async void OnSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            while (isUpdatingNow) await Task.Delay(50);
            lock (CompletedLock) isUpdatingNow = true;

            foreach (CompletedModel completedItem in CompletedCollection)
            {
                completedItem.IsSelected = false;
            }

            lock (CompletedLock) isUpdatingNow = false;
        }

        /// <summary>
        /// 显示删除选项浮出控件
        /// </summary>
        private void OnDeleteOptionsClicked(object sender, RoutedEventArgs args)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        /// <summary>
        /// 删除选中的任务
        /// </summary>
        private async void OnDeleteSelectedClicked(object sender, RoutedEventArgs args)
        {
            List<BackgroundModel> SelectedCompletedDataList = new List<BackgroundModel>();

            foreach (CompletedModel completedItem in CompletedCollection.Where(item => item.IsSelected is true))
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
                await ContentDialogHelper.ShowAsync(new SelectEmptyPromptDialog(), this);
                return;
            }

            // 当前任务正在安装时，不进行其他任何操作
            if (SelectedCompletedDataList.Exists(item => item.IsInstalling is true))
            {
                await ContentDialogHelper.ShowAsync(new InstallingNotifyDialog(), this);
                return;
            }

            // 删除时显示删除确认对话框
            ContentDialogResult result = await ContentDialogHelper.ShowAsync(new DeletePromptDialog(DeleteKind.Download), this);

            if (result is ContentDialogResult.Primary)
            {
                IsSelectMode = false;

                bool DeleteSelectedResult = await DownloadXmlService.DeleteSelectedAsync(SelectedCompletedDataList);

                while (isUpdatingNow) await Task.Delay(50);
                lock (CompletedLock) isUpdatingNow = true;

                foreach (CompletedModel completedItem in CompletedCollection)
                {
                    completedItem.IsSelectMode = false;
                }

                foreach (BackgroundModel backgroundItem in SelectedCompletedDataList)
                {
                    try
                    {
                        CompletedCollection.Remove(CompletedCollection.First(item => item.DownloadKey == backgroundItem.DownloadKey));
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Warning, "Delete completed download record failed.", e);
                        continue;
                    }
                }

                lock (CompletedLock) isUpdatingNow = false;
            }
        }

        /// <summary>
        /// 删除选中的任务（包括文件）
        /// </summary>
        private async void OnDeleteSelectedWithFileClicked(object sender, RoutedEventArgs args)
        {
            List<BackgroundModel> SelectedCompletedDataList = new List<BackgroundModel>();

            foreach (CompletedModel completedItem in CompletedCollection.Where(item => item.IsSelected is true))
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
                await ContentDialogHelper.ShowAsync(new SelectEmptyPromptDialog(), this);
                return;
            }

            // 当前任务正在安装时，不进行其他任何操作
            if (SelectedCompletedDataList.Exists(item => item.IsInstalling is true))
            {
                await ContentDialogHelper.ShowAsync(new InstallingNotifyDialog(), this);
                return;
            }

            // 删除时显示删除确认对话框
            ContentDialogResult result = await ContentDialogHelper.ShowAsync(new DeletePromptDialog(DeleteKind.DownloadWithFile), this);

            if (result is ContentDialogResult.Primary)
            {
                IsSelectMode = false;

                while (isUpdatingNow) await Task.Delay(50);
                lock (CompletedLock) isUpdatingNow = true;

                foreach (CompletedModel completedItem in CompletedCollection)
                {
                    completedItem.IsSelectMode = false;
                }

                foreach (BackgroundModel completedItem in SelectedCompletedDataList)
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

                    // 删除记录
                    try
                    {
                        bool DeleteResult = await DownloadXmlService.DeleteAsync(completedItem.DownloadKey);

                        if (DeleteResult)
                        {
                            CompletedCollection.Remove(CompletedCollection.First(item => item.DownloadKey == completedItem.DownloadKey));
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Warning, "Delete completed download record failed.", e);
                        continue;
                    }
                }

                lock (CompletedLock) isUpdatingNow = false;
            }
        }

        /// <summary>
        /// 分享选中的文件
        /// </summary>
        private async void OnShareSelectedFileClicked(object sender, RoutedEventArgs args)
        {
            List<BackgroundModel> SelectedCompletedDataList = new List<BackgroundModel>();

            foreach (CompletedModel completedItem in CompletedCollection.Where(item => item.IsSelected is true))
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
                await ContentDialogHelper.ShowAsync(new SelectEmptyPromptDialog(), this);
                return;
            }

            try
            {
                if (RuntimeHelper.IsElevated)
                {
                    await Task.Run(async () =>
                    {
                        List<StorageFile> SelectedFileList = new List<StorageFile>();
                        foreach (BackgroundModel completedItem in SelectedCompletedDataList)
                        {
                            SelectedFileList.Add(await StorageFile.GetFileFromPathAsync(completedItem.FilePath));
                        }
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            CopyPasteHelper.CopyFilesToClipBoard(SelectedFileList);
                            new DataCopyNotification(this, DataCopyKind.ShareFile, true, SelectedFileList.Count).Show();
                        });
                    });
                }
                else
                {
                    DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();

                    dataTransferManager.DataRequested += async (sender, args) =>
                    {
                        DataRequestDeferral deferral = args.Request.GetDeferral();

                        args.Request.Data.Properties.Title = string.Format(ResourceService.GetLocalized("Download/ShareFileTitle"));

                        List<StorageFile> SelectedFileList = new List<StorageFile>();
                        foreach (BackgroundModel completedItem in SelectedCompletedDataList)
                        {
                            SelectedFileList.Add(await StorageFile.GetFileFromPathAsync(completedItem.FilePath));
                        }
                        args.Request.Data.SetStorageItems(SelectedFileList);
                        deferral.Complete();
                    };

                    ShareUIOptions options = new ShareUIOptions();
                    options.Theme = (ShareUITheme)ActualTheme;
                    DataTransferManager.ShowShareUI(options);
                }
            }
            catch (Exception e)
            {
                new ShareFailedNotification(this, true, SelectedCompletedDataList.Count).Show();
                LogService.WriteLog(LoggingLevel.Warning, "Share selected files failed.", e);
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
        private async void OnItemClicked(object sender, ItemClickEventArgs args)
        {
            CompletedModel completedItem = (CompletedModel)args.ClickedItem;
            int ClickedIndex = CompletedCollection.IndexOf(completedItem);

            while (isUpdatingNow) await Task.Delay(50);
            lock (CompletedLock) isUpdatingNow = true;

            if (ClickedIndex >= 0 && ClickedIndex < CompletedCollection.Count)
            {
                CompletedCollection[ClickedIndex].IsSelected = !CompletedCollection[ClickedIndex].IsSelected;
            }

            lock (CompletedLock) isUpdatingNow = false;
        }

        #endregion 第二部分：已下载完成控件——挂载的事件

        #region 第三部分：自定义事件

        /// <summary>
        /// 订阅事件，下载中列表内容有完成项目时通知UI更改
        /// </summary>
        public void OnDownloadingListItemsChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (SelectedIndex is 2)
            {
                if (args.Action is NotifyCollectionChangedAction.Remove)
                {
                    foreach (object item in args.OldItems)
                    {
                        BackgroundModel backgroundItem = item as BackgroundModel;
                        if (backgroundItem is not null)
                        {
                            if (backgroundItem.DownloadFlag is 4)
                            {
                                DispatcherQueue.TryEnqueue(async () =>
                                {
                                    while (isUpdatingNow) await Task.Delay(50);
                                    lock (CompletedLock) isUpdatingNow = true;

                                    BackgroundModel item = await DownloadXmlService.QueryWithKeyAsync(backgroundItem.DownloadKey);

                                    CompletedCollection.Add(new CompletedModel
                                    {
                                        DownloadKey = item.DownloadKey,
                                        FileName = item.FileName,
                                        FileLink = item.FileLink,
                                        FilePath = item.FilePath,
                                        TotalSize = item.TotalSize,
                                        DownloadFlag = item.DownloadFlag
                                    });

                                    lock (CompletedLock) isUpdatingNow = false;
                                });
                            }
                        }
                    }
                }
            }
        }

        #endregion 第三部分：自定义事件

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 本地化已下载完成数量统计信息
        /// </summary>
        private string LocalizeCompletedCountInfo(int count)
        {
            if (count is 0)
            {
                return ResourceService.GetLocalized("Download/CompletedEmpty");
            }
            else
            {
                return string.Format(ResourceService.GetLocalized("Download/CompletedCountInfo"), count);
            }
        }

        /// <summary>
        /// 从数据库中加载已下载完成的数据
        /// </summary>
        public async Task GetCompletedDataListAsync()
        {
            List<BackgroundModel> DownloadRawList = await DownloadXmlService.QueryWithFlagAsync(4);

            while (isUpdatingNow) await Task.Delay(50);
            lock (CompletedLock) isUpdatingNow = true;

            CompletedCollection.Clear();

            foreach (BackgroundModel downloadRawData in DownloadRawList)
            {
                CompletedCollection.Add(new CompletedModel
                {
                    DownloadKey = downloadRawData.DownloadKey,
                    FileName = downloadRawData.FileName,
                    FileLink = downloadRawData.FileLink,
                    FilePath = downloadRawData.FilePath,
                    TotalSize = downloadRawData.TotalSize,
                    DownloadFlag = downloadRawData.DownloadFlag
                });
                await Task.Delay(1);
            }

            lock (CompletedLock) isUpdatingNow = false;
        }
    }
}
