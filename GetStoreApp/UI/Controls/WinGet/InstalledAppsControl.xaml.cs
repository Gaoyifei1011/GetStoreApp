using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Controls.Extensions;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.WinGet;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.WinGet;
using GetStoreApp.UI.TeachingTips;
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

namespace GetStoreApp.UI.Controls.WinGet
{
    /// <summary>
    /// WinGet 程序包页面：已安装应用控件
    /// </summary>
    public sealed partial class InstalledAppsControl : Grid, INotifyPropertyChanged
    {
        private readonly object InstalledAppsLock = new object();

        private bool isInitialized = false;

        private AutoResetEvent autoResetEvent;
        private PackageManager InstalledAppsManager;

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

        private bool _isInstalledAppsEmpty;

        public bool IsInstalledAppsEmpty
        {
            get { return _isInstalledAppsEmpty; }

            set
            {
                _isInstalledAppsEmpty = value;
                OnPropertyChanged();
            }
        }

        private bool _isIncrease = true;

        public bool IsIncrease
        {
            get { return _isIncrease; }

            set
            {
                _isIncrease = value;
                OnPropertyChanged();
            }
        }

        private string _searchText = string.Empty;

        public string SearchText
        {
            get { return _searchText; }

            set
            {
                _searchText = value;
                OnPropertyChanged();
            }
        }

        private AppSortRuleKind _selectedRule = AppSortRuleKind.DisplayName;

        public AppSortRuleKind SelectedRule
        {
            get { return _selectedRule; }

            set
            {
                _selectedRule = value;
                OnPropertyChanged();
            }
        }

        private List<MatchResult> MatchResultList;

        private ObservableCollection<InstalledAppsModel> InstalledAppsCollection { get; } = new ObservableCollection<InstalledAppsModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        public InstalledAppsControl()
        {
            InitializeComponent();
        }

        #region 第一部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 复制卸载命令
        /// </summary>
        private void OnCopyUnInstallTextExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            string appId = args.Parameter as string;
            if (appId is not null)
            {
                string copyContent = string.Format("winget uninstall {0}", appId);
                bool copyResult = CopyPasteHelper.CopyTextToClipBoard(copyContent);

                TeachingTipHelper.Show(new DataCopyTip(DataCopyKind.WinGetUnInstall, copyResult));
            }
        }

        /// <summary>
        /// 卸载应用
        /// </summary>
        private void OnUnInstallExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            InstalledAppsModel installedApps = args.Parameter as InstalledAppsModel;

