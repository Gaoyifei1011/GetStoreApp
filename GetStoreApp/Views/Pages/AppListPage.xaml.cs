using GetStoreApp.Extensions.DataType.Classes;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models.Controls.AppManager;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.AppNotifications.Builder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Diagnostics;
using Windows.Management.Core;
using Windows.Management.Deployment;
using Windows.Storage;
using Windows.System;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 应用管理列表页
    /// </summary>
    public sealed partial class AppListPage : Page, INotifyPropertyChanged
    {
        private readonly string PackageEmptyDescription = ResourceService.GetLocalized("AppManager/PackageEmptyDescription");
        private readonly string PackageEmptyWithConditionDescription = ResourceService.GetLocalized("AppManager/PackageEmptyWithConditionDescription");
        private readonly string Unknown = ResourceService.GetLocalized("AppManager/Unknown");
        private readonly string Yes = ResourceService.GetLocalized("AppManager/Yes");
        private readonly string No = ResourceService.GetLocalized("AppManager/No");
        private readonly string PackageCountInfo = ResourceService.GetLocalized("AppManager/PackageCountInfo");
        private bool isInitialized;
        private bool needToRefreshData;
        private PackageManager packageManager;

        private string _searchText = string.Empty;

        public string SearchText
        {
            get { return _searchText; }

            set
            {
                if (!Equals(_searchText, value))
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

        private AppManagerResultKind _appManagerResultKind;

        public AppManagerResultKind AppManagerResultKind
        {
            get { return _appManagerResultKind; }

            set
            {
                if (!Equals(_appManagerResultKind, value))
                {
                    _appManagerResultKind = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AppManagerResultKind)));
                }
            }
        }

        private bool _isAppFramework;

        public bool IsAppFramework
        {
            get { return _isAppFramework; }

            set
            {
                if (!Equals(_isAppFramework, value))
                {
                    _isAppFramework = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAppFramework)));
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

        private bool _isStoreSignatureSelected = true;

        public bool IsStoreSignatureSelected
        {
            get { return _isStoreSignatureSelected; }

            set
            {
                if (!Equals(_isStoreSignatureSelected, value))
                {
                    _isStoreSignatureSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsStoreSignatureSelected)));
                }
            }
        }

        private bool _isSystemSignatureSelected;

        public bool IsSystemSignatureSelected
        {
            get { return _isSystemSignatureSelected; }

            set
            {
                if (!Equals(_isSystemSignatureSelected, value))
                {
                    _isSystemSignatureSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSystemSignatureSelected)));
                }
            }
        }

        private bool _isEnterpriseSignatureSelected;

        public bool IsEnterpriseSignatureSelected
        {
            get { return _isEnterpriseSignatureSelected; }

            set
            {
                if (!Equals(_isEnterpriseSignatureSelected, value))
                {
                    _isEnterpriseSignatureSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnterpriseSignatureSelected)));
                }
            }
        }

        private bool _isDeveloperSignatureSelected;

        public bool IsDeveloperSignatureSelected
        {
            get { return _isDeveloperSignatureSelected; }

            set
            {
                if (!Equals(_isDeveloperSignatureSelected, value))
                {
                    _isDeveloperSignatureSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDeveloperSignatureSelected)));
                }
            }
        }

        private bool _isNoneSignatureSelected;

        public bool IsNoneSignatureSelected
        {
            get { return _isNoneSignatureSelected; }

            set
            {
                if (!Equals(_isNoneSignatureSelected, value))
                {
                    _isNoneSignatureSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsNoneSignatureSelected)));
                }
            }
        }

        private string _appManagerFailedContent;

        public string AppManagerFailedContent
        {
            get { return _appManagerFailedContent; }

            set
            {
                if (!Equals(_appManagerFailedContent, value))
                {
                    _appManagerFailedContent = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AppManagerFailedContent)));
                }
            }
        }

        private List<PackageModel> AppManagerList { get; } = [];

        private ObservableCollection<PackageModel> AppManagerCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public AppListPage()
        {
            InitializeComponent();
        }

        #region 第一部分：重写父类事件

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (!isInitialized)
            {
                isInitialized = true;

                packageManager = new();
                AppManagerResultKind = AppManagerResultKind.Loading;
                AppManagerList.Clear();
                AppManagerCollection.Clear();

                List<PackageModel> packageList = await Task.Run(() =>
                {
                    List<PackageModel> packageList = [];

                    try
                    {
                        foreach (Package package in packageManager.FindPackagesForUser(string.Empty))
                        {
                            packageList.Add(new PackageModel()
                            {
                                LogoImage = package.Logo,
                                IsFramework = GetIsFramework(package),
                                AppListEntryCount = GetAppListEntriesCount(package),
                                DisplayName = GetDisplayName(package),
                                InstallDate = GetInstallDate(package),
                                PublisherDisplayName = GetPublisherDisplayName(package),
                                Version = GetVersion(package),
                                SignatureKind = GetSignatureKind(package),
                                InstalledDate = GetInstalledDate(package),
                                Package = package,
                                IsUninstalling = false
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "Find current user packages failed", e);
                    }

                    return packageList;
                });

                AppManagerList.AddRange(packageList);

                if (AppManagerList.Count is 0)
                {
                    AppManagerResultKind = AppManagerResultKind.Failed;
                    AppManagerFailedContent = PackageEmptyDescription;
                }
                else
                {
                    List<PackageModel> filterSortPackageList = await Task.Run(() =>
                    {
                        List<PackageModel> filterSortPackageList = [];

                        try
                        {
                            List<PackageModel> conditionWithFrameworkList = [];

                            // 根据选项是否筛选包含框架包的数据
                            if (IsAppFramework)
                            {
                                foreach (PackageModel packageItem in AppManagerList)
                                {
                                    if (Equals(packageItem.IsFramework, IsAppFramework))
                                    {
                                        conditionWithFrameworkList.Add(packageItem);
                                    }
                                }
                            }
                            else
                            {
                                conditionWithFrameworkList.AddRange(AppManagerList);
                            }

                            // 根据选项是否筛选包含特定签名类型的数据
                            List<PackageModel> conditionWithSignatureKindList = [];
                            foreach (PackageModel packageItem in conditionWithFrameworkList)
                            {
                                if (Equals(packageItem.SignatureKind, PackageSignatureKind.Store) && IsStoreSignatureSelected)
                                {
                                    conditionWithSignatureKindList.Add(packageItem);
                                }
                                else if (Equals(packageItem.SignatureKind, PackageSignatureKind.System) && IsSystemSignatureSelected)
                                {
                                    conditionWithSignatureKindList.Add(packageItem);
                                }
                                else if (Equals(packageItem.SignatureKind, PackageSignatureKind.Enterprise) && IsEnterpriseSignatureSelected)
                                {
                                    conditionWithSignatureKindList.Add(packageItem);
                                }
                                else if (Equals(packageItem.SignatureKind, PackageSignatureKind.Developer) && IsDeveloperSignatureSelected)
                                {
                                    conditionWithSignatureKindList.Add(packageItem);
                                }
                                else if (Equals(packageItem.SignatureKind, PackageSignatureKind.None) && IsNoneSignatureSelected)
                                {
                                    conditionWithSignatureKindList.Add(packageItem);
                                }
                            }

                            List<PackageModel> searchedList = [];

                            // 根据搜索内容筛选包含特定签名类型的数据
                            if (string.IsNullOrEmpty(SearchText))
                            {
                                searchedList.AddRange(conditionWithSignatureKindList);
                            }
                            else
                            {
                                foreach (PackageModel packageItem in conditionWithSignatureKindList)
                                {
                                    if (packageItem.DisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || packageItem.PublisherDisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                                    {
                                        searchedList.Add(packageItem);
                                    }
                                }
                            }

                            // 对过滤后的列表数据进行排序
                            switch (SelectedAppSortRuleKind)
                            {
                                case AppSortRuleKind.DisplayName:
                                    {
                                        if (IsIncrease)
                                        {
                                            searchedList.Sort((item1, item2) => item1.DisplayName.CompareTo(item2.DisplayName));
                                        }
                                        else
                                        {
                                            searchedList.Sort((item1, item2) => item2.DisplayName.CompareTo(item1.DisplayName));
                                        }
                                        break;
                                    }
                                case AppSortRuleKind.PublisherName:
                                    {
                                        if (IsIncrease)
                                        {
                                            searchedList.Sort((item1, item2) => item1.PublisherDisplayName.CompareTo(item2.PublisherDisplayName));
                                        }
                                        else
                                        {
                                            searchedList.Sort((item1, item2) => item2.PublisherDisplayName.CompareTo(item1.PublisherDisplayName));
                                        }
                                        break;
                                    }
                                case AppSortRuleKind.InstallDate:
                                    {
                                        if (IsIncrease)
                                        {
                                            searchedList.Sort((item1, item2) => item1.InstalledDate.CompareTo(item2.InstalledDate));
                                        }
                                        else
                                        {
                                            searchedList.Sort((item1, item2) => item2.InstalledDate.CompareTo(item1.InstalledDate));
                                        }
                                        break;
                                    }
                            }

                            filterSortPackageList.AddRange(searchedList);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LoggingLevel.Error, "Filter and sort package list failed", e);
                        }

                        return filterSortPackageList;
                    });

                    foreach (PackageModel packageItem in filterSortPackageList)
                    {
                        AppManagerCollection.Add(packageItem);
                    }

                    AppManagerResultKind = AppManagerCollection.Count is 0 ? AppManagerResultKind.Failed : AppManagerResultKind.Successfully;
                    AppManagerFailedContent = AppManagerCollection.Count is 0 ? PackageEmptyWithConditionDescription : string.Empty;
                }
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第一部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 打开应用
        /// </summary>
        private void OnOpenAppExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is Package package)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await package.GetAppListEntries()[0].LaunchAsync();
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, string.Format("Open app {0} failed", package.DisplayName), e);
                    }
                });
            }
        }

        /// <summary>
        /// 打开应用缓存目录
        /// </summary>
        private void OnOpenCacheFolderExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is Package package)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        if (ApplicationDataManager.CreateForPackageFamily(package.Id.FamilyName) is ApplicationData applicationData)
                        {
                            await Launcher.LaunchFolderAsync(applicationData.LocalFolder);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Information, "Open app cache folder failed.", e);
                    }
                });
            }
        }

        /// <summary>
        /// 打开应用安装目录
        /// </summary>
        private void OnOpenInstalledFolderExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is Package package)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await Launcher.LaunchFolderPathAsync(package.InstalledPath);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Warning, string.Format("{0} app installed folder open failed", package.DisplayName), e);
                    }
                });
            }
        }

        /// <summary>
        /// 打开应用清单文件
        /// </summary>
        private void OnOpenManifestExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is Package package)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        if (await StorageFile.GetFileFromPathAsync(Path.Combine(package.InstalledPath, "AppxManifest.xml")) is StorageFile file)
                        {
                            await Launcher.LaunchFileAsync(file);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, string.Format("{0}'s AppxManifest.xml file open failed", package.DisplayName), e);
                    }
                });
            }
        }

        /// <summary>
        /// 打开商店
        /// </summary>
        private void OnOpenStoreExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is Package package)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await Launcher.LaunchUriAsync(new Uri($"ms-windows-store://pdp/?PFN={package.Id.FamilyName}"));
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, string.Format("Open microsoft store {0} failed", package.DisplayName), e);
                    }
                });
            }
        }

        /// <summary>
        /// 卸载应用
        /// </summary>
        private async void OnUninstallExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is PackageModel package)
            {
                package.IsUninstalling = true;

                try
                {
                    DeploymentResult deploymentResult = await Task.Run(async () =>
                    {
                        return await packageManager.RemovePackageAsync(package.Package.Id.FullName, RemovalOptions.None);
                    });

                    // 卸载成功
                    if (deploymentResult.ExtendedErrorCode is null)
                    {
                        // 显示 UWP 应用卸载成功通知
                        await Task.Run(() =>
                        {
                            AppNotificationBuilder appNotificationBuilder = new();
                            appNotificationBuilder.AddArgument("action", "OpenApp");
                            appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/UWPUninstallSuccessfully"), package.Package.DisplayName));
                            ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                        });

                        AppManagerList.Remove(package);
                        AppManagerCollection.Remove(package);
                    }

                    // 卸载失败
                    else
                    {
                        // 显示 UWP 应用卸载失败通知
                        await Task.Run(() =>
                        {
                            AppNotificationBuilder appNotificationBuilder = new();
                            appNotificationBuilder.AddArgument("action", "OpenApp");
                            appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/UWPUninstallFailed1"), package.Package.DisplayName));
                            appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/UWPUninstallFailed2"));

                            appNotificationBuilder.AddText(string.Join(Environment.NewLine, new string[]
                            {
                                    ResourceService.GetLocalized("Notification/UWPUninstallFailed3"),
                                    string.Format(ResourceService.GetLocalized("Notification/UWPUninstallFailed4"), deploymentResult.ExtendedErrorCode is not null ? deploymentResult.ExtendedErrorCode.HResult : Unknown),
                                    string.Format(ResourceService.GetLocalized("Notification/UWPUninstallFailed5"), deploymentResult.ErrorText)
                            }));
                            AppNotificationButton openSettingsButton = new(ResourceService.GetLocalized("Notification/OpenSettings"));
                            openSettingsButton.Arguments.Add("action", "OpenSettings");
                            appNotificationBuilder.AddButton(openSettingsButton);
                            ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                            LogService.WriteLog(LoggingLevel.Information, string.Format("Uninstall app {0} failed", package.Package.DisplayName), deploymentResult.ExtendedErrorCode is not null ? deploymentResult.ExtendedErrorCode : new Exception());
                        });

                        package.IsUninstalling = false;
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Information, string.Format("Uninstall app {0} failed", package.Package.Id.FullName), e);
                }
            }
        }

        /// <summary>
        /// 查看应用信息
        /// </summary>
        private async void OnViewInformationExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is PackageModel packageItem)
            {
                AppInformation appInformation = new();

                await Task.Run(() =>
                {
                    appInformation.DisplayName = packageItem.DisplayName;

                    try
                    {
                        appInformation.PackageFamilyName = string.IsNullOrEmpty(packageItem.Package.Id.FamilyName) ? Unknown : packageItem.Package.Id.FamilyName;
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                        appInformation.PackageFamilyName = Unknown;
                    }

                    try
                    {
                        appInformation.PackageFullName = string.IsNullOrEmpty(packageItem.Package.Id.FullName) ? Unknown : packageItem.Package.Id.FullName;
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                        appInformation.PackageFullName = Unknown;
                    }

                    try
                    {
                        appInformation.Description = string.IsNullOrEmpty(packageItem.Package.Description) ? Unknown : packageItem.Package.Description;
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                        appInformation.Description = Unknown;
                    }

                    appInformation.PublisherDisplayName = packageItem.PublisherDisplayName;

                    try
                    {
                        appInformation.PublisherId = string.IsNullOrEmpty(packageItem.Package.Id.PublisherId) ? Unknown : packageItem.Package.Id.PublisherId;
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                        appInformation.PublisherId = Unknown;
                    }

                    appInformation.Version = packageItem.Version;
                    appInformation.InstallDate = packageItem.InstallDate;

                    try
                    {
                        appInformation.Architecture = string.IsNullOrEmpty(packageItem.Package.Id.Architecture.ToString()) ? Unknown : packageItem.Package.Id.Architecture.ToString();
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                        appInformation.Architecture = Unknown;
                    }

                    appInformation.SignatureKind = ResourceService.GetLocalized(string.Format("AppManager/Signature{0}", packageItem.SignatureKind.ToString()));

                    try
                    {
                        appInformation.ResourceId = string.IsNullOrEmpty(packageItem.Package.Id.ResourceId) ? Unknown : packageItem.Package.Id.ResourceId;
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                        appInformation.ResourceId = Unknown;
                    }

                    try
                    {
                        appInformation.IsBundle = packageItem.Package.IsBundle ? Yes : No;
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                        appInformation.IsBundle = Unknown;
                    }

                    try
                    {
                        appInformation.IsDevelopmentMode = packageItem.Package.IsDevelopmentMode ? Yes : No;
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                        appInformation.IsDevelopmentMode = Unknown;
                    }

                    appInformation.IsFramework = packageItem.IsFramework ? Yes : No;

                    try
                    {
                        appInformation.IsOptional = packageItem.Package.IsOptional ? Yes : No;
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                        appInformation.IsOptional = Unknown;
                    }

                    try
                    {
                        appInformation.IsResourcePackage = packageItem.Package.IsResourcePackage ? Yes : No;
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                        appInformation.IsResourcePackage = Unknown;
                    }

                    try
                    {
                        appInformation.IsStub = packageItem.Package.IsStub ? Yes : No;
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                        appInformation.IsStub = Unknown;
                    }

                    try
                    {
                        appInformation.VertifyIsOK = packageItem.Package.Status.VerifyIsOK() ? Yes : No;
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                        appInformation.VertifyIsOK = Unknown;
                    }

                    try
                    {
                        IReadOnlyList<AppListEntry> appListEntriesList = packageItem.Package.GetAppListEntries();
                        for (int index = 0; index < appListEntriesList.Count; index++)
                        {
                            appInformation.AppListEntryList.Add(new AppListEntryModel()
                            {
                                DisplayName = appListEntriesList[index].DisplayInfo.DisplayName,
                                Description = appListEntriesList[index].DisplayInfo.Description,
                                AppUserModelId = appListEntriesList[index].AppUserModelId,
                                AppListEntry = appListEntriesList[index],
                                PackageFullName = packageItem.Package.Id.FullName
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                    }

                    try
                    {
                        IReadOnlyList<Package> dependcies = packageItem.Package.Dependencies;

                        if (dependcies.Count > 0)
                        {
                            for (int index = 0; index < dependcies.Count; index++)
                            {
                                try
                                {
                                    appInformation.DependenciesList.Add(new PackageModel()
                                    {
                                        DisplayName = dependcies[index].DisplayName,
                                        PublisherDisplayName = dependcies[index].PublisherDisplayName,
                                        Version = new Version(dependcies[index].Id.Version.Major, dependcies[index].Id.Version.Minor, dependcies[index].Id.Version.Build, dependcies[index].Id.Version.Revision).ToString(),
                                        Package = dependcies[index]
                                    });
                                }
                                catch
                                {
                                    continue;
                                }
                            }
                        }

                        appInformation.DependenciesList.Sort((item1, item2) => item1.DisplayName.CompareTo(item2.DisplayName));
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                    }
                });

                if (MainWindow.Current.GetFrameContent() is AppManagerPage appManagerPage && Equals(appManagerPage.GetCurrentPageType(), typeof(AppListPage)))
                {
                    appManagerPage.NavigateTo(appManagerPage.PageList[1], appInformation, true);
                }
            }
        }

        #endregion 第一部分：XamlUICommand 命令调用时挂载的事件

        #region 第二部分：应用管理页面——挂载的事件

        /// <summary>
        /// 打开设置中的安装的应用
        /// </summary>
        private async void OnInstalledAppsClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures"));
        }

        /// <summary>
        /// 根据输入的内容检索应用
        /// </summary>
        private async void OnQuerySubmitted(object sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrEmpty(SearchText))
            {
                AppManagerResultKind = AppManagerResultKind.Loading;
                AppManagerCollection.Clear();

                List<PackageModel> filterSortPackageList = await Task.Run(() =>
                {
                    List<PackageModel> filterSortPackageList = [];

                    try
                    {
                        List<PackageModel> conditionWithFrameworkList = [];

                        // 根据选项是否筛选包含框架包的数据
                        if (IsAppFramework)
                        {
                            foreach (PackageModel packageItem in AppManagerList)
                            {
                                if (Equals(packageItem.IsFramework, IsAppFramework))
                                {
                                    conditionWithFrameworkList.Add(packageItem);
                                }
                            }
                        }
                        else
                        {
                            conditionWithFrameworkList.AddRange(AppManagerList);
                        }

                        // 根据选项是否筛选包含特定签名类型的数据
                        List<PackageModel> conditionWithSignatureKindList = [];
                        foreach (PackageModel packageItem in conditionWithFrameworkList)
                        {
                            if (Equals(packageItem.SignatureKind, PackageSignatureKind.Store) && IsStoreSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                            else if (Equals(packageItem.SignatureKind, PackageSignatureKind.System) && IsSystemSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                            else if (Equals(packageItem.SignatureKind, PackageSignatureKind.Enterprise) && IsEnterpriseSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                            else if (Equals(packageItem.SignatureKind, PackageSignatureKind.Developer) && IsDeveloperSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                            else if (Equals(packageItem.SignatureKind, PackageSignatureKind.None) && IsNoneSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                        }

                        List<PackageModel> searchedList = [];

                        // 根据搜索内容筛选包含特定签名类型的数据
                        if (string.IsNullOrEmpty(SearchText))
                        {
                            searchedList.AddRange(conditionWithSignatureKindList);
                        }
                        else
                        {
                            foreach (PackageModel packageItem in conditionWithSignatureKindList)
                            {
                                if (packageItem.DisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || packageItem.PublisherDisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                                {
                                    searchedList.Add(packageItem);
                                }
                            }
                        }

                        // 对过滤后的列表数据进行排序
                        switch (SelectedAppSortRuleKind)
                        {
                            case AppSortRuleKind.DisplayName:
                                {
                                    if (IsIncrease)
                                    {
                                        searchedList.Sort((item1, item2) => item1.DisplayName.CompareTo(item2.DisplayName));
                                    }
                                    else
                                    {
                                        searchedList.Sort((item1, item2) => item2.DisplayName.CompareTo(item1.DisplayName));
                                    }
                                    break;
                                }
                            case AppSortRuleKind.PublisherName:
                                {
                                    if (IsIncrease)
                                    {
                                        searchedList.Sort((item1, item2) => item1.PublisherDisplayName.CompareTo(item2.PublisherDisplayName));
                                    }
                                    else
                                    {
                                        searchedList.Sort((item1, item2) => item2.PublisherDisplayName.CompareTo(item1.PublisherDisplayName));
                                    }
                                    break;
                                }
                            case AppSortRuleKind.InstallDate:
                                {
                                    if (IsIncrease)
                                    {
                                        searchedList.Sort((item1, item2) => item1.InstalledDate.CompareTo(item2.InstalledDate));
                                    }
                                    else
                                    {
                                        searchedList.Sort((item1, item2) => item2.InstalledDate.CompareTo(item1.InstalledDate));
                                    }
                                    break;
                                }
                        }

                        filterSortPackageList.AddRange(searchedList);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "Filter and sort package list failed", e);
                    }

                    return filterSortPackageList;
                });

                foreach (PackageModel packageItem in filterSortPackageList)
                {
                    AppManagerCollection.Add(packageItem);
                }

                AppManagerResultKind = AppManagerCollection.Count is 0 ? AppManagerResultKind.Failed : AppManagerResultKind.Successfully;
                AppManagerFailedContent = AppManagerCollection.Count is 0 ? PackageEmptyWithConditionDescription : string.Empty;
            }
        }

        /// <summary>
        /// 文本输入框内容为空时，复原原来的内容
        /// </summary>
        private async void OnTextChanged(object sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (sender is AutoSuggestBox autoSuggestBox)
            {
                SearchText = autoSuggestBox.Text;
                if (string.IsNullOrEmpty(SearchText))
                {
                    AppManagerResultKind = AppManagerResultKind.Loading;
                    AppManagerCollection.Clear();

                    List<PackageModel> filterSortPackageList = await Task.Run(() =>
                    {
                        List<PackageModel> filterSortPackageList = [];

                        try
                        {
                            List<PackageModel> conditionWithFrameworkList = [];

                            // 根据选项是否筛选包含框架包的数据
                            if (IsAppFramework)
                            {
                                foreach (PackageModel packageItem in AppManagerList)
                                {
                                    if (Equals(packageItem.IsFramework, IsAppFramework))
                                    {
                                        conditionWithFrameworkList.Add(packageItem);
                                    }
                                }
                            }
                            else
                            {
                                conditionWithFrameworkList.AddRange(AppManagerList);
                            }

                            // 根据选项是否筛选包含特定签名类型的数据
                            List<PackageModel> conditionWithSignatureKindList = [];
                            foreach (PackageModel packageItem in conditionWithFrameworkList)
                            {
                                if (Equals(packageItem.SignatureKind, PackageSignatureKind.Store) && IsStoreSignatureSelected)
                                {
                                    conditionWithSignatureKindList.Add(packageItem);
                                }
                                else if (Equals(packageItem.SignatureKind, PackageSignatureKind.System) && IsSystemSignatureSelected)
                                {
                                    conditionWithSignatureKindList.Add(packageItem);
                                }
                                else if (Equals(packageItem.SignatureKind, PackageSignatureKind.Enterprise) && IsEnterpriseSignatureSelected)
                                {
                                    conditionWithSignatureKindList.Add(packageItem);
                                }
                                else if (Equals(packageItem.SignatureKind, PackageSignatureKind.Developer) && IsDeveloperSignatureSelected)
                                {
                                    conditionWithSignatureKindList.Add(packageItem);
                                }
                                else if (Equals(packageItem.SignatureKind, PackageSignatureKind.None) && IsNoneSignatureSelected)
                                {
                                    conditionWithSignatureKindList.Add(packageItem);
                                }
                            }

                            List<PackageModel> searchedList = [];

                            // 根据搜索内容筛选包含特定签名类型的数据
                            if (string.IsNullOrEmpty(SearchText))
                            {
                                searchedList.AddRange(conditionWithSignatureKindList);
                            }
                            else
                            {
                                foreach (PackageModel packageItem in conditionWithSignatureKindList)
                                {
                                    if (packageItem.DisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || packageItem.PublisherDisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                                    {
                                        searchedList.Add(packageItem);
                                    }
                                }
                            }

                            // 对过滤后的列表数据进行排序
                            switch (SelectedAppSortRuleKind)
                            {
                                case AppSortRuleKind.DisplayName:
                                    {
                                        if (IsIncrease)
                                        {
                                            searchedList.Sort((item1, item2) => item1.DisplayName.CompareTo(item2.DisplayName));
                                        }
                                        else
                                        {
                                            searchedList.Sort((item1, item2) => item2.DisplayName.CompareTo(item1.DisplayName));
                                        }
                                        break;
                                    }
                                case AppSortRuleKind.PublisherName:
                                    {
                                        if (IsIncrease)
                                        {
                                            searchedList.Sort((item1, item2) => item1.PublisherDisplayName.CompareTo(item2.PublisherDisplayName));
                                        }
                                        else
                                        {
                                            searchedList.Sort((item1, item2) => item2.PublisherDisplayName.CompareTo(item1.PublisherDisplayName));
                                        }
                                        break;
                                    }
                                case AppSortRuleKind.InstallDate:
                                    {
                                        if (IsIncrease)
                                        {
                                            searchedList.Sort((item1, item2) => item1.InstalledDate.CompareTo(item2.InstalledDate));
                                        }
                                        else
                                        {
                                            searchedList.Sort((item1, item2) => item2.InstalledDate.CompareTo(item1.InstalledDate));
                                        }
                                        break;
                                    }
                            }

                            filterSortPackageList.AddRange(searchedList);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LoggingLevel.Error, "Filter and sort package list failed", e);
                        }

                        return filterSortPackageList;
                    });

                    foreach (PackageModel packageItem in filterSortPackageList)
                    {
                        AppManagerCollection.Add(packageItem);
                    }

                    AppManagerResultKind = AppManagerCollection.Count is 0 ? AppManagerResultKind.Failed : AppManagerResultKind.Successfully;
                    AppManagerFailedContent = AppManagerCollection.Count is 0 ? PackageEmptyWithConditionDescription : string.Empty;
                }
            }
        }

        /// <summary>
        /// 根据排序方式对列表进行排序
        /// </summary>
        private async void OnSortWayClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is string increase)
            {
                IsIncrease = Convert.ToBoolean(increase);

                if (AppManagerResultKind is AppManagerResultKind.Successfully)
                {
                    AppManagerResultKind = AppManagerResultKind.Loading;
                    AppManagerCollection.Clear();

                    List<PackageModel> filterSortPackageList = await Task.Run(() =>
                    {
                        List<PackageModel> filterSortPackageList = [];

                        try
                        {
                            List<PackageModel> conditionWithFrameworkList = [];

                            // 根据选项是否筛选包含框架包的数据
                            if (IsAppFramework)
                            {
                                foreach (PackageModel packageItem in AppManagerList)
                                {
                                    if (Equals(packageItem.IsFramework, IsAppFramework))
                                    {
                                        conditionWithFrameworkList.Add(packageItem);
                                    }
                                }
                            }
                            else
                            {
                                conditionWithFrameworkList.AddRange(AppManagerList);
                            }

                            // 根据选项是否筛选包含特定签名类型的数据
                            List<PackageModel> conditionWithSignatureKindList = [];
                            foreach (PackageModel packageItem in conditionWithFrameworkList)
                            {
                                if (Equals(packageItem.SignatureKind, PackageSignatureKind.Store) && IsStoreSignatureSelected)
                                {
                                    conditionWithSignatureKindList.Add(packageItem);
                                }
                                else if (Equals(packageItem.SignatureKind, PackageSignatureKind.System) && IsSystemSignatureSelected)
                                {
                                    conditionWithSignatureKindList.Add(packageItem);
                                }
                                else if (Equals(packageItem.SignatureKind, PackageSignatureKind.Enterprise) && IsEnterpriseSignatureSelected)
                                {
                                    conditionWithSignatureKindList.Add(packageItem);
                                }
                                else if (Equals(packageItem.SignatureKind, PackageSignatureKind.Developer) && IsDeveloperSignatureSelected)
                                {
                                    conditionWithSignatureKindList.Add(packageItem);
                                }
                                else if (Equals(packageItem.SignatureKind, PackageSignatureKind.None) && IsNoneSignatureSelected)
                                {
                                    conditionWithSignatureKindList.Add(packageItem);
                                }
                            }

                            List<PackageModel> searchedList = [];

                            // 根据搜索内容筛选包含特定签名类型的数据
                            if (string.IsNullOrEmpty(SearchText))
                            {
                                searchedList.AddRange(conditionWithSignatureKindList);
                            }
                            else
                            {
                                foreach (PackageModel packageItem in conditionWithSignatureKindList)
                                {
                                    if (packageItem.DisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || packageItem.PublisherDisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                                    {
                                        searchedList.Add(packageItem);
                                    }
                                }
                            }

                            // 对过滤后的列表数据进行排序
                            switch (SelectedAppSortRuleKind)
                            {
                                case AppSortRuleKind.DisplayName:
                                    {
                                        if (IsIncrease)
                                        {
                                            searchedList.Sort((item1, item2) => item1.DisplayName.CompareTo(item2.DisplayName));
                                        }
                                        else
                                        {
                                            searchedList.Sort((item1, item2) => item2.DisplayName.CompareTo(item1.DisplayName));
                                        }
                                        break;
                                    }
                                case AppSortRuleKind.PublisherName:
                                    {
                                        if (IsIncrease)
                                        {
                                            searchedList.Sort((item1, item2) => item1.PublisherDisplayName.CompareTo(item2.PublisherDisplayName));
                                        }
                                        else
                                        {
                                            searchedList.Sort((item1, item2) => item2.PublisherDisplayName.CompareTo(item1.PublisherDisplayName));
                                        }
                                        break;
                                    }
                                case AppSortRuleKind.InstallDate:
                                    {
                                        if (IsIncrease)
                                        {
                                            searchedList.Sort((item1, item2) => item1.InstalledDate.CompareTo(item2.InstalledDate));
                                        }
                                        else
                                        {
                                            searchedList.Sort((item1, item2) => item2.InstalledDate.CompareTo(item1.InstalledDate));
                                        }
                                        break;
                                    }
                            }

                            filterSortPackageList.AddRange(searchedList);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LoggingLevel.Error, "Filter and sort package list failed", e);
                        }

                        return filterSortPackageList;
                    });

                    foreach (PackageModel packageItem in filterSortPackageList)
                    {
                        AppManagerCollection.Add(packageItem);
                    }

                    AppManagerResultKind = AppManagerCollection.Count is 0 ? AppManagerResultKind.Failed : AppManagerResultKind.Successfully;
                    AppManagerFailedContent = AppManagerCollection.Count is 0 ? PackageEmptyWithConditionDescription : string.Empty;
                }
            }
        }

        /// <summary>
        /// 根据排序规则对列表进行排序
        /// </summary>
        private async void OnSortRuleClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is AppSortRuleKind appSortRuleKind)
            {
                SelectedAppSortRuleKind = appSortRuleKind;

                if (AppManagerResultKind is AppManagerResultKind.Successfully)
                {
                    AppManagerResultKind = AppManagerResultKind.Loading;
                    AppManagerCollection.Clear();

                    List<PackageModel> filterSortPackageList = await Task.Run(() =>
                    {
                        List<PackageModel> filterSortPackageList = [];

                        try
                        {
                            List<PackageModel> conditionWithFrameworkList = [];

                            // 根据选项是否筛选包含框架包的数据
                            if (IsAppFramework)
                            {
                                foreach (PackageModel packageItem in AppManagerList)
                                {
                                    if (Equals(packageItem.IsFramework, IsAppFramework))
                                    {
                                        conditionWithFrameworkList.Add(packageItem);
                                    }
                                }
                            }
                            else
                            {
                                conditionWithFrameworkList.AddRange(AppManagerList);
                            }

                            // 根据选项是否筛选包含特定签名类型的数据
                            List<PackageModel> conditionWithSignatureKindList = [];
                            foreach (PackageModel packageItem in conditionWithFrameworkList)
                            {
                                if (Equals(packageItem.SignatureKind, PackageSignatureKind.Store) && IsStoreSignatureSelected)
                                {
                                    conditionWithSignatureKindList.Add(packageItem);
                                }
                                else if (Equals(packageItem.SignatureKind, PackageSignatureKind.System) && IsSystemSignatureSelected)
                                {
                                    conditionWithSignatureKindList.Add(packageItem);
                                }
                                else if (Equals(packageItem.SignatureKind, PackageSignatureKind.Enterprise) && IsEnterpriseSignatureSelected)
                                {
                                    conditionWithSignatureKindList.Add(packageItem);
                                }
                                else if (Equals(packageItem.SignatureKind, PackageSignatureKind.Developer) && IsDeveloperSignatureSelected)
                                {
                                    conditionWithSignatureKindList.Add(packageItem);
                                }
                                else if (Equals(packageItem.SignatureKind, PackageSignatureKind.None) && IsNoneSignatureSelected)
                                {
                                    conditionWithSignatureKindList.Add(packageItem);
                                }
                            }

                            List<PackageModel> searchedList = [];

                            // 根据搜索内容筛选包含特定签名类型的数据
                            if (string.IsNullOrEmpty(SearchText))
                            {
                                searchedList.AddRange(conditionWithSignatureKindList);
                            }
                            else
                            {
                                foreach (PackageModel packageItem in conditionWithSignatureKindList)
                                {
                                    if (packageItem.DisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || packageItem.PublisherDisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                                    {
                                        searchedList.Add(packageItem);
                                    }
                                }
                            }

                            // 对过滤后的列表数据进行排序
                            switch (SelectedAppSortRuleKind)
                            {
                                case AppSortRuleKind.DisplayName:
                                    {
                                        if (IsIncrease)
                                        {
                                            searchedList.Sort((item1, item2) => item1.DisplayName.CompareTo(item2.DisplayName));
                                        }
                                        else
                                        {
                                            searchedList.Sort((item1, item2) => item2.DisplayName.CompareTo(item1.DisplayName));
                                        }
                                        break;
                                    }
                                case AppSortRuleKind.PublisherName:
                                    {
                                        if (IsIncrease)
                                        {
                                            searchedList.Sort((item1, item2) => item1.PublisherDisplayName.CompareTo(item2.PublisherDisplayName));
                                        }
                                        else
                                        {
                                            searchedList.Sort((item1, item2) => item2.PublisherDisplayName.CompareTo(item1.PublisherDisplayName));
                                        }
                                        break;
                                    }
                                case AppSortRuleKind.InstallDate:
                                    {
                                        if (IsIncrease)
                                        {
                                            searchedList.Sort((item1, item2) => item1.InstalledDate.CompareTo(item2.InstalledDate));
                                        }
                                        else
                                        {
                                            searchedList.Sort((item1, item2) => item2.InstalledDate.CompareTo(item1.InstalledDate));
                                        }
                                        break;
                                    }
                            }

                            filterSortPackageList.AddRange(searchedList);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LoggingLevel.Error, "Filter and sort package list failed", e);
                        }

                        return filterSortPackageList;
                    });

                    foreach (PackageModel packageItem in filterSortPackageList)
                    {
                        AppManagerCollection.Add(packageItem);
                    }

                    AppManagerResultKind = AppManagerCollection.Count is 0 ? AppManagerResultKind.Failed : AppManagerResultKind.Successfully;
                    AppManagerFailedContent = AppManagerCollection.Count is 0 ? PackageEmptyWithConditionDescription : string.Empty;
                }
            }
        }

        /// <summary>
        /// 根据过滤方式对列表进行过滤
        /// </summary>
        private void OnFilterWayClicked(object sender, RoutedEventArgs args)
        {
            IsAppFramework = !IsAppFramework;
            needToRefreshData = true;
        }

        /// <summary>
        /// 根据签名规则进行过滤
        /// </summary>
        private void OnSignatureRuleClicked(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleButton toggleButton && toggleButton.Tag is not null)
            {
                PackageSignatureKind signatureKind = (PackageSignatureKind)toggleButton.Tag;

                if (signatureKind is PackageSignatureKind.Store)
                {
                    IsStoreSignatureSelected = !IsStoreSignatureSelected;
                }
                else if (signatureKind is PackageSignatureKind.System)
                {
                    IsSystemSignatureSelected = !IsSystemSignatureSelected;
                }
                else if (signatureKind is PackageSignatureKind.Enterprise)
                {
                    IsEnterpriseSignatureSelected = !IsEnterpriseSignatureSelected;
                }
                else if (signatureKind is PackageSignatureKind.Developer)
                {
                    IsDeveloperSignatureSelected = !IsDeveloperSignatureSelected;
                }
                else if (signatureKind is PackageSignatureKind.None)
                {
                    IsNoneSignatureSelected = !IsNoneSignatureSelected;
                }

                needToRefreshData = true;
            }
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        private async void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            AppManagerResultKind = AppManagerResultKind.Loading;
            AppManagerList.Clear();
            AppManagerCollection.Clear();

            List<PackageModel> packageList = await Task.Run(() =>
            {
                List<PackageModel> packageList = [];

                try
                {
                    foreach (Package package in packageManager.FindPackagesForUser(string.Empty))
                    {
                        packageList.Add(new PackageModel()
                        {
                            LogoImage = package.Logo,
                            IsFramework = GetIsFramework(package),
                            AppListEntryCount = GetAppListEntriesCount(package),
                            DisplayName = GetDisplayName(package),
                            InstallDate = GetInstallDate(package),
                            PublisherDisplayName = GetPublisherDisplayName(package),
                            Version = GetVersion(package),
                            SignatureKind = GetSignatureKind(package),
                            InstalledDate = GetInstalledDate(package),
                            Package = package,
                            IsUninstalling = false
                        });
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "Find current user packages failed", e);
                }

                return packageList;
            });

            AppManagerList.AddRange(packageList);

            if (AppManagerList.Count is 0)
            {
                AppManagerResultKind = AppManagerResultKind.Failed;
                AppManagerFailedContent = PackageEmptyDescription;
            }
            else
            {
                List<PackageModel> filterSortPackageList = await Task.Run(() =>
                {
                    List<PackageModel> filterSortPackageList = [];

                    try
                    {
                        List<PackageModel> conditionWithFrameworkList = [];

                        // 根据选项是否筛选包含框架包的数据
                        if (IsAppFramework)
                        {
                            foreach (PackageModel packageItem in AppManagerList)
                            {
                                if (Equals(packageItem.IsFramework, IsAppFramework))
                                {
                                    conditionWithFrameworkList.Add(packageItem);
                                }
                            }
                        }
                        else
                        {
                            conditionWithFrameworkList.AddRange(AppManagerList);
                        }

                        // 根据选项是否筛选包含特定签名类型的数据
                        List<PackageModel> conditionWithSignatureKindList = [];
                        foreach (PackageModel packageItem in conditionWithFrameworkList)
                        {
                            if (Equals(packageItem.SignatureKind, PackageSignatureKind.Store) && IsStoreSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                            else if (Equals(packageItem.SignatureKind, PackageSignatureKind.System) && IsSystemSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                            else if (Equals(packageItem.SignatureKind, PackageSignatureKind.Enterprise) && IsEnterpriseSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                            else if (Equals(packageItem.SignatureKind, PackageSignatureKind.Developer) && IsDeveloperSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                            else if (Equals(packageItem.SignatureKind, PackageSignatureKind.None) && IsNoneSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                        }

                        List<PackageModel> searchedList = [];

                        // 根据搜索内容筛选包含特定签名类型的数据
                        if (string.IsNullOrEmpty(SearchText))
                        {
                            searchedList.AddRange(conditionWithSignatureKindList);
                        }
                        else
                        {
                            foreach (PackageModel packageItem in conditionWithSignatureKindList)
                            {
                                if (packageItem.DisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || packageItem.PublisherDisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                                {
                                    searchedList.Add(packageItem);
                                }
                            }
                        }

                        // 对过滤后的列表数据进行排序
                        switch (SelectedAppSortRuleKind)
                        {
                            case AppSortRuleKind.DisplayName:
                                {
                                    if (IsIncrease)
                                    {
                                        searchedList.Sort((item1, item2) => item1.DisplayName.CompareTo(item2.DisplayName));
                                    }
                                    else
                                    {
                                        searchedList.Sort((item1, item2) => item2.DisplayName.CompareTo(item1.DisplayName));
                                    }
                                    break;
                                }
                            case AppSortRuleKind.PublisherName:
                                {
                                    if (IsIncrease)
                                    {
                                        searchedList.Sort((item1, item2) => item1.PublisherDisplayName.CompareTo(item2.PublisherDisplayName));
                                    }
                                    else
                                    {
                                        searchedList.Sort((item1, item2) => item2.PublisherDisplayName.CompareTo(item1.PublisherDisplayName));
                                    }
                                    break;
                                }
                            case AppSortRuleKind.InstallDate:
                                {
                                    if (IsIncrease)
                                    {
                                        searchedList.Sort((item1, item2) => item1.InstalledDate.CompareTo(item2.InstalledDate));
                                    }
                                    else
                                    {
                                        searchedList.Sort((item1, item2) => item2.InstalledDate.CompareTo(item1.InstalledDate));
                                    }
                                    break;
                                }
                        }

                        filterSortPackageList.AddRange(searchedList);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "Filter and sort package list failed", e);
                    }

                    return filterSortPackageList;
                });

                foreach (PackageModel packageItem in filterSortPackageList)
                {
                    AppManagerCollection.Add(packageItem);
                }

                AppManagerResultKind = AppManagerCollection.Count is 0 ? AppManagerResultKind.Failed : AppManagerResultKind.Successfully;
                AppManagerFailedContent = AppManagerCollection.Count is 0 ? PackageEmptyWithConditionDescription : string.Empty;
            }
        }

        /// <summary>
        /// 浮出菜单关闭后更新数据
        /// </summary>
        private async void OnClosed(object sender, object args)
        {
            if (needToRefreshData)
            {
                AppManagerResultKind = AppManagerResultKind.Loading;
                AppManagerCollection.Clear();

                List<PackageModel> filterSortPackageList = await Task.Run(() =>
                {
                    List<PackageModel> filterSortPackageList = [];

                    try
                    {
                        List<PackageModel> conditionWithFrameworkList = [];

                        // 根据选项是否筛选包含框架包的数据
                        if (IsAppFramework)
                        {
                            foreach (PackageModel packageItem in AppManagerList)
                            {
                                if (Equals(packageItem.IsFramework, IsAppFramework))
                                {
                                    conditionWithFrameworkList.Add(packageItem);
                                }
                            }
                        }
                        else
                        {
                            conditionWithFrameworkList.AddRange(AppManagerList);
                        }

                        // 根据选项是否筛选包含特定签名类型的数据
                        List<PackageModel> conditionWithSignatureKindList = [];
                        foreach (PackageModel packageItem in conditionWithFrameworkList)
                        {
                            if (Equals(packageItem.SignatureKind, PackageSignatureKind.Store) && IsStoreSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                            else if (Equals(packageItem.SignatureKind, PackageSignatureKind.System) && IsSystemSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                            else if (Equals(packageItem.SignatureKind, PackageSignatureKind.Enterprise) && IsEnterpriseSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                            else if (Equals(packageItem.SignatureKind, PackageSignatureKind.Developer) && IsDeveloperSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                            else if (Equals(packageItem.SignatureKind, PackageSignatureKind.None) && IsNoneSignatureSelected)
                            {
                                conditionWithSignatureKindList.Add(packageItem);
                            }
                        }

                        List<PackageModel> searchedList = [];

                        // 根据搜索内容筛选包含特定签名类型的数据
                        if (string.IsNullOrEmpty(SearchText))
                        {
                            searchedList.AddRange(conditionWithSignatureKindList);
                        }
                        else
                        {
                            foreach (PackageModel packageItem in conditionWithSignatureKindList)
                            {
                                if (packageItem.DisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || packageItem.PublisherDisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                                {
                                    searchedList.Add(packageItem);
                                }
                            }
                        }

                        // 对过滤后的列表数据进行排序
                        switch (SelectedAppSortRuleKind)
                        {
                            case AppSortRuleKind.DisplayName:
                                {
                                    if (IsIncrease)
                                    {
                                        searchedList.Sort((item1, item2) => item1.DisplayName.CompareTo(item2.DisplayName));
                                    }
                                    else
                                    {
                                        searchedList.Sort((item1, item2) => item2.DisplayName.CompareTo(item1.DisplayName));
                                    }
                                    break;
                                }
                            case AppSortRuleKind.PublisherName:
                                {
                                    if (IsIncrease)
                                    {
                                        searchedList.Sort((item1, item2) => item1.PublisherDisplayName.CompareTo(item2.PublisherDisplayName));
                                    }
                                    else
                                    {
                                        searchedList.Sort((item1, item2) => item2.PublisherDisplayName.CompareTo(item1.PublisherDisplayName));
                                    }
                                    break;
                                }
                            case AppSortRuleKind.InstallDate:
                                {
                                    if (IsIncrease)
                                    {
                                        searchedList.Sort((item1, item2) => item1.InstalledDate.CompareTo(item2.InstalledDate));
                                    }
                                    else
                                    {
                                        searchedList.Sort((item1, item2) => item2.InstalledDate.CompareTo(item1.InstalledDate));
                                    }
                                    break;
                                }
                        }

                        filterSortPackageList.AddRange(searchedList);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "Filter and sort package list failed", e);
                    }

                    return filterSortPackageList;
                });

                foreach (PackageModel packageItem in filterSortPackageList)
                {
                    AppManagerCollection.Add(packageItem);
                }

                AppManagerResultKind = AppManagerCollection.Count is 0 ? AppManagerResultKind.Failed : AppManagerResultKind.Successfully;
                AppManagerFailedContent = AppManagerCollection.Count is 0 ? PackageEmptyWithConditionDescription : string.Empty;
            }

            needToRefreshData = false;
        }

        #endregion 第二部分：应用管理页面——挂载的事件

        /// <summary>
        /// 获取加载应用是否成功
        /// </summary>
        private Visibility GetAppManagerSuccessfullyState(AppManagerResultKind appManagerResultKind, bool isSuccessfully)
        {
            return isSuccessfully ? appManagerResultKind is AppManagerResultKind.Successfully ? Visibility.Visible : Visibility.Collapsed : appManagerResultKind is AppManagerResultKind.Successfully ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// 检查搜索应用是否成功
        /// </summary>
        private Visibility CheckAppManagerState(AppManagerResultKind appManagerResultKind, AppManagerResultKind comparedAppManagerResultKind)
        {
            return Equals(appManagerResultKind, comparedAppManagerResultKind) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取是否正在加载中
        /// </summary>

        private bool GetIsLoading(AppManagerResultKind appManagerResultKind)
        {
            return !Equals(appManagerResultKind, AppManagerResultKind.Loading);
        }

        /// <summary>
        /// 获取应用包是否为框架包
        /// </summary>
        private static bool GetIsFramework(Package package)
        {
            try
            {
                return package.IsFramework;
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                return false;
            }
        }

        /// <summary>
        /// 获取应用包的入口数
        /// </summary>
        private static int GetAppListEntriesCount(Package package)
        {
            try
            {
                return package.GetAppListEntries().Count;
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                return 0;
            }
        }

        /// <summary>
        /// 获取应用的显示名称
        /// </summary>
        private string GetDisplayName(Package package)
        {
            try
            {
                return string.IsNullOrEmpty(package.DisplayName) ? Unknown : package.DisplayName;
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                return Unknown;
            }
        }

        /// <summary>
        /// 获取应用的发布者显示名称
        /// </summary>
        private string GetPublisherDisplayName(Package package)
        {
            try
            {
                return string.IsNullOrEmpty(package.PublisherDisplayName) ? Unknown : package.PublisherDisplayName;
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                return Unknown;
            }
        }

        /// <summary>
        /// 获取应用的版本信息
        /// </summary>
        private static string GetVersion(Package package)
        {
            try
            {
                return new Version(package.Id.Version.Major, package.Id.Version.Minor, package.Id.Version.Build, package.Id.Version.Revision).ToString();
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                return new Version().ToString();
            }
        }

        /// <summary>
        /// 获取应用的安装日期
        /// </summary>
        private static string GetInstallDate(Package package)
        {
            try
            {
                return package.InstalledDate.ToString("yyyy/MM/dd HH:mm");
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                return DateTimeOffset.FromUnixTimeSeconds(0).ToString("yyyy/MM/dd HH:mm");
            }
        }

        /// <summary>
        /// 获取应用包签名方式
        /// </summary>
        private static PackageSignatureKind GetSignatureKind(Package package)
        {
            try
            {
                return package.SignatureKind;
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                return PackageSignatureKind.None;
            }
        }

        /// <summary>
        /// 获取应用包安装日期
        /// </summary>
        private static DateTimeOffset GetInstalledDate(Package package)
        {
            try
            {
                return package.InstalledDate;
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                return DateTimeOffset.FromUnixTimeSeconds(0);
            }
        }
    }
}
