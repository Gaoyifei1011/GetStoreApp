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
using System.Runtime.InteropServices.Marshalling;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;
using Windows.System;
using WinRT;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// WinGet 更新应用界面
    /// </summary>
    public sealed partial class WinGetUpgradePage : Page, INotifyPropertyChanged
    {
        private readonly string NotAvailableString = ResourceService.GetLocalized("WinGetUpgrade/NotAvailable");
        private readonly string UpgradableAppsCountInfoString = ResourceService.GetLocalized("WinGetUpgrade/UpgradableAppsCountInfo");
        private readonly string UpgradableAppsEmptyDescriptionString = ResourceService.GetLocalized("WinGetUpgrade/UpgradableAppsEmptyDescription");
        private readonly string UpgradableAppsFailedString = ResourceService.GetLocalized("WinGetUpgrade/UpgradableAppsFailed");
        private readonly string UpgradableCatalogReferenceFailedString = ResourceService.GetLocalized("WinGetUpgrade/UpgradableCatalogReferenceFailed");
        private readonly string UpgradableFindAppsFailedString = ResourceService.GetLocalized("WinGetUpgrade/UpgradableFindAppsFailed");
        private readonly string UpgradableNotSelectSourceString = ResourceService.GetLocalized("WinGetUpgrade/UpgradableNotSelectSource");
        private readonly Lock UpgradableAppsLock = new();
        private WinGetPage WinGetPageInstance;

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

        private UpgradableAppsResultKind _upgradableAppsResultKind = UpgradableAppsResultKind.NotCheckUpdate;

        public UpgradableAppsResultKind UpgradableAppsResultKind
        {
            get { return _upgradableAppsResultKind; }

            set
            {
                if (!Equals(_upgradableAppsResultKind, value))
                {
                    _upgradableAppsResultKind = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UpgradableAppsResultKind)));
                }
            }
        }

        private string _upgradableFailedContent;

        public string UpgradableFailedContent
        {
            get { return _upgradableFailedContent; }

            set
            {
                if (!string.Equals(_upgradableFailedContent, value))
                {
                    _upgradableFailedContent = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UpgradableFailedContent)));
                }
            }
        }

        private ObservableCollection<UpgradableAppsModel> UpgradableAppsCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public WinGetUpgradePage()
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
                GlobalNotificationService.ApplicationExit += OnApplicationExit;
                WinGetPageInstance.UpgradeAppsPackageOperationEvent += OnUpgradeAppsPackageOperationEvent;
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 应用更新
        /// </summary>
        private async void OnUpgradeExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is UpgradableAppsModel upgradableApps)
            {
                // 禁用当前应用的可更新状态
                UpgradableAppsLock.Enter();
                try
                {
                    upgradableApps.IsUpgrading = true;
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
                finally
                {
                    UpgradableAppsLock.Exit();
                }

                InstallOptions installOptions = await Task.Run(WinGetFactoryHelper.CreateInstallOptions);

                await WinGetPageInstance.AddTaskAsync(new PackageOperationModel()
                {
                    PackageOperationKind = PackageOperationKind.Upgrade,
                    AppID = upgradableApps.AppID,
                    AppName = upgradableApps.AppName,
                    AppVersion = upgradableApps.CatalogPackage.DefaultInstallVersion.Version,
                    PackagePath = Path.Combine(Path.GetTempPath(), "WinGet"),
                    PackageOperationProgress = 0,
                    PackageInstallProgressState = PackageInstallProgressState.Queued,
                    PackageVersionId = null,
                    DownloadedFileSize = VolumeSizeHelper.ConvertVolumeSizeToString(0),
                    TotalFileSize = VolumeSizeHelper.ConvertVolumeSizeToString(0),
                    PackageInstallProgress = null,
                    UpgradableApps = upgradableApps,
                    InstallOptions = installOptions,
                });
            }
        }

        /// <summary>
        /// 查看版本信息
        /// </summary>

        private async void OnViewVersionInfoExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is UpgradableAppsModel upgradableApps && WinGetPageInstance is not null)
            {
                await MainWindow.Current.ShowDialogAsync(new WinGetAppsVersionDialog(WinGetOperationKind.VersionInfo, WinGetPageInstance, upgradableApps));
            }
        }

        #endregion 第二部分：XamlUICommand 命令调用时挂载的事件

        #region 第三部分：WinGet 更新应用界面——挂载的事件

        /// <summary>
        /// 根据排序方式对列表进行排序
        /// </summary>
        private void OnSortWayClicked(object sender, RoutedEventArgs args)
        {
            if (sender.As<RadioMenuFlyoutItem>().Tag is string increase && UpgradableAppsResultKind is UpgradableAppsResultKind.Successfully)
            {
                IsIncrease = Convert.ToBoolean(increase);
                InitializeMatchedUpgradableApps();
            }
        }

        /// <summary>
        /// 根据排序规则对列表进行排序
        /// </summary>
        private void OnSortRuleClicked(object sender, RoutedEventArgs args)
        {
            if (sender.As<RadioMenuFlyoutItem>().Tag is AppSortRuleKind appSortRuleKind && UpgradableAppsResultKind is UpgradableAppsResultKind.Successfully)
            {
                SelectedAppSortRuleKind = appSortRuleKind;
                InitializeMatchedUpgradableApps();
            }
        }

        /// <summary>
        /// 检查更新
        /// </summary>
        private async void OnCheckUpdateClicked(object sender, RoutedEventArgs args)
        {
            await InitializeUpgradableAppsDataAsync();
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
                string wingetTempPath = Path.Combine(Path.GetTempPath(), "WinGet");
                if (Directory.Exists(wingetTempPath))
                {
                    await Launcher.LaunchFolderPathAsync(wingetTempPath);
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
        /// 配置 WinGet 数据源
        /// </summary>
        private void OnDataSourceSettingsClicked(object sender, RoutedEventArgs args)
        {
            MainWindow.Current.NavigateTo(typeof(SettingsPage), AppNaviagtionArgs.WinGetDataSource);
        }

        #endregion 第三部分：WinGet 更新应用界面——挂载的事件

        #region 第四部分：WinGet 更新应用界面——自定义事件

        /// <summary>
        /// 应用程序退出时触发的事件
        /// </summary>
        private void OnApplicationExit()
        {
            try
            {
                GlobalNotificationService.ApplicationExit -= OnApplicationExit;
                WinGetPageInstance.UpgradeAppsPackageOperationEvent -= OnUpgradeAppsPackageOperationEvent;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetUpgradePage), nameof(OnApplicationExit), 1, e);
            }
        }

        /// <summary>
        /// 可更新项目安装完成后发生的事件
        /// </summary>
        private void OnUpgradeAppsPackageOperationEvent(bool result, bool isCanceled, UpgradableAppsModel upgradableApps, InstallResult installResult)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                UpgradableAppsLock.Enter();

                try
                {
                    if (result && installResult.Status is InstallResultStatus.Ok)
                    {
                        foreach (UpgradableAppsModel upgradableAppsItem in UpgradableAppsCollection)
                        {
                            if (string.Equals(upgradableApps.AppID, upgradableAppsItem.AppID) && string.Equals(upgradableApps.AppCurrentVersion, upgradableAppsItem.AppCurrentVersion) && Equals(upgradableApps.AppNewestVersion, upgradableApps.AppNewestVersion))
                            {
                                UpgradableAppsCollection.Remove(upgradableApps);
                                break;
                            }
                        }

                        if (UpgradableAppsCollection.Count is 0)
                        {
                            UpgradableAppsResultKind = UpgradableAppsResultKind.Failed;
                            UpgradableFailedContent = UpgradableAppsEmptyDescriptionString;
                        }
                        else
                        {
                            UpgradableAppsResultKind = UpgradableAppsResultKind.Successfully;
                        }
                    }
                    else
                    {
                        foreach (UpgradableAppsModel upgradableAppsItem in UpgradableAppsCollection)
                        {
                            if (string.Equals(upgradableApps.AppID, upgradableAppsItem.AppID) && string.Equals(upgradableApps.AppCurrentVersion, upgradableAppsItem.AppCurrentVersion) && Equals(upgradableApps.AppNewestVersion, upgradableApps.AppNewestVersion))
                            {
                                upgradableAppsItem.IsUpgrading = false;
                                break;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
                finally
                {
                    UpgradableAppsLock.Exit();
                }
            });
        }

        #endregion 第四部分：WinGet 更新应用界面——自定义事件

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
                                CreateCompositePackageCatalogOptions createCompositePackageCatalogOptions = WinGetFactoryHelper.CreateCreateCompositePackageCatalogOptions();
                                createCompositePackageCatalogOptions.CompositeSearchBehavior = CompositeSearchBehavior.LocalCatalogs;
                                createCompositePackageCatalogOptions.Catalogs.Add(packageCatalogReference);
                                packageCatalogReference = packageManager.CreateCompositePackageCatalog(createCompositePackageCatalogOptions);
                                break;
                            }
                        }
                    }
                    // 使用自定义源
                    else
                    {
                        packageCatalogReference = packageManager.GetPackageCatalogByName(winGetDataSourceName.Key);

                        if (packageCatalogReference is not null)
                        {
                            CreateCompositePackageCatalogOptions createCompositePackageCatalogOptions = WinGetFactoryHelper.CreateCreateCompositePackageCatalogOptions();
                            createCompositePackageCatalogOptions.CompositeSearchBehavior = CompositeSearchBehavior.LocalCatalogs;
                            createCompositePackageCatalogOptions.Catalogs.Add(packageCatalogReference);
                            packageCatalogReference = packageManager.CreateCompositePackageCatalog(createCompositePackageCatalogOptions);
                        }
                    }
                }

                return packageCatalogReference;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetUpgradePage), nameof(GetPackageCatalogReference), 1, e);
                return packageCatalogReference;
            }
        }

        /// <summary>
        /// 初始化可更新应用数据
        /// </summary>
        private async Task InitializeUpgradableAppsDataAsync()
        {
            UpgradableAppsResultKind = UpgradableAppsResultKind.Querying;
            UpgradableAppsCollection.Clear();

            PackageManager packageManager = await Task.Run(WinGetFactoryHelper.CreatePackageManager);

            PackageCatalogReference packageCatalogReference = await Task.Run(() =>
            {
                return GetPackageCatalogReference(packageManager);
            });

            if (packageCatalogReference is not null)
            {
                (ConnectResult connectResult, FindPackagesResult findPackagesResult, List<UpgradableAppsModel> upgradableAppsList) = await Task.Run(() =>
                {
                    return UpgradableAppsAsync(packageCatalogReference);
                });

                if (connectResult.Status is ConnectResultStatus.Ok)
                {
                    if (findPackagesResult.Status is FindPackagesResultStatus.Ok)
                    {
                        if (upgradableAppsList.Count is 0)
                        {
                            UpgradableAppsResultKind = UpgradableAppsResultKind.Failed;
                            UpgradableFailedContent = UpgradableAppsEmptyDescriptionString;
                        }
                        else
                        {
                            foreach (UpgradableAppsModel upgradableAppsItem in upgradableAppsList)
                            {
                                UpgradableAppsCollection.Add(upgradableAppsItem);
                            }

                            UpgradableAppsResultKind = UpgradableAppsResultKind.Successfully;
                        }
                    }
                    else
                    {
                        UpgradableAppsResultKind = UpgradableAppsResultKind.Failed;
                        UpgradableFailedContent = string.Format(UpgradableAppsFailedString, UpgradableFindAppsFailedString, findPackagesResult.ExtendedErrorCode is not null ? "0x" + Convert.ToString(findPackagesResult.ExtendedErrorCode.HResult, 16).ToUpperInvariant() : NotAvailableString);
                    }
                }
                else
                {
                    UpgradableAppsResultKind = UpgradableAppsResultKind.Failed;
                    UpgradableFailedContent = string.Format(UpgradableAppsFailedString, UpgradableCatalogReferenceFailedString, findPackagesResult.ExtendedErrorCode is not null ? "0x" + Convert.ToString(findPackagesResult.ExtendedErrorCode.HResult, 16).ToUpperInvariant() : NotAvailableString);
                }
            }
            else
            {
                UpgradableAppsResultKind = UpgradableAppsResultKind.Failed;
                UpgradableFailedContent = UpgradableNotSelectSourceString;
            }
        }

        /// <summary>
        /// 初始化符合的可更新应用结果
        /// </summary>
        private void InitializeMatchedUpgradableApps()
        {
            UpgradableAppsResultKind = UpgradableAppsResultKind.Querying;
            List<UpgradableAppsModel> upgradableAppsList = [.. UpgradableAppsCollection];
            UpgradableAppsCollection.Clear();
            if (SelectedAppSortRuleKind is AppSortRuleKind.DisplayName)
            {
                if (IsIncrease)
                {
                    upgradableAppsList.Sort((item1, item2) => item1.AppName.CompareTo(item2.AppName));
                }
                else
                {
                    upgradableAppsList.Sort((item1, item2) => item2.AppName.CompareTo(item1.AppName));
                }
            }
            else
            {
                if (IsIncrease)
                {
                    upgradableAppsList.Sort((item1, item2) => item1.AppPublisher.CompareTo(item2.AppPublisher));
                }
                else
                {
                    upgradableAppsList.Sort((item1, item2) => item2.AppPublisher.CompareTo(item1.AppPublisher));
                }
            }

            foreach (UpgradableAppsModel upgradableAppsItem in upgradableAppsList)
            {
                UpgradableAppsCollection.Add(upgradableAppsItem);
            }
            UpgradableAppsResultKind = UpgradableAppsResultKind.Successfully;
        }

        /// <summary>
        /// 获取可更新应用
        /// </summary>
        private async Task<(ConnectResult, FindPackagesResult, List<UpgradableAppsModel>)> UpgradableAppsAsync(PackageCatalogReference packageCatalogReference)
        {
            (ConnectResult connectResult, FindPackagesResult findPackagesResult, List<UpgradableAppsModel> upgradableAppsList) upgradableAppsResult = ValueTuple.Create<ConnectResult, FindPackagesResult, List<UpgradableAppsModel>>(null, null, null);

            try
            {
                ConnectResult connectResult = await packageCatalogReference.ConnectAsync();
                upgradableAppsResult.connectResult = connectResult;

                if (connectResult is not null && connectResult.Status is ConnectResultStatus.Ok)
                {
                    FindPackagesOptions findPackagesOptions = WinGetFactoryHelper.CreateFindPackagesOptions();
                    FindPackagesResult findPackagesResult = await connectResult.PackageCatalog.FindPackagesAsync(findPackagesOptions);
                    upgradableAppsResult.findPackagesResult = findPackagesResult;

                    if (findPackagesResult is not null && findPackagesResult.Status is FindPackagesResultStatus.Ok)
                    {
                        List<UpgradableAppsModel> upgradableAppsList = [];

                        for (int index = 0; index < findPackagesResult.Matches.Count; index++)
                        {
                            MatchResult matchItem = findPackagesResult.Matches[index];

                            if (matchItem.CatalogPackage is not null && matchItem.CatalogPackage.IsUpdateAvailable)
                            {
                                bool isUpgrading = false;
                                WinGetPageInstance.PackageOperationLock.Enter();
                                try
                                {
                                    foreach (PackageOperationModel packageOperationItem in WinGetPageInstance.PackageOperationCollection)
                                    {
                                        if (string.Equals(matchItem.CatalogPackage.DefaultInstallVersion.Id, packageOperationItem.AppID) && string.Equals(matchItem.CatalogPackage.DefaultInstallVersion.Version, packageOperationItem.AppVersion) && packageOperationItem.PackageOperationKind is PackageOperationKind.Upgrade)
                                        {
                                            isUpgrading = true;
                                            break;
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                                }
                                finally
                                {
                                    WinGetPageInstance.PackageOperationLock.Exit();
                                }

                                upgradableAppsList.Add(new UpgradableAppsModel()
                                {
                                    AppID = matchItem.CatalogPackage.DefaultInstallVersion.Id,
                                    AppName = string.IsNullOrEmpty(matchItem.CatalogPackage.DefaultInstallVersion.DisplayName) ? NotAvailableString : matchItem.CatalogPackage.DefaultInstallVersion.DisplayName,
                                    AppPublisher = string.IsNullOrEmpty(matchItem.CatalogPackage.DefaultInstallVersion.Publisher) ? NotAvailableString : matchItem.CatalogPackage.DefaultInstallVersion.Publisher,
                                    AppCurrentVersion = string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.Version) ? NotAvailableString : matchItem.CatalogPackage.InstalledVersion.Version,
                                    AppNewestVersion = string.IsNullOrEmpty(matchItem.CatalogPackage.DefaultInstallVersion.Version) ? NotAvailableString : matchItem.CatalogPackage.DefaultInstallVersion.Version,
                                    IsUpgrading = isUpgrading,
                                    CatalogPackage = matchItem.CatalogPackage,
                                });
                            }
                        }

                        if (SelectedAppSortRuleKind is AppSortRuleKind.DisplayName)
                        {
                            if (IsIncrease)
                            {
                                upgradableAppsList.Sort((item1, item2) => item1.AppName.CompareTo(item2.AppName));
                            }
                            else
                            {
                                upgradableAppsList.Sort((item1, item2) => item2.AppName.CompareTo(item1.AppName));
                            }
                        }
                        else
                        {
                            if (IsIncrease)
                            {
                                upgradableAppsList.Sort((item1, item2) => item1.AppPublisher.CompareTo(item2.AppPublisher));
                            }
                            else
                            {
                                upgradableAppsList.Sort((item1, item2) => item2.AppPublisher.CompareTo(item1.AppPublisher));
                            }
                        }

                        upgradableAppsResult.upgradableAppsList = upgradableAppsList;
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetUpgradePage), nameof(UpgradableAppsAsync), 1, e);
            }

            return upgradableAppsResult;
        }

        /// <summary>
        /// 获取可更新应用是否成功
        /// </summary>
        private Visibility GetUpgradableAppsSuccessfullyState(UpgradableAppsResultKind upgradableAppsResultKind, bool isSuccessfully)
        {
            return isSuccessfully ? upgradableAppsResultKind is UpgradableAppsResultKind.Successfully ? Visibility.Visible : Visibility.Collapsed : upgradableAppsResultKind is not UpgradableAppsResultKind.Successfully ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 检查可更新应用是否成功
        /// </summary>
        private Visibility CheckUpgradableAppsState(UpgradableAppsResultKind upgradableAppsResultKind, UpgradableAppsResultKind comparedSearchAppsResultKind)
        {
            return Equals(upgradableAppsResultKind, comparedSearchAppsResultKind) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取是否正在检查更新中
        /// </summary>

        private bool GetIsCheckingUpdate(UpgradableAppsResultKind upgradableAppsResultKind)
        {
            return upgradableAppsResultKind is not UpgradableAppsResultKind.Querying;
        }
    }
}
