using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Controls.Extensions;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.WinGet;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.WinGet;
using GetStoreApp.UI.TeachingTips;
using GetStoreApp.Views.Pages;
using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;
using Windows.System;

namespace GetStoreApp.UI.Controls.WinGet
{
    /// <summary>
    /// WinGet 程序包页面：可升级应用控件
    /// </summary>
    public sealed partial class UpgradableAppsControl : Grid, INotifyPropertyChanged
    {
        private readonly object UpgradableAppsLock = new object();

        private bool isInitialized = false;

        private AutoResetEvent autoResetEvent;
        private PackageManager UpgradableAppsManager;
        private WinGetPage WinGetInstance;

        private bool _isLoadedCompleted = false;

        public bool IsLoadedCompleted
        {
            get { return _isLoadedCompleted; }

            set
            {
                _isLoadedCompleted = value;
                OnPropertyChanged();
            }
        }

        private bool _isUpgradableAppsEmpty;

        public bool IsUpgradableAppsEmpty
        {
            get { return _isUpgradableAppsEmpty; }

            set
            {
                _isUpgradableAppsEmpty = value;
                OnPropertyChanged();
            }
        }

        private List<MatchResult> MatchResultList;

        private ObservableCollection<UpgradableAppsModel> UpgradableAppsCollection { get; } = new ObservableCollection<UpgradableAppsModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        public UpgradableAppsControl()
        {
            InitializeComponent();
        }

        #region 第一部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 复制升级命令
        /// </summary>
        private void OnCopyUpgradeTextExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            string appId = args.Parameter as string;
            if (appId is not null)
            {
                string copyContent = string.Format("winget install {0}", appId);
                CopyPasteHelper.CopyTextToClipBoard(copyContent);

                TeachingTipHelper.Show(new DataCopyTip(DataCopyKind.WinGetUpgradeInstall));
            }
        }

