using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Controls.Extensions;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.WinGet;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.WinGet;
using GetStoreApp.UI.Notifications;
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

namespace GetStoreApp.UI.Controls.WinGet
{
    /// <summary>
    /// WinGet 程序包页面：搜索应用控件
    /// </summary>
    public sealed partial class SearchAppsControl : Grid, INotifyPropertyChanged
    {
        private PackageManager SearchAppsManager { get; set; }

        private readonly object SearchAppsDataListObject = new object();

        internal WinGetPage WinGetInstance;

        private string cachedSearchText;

        private bool isInitialized = false;

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

        // 使用命令安装
        public XamlUICommand InstallWithCmdCommand { get; } = new XamlUICommand();

        private List<MatchResult> MatchResultList;

        public ObservableCollection<SearchAppsModel> SearchAppsDataList { get; set; } = new ObservableCollection<SearchAppsModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        public SearchAppsControl()
        {
            InitializeComponent();

            InstallCommand.ExecuteRequested += async (sender, args) =>
            {
                SearchAppsModel searchApps = args.Parameter as SearchAppsModel;
                if (searchApps is not null)
                {
                    try
                    {
                        lock (SearchAppsDataListObject)
                        {
                            // 禁用当前应用的可安装状态
                            foreach (SearchAppsModel searchAppsItem in SearchAppsDataList)
                            {
                                if (searchAppsItem.AppID == searchApps.AppID)
                                {
                                    searchAppsItem.IsInstalling = true;
                                    break;
                                }
                            }
                        }

                        InstallOptions installOptions = WinGetService.CreateInstallOptions();

                        installOptions.PackageInstallMode = (PackageInstallMode)Enum.Parse(typeof(PackageInstallMode), WinGetConfigService.WinGetInstallMode.SelectedValue);
                        installOptions.PackageInstallScope = PackageInstallScope.Any;

                        // 更新安装进度
                        Progress<InstallProgress> progressCallBack = new Progress<InstallProgress>((installProgress) =>
                        {
                            switch (installProgress.State)
                            {
                                // 处于等待中状态
                                case PackageInstallProgressState.Queued:
                                    {
                                        lock (WinGetInstance.InstallingAppsObject)
                                        {
                                            foreach (InstallingAppsModel installingItem in WinGetInstance.InstallingAppsList)
                                            {
                                                if (installingItem.AppID == searchApps.AppID)
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
                                        lock (WinGetInstance.InstallingAppsObject)
                                        {
                                            foreach (InstallingAppsModel installingItem in WinGetInstance.InstallingAppsList)
                                            {
                                                if (installingItem.AppID == searchApps.AppID)
                                                {
                                                    installingItem.InstallProgressState = PackageInstallProgressState.Downloading;
                                                    installingItem.DownloadProgress = Math.Round(installProgress.DownloadProgress * 100, 2); installingItem.DownloadedFileSize = Convert.ToString(FileSizeHelper.ConvertFileSizeToString(installProgress.BytesDownloaded));
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
                                        lock (WinGetInstance.InstallingAppsObject)
                                        {
                                            foreach (InstallingAppsModel installingItem in WinGetInstance.InstallingAppsList)
                                            {
                                                if (installingItem.AppID == searchApps.AppID)
                                                {
                                                    installingItem.InstallProgressState = PackageInstallProgressState.Installing;
                                                    installingItem.DownloadProgress = 100;
                                                    break;
                                                }
                                            }
                                        }
                                        break;
                                    }
                                // 安装完成后等待其他操作状态
                                case PackageInstallProgressState.PostInstall:
                                    {
                                        lock (WinGetInstance.InstallingAppsObject)
                                        {
                                            foreach (InstallingAppsModel installingItem in WinGetInstance.InstallingAppsList)
                                            {
                                                if (installingItem.AppID == searchApps.AppID)
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
                                        lock (WinGetInstance.InstallingAppsObject)
                                        {
                                            foreach (InstallingAppsModel installingItem in WinGetInstance.InstallingAppsList)
                                            {
                                                if (installingItem.AppID == searchApps.AppID)
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
                        CancellationTokenSource installTokenSource = new CancellationTokenSource();

                        // 添加任务
                        lock (WinGetInstance.InstallingAppsObject)
                        {
                            WinGetInstance.InstallingAppsList.Add(new InstallingAppsModel()
                            {
                                AppID = searchApps.AppID,
                                AppName = searchApps.AppName,
                                DownloadProgress = 0,
                                InstallProgressState = PackageInstallProgressState.Queued,
                                DownloadedFileSize = FileSizeHelper.ConvertFileSizeToString(0),
                                TotalFileSize = FileSizeHelper.ConvertFileSizeToString(0)
                            });
                            WinGetInstance.InstallingStateDict.Add(searchApps.AppID, installTokenSource);
                        }

                        InstallResult installResult = await SearchAppsManager.InstallPackageAsync(MatchResultList.Find(item => item.CatalogPackage.DefaultInstallVersion.Id == searchApps.AppID).CatalogPackage, installOptions).AsTask(installTokenSource.Token, progressCallBack);

                        // 获取安装完成后的结果信息
                        if (installResult.Status is InstallResultStatus.Ok)
                        {
                            ToastNotificationService.Show(NotificationArgs.WinGetInstallSuccessfully, searchApps.AppName);

                            // 检测是否需要重启设备完成应用的卸载，如果是，询问用户是否需要重启设备
                            if (installResult.RebootRequired)
                            {
                                ContentDialogResult Result = await ContentDialogHelper.ShowAsync(new RebootDialog(WinGetOptionArgs.UpgradeInstall, searchApps.AppName), this);
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
                            ToastNotificationService.Show(NotificationArgs.WinGetInstallFailed, searchApps.AppName, searchApps.AppID);
                        }

                        lock (SearchAppsDataListObject)
                        {
                            // 应用安装失败，将当前任务状态修改为可安装状态
                            foreach (SearchAppsModel searchAppsItem in SearchAppsDataList)
                            {
                                if (searchAppsItem.AppID == searchApps.AppID)
                                {
                                    searchAppsItem.IsInstalling = false;
                                    break;
                                }
                            }
                        }

                        // 完成任务后从任务管理中删除任务
                        lock (WinGetInstance.InstallingAppsObject)
                        {
                            foreach (InstallingAppsModel installingAppsItem in WinGetInstance.InstallingAppsList)
                            {
                                if (installingAppsItem.AppID == searchApps.AppID)
                                {
                                    WinGetInstance.InstallingAppsList.Remove(installingAppsItem);
                                    break;
                                }
                            }
                            WinGetInstance.InstallingStateDict.Remove(searchApps.AppID);
                        }
                    }
                    // 操作被用户所取消异常
                    catch (OperationCanceledException e)
                    {
                        LogService.WriteLog(LogType.INFO, "App installing operation canceled.", e);

                        lock (SearchAppsDataListObject)
                        {
                            // 应用安装失败，将当前任务状态修改为可安装状态
                            foreach (SearchAppsModel searchAppsItem in SearchAppsDataList)
                            {
                                if (searchAppsItem.AppID == searchApps.AppID)
                                {
                                    searchAppsItem.IsInstalling = false;
                                    break;
                                }
                            }
                        }

                        // 完成任务后从任务管理中删除任务
                        lock (WinGetInstance.InstallingAppsObject)
                        {
                            foreach (InstallingAppsModel installingAppsItem in WinGetInstance.InstallingAppsList)
                            {
                                if (installingAppsItem.AppID == searchApps.AppID)
                                {
                                    WinGetInstance.InstallingAppsList.Remove(installingAppsItem);
                                    break;
                                }
                            }
                            WinGetInstance.InstallingStateDict.Remove(searchApps.AppID);
                        }
                    }
                    // 其他异常
                    catch (Exception e)
                    {
                        LogService.WriteLog(LogType.ERROR, "App installing failed.", e);

                        lock (SearchAppsDataListObject)
                        {
                            // 应用安装失败，将当前任务状态修改为可安装状态
                            foreach (SearchAppsModel searchAppsItem in SearchAppsDataList)
                            {
                                if (searchAppsItem.AppID == searchApps.AppID)
                                {
                                    searchAppsItem.IsInstalling = false;
                                    break;
                                }
                            }
                        }

                        // 完成任务后从任务管理中删除任务
                        lock (WinGetInstance.InstallingAppsObject)
                        {
                            foreach (InstallingAppsModel installingAppsItem in WinGetInstance.InstallingAppsList)
                            {
                                if (installingAppsItem.AppID == searchApps.AppID)
                                {
                                    WinGetInstance.InstallingAppsList.Remove(installingAppsItem);
                                    break;
                                }
                            }
                            WinGetInstance.InstallingStateDict.Remove(searchApps.AppID);
                        }

                        ToastNotificationService.Show(NotificationArgs.WinGetInstallFailed, searchApps.AppName, searchApps.AppID);
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

                    new WinGetCopyNotification(this, WinGetOptionArgs.SearchInstall).Show();
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
        /// 本地化应用数量统计信息
        /// </summary>
        public string LocalizeSearchAppsCountInfo(int count)
        {
            if (count is 0)
            {
                return ResourceService.GetLocalized("WinGet/SearchedAppsCountEmpty");
            }
            else
            {
                return string.Format(ResourceService.GetLocalized("WinGet/SearchedAppsCountInfo"), count);
            }
        }

        public bool IsSearchBoxEnabled(bool notSearched, bool isSearchCompleted)
        {
            if (notSearched)
            {
                return true;
            }
            else
            {
                if (isSearchCompleted)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 初始化搜索应用内容
        /// </summary>
        public void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (!isInitialized)
            {
                try
                {
                    SearchAppsManager = WinGetService.CreatePackageManager();
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LogType.ERROR, "Search apps information initialized failed.", e);
                    return;
                }
                finally
                {
                    isInitialized = true;
                }
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
            catch (Exception e)
            {
                LogService.WriteLog(LogType.WARNING, "Get search apps information failed.", e);
            }
        }

        /// <summary>
        /// 初始化列表数据
        /// </summary>
        public void InitializeData()
        {
            lock (SearchAppsDataListObject)
            {
                SearchAppsDataList.Clear();
            }

            if (MatchResultList is not null)
            {
                lock (SearchAppsDataListObject)
                {
                    foreach (MatchResult matchItem in MatchResultList)
                    {
                        if (matchItem.CatalogPackage.DefaultInstallVersion is not null)
                        {
                            bool isInstalling = false;
                            foreach (InstallingAppsModel installingAppsItem in WinGetInstance.InstallingAppsList)
                            {
                                if (matchItem.CatalogPackage.DefaultInstallVersion.Id == installingAppsItem.AppID)
                                {
                                    isInstalling = true;
                                    break;
                                }
                            }
                            SearchAppsDataList.Add(new SearchAppsModel()
                            {
                                AppID = matchItem.CatalogPackage.DefaultInstallVersion.Id,
                                AppName = string.IsNullOrEmpty(matchItem.CatalogPackage.DefaultInstallVersion.DisplayName) || matchItem.CatalogPackage.DefaultInstallVersion.DisplayName.Equals("Unknown", StringComparison.OrdinalIgnoreCase) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.DefaultInstallVersion.DisplayName,
                                AppPublisher = string.IsNullOrEmpty(matchItem.CatalogPackage.DefaultInstallVersion.Publisher) || matchItem.CatalogPackage.DefaultInstallVersion.Publisher.Equals("Unknown", StringComparison.OrdinalIgnoreCase) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.DefaultInstallVersion.Publisher,
                                AppVersion = string.IsNullOrEmpty(matchItem.CatalogPackage.DefaultInstallVersion.Version) || matchItem.CatalogPackage.DefaultInstallVersion.Version.Equals("Unknown", StringComparison.OrdinalIgnoreCase) ? ResourceService.GetLocalized("WinGet/Unknown") : matchItem.CatalogPackage.DefaultInstallVersion.Version,
                                IsInstalling = isInstalling,
                            });
                            Task.Delay(1);
                        }
                    }
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
    }
}
