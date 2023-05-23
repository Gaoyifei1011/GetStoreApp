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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Controls.WinGet
{
    /// <summary>
    /// WinGet 程序包页面：搜索应用控件视图模型
    /// </summary>
    public sealed class SearchAppsViewModel : ViewModelBase
    {
        private PackageManager SearchAppsManager { get; set; }

        private string cachedSearchText;

        private bool _notSearched = true;

        public bool NotSearched
        {
            get { return _notSearched; }

            set
            {
                _notSearched = value;
                OnPropertyChanged();
            }
        }

        private bool _isSearchCompleted = false;

        public bool IsSearchCompleted
        {
            get { return _isSearchCompleted; }

            set
            {
                _isSearchCompleted = value;
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

        // 安装应用
        public XamlUICommand InstallCommand { get; } = new XamlUICommand();

        // 复制安装命令
        public XamlUICommand CopyInstallTextCommand { get; } = new XamlUICommand();

        private List<MatchResult> MatchResultList;

        public ObservableCollection<SearchAppsModel> SearchAppsDataList { get; set; } = new ObservableCollection<SearchAppsModel>();

        /// <summary>
        /// 初始化搜索应用内容
        /// </summary>
        public void OnLoaded(object sender, RoutedEventArgs args)
        {
            try
            {
                SearchAppsManager = WinGetService.CreatePackageManager();
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// 更新已安装应用数据
        /// </summary>
        public async void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            MatchResultList = null;
            IsSearchCompleted = false;
            await Task.Delay(500);
            if (string.IsNullOrEmpty(cachedSearchText))
            {
                IsSearchCompleted = true;
                return;
            }
            await GetSearchAppsAsync();
            InitializeData();
            IsSearchCompleted = true;
        }

        /// <summary>
        /// 根据输入的内容检索应用
        /// </summary>
        public async void OnQuerySubmitted(object sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            cachedSearchText = SearchText;
            NotSearched = false;
            IsSearchCompleted = false;
            await Task.Delay(500);
            if (string.IsNullOrEmpty(cachedSearchText))
            {
                IsSearchCompleted = true;
                return;
            }
            await GetSearchAppsAsync();
            InitializeData();
            IsSearchCompleted = true;
        }

        public SearchAppsViewModel()
        {
            InstallCommand.ExecuteRequested += async (sender, args) =>
            {
                SearchAppsModel searchApps = args.Parameter as SearchAppsModel;
                if (searchApps is not null)
                {
                    try
                    {
                        foreach (SearchAppsModel searchAppsItem in SearchAppsDataList)
                        {
                            if (searchAppsItem.AppID == searchApps.AppID)
                            {
                                searchAppsItem.IsInstalling = true;
                                break;
                            }
                        }

                        InstallOptions installOptions = WinGetService.CreateInstallOptions();

                        installOptions.PackageInstallMode = PackageInstallMode.Interactive;
                        installOptions.PackageInstallScope = PackageInstallScope.Any;

                        // 更新安装进度
                        Progress<InstallProgress> progressCallBack = new Progress<InstallProgress>((installProgress) =>
                        {
                            switch (installProgress.State)
                            {
                                // 处于等待中状态
                                case PackageInstallProgressState.Queued:
                                    {
                                        break;
                                    }
                                // 处于下载中状态
                                case PackageInstallProgressState.Downloading:
                                    {
                                        break;
                                    }
                                // 处于安装中状态
                                case PackageInstallProgressState.Installing:
                                    {
                                        break;
                                    }
                                // 挂起状态
                                case PackageInstallProgressState.PostInstall:
                                    {
                                        break;
                                    }
                                // 处于安装完成状态
                                case PackageInstallProgressState.Finished:
                                    {
                                        break;
                                    }
                            }
                        });

                        InstallResult installResult = await SearchAppsManager.InstallPackageAsync(MatchResultList.Find(item => item.CatalogPackage.DefaultInstallVersion.Id == searchApps.AppID).CatalogPackage, installOptions).AsTask(progressCallBack);

                        // 获取安装完成后的结果信息
                        if (installResult.Status == InstallResultStatus.Ok)
                        {
                            // 检测是否需要重启设备完成应用的卸载，如果是，询问用户是否需要重启设备
                            if (installResult.RebootRequired)
                            {
                                ContentDialogResult Result = await new RebootDialog(WinGetOptionArgs.UpgradeInstall, searchApps.AppName).ShowAsync();
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
                        }
                        else
                        {

                        }

                        foreach (SearchAppsModel searchAppsItem in SearchAppsDataList)
                        {
                            if (searchAppsItem.AppID == searchApps.AppID)
                            {
                                searchAppsItem.IsInstalling = false;
                                break;
                            }
                        }
                    }
                    // 操作被用户所取消异常
                    catch (OperationCanceledException)
                    {
                    }
                    // 其他异常
                    catch (Exception)
                    {
                    }
                }
            };

            CopyInstallTextCommand.ExecuteRequested += (sender, args) =>
            {
                string appId = args.Parameter as string;
                if (appId is not null)
                {
                    string copyContent = string.Format("winget install {0}", appId);
                    CopyPasteHelper.CopyToClipBoard(copyContent);

                    new WinGetCopyNotification(true, WinGetOptionArgs.SearchInstall).Show();
                }
            };
        }

        /// <summary>
        /// 搜索应用
        /// </summary>
        public async Task GetSearchAppsAsync()
        {
            try
            {
                await Task.Run(async () =>
                {
                    List<PackageCatalogReference> packageCatalogReferences = SearchAppsManager.GetPackageCatalogs().ToList();
                    CreateCompositePackageCatalogOptions createCompositePackageCatalogOptions = WinGetService.CreateCreateCompositePackageCatalogOptions();
                    foreach (PackageCatalogReference catalogReference in packageCatalogReferences)
                    {
                        createCompositePackageCatalogOptions.Catalogs.Add(catalogReference);
                    }
                    PackageCatalogReference catalogRef = SearchAppsManager.CreateCompositePackageCatalog(createCompositePackageCatalogOptions);
                    ConnectResult connectResult = await catalogRef.ConnectAsync();
                    PackageCatalog searchCatalog = connectResult.PackageCatalog;

                    if (searchCatalog is not null)
                    {
                        FindPackagesOptions findPackagesOptions = WinGetService.CreateFindPackagesOptions();
                        PackageMatchFilter nameMatchFilter = WinGetService.CreatePacakgeMatchFilter();
                        // 根据应用的名称寻找符合条件的结果
                        nameMatchFilter.Field = PackageMatchField.Name;
                        nameMatchFilter.Option = PackageFieldMatchOption.ContainsCaseInsensitive;
                        nameMatchFilter.Value = cachedSearchText;
                        findPackagesOptions.Filters.Add(nameMatchFilter);
                        FindPackagesResult findResult = await connectResult.PackageCatalog.FindPackagesAsync(findPackagesOptions);
                        MatchResultList = findResult.Matches.ToList();
                    }
                });
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 初始化列表数据
        /// </summary>
        public void InitializeData()
        {
            SearchAppsDataList.Clear();
            if (MatchResultList is not null)
            {
                foreach (MatchResult matchItem in MatchResultList)
                {
                    if (matchItem.CatalogPackage.DefaultInstallVersion is not null)
                    {
                        SearchAppsDataList.Add(new SearchAppsModel()
                        {
                            AppID = matchItem.CatalogPackage.DefaultInstallVersion.Id,
                            AppName = string.IsNullOrEmpty(matchItem.CatalogPackage.DefaultInstallVersion.DisplayName) || matchItem.CatalogPackage.DefaultInstallVersion.DisplayName.Equals("Unknown", StringComparison.OrdinalIgnoreCase) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.DefaultInstallVersion.DisplayName,
                            AppPublisher = string.IsNullOrEmpty(matchItem.CatalogPackage.DefaultInstallVersion.Publisher) || matchItem.CatalogPackage.DefaultInstallVersion.Publisher.Equals("Unknown", StringComparison.OrdinalIgnoreCase) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.DefaultInstallVersion.Publisher,
                            AppVersion = string.IsNullOrEmpty(matchItem.CatalogPackage.DefaultInstallVersion.Version) || matchItem.CatalogPackage.DefaultInstallVersion.Version.Equals("Unknown", StringComparison.OrdinalIgnoreCase) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.DefaultInstallVersion.Version,
                        });
                    }
                }
            }
        }
    }
}
