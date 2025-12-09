using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Helpers.WinGet;
using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Settings;
using GetStoreApp.Views.Dialogs;
using GetStoreApp.Views.Windows;
using Microsoft.Management.Deployment;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;
using Windows.System;
using WinRT;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// WinGet 搜索应用界面
    /// </summary>
    public sealed partial class WinGetSearchPage : Page, INotifyPropertyChanged
    {
        private readonly string NotAvailableString = ResourceService.GetLocalized("WinGetSearch/NotAvailable");
        private readonly string SearchAppsEmptyDescriptionString = ResourceService.GetLocalized("WinGetSearch/SearchAppsEmptyDescription");
        private readonly string SearchAppsFailedString = ResourceService.GetLocalized("WinGetSearch/SearchAppsFailed");
        private readonly string SearchCatalogReferenceFailedString = ResourceService.GetLocalized("WinGetSearch/SearchCatalogReferenceFailed");
        private readonly string SearchedAppsCountInfoString = ResourceService.GetLocalized("WinGetSearch/SearchedAppsCountInfo");
        private readonly string SearchFindAppsFailedString = ResourceService.GetLocalized("WinGetSearch/SearchFindAppsFailed");
        private readonly string SearchNotSelectSourceString = ResourceService.GetLocalized("WinGetSearch/SearchNotSelectSource");
        private string cachedSearchText;
        private WinGetPage WinGetPageInstance;

        private string _searchText = string.Empty;

        public string SearchText
        {
            get { return _searchText; }

            set
            {
                if (!string.Equals(_searchText, value))
                {
                    _searchText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchText)));
                }
            }
        }

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

        private SearchAppsResultKind _searchAppsResultKind = SearchAppsResultKind.NotSearch;

        public SearchAppsResultKind SearchAppsResultKind
        {
            get { return _searchAppsResultKind; }

            set
            {
                if (!Equals(_searchAppsResultKind, value))
                {
                    _searchAppsResultKind = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchAppsResultKind)));
                }
            }
        }

        private PackageMatchField _packageMatchField = PackageMatchField.Name;

        public PackageMatchField PackageMatchField
        {
            get { return _packageMatchField; }

            set
            {
                if (!Equals(_packageMatchField, value))
                {
                    _packageMatchField = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PackageMatchField)));
                }
            }
        }

        private PackageFieldMatchOption _packageFieldMatchOption = PackageFieldMatchOption.EqualsCaseInsensitive;

        public PackageFieldMatchOption PackageFieldMatchOption
        {
            get { return _packageFieldMatchOption; }

            set
            {
                if (!Equals(_packageFieldMatchOption, value))
                {
                    _packageFieldMatchOption = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PackageFieldMatchOption)));
                }
            }
        }

        private string _searchFailedContent;

        public string SearchFailedContent
        {
            get { return _searchFailedContent; }

            set
            {
                if (!string.Equals(_searchFailedContent, value))
                {
                    _searchFailedContent = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchFailedContent)));
                }
            }
        }

        private ObservableCollection<SearchAppsModel> SearchAppsCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public WinGetSearchPage()
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

            if (args.Parameter is WinGetPage winGetPage && WinGetPageInstance is null)
            {
                WinGetPageInstance = winGetPage;
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 下载应用
        /// </summary>
        private async void OnDownloadExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is SearchAppsModel searchApps && searchApps.CatalogPackage.DefaultInstallVersion is not null && WinGetPageInstance is not null)
            {
                DownloadOptions downloadOptions = await Task.Run(WinGetFactoryHelper.CreateDownloadOptions);

                await WinGetPageInstance.AddTaskAsync(new PackageOperationModel()
                {
                    PackageOperationKind = PackageOperationKind.Download,
                    AppID = searchApps.AppID,
                    AppName = searchApps.AppName,
                    AppVersion = searchApps.CatalogPackage.DefaultInstallVersion.Version,
                    PackagePath = WinGetConfigService.DefaultDownloadFolder,
                    PackageOperationProgress = 0,
                    PackageDownloadProgressState = PackageDownloadProgressState.Queued,
                    PackageVersionId = null,
                    DownloadedFileSize = VolumeSizeHelper.ConvertVolumeSizeToString(0),
                    TotalFileSize = VolumeSizeHelper.ConvertVolumeSizeToString(0),
                    PackageDownloadProgress = null,
                    SearchApps = searchApps,
                    DownloadOptions = downloadOptions
                });
            }
        }

        /// <summary>
        /// 安装应用
        /// </summary>
        private async void OnInstallExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is SearchAppsModel searchApps && WinGetPageInstance is not null)
            {
                InstallOptions installOptions = await Task.Run(WinGetFactoryHelper.CreateInstallOptions);

                await WinGetPageInstance.AddTaskAsync(new PackageOperationModel()
                {
                    PackageOperationKind = PackageOperationKind.Install,
                    AppID = searchApps.AppID,
                    AppName = searchApps.AppName,
                    AppVersion = searchApps.CatalogPackage.DefaultInstallVersion.Version,
                    PackagePath = Path.Combine(Path.GetTempPath(), "WinGet"),
                    PackageOperationProgress = 0,
                    PackageInstallProgressState = PackageInstallProgressState.Queued,
                    PackageVersionId = null,
                    DownloadedFileSize = VolumeSizeHelper.ConvertVolumeSizeToString(0),
                    TotalFileSize = VolumeSizeHelper.ConvertVolumeSizeToString(0),
                    PackageInstallProgress = null,
                    SearchApps = searchApps,
                    InstallOptions = installOptions
                });
            }
        }

        /// <summary>
        /// 修复应用
        /// </summary>
        private async void OnRepairExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is SearchAppsModel searchApps && WinGetPageInstance is not null)
            {
                RepairOptions repairOptions = await Task.Run(WinGetFactoryHelper.CreateRepairOptions);

                await WinGetPageInstance.AddTaskAsync(new PackageOperationModel()
                {
                    PackageOperationKind = PackageOperationKind.Repair,
                    AppID = searchApps.AppID,
                    AppName = searchApps.AppName,
                    AppVersion = searchApps.CatalogPackage.DefaultInstallVersion.Version,
                    PackagePath = Path.Combine(Path.GetTempPath(), "WinGet"),
                    PackageOperationProgress = 0,
                    PackageRepairProgressState = PackageRepairProgressState.Queued,
                    PackageVersionId = null,
                    DownloadedFileSize = VolumeSizeHelper.ConvertVolumeSizeToString(0),
                    TotalFileSize = VolumeSizeHelper.ConvertVolumeSizeToString(0),
                    PackageRepairProgress = null,
                    SearchApps = searchApps,
                    RepairOptions = repairOptions
                });
            }
        }

        /// <summary>
        /// 查看版本信息
        /// </summary>

        private async void OnViewVersionInfoExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is SearchAppsModel searchApps && WinGetPageInstance is not null)
            {
                await MainWindow.Current.ShowDialogAsync(new WinGetAppsVersionDialog(WinGetOperationKind.VersionInfo, WinGetPageInstance, searchApps));
            }
        }

        #endregion 第二部分：XamlUICommand 命令调用时挂载的事件

        #region 第三部分：WinGet 搜索应用界面——挂载的事件

        /// <summary>
        /// 根据排序方式对列表进行排序
        /// </summary>
        private void OnSortWayClicked(object sender, RoutedEventArgs args)
        {
            if (sender.As<RadioMenuFlyoutItem>().Tag is string increase && SearchAppsResultKind is SearchAppsResultKind.Successfully)
            {
                IsIncrease = Convert.ToBoolean(increase);
                SearchAppsResultKind = SearchAppsResultKind.Searching;
                List<SearchAppsModel> searchAppsList = [.. SearchAppsCollection];
                SearchAppsCollection.Clear();
                if (IsIncrease)
                {
                    searchAppsList.Sort((item1, item2) => item1.AppName.CompareTo(item2.AppName));
                }
                else
                {
                    searchAppsList.Sort((item1, item2) => item2.AppName.CompareTo(item1.AppName));
                }

                foreach (SearchAppsModel searchAppsItem in searchAppsList)
                {
                    SearchAppsCollection.Add(searchAppsItem);
                }
                SearchAppsResultKind = SearchAppsResultKind.Successfully;
            }
        }

        /// <summary>
        /// 更新应用搜索过滤方式
        /// </summary>
        private void OnPackageMatchFieldClicked(object sender, RoutedEventArgs args)
        {
            if (sender.As<RadioMenuFlyoutItem>().Tag.As<PackageMatchField>() is PackageMatchField packageMatchField)
            {
                PackageMatchField = packageMatchField;
            }
        }

        /// <summary>
        /// 更新应用搜索过滤规则
        /// </summary>
        private void OnPackageFieldMatchOptionClicked(object sender, RoutedEventArgs args)
        {
            if (sender.As<RadioMenuFlyoutItem>().Tag.As<PackageFieldMatchOption>() is PackageFieldMatchOption packageFieldMatchOption)
            {
                PackageFieldMatchOption = packageFieldMatchOption;
            }
        }

        /// <summary>
        /// 打开任务管理
        /// </summary>

        private void OnTaskManagerClicked(object sender, RoutedEventArgs args)
        {
            WinGetPageInstance?.ShowTaskManager();
        }

        /// <summary>
        /// 打开临时下载目录
        /// </summary>
        private void OnOpenTempFolderClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                string winGetTempPath = Path.Combine(Path.GetTempPath(), "WinGet");
                if (Directory.Exists(winGetTempPath))
                {
                    await Launcher.LaunchFolderPathAsync(winGetTempPath);
                }
                else
                {
                    await Launcher.LaunchFolderPathAsync(Path.GetTempPath());
                }
            });
        }

        /// <summary>
        /// 了解 WinGet 程序包具体的使用说明
        /// </summary>
        private void OnUseInstructionClicked(object sender, RoutedEventArgs args)
        {
            WinGetPageInstance?.ShowUseInstruction();
        }

        /// <summary>
        /// 刷新搜索应用数据
        /// </summary>
        private async void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            SearchAppsResultKind = SearchAppsResultKind.Searching;
            SearchAppsCollection.Clear();

            if (string.IsNullOrEmpty(cachedSearchText))
            {
                SearchAppsResultKind = SearchAppsResultKind.Failed;
                SearchFailedContent = SearchAppsEmptyDescriptionString;
            }
            else
            {
                await InitializeSearchAppsDataAsync();
            }
        }

        /// <summary>
        /// 配置 WinGet 数据源
        /// </summary>
        private void OnDataSourceSettingsClicked(object sender, RoutedEventArgs args)
        {
            MainWindow.Current.NavigateTo(typeof(SettingsPage), AppNaviagtionArgs.WinGetDataSource);
        }

        /// <summary>
        /// 根据输入的内容检索应用
        /// </summary>
        private async void OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrEmpty(SearchText))
            {
                cachedSearchText = SearchText;
                SearchAppsResultKind = SearchAppsResultKind.Searching;
                SearchAppsCollection.Clear();
                await InitializeSearchAppsDataAsync();
            }
        }

        /// <summary>
        /// 文本输入框内容为空时，复原原来的内容
        /// </summary>
        private void OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            SearchText = sender.Text;
        }

        #endregion 第三部分：WinGet 搜索应用界面——挂载的事件

        /// <summary>
        /// 获取设置中选择的 WinGet 数据源
        /// </summary>

        private PackageCatalogReference GetPackageCatalogReference(PackageManager packageManager)
        {
            PackageCatalogReference packageCatalogReference = null;

            try
            {
                KeyValuePair<string, bool> winGetDataSourceName = WinGetConfigService.GetWinGetDataSourceName();

                if (!Equals(winGetDataSourceName, default))
                {
                    // 使用内置源
                    if (winGetDataSourceName.Value)
                    {
                        foreach (KeyValuePair<string, PredefinedPackageCatalog> predefinedPackageCatalog in WinGetConfigService.PredefinedPackageCatalogList)
                        {
                            if (string.Equals(winGetDataSourceName.Key, predefinedPackageCatalog.Key))
                            {
                                packageCatalogReference = packageManager.GetPredefinedPackageCatalog(predefinedPackageCatalog.Value);
                                break;
                            }
                        }
                    }
                    // 使用自定义源
                    else
                    {
                        packageCatalogReference = packageManager.GetPackageCatalogByName(winGetDataSourceName.Key);
                    }
                }

                return packageCatalogReference;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetSearchPage), nameof(GetPackageCatalogReference), 1, e);
                return packageCatalogReference;
            }
        }

        /// <summary>
        /// 初始化搜索应用数据
        /// </summary>
        private async Task InitializeSearchAppsDataAsync()
        {
            PackageManager packageManager = await Task.Run(WinGetFactoryHelper.CreatePackageManager);

            PackageCatalogReference packageCatalogReference = await Task.Run(() =>
            {
                return GetPackageCatalogReference(packageManager);
            });

            if (packageCatalogReference is not null)
            {
                (ConnectResult connectResult, FindPackagesResult findPackagesResult, List<SearchAppsModel> searchAppsList) = await Task.Run(() =>
                {
                    return SearchAppsAsync(packageCatalogReference);
                });

                if (connectResult.Status is ConnectResultStatus.Ok)
                {
                    if (findPackagesResult.Status is FindPackagesResultStatus.Ok)
                    {
                        if (searchAppsList.Count is 0)
                        {
                            SearchAppsResultKind = SearchAppsResultKind.Failed;
                            SearchFailedContent = SearchAppsEmptyDescriptionString;
                        }
                        else
                        {
                            foreach (SearchAppsModel searchAppsItem in searchAppsList)
                            {
                                SearchAppsCollection.Add(searchAppsItem);
                            }

                            SearchAppsResultKind = SearchAppsResultKind.Successfully;
                        }
                    }
                    else
                    {
                        SearchAppsResultKind = SearchAppsResultKind.Failed;
                        SearchFailedContent = string.Format(SearchAppsFailedString, SearchFindAppsFailedString, findPackagesResult.ExtendedErrorCode is not null ? "0x" + Convert.ToString(findPackagesResult.ExtendedErrorCode.HResult, 16).ToUpperInvariant() : NotAvailableString);
                    }
                }
                else
                {
                    SearchAppsResultKind = SearchAppsResultKind.Failed;
                    SearchFailedContent = string.Format(SearchAppsFailedString, SearchCatalogReferenceFailedString, findPackagesResult.ExtendedErrorCode is not null ? "0x" + Convert.ToString(findPackagesResult.ExtendedErrorCode.HResult, 16).ToUpperInvariant() : NotAvailableString);
                }
            }
            else
            {
                SearchAppsResultKind = SearchAppsResultKind.Failed;
                SearchFailedContent = SearchNotSelectSourceString;
            }
        }

        /// <summary>
        /// 搜索应用
        /// </summary>
        private async Task<(ConnectResult, FindPackagesResult, List<SearchAppsModel>)> SearchAppsAsync(PackageCatalogReference packageCatalogReference)
        {
            (ConnectResult connectResult, FindPackagesResult findPackagesResult, List<SearchAppsModel> searchAppsList) searchAppsResult = ValueTuple.Create<ConnectResult, FindPackagesResult, List<SearchAppsModel>>(null, null, null);

            try
            {
                ConnectResult connectResult = await packageCatalogReference.ConnectAsync();
                searchAppsResult.connectResult = connectResult;

                if (connectResult is not null && connectResult.Status is ConnectResultStatus.Ok)
                {
                    FindPackagesOptions findPackagesOptions = WinGetFactoryHelper.CreateFindPackagesOptions();
                    PackageMatchFilter packageMatchFilter = WinGetFactoryHelper.CreatePackageMatchFilter();
                    packageMatchFilter.Field = PackageMatchField;
                    packageMatchFilter.Option = PackageFieldMatchOption;
                    packageMatchFilter.Value = cachedSearchText;

                    findPackagesOptions.Filters.Add(packageMatchFilter);
                    FindPackagesResult findPackagesResult = await connectResult.PackageCatalog.FindPackagesAsync(findPackagesOptions);
                    searchAppsResult.findPackagesResult = findPackagesResult;

                    if (findPackagesResult is not null && findPackagesResult.Status is FindPackagesResultStatus.Ok)
                    {
                        List<SearchAppsModel> searchAppsList = [];

                        int count = findPackagesResult.Matches.Count;
                        for (int index = 0; index < findPackagesResult.Matches.Count; index++)
                        {
                            MatchResult matchItem = findPackagesResult.Matches[index];

                            if (matchItem.CatalogPackage is CatalogPackage catalogPackage && catalogPackage.Id is not null)
                            {
                                searchAppsList.Add(new SearchAppsModel()
                                {
                                    AppID = catalogPackage.Id,
                                    AppName = string.IsNullOrEmpty(catalogPackage.Name) || string.Equals(catalogPackage.Name, "N/A", StringComparison.OrdinalIgnoreCase) ? NotAvailableString : catalogPackage.Name,
                                    CatalogPackage = matchItem.CatalogPackage,
                                });
                            }
                        }

                        if (IsIncrease)
                        {
                            searchAppsList.Sort((item1, item2) => item1.AppName.CompareTo(item2.AppName));
                        }
                        else
                        {
                            searchAppsList.Sort((item1, item2) => item2.AppName.CompareTo(item1.AppName));
                        }

                        searchAppsResult.searchAppsList = searchAppsList;
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetSearchPage), nameof(SearchAppsAsync), 1, e);
            }

            return searchAppsResult;
        }

        /// <summary>
        /// 获取搜索应用是否成功
        /// </summary>
        public Visibility GetSearchAppsSuccessfullyState(SearchAppsResultKind searchAppsResultKind, bool isSuccessfully)
        {
            return isSuccessfully ? Equals(searchAppsResultKind, SearchAppsResultKind.Successfully) ? Visibility.Visible : Visibility.Collapsed : !Equals(searchAppsResultKind, SearchAppsResultKind.Successfully) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查搜索应用是否成功
        /// </summary>
        public Visibility CheckSearchAppsState(SearchAppsResultKind searchAppsResultKind, SearchAppsResultKind comparedSearchAppsResultKind)
        {
            return Equals(searchAppsResultKind, comparedSearchAppsResultKind) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取搜索框是否可进行搜索
        /// </summary>

        public bool GetSearchBoxEnabled(SearchAppsResultKind searchAppsResultKind)
        {
            return searchAppsResultKind is not SearchAppsResultKind.Searching;
        }

        /// <summary>
        /// 获取搜索框是否可进行搜索
        /// </summary>

        public bool GetSearchRefreshEnabled(SearchAppsResultKind searchAppsResultKind)
        {
            return !(Equals(searchAppsResultKind, SearchAppsResultKind.Searching) || Equals(searchAppsResultKind, SearchAppsResultKind.NotSearch));
        }
    }
}
