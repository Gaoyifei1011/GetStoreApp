using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.DataType.Events;
using GetStoreApp.Models.Controls.Download;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings.Advanced;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.Common;
using GetStoreApp.UI.Dialogs.Download;
using GetStoreApp.UI.Notifications;
using GetStoreApp.ViewModels.Base;
using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Management.Deployment;
using Windows.Storage;
using Windows.System;
using WinRT;

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

        private bool isUpdatingNow = false;

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

        // 安装应用
        public XamlUICommand InstallCommand { get; } = new XamlUICommand();

        // 打开当前项目存储的文件夹
        public XamlUICommand OpenItemFolderCommand { get; } = new XamlUICommand();

        // 删除当前任务
        public XamlUICommand DeleteCommand { get; } = new XamlUICommand();

        // 删除当前任务（包括文件）
        public XamlUICommand DeleteWithFileCommand { get; } = new XamlUICommand();

        // 共享文件
        public XamlUICommand ShareFileCommand { get; } = new XamlUICommand();

        // 查看文件信息
        public XamlUICommand FileInformationCommand { get; } = new XamlUICommand();

        /// <summary>
        /// 打开默认保存的文件夹
        /// </summary>
        public async void OnOpenFolderClicked(object sender, RoutedEventArgs args)
        {
            await DownloadOptionsService.OpenFolderAsync(DownloadOptionsService.DownloadFolder);
        }

        /// <summary>
        /// 进入多选模式
        /// </summary>
        public async void OnSelectClicked(object sender, RoutedEventArgs args)
        {
            while (isUpdatingNow) await Task.Delay(50);
            lock (CompletedDataListLock) isUpdatingNow = true;

            foreach (CompletedModel completedItem in CompletedDataList)
            {
                completedItem.IsSelected = false;
            }

            IsSelectMode = true;
            lock (CompletedDataListLock) isUpdatingNow = false;
        }

        /// <summary>
        /// 全部选择
        /// </summary>
        public async void OnSelectAllClicked(object sender, RoutedEventArgs args)
        {
            while (isUpdatingNow) await Task.Delay(50);
            lock (CompletedDataListLock) isUpdatingNow = true;

            foreach (CompletedModel completedItem in CompletedDataList)
            {
                completedItem.IsSelected = true;
            }

            lock (CompletedDataListLock) isUpdatingNow = false;
        }

        /// <summary>
        ///  全部不选
        /// </summary>
        public async void OnSelectNoneClicked(object sender, RoutedEventArgs args)
        {
            while (isUpdatingNow) await Task.Delay(50);
            lock (CompletedDataListLock) isUpdatingNow = true;

            foreach (CompletedModel completedItem in CompletedDataList)
            {
                completedItem.IsSelected = false;
            }

            lock (CompletedDataListLock) isUpdatingNow = false;
        }

        /// <summary>
        /// 删除选中的任务
        /// </summary>
        public async void OnDelectSelectedClicked(object sender, RoutedEventArgs args)
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

                bool DeleteSelectedResult = await DownloadXmlService.DeleteSelectedAsync(SelectedCompletedDataList);

                while (isUpdatingNow) await Task.Delay(50);
                lock (CompletedDataListLock) isUpdatingNow = true;

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

                lock (CompletedDataListLock) isUpdatingNow = false;
            }
        }

        /// <summary>
        /// 删除选中的任务（包括文件）
        /// </summary>
        public async void OnDeleteSelectedWithFileClicked(object sender, RoutedEventArgs args)
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

                while (isUpdatingNow) await Task.Delay(50);
                lock (CompletedDataListLock) isUpdatingNow = true;

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
                    catch (Exception) { }

                    // 删除记录
                    try
                    {
                        bool DeleteResult = await DownloadXmlService.DeleteAsync(completedItem.DownloadKey);

                        if (DeleteResult)
                        {
                            CompletedDataList.Remove(CompletedDataList.First(item => item.DownloadKey == item.DownloadKey));
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                lock (CompletedDataListLock) isUpdatingNow = false;
            }
        }

        /// <summary>
        /// 分享选中的文件
        /// </summary>
        public async void OnShareSelectedFileClicked(object sender, RoutedEventArgs args)
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

            try
            {
                IDataTransferManagerInterop interop = DataTransferManager.As<IDataTransferManagerInterop>();

                IntPtr result = interop.GetForWindow(Program.ApplicationRoot.MainWindow.GetMainWindowHandle(), new Guid(0xa5caee9b, 0x8708, 0x49d1, 0x8d, 0x36, 0x67, 0xd2, 0x5a, 0x8d, 0xa0, 0x0c));

                DataTransferManager dataTransferManager = MarshalInterface<DataTransferManager>.FromAbi(result);

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

                interop.ShowShareUIForWindow(Program.ApplicationRoot.MainWindow.GetMainWindowHandle());
            }
            catch (Exception)
            {
                new ShareFailedNotification(true, SelectedCompletedDataList.Count).Show();
            }
        }

        /// <summary>
        /// 退出多选模式
        /// </summary>
        public void OnCancelClicked(object sender, RoutedEventArgs args)
        {
            IsSelectMode = false;
        }

        /// <summary>
        /// 在多选模式下点击项目选择相应的条目
        /// </summary>
        public async void OnItemClicked(object sender, ItemClickEventArgs args)
        {
            CompletedModel completedItem = (CompletedModel)args.ClickedItem;
            int ClickedIndex = CompletedDataList.IndexOf(completedItem);

            while (isUpdatingNow) await Task.Delay(50);
            lock (CompletedDataListLock) isUpdatingNow = true;

            if (ClickedIndex >= 0 && ClickedIndex < CompletedDataList.Count)
            {
                CompletedDataList[ClickedIndex].IsSelected = !CompletedDataList[ClickedIndex].IsSelected;
            }

            lock (CompletedDataListLock) isUpdatingNow = false;
        }

        /// <summary>
        /// 订阅事件，下载中列表内容有完成项目时通知UI更改
        /// </summary>
        public void OnDownloadingListItemsChanged(object sender, ItemsChangedEventArgs<BackgroundModel> args)
        {
            if (args.RemovedItems.Any(item => item.DownloadFlag is 4))
            {
                Program.ApplicationRoot.MainWindow.DispatcherQueue.TryEnqueue(async () =>
                {
                    while (isUpdatingNow) await Task.Delay(50);
                    lock (CompletedDataListLock) isUpdatingNow = true;

                    foreach (BackgroundModel backgroundItem in args.RemovedItems)
                    {
                        if (backgroundItem.DownloadFlag is 4)
                        {
                            BackgroundModel item = await DownloadXmlService.QueryWithKeyAsync(backgroundItem.DownloadKey);

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

                    lock (CompletedDataListLock) isUpdatingNow = false;
                });
            }
        }

        public CompletedViewModel()
        {
            InstallCommand.ExecuteRequested += async (sender, args) =>
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

                            if (InstallModeService.InstallMode.InternalName == InstallModeService.InstallModeList[0].InternalName)
                            {
                                await Launcher.LaunchFileAsync(CompletedFile);
                            }

                            // 直接安装
                            else if (InstallModeService.InstallMode.InternalName == InstallModeService.InstallModeList[1].InternalName)
                            {
                                // 标记安装状态
                                try
                                {
                                    int InstallIndex = CompletedDataList.IndexOf(CompletedDataList.First(item => item.DownloadKey == completedItem.DownloadKey));

                                    if (InstallIndex is not -1 && InstallIndex < CompletedDataList.Count)
                                    {
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
                                            AppNotificationService.Show(NotificationArgs.InstallApp, "Successfully", CompletedFile.Name);
                                        }
                                        // 安装失败显示失败信息
                                        catch (Exception e)
                                        {
                                            CompletedDataList[InstallIndex].InstallError = true;
                                            // 显示安装失败通知
                                            AppNotificationService.Show(NotificationArgs.InstallApp, "Error", CompletedFile.Name, e.Message);
                                        }
                                        // 恢复原来的安装信息显示（并延缓当前安装信息显示时间0.5秒）
                                        finally
                                        {
                                            await Task.Delay(500);
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
                        }
                        catch (Exception)
                        {
                            return;
                        }
                    }
                }
            };

            OpenItemFolderCommand.ExecuteRequested += async (sender, args) =>
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
            };

            DeleteCommand.ExecuteRequested += async (sender, args) =>
            {
                CompletedModel completedItem = args.Parameter as CompletedModel;
                if (completedItem is not null)
                {
                    if (completedItem.IsInstalling is true)
                    {
                        await new InstallingNotifyDialog().ShowAsync();
                        return;
                    }

                    bool DeleteResult = await DownloadXmlService.DeleteAsync(completedItem.DownloadKey);

                    if (DeleteResult)
                    {
                        while (isUpdatingNow) await Task.Delay(50);
                        lock (CompletedDataListLock) isUpdatingNow = true;

                        try
                        {
                            CompletedDataList.Remove(completedItem);
                        }
                        catch (Exception) { }

                        lock (CompletedDataListLock) isUpdatingNow = false;
                    }
                }
            };

            DeleteWithFileCommand.ExecuteRequested += async (sender, args) =>
            {
                CompletedModel completedItem = args.Parameter as CompletedModel;
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
                        if (File.Exists(completedItem.FilePath))
                        {
                            File.Delete(completedItem.FilePath);
                        }
                    }
                    catch (Exception) { }

                    bool DeleteResult = await DownloadXmlService.DeleteAsync(completedItem.DownloadKey);

                    // 删除记录
                    if (DeleteResult)
                    {
                        while (isUpdatingNow) await Task.Delay(50);
                        lock (CompletedDataListLock) isUpdatingNow = true;

                        try
                        {
                            CompletedDataList.Remove(completedItem);
                        }
                        catch (Exception) { }

                        lock (CompletedDataListLock) isUpdatingNow = false;
                    }
                }
            };

            ShareFileCommand.ExecuteRequested += (sender, args) =>
            {
                CompletedModel completedItem = args.Parameter as CompletedModel;

                if (completedItem is not null)
                {
                    try
                    {
                        IDataTransferManagerInterop interop = DataTransferManager.As<IDataTransferManagerInterop>();

                        IntPtr result = interop.GetForWindow(Program.ApplicationRoot.MainWindow.GetMainWindowHandle(), new Guid(0xa5caee9b, 0x8708, 0x49d1, 0x8d, 0x36, 0x67, 0xd2, 0x5a, 0x8d, 0xa0, 0x0c));

                        DataTransferManager dataTransferManager = MarshalInterface<DataTransferManager>.FromAbi(result);

                        dataTransferManager.DataRequested += async (sender, args) =>
                        {
                            DataRequestDeferral deferral = args.Request.GetDeferral();

                            args.Request.Data.Properties.Title = string.Format(ResourceService.GetLocalized("Download/ShareFileTitle"));
                            args.Request.Data.SetStorageItems(new List<StorageFile>() { await StorageFile.GetFileFromPathAsync(completedItem.FilePath) });
                            deferral.Complete();
                        };

                        interop.ShowShareUIForWindow(Program.ApplicationRoot.MainWindow.GetMainWindowHandle());
                    }
                    catch (Exception)
                    {
                        new ShareFailedNotification(false).Show();
                    }
                }
            };

            FileInformationCommand.ExecuteRequested += async (sender, args) =>
            {
                CompletedModel completedItem = args.Parameter as CompletedModel;

                if (completedItem is not null)
                {
                    await new FileInformationDialog(completedItem).ShowAsync();
                }
            };

            // 订阅事件
            DownloadSchedulerService.DownloadingList.ItemsChanged += OnDownloadingListItemsChanged;
        }

        /// <summary>
        /// 从数据库中加载已下载完成的数据
        /// </summary>
        public async Task GetCompletedDataListAsync()
        {
            List<BackgroundModel> DownloadRawList = await DownloadXmlService.QueryWithFlagAsync(4);

            while (isUpdatingNow) await Task.Delay(50);
            lock (CompletedDataListLock) isUpdatingNow = true;

            CompletedDataList.Clear();

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

            lock (CompletedDataListLock) isUpdatingNow = false;
        }
    }
}
