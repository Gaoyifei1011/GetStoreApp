using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.WinGet;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.WinGet;
using GetStoreApp.UI.TeachingTips;
using GetStoreApp.Views.Pages;
using GetStoreApp.Views.Windows;
using GetStoreApp.WindowsAPI.ComTypes;
using GetStoreApp.WindowsAPI.PInvoke.Ole32;
using Microsoft.Management.Deployment;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.Windows.AppNotifications.Builder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;
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
        private readonly string InstalledAppsEmptyDescription = ResourceService.GetLocalized("WinGet/InstalledAppsEmptyDescription");
        private readonly string InstalledAppsFailed = ResourceService.GetLocalized("WinGet/InstalledAppsFailed");
        private readonly string InstalledFindAppsFailed = ResourceService.GetLocalized("WinGet/InstalledFindAppsFailed");
        private readonly string InstalledCatalogReferenceFailed = ResourceService.GetLocalized("WinGet/InstalledCatalogReferenceFailed");
        private readonly string InstalledNotSelectSource = ResourceService.GetLocalized("WinGet/InstalledNotSelectSource");
        private readonly Guid CLSID_OpenControlPanel = new("06622D85-6856-4460-8DE1-A81921B41C4B");
        private bool isInitialized;
        private IOpenControlPanel openControlPanel;
        private WinGetPage WinGetInstance;

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

        private InstalledAppsResultKind _installedAppsResultKind;

        public InstalledAppsResultKind InstalledAppsResultKind
        {
            get { return _installedAppsResultKind; }

            set
            {
                if (!Equals(_installedAppsResultKind, value))
                {
                    _installedAppsResultKind = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InstalledAppsResultKind)));
                }
            }
        }

        private string _installedFailedContent;

        public string InstalledFailedContent
        {
            get { return _installedFailedContent; }

            set
            {
                if (!Equals(_installedFailedContent, value))
                {
                    _installedFailedContent = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InstalledFailedContent)));
                }
            }
        }

        private List<InstalledAppsModel> InstalledAppsList { get; } = [];

        private ObservableCollection<InstalledAppsModel> InstalledAppsCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public InstalledAppsControl()
        {
            InitializeComponent();

            Task.Run(() =>
            {
                int createResult = Ole32Library.CoCreateInstance(CLSID_OpenControlPanel, IntPtr.Zero, CLSCTX.CLSCTX_INPROC_SERVER | CLSCTX.CLSCTX_INPROC_HANDLER | CLSCTX.CLSCTX_LOCAL_SERVER | CLSCTX.CLSCTX_REMOTE_SERVER, typeof(IOpenControlPanel).GUID, out IntPtr ppv);

                if (createResult is 0)
                {
                    openControlPanel = (IOpenControlPanel)Program.StrategyBasedComWrappers.GetOrCreateObjectForComInstance(ppv, CreateObjectFlags.Unwrap);
                }
            });
        }

        #region 第一部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 复制卸载命令
        /// </summary>
        private async void OnCopyUninstallTextExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string appId && !string.IsNullOrEmpty(appId))
            {
                string copyContent = string.Format("winget uninstall {0}", appId);
                bool copyResult = CopyPasteHelper.CopyTextToClipBoard(copyContent);

                await MainWindow.Current.ShowNotificationAsync(new MainDataCopyTip(DataCopyKind.WinGetUninstall, copyResult));
            }
        }

        /// <summary>
        /// 卸载应用
        /// </summary>
        private async void OnUninstallExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is InstalledAppsModel installedApps)
            {
                // 禁用当前应用的可卸载状态
                foreach (InstalledAppsModel installedAppsItem in InstalledAppsList)
                {
                    if (installedAppsItem.AppID.Equals(installedApps.AppID))
                    {
                        installedAppsItem.IsUninstalling = true;
                        break;
                    }
                }

                //UninstallingOrRepairingLock.Enter();
                //UninstallingOrRepairingSet.Add(installedApps.AppID);
                //UninstallingOrRepairingLock.Exit();

                UninstallResult uninstallResult = await Task.Run(async () =>
                {
                    try
                    {
                        PackageManager packageManager = new();
                        return await packageManager.UninstallPackageAsync(installedApps.CatalogPackage, new()
                        {
                            // TODO：未完成
                            PackageUninstallMode = PackageUninstallMode.Interactive,
                            PackageUninstallScope = PackageUninstallScope.Any
                        });
                    }

                    // 其他异常
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "App uninstalling failed.", e);
                        return null;
                    }
                });

                if (uninstallResult is not null)
                {
                    // WinGet 应用卸载成功
                    if (uninstallResult.Status is UninstallResultStatus.Ok)
                    {
                        InstalledAppsCollection.Remove(installedApps);
                        InstalledAppsList.Remove(installedApps);

                        // 检测是否需要重启设备完成应用的卸载，如果是，询问用户是否需要重启设备
                        if (uninstallResult.RebootRequired)
                        {
                            ContentDialogResult contentDialogResult = await MainWindow.Current.ShowDialogAsync(new RebootDialog(WinGetOptionKind.Uninstall, installedApps.AppName));

                            if (contentDialogResult is ContentDialogResult.Primary)
                            {
                                await Task.Run(() =>
                                {
                                    ShutdownHelper.Restart(ResourceService.GetLocalized("WinGet/RestartPC"), TimeSpan.FromSeconds(120));
                                });
                            }
                        }
                    }
                    else
                    {
                        // 应用卸载失败，将当前任务状态修改为可卸载状态
                        foreach (InstalledAppsModel installedAppsItem in InstalledAppsList)
                        {
                            if (installedAppsItem.AppID.Equals(installedApps.AppID))
                            {
                                installedAppsItem.IsUninstalling = false;
                                break;
                            }
                        }

                        //UninstallingOrRepairingLock.Enter();
                        //UninstallingOrRepairingSet.Remove(installedApps.AppID);
                        //UninstallingOrRepairingLock.Exit();

                        await Task.Run(() =>
                        {
                            // 显示 WinGet 卸载应用失败通知
                            AppNotificationBuilder appNotificationBuilder = new();
                            appNotificationBuilder.AddArgument("action", "OpenApp");
                            appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/WinGetUninstallFailed1"), installedApps.AppName));
                            appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/WinGetUninstallFailed2"));
                            appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/WinGetUninstallFailed3"));
                            AppNotificationButton openSettingsButton = new(ResourceService.GetLocalized("Notification/OpenSettings"));
                            openSettingsButton.Arguments.Add("action", "OpenSettings");
                            appNotificationBuilder.AddButton(openSettingsButton);
                            ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                        });
                    }
                }
                else
                {
                    // 应用卸载失败，将当前任务状态修改为可卸载状态
                    foreach (InstalledAppsModel installedAppsItem in InstalledAppsList)
                    {
                        if (installedAppsItem.AppID.Equals(installedApps.AppID))
                        {
                            installedAppsItem.IsUninstalling = false;
                            break;
                        }
                    }

                    //UninstallingOrRepairingLock.Enter();
                    //UninstallingOrRepairingSet.Remove(installedApps.AppID);
                    //UninstallingOrRepairingLock.Exit();

                    // 显示 WinGet 卸载应用失败通知
                    AppNotificationBuilder appNotificationBuilder = new();
                    appNotificationBuilder.AddArgument("action", "OpenApp");
                    appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/WinGetUninstallFailed1"), installedApps.AppName));
                    appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/WinGetUninstallFailed2"));
                    appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/WinGetUninstallFailed3"));
                    AppNotificationButton openSettingsButton = new(ResourceService.GetLocalized("Notification/OpenSettings"));
                    openSettingsButton.Arguments.Add("action", "OpenSettings");
                    appNotificationBuilder.AddButton(openSettingsButton);
                    ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                }
            }
        }

        /// <summary>
        /// 修复应用
        /// </summary>

        private async void OnRepairExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is InstalledAppsModel installedApps)
            {
                // 禁用当前应用的可卸载状态
                foreach (InstalledAppsModel installedAppsItem in InstalledAppsList)
                {
                    if (installedAppsItem.AppID.Equals(installedApps.AppID))
                    {
                        installedAppsItem.IsUninstalling = true;
                        break;
                    }
                }

                //UninstallingOrRepairingLock.Enter();
                //UninstallingOrRepairingSet.Add(installedApps.AppID);
                //UninstallingOrRepairingLock.Exit();

                RepairResult repairResult = await Task.Run(async () =>
                {
                    try
                    {
                        PackageManager packageManager = new();
                        return await packageManager.RepairPackageAsync(installedApps.CatalogPackage, new()
                        {
                            // TODO：未完成
                            PackageRepairMode = PackageRepairMode.Interactive,
                            PackageRepairScope = PackageRepairScope.Any
                        });
                    }

                    // 其他异常
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "App uninstalling failed.", e);
                        return null;
                    }
                });

                if (repairResult is not null)
                {
                    // WinGet 应用修复成功
                    if (repairResult.Status is RepairResultStatus.Ok)
                    {
                        installedApps.IsUninstalling = false;

                        //UninstallingOrRepairingLock.Enter();
                        //UninstallingOrRepairingSet.Remove(installedApps.AppID);
                        //UninstallingOrRepairingLock.Exit();

                        // 检测是否需要重启设备完成应用的修复，如果是，询问用户是否需要重启设备
                        if (repairResult.RebootRequired)
                        {
                            ContentDialogResult contentDialogResult = await MainWindow.Current.ShowDialogAsync(new RebootDialog(WinGetOptionKind.Repair, installedApps.AppName));

                            if (contentDialogResult is ContentDialogResult.Primary)
                            {
                                await Task.Run(() =>
                                {
                                    ShutdownHelper.Restart(ResourceService.GetLocalized("WinGet/RestartPC"), TimeSpan.FromSeconds(120));
                                });
                            }
                        }
                    }
                    else
                    {
                        // 应用修复失败，将当前任务状态修改为可卸载状态
                        foreach (InstalledAppsModel installedAppsItem in InstalledAppsList)
                        {
                            if (installedAppsItem.AppID.Equals(installedApps.AppID))
                            {
                                installedAppsItem.IsUninstalling = false;
                                break;
                            }
                        }

                        //UninstallingOrRepairingLock.Enter();
                        //UninstallingOrRepairingSet.Remove(installedApps.AppID);
                        //UninstallingOrRepairingLock.Exit();

                        await Task.Run(() =>
                        {
                            // 显示 WinGet 修复应用失败通知
                            AppNotificationBuilder appNotificationBuilder = new();
                            appNotificationBuilder.AddArgument("action", "OpenApp");
                            appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/WinGetRepairFailed1"), installedApps.AppName));
                            appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/WinGetRepairFailed2"));
                            appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/WinGetRepairFailed3"));
                            AppNotificationButton openSettingsButton = new(ResourceService.GetLocalized("Notification/OpenSettings"));
                            openSettingsButton.Arguments.Add("action", "OpenSettings");
                            appNotificationBuilder.AddButton(openSettingsButton);
                            ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                        });
                    }
                }
                else
                {
                    // 应用修复失败，将当前任务状态修改为可卸载状态
                    foreach (InstalledAppsModel installedAppsItem in InstalledAppsList)
                    {
                        if (installedAppsItem.AppID.Equals(installedApps.AppID))
                        {
                            installedAppsItem.IsUninstalling = false;
                            break;
                        }
                    }

                    //UninstallingOrRepairingLock.Enter();
                    //UninstallingOrRepairingSet.Remove(installedApps.AppID);
                    //UninstallingOrRepairingLock.Exit();

                    // 显示 WinGet 修复应用失败通知
                    AppNotificationBuilder appNotificationBuilder = new();
                    appNotificationBuilder.AddArgument("action", "OpenApp");
                    appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/WinGetRepairFailed1"), installedApps.AppName));
                    appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/WinGetRepairFailed2"));
                    appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/WinGetRepairFailed3"));
                    AppNotificationButton openSettingsButton = new(ResourceService.GetLocalized("Notification/OpenSettings"));
                    openSettingsButton.Arguments.Add("action", "OpenSettings");
                    appNotificationBuilder.AddButton(openSettingsButton);
                    ToastNotificationService.Show(appNotificationBuilder.BuildNotification());
                }
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
                InstalledAppsResultKind = InstalledAppsResultKind.Querying;
                InstalledAppsList.Clear();
                InstalledAppsCollection.Clear();

                PackageCatalogReference packageCatalogReference = await Task.Run(() =>
                {
                    PackageManager packageManager = new();
                    return packageManager.GetLocalPackageCatalog(LocalPackageCatalog.InstalledPackages);
                });

                if (packageCatalogReference is not null)
                {
                    (ConnectResult connectResult, FindPackagesResult findPackagesResult, List<InstalledAppsModel> upgradableAppsList) = await Task.Run(() =>
                    {
                        return InstalledAppsAsync(packageCatalogReference);
                    });

                    if (connectResult.Status is ConnectResultStatus.Ok)
                    {
                        if (findPackagesResult.Status is FindPackagesResultStatus.Ok)
                        {
                            if (upgradableAppsList.Count is 0)
                            {
                                InstalledAppsResultKind = InstalledAppsResultKind.Failed;
                                InstalledFailedContent = InstalledAppsEmptyDescription;
                            }
                            else
                            {
                                foreach (InstalledAppsModel installedAppsItem in upgradableAppsList)
                                {
                                    InstalledAppsList.Add(installedAppsItem);
                                }

                                foreach (InstalledAppsModel installedAppsItem in InstalledAppsList)
                                {
                                    if (string.IsNullOrEmpty(SearchText))
                                    {
                                        InstalledAppsCollection.Add(installedAppsItem);
                                    }
                                    else
                                    {
                                        if (installedAppsItem.AppName.Contains(SearchText) || installedAppsItem.AppPublisher.Contains(SearchText))
                                        {
                                            InstalledAppsCollection.Add(installedAppsItem);
                                        }
                                    }
                                }

                                InstalledAppsResultKind = string.IsNullOrEmpty(SearchText) ? InstalledAppsResultKind.Successfully : InstalledAppsResultKind.SearchResult;
                            }
                        }
                        else
                        {
                            InstalledAppsResultKind = InstalledAppsResultKind.Failed;
                            InstalledFailedContent = string.Format(InstalledAppsFailed, InstalledFindAppsFailed, findPackagesResult.ExtendedErrorCode is not null ? findPackagesResult.ExtendedErrorCode.HResult : Unknown);
                        }
                    }
                    else
                    {
                        InstalledAppsResultKind = InstalledAppsResultKind.Failed;
                        InstalledFailedContent = string.Format(InstalledAppsFailed, InstalledCatalogReferenceFailed, findPackagesResult.ExtendedErrorCode is not null ? findPackagesResult.ExtendedErrorCode.HResult : Unknown);
                    }
                }
                else
                {
                    InstalledAppsResultKind = InstalledAppsResultKind.Failed;
                    InstalledFailedContent = InstalledNotSelectSource;
                }
            }
        }

        /// <summary>
        /// 根据排序方式对列表进行排序
        /// </summary>
        private async void OnSortWayClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is string increase && (InstalledAppsResultKind is InstalledAppsResultKind.Successfully || InstalledAppsResultKind is InstalledAppsResultKind.SearchResult))
            {
                IsIncrease = Convert.ToBoolean(increase);
                InstalledAppsResultKind = InstalledAppsResultKind.Querying;
                InstalledAppsCollection.Clear();
                if (SelectedAppSortRuleKind is AppSortRuleKind.DisplayName)
                {
                    if (IsIncrease)
                    {
                        InstalledAppsList.Sort((item1, item2) => item1.AppName.CompareTo(item2.AppName));
                    }
                    else
                    {
                        InstalledAppsList.Sort((item1, item2) => item2.AppName.CompareTo(item1.AppName));
                    }
                }
                else
                {
                    if (IsIncrease)
                    {
                        InstalledAppsList.Sort((item1, item2) => item1.AppPublisher.CompareTo(item2.AppPublisher));
                    }
                    else
                    {
                        InstalledAppsList.Sort((item1, item2) => item2.AppPublisher.CompareTo(item1.AppPublisher));
                    }
                }
                await Task.Delay(500);
                foreach (InstalledAppsModel installedAppsItem in InstalledAppsList)
                {
                    if (string.IsNullOrEmpty(SearchText))
                    {
                        InstalledAppsCollection.Add(installedAppsItem);
                    }
                    else
                    {
                        if (installedAppsItem.AppName.Contains(SearchText) || installedAppsItem.AppPublisher.Contains(SearchText))
                        {
                            InstalledAppsCollection.Add(installedAppsItem);
                        }
                    }
                }
                InstalledAppsResultKind = string.IsNullOrEmpty(SearchText) ? InstalledAppsResultKind.Successfully : InstalledAppsResultKind.SearchResult;
            }
        }

        /// <summary>
        /// 根据排序规则对列表进行排序
        /// </summary>
        private async void OnSortRuleClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is AppSortRuleKind appSortRuleKind && (InstalledAppsResultKind is InstalledAppsResultKind.Successfully || InstalledAppsResultKind is InstalledAppsResultKind.SearchResult))
            {
                SelectedAppSortRuleKind = appSortRuleKind;
                InstalledAppsResultKind = InstalledAppsResultKind.Querying;
                InstalledAppsCollection.Clear();
                if (SelectedAppSortRuleKind is AppSortRuleKind.DisplayName)
                {
                    if (IsIncrease)
                    {
                        InstalledAppsList.Sort((item1, item2) => item1.AppName.CompareTo(item2.AppName));
                    }
                    else
                    {
                        InstalledAppsList.Sort((item1, item2) => item2.AppName.CompareTo(item1.AppName));
                    }
                }
                else
                {
                    if (IsIncrease)
                    {
                        InstalledAppsList.Sort((item1, item2) => item1.AppPublisher.CompareTo(item2.AppPublisher));
                    }
                    else
                    {
                        InstalledAppsList.Sort((item1, item2) => item2.AppPublisher.CompareTo(item1.AppPublisher));
                    }
                }
                await Task.Delay(500);
                foreach (InstalledAppsModel installedAppsItem in InstalledAppsList)
                {
                    if (string.IsNullOrEmpty(SearchText))
                    {
                        InstalledAppsCollection.Add(installedAppsItem);
                    }
                    else
                    {
                        if (installedAppsItem.AppName.Contains(SearchText) || installedAppsItem.AppPublisher.Contains(SearchText))
                        {
                            InstalledAppsCollection.Add(installedAppsItem);
                        }
                    }
                }
                InstalledAppsResultKind = string.IsNullOrEmpty(SearchText) ? InstalledAppsResultKind.Successfully : InstalledAppsResultKind.SearchResult;
            }
        }

        /// <summary>
        /// 更新已安装应用数据
        /// </summary>
        private async void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            InstalledAppsResultKind = InstalledAppsResultKind.Querying;
            InstalledAppsList.Clear();
            InstalledAppsCollection.Clear();

            PackageCatalogReference packageCatalogReference = await Task.Run(() =>
            {
                PackageManager packageManager = new();
                return packageManager.GetLocalPackageCatalog(LocalPackageCatalog.InstalledPackages);
            });

            if (packageCatalogReference is not null)
            {
                (ConnectResult connectResult, FindPackagesResult findPackagesResult, List<InstalledAppsModel> upgradableAppsList) = await Task.Run(() =>
                {
                    return InstalledAppsAsync(packageCatalogReference);
                });

                if (connectResult.Status is ConnectResultStatus.Ok)
                {
                    if (findPackagesResult.Status is FindPackagesResultStatus.Ok)
                    {
                        if (upgradableAppsList.Count is 0)
                        {
                            InstalledAppsResultKind = InstalledAppsResultKind.Failed;
                            InstalledFailedContent = InstalledAppsEmptyDescription;
                        }
                        else
                        {
                            foreach (InstalledAppsModel installedAppsItem in upgradableAppsList)
                            {
                                InstalledAppsList.Add(installedAppsItem);
                            }

                            foreach (InstalledAppsModel installedAppsItem in InstalledAppsList)
                            {
                                if (string.IsNullOrEmpty(SearchText))
                                {
                                    InstalledAppsCollection.Add(installedAppsItem);
                                }
                                else
                                {
                                    if (installedAppsItem.AppName.Contains(SearchText) || installedAppsItem.AppPublisher.Contains(SearchText))
                                    {
                                        InstalledAppsCollection.Add(installedAppsItem);
                                    }
                                }
                            }

                            InstalledAppsResultKind = string.IsNullOrEmpty(SearchText) ? InstalledAppsResultKind.Successfully : InstalledAppsResultKind.SearchResult;
                        }
                    }
                    else
                    {
                        InstalledAppsResultKind = InstalledAppsResultKind.Failed;
                        InstalledFailedContent = string.Format(InstalledAppsFailed, InstalledFindAppsFailed, findPackagesResult.ExtendedErrorCode is not null ? findPackagesResult.ExtendedErrorCode.HResult : Unknown);
                    }
                }
                else
                {
                    InstalledAppsResultKind = InstalledAppsResultKind.Failed;
                    InstalledFailedContent = string.Format(InstalledAppsFailed, InstalledCatalogReferenceFailed, findPackagesResult.ExtendedErrorCode is not null ? findPackagesResult.ExtendedErrorCode.HResult : Unknown);
                }
            }
            else
            {
                InstalledAppsResultKind = InstalledAppsResultKind.Failed;
                InstalledFailedContent = InstalledNotSelectSource;
            }
        }

        /// <summary>
        /// 打开控制面板的程序与功能
        /// </summary>
        private void OnControlPanelClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                openControlPanel?.Open("Microsoft.ProgramsAndFeatures", null, IntPtr.Zero);
            });
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
        private async void OnQuerySubmitted(object sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrEmpty(SearchText))
            {
                InstalledAppsResultKind = InstalledAppsResultKind.Querying;
                InstalledAppsCollection.Clear();
                await Task.Delay(500);
                foreach (InstalledAppsModel installedAppsItem in InstalledAppsList)
                {
                    if (string.IsNullOrEmpty(SearchText))
                    {
                        InstalledAppsCollection.Add(installedAppsItem);
                    }
                    else
                    {
                        if (installedAppsItem.AppName.Contains(SearchText) || installedAppsItem.AppPublisher.Contains(SearchText))
                        {
                            InstalledAppsCollection.Add(installedAppsItem);
                        }
                    }
                }
                InstalledAppsResultKind = string.IsNullOrEmpty(SearchText) ? InstalledAppsResultKind.Successfully : InstalledAppsResultKind.SearchResult;
            }
        }

        /// <summary>
        /// 文本输入框内容为空时，复原原来的内容
        /// </summary>
        private void OnTextChanged(object sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (sender is AutoSuggestBox autoSuggestBox)
            {
                SearchText = autoSuggestBox.Text;
                InstalledAppsCollection.Clear();
                if (string.IsNullOrEmpty(SearchText) && InstalledAppsResultKind is InstalledAppsResultKind.SearchResult)
                {
                    foreach (InstalledAppsModel installedAppsItem in InstalledAppsList)
                    {
                        InstalledAppsCollection.Add(installedAppsItem);
                    }
                }
            }
        }

        #endregion 第二部分：已安装应用控件——挂载的事件

        public void InitializeWingetInstance(WinGetPage wingetInstance)
        {
            WinGetInstance = wingetInstance;
        }

        /// <summary>
        /// 获取已安装应用
        /// </summary>
        private async Task<(ConnectResult, FindPackagesResult, List<InstalledAppsModel>)> InstalledAppsAsync(PackageCatalogReference packageCatalogReference)
        {
            (ConnectResult connectResult, FindPackagesResult findPackagesResult, List<InstalledAppsModel> installedAppsList) installedAppsResult = ValueTuple.Create<ConnectResult, FindPackagesResult, List<InstalledAppsModel>>(null, null, null);

            try
            {
                ConnectResult connectResult = await packageCatalogReference.ConnectAsync();
                installedAppsResult.connectResult = connectResult;

                if (connectResult is not null && connectResult.Status is ConnectResultStatus.Ok)
                {
                    FindPackagesOptions findPackagesOptions = new();
                    FindPackagesResult findPackagesResult = await connectResult.PackageCatalog.FindPackagesAsync(findPackagesOptions);
                    installedAppsResult.findPackagesResult = findPackagesResult;

                    if (findPackagesResult is not null && findPackagesResult.Status is FindPackagesResultStatus.Ok)
                    {
                        List<InstalledAppsModel> installedAppsList = [];

                        for (int index = 0; index < findPackagesResult.Matches.Count; index++)
                        {
                            MatchResult matchItem = findPackagesResult.Matches[index];

                            if (matchItem.CatalogPackage is not null && !string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.Publisher))
                            {
                                bool isUninstalling = false;
                                //UninstallingOrRepairingLock.Enter();
                                //if (UninstallingOrRepairingSet.Contains(matchItem.CatalogPackage.InstalledVersion.Id))
                                //{
                                //    isUninstalling = true;
                                //}
                                //UninstallingOrRepairingLock.Exit();

                                installedAppsList.Add(new InstalledAppsModel()
                                {
                                    AppID = matchItem.CatalogPackage.InstalledVersion.Id,
                                    AppName = string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.DisplayName) ? Unknown : matchItem.CatalogPackage.InstalledVersion.DisplayName,
                                    AppPublisher = string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.Publisher) ? Unknown : matchItem.CatalogPackage.InstalledVersion.Publisher,
                                    AppVersion = string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.Version) ? Unknown : matchItem.CatalogPackage.InstalledVersion.Version,
                                    IsUninstalling = isUninstalling,
                                    CatalogPackage = matchItem.CatalogPackage,
                                });
                            }
                        }

                        if (SelectedAppSortRuleKind is AppSortRuleKind.DisplayName)
                        {
                            if (IsIncrease)
                            {
                                installedAppsList.Sort((item1, item2) => item1.AppName.CompareTo(item2.AppName));
                            }
                            else
                            {
                                installedAppsList.Sort((item1, item2) => item2.AppName.CompareTo(item1.AppName));
                            }
                        }
                        else
                        {
                            if (IsIncrease)
                            {
                                installedAppsList.Sort((item1, item2) => item1.AppPublisher.CompareTo(item2.AppPublisher));
                            }
                            else
                            {
                                installedAppsList.Sort((item1, item2) => item2.AppPublisher.CompareTo(item1.AppPublisher));
                            }
                        }

                        installedAppsResult.installedAppsList = installedAppsList;
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Uninstall winget app failed", e);
            }

            return installedAppsResult;
        }

        /// <summary>
        /// 获取搜索应用是否成功
        /// </summary>
        public Visibility GetInstalledAppsSuccessfullyState(InstalledAppsResultKind installedAppsResultKind, int count, bool isSuccessfully)
        {
            if (isSuccessfully)
            {
                if (InstalledAppsResultKind.Equals(InstalledAppsResultKind.Successfully))
                {
                    return Visibility.Visible;
                }
                else if (InstalledAppsResultKind.Equals(InstalledAppsResultKind.SearchResult))
                {
                    return count > 0 ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
            else
            {
                if (InstalledAppsResultKind.Equals(InstalledAppsResultKind.Successfully))
                {
                    return Visibility.Collapsed;
                }
                else if (InstalledAppsResultKind.Equals(InstalledAppsResultKind.SearchResult))
                {
                    return count > 0 ? Visibility.Collapsed : Visibility.Visible;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// 检查搜索应用是否成功
        /// </summary>
        public Visibility CheckInstalledAppsState(InstalledAppsResultKind installedAppsResultKind, InstalledAppsResultKind comparedInstalledAppsResultKind)
        {
            return installedAppsResultKind.Equals(comparedInstalledAppsResultKind) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取是否正在搜索中
        /// </summary>

        public bool GetIsInstalling(InstalledAppsResultKind installedAppsResultKind)
        {
            return !installedAppsResultKind.Equals(InstalledAppsResultKind.Querying);
        }
    }
}
