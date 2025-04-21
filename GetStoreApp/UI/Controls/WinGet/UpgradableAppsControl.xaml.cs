using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.WinGet;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.WinGet;
using GetStoreApp.UI.TeachingTips;
using GetStoreApp.Views.Pages;
using GetStoreApp.Views.Windows;
using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using Microsoft.Management.Deployment;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.Windows.AppNotifications.Builder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;
using Windows.System;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.UI.Controls.WinGet
{
    /// <summary>
    /// WinGet 程序包页面：可更新应用控件
    /// </summary>
    public sealed partial class UpgradableAppsControl : Grid, INotifyPropertyChanged
    {
        private readonly string Unknown = ResourceService.GetLocalized("WinGet/Unknown");
        private readonly string UpgradableAppsCountInfo = ResourceService.GetLocalized("WinGet/UpgradableAppsCountInfo");
        private readonly string UpgradableAppsEmptyDescription = ResourceService.GetLocalized("WinGet/UpgradableAppsEmptyDescription");
        private readonly string UpgradableAppsFailed = ResourceService.GetLocalized("WinGet/UpgradableAppsFailed");
        private readonly string UpgradableFindAppsFailed = ResourceService.GetLocalized("WinGet/UpgradableFindAppsFailed");
        private readonly string UpgradableCatalogReferenceFailed = ResourceService.GetLocalized("WinGet/UpgradableCatalogReferenceFailed");
        private readonly string UpgradableNotSelectSource = ResourceService.GetLocalized("WinGet/UpgradableNotSelectSource");
        private WinGetPage WinGetInstance;

        private bool _isIncrease = true;

        public bool IsIncrease
        {
            get { return _isIncrease; }

            set
            {
                if (!Equals(_isIncrease, value))
                {
                    _isIncrease = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsIncrease)));
                }
            }
        }

        private AppSortRuleKind _selectedAppSortRuleKind = AppSortRuleKind.DisplayName;

        public AppSortRuleKind SelectedAppSortRuleKind
        {
            get { return _selectedAppSortRuleKind; }

            set
            {
                if (!Equals(_selectedAppSortRuleKind, value))
                {
                    _selectedAppSortRuleKind = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedAppSortRuleKind)));
                }
            }
        }

        private UpgradableAppsResultKind _upgradableAppsResultKind = UpgradableAppsResultKind.NotCheckUpdate;

        public UpgradableAppsResultKind UpgradableAppsResultKind
        {
            get { return _upgradableAppsResultKind; }

            set
            {
                if (!Equals(_upgradableAppsResultKind, value))
                {
                    _upgradableAppsResultKind = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UpgradableAppsResultKind)));
                }
            }
        }

        private string _upgradableFailedContent;

        public string UpgradableFailedContent
        {
            get { return _upgradableFailedContent; }

            set
            {
                if (!Equals(_upgradableFailedContent, value))
                {
                    _upgradableFailedContent = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UpgradableFailedContent)));
                }
            }
        }

        private ObservableCollection<UpgradableAppsModel> UpgradableAppsCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public UpgradableAppsControl()
        {
            InitializeComponent();
        }

        #region 第一部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 复制升级命令
        /// </summary>
        private async void OnCopyUpgradeTextExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string appId && !string.IsNullOrEmpty(appId))
            {
                string copyContent = string.Format("winget upgrade {0}", appId);
                bool copyResult = CopyPasteHelper.CopyTextToClipBoard(copyContent);

                await MainWindow.Current.ShowNotificationAsync(new MainDataCopyTip(DataCopyKind.WinGetUpgradeInstall, copyResult));
            }
        }

        /// <summary>
        /// 使用命令安装
        /// </summary>
        private void OnInstallWithCmdExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string appId && !string.IsNullOrEmpty(appId))
            {
                Task.Run(() =>
                {
                    Shell32Library.ShellExecute(IntPtr.Zero, "open", "winget.exe", string.Format("install {0}", appId), null, WindowShowStyle.SW_SHOWNORMAL);
                });
            }
        }

        /// <summary>
        /// 应用升级
        /// </summary>
        private void OnUpdateExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is UpgradableAppsModel upgradableApps)
            {
                // 禁用当前应用的可更新状态
                foreach (UpgradableAppsModel upgradableAppsItem in UpgradableAppsCollection)
                {
                    if (upgradableAppsItem.AppID == upgradableApps.AppID)
                    {
                        upgradableAppsItem.IsUpgrading = true;
                        break;
                    }
                }

                // 添加任务
                WinGetInstance.InstallingAppsLock.Enter();
                try
                {
                    WinGetInstance.InstallingAppsCollection.Add(new InstallingAppsModel()
                    {
                        AppID = upgradableApps.AppID,
                        AppName = upgradableApps.AppName,
                        DownloadProgress = 0,
                        InstallProgressState = PackageInstallProgressState.Queued,
                        DownloadedFileSize = FileSizeHelper.ConvertFileSizeToString(0),
                        TotalFileSize = FileSizeHelper.ConvertFileSizeToString(0),
                        InstallingAppsProgress = null
                    });
                }
                catch (Exception) { }
                finally
                {
                    WinGetInstance.InstallingAppsLock.Exit();
                }

                Task.Run(() =>
                {
                    try
                    {
                        // 第一部分：添加安装任务
                        PackageManager packageManager = new();
                        IAsyncOperationWithProgress<InstallResult, InstallProgress> installPackageWithProgress = packageManager.UpgradePackageAsync(upgradableApps.CatalogPackage, new()
                        {
                            PackageInstallMode = Enum.TryParse(WinGetConfigService.WinGetInstallMode.Key, out PackageInstallMode packageInstallMode) ? packageInstallMode : PackageInstallMode.Default,
                            PackageInstallScope = PackageInstallScope.Any
                        });

                        WinGetInstance.InstallingAppsLock.Enter();

                        try
                        {
                            foreach (InstallingAppsModel installingAppsItem in WinGetInstance.InstallingAppsCollection)
                            {
                                if (installingAppsItem.AppID.Equals(upgradableApps.AppID) && installingAppsItem.InstallingAppsProgress is null)
                                {
                                    installingAppsItem.InstallingAppsProgress = installPackageWithProgress;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                        }
                        finally
                        {
                            WinGetInstance.InstallingAppsLock.Exit();
                        }

                        // 第二部分：更新升级进度
                        installPackageWithProgress.Progress = (result, progress) => OnInstallPackageProgressing(result, progress, upgradableApps);

                        // 第三部分：安装已完成
                        installPackageWithProgress.Completed = (result, status) => OnInstallPackageCompleted(result, status, upgradableApps);
                    }
                    // 其他异常
                    catch (Exception e)
                    {
                        // 启用当前应用的可更新状态
                        foreach (UpgradableAppsModel upgradableAppsItem in UpgradableAppsCollection)
                        {
                            if (upgradableAppsItem.AppID == upgradableApps.AppID)
                            {
                                upgradableAppsItem.IsUpgrading = false;
                                break;
                            }
                        }

                        LogService.WriteLog(LoggingLevel.Error, "App upgrading failed.", e);
                    }
                });
            }
        }

        #endregion 第一部分：XamlUICommand 命令调用时挂载的事件

        #region 第二部分：可更新应用控件——挂载的事件

        /// <summary>
        /// 根据排序方式对列表进行排序
        /// </summary>
        private async void OnSortWayClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is string increase && UpgradableAppsResultKind is UpgradableAppsResultKind.Successfully)
            {
                IsIncrease = Convert.ToBoolean(increase);
                UpgradableAppsResultKind = UpgradableAppsResultKind.Querying;
                List<UpgradableAppsModel> upgradableAppsList = [.. UpgradableAppsCollection];
                UpgradableAppsCollection.Clear();
                if (SelectedAppSortRuleKind is AppSortRuleKind.DisplayName)
                {
                    if (IsIncrease)
                    {
                        upgradableAppsList.Sort((item1, item2) => item1.AppName.CompareTo(item2.AppName));
                    }
                    else
                    {
                        upgradableAppsList.Sort((item1, item2) => item2.AppName.CompareTo(item1.AppName));
                    }
                }
                else
                {
                    if (IsIncrease)
                    {
                        upgradableAppsList.Sort((item1, item2) => item1.AppPublisher.CompareTo(item2.AppPublisher));
                    }
                    else
                    {
                        upgradableAppsList.Sort((item1, item2) => item2.AppPublisher.CompareTo(item1.AppPublisher));
                    }
                }
                await Task.Delay(500);
                foreach (UpgradableAppsModel upgradableAppsItem in upgradableAppsList)
                {
                    UpgradableAppsCollection.Add(upgradableAppsItem);
                }
                UpgradableAppsResultKind = UpgradableAppsResultKind.Successfully;
            }
        }

        /// <summary>
        /// 根据排序规则对列表进行排序
        /// </summary>
        private async void OnSortRuleClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is AppSortRuleKind appSortRuleKind && UpgradableAppsResultKind is UpgradableAppsResultKind.Successfully)
            {
                SelectedAppSortRuleKind = appSortRuleKind;
                UpgradableAppsResultKind = UpgradableAppsResultKind.Querying;
                List<UpgradableAppsModel> upgradableAppsList = [.. UpgradableAppsCollection];
                UpgradableAppsCollection.Clear();
                if (SelectedAppSortRuleKind is AppSortRuleKind.DisplayName)
                {
                    if (IsIncrease)
                    {
                        upgradableAppsList.Sort((item1, item2) => item1.AppName.CompareTo(item2.AppName));
                    }
                    else
                    {
                        upgradableAppsList.Sort((item1, item2) => item2.AppName.CompareTo(item1.AppName));
                    }
                }
                else
                {
                    if (IsIncrease)
                    {
                        upgradableAppsList.Sort((item1, item2) => item1.AppPublisher.CompareTo(item2.AppPublisher));
                    }
                    else
                    {
                        upgradableAppsList.Sort((item1, item2) => item2.AppPublisher.CompareTo(item1.AppPublisher));
                    }
                }
                await Task.Delay(500);
                foreach (UpgradableAppsModel upgradableAppsItem in upgradableAppsList)
                {
                    UpgradableAppsCollection.Add(upgradableAppsItem);
                }
                UpgradableAppsResultKind = UpgradableAppsResultKind.Successfully;
            }
        }

        /// <summary>
        /// 打开临时下载目录
        /// </summary>
        private async void OnOpenTempFolderClicked(object sender, RoutedEventArgs args)
        {
            string wingetTempPath = Path.Combine(Path.GetTempPath(), "WinGet");
            if (Directory.Exists(wingetTempPath))
            {
                await Launcher.LaunchFolderPathAsync(wingetTempPath);
            }
            else
            {
                await Launcher.LaunchFolderPathAsync(Path.GetTempPath());
            }
        }

        /// <summary>
        /// 检查更新
        /// </summary>
        private async void OnCheckUpdateClicked(object sender, RoutedEventArgs args)
        {
            UpgradableAppsResultKind = UpgradableAppsResultKind.Querying;
            UpgradableAppsCollection.Clear();

            PackageCatalogReference packageCatalogReference = await Task.Run(() =>
            {
                PackageManager packageManager = new();
                return packageManager.GetLocalPackageCatalog(LocalPackageCatalog.InstalledPackages);
            });

            if (packageCatalogReference is not null)
            {
                (ConnectResult connectResult, FindPackagesResult findPackagesResult, List<UpgradableAppsModel> upgradableAppsList) = await Task.Run(() =>
                {
                    return UpgradableAppsAsync(packageCatalogReference);
                });

                if (connectResult.Status is ConnectResultStatus.Ok)
                {
                    if (findPackagesResult.Status is FindPackagesResultStatus.Ok)
                    {
                        if (upgradableAppsList.Count is 0)
                        {
                            UpgradableAppsResultKind = UpgradableAppsResultKind.Failed;
                            UpgradableFailedContent = UpgradableAppsEmptyDescription;
                        }
                        else
                        {
                            foreach (UpgradableAppsModel upgradableAppsItem in upgradableAppsList)
                            {
                                UpgradableAppsCollection.Add(upgradableAppsItem);
                            }

                            UpgradableAppsResultKind = UpgradableAppsResultKind.Successfully;
                        }
                    }
                    else
                    {
                        UpgradableAppsResultKind = UpgradableAppsResultKind.Failed;
                        UpgradableFailedContent = string.Format(UpgradableAppsFailed, UpgradableFindAppsFailed, findPackagesResult.ExtendedErrorCode is not null ? findPackagesResult.ExtendedErrorCode.HResult : Unknown);
                    }
                }
                else
                {
                    UpgradableAppsResultKind = UpgradableAppsResultKind.Failed;
                    UpgradableFailedContent = string.Format(UpgradableAppsFailed, UpgradableCatalogReferenceFailed, findPackagesResult.ExtendedErrorCode is not null ? findPackagesResult.ExtendedErrorCode.HResult : Unknown);
                }
            }
            else
            {
                UpgradableAppsResultKind = UpgradableAppsResultKind.Failed;
                UpgradableFailedContent = UpgradableNotSelectSource;
            }
        }

        /// <summary>
        /// 配置 WinGet 数据源
        /// </summary>
        private void OnDataSourceSettingsClicked(object sender, RoutedEventArgs args)
        {
            MainWindow.Current.NavigateTo(typeof(SettingsPage), AppNaviagtionArgs.WinGetDataSource);
        }

        #endregion 第二部分：可更新应用控件——挂载的事件

        #region 第三部分：可更新应用控件——自定义事件

        /// <summary>
        /// 应用安装状态发生变化时触发的事件
        /// </summary>
        private void OnInstallPackageProgressing(IAsyncOperationWithProgress<InstallResult, InstallProgress> result, InstallProgress progress, UpgradableAppsModel upgradableApps)
        {
            switch (progress.State)
            {
                // 处于等待中状态
                case PackageInstallProgressState.Queued:
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            WinGetInstance.InstallingAppsLock.Enter();
                            try
                            {
                                foreach (InstallingAppsModel installingItem in WinGetInstance.InstallingAppsCollection)
                                {
                                    if (installingItem.AppID.Equals(upgradableApps.AppID))
                                    {
                                        installingItem.InstallProgressState = PackageInstallProgressState.Queued;
                                        break;
                                    }
                                }
                            }
                            catch (Exception) { }
                            finally
                            {
                                WinGetInstance.InstallingAppsLock.Exit();
                            }
                        });

                        break;
                    }
                // 处于下载中状态
                case PackageInstallProgressState.Downloading:
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            WinGetInstance.InstallingAppsLock.Enter();
                            try
                            {
                                foreach (InstallingAppsModel installingItem in WinGetInstance.InstallingAppsCollection)
                                {
                                    if (installingItem.AppID.Equals(upgradableApps.AppID))
                                    {
                                        installingItem.InstallProgressState = PackageInstallProgressState.Downloading;
                                        installingItem.DownloadProgress = Math.Round(progress.DownloadProgress * 100, 2); installingItem.DownloadedFileSize = Convert.ToString(FileSizeHelper.ConvertFileSizeToString(progress.BytesDownloaded));
                                        installingItem.TotalFileSize = Convert.ToString(FileSizeHelper.ConvertFileSizeToString(progress.BytesRequired));
                                        break;
                                    }
                                }
                            }
                            catch (Exception) { }
                            finally
                            {
                                WinGetInstance.InstallingAppsLock.Exit();
                            }
                        });

                        break;
                    }
                // 处于安装中状态
                case PackageInstallProgressState.Installing:
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            foreach (InstallingAppsModel installingItem in WinGetInstance.InstallingAppsCollection)
                            {
                                if (installingItem.AppID.Equals(upgradableApps.AppID))
                                {
                                    installingItem.InstallProgressState = PackageInstallProgressState.Installing;
                                    installingItem.DownloadProgress = 100;
                                    break;
                                }
                            }
                        });

                        break;
                    }
                // 挂起状态
                case PackageInstallProgressState.PostInstall:
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            WinGetInstance.InstallingAppsLock.Enter();
                            try
                            {
                                foreach (InstallingAppsModel installingItem in WinGetInstance.InstallingAppsCollection)
                                {
                                    if (installingItem.AppID.Equals(upgradableApps.AppID))
                                    {
                                        installingItem.InstallProgressState = PackageInstallProgressState.PostInstall;
                                        break;
                                    }
                                }
                            }
                            catch (Exception) { }
                            finally
                            {
                                WinGetInstance.InstallingAppsLock.Exit();
                            }
                        });

                        break;
                    }
                // 处于安装完成状态
                case PackageInstallProgressState.Finished:
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            WinGetInstance.InstallingAppsLock.Enter();
                            try
                            {
                                foreach (InstallingAppsModel installingItem in WinGetInstance.InstallingAppsCollection)
                                {
                                    if (installingItem.AppID.Equals(upgradableApps.AppID))
                                    {
                                        installingItem.InstallProgressState = PackageInstallProgressState.Finished;
                                        installingItem.DownloadProgress = 100;
                                        break;
                                    }
                                }
                            }
                            catch (Exception) { }
                            finally
                            {
                                WinGetInstance.InstallingAppsLock.Exit();
                            }
                        });

                        break;
                    }
            }
        }

        /// <summary>
        /// 应用安装完成时时触发的事件
        /// </summary>
        private void OnInstallPackageCompleted(IAsyncOperationWithProgress<InstallResult, InstallProgress> result, AsyncStatus status, UpgradableAppsModel upgradableApps)
        {
            // 安装过程已顺利完成
            if (status is AsyncStatus.Completed)
            {
                InstallResult installResult = result.GetResults();

                // 应用安装成功
                if (installResult.Status is InstallResultStatus.Ok)
                {
                    // 显示 WinGet 应用更新成功通知
                    AppNotificationBuilder appNotificationBuilder = new();
                    appNotificationBuilder.AddArgument("action", "OpenApp");
                    appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/WinGetUpgradeSuccessfully"), upgradableApps.AppName));
                    ToastNotificationService.Show(appNotificationBuilder.BuildNotification());

                    // 检测是否需要重启设备完成应用的更新，如果是，询问用户是否需要重启设备
                    if (installResult.RebootRequired)
                    {
                        DispatcherQueue.TryEnqueue(async () =>
                        {
                            ContentDialogResult contentDialogResult = await MainWindow.Current.ShowDialogAsync(new RebootDialog(WinGetOptionKind.UpgradeInstall, upgradableApps.AppName));

                            if (contentDialogResult is ContentDialogResult.Primary)
                            {
                                await Task.Run(() =>
                                {
                                    ShutdownHelper.Restart(ResourceService.GetLocalized("WinGet/RestartPC"), TimeSpan.FromSeconds(120));
                                });
                            }
                        });
                    }
                }
                else
                {
                    // 显示 WinGet 应用升级失败通知
                    AppNotificationBuilder appNotificationBuilder = new();
                    appNotificationBuilder.AddArgument("action", "OpenApp");
                    appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/WinGetUpgradeFailed1"), upgradableApps.AppName));
                    appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/WinGetUpgradeFailed2"));
                    appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/WinGetUpgradeFailed3"));
                    AppNotificationButton installWithCommandButton = new(ResourceService.GetLocalized("Notification/InstallWithCommand"));
                    installWithCommandButton.Arguments.Add("action", string.Format("InstallWithCommand:{0}", "TestAppID"));
                    AppNotificationButton openDownloadFolderButton = new(ResourceService.GetLocalized("Notification/OpenDownloadFolder"));
                    openDownloadFolderButton.Arguments.Add("action", "OpenDownloadFolder");
                    appNotificationBuilder.AddButton(installWithCommandButton);
                    appNotificationBuilder.AddButton(openDownloadFolderButton);
                    ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                }

                DispatcherQueue.TryEnqueue(() =>
                {
                    // 应用更新失败，将当前任务状态修改为可更新状态
                    foreach (UpgradableAppsModel upgradableAppsItem in UpgradableAppsCollection)
                    {
                        if (upgradableAppsItem.AppID.Equals(upgradableApps.AppID))
                        {
                            upgradableAppsItem.IsUpgrading = false;
                            break;
                        }
                    }

                    // 完成任务后从任务管理中删除任务
                    WinGetInstance.InstallingAppsLock.Enter();
                    try
                    {
                        foreach (InstallingAppsModel installingAppsItem in WinGetInstance.InstallingAppsCollection)
                        {
                            if (installingAppsItem.AppID.Equals(upgradableApps.AppID))
                            {
                                WinGetInstance.InstallingAppsCollection.Remove(installingAppsItem);
                                break;
                            }
                        }
                    }
                    catch (Exception) { }
                    finally
                    {
                        WinGetInstance.InstallingAppsLock.Exit();
                    }
                });
            }
            // 安装过程已被用户取消
            else if (status is AsyncStatus.Canceled)
            {
                LogService.WriteLog(LoggingLevel.Information, "App upgrading operation canceled.", new Exception());

                DispatcherQueue.TryEnqueue(() =>
                {
                    // 应用安装失败，将当前任务状态修改为可安装状态
                    foreach (UpgradableAppsModel upgradableAppsItem in UpgradableAppsCollection)
                    {
                        if (upgradableAppsItem.AppID.Equals(upgradableApps.AppID))
                        {
                            upgradableAppsItem.IsUpgrading = false;
                            break;
                        }
                    }

                    // 完成任务后从任务管理中删除任务
                    WinGetInstance.InstallingAppsLock.Enter();
                    try
                    {
                        foreach (InstallingAppsModel installingAppsItem in WinGetInstance.InstallingAppsCollection)
                        {
                            if (installingAppsItem.AppID.Equals(upgradableApps.AppID))
                            {
                                WinGetInstance.InstallingAppsCollection.Remove(installingAppsItem);
                                break;
                            }
                        }
                    }
                    catch (Exception) { }
                    finally
                    {
                        WinGetInstance.InstallingAppsLock.Exit();
                    }
                });
            }
            // 安装过程发生错误
            else if (status is AsyncStatus.Error)
            {
                LogService.WriteLog(LoggingLevel.Error, "App installing failed.", result.ErrorCode);

                DispatcherQueue.TryEnqueue(() =>
                {
                    // 应用更新失败，将当前任务状态修改为可更新状态
                    foreach (UpgradableAppsModel upgradableAppsItem in UpgradableAppsCollection)
                    {
                        if (upgradableAppsItem.AppID.Equals(upgradableApps.AppID))
                        {
                            upgradableAppsItem.IsUpgrading = false;
                            break;
                        }
                    }

                    // 完成任务后从任务管理中删除任务
                    WinGetInstance.InstallingAppsLock.Enter();
                    try
                    {
                        foreach (InstallingAppsModel installingAppsItem in WinGetInstance.InstallingAppsCollection)
                        {
                            if (installingAppsItem.AppID.Equals(upgradableApps.AppID))
                            {
                                WinGetInstance.InstallingAppsCollection.Remove(installingAppsItem);
                                break;
                            }
                        }
                    }
                    catch (Exception) { }
                    finally
                    {
                        WinGetInstance.InstallingAppsLock.Exit();
                    }
                });

                // 显示 WinGet 应用安装失败通知
                AppNotificationBuilder appNotificationBuilder = new();
                appNotificationBuilder.AddArgument("action", "OpenApp");
                appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/WinGetUpgradeFailed1"), upgradableApps.AppName));
                appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/WinGetUpgradeFailed2"));
                appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/WinGetUpgradeFailed3"));
                AppNotificationButton installWithCommandButton = new(ResourceService.GetLocalized("Notification/InstallWithCommand"));
                installWithCommandButton.Arguments.Add("action", string.Format("InstallWithCommand:{0}", "TestAppID"));
                AppNotificationButton openDownloadFolderButton = new(ResourceService.GetLocalized("Notification/OpenDownloadFolder"));
                openDownloadFolderButton.Arguments.Add("action", "OpenDownloadFolder");
                appNotificationBuilder.AddButton(installWithCommandButton);
                appNotificationBuilder.AddButton(openDownloadFolderButton);
                ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
            }

            result.Close();
        }

        #endregion 第三部分：可更新应用控件——自定义事件

        public void InitializeWingetInstance(WinGetPage wingetInstance)
        {
            WinGetInstance = wingetInstance;
        }

        /// <summary>
        /// 获取可更新应用
        /// </summary>
        private async Task<(ConnectResult, FindPackagesResult, List<UpgradableAppsModel>)> UpgradableAppsAsync(PackageCatalogReference packageCatalogReference)
        {
            (ConnectResult connectResult, FindPackagesResult findPackagesResult, List<UpgradableAppsModel> upgradableAppsList) upgradableAppsResult = ValueTuple.Create<ConnectResult, FindPackagesResult, List<UpgradableAppsModel>>(null, null, null);

            try
            {
                ConnectResult connectResult = await packageCatalogReference.ConnectAsync();
                upgradableAppsResult.connectResult = connectResult;

                if (connectResult is not null && connectResult.Status is ConnectResultStatus.Ok)
                {
                    FindPackagesOptions findPackagesOptions = new();
                    FindPackagesResult findPackagesResult = await connectResult.PackageCatalog.FindPackagesAsync(findPackagesOptions);
                    upgradableAppsResult.findPackagesResult = findPackagesResult;

                    if (findPackagesResult is not null && findPackagesResult.Status is FindPackagesResultStatus.Ok)
                    {
                        List<UpgradableAppsModel> upgradableAppsList = [];

                        for (int index = 0; index < findPackagesResult.Matches.Count; index++)
                        {
                            MatchResult matchItem = findPackagesResult.Matches[index];

                            if (matchItem.CatalogPackage is not null && matchItem.CatalogPackage.IsUpdateAvailable)
                            {
                                bool isUpgrading = false;
                                WinGetInstance.InstallingAppsLock.Enter();
                                try
                                {
                                    foreach (InstallingAppsModel installingAppsItem in WinGetInstance.InstallingAppsCollection)
                                    {
                                        if (matchItem.CatalogPackage.DefaultInstallVersion.Id.Equals(installingAppsItem.AppID))
                                        {
                                            isUpgrading = true;
                                            break;
                                        }
                                    }
                                }
                                catch (Exception) { }
                                finally
                                {
                                    WinGetInstance.InstallingAppsLock.Exit();
                                }

                                upgradableAppsList.Add(new UpgradableAppsModel()
                                {
                                    AppID = matchItem.CatalogPackage.DefaultInstallVersion.Id,
                                    AppName = string.IsNullOrEmpty(matchItem.CatalogPackage.DefaultInstallVersion.DisplayName) ? Unknown : matchItem.CatalogPackage.DefaultInstallVersion.DisplayName,
                                    AppPublisher = string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.Publisher) ? Unknown : matchItem.CatalogPackage.InstalledVersion.Publisher,
                                    AppCurrentVersion = string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.Version) ? Unknown : matchItem.CatalogPackage.InstalledVersion.Version,
                                    AppNewestVersion = string.IsNullOrEmpty(matchItem.CatalogPackage.DefaultInstallVersion.Version) ? Unknown : matchItem.CatalogPackage.DefaultInstallVersion.Version,
                                    IsUpgrading = isUpgrading,
                                    CatalogPackage = matchItem.CatalogPackage,
                                });
                            }
                        }

                        if (SelectedAppSortRuleKind is AppSortRuleKind.DisplayName)
                        {
                            if (IsIncrease)
                            {
                                upgradableAppsList.Sort((item1, item2) => item1.AppName.CompareTo(item2.AppName));
                            }
                            else
                            {
                                upgradableAppsList.Sort((item1, item2) => item2.AppName.CompareTo(item1.AppName));
                            }
                        }
                        else
                        {
                            if (IsIncrease)
                            {
                                upgradableAppsList.Sort((item1, item2) => item1.AppPublisher.CompareTo(item2.AppPublisher));
                            }
                            else
                            {
                                upgradableAppsList.Sort((item1, item2) => item2.AppPublisher.CompareTo(item1.AppPublisher));
                            }
                        }

                        upgradableAppsResult.upgradableAppsList = upgradableAppsList;
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Update winget app failed", e);
            }

            return upgradableAppsResult;
        }

        /// <summary>
        /// 获取可更新应用是否成功
        /// </summary>
        public Visibility GetUpgradableAppsSuccessfullyState(UpgradableAppsResultKind upgradableAppsResultKind, bool isSuccessfully)
        {
            return isSuccessfully ? upgradableAppsResultKind.Equals(UpgradableAppsResultKind.Successfully) ? Visibility.Visible : Visibility.Collapsed : !upgradableAppsResultKind.Equals(UpgradableAppsResultKind.Successfully) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查可更新应用是否成功
        /// </summary>
        public Visibility CheckUpgradableAppsState(UpgradableAppsResultKind upgradableAppsResultKind, UpgradableAppsResultKind comparedSearchAppsResultKind)
        {
            return upgradableAppsResultKind.Equals(comparedSearchAppsResultKind) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取是否正在检查更新中
        /// </summary>

        public bool GetIsCheckingUpdate(UpgradableAppsResultKind upgradableAppsResultKind)
        {
            return !upgradableAppsResultKind.Equals(UpgradableAppsResultKind.Querying);
        }
    }
}
