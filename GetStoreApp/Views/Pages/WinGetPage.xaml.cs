using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.Dialogs;
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
        private readonly string PackageDownloadFailedContent1String = ResourceService.GetLocalized("WinGet/PackageDownloadFailedContent1");
        private readonly string PackageDownloadFailedContent2String = ResourceService.GetLocalized("WinGet/PackageDownloadFailedContent2");
        private readonly string PackageDownloadFailedContent3String = ResourceService.GetLocalized("WinGet/PackageDownloadFailedContent3");
        private readonly string PackageDownloadFailedContent4String = ResourceService.GetLocalized("WinGet/PackageDownloadFailedContent4");
        private readonly string PackageInstallFailedContent1String = ResourceService.GetLocalized("WinGet/PackageInstallFailedContent1");
        private readonly string PackageInstallFailedContent2String = ResourceService.GetLocalized("WinGet/PackageInstallFailedContent2");
        private readonly string PackageInstallFailedContent3String = ResourceService.GetLocalized("WinGet/PackageInstallFailedContent3");
        private readonly string PackageInstallFailedContent4String = ResourceService.GetLocalized("WinGet/PackageInstallFailedContent4");
        private readonly string PackageRepairFailedContent1String = ResourceService.GetLocalized("WinGet/PackageRepairFailedContent1");
        private readonly string PackageRepairFailedContent2String = ResourceService.GetLocalized("WinGet/PackageRepairFailedContent2");
        private readonly string PackageRepairFailedContent3String = ResourceService.GetLocalized("WinGet/PackageRepairFailedContent3");
        private readonly string PackageRepairFailedContent4String = ResourceService.GetLocalized("WinGet/PackageRepairFailedContent4");
        private readonly string PackageUninstallFailedContent1String = ResourceService.GetLocalized("WinGet/PackageUninstallFailedContent1");
        private readonly string PackageUninstallFailedContent2String = ResourceService.GetLocalized("WinGet/PackageUninstallFailedContent2");
        private readonly string PackageUninstallFailedContent3String = ResourceService.GetLocalized("WinGet/PackageUninstallFailedContent3");
        private readonly string PackageUninstallFailedContent4String = ResourceService.GetLocalized("WinGet/PackageUninstallFailedContent4");
        private readonly string PackageUpgradeFailedContent1String = ResourceService.GetLocalized("WinGet/PackageUpgradeFailedContent1");
        private readonly string PackageUpgradeFailedContent2String = ResourceService.GetLocalized("WinGet/PackageUpgradeFailedContent2");
        private readonly string PackageUpgradeFailedContent3String = ResourceService.GetLocalized("WinGet/PackageUpgradeFailedContent3");
        private readonly string PackageUpgradeFailedContent4String = ResourceService.GetLocalized("WinGet/PackageUpgradeFailedContent4");
        private readonly string RestartPCString = ResourceService.GetLocalized("WinGet/RestartPC");
        private readonly string UnknownString = ResourceService.GetLocalized("WinGet/Unknown");
        private readonly string WinGetPackageDownloadAgreementsNotAcceptedString = ResourceService.GetLocalized("WinGet/WinGetPackageDownloadAgreementsNotAccepted");
        private readonly string WinGetPackageDownloadBlockedByPolicyString = ResourceService.GetLocalized("WinGet/WinGetPackageDownloadBlockedByPolicy");
        private readonly string WinGetPackageDownloadCatalogErrorString = ResourceService.GetLocalized("WinGet/WinGetPackageDownloadCatalogError");
        private readonly string WinGetPackageDownloadErrorString = ResourceService.GetLocalized("WinGet/WinGetPackageDownloadError");
        private readonly string WinGetPackageDownloadInternalErrorString = ResourceService.GetLocalized("WinGet/WinGetPackageDownloadInternalError");
        private readonly string WinGetPackageDownloadInvalidOptionsString = ResourceService.GetLocalized("WinGet/WinGetPackageDownloadInvalidOptions");
        private readonly string WinGetPackageDownloadManifestErrorString = ResourceService.GetLocalized("WinGet/WinGetPackageDownloadManifestError");
        private readonly string WinGetPackageDownloadNoApplicableInstallersString = ResourceService.GetLocalized("WinGet/WinGetPackageDownloadNoApplicableInstallers");
        private readonly string WinGetPackageDownloadOtherErrorString = ResourceService.GetLocalized("WinGet/WinGetPackageDownloadOtherError");
        private readonly string WinGetPackageInstallAgreementsNotAcceptedString = ResourceService.GetLocalized("WinGet/WinGetPackageInstallAgreementsNotAccepted");
        private readonly string WinGetPackageInstallBlockedByPolicyString = ResourceService.GetLocalized("WinGet/WinGetPackageInstallBlockedByPolicy");
        private readonly string WinGetPackageInstallCatalogErrorString = ResourceService.GetLocalized("WinGet/WinGetPackageInstallCatalogError");
        private readonly string WinGetPackageInstallDownloadErrorString = ResourceService.GetLocalized("WinGet/WinGetPackageInstallDownloadError");
        private readonly string WinGetPackageInstallErrorString = ResourceService.GetLocalized("WinGet/WinGetPackageInstallError");
        private readonly string WinGetPackageInstallInternalErrorString = ResourceService.GetLocalized("WinGet/WinGetPackageInstallInternalError");
        private readonly string WinGetPackageInstallInvalidOptionsString = ResourceService.GetLocalized("WinGet/WinGetPackageInstallInvalidOptions");
        private readonly string WinGetPackageInstallManifestErrorString = ResourceService.GetLocalized("WinGet/WinGetPackageInstallManifestError");
        private readonly string WinGetPackageInstallNoApplicableInstallersString = ResourceService.GetLocalized("WinGet/WinGetPackageInstallNoApplicableInstallers");
        private readonly string WinGetPackageInstallOtherErrorString = ResourceService.GetLocalized("WinGet/WinGetPackageInstallOtherError");
        private readonly string WinGetPackageOperationCompletedString = ResourceService.GetLocalized("WinGet/WinGetPackageOperationCompleted");
        private readonly string WinGetPackageRepairAgreementsNotAcceptedString = ResourceService.GetLocalized("WinGet/WinGetPackageRepairAgreementsNotAccepted");
        private readonly string WinGetPackageRepairBlockedByPolicyString = ResourceService.GetLocalized("WinGet/WinGetPackageRepairBlockedByPolicy");
        private readonly string WinGetPackageRepairCatalogErrorString = ResourceService.GetLocalized("WinGet/WinGetPackageRepairCatalogError");
        private readonly string WinGetPackageRepairErrorString = ResourceService.GetLocalized("WinGet/WinGetPackageRepairError");
        private readonly string WinGetPackageRepairInternalErrorString = ResourceService.GetLocalized("WinGet/WinGetPackageRepairInternalError");
        private readonly string WinGetPackageRepairInvalidOptionsString = ResourceService.GetLocalized("WinGet/WinGetPackageRepairInvalidOptions");
        private readonly string WinGetPackageRepairManifestErrorString = ResourceService.GetLocalized("WinGet/WinGetPackageRepairManifestError");
        private readonly string WinGetPackageRepairNoApplicableRepairerString = ResourceService.GetLocalized("WinGet/WinGetPackageRepairNoApplicableRepairer");
        private readonly string WinGetPackageRepairOtherErrorString = ResourceService.GetLocalized("WinGet/WinGetPackageRepairOtherError");
        private readonly string WinGetPackageUninstallBlockedByPolicyString = ResourceService.GetLocalized("WinGet/WinGetPackageUninstallBlockedByPolicy");
        private readonly string WinGetPackageUninstallCatalogErrorString = ResourceService.GetLocalized("WinGet/WinGetPackageUninstallCatalogError");
        private readonly string WinGetPackageUninstallInternalErrorString = ResourceService.GetLocalized("WinGet/WinGetPackageUninstallInternalError");
        private readonly string WinGetPackageUninstallInvalidOptionsString = ResourceService.GetLocalized("WinGet/WinGetPackageUninstallInvalidOptions");
        private readonly string WinGetPackageUninstallErrorString = ResourceService.GetLocalized("WinGet/WinGetPackageUninstallError");
        private readonly string WinGetPackageUninstallManifestErrorString = ResourceService.GetLocalized("WinGet/WinGetPackageUninstallManifestError");
        private readonly string WinGetPackageUninstallOtherErrorString = ResourceService.GetLocalized("WinGet/WinGetPackageUninstallOtherError");
        private readonly string WinGetPackageUpgradeAgreementsNotAcceptedString = ResourceService.GetLocalized("WinGet/WinGetPackageUpgradeAgreementsNotAccepted");
        private readonly string WinGetPackageUpgradeBlockedByPolicyString = ResourceService.GetLocalized("WinGet/WinGetPackageUpgradeBlockedByPolicy");
        private readonly string WinGetPackageUpgradeCatalogErrorString = ResourceService.GetLocalized("WinGet/WinGetPackageUpgradeCatalogError");
        private readonly string WinGetPackageUpgradeDownloadErrorString = ResourceService.GetLocalized("WinGet/WinGetPackageUpgradeDownloadError");
        private readonly string WinGetPackageUpgradeErrorString = ResourceService.GetLocalized("WinGet/WinGetPackageUpgradeError");
        private readonly string WinGetPackageUpgradeInternalErrorString = ResourceService.GetLocalized("WinGet/WinGetPackageUpgradeInternalError");
        private readonly string WinGetPackageUpgradeInvalidOptionsString = ResourceService.GetLocalized("WinGet/WinGetPackageUpgradeInvalidOptions");
        private readonly string WinGetPackageUpgradeManifestErrorString = ResourceService.GetLocalized("WinGet/WinGetPackageUpgradeManifestError");
        private readonly string WinGetPackageUpgradeNoApplicableInstallersString = ResourceService.GetLocalized("WinGet/WinGetPackageUpgradeNoApplicableInstallers");
        private readonly string WinGetPackageUpgradeNoApplicableUpgradeString = ResourceService.GetLocalized("WinGet/WinGetPackageUpgradeNoApplicableUpgrade");
        private readonly string WinGetPackageUpgradeOtherErrorString = ResourceService.GetLocalized("WinGet/WinGetPackageUpgradeOtherError");
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

        private WinGetPaneKind _winGetPaneKind;

        public WinGetPaneKind WinGetPaneKind
        {
            get { return _winGetPaneKind; }

            set
            {
                if (!Equals(_winGetPaneKind, value))
                {
                    _winGetPaneKind = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WinGetPaneKind)));
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
            if (GetCurrentPageType() is null)
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
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetPage), nameof(OnCancelTaskExecuteRequested), 1, e);
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
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetPage), nameof(OnRemoveTaskExecuteRequested), 1, e);
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
                    try
                    {
                        await Launcher.LaunchFolderAsync(await StorageFolder.GetFolderFromPathAsync(packagePath));
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                    }
                });
            }
        }

        #endregion 第二部分：XamlUICommand 命令调用时挂载的事件

        #region 第三部分：WinGet 程序包页面——挂载的事件

        /// <summary>
        /// 了解 WinGet 程序包
        /// </summary>
        private void OnLearnWinGetClicked(object sender, RoutedEventArgs args)
        {
            WinGetSplitView.IsPaneOpen = false;
        }

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

            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetPage), nameof(OnNavigationFailed), 1, args.Exception);
        }

        #endregion 第三部分：WinGet 程序包页面——挂载的事件

        #region 第四部分：WinGet 程序包页面——自定义事件

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

        #endregion 第四部分：WinGet 程序包页面——自定义事件

        /// <summary>
        /// 显示任务管理
        /// </summary>
        public void ShowTaskManager()
        {
            WinGetPaneKind = WinGetPaneKind.TaskManager;

            if (!WinGetSplitView.IsPaneOpen)
            {
                WinGetSplitView.OpenPaneLength = 400;
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
                                    IAsyncOperationWithProgress<DownloadResult, PackageDownloadProgress> downloadPackageWithProgress = packageManager.DownloadPackageAsync(packageOperation.SearchApps.CatalogPackage, packageOperation.DownloadOptions);

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
                                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetPage), nameof(AddTaskAsync), 1, e);
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
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    List<string> packageDownloadFailedContentList = [];
                                                    packageDownloadFailedContentList.Add(PackageDownloadFailedContent1String);
                                                    packageDownloadFailedContentList.Add(downloadResult.ExtendedErrorCode is not null ? string.Format(PackageDownloadFailedContent2String, WinGetPackageDownloadBlockedByPolicyString) : string.Format(PackageDownloadFailedContent3String, WinGetPackageDownloadBlockedByPolicyString, downloadResult.ExtendedErrorCode.Message));
                                                    packageDownloadFailedContentList.Add(downloadResult.ExtendedErrorCode is not null ? string.Format(PackageDownloadFailedContent4String, "0x" + Convert.ToString(downloadResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageDownloadFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageDownloadFailedContentList = [];
                                                    packageDownloadFailedContentList.Add(PackageDownloadFailedContent1String);
                                                    packageDownloadFailedContentList.Add(downloadResult.ExtendedErrorCode is not null ? string.Format(PackageDownloadFailedContent2String, WinGetPackageDownloadCatalogErrorString) : string.Format(PackageDownloadFailedContent3String, WinGetPackageDownloadCatalogErrorString, downloadResult.ExtendedErrorCode.Message));
                                                    packageDownloadFailedContentList.Add(downloadResult.ExtendedErrorCode is not null ? string.Format(PackageDownloadFailedContent4String, "0x" + Convert.ToString(downloadResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageDownloadFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageDownloadFailedContentList = [];
                                                    packageDownloadFailedContentList.Add(PackageDownloadFailedContent1String);
                                                    packageDownloadFailedContentList.Add(downloadResult.ExtendedErrorCode is not null ? string.Format(PackageDownloadFailedContent2String, WinGetPackageDownloadInternalErrorString) : string.Format(PackageDownloadFailedContent3String, WinGetPackageDownloadInternalErrorString, downloadResult.ExtendedErrorCode.Message));
                                                    packageDownloadFailedContentList.Add(downloadResult.ExtendedErrorCode is not null ? string.Format(PackageDownloadFailedContent4String, "0x" + Convert.ToString(downloadResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageDownloadFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageDownloadFailedContentList = [];
                                                    packageDownloadFailedContentList.Add(PackageDownloadFailedContent1String);
                                                    packageDownloadFailedContentList.Add(downloadResult.ExtendedErrorCode is not null ? string.Format(PackageDownloadFailedContent2String, WinGetPackageDownloadInvalidOptionsString) : string.Format(PackageDownloadFailedContent3String, WinGetPackageDownloadInvalidOptionsString, downloadResult.ExtendedErrorCode.Message));
                                                    packageDownloadFailedContentList.Add(downloadResult.ExtendedErrorCode is not null ? string.Format(PackageDownloadFailedContent4String, "0x" + Convert.ToString(downloadResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageDownloadFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageDownloadFailedContentList = [];
                                                    packageDownloadFailedContentList.Add(PackageDownloadFailedContent1String);
                                                    packageDownloadFailedContentList.Add(downloadResult.ExtendedErrorCode is not null ? string.Format(PackageDownloadFailedContent2String, WinGetPackageDownloadErrorString) : string.Format(PackageDownloadFailedContent3String, WinGetPackageDownloadErrorString, downloadResult.ExtendedErrorCode.Message));
                                                    packageDownloadFailedContentList.Add(downloadResult.ExtendedErrorCode is not null ? string.Format(PackageDownloadFailedContent4String, "0x" + Convert.ToString(downloadResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageDownloadFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageDownloadFailedContentList = [];
                                                    packageDownloadFailedContentList.Add(PackageDownloadFailedContent1String);
                                                    packageDownloadFailedContentList.Add(downloadResult.ExtendedErrorCode is not null ? string.Format(PackageDownloadFailedContent2String, WinGetPackageDownloadManifestErrorString) : string.Format(PackageDownloadFailedContent3String, WinGetPackageDownloadManifestErrorString, downloadResult.ExtendedErrorCode.Message));
                                                    packageDownloadFailedContentList.Add(downloadResult.ExtendedErrorCode is not null ? string.Format(PackageDownloadFailedContent4String, "0x" + Convert.ToString(downloadResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageDownloadFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageDownloadFailedContentList = [];
                                                    packageDownloadFailedContentList.Add(PackageDownloadFailedContent1String);
                                                    packageDownloadFailedContentList.Add(downloadResult.ExtendedErrorCode is not null ? string.Format(PackageDownloadFailedContent2String, WinGetPackageDownloadNoApplicableInstallersString) : string.Format(PackageDownloadFailedContent3String, WinGetPackageDownloadNoApplicableInstallersString, downloadResult.ExtendedErrorCode.Message));
                                                    packageDownloadFailedContentList.Add(downloadResult.ExtendedErrorCode is not null ? string.Format(PackageDownloadFailedContent4String, "0x" + Convert.ToString(downloadResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageDownloadFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageDownloadFailedContentList = [];
                                                    packageDownloadFailedContentList.Add(PackageDownloadFailedContent1String);
                                                    packageDownloadFailedContentList.Add(downloadResult.ExtendedErrorCode is not null ? string.Format(PackageDownloadFailedContent2String, WinGetPackageDownloadAgreementsNotAcceptedString) : string.Format(PackageDownloadFailedContent3String, WinGetPackageDownloadAgreementsNotAcceptedString, downloadResult.ExtendedErrorCode.Message));
                                                    packageDownloadFailedContentList.Add(downloadResult.ExtendedErrorCode is not null ? string.Format(PackageDownloadFailedContent4String, "0x" + Convert.ToString(downloadResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageDownloadFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                        packageOperation.PackageOperationFailedContent = UnknownString;
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
                                    string packageOperationFailedContent = await Task.Run(() =>
                                    {
                                        List<string> packageDownloadFailedContentList = [];
                                        packageDownloadFailedContentList.Add(PackageDownloadFailedContent1String);
                                        packageDownloadFailedContentList.Add(exception is not null ? string.Format(PackageDownloadFailedContent2String, WinGetPackageDownloadOtherErrorString) : string.Format(PackageDownloadFailedContent3String, WinGetPackageDownloadOtherErrorString, exception.Message));
                                        packageDownloadFailedContentList.Add(exception is not null ? string.Format(PackageDownloadFailedContent4String, "0x" + Convert.ToString(exception.HResult, 16).ToUpper()) : UnknownString);
                                        return string.Join(Environment.NewLine, packageDownloadFailedContentList);
                                    });

                                    PackageOperationLock.Enter();
                                    try
                                    {
                                        packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                        packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                    IAsyncOperationWithProgress<InstallResult, InstallProgress> installPackageWithProgress = packageManager.InstallPackageAsync(packageOperation.SearchApps.CatalogPackage, packageOperation.InstallOptions);

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
                                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetPage), nameof(AddTaskAsync), 2, e);
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
                                                            ShutdownHelper.Restart(RestartPCString, TimeSpan.FromSeconds(120));
                                                        });
                                                    }
                                                }
                                                break;
                                            }
                                        // 被组策略阻止
                                        case InstallResultStatus.BlockedByPolicy:
                                            {
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageInstallFailedContentList = [];
                                                    packageInstallFailedContentList.Add(PackageInstallFailedContent1String);
                                                    packageInstallFailedContentList.Add(installResult.ExtendedErrorCode is not null ? string.Format(PackageInstallFailedContent2String, WinGetPackageInstallBlockedByPolicyString) : string.Format(PackageInstallFailedContent3String, WinGetPackageInstallBlockedByPolicyString, installResult.ExtendedErrorCode.Message));
                                                    packageInstallFailedContentList.Add(installResult.ExtendedErrorCode is not null ? string.Format(PackageInstallFailedContent4String, "0x" + Convert.ToString(installResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageInstallFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageInstallFailedContentList = [];
                                                    packageInstallFailedContentList.Add(PackageInstallFailedContent1String);
                                                    packageInstallFailedContentList.Add(installResult.ExtendedErrorCode is not null ? string.Format(PackageInstallFailedContent2String, WinGetPackageInstallCatalogErrorString) : string.Format(PackageInstallFailedContent3String, WinGetPackageInstallCatalogErrorString, installResult.ExtendedErrorCode.Message));
                                                    packageInstallFailedContentList.Add(installResult.ExtendedErrorCode is not null ? string.Format(PackageInstallFailedContent2String, "0x" + Convert.ToString(installResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageInstallFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageInstallFailedContentList = [];
                                                    packageInstallFailedContentList.Add(PackageInstallFailedContent1String);
                                                    packageInstallFailedContentList.Add(installResult.ExtendedErrorCode is not null ? string.Format(PackageInstallFailedContent2String, WinGetPackageInstallInternalErrorString) : string.Format(PackageInstallFailedContent3String, WinGetPackageInstallInternalErrorString, installResult.ExtendedErrorCode.Message));
                                                    packageInstallFailedContentList.Add(installResult.ExtendedErrorCode is not null ? string.Format(PackageInstallFailedContent4String, "0x" + Convert.ToString(installResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageInstallFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageInstallFailedContentList = [];
                                                    packageInstallFailedContentList.Add(PackageInstallFailedContent1String);
                                                    packageInstallFailedContentList.Add(installResult.ExtendedErrorCode is not null ? string.Format(PackageInstallFailedContent2String, WinGetPackageInstallInvalidOptionsString) : string.Format(PackageInstallFailedContent3String, WinGetPackageInstallInvalidOptionsString, installResult.ExtendedErrorCode.Message));
                                                    packageInstallFailedContentList.Add(installResult.ExtendedErrorCode is not null ? string.Format(PackageInstallFailedContent4String, "0x" + Convert.ToString(installResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageInstallFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageInstallFailedContentList = [];
                                                    packageInstallFailedContentList.Add(PackageInstallFailedContent1String);
                                                    packageInstallFailedContentList.Add(installResult.ExtendedErrorCode is not null ? string.Format(PackageInstallFailedContent2String, WinGetPackageInstallDownloadErrorString) : string.Format(PackageInstallFailedContent3String, WinGetPackageInstallDownloadErrorString, installResult.ExtendedErrorCode.Message));
                                                    packageInstallFailedContentList.Add(installResult.ExtendedErrorCode is not null ? string.Format(PackageInstallFailedContent4String, "0x" + Convert.ToString(installResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageInstallFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageInstallFailedContentList = [];
                                                    packageInstallFailedContentList.Add(PackageInstallFailedContent1String);
                                                    packageInstallFailedContentList.Add(installResult.ExtendedErrorCode is not null ? string.Format(PackageInstallFailedContent2String, WinGetPackageInstallErrorString) : string.Format(PackageInstallFailedContent3String, WinGetPackageInstallErrorString, installResult.ExtendedErrorCode.Message));
                                                    packageInstallFailedContentList.Add(installResult.ExtendedErrorCode is not null ? string.Format(PackageInstallFailedContent4String, "0x" + Convert.ToString(installResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageInstallFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageInstallFailedContentList = [];
                                                    packageInstallFailedContentList.Add(PackageInstallFailedContent1String);
                                                    packageInstallFailedContentList.Add(installResult.ExtendedErrorCode is not null ? string.Format(PackageInstallFailedContent2String, WinGetPackageInstallManifestErrorString) : string.Format(PackageInstallFailedContent3String, WinGetPackageInstallManifestErrorString, installResult.ExtendedErrorCode.Message));
                                                    packageInstallFailedContentList.Add(installResult.ExtendedErrorCode is not null ? string.Format(PackageInstallFailedContent4String, "0x" + Convert.ToString(installResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageInstallFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageInstallFailedContentList = [];
                                                    packageInstallFailedContentList.Add(PackageInstallFailedContent1String);
                                                    packageInstallFailedContentList.Add(installResult.ExtendedErrorCode is not null ? string.Format(PackageInstallFailedContent2String, WinGetPackageInstallNoApplicableInstallersString) : string.Format(PackageInstallFailedContent3String, WinGetPackageInstallNoApplicableInstallersString, installResult.ExtendedErrorCode.Message));
                                                    packageInstallFailedContentList.Add(installResult.ExtendedErrorCode is not null ? string.Format(PackageInstallFailedContent4String, "0x" + Convert.ToString(installResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageInstallFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageInstallFailedContentList = [];
                                                    packageInstallFailedContentList.Add(PackageInstallFailedContent1String);
                                                    packageInstallFailedContentList.Add(installResult.ExtendedErrorCode is not null ? string.Format(PackageInstallFailedContent2String, WinGetPackageInstallAgreementsNotAcceptedString) : string.Format(PackageInstallFailedContent3String, WinGetPackageInstallAgreementsNotAcceptedString, installResult.ExtendedErrorCode.Message));
                                                    packageInstallFailedContentList.Add(installResult.ExtendedErrorCode is not null ? string.Format(PackageInstallFailedContent4String, "0x" + Convert.ToString(installResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageInstallFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                        packageOperation.PackageOperationFailedContent = UnknownString;
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
                                    string packageOperationFailedContent = await Task.Run(() =>
                                    {
                                        List<string> packageInstallFailedContentList = [];
                                        packageInstallFailedContentList.Add(PackageInstallFailedContent1String);
                                        packageInstallFailedContentList.Add(exception is not null ? string.Format(PackageInstallFailedContent2String, WinGetPackageInstallOtherErrorString) : string.Format(PackageInstallFailedContent3String, WinGetPackageInstallOtherErrorString, exception.Message));
                                        packageInstallFailedContentList.Add(exception is not null ? string.Format(PackageInstallFailedContent4String, "0x" + Convert.ToString(exception.HResult, 16).ToUpper()) : UnknownString);
                                        return string.Join(Environment.NewLine, packageInstallFailedContentList);
                                    });

                                    PackageOperationLock.Enter();
                                    try
                                    {
                                        packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                        packageOperation.PackageOperationFailedContent = UnknownString;
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
                                    IAsyncOperationWithProgress<UninstallResult, UninstallProgress> uninstallPackageWithProgress = packageManager.UninstallPackageAsync(packageOperation.InstalledApps.CatalogPackage, packageOperation.UninstallOptions);

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
                                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetPage), nameof(AddTaskAsync), 3, e);
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
                                                            ShutdownHelper.Restart(RestartPCString, TimeSpan.FromSeconds(120));
                                                        });
                                                    }
                                                }
                                                break;
                                            }
                                        // 被组策略阻止
                                        case UninstallResultStatus.BlockedByPolicy:
                                            {
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageUninstallFailedContentList = [];
                                                    packageUninstallFailedContentList.Add(PackageUninstallFailedContent1String);
                                                    packageUninstallFailedContentList.Add(uninstallResult.ExtendedErrorCode is not null ? string.Format(PackageUninstallFailedContent2String, WinGetPackageUninstallBlockedByPolicyString) : string.Format(PackageUninstallFailedContent3String, WinGetPackageUninstallBlockedByPolicyString, uninstallResult.ExtendedErrorCode.Message));
                                                    packageUninstallFailedContentList.Add(uninstallResult.ExtendedErrorCode is not null ? string.Format(PackageUninstallFailedContent4String, "0x" + Convert.ToString(uninstallResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageUninstallFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageUninstallFailedContentList = [];
                                                    packageUninstallFailedContentList.Add(PackageUninstallFailedContent1String);
                                                    packageUninstallFailedContentList.Add(uninstallResult.ExtendedErrorCode is not null ? string.Format(PackageUninstallFailedContent2String, WinGetPackageUninstallCatalogErrorString) : string.Format(PackageUninstallFailedContent3String, WinGetPackageUninstallCatalogErrorString, uninstallResult.ExtendedErrorCode.Message));
                                                    packageUninstallFailedContentList.Add(uninstallResult.ExtendedErrorCode is not null ? string.Format(PackageUninstallFailedContent4String, "0x" + Convert.ToString(uninstallResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageUninstallFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageUninstallFailedContentList = [];
                                                    packageUninstallFailedContentList.Add(PackageUninstallFailedContent1String);
                                                    packageUninstallFailedContentList.Add(uninstallResult.ExtendedErrorCode is not null ? string.Format(PackageUninstallFailedContent2String, WinGetPackageUninstallInternalErrorString) : string.Format(PackageUninstallFailedContent3String, WinGetPackageUninstallInternalErrorString, uninstallResult.ExtendedErrorCode.Message));
                                                    packageUninstallFailedContentList.Add(uninstallResult.ExtendedErrorCode is not null ? string.Format(PackageUninstallFailedContent4String, "0x" + Convert.ToString(uninstallResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageUninstallFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageUninstallFailedContentList = [];
                                                    packageUninstallFailedContentList.Add(PackageUninstallFailedContent1String);
                                                    packageUninstallFailedContentList.Add(uninstallResult.ExtendedErrorCode is not null ? string.Format(PackageUninstallFailedContent2String, WinGetPackageUninstallInvalidOptionsString) : string.Format(PackageUninstallFailedContent3String, WinGetPackageUninstallInvalidOptionsString, uninstallResult.ExtendedErrorCode.Message));
                                                    packageUninstallFailedContentList.Add(uninstallResult.ExtendedErrorCode is not null ? string.Format(PackageUninstallFailedContent4String, "0x" + Convert.ToString(uninstallResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageUninstallFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageUninstallFailedContentList = [];
                                                    packageUninstallFailedContentList.Add(PackageUninstallFailedContent1String);
                                                    packageUninstallFailedContentList.Add(uninstallResult.ExtendedErrorCode is not null ? string.Format(PackageUninstallFailedContent2String, WinGetPackageUninstallErrorString) : string.Format(PackageUninstallFailedContent3String, WinGetPackageUninstallErrorString, uninstallResult.ExtendedErrorCode.Message));
                                                    packageUninstallFailedContentList.Add(uninstallResult.ExtendedErrorCode is not null ? string.Format(PackageUninstallFailedContent4String, "0x" + Convert.ToString(uninstallResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageUninstallFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageUninstallFailedContentList = [];
                                                    packageUninstallFailedContentList.Add(PackageUninstallFailedContent1String);
                                                    packageUninstallFailedContentList.Add(uninstallResult.ExtendedErrorCode is not null ? string.Format(PackageUninstallFailedContent2String, WinGetPackageUninstallManifestErrorString) : string.Format(PackageUninstallFailedContent3String, WinGetPackageUninstallManifestErrorString, uninstallResult.ExtendedErrorCode.Message));
                                                    packageUninstallFailedContentList.Add(uninstallResult.ExtendedErrorCode is not null ? string.Format(PackageUninstallFailedContent4String, "0x" + Convert.ToString(uninstallResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageUninstallFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                        packageOperation.PackageOperationFailedContent = UnknownString;
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
                                string packageOperationFailedContent = await Task.Run(() =>
                                {
                                    List<string> packageUninstallFailedContentList = [];
                                    packageUninstallFailedContentList.Add(PackageUninstallFailedContent1String);
                                    packageUninstallFailedContentList.Add(exception is not null ? string.Format(PackageUninstallFailedContent2String, WinGetPackageUninstallOtherErrorString) : string.Format(PackageUninstallFailedContent3String, WinGetPackageUninstallOtherErrorString, exception.Message));
                                    packageUninstallFailedContentList.Add(exception is not null ? string.Format(PackageUninstallFailedContent4String, "0x" + Convert.ToString(exception.HResult, 16).ToUpper()) : UnknownString);
                                    return string.Join(Environment.NewLine, packageUninstallFailedContentList);
                                });

                                PackageOperationLock.Enter();
                                try
                                {
                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                    IAsyncOperationWithProgress<RepairResult, RepairProgress> repairPackageWithProgress = packageManager.RepairPackageAsync(packageOperation.SearchApps.CatalogPackage, packageOperation.RepairOptions);

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
                                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetPage), nameof(AddTaskAsync), 4, e);
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
                                                            ShutdownHelper.Restart(RestartPCString, TimeSpan.FromSeconds(120));
                                                        });
                                                    }
                                                }
                                                break;
                                            }
                                        // 被组策略阻止
                                        case RepairResultStatus.BlockedByPolicy:
                                            {
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageRepairFailedContentList = [];
                                                    packageRepairFailedContentList.Add(PackageRepairFailedContent1String);
                                                    packageRepairFailedContentList.Add(repairResult.ExtendedErrorCode is not null ? string.Format(PackageRepairFailedContent2String, WinGetPackageRepairBlockedByPolicyString) : string.Format(PackageRepairFailedContent3String, WinGetPackageRepairBlockedByPolicyString, repairResult.ExtendedErrorCode.Message));
                                                    packageRepairFailedContentList.Add(repairResult.ExtendedErrorCode is not null ? string.Format(PackageRepairFailedContent4String, "0x" + Convert.ToString(repairResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageRepairFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationFailedContent = UnknownString;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageRepairFailedContentList = [];
                                                    packageRepairFailedContentList.Add(PackageRepairFailedContent1String);
                                                    packageRepairFailedContentList.Add(repairResult.ExtendedErrorCode is not null ? string.Format(PackageRepairFailedContent2String, WinGetPackageRepairCatalogErrorString) : string.Format(PackageRepairFailedContent3String, WinGetPackageRepairCatalogErrorString, repairResult.ExtendedErrorCode.Message));
                                                    packageRepairFailedContentList.Add(repairResult.ExtendedErrorCode is not null ? string.Format(PackageRepairFailedContent4String, "0x" + Convert.ToString(repairResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageRepairFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationFailedContent = UnknownString;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageRepairFailedContentList = [];
                                                    packageRepairFailedContentList.Add(PackageRepairFailedContent1String);
                                                    packageRepairFailedContentList.Add(repairResult.ExtendedErrorCode is not null ? string.Format(PackageRepairFailedContent2String, WinGetPackageRepairInternalErrorString) : string.Format(PackageRepairFailedContent3String, WinGetPackageRepairInternalErrorString, repairResult.ExtendedErrorCode.Message));
                                                    packageRepairFailedContentList.Add(repairResult.ExtendedErrorCode is not null ? string.Format(PackageRepairFailedContent4String, "0x" + Convert.ToString(repairResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageRepairFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageRepairFailedContentList = [];
                                                    packageRepairFailedContentList.Add(PackageRepairFailedContent1String);
                                                    packageRepairFailedContentList.Add(repairResult.ExtendedErrorCode is not null ? string.Format(PackageRepairFailedContent2String, WinGetPackageRepairInvalidOptionsString) : string.Format(PackageRepairFailedContent3String, WinGetPackageRepairInvalidOptionsString, repairResult.ExtendedErrorCode.Message));
                                                    packageRepairFailedContentList.Add(repairResult.ExtendedErrorCode is not null ? string.Format(PackageRepairFailedContent4String, "0x" + Convert.ToString(repairResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageRepairFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageRepairFailedContentList = [];
                                                    packageRepairFailedContentList.Add(PackageRepairFailedContent1String);
                                                    packageRepairFailedContentList.Add(repairResult.ExtendedErrorCode is not null ? string.Format(PackageRepairFailedContent2String, WinGetPackageRepairErrorString) : string.Format(PackageRepairFailedContent3String, WinGetPackageRepairErrorString, repairResult.ExtendedErrorCode.Message));
                                                    packageRepairFailedContentList.Add(repairResult.ExtendedErrorCode is not null ? string.Format(PackageRepairFailedContent4String, "0x" + Convert.ToString(repairResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageRepairFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageRepairFailedContentList = [];
                                                    packageRepairFailedContentList.Add(PackageRepairFailedContent1String);
                                                    packageRepairFailedContentList.Add(repairResult.ExtendedErrorCode is not null ? string.Format(PackageRepairFailedContent2String, WinGetPackageRepairManifestErrorString) : string.Format(PackageRepairFailedContent3String, WinGetPackageRepairManifestErrorString, repairResult.ExtendedErrorCode.Message));
                                                    packageRepairFailedContentList.Add(repairResult.ExtendedErrorCode is not null ? string.Format(PackageRepairFailedContent4String, "0x" + Convert.ToString(repairResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageRepairFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageRepairFailedContentList = [];
                                                    packageRepairFailedContentList.Add(PackageRepairFailedContent1String);
                                                    packageRepairFailedContentList.Add(repairResult.ExtendedErrorCode is not null ? string.Format(PackageRepairFailedContent2String, WinGetPackageRepairNoApplicableRepairerString) : string.Format(PackageRepairFailedContent3String, WinGetPackageRepairNoApplicableRepairerString, repairResult.ExtendedErrorCode.Message));
                                                    packageRepairFailedContentList.Add(repairResult.ExtendedErrorCode is not null ? string.Format(PackageRepairFailedContent4String, "0x" + Convert.ToString(repairResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageRepairFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageRepairFailedContentList = [];
                                                    packageRepairFailedContentList.Add(PackageRepairFailedContent1String);
                                                    packageRepairFailedContentList.Add(repairResult.ExtendedErrorCode is not null ? string.Format(PackageRepairFailedContent2String, WinGetPackageRepairAgreementsNotAcceptedString) : string.Format(PackageRepairFailedContent3String, WinGetPackageRepairAgreementsNotAcceptedString, repairResult.ExtendedErrorCode.Message));
                                                    packageRepairFailedContentList.Add(repairResult.ExtendedErrorCode is not null ? string.Format(PackageRepairFailedContent4String, "0x" + Convert.ToString(repairResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageRepairFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                        packageOperation.PackageOperationFailedContent = UnknownString;
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
                                string packageOperationFailedContent = await Task.Run(() =>
                                {
                                    List<string> packageRepairFailedContentList = [];
                                    packageRepairFailedContentList.Add(PackageRepairFailedContent1String);
                                    packageRepairFailedContentList.Add(exception is not null ? string.Format(PackageRepairFailedContent2String, WinGetPackageRepairOtherErrorString) : string.Format(PackageRepairFailedContent3String, WinGetPackageRepairOtherErrorString, exception.Message));
                                    packageRepairFailedContentList.Add(exception is not null ? string.Format(PackageRepairFailedContent4String, "0x" + Convert.ToString(exception.HResult, 16).ToUpper()) : UnknownString);
                                    return string.Join(Environment.NewLine, packageRepairFailedContentList);
                                });

                                PackageOperationLock.Enter();
                                try
                                {
                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                    IAsyncOperationWithProgress<InstallResult, InstallProgress> upgradePackageWithProgress = packageManager.UpgradePackageAsync(packageOperation.UpgradableApps.CatalogPackage, packageOperation.InstallOptions);

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
                                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetPage), nameof(AddTaskAsync), 5, e);
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
                                                            ShutdownHelper.Restart(RestartPCString, TimeSpan.FromSeconds(120));
                                                        });
                                                    }
                                                }
                                                break;
                                            }
                                        // 被组策略阻止
                                        case InstallResultStatus.BlockedByPolicy:
                                            {
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageUpgradeFailedContentList = [];
                                                    packageUpgradeFailedContentList.Add(PackageUpgradeFailedContent1String);
                                                    packageUpgradeFailedContentList.Add(installResult.ExtendedErrorCode is not null ? string.Format(PackageUpgradeFailedContent2String, WinGetPackageUpgradeBlockedByPolicyString) : string.Format(PackageUpgradeFailedContent3String, WinGetPackageUpgradeBlockedByPolicyString, installResult.ExtendedErrorCode.Message));
                                                    packageUpgradeFailedContentList.Add(installResult.ExtendedErrorCode is not null ? string.Format(PackageUpgradeFailedContent4String, "0x" + Convert.ToString(installResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageUpgradeFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageUpgradeFailedContentList = [];
                                                    packageUpgradeFailedContentList.Add(PackageUpgradeFailedContent1String);
                                                    packageUpgradeFailedContentList.Add(installResult.ExtendedErrorCode is not null ? string.Format(PackageUpgradeFailedContent2String, WinGetPackageUpgradeCatalogErrorString) : string.Format(PackageUpgradeFailedContent3String, WinGetPackageUpgradeCatalogErrorString, installResult.ExtendedErrorCode.Message));
                                                    packageUpgradeFailedContentList.Add(installResult.ExtendedErrorCode is not null ? string.Format(PackageUpgradeFailedContent4String, "0x" + Convert.ToString(installResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageUpgradeFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageUpgradeFailedContentList = [];
                                                    packageUpgradeFailedContentList.Add(PackageUpgradeFailedContent1String);
                                                    packageUpgradeFailedContentList.Add(installResult.ExtendedErrorCode is not null ? string.Format(PackageUpgradeFailedContent2String, WinGetPackageUpgradeInternalErrorString) : string.Format(PackageUpgradeFailedContent3String, WinGetPackageUpgradeInternalErrorString, installResult.ExtendedErrorCode.Message));
                                                    packageUpgradeFailedContentList.Add(installResult.ExtendedErrorCode is not null ? string.Format(PackageUpgradeFailedContent4String, "0x" + Convert.ToString(installResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageUpgradeFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageUpgradeFailedContentList = [];
                                                    packageUpgradeFailedContentList.Add(PackageUpgradeFailedContent1String);
                                                    packageUpgradeFailedContentList.Add(installResult.ExtendedErrorCode is not null ? string.Format(PackageUpgradeFailedContent2String, WinGetPackageUpgradeInvalidOptionsString) : string.Format(PackageUpgradeFailedContent3String, WinGetPackageUpgradeInvalidOptionsString, installResult.ExtendedErrorCode.Message));
                                                    packageUpgradeFailedContentList.Add(installResult.ExtendedErrorCode is not null ? string.Format(PackageUpgradeFailedContent4String, "0x" + Convert.ToString(installResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageUpgradeFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageUpgradeFailedContentList = [];
                                                    packageUpgradeFailedContentList.Add(PackageUpgradeFailedContent1String);
                                                    packageUpgradeFailedContentList.Add(installResult.ExtendedErrorCode is not null ? string.Format(PackageUpgradeFailedContent2String, WinGetPackageUpgradeDownloadErrorString) : string.Format(PackageUpgradeFailedContent3String, WinGetPackageUpgradeDownloadErrorString, installResult.ExtendedErrorCode.Message));
                                                    packageUpgradeFailedContentList.Add(installResult.ExtendedErrorCode is not null ? string.Format(PackageUpgradeFailedContent4String, "0x" + Convert.ToString(installResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageUpgradeFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageUpgradeFailedContentList = [];
                                                    packageUpgradeFailedContentList.Add(PackageUpgradeFailedContent1String);
                                                    packageUpgradeFailedContentList.Add(installResult.ExtendedErrorCode is not null ? string.Format(PackageUpgradeFailedContent2String, WinGetPackageUpgradeErrorString) : string.Format(PackageUpgradeFailedContent3String, WinGetPackageUpgradeErrorString, installResult.ExtendedErrorCode.Message));
                                                    packageUpgradeFailedContentList.Add(installResult.ExtendedErrorCode is not null ? string.Format(PackageUpgradeFailedContent4String, "0x" + Convert.ToString(installResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageUpgradeFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageUpgradeFailedContentList = [];
                                                    packageUpgradeFailedContentList.Add(PackageUpgradeFailedContent1String);
                                                    packageUpgradeFailedContentList.Add(installResult.ExtendedErrorCode is not null ? string.Format(PackageUpgradeFailedContent2String, WinGetPackageUpgradeManifestErrorString) : string.Format(PackageUpgradeFailedContent3String, WinGetPackageUpgradeManifestErrorString, installResult.ExtendedErrorCode.Message));
                                                    packageUpgradeFailedContentList.Add(installResult.ExtendedErrorCode is not null ? string.Format(PackageUpgradeFailedContent4String, "0x" + Convert.ToString(installResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageUpgradeFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageUpgradeFailedContentList = [];
                                                    packageUpgradeFailedContentList.Add(PackageUpgradeFailedContent1String);
                                                    packageUpgradeFailedContentList.Add(installResult.ExtendedErrorCode is not null ? string.Format(PackageUpgradeFailedContent2String, WinGetPackageUpgradeNoApplicableInstallersString) : string.Format(PackageUpgradeFailedContent3String, WinGetPackageUpgradeNoApplicableInstallersString, installResult.ExtendedErrorCode.Message));
                                                    packageUpgradeFailedContentList.Add(installResult.ExtendedErrorCode is not null ? string.Format(PackageUpgradeFailedContent4String, "0x" + Convert.ToString(installResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageUpgradeFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageUpgradeFailedContentList = [];
                                                    packageUpgradeFailedContentList.Add(PackageUpgradeFailedContent1String);
                                                    packageUpgradeFailedContentList.Add(installResult.ExtendedErrorCode is not null ? string.Format(PackageUpgradeFailedContent2String, WinGetPackageUpgradeNoApplicableUpgradeString) : string.Format(PackageUpgradeFailedContent3String, WinGetPackageUpgradeNoApplicableUpgradeString, installResult.ExtendedErrorCode.Message));
                                                    packageUpgradeFailedContentList.Add(installResult.ExtendedErrorCode is not null ? string.Format(PackageUpgradeFailedContent4String, "0x" + Convert.ToString(installResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageUpgradeFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                                string packageOperationFailedContent = await Task.Run(() =>
                                                {
                                                    List<string> packageUpgradeFailedContentList = [];
                                                    packageUpgradeFailedContentList.Add(PackageUpgradeFailedContent1String);
                                                    packageUpgradeFailedContentList.Add(installResult.ExtendedErrorCode is not null ? string.Format(PackageUpgradeFailedContent2String, WinGetPackageUpgradeAgreementsNotAcceptedString) : string.Format(PackageUpgradeFailedContent3String, WinGetPackageUpgradeAgreementsNotAcceptedString, installResult.ExtendedErrorCode.Message));
                                                    packageUpgradeFailedContentList.Add(installResult.ExtendedErrorCode is not null ? string.Format(PackageUpgradeFailedContent4String, "0x" + Convert.ToString(installResult.ExtendedErrorCode.HResult, 16).ToUpper()) : UnknownString);
                                                    return string.Join(Environment.NewLine, packageUpgradeFailedContentList);
                                                });

                                                PackageOperationLock.Enter();
                                                try
                                                {
                                                    packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                                    packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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
                                        packageOperation.PackageOperationFailedContent = UnknownString;
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
                                    string packageOperationFailedContent = await Task.Run(() =>
                                    {
                                        List<string> packageUpgradeFailedContentList = [];
                                        packageUpgradeFailedContentList.Add(PackageUpgradeFailedContent1String);
                                        packageUpgradeFailedContentList.Add(exception is not null ? string.Format(PackageUpgradeFailedContent2String, WinGetPackageUpgradeOtherErrorString) : string.Format(PackageUpgradeFailedContent3String, WinGetPackageUpgradeOtherErrorString, exception.Message));
                                        packageUpgradeFailedContentList.Add(exception is not null ? string.Format(PackageUpgradeFailedContent4String, "0x" + Convert.ToString(exception.HResult, 16).ToUpper()) : UnknownString);
                                        return string.Join(Environment.NewLine, packageUpgradeFailedContentList);
                                    });

                                    PackageOperationLock.Enter();
                                    try
                                    {
                                        packageOperation.PackageOperationResultKind = PackageOperationResultKind.Failed;
                                        packageOperation.PackageOperationFailedContent = packageOperationFailedContent;
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

                            ShowTaskCompletedNotification();
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 显示使用说明
        /// </summary>
        public async Task ShowUseInstructionAsync()
        {
            WinGetPaneKind = WinGetPaneKind.UseInstruction;
            await Task.Delay(300);
            if (!WinGetSplitView.IsPaneOpen)
            {
                WinGetSplitView.OpenPaneLength = 320;
                WinGetSplitView.IsPaneOpen = true;
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
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetPage), nameof(NavigateTo), 1, e);
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
                    appNotificationBuilder.AddText(WinGetPackageOperationCompletedString);
                    ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                });
            }
        }

        /// <summary>
        /// 检查 WinGet 浮出面板状态
        /// </summary>
        private Visibility CheckWinGetPaneKindState(WinGetPaneKind winGetPaneKind, WinGetPaneKind comparedWinGetPaneKind)
        {
            return Equals(winGetPaneKind, comparedWinGetPaneKind) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
