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
        private readonly string Unknown = ResourceService.GetLocalized("WinGet/Unknown");
        private readonly string PackageDownloadFailedContent = ResourceService.GetLocalized("WinGet/PackageDownloadFailedContent");
        private readonly string PackageInstallFailedContent = ResourceService.GetLocalized("WinGet/PackageInstallFailedContent");
        private readonly string PackageUninstallFailedContent = ResourceService.GetLocalized("WinGet/PackageUninstallFailedContent");
        private readonly string PackageRepairFailedContent = ResourceService.GetLocalized("WinGet/PackageRepairFailedContent");
        private readonly string PackageUpgradeFailedContent = ResourceService.GetLocalized("WinGet/PackageUpgradeFailedContent");
        private readonly string WinGetPackageDownloadBlockedByPolicy = ResourceService.GetLocalized("WinGet/WinGetPackageDownloadBlockedByPolicy");
        private readonly string WinGetPackageDownloadCatalogError = ResourceService.GetLocalized("WinGet/WinGetPackageDownloadCatalogError");
        private readonly string WinGetPackageDownloadInternalError = ResourceService.GetLocalized("WinGet/WinGetPackageDownloadInternalError");
        private readonly string WinGetPackageDownloadInvalidOptions = ResourceService.GetLocalized("WinGet/WinGetPackageDownloadInvalidOptions");
        private readonly string WinGetPackageDownloadError = ResourceService.GetLocalized("WinGet/WinGetPackageDownloadError");
        private readonly string WinGetPackageDownloadManifestError = ResourceService.GetLocalized("WinGet/WinGetPackageDownloadManifestError");
        private readonly string WinGetPackageDownloadNoApplicableInstallers = ResourceService.GetLocalized("WinGet/WinGetPackageDownloadNoApplicableInstallers");
        private readonly string WinGetPackageDownloadAgreementsNotAccepted = ResourceService.GetLocalized("WinGet/WinGetPackageDownloadAgreementsNotAccepted");
        private readonly string WinGetPackageDownloadOtherError = ResourceService.GetLocalized("WinGet/WinGetPackageDownloadOtherError");
        private readonly string WinGetPackageInstallBlockedByPolicy = ResourceService.GetLocalized("WinGet/WinGetPackageInstallBlockedByPolicy");
        private readonly string WinGetPackageInstallCatalogError = ResourceService.GetLocalized("WinGet/WinGetPackageInstallCatalogError");
        private readonly string WinGetPackageInstallInternalError = ResourceService.GetLocalized("WinGet/WinGetPackageInstallInternalError");
        private readonly string WinGetPackageInstallInvalidOptions = ResourceService.GetLocalized("WinGet/WinGetPackageInstallInvalidOptions");
        private readonly string WinGetPackageInstallDownloadError = ResourceService.GetLocalized("WinGet/WinGetPackageInstallDownloadError");
        private readonly string WinGetPackageInstallError = ResourceService.GetLocalized("WinGet/WinGetPackageInstallError");
        private readonly string WinGetPackageInstallManifestError = ResourceService.GetLocalized("WinGet/WinGetPackageInstallManifestError");
        private readonly string WinGetPackageInstallNoApplicableInstallers = ResourceService.GetLocalized("WinGet/WinGetPackageInstallNoApplicableInstallers");
        private readonly string WinGetPackageInstallAgreementsNotAccepted = ResourceService.GetLocalized("WinGet/WinGetPackageInstallAgreementsNotAccepted");
        private readonly string WinGetPackageInstallOtherError = ResourceService.GetLocalized("WinGet/WinGetPackageInstallOtherError");
        private readonly string WinGetPackageUninstallBlockedByPolicy = ResourceService.GetLocalized("WinGet/WinGetPackageUninstallBlockedByPolicy");
        private readonly string WinGetPackageUninstallCatalogError = ResourceService.GetLocalized("WinGet/WinGetPackageUninstallCatalogError");
        private readonly string WinGetPackageUninstallInternalError = ResourceService.GetLocalized("WinGet/WinGetPackageUninstallInternalError");
        private readonly string WinGetPackageUninstallInvalidOptions = ResourceService.GetLocalized("WinGet/WinGetPackageUninstallInvalidOptions");
        private readonly string WinGetPackageUninstallError = ResourceService.GetLocalized("WinGet/WinGetPackageUninstallError");
        private readonly string WinGetPackageUninstallManifestError = ResourceService.GetLocalized("WinGet/WinGetPackageUninstallManifestError");
        private readonly string WinGetPackageUninstallOtherError = ResourceService.GetLocalized("WinGet/WinGetPackageUninstallOtherError");
        private readonly string WinGetPackageRepairBlockedByPolicy = ResourceService.GetLocalized("WinGet/WinGetPackageRepairBlockedByPolicy");
        private readonly string WinGetPackageRepairCatalogError = ResourceService.GetLocalized("WinGet/WinGetPackageRepairCatalogError");
        private readonly string WinGetPackageRepairInternalError = ResourceService.GetLocalized("WinGet/WinGetPackageRepairInternalError");
        private readonly string WinGetPackageRepairInvalidOptions = ResourceService.GetLocalized("WinGet/WinGetPackageRepairInvalidOptions");
        private readonly string WinGetPackageRepairError = ResourceService.GetLocalized("WinGet/WinGetPackageRepairError");
        private readonly string WinGetPackageRepairManifestError = ResourceService.GetLocalized("WinGet/WinGetPackageRepairManifestError");
        private readonly string WinGetPackageRepairNoApplicableRepairer = ResourceService.GetLocalized("WinGet/WinGetPackageRepairNoApplicableRepairer");
        private readonly string WinGetPackageRepairAgreementsNotAccepted = ResourceService.GetLocalized("WinGet/WinGetPackageRepairAgreementsNotAccepted");
        private readonly string WinGetPackageRepairOtherError = ResourceService.GetLocalized("WinGet/WinGetPackageRepairOtherError");
        private readonly string WinGetPackageUpgradeBlockedByPolicy = ResourceService.GetLocalized("WinGet/WinGetPackageUpgradeBlockedByPolicy");
        private readonly string WinGetPackageUpgradeCatalogError = ResourceService.GetLocalized("WinGet/WinGetPackageUpgradeCatalogError");
        private readonly string WinGetPackageUpgradeInternalError = ResourceService.GetLocalized("WinGet/WinGetPackageUpgradeInternalError");
        private readonly string WinGetPackageUpgradeInvalidOptions = ResourceService.GetLocalized("WinGet/WinGetPackageUpgradeInvalidOptions");
        private readonly string WinGetPackageUpgradeDownloadError = ResourceService.GetLocalized("WinGet/WinGetPackageUpgradeDownloadError");
        private readonly string WinGetPackageUpgradeError = ResourceService.GetLocalized("WinGet/WinGetPackageUpgradeError");
        private readonly string WinGetPackageUpgradeManifestError = ResourceService.GetLocalized("WinGet/WinGetPackageUpgradeManifestError");
        private readonly string WinGetPackageUpgradeNoApplicableInstallers = ResourceService.GetLocalized("WinGet/WinGetPackageUpgradeNoApplicableInstallers");
        private readonly string WinGetPackageUpgradeNoApplicableUpgrade = ResourceService.GetLocalized("WinGet/WinGetPackageUpgradeNoApplicableUpgrade");
        private readonly string WinGetPackageUpgradeAgreementsNotAccepted = ResourceService.GetLocalized("WinGet/WinGetPackageUpgradeAgreementsNotAccepted");
        private readonly string WinGetPackageUpgradeOtherError = ResourceService.GetLocalized("WinGet/WinGetPackageUpgradeOtherError");
        private readonly string WinGetPackageOperationCompleted = ResourceService.GetLocalized("Notification/WinGetPackageOperationCompleted");
        public readonly Lock PackageOperationLock = new();

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

        private List<Type> PageList { get; } = [typeof(WinGetSearchPage), typeof(WinGetInstalledPage), typeof(WinGetUpgradePage)];

        public ObservableCollection<PackageOperationModel> PackageOperationCollection { get; } = [];

        public event Action<bool, bool, InstalledAppsModel, UninstallResult> InstalledAppsPackageOperationEvent;

        public event Action<bool, bool, UpgradableAppsModel, InstallResult> UpgradeAppsPackageOperationEvent;

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
                    else if (packageOperation.PackageOperationKind is PackageOperationKind.Install && packageOperation.PackageInstallProgressState is not PackageInstallProgressState.Finished && packageOperation.PackageInstallProgress is not null)
                    {
                        packageOperation.PackageInstallProgress.Cancel();
                    }
                    else if (packageOperation.PackageOperationKind is PackageOperationKind.Uninstall && packageOperation.PackageUninstallProgressState is not PackageUninstallProgressState.Finished && packageOperation.PackageUninstallProgress is not null)
                    {
                        packageOperation.PackageUninstallProgress.Cancel();
                    }
                    else if (packageOperation.PackageOperationKind is PackageOperationKind.Repair && packageOperation.PackageRepairProgressState is not PackageRepairProgressState.Finished && packageOperation.PackageRepairProgress is not null)
                    {
                        packageOperation.PackageRepairProgress.Cancel();
                    }
                    else if (packageOperation.PackageOperationKind is PackageOperationKind.Upgrade && packageOperation.PackageInstallProgressState is not PackageInstallProgressState.Finished && packageOperation.PackageInstallProgress is not null)
                    {
                        packageOperation.PackageInstallProgress.Cancel();
                    }

                    packageOperation.PackageOperationProgress = 100;
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
        /// 更新应用下载进度
        /// </summary>
        private void OnPackageDownloadProgress(IAsyncOperationWithProgress<DownloadResult, PackageDownloadProgress> result, PackageDownloadProgress packageDownloadProgress, PackageOperationModel packageOperation)
        {
            if (packageOperation.PackageOperationResultKind is PackageOperationResultKind.Normal)
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
                                    packageOperation.PackageDownloadProgressState = packageDownloadProgress.State;
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
                                    packageOperation.PackageDownloadProgressState = packageDownloadProgress.State;
                                    packageOperation.PackageOperationProgress = Math.Round(packageDownloadProgress.DownloadProgress * 100, 2);
                                    packageOperation.DownloadedFileSize = Convert.ToString(FileSizeHelper.ConvertFileSizeToString(packageDownloadProgress.BytesDownloaded));
                                    packageOperation.TotalFileSize = Convert.ToString(FileSizeHelper.ConvertFileSizeToString(packageDownloadProgress.BytesRequired));
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
        /// 更新应用安装或更新进度
        /// </summary>
        private void OnPackageInstallProgress(IAsyncOperationWithProgress<InstallResult, InstallProgress> result, InstallProgress installProgress, PackageOperationModel packageOperation)
        {
            if (packageOperation.PackageOperationResultKind is PackageOperationResultKind.Normal)
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
                                    packageOperation.PackageInstallProgressState = installProgress.State;
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
                                    packageOperation.PackageInstallProgressState = installProgress.State;
                                    packageOperation.PackageOperationProgress = Math.Round(installProgress.DownloadProgress * 100, 2);
                                    packageOperation.DownloadedFileSize = Convert.ToString(FileSizeHelper.ConvertFileSizeToString(installProgress.BytesDownloaded));
                                    packageOperation.TotalFileSize = Convert.ToString(FileSizeHelper.ConvertFileSizeToString(installProgress.BytesRequired));
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
                                    packageOperation.PackageInstallProgressState = installProgress.State;
                                    packageOperation.PackageOperationProgress = 100;
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
                                    packageOperation.PackageInstallProgressState = installProgress.State;
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
        /// 更新应用卸载进度
        /// </summary>
        private void OnPackageUninstallProgress(IAsyncOperationWithProgress<UninstallResult, UninstallProgress> result, UninstallProgress uninstallProgress, PackageOperationModel packageOperation)
        {
            if (packageOperation.PackageOperationResultKind is PackageOperationResultKind.Normal)
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
                                    packageOperation.PackageUninstallProgressState = uninstallProgress.State;
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
                                    packageOperation.PackageUninstallProgressState = uninstallProgress.State;
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
                                    packageOperation.PackageUninstallProgressState = uninstallProgress.State;
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
        /// 更新应用修复进度
        /// </summary>
        private void OnPackageRepairProgress(IAsyncOperationWithProgress<RepairResult, RepairProgress> result, RepairProgress repairProgress, PackageOperationModel packageOperation)
        {
            if (packageOperation.PackageOperationResultKind is PackageOperationResultKind.Normal)
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
                                    packageOperation.PackageRepairProgressState = repairProgress.State;
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
                                    packageOperation.PackageRepairProgressState = repairProgress.State;
                                    packageOperation.PackageOperationProgress = Math.Round(repairProgress.RepairCompletionProgress * 100, 2);
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
                                    packageOperation.PackageRepairProgressState = repairProgress.State;
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
                                    packageOperation.PackageRepairProgressState = repairProgress.State;
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
                            (bool result, bool isCanceled, DownloadResult downloadResult, Exception exception) = await Task.Run(async () =>
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
                                        packageOperation.PackageDownloadProgress = downloadPackageWithProgress;
                                    }
                                    catch (Exception e)
                                    {
                                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                    }
                                    finally
                                    {
                                        PackageOperationLock.Exit();
                                    }

                                    // 第二部分：更新应用下载进度
                                    downloadPackageWithProgress.Progress = (result, progress) => OnPackageDownloadProgress(result, progress, packageOperation);
                                    return ValueTuple.Create<bool, bool, DownloadResult, Exception>(true, false, await downloadPackageWithProgress, null);
                                }
                                // 任务已取消
                                catch (TaskCanceledException e)
                                {
                                    return ValueTuple.Create<bool, bool, DownloadResult, Exception>(false, true, null, e);
                                }
                                // 其他异常
                                catch (Exception e)
                                {
                                    LogService.WriteLog(LoggingLevel.Error, "App download failed.", e);
                                    return ValueTuple.Create<bool, bool, DownloadResult, Exception>(false, false, null, e);
                                }
                            });

                            // 应用包操作成功
                            if (result)
                            {
                                // 存在下载结果
                                if (downloadResult is not null)
                                {
                                    PackageOperationLock.Enter();
                                    try
                                    {
                                        packageOperation.PackageDownloadProgressState = PackageDownloadProgressState.Finished;
                                        packageOperation.PackageOperationProgress = 100;
                                    }
                                    catch (Exception e)
                                    {
                                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                    }
                                    finally
                                    {
                                        PackageOperationLock.Exit();
                                    }

                                    switch (downloadResult.Status)
                                    {
                                        // 被策略阻止
                                        case DownloadResultStatus.BlockedByPolicy:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageDownloadFailedContent, WinGetPackageDownloadBlockedByPolicy, downloadResult.ExtendedErrorCode is not null ? downloadResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                        // 数据源错误
                                        case DownloadResultStatus.CatalogError:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageDownloadFailedContent, WinGetPackageDownloadCatalogError, downloadResult.ExtendedErrorCode is not null ? downloadResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                        // 内部错误
                                        case DownloadResultStatus.InternalError:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageDownloadFailedContent, WinGetPackageDownloadInternalError, downloadResult.ExtendedErrorCode is not null ? downloadResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                        // 非法错误
                                        case DownloadResultStatus.InvalidOptions:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageDownloadFailedContent, WinGetPackageDownloadInvalidOptions, downloadResult.ExtendedErrorCode is not null ? downloadResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                        // 下载错误
                                        case DownloadResultStatus.DownloadError:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageDownloadFailedContent, WinGetPackageDownloadError, downloadResult.ExtendedErrorCode is not null ? downloadResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                        // 清单错误
                                        case DownloadResultStatus.ManifestError:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageDownloadFailedContent, WinGetPackageDownloadManifestError, downloadResult.ExtendedErrorCode is not null ? downloadResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                        // 没有合适的应用安装包
                                        case DownloadResultStatus.NoApplicableInstallers:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageDownloadFailedContent, WinGetPackageDownloadNoApplicableInstallers, downloadResult.ExtendedErrorCode is not null ? downloadResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                        // 未接受许可协议
                                        case DownloadResultStatus.PackageAgreementsNotAccepted:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageDownloadFailedContent, WinGetPackageDownloadAgreementsNotAccepted, downloadResult.ExtendedErrorCode is not null ? downloadResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                    }
                                }
                                // 不存在下载结果
                                else
                                {
                                    PackageOperationLock.Enter();
                                    try
                                    {
                                        packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                        packageOperation.PackageOperationFailedContent = Unknown;
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
                            }
                            // 应用包操作失败
                            else
                            {
                                // 下载任务已取消
                                if (isCanceled)
                                {
                                    PackageOperationLock.Enter();
                                    try
                                    {
                                        PackageOperationCollection.Remove(packageOperation);
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
                                else
                                {
                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                    packageOperation.PackageOperationFailedContent = string.Format(PackageDownloadFailedContent, WinGetPackageDownloadOtherError, exception is not null ? exception.HResult : Unknown);
                                }
                            }

                            ShowTaskCompletedNotification();
                        }
                        break;
                    }
                // 添加安装任务
                case PackageOperationKind.Install:
                    {
                        bool isExisted = AddPackageOperationTask(packageOperation);

                        if (!isExisted)
                        {
                            (bool result, bool isCanceled, InstallResult installResult, Exception exception) = await Task.Run(async () =>
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
                                        packageOperation.PackageInstallProgress = installPackageWithProgress;
                                    }
                                    catch (Exception e)
                                    {
                                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                    }
                                    finally
                                    {
                                        PackageOperationLock.Exit();
                                    }

                                    // 第二部分：更新应用安装进度
                                    installPackageWithProgress.Progress = (result, progress) => OnPackageInstallProgress(result, progress, packageOperation);
                                    return ValueTuple.Create<bool, bool, InstallResult, Exception>(true, false, await installPackageWithProgress, null);
                                }
                                // 任务已取消
                                catch (TaskCanceledException e)
                                {
                                    return ValueTuple.Create<bool, bool, InstallResult, Exception>(false, true, null, e);
                                }
                                // 其他异常
                                catch (Exception e)
                                {
                                    LogService.WriteLog(LoggingLevel.Error, "App install failed.", e);
                                    return ValueTuple.Create<bool, bool, InstallResult, Exception>(false, false, null, e);
                                }
                            });

                            if (result)
                            {
                                // 存在安装结果
                                if (installResult is not null)
                                {
                                    PackageOperationLock.Enter();
                                    try
                                    {
                                        packageOperation.PackageInstallProgressState = PackageInstallProgressState.Finished;
                                        packageOperation.PackageOperationProgress = 100;
                                    }
                                    catch (Exception e)
                                    {
                                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                    }
                                    finally
                                    {
                                        PackageOperationLock.Exit();
                                    }

                                    switch (installResult.Status)
                                    {
                                        // 安装成功
                                        case InstallResultStatus.Ok:
                                            {
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
                                                break;
                                            }
                                        // 被组策略阻止
                                        case InstallResultStatus.BlockedByPolicy:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageInstallFailedContent, WinGetPackageInstallBlockedByPolicy, installResult.ExtendedErrorCode is not null ? installResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                        // 数据源错误
                                        case InstallResultStatus.CatalogError:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageInstallFailedContent, WinGetPackageInstallCatalogError, installResult.ExtendedErrorCode is not null ? installResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                        // 内部错误
                                        case InstallResultStatus.InternalError:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageInstallFailedContent, WinGetPackageInstallInternalError, installResult.ExtendedErrorCode is not null ? installResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                        // 非法错误
                                        case InstallResultStatus.InvalidOptions:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageInstallFailedContent, WinGetPackageInstallInvalidOptions, installResult.ExtendedErrorCode is not null ? installResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                        // 下载错误
                                        case InstallResultStatus.DownloadError:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageInstallFailedContent, WinGetPackageInstallDownloadError, installResult.ExtendedErrorCode is not null ? installResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                        // 安装错误
                                        case InstallResultStatus.InstallError:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageInstallFailedContent, WinGetPackageInstallError, installResult.ExtendedErrorCode is not null ? installResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                        // 清单错误
                                        case InstallResultStatus.ManifestError:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageInstallFailedContent, WinGetPackageInstallManifestError, installResult.ExtendedErrorCode is not null ? installResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                        // 没有合适的应用安装包
                                        case InstallResultStatus.NoApplicableInstallers:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageInstallFailedContent, WinGetPackageInstallNoApplicableInstallers, installResult.ExtendedErrorCode is not null ? installResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                        // 未接受许可协议
                                        case InstallResultStatus.PackageAgreementsNotAccepted:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageInstallFailedContent, WinGetPackageInstallAgreementsNotAccepted, installResult.ExtendedErrorCode is not null ? installResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                    }
                                }
                                // 不存在安装结果
                                else
                                {
                                    PackageOperationLock.Enter();
                                    try
                                    {
                                        packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                        packageOperation.PackageOperationFailedContent = Unknown;
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
                            }
                            // 应用包操作失败
                            else
                            {
                                // 安装任务已取消
                                if (isCanceled)
                                {
                                    PackageOperationLock.Enter();
                                    try
                                    {
                                        PackageOperationCollection.Remove(packageOperation);
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
                                // 安装任务发生其他异常
                                else
                                {
                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                    packageOperation.PackageOperationFailedContent = string.Format(PackageInstallFailedContent, WinGetPackageInstallOtherError, exception is not null ? exception.HResult : Unknown);
                                }
                            }

                            ShowTaskCompletedNotification();
                        }
                        break;
                    }
                // 添加卸载任务
                case PackageOperationKind.Uninstall:
                    {
                        bool isExisted = AddPackageOperationTask(packageOperation);

                        if (!isExisted)
                        {
                            (bool result, bool isCanceled, UninstallResult uninstallResult, Exception exception) = await Task.Run(async () =>
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
                                        packageOperation.PackageUninstallProgress = uninstallPackageWithProgress;
                                    }
                                    catch (Exception e)
                                    {
                                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                    }
                                    finally
                                    {
                                        PackageOperationLock.Exit();
                                    }

                                    // 第二部分：更新应用卸载进度
                                    uninstallPackageWithProgress.Progress = (result, progress) => OnPackageUninstallProgress(result, progress, packageOperation);
                                    return ValueTuple.Create<bool, bool, UninstallResult, Exception>(true, false, await uninstallPackageWithProgress, null);
                                }
                                // 任务已取消
                                catch (TaskCanceledException e)
                                {
                                    return ValueTuple.Create<bool, bool, UninstallResult, Exception>(false, true, null, e);
                                }
                                // 其他异常
                                catch (Exception e)
                                {
                                    LogService.WriteLog(LoggingLevel.Error, "App uninstall failed.", e);
                                    return ValueTuple.Create<bool, bool, UninstallResult, Exception>(false, false, null, e);
                                }
                            });

                            InstalledAppsPackageOperationEvent?.Invoke(result, isCanceled, packageOperation.InstalledApps, uninstallResult);

                            // 应用包操作成功
                            if (result)
                            {
                                // 存在卸载结果
                                if (uninstallResult is not null)
                                {
                                    PackageOperationLock.Enter();
                                    try
                                    {
                                        packageOperation.PackageUninstallProgressState = PackageUninstallProgressState.Finished;
                                        packageOperation.PackageOperationProgress = 100;
                                    }
                                    catch (Exception e)
                                    {
                                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                    }
                                    finally
                                    {
                                        PackageOperationLock.Exit();
                                    }

                                    switch (uninstallResult.Status)
                                    {
                                        // 卸载成功
                                        case UninstallResultStatus.Ok:
                                            {
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
                                                break;
                                            }
                                        // 被组策略阻止
                                        case UninstallResultStatus.BlockedByPolicy:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageUninstallFailedContent, WinGetPackageUninstallBlockedByPolicy, uninstallResult.ExtendedErrorCode is not null ? uninstallResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                        // 数据源错误
                                        case UninstallResultStatus.CatalogError:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageUninstallFailedContent, WinGetPackageUninstallCatalogError, uninstallResult.ExtendedErrorCode is not null ? uninstallResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                        // 内部错误
                                        case UninstallResultStatus.InternalError:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageUninstallFailedContent, WinGetPackageUninstallInternalError, uninstallResult.ExtendedErrorCode is not null ? uninstallResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                        // 非法错误
                                        case UninstallResultStatus.InvalidOptions:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageUninstallFailedContent, WinGetPackageUninstallInvalidOptions, uninstallResult.ExtendedErrorCode is not null ? uninstallResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                        // 卸载错误
                                        case UninstallResultStatus.UninstallError:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageUninstallFailedContent, WinGetPackageUninstallError, uninstallResult.ExtendedErrorCode is not null ? uninstallResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                        // 清单错误
                                        case UninstallResultStatus.ManifestError:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageUninstallFailedContent, WinGetPackageUninstallManifestError, uninstallResult.ExtendedErrorCode is not null ? uninstallResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                    }
                                }
                                // 不存在卸载结果
                                else
                                {
                                    PackageOperationLock.Enter();
                                    try
                                    {
                                        packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                        packageOperation.PackageOperationFailedContent = Unknown;
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
                            }
                            // 应用包操作失败
                            else
                            {
                                packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                packageOperation.PackageOperationFailedContent = string.Format(PackageUninstallFailedContent, WinGetPackageUninstallOtherError, exception is not null ? exception.HResult : Unknown);
                            }

                            ShowTaskCompletedNotification();
                        }
                        break;
                    }
                // 添加修复任务
                case PackageOperationKind.Repair:
                    {
                        bool isExisted = AddPackageOperationTask(packageOperation);

                        if (!isExisted)
                        {
                            (bool result, bool isCanceled, RepairResult repairResult, Exception exception) = await Task.Run(async () =>
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
                                        packageOperation.PackageRepairProgress = repairPackageWithProgress;
                                    }
                                    catch (Exception e)
                                    {
                                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                    }
                                    finally
                                    {
                                        PackageOperationLock.Exit();
                                    }

                                    // 第二部分：更新应用安装进度
                                    repairPackageWithProgress.Progress = (result, progress) => OnPackageRepairProgress(result, progress, packageOperation);
                                    return ValueTuple.Create<bool, bool, RepairResult, Exception>(true, false, await repairPackageWithProgress, null);
                                }
                                // 任务已取消
                                catch (TaskCanceledException e)
                                {
                                    return ValueTuple.Create<bool, bool, RepairResult, Exception>(false, true, null, e);
                                }
                                // 其他异常
                                catch (Exception e)
                                {
                                    LogService.WriteLog(LoggingLevel.Error, "App repair failed.", e);
                                    return ValueTuple.Create<bool, bool, RepairResult, Exception>(false, false, null, e);
                                }
                            });

                            // 应用包操作成功
                            if (result)
                            {
                                // 存在修复结果
                                if (repairResult is not null)
                                {
                                    PackageOperationLock.Enter();
                                    try
                                    {
                                        packageOperation.PackageRepairProgressState = PackageRepairProgressState.Finished;
                                        packageOperation.PackageOperationProgress = 100;
                                    }
                                    catch (Exception e)
                                    {
                                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                    }
                                    finally
                                    {
                                        PackageOperationLock.Exit();
                                    }

                                    switch (repairResult.Status)
                                    {
                                        // 修复成功
                                        case RepairResultStatus.Ok:
                                            {
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
                                                break;
                                            }
                                        // 被组策略阻止
                                        case RepairResultStatus.BlockedByPolicy:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationFailedContent = Unknown;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageRepairFailedContent, WinGetPackageRepairBlockedByPolicy, repairResult.ExtendedErrorCode is not null ? repairResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                        // 数据源错误
                                        case RepairResultStatus.CatalogError:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationFailedContent = Unknown;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageRepairFailedContent, WinGetPackageRepairCatalogError, repairResult.ExtendedErrorCode is not null ? repairResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                        // 内部错误
                                        case RepairResultStatus.InternalError:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageRepairFailedContent, WinGetPackageRepairInternalError, repairResult.ExtendedErrorCode is not null ? repairResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                        // 非法错误
                                        case RepairResultStatus.InvalidOptions:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageRepairFailedContent, WinGetPackageRepairInvalidOptions, repairResult.ExtendedErrorCode is not null ? repairResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                        // 修复错误
                                        case RepairResultStatus.RepairError:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageRepairFailedContent, WinGetPackageRepairError, repairResult.ExtendedErrorCode is not null ? repairResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                        // 清单错误
                                        case RepairResultStatus.ManifestError:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageRepairFailedContent, WinGetPackageRepairManifestError, repairResult.ExtendedErrorCode is not null ? repairResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                        // 没有合适的应用修复包
                                        case RepairResultStatus.NoApplicableRepairer:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageRepairFailedContent, WinGetPackageRepairNoApplicableRepairer, repairResult.ExtendedErrorCode is not null ? repairResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                        // 未接受许可协议
                                        case RepairResultStatus.PackageAgreementsNotAccepted:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageRepairFailedContent, WinGetPackageRepairAgreementsNotAccepted, repairResult.ExtendedErrorCode is not null ? repairResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                    }
                                }
                                // 不存在修复结果
                                else
                                {
                                    PackageOperationLock.Enter();
                                    try
                                    {
                                        packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                        packageOperation.PackageOperationFailedContent = Unknown;
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
                            }
                            // 应用包操作失败
                            else
                            {
                                packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                packageOperation.PackageOperationFailedContent = string.Format(PackageRepairFailedContent, WinGetPackageRepairOtherError, exception is not null ? exception.HResult : Unknown);
                            }

                            ShowTaskCompletedNotification();
                        }
                        break;
                    }
                // 添加更新任务
                case PackageOperationKind.Upgrade:
                    {
                        bool isExisted = AddPackageOperationTask(packageOperation);

                        if (!isExisted)
                        {
                            (bool result, bool isCanceled, InstallResult installResult, Exception exception) = await Task.Run(async () =>
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

                                    // 第二部分：更新应用安装进度
                                    upgradePackageWithProgress.Progress = (result, progress) => OnPackageInstallProgress(result, progress, packageOperation);
                                    return ValueTuple.Create<bool, bool, InstallResult, Exception>(true, false, await upgradePackageWithProgress, null);
                                }
                                // 任务已取消
                                catch (TaskCanceledException e)
                                {
                                    return ValueTuple.Create<bool, bool, InstallResult, Exception>(false, true, null, e);
                                }
                                // 其他异常
                                catch (Exception e)
                                {
                                    LogService.WriteLog(LoggingLevel.Error, "App upgrade failed.", e);
                                    return ValueTuple.Create<bool, bool, InstallResult, Exception>(false, false, null, e);
                                }
                            });

                            UpgradeAppsPackageOperationEvent.Invoke(result, isCanceled, packageOperation.UpgradableApps, installResult);

                            if (result)
                            {
                                // 存在更新结果
                                if (installResult is not null)
                                {
                                    PackageOperationLock.Enter();
                                    try
                                    {
                                        packageOperation.PackageInstallProgressState = PackageInstallProgressState.Finished;
                                        packageOperation.PackageOperationProgress = 100;
                                    }
                                    catch (Exception e)
                                    {
                                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                    }
                                    finally
                                    {
                                        PackageOperationLock.Exit();
                                    }

                                    switch (installResult.Status)
                                    {
                                        // 安装成功
                                        case InstallResultStatus.Ok:
                                            {
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
                                                break;
                                            }
                                        // 被组策略阻止
                                        case InstallResultStatus.BlockedByPolicy:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageUpgradeFailedContent, WinGetPackageUpgradeBlockedByPolicy, installResult.ExtendedErrorCode is not null ? installResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                        // 数据源错误
                                        case InstallResultStatus.CatalogError:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageUpgradeFailedContent, WinGetPackageUpgradeCatalogError, installResult.ExtendedErrorCode is not null ? installResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                        // 内部错误
                                        case InstallResultStatus.InternalError:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageUpgradeFailedContent, WinGetPackageUpgradeInternalError, installResult.ExtendedErrorCode is not null ? installResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                        // 非法错误
                                        case InstallResultStatus.InvalidOptions:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageUpgradeFailedContent, WinGetPackageUpgradeInvalidOptions, installResult.ExtendedErrorCode is not null ? installResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                        // 下载错误
                                        case InstallResultStatus.DownloadError:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageUpgradeFailedContent, WinGetPackageUpgradeDownloadError, installResult.ExtendedErrorCode is not null ? installResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                        // 安装错误
                                        case InstallResultStatus.InstallError:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageUpgradeFailedContent, WinGetPackageUpgradeError, installResult.ExtendedErrorCode is not null ? installResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                        // 清单错误
                                        case InstallResultStatus.ManifestError:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageUpgradeFailedContent, WinGetPackageUpgradeManifestError, installResult.ExtendedErrorCode is not null ? installResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                        // 没有合适的应用安装包
                                        case InstallResultStatus.NoApplicableInstallers:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageUpgradeFailedContent, WinGetPackageUpgradeNoApplicableInstallers, installResult.ExtendedErrorCode is not null ? installResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                        // 没有合适的应用更新包
                                        case InstallResultStatus.NoApplicableUpgrade:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageUpgradeFailedContent, WinGetPackageUpgradeNoApplicableUpgrade, installResult.ExtendedErrorCode is not null ? installResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                        // 未接受许可协议
                                        case InstallResultStatus.PackageAgreementsNotAccepted:
                                            {
                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = string.Format(PackageUpgradeFailedContent, WinGetPackageUpgradeAgreementsNotAccepted, installResult.ExtendedErrorCode is not null ? installResult.ExtendedErrorCode.HResult : Unknown);
                                                }
                                                catch (Exception e)
                                                {
                                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                                }
                                                finally
                                                {
                                                    PackageOperationLock.Exit();
                                                }
                                                break;
                                            }
                                    }
                                }
                                // 不存在更新结果
                                else
                                {
                                    PackageOperationLock.Enter();
                                    try
                                    {
                                        packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                        packageOperation.PackageOperationFailedContent = Unknown;
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
                            }
                            // 应用包操作失败
                            else
                            {
                                // 安装任务已取消
                                if (isCanceled)
                                {
                                    PackageOperationLock.Enter();
                                    try
                                    {
                                        PackageOperationCollection.Remove(packageOperation);
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
                                // 安装任务发生其他异常
                                else
                                {
                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                    packageOperation.PackageOperationFailedContent = string.Format(PackageUpgradeFailedContent, WinGetPackageUpgradeOtherError, exception is not null ? exception.HResult : Unknown);
                                }
                            }

                            ShowTaskCompletedNotification();
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
                    // 重复项添加时，移除已经完成的或删除已经失败的任务
                    if (packageOperationItem.PackageOperationKind is PackageOperationKind.Download && (packageOperationItem.PackageDownloadProgressState is PackageDownloadProgressState.Finished || packageOperationItem.PackageOperationResultKind is PackageOperationResultKind.Failed))
                    {
                        PackageOperationCollection.Remove(packageOperationItem);
                        break;
                    }
                    else if (packageOperationItem.PackageOperationKind is PackageOperationKind.Install && (packageOperationItem.PackageInstallProgressState is PackageInstallProgressState.Finished || packageOperationItem.PackageOperationResultKind is PackageOperationResultKind.Failed))
                    {
                        PackageOperationCollection.Remove(packageOperationItem);
                        break;
                    }
                    else if (packageOperationItem.PackageOperationKind is PackageOperationKind.Uninstall && (packageOperationItem.PackageUninstallProgressState is PackageUninstallProgressState.Finished || packageOperationItem.PackageOperationResultKind is PackageOperationResultKind.Failed))
                    {
                        PackageOperationCollection.Remove(packageOperationItem);
                        break;
                    }
                    else if (packageOperationItem.PackageOperationKind is PackageOperationKind.Repair && (packageOperationItem.PackageRepairProgressState is PackageRepairProgressState.Finished || packageOperationItem.PackageOperationResultKind is PackageOperationResultKind.Failed))
                    {
                        PackageOperationCollection.Remove(packageOperationItem);
                        break;
                    }
                    else if (packageOperationItem.PackageOperationKind is PackageOperationKind.Upgrade && (packageOperationItem.PackageInstallProgressState is PackageInstallProgressState.Finished || packageOperationItem.PackageOperationResultKind is PackageOperationResultKind.Failed))
                    {
                        PackageOperationCollection.Remove(packageOperationItem);
                        break;
                    }
                    else if (Equals(packageOperation.AppID, packageOperationItem.AppID) && Equals(packageOperation.AppVersion, packageOperationItem.AppVersion) && Equals(packageOperation.PackageOperationKind, packageOperationItem.PackageOperationKind))
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

        /// <summary>
        /// 显示所有任务都已经操作完成通知
        /// </summary>
        private void ShowTaskCompletedNotification()
        {
            bool taskCompleted = true;
            PackageOperationLock.Enter();

            try
            {
                foreach (PackageOperationModel packageOperationItem in PackageOperationCollection)
                {
                    if (packageOperationItem.PackageOperationResultKind is PackageOperationResultKind.Normal)
                    {
                        if (packageOperationItem.PackageOperationKind is PackageOperationKind.Download && packageOperationItem.PackageDownloadProgressState is not PackageDownloadProgressState.Finished)
                        {
                            taskCompleted = false;
                            break;
                        }
                        else if (packageOperationItem.PackageOperationKind is PackageOperationKind.Install && packageOperationItem.PackageInstallProgressState is not PackageInstallProgressState.Finished)
                        {
                            taskCompleted = false;
                            break;
                        }
                        else if (packageOperationItem.PackageOperationKind is PackageOperationKind.Uninstall && packageOperationItem.PackageUninstallProgressState is not PackageUninstallProgressState.Finished)
                        {
                            taskCompleted = false;
                            break;
                        }
                        else if (packageOperationItem.PackageOperationKind is PackageOperationKind.Repair && packageOperationItem.PackageRepairProgressState is not PackageRepairProgressState.Finished)
                        {
                            taskCompleted = false;
                            break;
                        }
                        else if (packageOperationItem.PackageOperationKind is PackageOperationKind.Upgrade && packageOperationItem.PackageInstallProgressState is not PackageInstallProgressState.Finished)
                        {
                            taskCompleted = false;
                            break;
                        }
                    }
                    else if (packageOperationItem.PackageOperationResultKind is PackageOperationResultKind.Cancel)
                    {
                        taskCompleted = false;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
            }

            if (taskCompleted)
            {
                Task.Run(() =>
                {
                    // 显示 WinGet 应用操作完成通知
                    AppNotificationBuilder appNotificationBuilder = new();
                    appNotificationBuilder.AddArgument("action", "OpenApp");
                    appNotificationBuilder.AddText(WinGetPackageOperationCompleted);
                    ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                });
            }
        }
    }
}