            if (installedApps is not null)
            {
                Task.Run(async () =>
                {
                    AutoResetEvent autoResetEvent = new AutoResetEvent(false);
                    try
                    {
                        UninstallOptions uninstallOptions = WinGetService.CreateUninstallOptions();

                        uninstallOptions.PackageUninstallMode = PackageUninstallMode.Interactive;
                        uninstallOptions.PackageUninstallScope = PackageUninstallScope.Any;

                        UninstallResult unInstallResult = await InstalledAppsManager.UninstallPackageAsync(MatchResultList.Find(item => item.CatalogPackage.InstalledVersion.Id == installedApps.AppID).CatalogPackage, uninstallOptions);

                        // 获取卸载后的结果信息
                        // 卸载成功，从列表中删除该应用
                        if (unInstallResult.Status is UninstallResultStatus.Ok)
                        {
                            ToastNotificationService.Show(NotificationKind.WinGetUnInstallSuccessfully, installedApps.AppName);

                            // 检测是否需要重启设备完成应用的卸载，如果是，询问用户是否需要重启设备
                            if (unInstallResult.RebootRequired)
                            {
                                ContentDialogResult result = ContentDialogResult.None;
                                DispatcherQueue.TryEnqueue(async () =>
                                {
                                    result = await ContentDialogHelper.ShowAsync(new RebootDialog(WinGetOptionKind.UnInstall, installedApps.AppName), this);
                                    autoResetEvent.Set();
                                });

                                autoResetEvent.WaitOne();
                                autoResetEvent.Dispose();

                                if (result is ContentDialogResult.Primary)
                                {
                                    Kernel32Library.GetStartupInfo(out STARTUPINFO shutdownStartupInfo);
                                    shutdownStartupInfo.lpReserved = IntPtr.Zero;
                                    shutdownStartupInfo.lpDesktop = IntPtr.Zero;
                                    shutdownStartupInfo.lpTitle = IntPtr.Zero;
                                    shutdownStartupInfo.dwX = 0;
                                    shutdownStartupInfo.dwY = 0;
                                    shutdownStartupInfo.dwXSize = 0;
                                    shutdownStartupInfo.dwYSize = 0;
                                    shutdownStartupInfo.dwXCountChars = 500;
                                    shutdownStartupInfo.dwYCountChars = 500;
                                    shutdownStartupInfo.dwFlags = STARTF.STARTF_USESHOWWINDOW;
                                    shutdownStartupInfo.wShowWindow = WindowShowStyle.SW_HIDE;
                                    shutdownStartupInfo.cbReserved2 = 0;
                                    shutdownStartupInfo.lpReserved2 = IntPtr.Zero;

                                    shutdownStartupInfo.cb = Marshal.SizeOf(typeof(STARTUPINFO));
                                    bool createResult = Kernel32Library.CreateProcess(null, string.Format("{0} {1}", Path.Combine(InfoHelper.SystemDataPath.Windows, "System32", "Shutdown.exe"), "-r -t 120"), IntPtr.Zero, IntPtr.Zero, false, CreateProcessFlags.CREATE_NO_WINDOW, IntPtr.Zero, null, ref shutdownStartupInfo, out PROCESS_INFORMATION shutdownInformation);

                                    if (createResult)
                                    {
                                        if (shutdownInformation.hProcess != IntPtr.Zero) Kernel32Library.CloseHandle(shutdownInformation.hProcess);
                                        if (shutdownInformation.hThread != IntPtr.Zero) Kernel32Library.CloseHandle(shutdownInformation.hThread);
                                    }
                                }
                            }

                            DispatcherQueue.TryEnqueue(() =>
                            {
                                lock (InstalledAppsLock)
                                {
                                    // 从已安装应用列表中移除已卸载完成的应用
                                    foreach (InstalledAppsModel installedAppsItem in InstalledAppsCollection)
                                    {
                                        if (installedAppsItem.AppID == installedApps.AppID)
                                        {
                                            InstalledAppsCollection.Remove(installedAppsItem);
                                            IsInstalledAppsEmpty = InstalledAppsCollection.Count is 0;
                                            break;
                                        }
                                    }
                                }
                            });
                        }
                        else
                        {
                            ToastNotificationService.Show(NotificationKind.WinGetUnInstallFailed, installedApps.AppName);
                        }
                    }
                    // 操作被用户所取消异常
                    catch (OperationCanceledException e)
                    {
                        LogService.WriteLog(LoggingLevel.Information, "App uninstalling operation canceled.", e);
                        ToastNotificationService.Show(NotificationKind.WinGetUnInstallFailed, installedApps.AppName);
                    }
                    // 其他异常
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "App uninstalling failed.", e);
                        ToastNotificationService.Show(NotificationKind.WinGetUnInstallFailed, installedApps.AppName);
                    }
                });
            }
        }

        #endregion 第一部分：XamlUICommand 命令调用时挂载的事件

        #region 第二部分：已安装应用控件——挂载的事件

        /// <summary>
        /// 初始化已安装应用信息
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (!isInitialized)
            {
                try
                {
                    InstalledAppsManager = WinGetService.CreatePackageManager();
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "Installed apps information initialized failed.", e);
                    return;
                }
                isInitialized = true;
                GetInstalledApps();
                InitializeData();
            }
        }

        /// <summary>
        /// 根据排序方式对列表进行排序
        /// </summary>
        private void OnSortWayClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem toggleMenuFlyoutItem = sender as ToggleMenuFlyoutItem;
            if (toggleMenuFlyoutItem is not null)
            {
                IsIncrease = Convert.ToBoolean(toggleMenuFlyoutItem.Tag);
                InitializeData();
            }
        }

        /// <summary>
        /// 根据排序规则对列表进行排序
        /// </summary>
        private void OnSortRuleClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem toggleMenuFlyoutItem = sender as ToggleMenuFlyoutItem;
            if (toggleMenuFlyoutItem is not null)
            {
                SelectedRule = (AppSortRuleKind)toggleMenuFlyoutItem.Tag;
                InitializeData();
            }
        }

        /// <summary>
        /// 更新已安装应用数据
        /// </summary>
        private void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            MatchResultList = null;
            IsLoadedCompleted = false;
            SearchText = string.Empty;
            GetInstalledApps();
            InitializeData();
        }

        /// <summary>
        /// 根据输入的内容检索应用
        /// </summary>
        private void OnQuerySubmitted(object sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrEmpty(SearchText))
            {
                InitializeData(true);
            }
        }

        /// <summary>
        /// 文本输入框内容为空时，复原原来的内容
        /// </summary>
        private void OnTextChanged(object sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            AutoSuggestBox autoSuggestBox = sender as AutoSuggestBox;
            if (autoSuggestBox is not null)
            {
                if (autoSuggestBox.Text == string.Empty && MatchResultList is not null)
                {
                    InitializeData();
                }
            }
        }

        #endregion 第二部分：已安装应用控件——挂载的事件

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
        private string LocalizeInstalledAppsCountInfo(int count)
        {
            if (count is 0)
            {
                return ResourceService.GetLocalized("WinGet/InstalledAppsCountEmpty");
            }
            else
            {
                return string.Format(ResourceService.GetLocalized("WinGet/InstalledAppsCountInfo"), count);
            }
        }

        /// <summary>
        /// 加载系统已安装的应用信息
        /// </summary>
        private void GetInstalledApps()
        {
            try
            {
                autoResetEvent ??= new AutoResetEvent(false);
                Task.Run(async () =>
                {
                    await Task.Delay(300);
                    PackageCatalogReference searchCatalogReference = InstalledAppsManager.GetLocalPackageCatalog(LocalPackageCatalog.InstalledPackages);

                    ConnectResult connectResult = await searchCatalogReference.ConnectAsync();
                    PackageCatalog installedCatalog = connectResult.PackageCatalog;

                    if (installedCatalog is not null)
                    {
                        FindPackagesOptions findPackagesOptions = WinGetService.CreateFindPackagesOptions();
                        FindPackagesResult findResult = await installedCatalog.FindPackagesAsync(findPackagesOptions);

                        MatchResultList = findResult.Matches.ToList().Where(item => item.CatalogPackage.InstalledVersion.Publisher != string.Empty).ToList();
                    }
                    autoResetEvent?.Set();
                });
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Get installed apps information failed.", e);
            }
        }

        /// <summary>
        /// 初始化列表数据
        /// </summary>
        private void InitializeData(bool hasSearchText = false)
        {
            lock (InstalledAppsLock)
            {
                InstalledAppsCollection.Clear();
            }

            Task.Run(() =>
            {
                autoResetEvent?.WaitOne();
                autoResetEvent?.Dispose();
                autoResetEvent = null;

                if (MatchResultList is not null)
                {
                    List<InstalledAppsModel> installedAppsList = new List<InstalledAppsModel>();

                    if (hasSearchText)
                    {
                        foreach (MatchResult matchItem in MatchResultList)
                        {
                            if (matchItem.CatalogPackage.InstalledVersion.DisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                            {
                                installedAppsList.Add(new InstalledAppsModel()
                                {
                                    AppID = matchItem.CatalogPackage.InstalledVersion.Id,
                                    AppName = string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.DisplayName) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.InstalledVersion.DisplayName,
                                    AppPublisher = string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.Publisher) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.InstalledVersion.Publisher,
                                    AppVersion = string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.Version) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.InstalledVersion.Version,
                                });
                            }
                        }
                    }
                    else
                    {
                        foreach (MatchResult matchItem in MatchResultList)
                        {
                            installedAppsList.Add(new InstalledAppsModel()
                            {
                                AppID = matchItem.CatalogPackage.InstalledVersion.Id,
                                AppName = string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.DisplayName) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.InstalledVersion.DisplayName,
                                AppPublisher = string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.Publisher) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.InstalledVersion.Publisher,
                                AppVersion = string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.Version) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.InstalledVersion.Version,
                            });
                        }
                    }

                    switch (SelectedRule)
                    {
                        case AppSortRuleKind.DisplayName:
                            {
                                if (IsIncrease)
                                {
                                    installedAppsList = installedAppsList.OrderBy(item => item.AppName).ToList();
                                }
                                else
                                {
                                    installedAppsList = installedAppsList.OrderByDescending(item => item.AppName).ToList();
                                }
                                break;
                            }
                        case AppSortRuleKind.PublisherName:
                            {
                                if (IsIncrease)
                                {
                                    installedAppsList = installedAppsList.OrderBy(item => item.AppPublisher).ToList();
                                }
                                else
                                {
                                    installedAppsList = installedAppsList.OrderByDescending(item => item.AppPublisher).ToList();
                                }
                                break;
                            }
                    }

                    DispatcherQueue.TryEnqueue(() =>
                    {
                        lock (InstalledAppsLock)
                        {
                            foreach (InstalledAppsModel installedApps in installedAppsList)
                            {
                                InstalledAppsCollection.Add(installedApps);
                            }
                            IsInstalledAppsEmpty = MatchResultList.Count is 0;
                            IsLoadedCompleted = true;
                        }
                    });
                }
                else
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        lock (InstalledAppsLock)
                        {
                            IsInstalledAppsEmpty = true;
                            IsLoadedCompleted = true;
                        }
                    });
                }
            });
        }
    }
}
