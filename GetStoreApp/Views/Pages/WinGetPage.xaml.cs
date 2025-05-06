using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.WinGet;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.WinGet;
using GetStoreApp.Views.Windows;
using Microsoft.Management.Deployment;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.AppNotifications.Builder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices.Marshalling;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;
using Windows.Storage;
using Windows.System;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// WinGet 程序包页面
    /// </summary>
    public sealed partial class WinGetPage : Page, INotifyPropertyChanged
    {
        private readonly Lock PackageOperationLock = new();

        private SelectorBarItem _selectedItem;

        public SelectorBarItem SelectedItem
        {
            get { return _selectedItem; }

            set
            {
                if (!Equals(_selectedItem, value))
                {
                    _selectedItem = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedItem)));
                }
            }
        }

        public List<Type> PageList { get; } = [typeof(WinGetSearchPage), typeof(WinGetInstalledPage), typeof(WinGetUpgradePage)];

        public ObservableCollection<PackageOperationModel> PackageOperationCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public WinGetPage()
        {
            InitializeComponent();
        }

        #region 第一部分：重写父类事件

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            WinGetFrame.ContentTransitions = SuppressNavigationTransitionCollection;

            // 第一次导航
            if (WinGetConfigService.IsWinGetInstalled && GetCurrentPageType() is null)
            {
                NavigateTo(PageList[0], this, null);
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 取消应用包任务
        /// </summary>
        private void OnCancelTaskExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is PackageOperationModel packageOperation)
            {
                PackageOperationLock.Enter();
                try
                {
                    if (packageOperation.PackageOperationKind is PackageOperationKind.Download && packageOperation.PackageDownloadProgressState is not PackageDownloadProgressState.Finished && packageOperation.PackageDownloadProgress is not null)
                    {
                        packageOperation.PackageDownloadProgress.Cancel();
                    }
                    else if (packageOperation.PackageOperationKind is PackageOperationKind.Install && packageOperation.PackageInstallProgressState is PackageInstallProgressState.Finished && packageOperation.PackageInstallProgress is not null)
                    {
                        packageOperation.PackageInstallProgress.Cancel();
                    }
                    else if (packageOperation.PackageOperationKind is PackageOperationKind.Uninstall && packageOperation.PackageUninstallProgressState is PackageUninstallProgressState.Finished && packageOperation.PackageUninstallProgress is not null)
                    {
                        packageOperation.PackageUninstallProgress.Cancel();
                    }
                    else if (packageOperation.PackageOperationKind is PackageOperationKind.Repair && packageOperation.PackageRepairProgressState is PackageRepairProgressState.Finished && packageOperation.PackageRepairProgress is not null)
                    {
                        packageOperation.PackageRepairProgress.Cancel();
                    }
                    else if (packageOperation.PackageOperationKind is PackageOperationKind.Upgrade && packageOperation.PackageInstallProgressState is PackageInstallProgressState.Finished && packageOperation.PackageInstallProgress is not null)
                    {
                        packageOperation.PackageInstallProgress.Cancel();
                    }

                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Cancel;
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "Cancel winget download task failed", e);
                }
                finally
                {
                    PackageOperationLock.Exit();
                }
            }
        }

        /// <summary>
        /// 移除任务
        /// </summary>
        private void OnRemoveTaskExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is PackageOperationModel packageOperation)
            {
                PackageOperationLock.Enter();
                try
                {
                    PackageOperationCollection.Remove(packageOperation);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "Remove winget task failed", e);
                }
                finally
                {
                    PackageOperationLock.Exit();
                }
            }
        }

        /// <summary>
        /// 打开应用包目录
        /// </summary>
        private void OnOpenPackageFolderExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string packagePath && !string.IsNullOrEmpty(packagePath) && Directory.Exists(packagePath))
            {
                Task.Run(async () =>
                {
                    await Launcher.LaunchFolderAsync(await StorageFolder.GetFolderFromPathAsync(packagePath));
                });
            }
        }

        #endregion 第二部分：XamlUICommand 命令调用时挂载的事件

        #region 第三部分：WinGet 程序包页面——挂载的事件

        /// <summary>
        /// 移除已完成任务
        /// </summary>
        private void OnRemoveFinishedTasklicked(object sender, RoutedEventArgs args)
        {
            PackageOperationLock.Enter();

            try
            {
                for (int index = PackageOperationCollection.Count - 1; index >= 0; index--)
                {
                    if (PackageOperationCollection[index].PackageOperationResultKind is PackageOperationResultKind.Failed)
                    {
                        PackageOperationCollection.RemoveAt(index);
                        continue;
                    }
                    else
                    {
                        if (PackageOperationCollection[index].PackageOperationKind is PackageOperationKind.Download && PackageOperationCollection[index].PackageDownloadProgressState is PackageDownloadProgressState.Finished)
                        {
                            PackageOperationCollection.RemoveAt(index);
                            continue;
                        }
                        else if (PackageOperationCollection[index].PackageOperationKind is PackageOperationKind.Install && PackageOperationCollection[index].PackageInstallProgressState is PackageInstallProgressState.Finished)
                        {
                            PackageOperationCollection.RemoveAt(index);
                            continue;
                        }
                        else if (PackageOperationCollection[index].PackageOperationKind is PackageOperationKind.Uninstall && PackageOperationCollection[index].PackageUninstallProgressState is PackageUninstallProgressState.Finished)
                        {
                            PackageOperationCollection.RemoveAt(index);
                            continue;
                        }
                        else if (PackageOperationCollection[index].PackageOperationKind is PackageOperationKind.Repair && PackageOperationCollection[index].PackageRepairProgressState is PackageRepairProgressState.Finished)
                        {
                            PackageOperationCollection.RemoveAt(index);
                            continue;
                        }
                        else if (PackageOperationCollection[index].PackageOperationKind is PackageOperationKind.Upgrade && PackageOperationCollection[index].PackageInstallProgressState is PackageInstallProgressState.Finished)
                        {
                            PackageOperationCollection.RemoveAt(index);
                            continue;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
            }
            finally
            {
                PackageOperationLock.Exit();
            }
        }

        /// <summary>
        /// 点击关闭按钮关闭任务管理
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            if (WinGetSplitView.IsPaneOpen)
            {
                WinGetSplitView.IsPaneOpen = false;
            }
        }

        /// <summary>
        /// 了解更多有关 WinGet 程序包的描述信息
        /// </summary>
        private async void OnLearnMoreClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri(@"https://learn.microsoft.com/windows/package-manager"));
        }

        /// <summary>
        /// 从微软商店中下载 WinGet 程序包管理器
        /// </summary>
        private async void OnDownloadFromMicrosoftStoreClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-windows-store://pdp/ProductId=9NBLGGH4NNS1"));
        }

        /// <summary>
        /// 从Github中下载 WinGet 程序包管理器
        /// </summary>
        private async void OnDownloadFromGithubClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/microsoft/winget-cli/releases"));
        }

        /// <summary>
        /// 点击选择器栏发生的事件
        /// </summary>
        private void OnSelectorBarTapped(object sender, TappedRoutedEventArgs args)
        {
            if (sender is SelectorBarItem selectorBarItem && selectorBarItem.Tag is string tag)
            {
                int index = Convert.ToInt32(tag);
                int currentIndex = PageList.FindIndex(item => Equals(item, GetCurrentPageType()));

                if (index is 0 && !Equals(GetCurrentPageType(), PageList[0]))
                {
                    NavigateTo(PageList[0], this, index > currentIndex);
                }
                else if (index is 1 && !Equals(GetCurrentPageType(), PageList[1]))
                {
                    NavigateTo(PageList[1], this, index > currentIndex);
                }
                else if (index is 2 && !Equals(GetCurrentPageType(), PageList[2]))
                {
                    NavigateTo(PageList[2], this, index > currentIndex);
                }
            }
        }

        /// <summary>
        /// 导航完成后发生
        /// </summary>
        private void OnNavigated(object sender, NavigationEventArgs args)
        {
            int index = PageList.FindIndex(item => Equals(item, GetCurrentPageType()));

            if (index >= 0 && index < WinGetSelectorBar.Items.Count)
            {
                SelectedItem = WinGetSelectorBar.Items[PageList.FindIndex(item => Equals(item, GetCurrentPageType()))];
            }
        }

        /// <summary>
        /// 导航失败时发生的事件
        /// </summary>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs args)
        {
            args.Handled = true;
            int index = PageList.FindIndex(item => Equals(item, GetCurrentPageType()));

            if (index >= 0 && index < WinGetSelectorBar.Items.Count)
            {
                SelectedItem = WinGetSelectorBar.Items[PageList.FindIndex(item => Equals(item, GetCurrentPageType()))];
            }

            LogService.WriteLog(LoggingLevel.Warning, string.Format(ResourceService.GetLocalized("WinGet/NavigationFailed"), args.SourcePageType.FullName), args.Exception);
        }

        #endregion 第三部分：WinGet 程序包页面——挂载的事件

        /// <summary>
        /// 更新下载进度
        /// </summary>
        private void OnPackageDownloadProgress(IAsyncOperationWithProgress<DownloadResult, PackageDownloadProgress> result, PackageDownloadProgress packageDownloadProgress, PackageOperationModel packageOperation)
        {
            switch (packageDownloadProgress.State)
            {
                // 处于等待中状态
                case PackageDownloadProgressState.Queued:
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            PackageOperationLock.Enter();
                            try
                            {
                                foreach (PackageOperationModel packageOperationItem in PackageOperationCollection)
                                {
                                    if (Equals(packageOperation.AppID, packageOperationItem.AppID) && Equals(packageOperation.AppVersion, packageOperationItem.AppVersion) && Equals(packageOperation.PackageOperationKind, packageOperationItem.PackageOperationKind))
                                    {
                                        packageOperationItem.PackageDownloadProgressState = packageDownloadProgress.State;
                                        break;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                            }
                            finally
                            {
                                PackageOperationLock.Exit();
                            }
                        });
                        break;
                    }
                // 处于下载中状态
                case PackageDownloadProgressState.Downloading:
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            PackageOperationLock.Enter();
                            try
                            {
                                foreach (PackageOperationModel packageOperationItem in PackageOperationCollection)
                                {
                                    if (Equals(packageOperation.AppID, packageOperationItem.AppID) && Equals(packageOperation.AppVersion, packageOperationItem.AppVersion) && Equals(packageOperation.PackageOperationKind, packageOperationItem.PackageOperationKind))
                                    {
                                        packageOperationItem.PackageDownloadProgressState = packageDownloadProgress.State;
                                        packageOperationItem.PackageOperationProgress = Math.Round(packageDownloadProgress.DownloadProgress * 100, 2);
                                        packageOperationItem.DownloadedFileSize = Convert.ToString(FileSizeHelper.ConvertFileSizeToString(packageDownloadProgress.BytesDownloaded));
                                        packageOperationItem.TotalFileSize = Convert.ToString(FileSizeHelper.ConvertFileSizeToString(packageDownloadProgress.BytesRequired));
                                        break;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                            }
                            finally
                            {
                                PackageOperationLock.Exit();
                            }
                        });
                        break;
                    }
                // 处于下载完成状态
                case PackageDownloadProgressState.Finished:
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            PackageOperationLock.Enter();
                            try
                            {
                                foreach (PackageOperationModel packageOperationItem in PackageOperationCollection)
                                {
                                    if (Equals(packageOperation.AppID, packageOperationItem.AppID) && Equals(packageOperation.AppVersion, packageOperationItem.AppVersion) && Equals(packageOperation.PackageOperationKind, packageOperationItem.PackageOperationKind))
                                    {
                                        packageOperationItem.PackageDownloadProgressState = packageDownloadProgress.State;
                                        packageOperationItem.PackageOperationProgress = 100;
                                        break;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                            }
                            finally
                            {
                                PackageOperationLock.Exit();
                            }
                        });
                        break;
                    }
            }
        }

        /// <summary>
        /// 更新安装或更新进度
        /// </summary>
        private void OnPackageInstallProgress(IAsyncOperationWithProgress<InstallResult, InstallProgress> result, InstallProgress installProgress, PackageOperationModel packageOperation)
        {
            if (packageOperation.PackageOperationKind is PackageOperationKind.Install)
            {
                switch (installProgress.State)
                {
                    // 处于等待中状态
                    case PackageInstallProgressState.Queued:
                        {
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                PackageOperationLock.Enter();
                                try
                                {
                                    foreach (PackageOperationModel packageOperationItem in PackageOperationCollection)
                                    {
                                        if (Equals(packageOperation.AppID, packageOperationItem.AppID) && Equals(packageOperation.AppVersion, packageOperationItem.AppVersion) && Equals(packageOperation.PackageOperationKind, packageOperationItem.PackageOperationKind))
                                        {
                                            packageOperationItem.PackageInstallProgressState = installProgress.State;
                                            break;
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                }
                                finally
                                {
                                    PackageOperationLock.Exit();
                                }
                            });
                            break;
                        }
                    // 处于下载中状态
                    case PackageInstallProgressState.Downloading:
                        {
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                PackageOperationLock.Enter();
                                try
                                {
                                    foreach (PackageOperationModel packageOperationItem in PackageOperationCollection)
                                    {
                                        if (Equals(packageOperation.AppID, packageOperationItem.AppID) && Equals(packageOperation.AppVersion, packageOperationItem.AppVersion) && Equals(packageOperation.PackageOperationKind, packageOperationItem.PackageOperationKind))
                                        {
                                            packageOperationItem.PackageInstallProgressState = installProgress.State;
                                            packageOperationItem.PackageOperationProgress = Math.Round(installProgress.DownloadProgress * 100, 2);
                                            packageOperationItem.DownloadedFileSize = Convert.ToString(FileSizeHelper.ConvertFileSizeToString(installProgress.BytesDownloaded));
                                            packageOperationItem.TotalFileSize = Convert.ToString(FileSizeHelper.ConvertFileSizeToString(installProgress.BytesRequired));
                                            break;
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                }
                                finally
                                {
                                    PackageOperationLock.Exit();
                                }
                            });
                            break;
                        }
                    // 处于安装中状态
                    case PackageInstallProgressState.Installing:
                        {
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                PackageOperationLock.Enter();
                                try
                                {
                                    foreach (PackageOperationModel packageOperationItem in PackageOperationCollection)
                                    {
                                        if (Equals(packageOperation.AppID, packageOperationItem.AppID) && Equals(packageOperation.AppVersion, packageOperationItem.AppVersion) && Equals(packageOperation.PackageOperationKind, packageOperationItem.PackageOperationKind))
                                        {
                                            packageOperationItem.PackageInstallProgressState = installProgress.State;
                                            packageOperationItem.PackageOperationProgress = 100;
                                            break;
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                }
                                finally
                                {
                                    PackageOperationLock.Exit();
                                }
                            });
                            break;
                        }
                    // 处于安装完成后等待其他操作状态
                    case PackageInstallProgressState.PostInstall:
                        {
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                PackageOperationLock.Enter();
                                try
                                {
                                    foreach (PackageOperationModel packageOperationItem in PackageOperationCollection)
                                    {
                                        if (Equals(packageOperation.AppID, packageOperationItem.AppID) && Equals(packageOperation.AppVersion, packageOperationItem.AppVersion) && Equals(packageOperation.PackageOperationKind, packageOperationItem.PackageOperationKind))
                                        {
                                            packageOperationItem.PackageInstallProgressState = installProgress.State;
                                            break;
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                }
                                finally
                                {
                                    PackageOperationLock.Exit();
                                }
                            });
                            break;
                        }
                    // 处于安装完成状态
                    case PackageInstallProgressState.Finished:
                        {
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                PackageOperationLock.Enter();
                                try
                                {
                                    foreach (PackageOperationModel packageOperationItem in PackageOperationCollection)
                                    {
                                        if (Equals(packageOperation.AppID, packageOperationItem.AppID) && Equals(packageOperation.AppVersion, packageOperationItem.AppVersion) && Equals(packageOperation.PackageOperationKind, packageOperationItem.PackageOperationKind))
                                        {
                                            packageOperationItem.PackageInstallProgressState = installProgress.State;
                                            packageOperationItem.PackageOperationProgress = 100;
                                            break;
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                }
                                finally
                                {
                                    PackageOperationLock.Exit();
                                }
                            });
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 更新卸载进度
        /// </summary>
        private void OnPackageUninstallProgress(IAsyncOperationWithProgress<UninstallResult, UninstallProgress> result, UninstallProgress uninstallProgress, PackageOperationModel packageOperation)
        {
            switch (uninstallProgress.State)
            {
                // 处于等待中状态
                case PackageUninstallProgressState.Queued:
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            PackageOperationLock.Enter();
                            try
                            {
                                foreach (PackageOperationModel packageOperationItem in PackageOperationCollection)
                                {
                                    if (Equals(packageOperation.AppID, packageOperationItem.AppID) && Equals(packageOperation.AppVersion, packageOperationItem.AppVersion) && Equals(packageOperation.PackageOperationKind, packageOperationItem.PackageOperationKind))
                                    {
                                        packageOperationItem.PackageUninstallProgressState = uninstallProgress.State;
                                        break;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                            }
                            finally
                            {
                                PackageOperationLock.Exit();
                            }
                        });
                        break;
                    }
                // 处于卸载中状态
                case PackageUninstallProgressState.Uninstalling:
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            PackageOperationLock.Enter();
                            try
                            {
                                foreach (PackageOperationModel packageOperationItem in PackageOperationCollection)
                                {
                                    if (Equals(packageOperation.AppID, packageOperationItem.AppID) && Equals(packageOperation.AppVersion, packageOperationItem.AppVersion) && Equals(packageOperation.PackageOperationKind, packageOperationItem.PackageOperationKind))
                                    {
                                        packageOperationItem.PackageUninstallProgressState = uninstallProgress.State;
                                        break;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                            }
                            finally
                            {
                                PackageOperationLock.Exit();
                            }
                        });
                        break;
                    }
                // 处于卸载完成后等待其他操作状态
                case PackageUninstallProgressState.PostUninstall:
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            PackageOperationLock.Enter();
                            try
                            {
                                foreach (PackageOperationModel packageOperationItem in PackageOperationCollection)
                                {
                                    if (Equals(packageOperation.AppID, packageOperationItem.AppID) && Equals(packageOperation.AppVersion, packageOperationItem.AppVersion) && Equals(packageOperation.PackageOperationKind, packageOperationItem.PackageOperationKind))
                                    {
                                        packageOperationItem.PackageUninstallProgressState = uninstallProgress.State;
                                        break;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                            }
                            finally
                            {
                                PackageOperationLock.Exit();
                            }
                        });
                        break;
                    }
                // 处于卸载完成状态
                case PackageUninstallProgressState.Finished:
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            PackageOperationLock.Enter();
                            try
                            {
                                foreach (PackageOperationModel packageOperationItem in PackageOperationCollection)
                                {
                                    if (Equals(packageOperation.AppID, packageOperationItem.AppID) && Equals(packageOperation.AppVersion, packageOperationItem.AppVersion) && Equals(packageOperation.PackageOperationKind, packageOperationItem.PackageOperationKind))
                                    {
                                        packageOperationItem.PackageUninstallProgressState = uninstallProgress.State;
                                        break;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                            }
                            finally
                            {
                                PackageOperationLock.Exit();
                            }
                        });
                        break;
                    }
                default:
                    break;
            }
        }

        /// <summary>
        /// 更新修复进度
        /// </summary>
        private void OnPackageRepairProgress(IAsyncOperationWithProgress<RepairResult, RepairProgress> result, RepairProgress repairProgress, PackageOperationModel packageOperation)
        {
            switch (repairProgress.State)
            {
                // 处于等待中状态
                case PackageRepairProgressState.Queued:
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            PackageOperationLock.Enter();
                            try
                            {
                                foreach (PackageOperationModel packageOperationItem in PackageOperationCollection)
                                {
                                    if (Equals(packageOperation.AppID, packageOperationItem.AppID) && Equals(packageOperation.AppVersion, packageOperationItem.AppVersion) && Equals(packageOperation.PackageOperationKind, packageOperationItem.PackageOperationKind))
                                    {
                                        packageOperationItem.PackageRepairProgressState = repairProgress.State;
                                        break;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                            }
                            finally
                            {
                                PackageOperationLock.Exit();
                            }
                        });
                        break;
                    }
                // 处于修复中状态
                case PackageRepairProgressState.Repairing:
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            PackageOperationLock.Enter();
                            try
                            {
                                foreach (PackageOperationModel packageOperationItem in PackageOperationCollection)
                                {
                                    if (Equals(packageOperation.AppID, packageOperationItem.AppID) && Equals(packageOperation.AppVersion, packageOperationItem.AppVersion) && Equals(packageOperation.PackageOperationKind, packageOperationItem.PackageOperationKind))
                                    {
                                        packageOperationItem.PackageRepairProgressState = repairProgress.State;
                                        packageOperationItem.PackageOperationProgress = Math.Round(repairProgress.RepairCompletionProgress * 100, 2);
                                        break;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                            }
                            finally
                            {
                                PackageOperationLock.Exit();
                            }
                        });
                        break;
                    }
                // 处于修复完成后等待其他操作状态
                case PackageRepairProgressState.PostRepair:
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            PackageOperationLock.Enter();
                            try
                            {
                                foreach (PackageOperationModel packageOperationItem in PackageOperationCollection)
                                {
                                    if (Equals(packageOperation.AppID, packageOperationItem.AppID) && Equals(packageOperation.AppVersion, packageOperationItem.AppVersion) && Equals(packageOperation.PackageOperationKind, packageOperationItem.PackageOperationKind))
                                    {
                                        packageOperationItem.PackageRepairProgressState = repairProgress.State;
                                        break;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                            }
                            finally
                            {
                                PackageOperationLock.Exit();
                            }
                        });
                        break;
                    }
                // 处于修复完成状态
                case PackageRepairProgressState.Finished:
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            PackageOperationLock.Enter();
                            try
                            {
                                foreach (PackageOperationModel packageOperationItem in PackageOperationCollection)
                                {
                                    if (Equals(packageOperation.AppID, packageOperationItem.AppID) && Equals(packageOperation.AppVersion, packageOperationItem.AppVersion) && Equals(packageOperation.PackageOperationKind, packageOperationItem.PackageOperationKind))
                                    {
                                        packageOperationItem.PackageRepairProgressState = repairProgress.State;
                                        break;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                            }
                            finally
                            {
                                PackageOperationLock.Exit();
                            }
                        });
                        break;
                    }
            }
        }

        /// <summary>
        /// 显示任务管理
        /// </summary>
        public void ShowTaskManager()
        {
            if (!WinGetSplitView.IsPaneOpen)
            {
                WinGetSplitView.IsPaneOpen = true;
            }
        }

        /// <summary>
        /// 添加任务
        /// </summary>

        public async Task AddTaskAsync(PackageOperationModel packageOperation)
        {
            switch (packageOperation.PackageOperationKind)
            {
                // 添加下载任务
                case PackageOperationKind.Download:
                    {
                        bool isExisted = AddPackageOperationTask(packageOperation);

                        if (!isExisted)
                        {
                            DownloadResult downloadResult = await Task.Run(async () =>
                            {
                                try
                                {
                                    PackageManager packageManager = new();
                                    DownloadOptions downloadOptions = new()
                                    {
                                        AcceptPackageAgreements = true,
                                        AllowHashMismatch = true, // TODO:设置选项
                                        PackageVersionId = packageOperation.PackageVersionId,
                                        DownloadDirectory = WinGetConfigService.DownloadFolder.Path,
                                        Scope = PackageInstallScope.Any, // TODO:设置选项
                                    };

                                    IAsyncOperationWithProgress<DownloadResult, PackageDownloadProgress> downloadPackageWithProgress = packageManager.DownloadPackageAsync(packageOperation.SearchApps.CatalogPackage, downloadOptions);

                                    PackageOperationLock.Enter();

                                    try
                                    {
                                        foreach (PackageOperationModel packageOperationItem in PackageOperationCollection)
                                        {
                                            if (Equals(packageOperation.AppID, packageOperationItem.AppID) && Equals(packageOperation.AppVersion, packageOperationItem.AppVersion) && Equals(packageOperation.PackageOperationKind, packageOperationItem.PackageOperationKind))
                                            {
                                                packageOperationItem.PackageDownloadProgress = downloadPackageWithProgress;
                                                break;
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                    }
                                    finally
                                    {
                                        PackageOperationLock.Exit();
                                    }

                                    // 第二部分：更新下载进度
                                    downloadPackageWithProgress.Progress = (result, progress) => OnPackageDownloadProgress(result, progress, packageOperation);
                                    return await downloadPackageWithProgress;
                                }
                                // 其他异常
                                catch (Exception e)
                                {
                                    LogService.WriteLog(LoggingLevel.Error, "App download failed.", e);
                                    return null;
                                }
                            });

                            // 下载成功
                            if (downloadResult is not null && downloadResult.Status is DownloadResultStatus.Ok)
                            {
                                await Task.Run(() =>
                                {
                                    // 显示 WinGet 应用下载成功通知
                                    AppNotificationBuilder appNotificationBuilder = new();
                                    appNotificationBuilder.AddArgument("action", "OpenApp");
                                    appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/WinGetDownloadSuccessfully"), packageOperation.SearchApps.AppName));
                                    ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                                });
                            }
                            // 下载失败
                            else
                            {
                                // 显示 WinGet 应用下载失败通知
                                KeyValuePair<string, bool> winGetDataSourceName = WinGetConfigService.GetWinGetDataSourceName();

                                AppNotificationBuilder appNotificationBuilder = new();
                                appNotificationBuilder.AddArgument("action", "OpenApp");
                                appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/WinGetDownloadFailed1"), packageOperation.SearchApps.AppName));
                                appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/WinGetDownloadFailed2"));
                                appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/WinGetDownloadFailed3"));
                                AppNotificationButton downloadWithCommandButton = new(ResourceService.GetLocalized("Notification/DownloadWithCommand"));
                                downloadWithCommandButton.Arguments.Add("action", Equals(winGetDataSourceName, default) ? string.Format("DownloadWithCommand:{0}:{1}", packageOperation.SearchApps.AppID, WinGetConfigService.DownloadFolder.Path) : string.Format("DownloadWithCommand:{0}:{1}:{2}", packageOperation.SearchApps.AppID, winGetDataSourceName.Key, WinGetConfigService.DownloadFolder.Path));
                                AppNotificationButton openDownloadFolderButton = new(ResourceService.GetLocalized("Notification/OpenDownloadFolder"));
                                openDownloadFolderButton.Arguments.Add("action", string.Format("OpenDownloadFolder:{0}", WinGetConfigService.DownloadFolder.Path));
                                appNotificationBuilder.AddButton(downloadWithCommandButton);
                                appNotificationBuilder.AddButton(openDownloadFolderButton);
                                ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                            }
                        }
                        break;
                    }
                // 添加安装任务
                case PackageOperationKind.Install:
                    {
                        bool isExisted = AddPackageOperationTask(packageOperation);

                        if (!isExisted)
                        {
                            InstallResult installResult = await Task.Run(async () =>
                            {
                                try
                                {
                                    PackageManager packageManager = new();
                                    InstallOptions installOptions = new()
                                    {
                                        AcceptPackageAgreements = true,
                                        AllowHashMismatch = true, // TODO:设置选项
                                        BypassIsStoreClientBlockedPolicyCheck = true, // TODO:设置选项
                                        Force = true,
                                        PackageVersionId = packageOperation.PackageVersionId,
                                        AllowUpgradeToUnknownVersion = true, // TODO:设置选项
                                        LogOutputPath = LogService.WinGetFolderPath,
                                        PackageInstallMode = Enum.TryParse(WinGetConfigService.WinGetInstallMode.Key, out PackageInstallMode packageInstallMode) ? packageInstallMode : PackageInstallMode.Default,
                                        PackageInstallScope = PackageInstallScope.Any,  // TODO:设置选项
                                    };

                                    IAsyncOperationWithProgress<InstallResult, InstallProgress> installPackageWithProgress = packageManager.InstallPackageAsync(packageOperation.SearchApps.CatalogPackage, installOptions);

                                    PackageOperationLock.Enter();

                                    try
                                    {
                                        foreach (PackageOperationModel packageOperationItem in PackageOperationCollection)
                                        {
                                            if (Equals(packageOperation.AppID, packageOperationItem.AppID) && Equals(packageOperation.AppVersion, packageOperationItem.AppVersion) && Equals(packageOperation.PackageOperationKind, packageOperationItem.PackageOperationKind))
                                            {
                                                packageOperationItem.PackageInstallProgress = installPackageWithProgress;
                                                break;
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                    }
                                    finally
                                    {
                                        PackageOperationLock.Exit();
                                    }

                                    // 第二部分：更新安装进度
                                    installPackageWithProgress.Progress = (result, progress) => OnPackageInstallProgress(result, progress, packageOperation);
                                    return await installPackageWithProgress;
                                }
                                // 其他异常
                                catch (Exception e)
                                {
                                    LogService.WriteLog(LoggingLevel.Error, "App install failed.", e);
                                    return null;
                                }
                            });

                            // 安装成功
                            if (installResult is not null && installResult.Status is InstallResultStatus.Ok)
                            {
                                await Task.Run(() =>
                                {
                                    // 显示 WinGet 应用安装成功通知
                                    AppNotificationBuilder appNotificationBuilder = new();
                                    appNotificationBuilder.AddArgument("action", "OpenApp");
                                    appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/WinGetInstallSuccessfully"), packageOperation.SearchApps.AppName));
                                    ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                                });

                                // 检测是否需要重启设备完成应用的安装，如果是，询问用户是否需要重启设备
                                if (installResult.RebootRequired)
                                {
                                    ContentDialogResult contentDialogResult = await MainWindow.Current.ShowDialogAsync(new RebootDialog(WinGetOperationKind.SearchInstall, packageOperation.SearchApps.AppName));

                                    if (contentDialogResult is ContentDialogResult.Primary)
                                    {
                                        await Task.Run(() =>
                                        {
                                            ShutdownHelper.Restart(ResourceService.GetLocalized("WinGet/RestartPC"), TimeSpan.FromSeconds(120));
                                        });
                                    }
                                }
                            }
                            // 安装失败
                            else
                            {
                                await Task.Run(() =>
                                {
                                    // 显示 WinGet 应用安装失败通知
                                    KeyValuePair<string, bool> winGetDataSourceName = WinGetConfigService.GetWinGetDataSourceName();

                                    AppNotificationBuilder appNotificationBuilder = new();
                                    appNotificationBuilder.AddArgument("action", "OpenApp");
                                    appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/WinGetInstallFailed1"), packageOperation.SearchApps.AppName));
                                    appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/WinGetInstallFailed2"));
                                    appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/WinGetInstallFailed3"));
                                    AppNotificationButton installWithCommandButton = new(ResourceService.GetLocalized("Notification/InstallWithCommand"));
                                    installWithCommandButton.Arguments.Add("action", Equals(winGetDataSourceName, default) ? string.Format("InstallWithCommand:{0}", packageOperation.SearchApps.AppID) : string.Format("InstallWithCommand:{0}:{1}", packageOperation.SearchApps.AppID, winGetDataSourceName.Key));
                                    AppNotificationButton openInstallFolderButton = new(ResourceService.GetLocalized("Notification/OpenDownloadFolder"));
                                    openInstallFolderButton.Arguments.Add("action", string.Format("OpenDownloadFolder:{0}", WinGetConfigService.DownloadFolder.Path));
                                    appNotificationBuilder.AddButton(installWithCommandButton);
                                    appNotificationBuilder.AddButton(openInstallFolderButton);
                                    ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                                });
                            }
                        }
                        break;
                    }
                // 添加卸载任务
                case PackageOperationKind.Uninstall:
                    {
                        bool isExisted = AddPackageOperationTask(packageOperation);

                        if (!isExisted)
                        {
                            UninstallResult uninstallResult = await Task.Run(async () =>
                            {
                                try
                                {
                                    PackageManager packageManager = new();
                                    UninstallOptions uninstallOptions = new()
                                    {
                                        Force = true,
                                        LogOutputPath = LogService.WinGetFolderPath,
                                        PackageVersionId = packageOperation.PackageVersionId,
                                        PackageUninstallMode = PackageUninstallMode.Default, // TODO:设置选项
                                        PackageUninstallScope = PackageUninstallScope.Any, // TODO:设置选项
                                    };

                                    IAsyncOperationWithProgress<UninstallResult, UninstallProgress> uninstallPackageWithProgress = packageManager.UninstallPackageAsync(packageOperation.InstalledApps.CatalogPackage, uninstallOptions);

                                    PackageOperationLock.Enter();

                                    try
                                    {
                                        foreach (PackageOperationModel packageOperationItem in PackageOperationCollection)
                                        {
                                            if (Equals(packageOperation.AppID, packageOperationItem.AppID) && Equals(packageOperation.AppVersion, packageOperationItem.AppVersion) && Equals(packageOperation.PackageOperationKind, packageOperationItem.PackageOperationKind))
                                            {
                                                packageOperationItem.PackageUninstallProgress = uninstallPackageWithProgress;
                                                break;
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                    }
                                    finally
                                    {
                                        PackageOperationLock.Exit();
                                    }

                                    // 第二部分：更新卸载进度
                                    uninstallPackageWithProgress.Progress = (result, progress) => OnPackageUninstallProgress(result, progress, packageOperation);
                                    return await uninstallPackageWithProgress;
                                }
                                // 其他异常
                                catch (Exception e)
                                {
                                    LogService.WriteLog(LoggingLevel.Error, "App uninstall failed.", e);
                                    return null;
                                }
                            });

                            // 卸载成功
                            if (uninstallResult is not null && uninstallResult.Status is UninstallResultStatus.Ok)
                            {
                                // TODO: 移除列表操作

                                // 检测是否需要重启设备完成应用的卸载，如果是，询问用户是否需要重启设备
                                if (uninstallResult.RebootRequired)
                                {
                                    ContentDialogResult contentDialogResult = await MainWindow.Current.ShowDialogAsync(new RebootDialog(WinGetOperationKind.Uninstall, packageOperation.InstalledApps.AppName));

                                    if (contentDialogResult is ContentDialogResult.Primary)
                                    {
                                        await Task.Run(() =>
                                        {
                                            ShutdownHelper.Restart(ResourceService.GetLocalized("WinGet/RestartPC"), TimeSpan.FromSeconds(120));
                                        });
                                    }
                                }
                            }
                            // 卸载失败
                            else
                            {
                                await Task.Run(() =>
                                {
                                    // 显示 WinGet 卸载应用失败通知
                                    AppNotificationBuilder appNotificationBuilder = new();
                                    appNotificationBuilder.AddArgument("action", "OpenApp");
                                    appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/WinGetUninstallFailed1"), packageOperation.InstalledApps.AppName));
                                    appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/WinGetUninstallFailed2"));
                                    appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/WinGetUninstallFailed3"));
                                    AppNotificationButton openSettingsButton = new(ResourceService.GetLocalized("Notification/OpenSettings"));
                                    openSettingsButton.Arguments.Add("action", "OpenSettings");
                                    appNotificationBuilder.AddButton(openSettingsButton);
                                    ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                                });
                            }
                        }
                        break;
                    }
                // 添加修复任务
                case PackageOperationKind.Repair:
                    {
                        bool isExisted = AddPackageOperationTask(packageOperation);

                        if (!isExisted)
                        {
                            RepairResult repairResult = await Task.Run(async () =>
                            {
                                try
                                {
                                    PackageManager packageManager = new();
                                    RepairOptions repairOptions = new()
                                    {
                                        AcceptPackageAgreements = true,
                                        AllowHashMismatch = true, // TODO:设置选项
                                        BypassIsStoreClientBlockedPolicyCheck = true, // TODO:设置选项
                                        Force = true,
                                        PackageVersionId = packageOperation.PackageVersionId,
                                        LogOutputPath = LogService.WinGetFolderPath,
                                        PackageRepairMode = PackageRepairMode.Default, // TODO:设置选项
                                        PackageRepairScope = PackageRepairScope.Any,  // TODO:设置选项
                                    };

                                    IAsyncOperationWithProgress<RepairResult, RepairProgress> repairPackageWithProgress = packageManager.RepairPackageAsync(packageOperation.SearchApps.CatalogPackage, repairOptions);

                                    PackageOperationLock.Enter();

                                    try
                                    {
                                        foreach (PackageOperationModel packageOperationItem in PackageOperationCollection)
                                        {
                                            if (Equals(packageOperation.AppID, packageOperationItem.AppID) && Equals(packageOperation.AppVersion, packageOperationItem.AppVersion) && Equals(packageOperation.PackageOperationKind, packageOperationItem.PackageOperationKind))
                                            {
                                                packageOperationItem.PackageRepairProgress = repairPackageWithProgress;
                                                break;
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                    }
                                    finally
                                    {
                                        PackageOperationLock.Exit();
                                    }

                                    // 第二部分：更新安装进度
                                    repairPackageWithProgress.Progress = (result, progress) => OnPackageRepairProgress(result, progress, packageOperation);
                                    return await repairPackageWithProgress;
                                }
                                // 其他异常
                                catch (Exception e)
                                {
                                    LogService.WriteLog(LoggingLevel.Error, "App repair failed.", e);
                                    return null;
                                }
                            });

                            if (repairResult is not null && repairResult.Status is RepairResultStatus.Ok)
                            {
                                await Task.Run(() =>
                                {
                                    // 显示 WinGet 应用修复成功通知
                                    AppNotificationBuilder appNotificationBuilder = new();
                                    appNotificationBuilder.AddArgument("action", "OpenApp");
                                    appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/WinGetRepairSuccessfully"), packageOperation.SearchApps.AppName));
                                    ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                                });

                                // 检测是否需要重启设备完成应用的修复，如果是，询问用户是否需要重启设备
                                if (repairResult.RebootRequired)
                                {
                                    ContentDialogResult contentDialogResult = await MainWindow.Current.ShowDialogAsync(new RebootDialog(WinGetOperationKind.SearchRepair, packageOperation.InstalledApps.AppName));

                                    if (contentDialogResult is ContentDialogResult.Primary)
                                    {
                                        await Task.Run(() =>
                                        {
                                            ShutdownHelper.Restart(ResourceService.GetLocalized("WinGet/RestartPC"), TimeSpan.FromSeconds(120));
                                        });
                                    }
                                }
                            }
                            else
                            {
                                await Task.Run(() =>
                                {
                                    // 显示 WinGet 应用修复失败通知
                                    KeyValuePair<string, bool> winGetDataSourceName = WinGetConfigService.GetWinGetDataSourceName();

                                    AppNotificationBuilder appNotificationBuilder = new();
                                    appNotificationBuilder.AddArgument("action", "OpenApp");
                                    appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/WinGetRepairFailed1"), packageOperation.SearchApps.AppName));
                                    appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/WinGetRepairFailed2"));
                                    appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/WinGetRepairFailed3"));
                                    AppNotificationButton repairWithCommandButton = new(ResourceService.GetLocalized("Notification/RepairWithCommand"));
                                    repairWithCommandButton.Arguments.Add("action", Equals(winGetDataSourceName, default) ? string.Format("RepairWithCommand:{0}", packageOperation.SearchApps.AppID) : string.Format("RepairWithCommand:{0}:{1}", packageOperation.SearchApps.AppID, winGetDataSourceName.Key));
                                    AppNotificationButton openRepairFolderButton = new(ResourceService.GetLocalized("Notification/OpenDownloadFolder"));
                                    openRepairFolderButton.Arguments.Add("action", string.Format("OpenDownloadFolder:{0}", WinGetConfigService.DownloadFolder.Path));
                                    appNotificationBuilder.AddButton(repairWithCommandButton);
                                    appNotificationBuilder.AddButton(openRepairFolderButton);
                                    ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                                });
                            }
                        }
                        break;
                    }
                // 添加更新任务
                case PackageOperationKind.Upgrade:
                    {
                        bool isExisted = AddPackageOperationTask(packageOperation);

                        if (!isExisted)
                        {
                            InstallResult installResult = await Task.Run(async () =>
                            {
                                try
                                {
                                    PackageManager packageManager = new();
                                    InstallOptions installOptions = new()
                                    {
                                        AcceptPackageAgreements = true,
                                        AllowHashMismatch = true, // TODO:设置选项
                                        BypassIsStoreClientBlockedPolicyCheck = true, // TODO:设置选项
                                        Force = true,
                                        PackageVersionId = packageOperation.PackageVersionId,
                                        AllowUpgradeToUnknownVersion = true, // TODO:设置选项
                                        LogOutputPath = LogService.WinGetFolderPath,
                                        PackageInstallMode = Enum.TryParse(WinGetConfigService.WinGetInstallMode.Key, out PackageInstallMode packageInstallMode) ? packageInstallMode : PackageInstallMode.Default,
                                        PackageInstallScope = PackageInstallScope.Any,  // TODO:设置选项
                                    };

                                    IAsyncOperationWithProgress<InstallResult, InstallProgress> upgradePackageWithProgress = packageManager.UpgradePackageAsync(packageOperation.UpgradableApps.CatalogPackage, installOptions);

                                    PackageOperationLock.Enter();

                                    try
                                    {
                                        foreach (PackageOperationModel packageOperationItem in PackageOperationCollection)
                                        {
                                            if (Equals(packageOperation.AppID, packageOperationItem.AppID) && Equals(packageOperation.AppVersion, packageOperationItem.AppVersion) && Equals(packageOperation.PackageOperationKind, packageOperationItem.PackageOperationKind))
                                            {
                                                packageOperationItem.PackageInstallProgress = upgradePackageWithProgress;
                                                break;
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                    }
                                    finally
                                    {
                                        PackageOperationLock.Exit();
                                    }

                                    // 第二部分：更新安装进度
                                    upgradePackageWithProgress.Progress = (result, progress) => OnPackageInstallProgress(result, progress, packageOperation);
                                    return await upgradePackageWithProgress;
                                }
                                // 其他异常
                                catch (Exception e)
                                {
                                    LogService.WriteLog(LoggingLevel.Error, "App repair failed.", e);
                                    return null;
                                }
                            });

                            if (installResult.Status is InstallResultStatus.Ok)
                            {
                                await Task.Run(() =>
                                {
                                    // 显示 WinGet 应用更新成功通知
                                    AppNotificationBuilder appNotificationBuilder = new();
                                    appNotificationBuilder.AddArgument("action", "OpenApp");
                                    appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/WinGetUpgradeSuccessfully"), packageOperation.UpgradableApps.AppName));
                                    ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                                });

                                // 检测是否需要重启设备完成应用的更新，如果是，询问用户是否需要重启设备
                                if (installResult.RebootRequired)
                                {
                                    ContentDialogResult contentDialogResult = await MainWindow.Current.ShowDialogAsync(new RebootDialog(WinGetOperationKind.Upgrade, packageOperation.UpgradableApps.AppName));

                                    if (contentDialogResult is ContentDialogResult.Primary)
                                    {
                                        await Task.Run(() =>
                                        {
                                            ShutdownHelper.Restart(ResourceService.GetLocalized("WinGet/RestartPC"), TimeSpan.FromSeconds(120));
                                        });
                                    }
                                }
                            }
                            else
                            {
                                await Task.Run(() =>
                                {
                                    // 显示 WinGet 应用升级失败通知
                                    AppNotificationBuilder appNotificationBuilder = new();
                                    appNotificationBuilder.AddArgument("action", "OpenApp");
                                    appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/WinGetUpgradeFailed1"), packageOperation.UpgradableApps.AppName));
                                    appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/WinGetUpgradeFailed2"));
                                    appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/WinGetUpgradeFailed3"));
                                    AppNotificationButton installWithCommandButton = new(ResourceService.GetLocalized("Notification/InstallWithCommand"));
                                    installWithCommandButton.Arguments.Add("action", string.Format("InstallWithCommand:{0}", "TestAppID"));
                                    AppNotificationButton openDownloadFolderButton = new(ResourceService.GetLocalized("Notification/OpenDownloadFolder"));
                                    openDownloadFolderButton.Arguments.Add("action", "OpenDownloadFolder");
                                    appNotificationBuilder.AddButton(installWithCommandButton);
                                    appNotificationBuilder.AddButton(openDownloadFolderButton);
                                    ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                                });
                            }
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 页面向前导航
        /// </summary>
        private void NavigateTo(Type navigationPageType, object parameter = null, bool? slideDirection = null)
        {
            try
            {
                if (slideDirection.HasValue)
                {
                    WinGetFrame.ContentTransitions = slideDirection.Value ? RightSlideNavigationTransitionCollection : LeftSlideNavigationTransitionCollection;
                }

                // 导航到该项目对应的页面
                WinGetFrame.Navigate(navigationPageType, parameter);
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, string.Format(ResourceService.GetLocalized("WinGet/NavigationFailed"), navigationPageType.FullName), e);
            }
        }

        /// <summary>
        /// 获取当前导航到的页
        /// </summary>
        private Type GetCurrentPageType()
        {
            return WinGetFrame.CurrentSourcePageType;
        }

        /// <summary>
        /// 添加并检测任务是否已经存在，不存在则添加任务
        /// </summary>
        private bool AddPackageOperationTask(PackageOperationModel packageOperation)
        {
            bool isExisted = false;
            PackageOperationLock.Enter();

            try
            {
                foreach (PackageOperationModel packageOperationItem in PackageOperationCollection)
                {
                    if (Equals(packageOperation.AppID, packageOperationItem.AppID) && Equals(packageOperation.AppVersion, packageOperationItem.AppVersion) && Equals(packageOperation.PackageOperationKind, packageOperationItem.PackageOperationKind))
                    {
                        isExisted = true;
                        break;
                    }
                }

                if (!isExisted)
                {
                    PackageOperationCollection.Add(packageOperation);
                }
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
            }
            finally
            {
                PackageOperationLock.Exit();
            }

            return isExisted;
        }
    }
}
