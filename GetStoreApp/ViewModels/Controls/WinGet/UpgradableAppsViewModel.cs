using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.WinGet;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.WinGet;
using GetStoreApp.UI.Notifications;
using GetStoreApp.ViewModels.Base;
using GetStoreApp.ViewModels.Pages;
using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using Microsoft.Management.Deployment;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Controls.WinGet
{
    public sealed class UpgradableAppsViewModel : ViewModelBase
    {
        private PackageManager UpgradableAppsManager { get; set; }

        internal WinGetViewModel WinGetVMInstance;

        private bool isInitialized = false;

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

        public ObservableCollection<UpgradableAppsModel> UpgradableAppsDataList { get; } = new ObservableCollection<UpgradableAppsModel>();

        // 应用升级
        public XamlUICommand UpdateCommand { get; } = new XamlUICommand();

        public XamlUICommand CopyUpgradeTextCommand { get; } = new XamlUICommand();

        public XamlUICommand InstallWithCmdCommand { get; } = new XamlUICommand();

        /// <summary>
        /// 初始化可升级应用信息
        /// </summary>
        public async void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (!isInitialized)
            {
                try
                {
                    UpgradableAppsManager = WinGetService.CreatePackageManager();
                }
                catch (Exception)
                {
                    return;
                }
                await Task.Delay(500);
                await GetUpgradableAppsAsync();
                InitializeData();
                if (MatchResultList is null || MatchResultList.Count is 0)
                {
                    IsUpgradableAppsEmpty = true;
                }
                else
                {
                    IsUpgradableAppsEmpty = false;
                }
                IsLoadedCompleted = true;
                isInitialized = true;
            }
        }

        /// <summary>
        /// 更新可升级应用数据
        /// </summary>
        public async void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            MatchResultList = null;
            IsLoadedCompleted = false;
            await Task.Delay(500);
            await GetUpgradableAppsAsync();
            InitializeData();
            if (MatchResultList is null || MatchResultList.Count is 0)
            {
                IsUpgradableAppsEmpty = true;
            }
            else
            {
                IsUpgradableAppsEmpty = false;
            }
            IsLoadedCompleted = true;
        }

        public UpgradableAppsViewModel()
        {
            UpdateCommand.ExecuteRequested += async (sender, args) =>
            {
                UpgradableAppsModel upgradableApps = args.Parameter as UpgradableAppsModel;
                if (upgradableApps is not null)
                {
                    try
                    {
                        // 禁用当前应用的可升级状态
                        foreach (UpgradableAppsModel upgradableAppsItem in UpgradableAppsDataList)
                        {
                            if (upgradableAppsItem.AppID == upgradableApps.AppID)
                            {
                                upgradableAppsItem.IsUpgrading = true;
                                break;
                            }
                        }

                        InstallOptions installOptions = WinGetService.CreateInstallOptions();

                        installOptions.PackageInstallMode = (PackageInstallMode)Enum.Parse(typeof(PackageInstallMode), WinGetConfigService.WinGetInstallMode.InternalName);
                        installOptions.PackageInstallScope = PackageInstallScope.Any;

                        // 更新升级进度
                        Progress<InstallProgress> progressCallBack = new Progress<InstallProgress>((installProgress) =>
                        {
                            switch (installProgress.State)
                            {
                                // 处于等待中状态
                                case PackageInstallProgressState.Queued:
                                    {
                                        lock (WinGetVMInstance.InstallingAppsObject)
                                        {
                                            foreach (InstallingAppsModel installingItem in WinGetVMInstance.InstallingAppsList)
                                            {
                                                if (installingItem.AppID == upgradableApps.AppID)
                                                {
                                                    installingItem.InstallProgressState = PackageInstallProgressState.Queued;
                                                    break;
                                                }
                                            }
                                        }
                                        break;
                                    }
                                // 处于下载中状态
                                case PackageInstallProgressState.Downloading:
                                    {
                                        lock (WinGetVMInstance.InstallingAppsObject)
                                        {
                                            foreach (InstallingAppsModel installingItem in WinGetVMInstance.InstallingAppsList)
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
                                        break;
                                    }
                                // 处于安装中状态
                                case PackageInstallProgressState.Installing:
                                    {
                                        lock (WinGetVMInstance.InstallingAppsObject)
                                        {
                                            foreach (InstallingAppsModel installingItem in WinGetVMInstance.InstallingAppsList)
                                            {
                                                if (installingItem.AppID == upgradableApps.AppID)
                                                {
                                                    installingItem.InstallProgressState = PackageInstallProgressState.Installing;
                                                    installingItem.DownloadProgress = 100;
                                                    break;
                                                }
                                            }
                                        }
                                        break;
                                    }
                                // 挂起状态
                                case PackageInstallProgressState.PostInstall:
                                    {
                                        lock (WinGetVMInstance.InstallingAppsObject)
                                        {
                                            foreach (InstallingAppsModel installingItem in WinGetVMInstance.InstallingAppsList)
                                            {
                                                if (installingItem.AppID == upgradableApps.AppID)
                                                {
                                                    installingItem.InstallProgressState = PackageInstallProgressState.PostInstall;
                                                    break;
                                                }
                                            }
                                        }
                                        break;
                                    }
                                // 处于安装完成状态
                                case PackageInstallProgressState.Finished:
                                    {
                                        lock (WinGetVMInstance.InstallingAppsObject)
                                        {
                                            foreach (InstallingAppsModel installingItem in WinGetVMInstance.InstallingAppsList)
                                            {
                                                if (installingItem.AppID == upgradableApps.AppID)
                                                {
                                                    installingItem.InstallProgressState = PackageInstallProgressState.Finished;
                                                    installingItem.DownloadProgress = 100;
                                                    break;
                                                }
                                            }
                                        }
                                        break;
                                    }
                            }
                        });

                        // 任务取消执行操作
                        CancellationTokenSource upgradeTokenSource = new CancellationTokenSource();

                        // 添加任务
                        lock (WinGetVMInstance.InstallingAppsObject)
                        {
                            WinGetVMInstance.InstallingAppsList.Add(new InstallingAppsModel()
                            {
                                AppID = upgradableApps.AppID,
                                AppName = upgradableApps.AppName,
                                DownloadProgress = 0,
                                InstallProgressState = PackageInstallProgressState.Queued,
                                DownloadedFileSize = FileSizeHelper.ConvertFileSizeToString(0),
                                TotalFileSize = FileSizeHelper.ConvertFileSizeToString(0)
                            });
                            WinGetVMInstance.InstallingStateDict.Add(upgradableApps.AppID, upgradeTokenSource);
                        }

                        InstallResult installResult = await UpgradableAppsManager.UpgradePackageAsync(MatchResultList.Find(item => item.CatalogPackage.DefaultInstallVersion.Id == upgradableApps.AppID).CatalogPackage, installOptions).AsTask(upgradeTokenSource.Token, progressCallBack);

                        // 获取升级完成后的结果信息
                        // 升级完成，从列表中删除该应用
                        if (installResult.Status == InstallResultStatus.Ok)
                        {
                            // 检测是否需要重启设备完成应用的卸载，如果是，询问用户是否需要重启设备
                            if (installResult.RebootRequired)
                            {
                                AppNotificationService.Show(NotificationArgs.UpgradeSuccessfully, upgradableApps.AppName);

                                ContentDialogResult Result = await new RebootDialog(WinGetOptionArgs.UpgradeInstall, upgradableApps.AppName).ShowAsync();
                                if (Result == ContentDialogResult.Primary)
                                {
                                    unsafe
                                    {
                                        Kernel32Library.GetStartupInfo(out STARTUPINFO RebootStartupInfo);
                                        RebootStartupInfo.lpReserved = null;
                                        RebootStartupInfo.lpDesktop = null;
                                        RebootStartupInfo.lpTitle = null;
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
                            }

                            // 完成任务后从任务管理中删除任务
                            lock (WinGetVMInstance.InstallingAppsObject)
                            {
                                foreach (InstallingAppsModel installingAppsItem in WinGetVMInstance.InstallingAppsList)
                                {
                                    if (installingAppsItem.AppID == upgradableApps.AppID)
                                    {
                                        WinGetVMInstance.InstallingAppsList.Remove(installingAppsItem);
                                        break;
                                    }
                                }
                                WinGetVMInstance.InstallingStateDict.Remove(upgradableApps.AppID);
                            }

                            // 从升级列表中移除已升级完成的任务
                            foreach (UpgradableAppsModel upgradableAppsItem in UpgradableAppsDataList)
                            {
                                if (upgradableAppsItem.AppID == upgradableApps.AppID)
                                {
                                    UpgradableAppsDataList.Remove(upgradableAppsItem);
                                    IsUpgradableAppsEmpty = UpgradableAppsDataList.Count is 0;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            // 应用升级失败，将当前任务状态修改为可升级状态
                            foreach (UpgradableAppsModel upgradableAppsItem in UpgradableAppsDataList)
                            {
                                if (upgradableAppsItem.AppID == upgradableApps.AppID)
                                {
                                    upgradableAppsItem.IsUpgrading = false;
                                }
                            }

                            // 应用升级失败，将当前任务状态修改为可升级状态
                            lock (WinGetVMInstance.InstallingAppsObject)
                            {
                                foreach (InstallingAppsModel installingAppsItem in WinGetVMInstance.InstallingAppsList)
                                {
                                    if (installingAppsItem.AppID == upgradableApps.AppID)
                                    {
                                        WinGetVMInstance.InstallingAppsList.Remove(installingAppsItem);
                                        break;
                                    }
                                }
                                WinGetVMInstance.InstallingStateDict.Remove(upgradableApps.AppID);
                            }

                            AppNotificationService.Show(NotificationArgs.UpgradeFailed, upgradableApps.AppName, upgradableApps.AppID);
                        }
                    }
                    // 操作被用户所取消异常
                    catch (OperationCanceledException)
                    {
                        // 应用升级失败，将当前任务状态修改为可升级状态
                        foreach (UpgradableAppsModel upgradableAppsItem in UpgradableAppsDataList)
                        {
                            if (upgradableAppsItem.AppID == upgradableApps.AppID)
                            {
                                upgradableAppsItem.IsUpgrading = false;
                                break;
                            }
                        }

                        // 应用升级失败，将当前任务状态修改为可升级状态
                        lock (WinGetVMInstance.InstallingAppsObject)
                        {
                            foreach (InstallingAppsModel installingAppsItem in WinGetVMInstance.InstallingAppsList)
                            {
                                if (installingAppsItem.AppID == upgradableApps.AppID)
                                {
                                    WinGetVMInstance.InstallingAppsList.Remove(installingAppsItem);
                                    break;
                                }
                            }
                            WinGetVMInstance.InstallingStateDict.Remove(upgradableApps.AppID);
                        }
                    }
                    // 其他异常
                    catch (Exception)
                    {
                        // 应用升级失败，从任务管理列表中移除当前任务
                        foreach (UpgradableAppsModel upgradableAppsItem in UpgradableAppsDataList)
                        {
                            if (upgradableAppsItem.AppID == upgradableApps.AppID)
                            {
                                upgradableAppsItem.IsUpgrading = false;
                                break;
                            }
                        }

                        // 应用升级失败，从任务管理列表中移除当前任务
                        lock (WinGetVMInstance.InstallingAppsObject)
                        {
                            foreach (InstallingAppsModel installingAppsItem in WinGetVMInstance.InstallingAppsList)
                            {
                                if (installingAppsItem.AppID == upgradableApps.AppID)
                                {
                                    WinGetVMInstance.InstallingAppsList.Remove(installingAppsItem);
                                    break;
                                }
                            }
                            WinGetVMInstance.InstallingStateDict.Remove(upgradableApps.AppID);
                        }

                        AppNotificationService.Show(NotificationArgs.UpgradeFailed, upgradableApps.AppName, upgradableApps.AppID);
                    }
                }
            };

            CopyUpgradeTextCommand.ExecuteRequested += (sender, args) =>
            {
                string appId = args.Parameter as string;
                if (appId is not null)
                {
                    string copyContent = string.Format("winget install {0}", appId);
                    CopyPasteHelper.CopyToClipBoard(copyContent);

                    new WinGetCopyNotification(true, WinGetOptionArgs.UpgradeInstall).Show();
                }
            };

            InstallWithCmdCommand.ExecuteRequested += (sender, args) =>
            {
                string appId = args.Parameter as string;
                if (appId is not null)
                {
                    unsafe
                    {
                        Kernel32Library.GetStartupInfo(out STARTUPINFO WinGetProcessStartupInfo);
                        WinGetProcessStartupInfo.lpReserved = null;
                        WinGetProcessStartupInfo.lpDesktop = null;
                        WinGetProcessStartupInfo.lpTitle = null;
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
                    }
                }
            };
        }

        /// <summary>
        /// 加载系统可升级的应用信息
        /// </summary>
        private async Task GetUpgradableAppsAsync()
        {
            try
            {
                await Task.Run(async () =>
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
                        var result = findResult.Matches.ToList();

                        MatchResultList = findResult.Matches.ToList().Where(item => item.CatalogPackage.IsUpdateAvailable == true).ToList();
                    }
                });
            }
            catch (Exception) { }
        }

        private void InitializeData()
        {
            UpgradableAppsDataList.Clear();
            if (MatchResultList is not null)
            {
                foreach (MatchResult matchItem in MatchResultList)
                {
                    bool isUpgrading = false;
                    foreach (InstallingAppsModel installingAppsItem in WinGetVMInstance.InstallingAppsList)
                    {
                        if (matchItem.CatalogPackage.DefaultInstallVersion.Id == installingAppsItem.AppID)
                        {
                            isUpgrading = true;
                            break;
                        }
                    }
                    UpgradableAppsDataList.Add(new UpgradableAppsModel()
                    {
                        AppID = matchItem.CatalogPackage.DefaultInstallVersion.Id,
                        AppName = string.IsNullOrEmpty(matchItem.CatalogPackage.DefaultInstallVersion.DisplayName) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.DefaultInstallVersion.DisplayName,
                        AppPublisher = string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.Publisher) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.InstalledVersion.Publisher,
                        AppCurrentVersion = string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.Version) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.InstalledVersion.Version,
                        AppNewestVersion = string.IsNullOrEmpty(matchItem.CatalogPackage.DefaultInstallVersion.Version) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.DefaultInstallVersion.Version,
                        IsUpgrading = isUpgrading
                    });
                }
            }
        }
    }
}
