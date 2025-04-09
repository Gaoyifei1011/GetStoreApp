using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.WinGet;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.WinGet;
using GetStoreApp.UI.TeachingTips;
using GetStoreApp.Views.Windows;
using Microsoft.Management.Deployment;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.Windows.AppNotifications.Builder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.UI.Controls.WinGet
{
    /// <summary>
    /// WinGet 程序包页面：已安装应用控件
    /// </summary>
    public sealed partial class InstalledAppsControl : Grid, INotifyPropertyChanged
    {
        private readonly string InstalledAppsCountInfo = ResourceService.GetLocalized("WinGet/InstalledAppsCountInfo");
        private readonly string Unknown = ResourceService.GetLocalized("WinGet/Unknown");
        private bool isInitialized;
        private PackageManager installedAppsManager;

        private bool _isLoadedCompleted;

        public bool IsLoadedCompleted
        {
            get { return _isLoadedCompleted; }

            set
            {
                if (!Equals(_isLoadedCompleted, value))
                {
                    _isLoadedCompleted = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoadedCompleted)));
                }
            }
        }

        private bool _isInstalledAppsEmpty;

        public bool IsInstalledAppsEmpty
        {
            get { return _isInstalledAppsEmpty; }

            set
            {
                if (!Equals(_isInstalledAppsEmpty, value))
                {
                    _isInstalledAppsEmpty = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsInstalledAppsEmpty)));
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

        private AppSortRuleKind _selectedRule = AppSortRuleKind.DisplayName;

        public AppSortRuleKind SelectedRule
        {
            get { return _selectedRule; }

            set
            {
                if (!Equals(_selectedRule, value))
                {
                    _selectedRule = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedRule)));
                }
            }
        }

        private List<MatchResult> MatchResultList { get; } = [];

        private ObservableCollection<InstalledAppsModel> InstalledAppsCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public InstalledAppsControl()
        {
            InitializeComponent();
        }

        #region 第一部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 复制卸载命令
        /// </summary>
        private async void OnCopyUnInstallTextExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string appId && !string.IsNullOrEmpty(appId))
            {
                string copyContent = string.Format("winget uninstall {0}", appId);
                bool copyResult = CopyPasteHelper.CopyTextToClipBoard(copyContent);

                await MainWindow.Current.ShowNotificationAsync(new MainDataCopyTip(DataCopyKind.WinGetUnInstall, copyResult));
            }
        }

        /// <summary>
        /// 卸载应用
        /// </summary>
        private void OnUnInstallExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is InstalledAppsModel installedApps)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        // TODO：添加修复操作 RepairOptions
                        // TODO：添加 PackageManagerSettings
                        UninstallResult unInstallResult = await installedAppsManager.UninstallPackageAsync(MatchResultList.Find(item => item.CatalogPackage.InstalledVersion.Id == installedApps.AppID).CatalogPackage, new()
                        {
                            // TODO：未完成
                            PackageUninstallMode = PackageUninstallMode.Interactive,
                            PackageUninstallScope = PackageUninstallScope.Any
                        });

                        // 获取卸载后的结果信息
                        // 卸载成功，从列表中删除该应用
                        if (unInstallResult.Status is UninstallResultStatus.Ok)
                        {
                            // 显示 WinGet 应用卸载成功通知
                            AppNotificationBuilder appNotificationBuilder = new();
                            appNotificationBuilder.AddArgument("action", "OpenApp");
                            appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/WinGetUnInstallSuccessfully"), installedApps.AppName));
                            ToastNotificationService.Show(appNotificationBuilder.BuildNotification());

                            DispatcherQueue.TryEnqueue(async () =>
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

                                // 检测是否需要重启设备完成应用的卸载，如果是，询问用户是否需要重启设备
                                if (unInstallResult.RebootRequired)
                                {
                                    ContentDialogResult contentDialogResult = await MainWindow.Current.ShowDialogAsync(new RebootDialog(WinGetOptionKind.UnInstall, installedApps.AppName));

                                    await Task.Run(() =>
                                    {
                                        if (contentDialogResult is ContentDialogResult.Primary)
                                        {
                                            ShutdownHelper.Restart(ResourceService.GetLocalized("WinGet/RestartPC"), TimeSpan.FromSeconds(120));
                                        }
                                    });
                                }
                            });
                        }
                        else
                        {
                            // 显示 WinGet 应用卸载失败通知
                            AppNotificationBuilder appNotificationBuilder = new();
                            appNotificationBuilder.AddArgument("action", "OpenApp");
                            appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/WinGetUnInstallFailed1"), installedApps.AppName));
                            appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/WinGetUpgradeFailed2"));
                            appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/WinGetUpgradeFailed3"));
                            AppNotificationButton openSettingsButton = new(ResourceService.GetLocalized("Notification/OpenSettings"));
                            openSettingsButton.Arguments.Add("action", "OpenSettings");
                            appNotificationBuilder.AddButton(openSettingsButton);
                            ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                        }
                    }
                    // 操作被用户所取消异常
                    catch (OperationCanceledException e)
                    {
                        LogService.WriteLog(LoggingLevel.Information, "App uninstalling operation canceled.", e);

                        // 显示 WinGet 应用升级失败通知
                        AppNotificationBuilder appNotificationBuilder = new();
                        appNotificationBuilder.AddArgument("action", "OpenApp");
                        appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/WinGetUnInstallFailed1"), installedApps.AppName));
                        appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/WinGetUpgradeFailed2"));
                        appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/WinGetUpgradeFailed3"));
                        AppNotificationButton openSettingsButton = new(ResourceService.GetLocalized("Notification/OpenSettings"));
                        openSettingsButton.Arguments.Add("action", "OpenSettings");
                        appNotificationBuilder.AddButton(openSettingsButton);
                        ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                    }
                    // 其他异常
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "App uninstalling failed.", e);

                        // 显示 WinGet 应用升级失败通知
                        AppNotificationBuilder appNotificationBuilder = new();
                        appNotificationBuilder.AddArgument("action", "OpenApp");
                        appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/WinGetUnInstallFailed1"), installedApps.AppName));
                        appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/WinGetUpgradeFailed2"));
                        appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/WinGetUpgradeFailed3"));
                        AppNotificationButton openSettingsButton = new(ResourceService.GetLocalized("Notification/OpenSettings"));
                        openSettingsButton.Arguments.Add("action", "OpenSettings");
                        appNotificationBuilder.AddButton(openSettingsButton);
                        ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                    }
                });
            }
        }

        #endregion 第一部分：XamlUICommand 命令调用时挂载的事件

        #region 第二部分：已安装应用控件——挂载的事件

        /// <summary>
        /// 已安装应用控件初始化完成后触发的事件
        /// </summary>
        private async void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (!isInitialized)
            {
                isInitialized = true;

                try
                {
                    installedAppsManager = new();
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "Installed apps information initialized failed.", e);
                    return;
                }

                await GetInstalledAppsAsync();
                await InitializeDataAsync();
            }
        }

        /// <summary>
        /// 根据排序方式对列表进行排序
        /// </summary>
        private async void OnSortWayClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem)
            {
                IsIncrease = Convert.ToBoolean(radioMenuFlyoutItem.Tag);
                await InitializeDataAsync();
            }
        }

        /// <summary>
        /// 根据排序规则对列表进行排序
        /// </summary>
        private async void OnSortRuleClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem)
            {
                SelectedRule = (AppSortRuleKind)radioMenuFlyoutItem.Tag;
                await InitializeDataAsync();
            }
        }

        /// <summary>
        /// 更新已安装应用数据
        /// </summary>
        private async void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            MatchResultList.Clear();
            IsLoadedCompleted = false;
            SearchText = string.Empty;
            await GetInstalledAppsAsync();
            await InitializeDataAsync();
        }

        /// <summary>
        /// 根据输入的内容检索应用
        /// </summary>
        private async void OnQuerySubmitted(object sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrEmpty(SearchText))
            {
                await InitializeDataAsync(true);
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
                if (string.IsNullOrEmpty(SearchText) && MatchResultList.Count > 0)
                {
                    await InitializeDataAsync();
                }
            }
        }

        #endregion 第二部分：已安装应用控件——挂载的事件

        /// <summary>
        /// 加载系统已安装的应用信息
        /// </summary>
        private async Task GetInstalledAppsAsync()
        {
            await Task.Run(async () =>
            {
                try
                {
                    PackageCatalogReference packageCatalogReference = installedAppsManager.GetLocalPackageCatalog(LocalPackageCatalog.InstalledPackages);
                    ConnectResult connectResult = await packageCatalogReference.ConnectAsync();

                    if (connectResult.Status is ConnectResultStatus.Ok)
                    {
                        FindPackagesOptions findPackagesOptions = new();
                        FindPackagesResult findResult = await connectResult.PackageCatalog.FindPackagesAsync(findPackagesOptions);

                        IReadOnlyList<MatchResult> list = findResult.Matches;

                        for (int index = 0; index < list.Count; index++)
                        {
                            MatchResult matchResultItem = list[index];
                            if (!string.IsNullOrEmpty(matchResultItem.CatalogPackage.InstalledVersion.Publisher))
                            {
                                MatchResultList.Add(matchResultItem);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "Get installed apps information failed.", e);
                }
            });
        }

        /// <summary>
        /// 初始化列表数据
        /// </summary>
        private async Task InitializeDataAsync(bool hasSearchText = false)
        {
            InstalledAppsCollection.Clear();
            if (MatchResultList.Count > 0)
            {
                List<InstalledAppsModel> installedAppsList = [];

                await Task.Run(() =>
                {
                    try
                    {
                        if (hasSearchText)
                        {
                            foreach (MatchResult matchItem in MatchResultList)
                            {
                                if (matchItem.CatalogPackage.InstalledVersion.DisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                                {
                                    installedAppsList.Add(new InstalledAppsModel()
                                    {
                                        AppID = matchItem.CatalogPackage.InstalledVersion.Id,
                                        AppName = string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.DisplayName) ? Unknown : matchItem.CatalogPackage.InstalledVersion.DisplayName,
                                        AppPublisher = string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.Publisher) ? Unknown : matchItem.CatalogPackage.InstalledVersion.Publisher,
                                        AppVersion = string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.Version) ? Unknown : matchItem.CatalogPackage.InstalledVersion.Version,
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
                                    AppName = string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.DisplayName) ? Unknown : matchItem.CatalogPackage.InstalledVersion.DisplayName,
                                    AppPublisher = string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.Publisher) ? Unknown : matchItem.CatalogPackage.InstalledVersion.Publisher,
                                    AppVersion = string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.Version) ? Unknown : matchItem.CatalogPackage.InstalledVersion.Version,
                                });
                            }
                        }

                        switch (SelectedRule)
                        {
                            case AppSortRuleKind.DisplayName:
                                {
                                    if (IsIncrease)
                                    {
                                        installedAppsList.Sort((item1, item2) => item1.AppName.CompareTo(item2.AppName));
                                    }
                                    else
                                    {
                                        installedAppsList.Sort((item1, item2) => item2.AppName.CompareTo(item1.AppName));
                                    }
                                    break;
                                }
                            case AppSortRuleKind.PublisherName:
                                {
                                    if (IsIncrease)
                                    {
                                        installedAppsList.Sort((item1, item2) => item1.AppPublisher.CompareTo(item2.AppPublisher));
                                    }
                                    else
                                    {
                                        installedAppsList.Sort((item1, item2) => item2.AppPublisher.CompareTo(item1.AppPublisher));
                                    }
                                    break;
                                }
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "Initialize installed apps data failed", e);
                    }
                });

                foreach (InstalledAppsModel installedApps in installedAppsList)
                {
                    InstalledAppsCollection.Add(installedApps);
                }
                IsInstalledAppsEmpty = MatchResultList.Count is 0;
            }
            else
            {
                IsInstalledAppsEmpty = true;
            }

            IsLoadedCompleted = true;
        }
    }
}