        /// <summary>
        /// 使用命令安装
        /// </summary>
        private void OnInstallWithCmdExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            string appId = args.Parameter as string;
            if (appId is not null)
            {
                Task.Run(() =>
                {
                    Kernel32Library.GetStartupInfo(out STARTUPINFO WinGetProcessStartupInfo);
                    WinGetProcessStartupInfo.lpReserved = IntPtr.Zero;
                    WinGetProcessStartupInfo.lpDesktop = IntPtr.Zero;
                    WinGetProcessStartupInfo.lpTitle = IntPtr.Zero;
                    WinGetProcessStartupInfo.dwX = 0;
                    WinGetProcessStartupInfo.dwY = 0;
                    WinGetProcessStartupInfo.dwXSize = 0;
                    WinGetProcessStartupInfo.dwYSize = 0;
                    WinGetProcessStartupInfo.dwXCountChars = 500;
                    WinGetProcessStartupInfo.dwYCountChars = 500;
                    WinGetProcessStartupInfo.dwFlags = STARTF.STARTF_USESHOWWINDOW;
                    WinGetProcessStartupInfo.wShowWindow = WindowShowStyle.SW_SHOW;
                    WinGetProcessStartupInfo.cbReserved2 = 0;
                    WinGetProcessStartupInfo.lpReserved2 = IntPtr.Zero;
                    WinGetProcessStartupInfo.cb = Marshal.SizeOf(typeof(STARTUPINFO));

                    bool createResult = Kernel32Library.CreateProcess(null, string.Format("winget install {0}", appId), IntPtr.Zero, IntPtr.Zero, false, CreateProcessFlags.CREATE_NEW_CONSOLE, IntPtr.Zero, null, ref WinGetProcessStartupInfo, out PROCESS_INFORMATION WinGetProcessInformation);

                    if (createResult)
                    {
                        if (WinGetProcessInformation.hProcess != IntPtr.Zero) Kernel32Library.CloseHandle(WinGetProcessInformation.hProcess);
                        if (WinGetProcessInformation.hThread != IntPtr.Zero) Kernel32Library.CloseHandle(WinGetProcessInformation.hThread);
                    }
                });
            }
        }

        /// <summary>
        /// 应用升级
        /// </summary>
        private void OnUpdateExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            UpgradableAppsModel upgradableApps = args.Parameter as UpgradableAppsModel;
            if (upgradableApps is not null)
            {
                Task.Run(async () =>
                {
                    AutoResetEvent autoResetEvent = new AutoResetEvent(false);
                    try
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            lock (UpgradableAppsLock)
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
                            }
                        });

                        InstallOptions installOptions = WinGetService.CreateInstallOptions();

                        installOptions.PackageInstallMode = (PackageInstallMode)Enum.Parse(typeof(PackageInstallMode), WinGetConfigService.WinGetInstallMode.Value.ToString());
                        installOptions.PackageInstallScope = PackageInstallScope.Any;

                        // 更新升级进度
                        Progress<InstallProgress> progressCallBack = new Progress<InstallProgress>((installProgress) =>
                        {
                            switch (installProgress.State)
                            {
                                // 处于等待中状态
                                case PackageInstallProgressState.Queued:
                                    {
                                        DispatcherQueue.TryEnqueue(() =>
                                        {
                                            lock (WinGetInstance.InstallingAppsObject)
                                            {
                                                foreach (InstallingAppsModel installingItem in WinGetInstance.InstallingAppsCollection)
                                                {
                                                    if (installingItem.AppID == upgradableApps.AppID)
                                                    {
                                                        installingItem.InstallProgressState = PackageInstallProgressState.Queued;
                                                        break;
                                                    }
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
                                            lock (WinGetInstance.InstallingAppsObject)
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
                                            }
                                        });

                                        break;
                                    }
                                // 处于安装中状态
                                case PackageInstallProgressState.Installing:
                                    {
                                        DispatcherQueue.TryEnqueue(() =>
                                        {
                                            lock (WinGetInstance.InstallingAppsObject)
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
                                            }
                                        });

                                        break;
                                    }
                                // 挂起状态
                                case PackageInstallProgressState.PostInstall:
                                    {
                                        DispatcherQueue.TryEnqueue(() =>
                                        {
                                            lock (WinGetInstance.InstallingAppsObject)
                                            {
                                                foreach (InstallingAppsModel installingItem in WinGetInstance.InstallingAppsCollection)
                                                {
                                                    if (installingItem.AppID == upgradableApps.AppID)
                                                    {
                                                        installingItem.InstallProgressState = PackageInstallProgressState.PostInstall;
                                                        break;
                                                    }
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
                                            lock (WinGetInstance.InstallingAppsObject)
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
                                            }
                                        });

                                        break;
                                    }
                            }
                        });

                        // 任务取消执行操作
                        CancellationTokenSource upgradeTokenSource = new CancellationTokenSource();

                        // 添加任务
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            lock (WinGetInstance.InstallingAppsObject)
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
                            }
                        });

                        WinGetInstance.InstallingStateDict.Add(upgradableApps.AppID, upgradeTokenSource);

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
                                    Kernel32Library.GetStartupInfo(out STARTUPINFO RebootStartupInfo);
                                    RebootStartupInfo.lpReserved = IntPtr.Zero;
                                    RebootStartupInfo.lpDesktop = IntPtr.Zero;
                                    RebootStartupInfo.lpTitle = IntPtr.Zero;
                                    RebootStartupInfo.dwX = 0;
                                    RebootStartupInfo.dwY = 0;
                                    RebootStartupInfo.dwXSize = 0;
                                    RebootStartupInfo.dwYSize = 0;
                                    RebootStartupInfo.dwXCountChars = 500;
                                    RebootStartupInfo.dwYCountChars = 500;
                                    RebootStartupInfo.dwFlags = STARTF.STARTF_USESHOWWINDOW;
                                    RebootStartupInfo.wShowWindow = WindowShowStyle.SW_HIDE;
                                    RebootStartupInfo.cbReserved2 = 0;
                                    RebootStartupInfo.lpReserved2 = IntPtr.Zero;

                                    RebootStartupInfo.cb = Marshal.SizeOf(typeof(STARTUPINFO));
                                    bool createResult = Kernel32Library.CreateProcess(null, string.Format("{0} {1}", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32", "Shutdown.exe"), "-r -t 120"), IntPtr.Zero, IntPtr.Zero, false, CreateProcessFlags.CREATE_NO_WINDOW, IntPtr.Zero, null, ref RebootStartupInfo, out PROCESS_INFORMATION RebootProcessInformation);

                                    if (createResult)
                                    {
                                        if (RebootProcessInformation.hProcess != IntPtr.Zero) Kernel32Library.CloseHandle(RebootProcessInformation.hProcess);
                                        if (RebootProcessInformation.hThread != IntPtr.Zero) Kernel32Library.CloseHandle(RebootProcessInformation.hThread);
                                    }
                                }
                            }

                            DispatcherQueue.TryEnqueue(() =>
                            {
                                // 完成任务后从任务管理中删除任务
                                lock (WinGetInstance.InstallingAppsObject)
                                {
                                    foreach (InstallingAppsModel installingAppsItem in WinGetInstance.InstallingAppsCollection)
                                    {
                                        if (installingAppsItem.AppID == upgradableApps.AppID)
                                        {
                                            WinGetInstance.InstallingAppsCollection.Remove(installingAppsItem);
                                            break;
                                        }
                                    }
                                    WinGetInstance.InstallingStateDict.Remove(upgradableApps.AppID);
                                }

                                lock (UpgradableAppsLock)
                                {
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
                                }
                            });
                        }
                        else
                        {
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                lock (UpgradableAppsLock)
                                {
                                    // 应用升级失败，将当前任务状态修改为可升级状态
                                    foreach (UpgradableAppsModel upgradableAppsItem in UpgradableAppsCollection)
                                    {
                                        if (upgradableAppsItem.AppID == upgradableApps.AppID)
                                        {
                                            upgradableAppsItem.IsUpgrading = false;
                                        }
                                    }
                                }

                                // 应用升级失败，将当前任务状态修改为可升级状态
                                lock (WinGetInstance.InstallingAppsObject)
                                {
                                    foreach (InstallingAppsModel installingAppsItem in WinGetInstance.InstallingAppsCollection)
                                    {
                                        if (installingAppsItem.AppID == upgradableApps.AppID)
                                        {
                                            WinGetInstance.InstallingAppsCollection.Remove(installingAppsItem);
                                            break;
                                        }
                                    }
                                    WinGetInstance.InstallingStateDict.Remove(upgradableApps.AppID);
                                }
                            });

                            ToastNotificationService.Show(NotificationKind.WinGetUpgradeFailed, upgradableApps.AppName, upgradableApps.AppID);
                        }
                    }
                    // 操作被用户所取消异常
                    catch (OperationCanceledException e)
                    {
                        LogService.WriteLog(LoggingLevel.Information, "App installing operation canceled.", e);

                        DispatcherQueue.TryEnqueue(() =>
                        {
                            lock (UpgradableAppsLock)
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
                            }

                            // 应用升级失败，将当前任务状态修改为可升级状态
                            lock (WinGetInstance.InstallingAppsObject)
                            {
                                foreach (InstallingAppsModel installingAppsItem in WinGetInstance.InstallingAppsCollection)
                                {
                                    if (installingAppsItem.AppID == upgradableApps.AppID)
                                    {
                                        WinGetInstance.InstallingAppsCollection.Remove(installingAppsItem);
                                        break;
                                    }
                                }
                                WinGetInstance.InstallingStateDict.Remove(upgradableApps.AppID);
                            }
                        });
                    }
                    // 其他异常
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "App installing failed.", e);

                        DispatcherQueue.TryEnqueue(() =>
                        {
                            lock (UpgradableAppsLock)
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
                            }

                            // 应用升级失败，从任务管理列表中移除当前任务
                            lock (WinGetInstance.InstallingAppsObject)
                            {
                                foreach (InstallingAppsModel installingAppsItem in WinGetInstance.InstallingAppsCollection)
                                {
                                    if (installingAppsItem.AppID == upgradableApps.AppID)
                                    {
                                        WinGetInstance.InstallingAppsCollection.Remove(installingAppsItem);
                                        break;
                                    }
                                }
                                WinGetInstance.InstallingStateDict.Remove(upgradableApps.AppID);
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
        /// 初始化可升级应用信息
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (!isInitialized)
            {
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
                isInitialized = true;
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
        /// 更新可升级应用数据
        /// </summary>
        private void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            MatchResultList = null;
            IsLoadedCompleted = false;
            GetUpgradableApps();
            InitializeData();
        }

        #endregion 第二部分：可升级应用控件——挂载的事件

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 本地化应用数量统计信息
        /// </summary>
        private string LocalizeUpgradableAppsCountInfo(int count)
        {
            if (count is 0)
            {
                return ResourceService.GetLocalized("WinGet/UpgradableAppsCountEmpty");
            }
            else
            {
                return string.Format(ResourceService.GetLocalized("WinGet/UpgradableAppsCountInfo"), count);
            }
        }

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
                    List<PackageCatalogReference> packageCatalogReferences = UpgradableAppsManager.GetPackageCatalogs().ToList();
                    CreateCompositePackageCatalogOptions createCompositePackageCatalogOptions = WinGetService.CreateCreateCompositePackageCatalogOptions();
                    PackageCatalogReference searchCatalogReference = UpgradableAppsManager.GetLocalPackageCatalog(LocalPackageCatalog.InstalledPackages);
                    foreach (PackageCatalogReference catalogReference in packageCatalogReferences)
                    {
                        createCompositePackageCatalogOptions.Catalogs.Add(catalogReference);
                    }
                    createCompositePackageCatalogOptions.CompositeSearchBehavior = CompositeSearchBehavior.LocalCatalogs;
                    PackageCatalogReference packageCatalogReference = UpgradableAppsManager.CreateCompositePackageCatalog(createCompositePackageCatalogOptions);
                    ConnectResult connectResult = await packageCatalogReference.ConnectAsync();
                    PackageCatalog upgradableCatalog = connectResult.PackageCatalog;

                    if (upgradableCatalog is not null)
                    {
                        FindPackagesOptions findPackagesOptions = WinGetService.CreateFindPackagesOptions();
                        FindPackagesResult findResult = await upgradableCatalog.FindPackagesAsync(findPackagesOptions);
                        List<MatchResult> result = findResult.Matches.ToList();

                        MatchResultList = findResult.Matches.ToList().Where(item => item.CatalogPackage.IsUpdateAvailable is true).ToList();
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
            lock (UpgradableAppsLock)
            {
                UpgradableAppsCollection.Clear();
            }

            Task.Run(() =>
            {
                autoResetEvent?.WaitOne();
                autoResetEvent?.Dispose();
                autoResetEvent = null;

                if (MatchResultList is not null)
                {
                    List<UpgradableAppsModel> upgradableAppsList = new List<UpgradableAppsModel>();
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
                        lock (UpgradableAppsLock)
                        {
                            foreach (UpgradableAppsModel upgradableAppsItem in upgradableAppsList)
                            {
                                UpgradableAppsCollection.Add(upgradableAppsItem);
                            }
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
