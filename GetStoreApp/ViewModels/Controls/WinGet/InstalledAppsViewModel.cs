using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.WinGet;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.WinGet;
using GetStoreApp.UI.Notifications;
using GetStoreApp.ViewModels.Base;
using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
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
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Controls.WinGet
{
    /// <summary>
    /// WinGet 程序包页面：已安装应用控件视图模型
    /// </summary>
    public sealed class InstalledAppsViewModel : ViewModelBase
    {
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

        public InstalledAppsViewModel()
        {
            PropertyChanged += OnPropertyChanged;
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
                        if (unInstallResult.Status == UninstallResultStatus.Ok)
                        {
                            // 检测是否需要重启设备完成应用的卸载，如果是，询问用户是否需要重启设备
                            if (unInstallResult.RebootRequired)
                            {
                                ContentDialogResult Result = await new RebootDialog(installedApps.AppName).ShowAsync();
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
                                    RebootStartupInfo.wShowWindow = 0;
                                    RebootStartupInfo.cbReserved2 = 0;
                                    RebootStartupInfo.lpReserved2 = IntPtr.Zero;

                                    RebootStartupInfo.cb = Marshal.SizeOf(typeof(STARTUPINFO));
                                    Kernel32Library.CreateProcess(null, string.Format("{0} {1}", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32", "Shutdown.exe"), "-r -t 120"), IntPtr.Zero, IntPtr.Zero, false, CreateProcessFlags.CREATE_NO_WINDOW, IntPtr.Zero, null, ref RebootStartupInfo, out PROCESS_INFORMATION RebootProcessInformation);

                                    Kernel32Library.CloseHandle(RebootProcessInformation.hProcess);
                                    Kernel32Library.CloseHandle(RebootProcessInformation.hThread);
                                }
                            }
                        }
                        else
                        {
                            await new UnInstallFailedDialog(installedApps.AppName).ShowAsync();
                        }
                    }
                    // 操作被用户所取消异常
                    catch (OperationCanceledException)
                    {
                        await new UnInstallFailedDialog(installedApps.AppName).ShowAsync();
                    }
                    // 其他异常
                    catch (Exception)
                    {
                        await new UnInstallFailedDialog(installedApps.AppName).ShowAsync();
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

                    new WinGetCopyNotification(true, WinGetCopyOptionsArgs.UnInstall).Show();
                }
            };
        }

        ~InstalledAppsViewModel()
        {
            PropertyChanged -= OnPropertyChanged;
        }

        /// <summary>
        /// 初始化已安装应用信息
        /// </summary>
        public async void OnLoaded(object sender, RoutedEventArgs args)
        {
            try
            {
                InstalledAppsManager = WinGetService.CreatePackageManager();
            }
            catch (Exception)
            {
                return;
            }
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
        public void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(SearchText))
            {
                if (SearchText == string.Empty && MatchResultList is not null)
                {
                    InitializeData();
                }
            }
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
            catch (Exception) { }
        }

        /// <summary>
        /// 初始化列表数据
        /// </summary>
        private void InitializeData([Optional, DefaultValue(false)] bool hasSearchText)
        {
            InstalledAppsDataList.Clear();
            if (MatchResultList is not null)
            {
                if (hasSearchText)
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
                        }
                    }
                }
                else
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
                    }
                }
            }
        }
    }
}
