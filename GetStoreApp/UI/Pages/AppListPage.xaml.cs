using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Converters;
using GetStoreApp.Models.Controls.UWPApp;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Window;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Management.Deployment;
using Windows.Storage;
using Windows.System;

namespace GetStoreApp.UI.Pages
{
    /// <summary>
    /// 应用信息列表页面
    /// </summary>
    public sealed partial class AppListPage : Page, INotifyPropertyChanged
    {
        private string Unknown { get; } = ResourceService.GetLocalized("UWPApp/Unknown");

        private string Yes { get; } = ResourceService.GetLocalized("UWPApp/Yes");

        private string No { get; } = ResourceService.GetLocalized("UWPApp/No");

        private readonly object AppShortListObject = new object();

        private string packageRootCacheFolder = @"C:\Users\Gaoyifei\AppData\Local\Packages";

        private bool isInitialized = false;

        private AutoResetEvent autoResetEvent;

        private PackageManager PackageManager { get; } = new PackageManager();

        public string SearchText { get; set; } = string.Empty;

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

        private bool _isPackageEmpty = true;

        public bool IsPackageEmpty
        {
            get { return _isPackageEmpty; }

            set
            {
                _isPackageEmpty = value;
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

        private bool _isFramework = false;

        public bool IsFramework
        {
            get { return _isFramework; }

            set
            {
                _isFramework = value;
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

        private PackageSignKind _signType = PackageSignKind.Store;

        public PackageSignKind SignType
        {
            get { return _signType; }

            set
            {
                _signType = value;
                OnPropertyChanged();
            }
        }

        // 查看应用信息
        public XamlUICommand ViewInformationCommand { get; } = new XamlUICommand();

        // 打开应用
        public XamlUICommand OpenAppCommand { get; } = new XamlUICommand();

        // 打开商店
        public XamlUICommand OpenStoreCommand { get; } = new XamlUICommand();

        // 获取应用安装包
        public XamlUICommand GetPackageCommand { get; } = new XamlUICommand();

        // 打开应用清单文件
        public XamlUICommand OpenManifestCommand { get; } = new XamlUICommand();

        // 打开应用安装目录
        public XamlUICommand OpenInstalledFolderCommand { get; } = new XamlUICommand();

        // 打开应用缓存目录
        public XamlUICommand OpenCacheFolderCommand { get; } = new XamlUICommand();

        // 卸载应用
        public XamlUICommand UnInstallCommand { get; } = new XamlUICommand();

        private List<Package> MatchResultList;

        public ObservableCollection<PackageModel> UwpAppDataList { get; } = new ObservableCollection<PackageModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        public AppListPage()
        {
            InitializeComponent();

            ViewInformationCommand.ExecuteRequested += (sender, args) =>
            {
                PackageModel packageItem = args.Parameter as PackageModel;
                UWPAppPage uwpAppPage = NavigationService.NavigationFrame.Content as UWPAppPage;

                if (packageItem is not null && uwpAppPage is not null)
                {
                    Task.Run(() =>
                    {
                        Dictionary<string, object> packageDict = new Dictionary<string, object>();

                        packageDict["DisplayName"] = packageItem.DisplayName;
                        try
                        {
                            packageDict["FamilyName"] = string.IsNullOrEmpty(packageItem.Package.Id.FamilyName) ? Unknown : packageItem.Package.Id.FamilyName;
                        }
                        catch
                        {
                            packageDict["DisplayName"] = Unknown;
                        }

                        try
                        {
                            packageDict["FullName"] = string.IsNullOrEmpty(packageItem.Package.Id.FullName) ? Unknown : packageItem.Package.Id.FullName;
                        }
                        catch
                        {
                            packageDict["FullName"] = Unknown;
                        }

                        try
                        {
                            packageDict["Description"] = string.IsNullOrEmpty(packageItem.Package.Description) ? Unknown : packageItem.Package.Description;
                        }
                        catch
                        {
                            packageDict["FullName"] = Unknown;
                        }

                        packageDict["PublisherName"] = packageItem.PublisherName;

                        try
                        {
                            packageDict["PublisherId"] = string.IsNullOrEmpty(packageItem.Package.Id.PublisherId) ? Unknown : packageItem.Package.Id.PublisherId;
                        }
                        catch
                        {
                            packageDict["PublisherId"] = Unknown;
                        }

                        packageDict["Version"] = packageItem.Version;
                        packageDict["InstalledDate"] = packageItem.InstallDate;

                        try
                        {
                            packageDict["Architecture"] = string.IsNullOrEmpty(packageItem.Package.Id.Architecture.ToString()) ? Unknown : packageItem.Package.Id.Architecture.ToString();
                        }
                        catch
                        {
                            packageDict["Architecture"] = Unknown;
                        }

                        packageDict["SignatureKind"] = ResourceService.GetLocalized(string.Format("UWPApp/Signature{0}", packageItem.SignatureKind.ToString()));

                        try
                        {
                            packageDict["ResourceId"] = string.IsNullOrEmpty(packageItem.Package.Id.ResourceId) ? Unknown : packageItem.Package.Id.ResourceId;
                        }
                        catch
                        {
                            packageDict["ResourceId"] = Unknown;
                        }

                        try
                        {
                            packageDict["IsBundle"] = packageItem.Package.IsBundle ? Yes : No;
                        }
                        catch
                        {
                            packageDict["IsBundle"] = Unknown;
                        }

                        try
                        {
                            packageDict["IsDevelopmentMode"] = packageItem.Package.IsDevelopmentMode ? Yes : No;
                        }
                        catch
                        {
                            packageDict["IsDevelopmentMode"] = Unknown;
                        }

                        packageDict["IsFramework"] = packageItem.IsFramework ? Yes : No;

                        try
                        {
                            packageDict["IsOptional"] = packageItem.Package.IsOptional ? Yes : No;
                        }
                        catch
                        {
                            packageDict["IsOptional"] = Unknown;
                        }

                        try
                        {
                            packageDict["IsResourcePackage"] = packageItem.Package.IsResourcePackage ? Yes : No;
                        }
                        catch
                        {
                            packageDict["IsResourcePackage"] = Unknown;
                        }

                        try
                        {
                            packageDict["IsStub"] = packageItem.Package.IsStub ? Yes : No;
                        }
                        catch
                        {
                            packageDict["IsStub"] = Unknown;
                        }

                        try
                        {
                            packageDict["VertifyIsOK"] = packageItem.Package.Status.VerifyIsOK() ? Yes : No;
                        }
                        catch
                        {
                            packageDict["VertifyIsOK"] = Unknown;
                        }

                        try
                        {
                            List<AppListEntryModel> appListEntryList = new List<AppListEntryModel>();
                            foreach (AppListEntry appListEntryItem in packageItem.Package.GetAppListEntries().ToList())
                            {
                                appListEntryList.Add(new AppListEntryModel()
                                {
                                    DisplayName = appListEntryItem.DisplayInfo.DisplayName,
                                    Description = appListEntryItem.DisplayInfo.Description,
                                    AppUserModelId = appListEntryItem.AppUserModelId,
                                    AppListEntry = appListEntryItem
                                });
                            }
                            packageDict["AppListEntryList"] = appListEntryList;
                        }
                        catch
                        {
                            packageDict["AppListEntryList"] = new List<AppListEntry>();
                        }

                        try
                        {
                            List<PackageModel> dependenciesList = new List<PackageModel>();
                            IReadOnlyList<Package> dependcies = packageItem.Package.Dependencies;

                            if (dependcies.Count > 0)
                            {
                                foreach (Package packageItem in dependcies.ToList())
                                {
                                    try
                                    {
                                        dependenciesList.Add(new PackageModel()
                                        {
                                            DisplayName = packageItem.DisplayName,
                                            PublisherName = packageItem.PublisherDisplayName,
                                            Version = string.Format("{0}.{1}.{2}.{3}",
                                                packageItem.Id.Version.Major,
                                                packageItem.Id.Version.Minor,
                                                packageItem.Id.Version.Build,
                                                packageItem.Id.Version.Revision
                                                ),
                                            Package = packageItem
                                        });
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                }
                            }

                            dependenciesList = dependenciesList.OrderBy(item => item.DisplayName).ToList();
                            packageDict["DependenciesList"] = dependenciesList;
                        }
                        catch
                        {
                            packageDict["DependenciesList"] = new List<PackageModel>();
                        }

                        DispatcherQueue.TryEnqueue(() =>
                        {
                            uwpAppPage.ShowAppInformation(packageDict);
                        });
                    });
                }
            };

            OpenAppCommand.ExecuteRequested += (sender, args) =>
            {
                Package package = args.Parameter as Package;

                if (package is not null)
                {
                    Task.Run(async () =>
                    {
                        try
                        {
                            await package.GetAppListEntries()[0].LaunchAsync();
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LogLevel.ERROR, string.Format("Open app {0} failed", package.DisplayName), e);
                        }
                    });
                }
            };

            OpenStoreCommand.ExecuteRequested += (sender, args) =>
            {
                Package package = args.Parameter as Package;

                if (package is not null)
                {
                    Task.Run(async () =>
                    {
                        try
                        {
                            await Launcher.LaunchUriAsync(new Uri($"ms-windows-store://pdp/?PFN={package.Id.FamilyName}"));
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LogLevel.ERROR, string.Format("Open microsoft store {0} failed", package.DisplayName), e);
                        }
                    });
                }
            };

            GetPackageCommand.ExecuteRequested += (sender, args) =>
            {
                Package package = args.Parameter as Package;

                if (package is not null)
                {
                    NavigationService.NavigateTo(typeof(StorePage), new object[] { AppNaviagtionArgs.Store, "PackageFamilyName", "Retail", package.Id.FamilyName });
                }
            };

            OpenManifestCommand.ExecuteRequested += (sender, args) =>
            {
                Package package = args.Parameter as Package;
                if (package is not null)
                {
                    Task.Run(async () =>
                    {
                        try
                        {
                            StorageFile file = await StorageFile.GetFileFromPathAsync(Path.Combine(package.InstalledPath, "AppxManifest.xml"));
                            if (file is not null)
                            {
                                await Launcher.LaunchFileAsync(file);
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LogLevel.ERROR, string.Format("{0}'s AppxManifest.xml file open failed", package.DisplayName), e);
                        }
                    });
                }
            };

            OpenInstalledFolderCommand.ExecuteRequested += (sender, args) =>
            {
                Package package = args.Parameter as Package;

                if (package is not null)
                {
                    Task.Run(async () =>
                    {
                        try
                        {
                            await Launcher.LaunchFolderPathAsync(package.InstalledPath);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LogLevel.WARNING, string.Format("{0} app installed folder open failed", package.DisplayName), e);
                        }
                    });
                }
            };

            OpenCacheFolderCommand.ExecuteRequested += (sender, args) =>
            {
                Package package = args.Parameter as Package;

                if (package is not null)
                {
                    Task.Run(async () =>
                    {
                        try
                        {
                            string packageVolume = Path.GetPathRoot(package.InstalledPath);
                            string packageCacheFolder = Path.Combine(packageRootCacheFolder.Replace(Path.GetPathRoot(packageRootCacheFolder), packageVolume), package.Id.FamilyName);
                            await Launcher.LaunchFolderPathAsync(packageCacheFolder);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LogLevel.INFO, "Open app cache folder failed.", e);
                        }
                    });
                }
            };

            UnInstallCommand.ExecuteRequested += (sender, args) =>
            {
                Package package = args.Parameter as Package;

                if (package is not null)
                {
                    foreach (PackageModel packageItem in UwpAppDataList)
                    {
                        if (packageItem.Package.Id.FullName == package.Id.FullName)
                        {
                            lock (AppShortListObject)
                            {
                                packageItem.IsUnInstalling = true;
                                break;
                            }
                        }
                    }

                    try
                    {
                        Task.Run(() =>
                        {
                            IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> uninstallOperation = PackageManager.RemovePackageAsync(package.Id.FullName);

                            ManualResetEvent uninstallCompletedEvent = new ManualResetEvent(false);

                            uninstallOperation.Completed = (result, progress) =>
                            {
                                // 卸载成功
                                if (result.Status is AsyncStatus.Completed)
                                {
                                    DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
                                    {
                                        lock (AppShortListObject)
                                        {
                                            foreach (PackageModel pacakgeItem in UwpAppDataList)
                                            {
                                                if (pacakgeItem.Package.Id.FullName == package.Id.FullName)
                                                {
                                                    ToastNotificationService.Show(NotificationKind.UWPUnInstallSuccessfully, pacakgeItem.Package.DisplayName);

                                                    UwpAppDataList.Remove(pacakgeItem);
                                                    break;
                                                }
                                            }
                                        }
                                    });
                                }

                                // 卸载失败
                                else if (result.Status is AsyncStatus.Error)
                                {
                                    DeploymentResult uninstallResult = uninstallOperation.GetResults();

                                    DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
                                    {
                                        lock (AppShortListObject)
                                        {
                                            foreach (PackageModel pacakgeItem in UwpAppDataList)
                                            {
                                                if (pacakgeItem.Package.Id.FullName == package.Id.FullName)
                                                {
                                                    ToastNotificationService.Show(NotificationKind.UWPUnInstallFailed,
                                                        pacakgeItem.Package.DisplayName,
                                                        uninstallResult.ExtendedErrorCode.HResult.ToString(),
                                                        uninstallResult.ErrorText
                                                        );

                                                    LogService.WriteLog(LogLevel.INFO, string.Format("UnInstall app {0} failed", pacakgeItem.Package.DisplayName), uninstallResult.ExtendedErrorCode);

                                                    pacakgeItem.IsUnInstalling = false;
                                                    break;
                                                }
                                            }
                                        }
                                    });
                                }

                                uninstallCompletedEvent.Set();
                            };

                            uninstallCompletedEvent.WaitOne();
                            uninstallCompletedEvent.Dispose();
                        });
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LogLevel.INFO, string.Format("UnInstall app {0} failed", package.Id.FullName), e);
                    }
                }
            };
        }

        /// <summary>
        /// 本地化应用管理记录数量统计信息
        /// </summary>
        public string LocalizeUWPAppCountInfo(int count)
        {
            if (count is 0)
            {
                return ResourceService.GetLocalized("UWPApp/PackageEmpty");
            }
            else
            {
                return string.Format(ResourceService.GetLocalized("UWPApp/PackageCountInfo"), count);
            }
        }

        public bool IsSelected(Enum value, Enum comparedValue)
        {
            return value.HasFlag(comparedValue);
        }

        /// <summary>
        /// 初始化已安装应用信息
        /// </summary>
        public void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (!isInitialized)
            {
                Task.Delay(500);
                GetInstalledApps();
                InitializeData();
                isInitialized = true;
            }
        }

        /// <summary>
        /// 显示排序规则
        /// </summary>
        public void OnSortClicked(object sender, RoutedEventArgs args)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        /// <summary>
        /// 根据排序方式对列表进行排序
        /// </summary>
        public void OnSortWayClicked(object sender, RoutedEventArgs args)
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
        public void OnSortRuleClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem toggleMenuFlyoutItem = sender as ToggleMenuFlyoutItem;
            if (toggleMenuFlyoutItem is not null)
            {
                SelectedRule = (AppSortRuleKind)toggleMenuFlyoutItem.Tag;
                InitializeData();
            }
        }

        /// <summary>
        /// 显示过滤规则
        /// </summary>
        public void OnFilterClicked(object sender, RoutedEventArgs args)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        /// <summary>
        /// 根据过滤方式对列表进行过滤
        /// </summary>
        public void OnFilterWayClicked(object sender, RoutedEventArgs args)
        {
            IsFramework = !IsFramework;
            InitializeData();
        }

        /// <summary>
        /// 根据签名规则进行过滤
        /// </summary>
        public void OnSignatureRuleClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem toggleMenuFlyoutItem = sender as ToggleMenuFlyoutItem;
            if (toggleMenuFlyoutItem is not null)
            {
                if (SignType.HasFlag((PackageSignKind)toggleMenuFlyoutItem.Tag))
                {
                    SignType &= ~(PackageSignKind)toggleMenuFlyoutItem.Tag;
                }
                else
                {
                    SignType |= (PackageSignKind)toggleMenuFlyoutItem.Tag;
                }

                InitializeData();
            }
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        public void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            MatchResultList = null;
            IsLoadedCompleted = false;
            SearchText = string.Empty;
            Task.Delay(500);
            GetInstalledApps();
            InitializeData();
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
        private void GetInstalledApps()
        {
            autoResetEvent ??= new AutoResetEvent(false);
            Task.Run(() =>
            {
                MatchResultList = PackageManager.FindPackagesForUser(string.Empty).ToList();
                if (MatchResultList is not null)
                {
                    DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
                    {
                        IsPackageEmpty = MatchResultList.Count is 0;
                    });
                }
                autoResetEvent?.Set();
            });
        }

        /// <summary>
        /// 初始化列表数据
        /// </summary>
        public void InitializeData(bool hasSearchText = false)
        {
            lock (AppShortListObject)
            {
                IsLoadedCompleted = false;
                UwpAppDataList.Clear();
            }

            Task.Run(() =>
            {
                autoResetEvent?.WaitOne();
                autoResetEvent?.Dispose();
                autoResetEvent = null;

                if (MatchResultList is not null)
                {
                    // 备份数据
                    List<Package> backupList = MatchResultList;
                    List<Package> appTypesList;

                    // 根据选项是否筛选包含框架包的数据
                    if (IsFramework)
                    {
                        appTypesList = backupList.Where(item => item.IsFramework == IsFramework).ToList();
                    }
                    else
                    {
                        appTypesList = backupList;
                    }

                    List<Package> filteredList = new List<Package>();
                    if (SignType.HasFlag(PackageSignKind.Store))
                    {
                        filteredList.AddRange(appTypesList.Where(item => item.SignatureKind == PackageSignatureKind.Store));
                    }

                    if (SignType.HasFlag(PackageSignKind.System))
                    {
                        filteredList.AddRange(appTypesList.Where(item => item.SignatureKind == PackageSignatureKind.System));
                    }

                    if (SignType.HasFlag(PackageSignKind.Enterprise))
                    {
                        filteredList.AddRange(appTypesList.Where(item => item.SignatureKind == PackageSignatureKind.Enterprise));
                    }

                    if (SignType.HasFlag(PackageSignKind.Developer))
                    {
                        filteredList.AddRange(appTypesList.Where(item => item.SignatureKind == PackageSignatureKind.Developer));
                    }

                    if (SignType.HasFlag(PackageSignKind.None))
                    {
                        filteredList.AddRange(appTypesList.Where(item => item.SignatureKind == PackageSignatureKind.None));
                    }

                    // 对过滤后的列表数据进行排序
                    switch (SelectedRule)
                    {
                        case AppSortRuleKind.DisplayName:
                            {
                                if (IsIncrease)
                                {
                                    filteredList = filteredList.OrderBy(item => item.DisplayName).ToList();
                                }
                                else
                                {
                                    filteredList = filteredList.OrderByDescending(item => item.DisplayName).ToList();
                                }
                                break;
                            }
                        case AppSortRuleKind.PublisherName:
                            {
                                if (IsIncrease)
                                {
                                    filteredList = filteredList.OrderBy(item => item.PublisherDisplayName).ToList();
                                }
                                else
                                {
                                    filteredList = filteredList.OrderByDescending(item => item.PublisherDisplayName).ToList();
                                }
                                break;
                            }
                        case AppSortRuleKind.InstallDate:
                            {
                                if (IsIncrease)
                                {
                                    filteredList = filteredList.OrderBy(item => item.InstalledDate).ToList();
                                }
                                else
                                {
                                    filteredList = filteredList.OrderByDescending(item => item.InstalledDate).ToList();
                                }
                                break;
                            }
                    }

                    List<PackageModel> packageList = new List<PackageModel>();

                    // 根据搜索条件对搜索符合要求的数据
                    if (hasSearchText)
                    {
                        filteredList = filteredList.Where(matchItem =>
                                matchItem.DisplayName.Contains(SearchText) ||
                                matchItem.Description.Contains(SearchText) ||
                                matchItem.PublisherDisplayName.Contains(SearchText)).ToList();
                    }

                    foreach (Package packageItem in filteredList)
                    {
                        packageList.Add(new PackageModel()
                        {
                            IsFramework = GetIsFramework(packageItem),
                            AppListEntryCount = GetAppListEntriesCount(packageItem),
                            DisplayName = GetDisplayName(packageItem),
                            InstallDate = GetInstallDate(packageItem),
                            PublisherName = GetPublisherName(packageItem),
                            Version = GetVersion(packageItem),
                            SignatureKind = GetSignatureKind(packageItem),
                            InstalledDate = GetInstalledDate(packageItem),
                            Package = packageItem,
                            IsUnInstalling = false
                        });
                    }

                    DispatcherQueue.TryEnqueue(() =>
                    {
                        lock (AppShortListObject)
                        {
                            foreach (PackageModel packageItem in packageList)
                            {
                                UwpAppDataList.Add(packageItem);
                            }
                        }

                        IsLoadedCompleted = true;
                    });
                }
            });
        }

        /// <summary>
        /// 获取应用包是否为框架包
        /// </summary>
        public static bool GetIsFramework(Package package)
        {
            try
            {
                return package.IsFramework;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取应用包的入口数
        /// </summary>
        public static int GetAppListEntriesCount(Package package)
        {
            try
            {
                return package.GetAppListEntries().Count;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取应用的显示名称
        /// </summary>
        public string GetDisplayName(Package package)
        {
            try
            {
                return string.IsNullOrEmpty(package.DisplayName) ? Unknown : package.DisplayName;
            }
            catch
            {
                return Unknown;
            }
        }

        /// <summary>
        /// 获取应用的发布者显示名称
        /// </summary>
        public string GetPublisherName(Package package)
        {
            try
            {
                return string.IsNullOrEmpty(package.PublisherDisplayName) ? Unknown : package.PublisherDisplayName;
            }
            catch
            {
                return Unknown;
            }
        }

        /// <summary>
        /// 获取应用的版本信息
        /// </summary>
        public string GetVersion(Package package)
        {
            try
            {
                return string.Format("{0}.{1}.{2}.{3}", package.Id.Version.Major, package.Id.Version.Minor, package.Id.Version.Build, package.Id.Version.Revision);
            }
            catch
            {
                return "0.0.0.0";
            }
        }

        /// <summary>
        /// 获取应用的安装日期
        /// </summary>
        public string GetInstallDate(Package package)
        {
            try
            {
                return package.InstalledDate.ToString("yyyy/MM/dd HH:mm", StringConverterHelper.AppCulture);
            }
            catch
            {
                return "1970/01/01 00:00";
            }
        }

        /// <summary>
        /// 获取应用包签名方式
        /// </summary>
        public PackageSignatureKind GetSignatureKind(Package package)
        {
            try
            {
                return package.SignatureKind;
            }
            catch
            {
                return PackageSignatureKind.None;
            }
        }

        public DateTimeOffset GetInstalledDate(Package package)
        {
            try
            {
                return package.InstalledDate;
            }
            catch
            {
                return new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
            }
        }
    }
}
