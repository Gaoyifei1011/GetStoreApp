using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Controls.Extensions;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.WinGet;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.WinGet;
using GetStoreApp.UI.Notifications;
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
using System.Threading.Tasks;

namespace GetStoreApp.UI.Controls.WinGet
{
    /// <summary>
    /// WinGet 程序包页面：已安装应用控件
    /// </summary>
    public sealed partial class InstalledAppsControl : Grid, INotifyPropertyChanged
    {
        private readonly object InstalledAppsDataListObject = new object();
        private bool isInitialized = false;

        private PackageManager InstalledAppsManager { get; set; }

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

        private List<MatchResult> MatchResultList;

        // 卸载应用
        public XamlUICommand UnInstallCommand { get; } = new XamlUICommand();

        // 复制卸载命令
        public XamlUICommand CopyUnInstallTextCommand { get; } = new XamlUICommand();

        public ObservableCollection<InstalledAppsModel> InstalledAppsDataList { get; set; } = new ObservableCollection<InstalledAppsModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        public InstalledAppsControl()
        {
            InitializeComponent();

            UnInstallCommand.ExecuteRequested += async (sender, args) =>
            {
                InstalledAppsModel installedApps = args.Parameter as InstalledAppsModel;
                if (installedApps is not null)
                {
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
                            ToastNotificationService.Show(NotificationArgs.WinGetUnInstallSuccessfully, installedApps.AppName);

                            // 检测是否需要重启设备完成应用的卸载，如果是，询问用户是否需要重启设备
                            if (unInstallResult.RebootRequired)
                            {
                                ContentDialogResult Result = await ContentDialogHelper.ShowAsync(new RebootDialog(WinGetOptionArgs.UnInstall, installedApps.AppName), this);
                                if (Result is ContentDialogResult.Primary)
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
                                        bool createResult = Kernel32Library.CreateProcess(null, string.Format("{0} {1}", Path.Combine(InfoHelper.SystemDataPath.Windows, "System32", "Shutdown.exe"), "-r -t 120"), IntPtr.Zero, IntPtr.Zero, false, CreateProcessFlags.CREATE_NO_WINDOW, IntPtr.Zero, null, ref RebootStartupInfo, out PROCESS_INFORMATION RebootProcessInformation);

                                        if (createResult)
                                        {
                                            if (RebootProcessInformation.hProcess != IntPtr.Zero) Kernel32Library.CloseHandle(RebootProcessInformation.hProcess);
                                            if (RebootProcessInformation.hThread != IntPtr.Zero) Kernel32Library.CloseHandle(RebootProcessInformation.hThread);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            ToastNotificationService.Show(NotificationArgs.WinGetUnInstallFailed, installedApps.AppName);
                        }
                    }
                    // 操作被用户所取消异常
                    catch (OperationCanceledException e)
                    {
                        LogService.WriteLog(LogType.INFO, "App uninstalling operation canceled.", e);
                        ToastNotificationService.Show(NotificationArgs.WinGetUnInstallFailed, installedApps.AppName);
                    }
                    // 其他异常
                    catch (Exception e)
                    {
                        LogService.WriteLog(LogType.ERROR, "App uninstalling failed.", e);
                        ToastNotificationService.Show(NotificationArgs.WinGetUnInstallFailed, installedApps.AppName);
                    }
                }
            };

            CopyUnInstallTextCommand.ExecuteRequested += (sender, args) =>
            {
                string appId = args.Parameter as string;
                if (appId is not null)
                {
                    string copyContent = string.Format("winget uninstall {0}", appId);
                    CopyPasteHelper.CopyToClipBoard(copyContent);

                    new WinGetCopyNotification(this, WinGetOptionArgs.UnInstall).Show();
                }
            };
        }

