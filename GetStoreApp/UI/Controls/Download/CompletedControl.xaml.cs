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
using GetStoreApp.WindowsAPI.ComTypes;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
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
        private readonly object completedLock = new object();

        private bool isUpdatingNow = false;

        private PackageManager packageManager = new PackageManager();

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

                await Task.Run(async () =>
                {
                    bool deleteResult = await DownloadXmlService.DeleteAsync(completedItem.DownloadKey);

                    if (deleteResult)
                    {
                        DispatcherQueue.TryEnqueue(async () =>
                        {
                            while (isUpdatingNow) await Task.Delay(50);
                            lock (completedLock) isUpdatingNow = true;

                            try
                            {
                                CompletedCollection.Remove(completedItem);
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(LoggingLevel.Warning, "Delete completed download record failed.", e);
                            }

                            lock (completedLock) isUpdatingNow = false;
                        });
                    }
                });
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

                await Task.Run(async () =>
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

                    bool deleteResult = await DownloadXmlService.DeleteAsync(completedItem.DownloadKey);

                    // 删除记录
                    if (deleteResult)
                    {
                        DispatcherQueue.TryEnqueue(async () =>
                        {
                            while (isUpdatingNow) await Task.Delay(50);
                            lock (completedLock) isUpdatingNow = true;

                            try
                            {
                                CompletedCollection.Remove(completedItem);
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(LoggingLevel.Warning, "Delete completed download record failed.", e);
                            }

                            lock (completedLock) isUpdatingNow = false;
                        });
                    }
                });
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
        private void OnInstallExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            CompletedModel completedItem = args.Parameter as CompletedModel;
            if (completedItem is not null)
            {
                Task.Run(async () =>
                {
                    // 普通应用：直接安装
                    if (completedItem.FilePath.EndsWith(".exe") || completedItem.FileName.EndsWith(".msi"))
                    {
                        ProcessHelper.StartProcess("explorer.exe ", completedItem.FilePath, out _);
                    }
                    // 商店打包应用：使用应用安装程序安装或直接安装
                    else
                    {
                        try
                        {
                            StorageFile completedFile = await StorageFile.GetFileFromPathAsync(completedItem.FilePath);

                            if (InstallModeService.InstallMode.Value.Equals(InstallModeService.InstallModeList[0].Value))
                            {
                                await Launcher.LaunchFileAsync(completedFile);
                            }

                            // 直接安装
                            else if (InstallModeService.InstallMode.Value.Equals(InstallModeService.InstallModeList[1].Value))
                            {
                                // 标记安装状态
                                try
                                {
                                    AutoResetEvent autoResetEvent = new AutoResetEvent(false);
                                    DispatcherQueue.TryEnqueue(() =>
                                    {
                                        lock (completedLock)
                                        {
                                            for (int index = 0; index < CompletedCollection.Count; index++)
                                            {
                                                if (CompletedCollection[index].DownloadKey.Equals(completedItem.DownloadKey))
                                                {
                                                    CompletedCollection[index].IsInstalling = true;
                                                    break;
                                                }
                                            }
                                        }
                                        autoResetEvent.Set();
                                    });

                                    autoResetEvent.WaitOne();
                                    autoResetEvent.Dispose();

                                    // 更新安装进度
                                    Progress<DeploymentProgress> progressCallBack = new Progress<DeploymentProgress>((installProgress) =>
                                    {
                                        DispatcherQueue.TryEnqueue(() =>
                                        {
                                            lock (completedLock)
                                            {
                                                for (int index = 0; index < CompletedCollection.Count; index++)
                                                {
                                                    if (CompletedCollection[index].DownloadKey.Equals(completedItem.DownloadKey))
                                                    {
                                                        CompletedCollection[index].InstallValue = installProgress.percentage;
                                                        break;
                                                    };
                                                }
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
                                            lock (completedLock)
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
        }

        /// <summary>
        /// 打开当前项目存储的文件夹
        /// </summary>
        private void OnOpenItemFolderExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            string filePath = args.Parameter as string;
            Task.Run(async () =>
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
            });
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
                    IDataTransferManagerInterop dataTransferManagerInterop = DataTransferManager.As<IDataTransferManagerInterop>();

                    DataTransferManager dataTransferManager = DataTransferManager.FromAbi(dataTransferManagerInterop.GetForWindow((IntPtr)MainWindow.Current.AppWindow.Id.Value, new Guid("A5CAEE9B-8708-49D1-8D36-67D25A8DA00C")));

                    dataTransferManager.DataRequested += async (sender, args) =>
                    {
                        DataRequestDeferral deferral = args.Request.GetDeferral();

                        args.Request.Data.Properties.Title = string.Format(ResourceService.GetLocalized("Download/ShareFileTitle"));
                        args.Request.Data.SetStorageItems(new List<StorageFile>() { await StorageFile.GetFileFromPathAsync(completedItem.FilePath) });
                        deferral.Complete();
                    };

                    dataTransferManagerInterop.ShowShareUIForWindow((IntPtr)MainWindow.Current.AppWindow.Id.Value);
                }
                catch (Exception e)
                {
                    TeachingTipHelper.Show(new ShareFailedTip(false));
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
            lock (completedLock) isUpdatingNow = true;

            foreach (CompletedModel completedItem in CompletedCollection)
            {
                completedItem.IsSelectMode = true;
                completedItem.IsSelected = false;
            }

            IsSelectMode = true;
            lock (completedLock) isUpdatingNow = false;
        }

        /// <summary>
        /// 全部选择
        /// </summary>
        private async void OnSelectAllClicked(object sender, RoutedEventArgs args)
        {
            while (isUpdatingNow) await Task.Delay(50);
            lock (completedLock) isUpdatingNow = true;

            foreach (CompletedModel completedItem in CompletedCollection)
            {
                completedItem.IsSelected = true;
            }

            lock (completedLock) isUpdatingNow = false;
        }

        /// <summary>
        ///  全部不选
        /// </summary>
        private async void OnSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            while (isUpdatingNow) await Task.Delay(50);
            lock (completedLock) isUpdatingNow = true;

            foreach (CompletedModel completedItem in CompletedCollection)
            {
                completedItem.IsSelected = false;
            }

            lock (completedLock) isUpdatingNow = false;
        }

        /// <summary>
        /// 删除选中的任务
        /// </summary>
        private async void OnDeleteSelectedClicked(object sender, RoutedEventArgs args)
        {
            List<BackgroundModel> selectedCompletedDataList = new List<BackgroundModel>();

            foreach (CompletedModel completedItem in CompletedCollection)
            {
                if (completedItem.IsSelected is true)
                {
                    selectedCompletedDataList.Add(new BackgroundModel
                    {
                        DownloadKey = completedItem.DownloadKey,
                        IsInstalling = completedItem.IsInstalling
                    });
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

                while (isUpdatingNow) await Task.Delay(50);
                lock (completedLock) isUpdatingNow = true;

                foreach (CompletedModel completedItem in CompletedCollection)
                {
                    completedItem.IsSelectMode = false;
                }

                lock (completedLock) isUpdatingNow = false;

                await Task.Run(async () =>
                {
                    await DownloadXmlService.DeleteSelectedAsync(selectedCompletedDataList);

                    DispatcherQueue.TryEnqueue(async () =>
                    {
                        while (isUpdatingNow) await Task.Delay(50);
                        lock (completedLock) isUpdatingNow = true;

                        foreach (BackgroundModel backgroundItem in selectedCompletedDataList)
                        {
                            foreach (CompletedModel completedItem in CompletedCollection)
                            {
                                if (completedItem.DownloadKey.Equals(backgroundItem.DownloadKey, StringComparison.OrdinalIgnoreCase))
                                {
                                    try
                                    {
                                        CompletedCollection.Remove(completedItem);
                                    }
                                    catch (Exception e)
                                    {
                                        LogService.WriteLog(LoggingLevel.Warning, "Delete completed download record failed.", e);
                                    }
                                    break;
                                }
                            }
                        }

                        lock (completedLock) isUpdatingNow = false;
                    });
                });
            }
        }

        /// <summary>
        /// 删除选中的任务（包括文件）
        /// </summary>
        private async void OnDeleteSelectedWithFileClicked(object sender, RoutedEventArgs args)
        {
            List<BackgroundModel> selectedCompletedDataList = new List<BackgroundModel>();

            foreach (CompletedModel completedItem in CompletedCollection)
            {
                if (completedItem.IsSelected is true)
                {
                    selectedCompletedDataList.Add(new BackgroundModel
                    {
                        DownloadKey = completedItem.DownloadKey,
                        FilePath = completedItem.FilePath,
                        IsInstalling = completedItem.IsInstalling,
                    });
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
            ContentDialogResult result = await ContentDialogHelper.ShowAsync(new DeletePromptDialog(DeleteKind.DownloadWithFile), this);

            if (result is ContentDialogResult.Primary)
            {
                IsSelectMode = false;

                while (isUpdatingNow) await Task.Delay(50);
                lock (completedLock) isUpdatingNow = true;

                foreach (CompletedModel completedItem in CompletedCollection)
                {
                    completedItem.IsSelectMode = false;
                }

                lock (completedLock) isUpdatingNow = false;

                await Task.Run(async () =>
                {
                    foreach (BackgroundModel completedItem in selectedCompletedDataList)
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
                            bool deleteResult = await DownloadXmlService.DeleteAsync(completedItem.DownloadKey);

                            if (deleteResult)
                            {
                                AutoResetEvent autoResetEvent = new AutoResetEvent(false);
                                DispatcherQueue.TryEnqueue(async () =>
                                {
                                    while (isUpdatingNow) await Task.Delay(50);
                                    lock (completedLock) isUpdatingNow = true;

                                    for (int index = 0; index < CompletedCollection.Count; index++)
                                    {
                                        if (CompletedCollection[index].DownloadKey.Equals(completedItem.DownloadKey, StringComparison.OrdinalIgnoreCase))
                                        {
                                            try
                                            {
                                                CompletedCollection.RemoveAt(index);
                                            }
                                            catch (Exception e)
                                            {
                                                LogService.WriteLog(LoggingLevel.Warning, "Delete completed download record failed.", e);
                                            }
                                            break;
                                        }
                                    }

                                    lock (completedLock) isUpdatingNow = false;
                                    autoResetEvent.Set();
                                });

                                autoResetEvent.WaitOne();
                                autoResetEvent.Dispose();
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LoggingLevel.Warning, "Delete completed download record failed.", e);
                            continue;
                        }
                    }
                });
            }
        }

        /// <summary>
        /// 分享选中的文件
        /// </summary>
        private void OnShareSelectedFileClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                List<BackgroundModel> selectedCompletedDataList = new List<BackgroundModel>();

                foreach (CompletedModel completedItem in CompletedCollection)
                {
                    if (completedItem.IsSelected is true)
                    {
                        selectedCompletedDataList.Add(new BackgroundModel
                        {
                            DownloadKey = completedItem.DownloadKey,
                            FilePath = completedItem.FilePath,
                            IsInstalling = completedItem.IsInstalling,
                        });
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

                DispatcherQueue.TryEnqueue(() =>
                {
                    try
                    {
                        IDataTransferManagerInterop dataTransferManagerInterop = DataTransferManager.As<IDataTransferManagerInterop>();

                        DataTransferManager dataTransferManager = DataTransferManager.FromAbi(dataTransferManagerInterop.GetForWindow((IntPtr)MainWindow.Current.AppWindow.Id.Value, new Guid("A5CAEE9B-8708-49D1-8D36-67D25A8DA00C")));

                        dataTransferManager.DataRequested += async (sender, args) =>
                        {
                            DataRequestDeferral deferral = args.Request.GetDeferral();

                            args.Request.Data.Properties.Title = string.Format(ResourceService.GetLocalized("Download/ShareFileTitle"));

                            List<StorageFile> selectedFileList = new List<StorageFile>();
                            foreach (BackgroundModel completedItem in selectedCompletedDataList)
                            {
                                try
                                {
                                    selectedFileList.Add(await StorageFile.GetFileFromPathAsync(completedItem.FilePath));
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

                        dataTransferManagerInterop.ShowShareUIForWindow((IntPtr)MainWindow.Current.AppWindow.Id.Value);
                    }
                    catch (Exception e)
                    {
                        TeachingTipHelper.Show(new ShareFailedTip(true, selectedCompletedDataList.Count));
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
        private async void OnItemClicked(object sender, ItemClickEventArgs args)
        {
            CompletedModel completedItem = (CompletedModel)args.ClickedItem;
            int clickedIndex = CompletedCollection.IndexOf(completedItem);

            while (isUpdatingNow) await Task.Delay(50);
            lock (completedLock) isUpdatingNow = true;

            if (clickedIndex >= 0 && clickedIndex < CompletedCollection.Count)
            {
                CompletedCollection[clickedIndex].IsSelected = !CompletedCollection[clickedIndex].IsSelected;
            }

            lock (completedLock) isUpdatingNow = false;
        }

        #endregion 第二部分：已下载完成控件——挂载的事件

        #region 第三部分：自定义事件

        /// <summary>
        /// 订阅事件，下载中列表内容有完成项目时通知UI更改
        /// </summary>
        public async void OnDownloadingListItemsChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (SelectedIndex is 2)
            {
                if (args.Action is NotifyCollectionChangedAction.Remove)
                {
                    foreach (object oldItem in args.OldItems)
                    {
                        BackgroundModel backgroundItem = oldItem as BackgroundModel;
                        if (backgroundItem is not null)
                        {
                            if (backgroundItem.DownloadFlag is 4)
                            {
                                BackgroundModel completedItem = await DownloadXmlService.QueryWithKeyAsync(backgroundItem.DownloadKey);

                                DispatcherQueue.TryEnqueue(async () =>
                                {
                                    while (isUpdatingNow) await Task.Delay(50);
                                    lock (completedLock) isUpdatingNow = true;

                                    CompletedCollection.Add(new CompletedModel
                                    {
                                        DownloadKey = completedItem.DownloadKey,
                                        FileName = completedItem.FileName,
                                        FileLink = completedItem.FileLink,
                                        FilePath = completedItem.FilePath,
                                        TotalSize = completedItem.TotalSize,
                                        DownloadFlag = completedItem.DownloadFlag
                                    });

                                    lock (completedLock) isUpdatingNow = false;
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
            List<BackgroundModel> downloadRawList = await DownloadXmlService.QueryWithFlagAsync(4);

            while (isUpdatingNow) await Task.Delay(50);
            lock (completedLock) isUpdatingNow = true;

            CompletedCollection.Clear();

            foreach (BackgroundModel downloadRawData in downloadRawList)
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

            lock (completedLock) isUpdatingNow = false;
        }
    }
}
