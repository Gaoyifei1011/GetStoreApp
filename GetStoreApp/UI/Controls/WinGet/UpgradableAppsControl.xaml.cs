using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Controls.Extensions;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.WinGet;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.WinGet;
using GetStoreApp.UI.TeachingTips;
using GetStoreApp.Views.Pages;
using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using Microsoft.Management.Deployment;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices.Marshalling;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;
using Windows.System;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.UI.Controls.WinGet
{
    /// <summary>
    /// WinGet 程序包页面：可升级应用控件
    /// </summary>
    public sealed partial class UpgradableAppsControl : Grid, INotifyPropertyChanged
    {
        private readonly PackageManager UpgradableAppsManager;
        private AutoResetEvent autoResetEvent;
        private WinGetPage WinGetInstance;

        private string UpgradableAppsCountInfo { get; } = ResourceService.GetLocalized("WinGet/UpgradableAppsCountInfo");

        private bool _isLoadedCompleted;

        public bool IsLoadedCompleted
        {
            get { return _isLoadedCompleted; }

            set
            {
                if (!Equals(_isLoadedCompleted, value))
                {
                    _isLoadedCompleted = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoadedCompleted)));
                }
            }
        }

        private bool _isUpgradableAppsEmpty;

        public bool IsUpgradableAppsEmpty
        {
            get { return _isUpgradableAppsEmpty; }

            set
            {
                if (!Equals(_isUpgradableAppsEmpty, value))
                {
                    _isUpgradableAppsEmpty = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsUpgradableAppsEmpty)));
                }
            }
        }

        private List<MatchResult> MatchResultList { get; } = [];

        private ObservableCollection<UpgradableAppsModel> UpgradableAppsCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public UpgradableAppsControl()
        {
            InitializeComponent();

            try
            {
                UpgradableAppsManager = WinGetService.CreatePackageManager();
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Upgradable apps information initialized failed.", e);
                return;
            }
            GetUpgradableApps();
            InitializeData();
        }

        #region 第一部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 复制升级命令
        /// </summary>
        private async void OnCopyUpgradeTextExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string appId && !string.IsNullOrEmpty(appId))
            {
                string copyContent = string.Format("winget install {0}", appId);
                bool copyResult = CopyPasteHelper.CopyTextToClipBoard(copyContent);

                await TeachingTipHelper.ShowAsync(new DataCopyTip(DataCopyKind.WinGetUpgradeInstall, copyResult));
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
                Task.Run(async () =>
                {
                    AutoResetEvent autoResetEvent = new(false);
                    try
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            // 禁用当前应用的可升级状态
                            foreach (UpgradableAppsModel upgradableAppsItem in UpgradableAppsCollection)
                            {
                                if (upgradableAppsItem.AppID == upgradableApps.AppID)
                                {
                                    upgradableAppsItem.IsUpgrading = true;
                                    break;
                                }
                            }
                        });

                        InstallOptions installOptions = WinGetService.CreateInstallOptions();

                        installOptions.PackageInstallMode = Enum.TryParse(WinGetConfigService.WinGetInstallMode.Key, out PackageInstallMode packageInstallMode) ? packageInstallMode : PackageInstallMode.Default;
                        installOptions.PackageInstallScope = PackageInstallScope.Any;

                        // 更新升级进度
                        Progress<InstallProgress> progressCallBack = new((installProgress) =>
                        {
                            switch (installProgress.State)
                            {
                                // 处于等待中状态
                                case PackageInstallProgressState.Queued:
                                    {
                                        DispatcherQueue.TryEnqueue(() =>
                                        {
                                            foreach (InstallingAppsModel installingItem in WinGetInstance.InstallingAppsCollection)
                                            {
                                                if (installingItem.AppID == upgradableApps.AppID)
                                                {
                                                    installingItem.InstallProgressState = PackageInstallProgressState.Queued;
                                                    break;
                                                }
                                            }
                                        });

                                        break;
                                    }
                                // 处于下载中状态
                                case PackageInstallProgressState.Downloading:
                                    {
                                        DispatcherQueue.TryEnqueue(() =>
                                        {
                                            foreach (InstallingAppsModel installingItem in WinGetInstance.InstallingAppsCollection)
                                            {
                                                if (installingItem.AppID == upgradableApps.AppID)
                                                {
                                                    installingItem.InstallProgressState = PackageInstallProgressState.Downloading;
                                                    installingItem.DownloadProgress = Math.Round(installProgress.DownloadProgress * 100, 2);
                                                    installingItem.DownloadedFileSize = Convert.ToString(FileSizeHelper.ConvertFileSizeToString(installProgress.BytesDownloaded));
                                                    installingItem.TotalFileSize = Convert.ToString(FileSizeHelper.ConvertFileSizeToString(installProgress.BytesRequired));
                                                    break;
                                                }
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
                                                if (installingItem.AppID == upgradableApps.AppID)
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
                                            foreach (InstallingAppsModel installingItem in WinGetInstance.InstallingAppsCollection)
                                            {
                                                if (installingItem.AppID == upgradableApps.AppID)
                                                {
                                                    installingItem.InstallProgressState = PackageInstallProgressState.PostInstall;
                                                    break;
                                                }
                                            }
                                        });

                                        break;
                                    }
                                // 处于安装完成状态
                                case PackageInstallProgressState.Finished:
                                    {
                                        DispatcherQueue.TryEnqueue(() =>
                                        {
                                            foreach (InstallingAppsModel installingItem in WinGetInstance.InstallingAppsCollection)
                                            {
                                                if (installingItem.AppID == upgradableApps.AppID)
                                                {
                                                    installingItem.InstallProgressState = PackageInstallProgressState.Finished;
                                                    installingItem.DownloadProgress = 100;
                                                    break;
                                                }
                                            }
                                        });

                                        break;
                                    }
                            }
                        });

                        // 任务取消执行操作
                        CancellationTokenSource upgradeTokenSource = new();

                        // 添加任务
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            WinGetInstance.InstallingAppsCollection.Add(new InstallingAppsModel()
                            {
                                AppID = upgradableApps.AppID,
                                AppName = upgradableApps.AppName,
                                DownloadProgress = 0,
                                InstallProgressState = PackageInstallProgressState.Queued,
                                DownloadedFileSize = FileSizeHelper.ConvertFileSizeToString(0),
                                TotalFileSize = FileSizeHelper.ConvertFileSizeToString(0)
                            });
                        });

                        WinGetInstance.installStateLock.Enter();

                        try
                        {
                            WinGetInstance.InstallingStateDict.Add(upgradableApps.AppID, upgradeTokenSource);
                        }
                        catch (Exception e)
                        {
                            ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                        }
                        finally
                        {
                            WinGetInstance.installStateLock.Exit();
                        }

                        InstallResult installResult = await UpgradableAppsManager.UpgradePackageAsync(MatchResultList.Find(item => item.CatalogPackage.DefaultInstallVersion.Id == upgradableApps.AppID).CatalogPackage, installOptions).AsTask(upgradeTokenSource.Token, progressCallBack);

                        // 获取升级完成后的结果信息
                        // 升级完成，从列表中删除该应用
                        if (installResult.Status is InstallResultStatus.Ok)
                        {
                            ToastNotificationService.Show(NotificationKind.WinGetUpgradeSuccessfully, upgradableApps.AppName);

                            // 检测是否需要重启设备完成应用的卸载，如果是，询问用户是否需要重启设备
                            if (installResult.RebootRequired)
                            {
                                ContentDialogResult result = ContentDialogResult.None;
                                DispatcherQueue.TryEnqueue(async () =>
                                {
                                    result = await ContentDialogHelper.ShowAsync(new RebootDialog(WinGetOptionKind.UpgradeInstall, upgradableApps.AppName), this);
                                    autoResetEvent.Set();
                                });

                                if (result is ContentDialogResult.Primary)
                                {
                                    Shell32Library.ShellExecute(IntPtr.Zero, "open", Path.Combine(InfoHelper.SystemDataPath.Windows, "System32", "Shutdown.exe"), "-r -t 120", null, WindowShowStyle.SW_SHOWNORMAL);
                                }
                            }

                            WinGetInstance.installStateLock.Enter();

                            try
                            {
                                WinGetInstance.InstallingStateDict.Remove(upgradableApps.AppID);
                            }
                            catch (Exception e)
                            {
                                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                            }
                            finally
                            {
                                WinGetInstance.installStateLock.Exit();
                            }

                            DispatcherQueue.TryEnqueue(() =>
                            {
                                // 完成任务后从任务管理中删除任务
                                foreach (InstallingAppsModel installingAppsItem in WinGetInstance.InstallingAppsCollection)
                                {
                                    if (installingAppsItem.AppID == upgradableApps.AppID)
                                    {
                                        WinGetInstance.InstallingAppsCollection.Remove(installingAppsItem);
                                        break;
                                    }
                                }

                                // 从升级列表中移除已升级完成的任务
                                foreach (UpgradableAppsModel upgradableAppsItem in UpgradableAppsCollection)
                                {
                                    if (upgradableAppsItem.AppID == upgradableApps.AppID)
                                    {
                                        UpgradableAppsCollection.Remove(upgradableAppsItem);
                                        IsUpgradableAppsEmpty = UpgradableAppsCollection.Count is 0;
                                        break;
                                    }
                                }
                            });
                        }
                        else
                        {
                            WinGetInstance.installStateLock.Enter();

                            try
                            {
                                WinGetInstance.InstallingStateDict.Remove(upgradableApps.AppID);
                            }
                            catch (Exception e)
                            {
                                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                            }
                            finally
                            {
                                WinGetInstance.installStateLock.Exit();
                            }

                            DispatcherQueue.TryEnqueue(() =>
                            {
                                // 应用升级失败，将当前任务状态修改为可升级状态
                                foreach (UpgradableAppsModel upgradableAppsItem in UpgradableAppsCollection)
                                {
                                    if (upgradableAppsItem.AppID == upgradableApps.AppID)
                                    {
                                        upgradableAppsItem.IsUpgrading = false;
                                    }
                                }

                                // 应用升级失败，将当前任务状态修改为可升级状态
                                foreach (InstallingAppsModel installingAppsItem in WinGetInstance.InstallingAppsCollection)
                                {
                                    if (installingAppsItem.AppID == upgradableApps.AppID)
                                    {
                                        WinGetInstance.InstallingAppsCollection.Remove(installingAppsItem);
                                        break;
                                    }
                                }
                            });

                            ToastNotificationService.Show(NotificationKind.WinGetUpgradeFailed, upgradableApps.AppName, upgradableApps.AppID);
                        }
                    }
                    // 操作被用户所取消异常
                    catch (OperationCanceledException e)
                    {
                        LogService.WriteLog(LoggingLevel.Information, "App installing operation canceled.", e);

                        WinGetInstance.installStateLock.Enter();

                        try
                        {
                            WinGetInstance.InstallingStateDict.Remove(upgradableApps.AppID);
                        }
                        catch (Exception exception)
                        {
                            ExceptionAsVoidMarshaller.ConvertToUnmanaged(exception);
                        }
                        finally
                        {
                            WinGetInstance.installStateLock.Exit();
                        }

                        DispatcherQueue.TryEnqueue(() =>
                        {
                            // 应用升级失败，将当前任务状态修改为可升级状态
                            foreach (UpgradableAppsModel upgradableAppsItem in UpgradableAppsCollection)
                            {
                                if (upgradableAppsItem.AppID == upgradableApps.AppID)
                                {
                                    upgradableAppsItem.IsUpgrading = false;
                                    break;
                                }
                            }

                            // 应用升级失败，将当前任务状态修改为可升级状态
                            foreach (InstallingAppsModel installingAppsItem in WinGetInstance.InstallingAppsCollection)
                            {
                                if (installingAppsItem.AppID == upgradableApps.AppID)
                                {
                                    WinGetInstance.InstallingAppsCollection.Remove(installingAppsItem);
                                    break;
                                }
                            }
                        });
                    }
                    // 其他异常
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "App installing failed.", e);

                        WinGetInstance.installStateLock.Enter();

                        try
                        {
                            WinGetInstance.InstallingStateDict.Remove(upgradableApps.AppID);
                        }
                        catch (Exception exception)
                        {
                            ExceptionAsVoidMarshaller.ConvertToUnmanaged(exception);
                        }
                        finally
                        {
                            WinGetInstance.installStateLock.Exit();
                        }

                        DispatcherQueue.TryEnqueue(() =>
                        {
                            // 应用升级失败，从任务管理列表中移除当前任务
                            foreach (UpgradableAppsModel upgradableAppsItem in UpgradableAppsCollection)
                            {
                                if (upgradableAppsItem.AppID == upgradableApps.AppID)
                                {
                                    upgradableAppsItem.IsUpgrading = false;
                                    break;
                                }
                            }

                            // 应用升级失败，从任务管理列表中移除当前任务
                            foreach (InstallingAppsModel installingAppsItem in WinGetInstance.InstallingAppsCollection)
                            {
                                if (installingAppsItem.AppID == upgradableApps.AppID)
                                {
                                    WinGetInstance.InstallingAppsCollection.Remove(installingAppsItem);
                                    break;
                                }
                            }
                        });

                        ToastNotificationService.Show(NotificationKind.WinGetUpgradeFailed, upgradableApps.AppName, upgradableApps.AppID);
                    }
                });
            }
        }

        #endregion 第一部分：XamlUICommand 命令调用时挂载的事件

        #region 第二部分：可升级应用控件——挂载的事件

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
        /// 更新可升级应用数据
        /// </summary>
        private void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            MatchResultList.Clear();
            IsLoadedCompleted = false;
            GetUpgradableApps();
            InitializeData();
        }

        #endregion 第二部分：可升级应用控件——挂载的事件

        public void InitializeWingetInstance(WinGetPage wingetInstance)
        {
            WinGetInstance = wingetInstance;
        }

        /// <summary>
        /// 加载系统可升级的应用信息
        /// </summary>
        private void GetUpgradableApps()
        {
            try
            {
                autoResetEvent ??= new AutoResetEvent(false);
                Task.Run(async () =>
                {
                    IReadOnlyList<PackageCatalogReference> packageCatalogsList = UpgradableAppsManager.GetPackageCatalogs();
                    CreateCompositePackageCatalogOptions createCompositePackageCatalogOptions = WinGetService.CreateCreateCompositePackageCatalogOptions();
                    PackageCatalogReference searchCatalogReference = UpgradableAppsManager.GetLocalPackageCatalog(LocalPackageCatalog.InstalledPackages);

                    for (int index = 0; index < packageCatalogsList.Count; index++)
                    {
                        PackageCatalogReference catalogReference = packageCatalogsList[index];
                        createCompositePackageCatalogOptions.Catalogs.Add(catalogReference);
                    }
                    createCompositePackageCatalogOptions.CompositeSearchBehavior = CompositeSearchBehavior.LocalCatalogs;
                    PackageCatalogReference packageCatalogReference = UpgradableAppsManager.CreateCompositePackageCatalog(createCompositePackageCatalogOptions);

                    if ((await packageCatalogReference.ConnectAsync()).PackageCatalog is PackageCatalog upgradableCatalog)
                    {
                        FindPackagesOptions findPackagesOptions = WinGetService.CreateFindPackagesOptions();
                        FindPackagesResult findResult = await upgradableCatalog.FindPackagesAsync(findPackagesOptions);

                        for (int index = 0; index < findResult.Matches.Count; index++)
                        {
                            MatchResult matchResultItem = findResult.Matches[index];
                            if (matchResultItem.CatalogPackage.IsUpdateAvailable is true)
                            {
                                MatchResultList.Add(matchResultItem);
                            }
                        }
                    }
                    autoResetEvent?.Set();
                });
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Get upgradable apps information failed.", e);
            }
        }

        /// <summary>
        /// 初始化可升级应用数据
        /// </summary>
        private void InitializeData()
        {
            UpgradableAppsCollection.Clear();

            Task.Run(() =>
            {
                autoResetEvent?.WaitOne();
                autoResetEvent?.Dispose();
                autoResetEvent = null;

                if (MatchResultList is not null)
                {
                    List<UpgradableAppsModel> upgradableAppsList = [];
                    foreach (MatchResult matchItem in MatchResultList)
                    {
                        bool isUpgrading = false;

                        foreach (InstallingAppsModel installingAppsItem in WinGetInstance.InstallingAppsCollection)
                        {
                            if (matchItem.CatalogPackage.DefaultInstallVersion.Id == installingAppsItem.AppID)
                            {
                                isUpgrading = true;
                                break;
                            }
                        }

                        upgradableAppsList.Add(new UpgradableAppsModel()
                        {
                            AppID = matchItem.CatalogPackage.DefaultInstallVersion.Id,
                            AppName = string.IsNullOrEmpty(matchItem.CatalogPackage.DefaultInstallVersion.DisplayName) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.DefaultInstallVersion.DisplayName,
                            AppPublisher = string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.Publisher) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.InstalledVersion.Publisher,
                            AppCurrentVersion = string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.Version) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.InstalledVersion.Version,
                            AppNewestVersion = string.IsNullOrEmpty(matchItem.CatalogPackage.DefaultInstallVersion.Version) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.DefaultInstallVersion.Version,
                            IsUpgrading = isUpgrading
                        });
                    }

                    DispatcherQueue.TryEnqueue(() =>
                    {
                        foreach (UpgradableAppsModel upgradableAppsItem in upgradableAppsList)
                        {
                            UpgradableAppsCollection.Add(upgradableAppsItem);
                        }

                        if (MatchResultList.Count is 0)
                        {
                            IsUpgradableAppsEmpty = true;
                        }
                        else
                        {
                            IsUpgradableAppsEmpty = false;
                        }
                        IsLoadedCompleted = true;
                    });
                }
                else
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        IsUpgradableAppsEmpty = true;
                        IsLoadedCompleted = true;
                    });
                }
            });
        }
    }
}