        /// <summary>
        /// 本地化应用数量统计信息
        /// </summary>
        public string LocalizeInstalledAppsCountInfo(int count)
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
        /// 初始化已安装应用信息
        /// </summary>
        public async void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (!isInitialized)
            {
                try
                {
                    InstalledAppsManager = WinGetService.CreatePackageManager();
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LogType.ERROR, "Installed apps information initialized failed.", e);
                    return;
                }
                await Task.Delay(500);
                await GetInstalledAppsAsync();
                InitializeData();
                IsInstalledAppsEmpty = MatchResultList.Count is 0;
                IsLoadedCompleted = true;
                isInitialized = true;
            }
        }

        /// <summary>
        /// 更新已安装应用数据
        /// </summary>
        public async void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            MatchResultList = null;
            IsLoadedCompleted = false;
            SearchText = string.Empty;
            await Task.Delay(500);
            await GetInstalledAppsAsync();
            InitializeData();
            IsInstalledAppsEmpty = MatchResultList.Count is 0;
            IsLoadedCompleted = true;
        }

        /// <summary>
        /// 根据输入的内容检索应用
        /// </summary>
        public void OnQuerySubmitted(object sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            InitializeData(true);
        }

        /// <summary>
        /// 文本输入框内容为空时，复原原来的内容
        /// </summary>
        public void OnTextChanged(object sender, AutoSuggestBoxTextChangedEventArgs args)
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

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 加载系统已安装的应用信息
        /// </summary>
        private async Task GetInstalledAppsAsync()
        {
            try
            {
                await Task.Run(async () =>
                {
                    PackageCatalogReference searchCatalogReference = InstalledAppsManager.GetLocalPackageCatalog(LocalPackageCatalog.InstalledPackages);

                    ConnectResult connectResult = await searchCatalogReference.ConnectAsync();
                    PackageCatalog installedCatalog = connectResult.PackageCatalog;

                    if (installedCatalog is not null)
                    {
                        FindPackagesOptions findPackagesOptions = WinGetService.CreateFindPackagesOptions();
                        FindPackagesResult findResult = await installedCatalog.FindPackagesAsync(findPackagesOptions);

                        MatchResultList = findResult.Matches.ToList().Where(item => item.CatalogPackage.InstalledVersion.Publisher != string.Empty).ToList();
                    }
                });
            }
            catch (Exception e)
            {
                LogService.WriteLog(LogType.ERROR, "Get installed apps information failed.", e);
            }
        }

        /// <summary>
        /// 初始化列表数据
        /// </summary>
        private void InitializeData(bool hasSearchText = false)
        {
            lock (InstalledAppsDataListObject)
            {
                InstalledAppsDataList.Clear();
            }

            if (MatchResultList is not null)
            {
                if (hasSearchText)
                {
                    lock (InstalledAppsDataListObject)
                    {
                        foreach (MatchResult matchItem in MatchResultList)
                        {
                            if (matchItem.CatalogPackage.InstalledVersion.DisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                            {
                                InstalledAppsDataList.Add(new InstalledAppsModel()
                                {
                                    AppID = matchItem.CatalogPackage.InstalledVersion.Id,
                                    AppName = string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.DisplayName) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.InstalledVersion.DisplayName,
                                    AppPublisher = string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.Publisher) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.InstalledVersion.Publisher,
                                    AppVersion = string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.Version) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.InstalledVersion.Version,
                                });
                                Task.Delay(1);
                            }
                        }
                    }
                }
                else
                {
                    lock (InstalledAppsDataListObject)
                    {
                        foreach (MatchResult matchItem in MatchResultList)
                        {
                            InstalledAppsDataList.Add(new InstalledAppsModel()
                            {
                                AppID = matchItem.CatalogPackage.InstalledVersion.Id,
                                AppName = string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.DisplayName) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.InstalledVersion.DisplayName,
                                AppPublisher = string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.Publisher) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.InstalledVersion.Publisher,
                                AppVersion = string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.Version) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.InstalledVersion.Version,
                            });
                            Task.Delay(1);
                        }
                    }
                }
            }
        }
    }
}
