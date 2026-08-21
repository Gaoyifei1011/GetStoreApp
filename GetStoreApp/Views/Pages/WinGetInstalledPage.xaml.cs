using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Helpers.WinGet;
using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.NotificationTips;
using GetStoreApp.Views.Windows;
using GetStoreApp.WindowsAPI.ComTypes;
using GetStoreApp.WindowsAPI.PInvoke.Ole32;
using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using Microsoft.Management.Deployment;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;
using WinRT;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// WinGet 已安装应用界面
    /// </summary>
    internal sealed partial class WinGetInstalledPage : Page, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string InstalledAppsCountInfoString = ResourceService.GetLocalized("WinGetInstalled/InstalledAppsCountInfo");
        private readonly string InstalledAppsEmptyDescriptionString = ResourceService.GetLocalized("WinGetInstalled/InstalledAppsEmptyDescription");
        private readonly string InstalledAppsFailedString = ResourceService.GetLocalized("WinGetInstalled/InstalledAppsFailed");
        private readonly string InstalledCatalogReferenceFailedString = ResourceService.GetLocalized("WinGetInstalled/InstalledCatalogReferenceFailed");
        private readonly string InstalledFindAppsFailedString = ResourceService.GetLocalized("WinGetInstalled/InstalledFindAppsFailed");
        private readonly string InstalledNotSelectSourceString = ResourceService.GetLocalized("WinGetInstalled/InstalledNotSelectSource");
        private readonly string NotAvailableString = ResourceService.GetLocalized("WinGetInstalled/NotAvailable");
        private readonly Guid CLSID_OpenControlPanel = new("06622D85-6856-4460-8DE1-A81921B41C4B");
        private readonly Lock InstalledAppsLock = new();
        private IOpenControlPanel openControlPanel;
        private WinGetPage WinGetPageInstance;

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、集合与事件

        private string _searchText;

        private string SearchText
        {
            get { return _searchText; }

            set
            {
                if (!string.Equals(_searchText, value))
                {
                    _searchText = value;
                    PropertyChanged?.Invoke(this, new(nameof(SearchText)));
                }
            }
        }

        private bool _isIncrease;

        private bool IsIncrease
        {
            get { return _isIncrease; }

            set
            {
                if (!Equals(_isIncrease, value))
                {
                    _isIncrease = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsIncrease)));
                }
            }
        }

        private AppSortRuleKind _selectedAppSortRuleKind = AppSortRuleKind.DisplayName;

        private AppSortRuleKind SelectedAppSortRuleKind
        {
            get { return _selectedAppSortRuleKind; }

            set
            {
                if (!Equals(_selectedAppSortRuleKind, value))
                {
                    _selectedAppSortRuleKind = value;
                    PropertyChanged?.Invoke(this, new(nameof(SelectedAppSortRuleKind)));
                }
            }
        }

        private bool _forceUninstall;

        private bool ForceUninstall
        {
            get { return _forceUninstall; }

            set
            {
                if (!Equals(_forceUninstall, value))
                {
                    _forceUninstall = value;
                    PropertyChanged?.Invoke(this, new(nameof(ForceUninstall)));
                }
            }
        }

        private int _selectedPackageUninstallScopeIndex;

        private int SelectedPackageUninstallScopeIndex
        {
            get { return _selectedPackageUninstallScopeIndex; }

            set
            {
                if (!Equals(_selectedPackageUninstallScopeIndex, value))
                {
                    _selectedPackageUninstallScopeIndex = value;
                    PropertyChanged?.Invoke(this, new(nameof(SelectedPackageUninstallScopeIndex)));
                }
            }
        }

        private int _selectedPackageUninstallModeIndex;

        private int SelectedPackageUninstallModeIndex
        {
            get { return _selectedPackageUninstallModeIndex; }

            set
            {
                if (!Equals(_selectedPackageUninstallModeIndex, value))
                {
                    _selectedPackageUninstallModeIndex = value;
                    PropertyChanged?.Invoke(this, new(nameof(SelectedPackageUninstallModeIndex)));
                }
            }
        }

        private InstalledAppsResultKind _installedAppsResultKind;

        private InstalledAppsResultKind InstalledAppsResultKind
        {
            get { return _installedAppsResultKind; }

            set
            {
                if (!Equals(_installedAppsResultKind, value))
                {
                    _installedAppsResultKind = value;
                    PropertyChanged?.Invoke(this, new(nameof(InstalledAppsResultKind)));
                }
            }
        }

        private string _installedFailedContent;

        private string InstalledFailedContent
        {
            get { return _installedFailedContent; }

            set
            {
                if (!string.Equals(_installedFailedContent, value))
                {
                    _installedFailedContent = value;
                    PropertyChanged?.Invoke(this, new(nameof(InstalledFailedContent)));
                }
            }
        }

        private List<InstalledAppsModel> InstalledAppsList { get; } = [];

        private ObservableCollection<InstalledAppsModel> InstalledAppsCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、集合与事件

        #region 第三部分：构造函数

        internal WinGetInstalledPage()
        {
            InitializeComponent();
            CreateOpenControlPanel();
        }

        #endregion 第三部分：构造函数

        #region 第四部分：父类虚方法重写

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (args.Parameter is WinGetPage winGetPage && WinGetPageInstance is null)
            {
                WinGetPageInstance = winGetPage;
                IsIncrease = true;
                MountWinGetEvent();
                await InitializeInstalledAppsDataAsync(SelectedAppSortRuleKind, IsIncrease);
            }
        }

        #endregion 第四部分：父类虚方法重写

        #region 第五部分：命令调用处理

        /// <summary>
        /// 复制卸载命令信息
        /// </summary>
        private async void OnCopyUninstallTextExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string appId && !string.IsNullOrEmpty(appId))
            {
                string uninstallArguments = await GenerateUninstallArgumentsAsync(true, appId, ForceUninstall, SelectedPackageUninstallModeIndex, SelectedPackageUninstallScopeIndex);
                if (!string.IsNullOrEmpty(uninstallArguments))
                {
                    bool copyResult = CopyPasteHelper.CopyTextToClipBoard(uninstallArguments);
                    await MainWindow.Current.ShowNotificationAsync(new CopyPasteMainNotificationTip(copyResult));
                }
            }
        }

        /// <summary>
        /// 使用命令卸载当前应用
        /// </summary>
        private async void OnUninstallWithCmdExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is string appId && !string.IsNullOrEmpty(appId))
            {
                string uninstallArguments = await GenerateUninstallArgumentsAsync(false, appId, ForceUninstall, SelectedPackageUninstallModeIndex, SelectedPackageUninstallScopeIndex);
                if (!string.IsNullOrEmpty(uninstallArguments))
                {
                    RunWinGetCommand(uninstallArguments);
                }
            }
        }

        /// <summary>
        /// 卸载应用
        /// </summary>
        private async void OnUninstallExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is InstalledAppsModel installedApps && WinGetPageInstance is not null)
            {
                // 禁用当前应用的可卸载状态
                InstalledAppsLock.Enter();
                try
                {
                    installedApps.IsUninstalling = true;
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
                finally
                {
                    InstalledAppsLock.Exit();
                }

                UninstallOptions uninstallOptions = await CreateUninstallOptionsAsync(ForceUninstall, SelectedPackageUninstallModeIndex, SelectedPackageUninstallScopeIndex);

                await WinGetPageInstance.AddTaskAsync(new()
                {
                    PackageOperationKind = PackageOperationKind.Uninstall,
                    AppID = installedApps.AppID,
                    AppName = installedApps.AppName,
                    AppVersion = installedApps.CatalogPackage.InstalledVersion.Version,
                    PackageOperationProgress = 0,
                    PackageUninstallProgressState = PackageUninstallProgressState.Queued,
                    PackageVersionId = null,
                    DownloadedFileSize = VolumeSizeHelper.ConvertVolumeSizeToString(0),
                    TotalFileSize = VolumeSizeHelper.ConvertVolumeSizeToString(0),
                    PackageUninstallProgress = null,
                    InstalledApps = installedApps,
                    UninstallOptions = uninstallOptions
                });
            }
        }

        #endregion 第五部分：命令调用处理

        #region 第六部分：挂载事件处理

        /// <summary>
        /// 根据排序方式对列表进行排序
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(RadioMenuFlyoutItem))]
        private void OnSortWayClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is bool increase && (InstalledAppsResultKind is InstalledAppsResultKind.Successfully || InstalledAppsResultKind is InstalledAppsResultKind.SearchResult))
            {
                IsIncrease = increase;
                if (InstalledAppsResultKind is InstalledAppsResultKind.Successfully || InstalledAppsResultKind is InstalledAppsResultKind.SearchResult)
                {
                    InitializeMatchedInstalledApps(SelectedAppSortRuleKind, IsIncrease, SearchText);
                }
            }
        }

        /// <summary>
        /// 根据排序规则对列表进行排序
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(RadioMenuFlyoutItem))]
        private void OnSortRuleClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is AppSortRuleKind appSortRuleKind && (InstalledAppsResultKind is InstalledAppsResultKind.Successfully || InstalledAppsResultKind is InstalledAppsResultKind.SearchResult))
            {
                SelectedAppSortRuleKind = appSortRuleKind;
                if (InstalledAppsResultKind is InstalledAppsResultKind.Successfully || InstalledAppsResultKind is InstalledAppsResultKind.SearchResult)
                {
                    InitializeMatchedInstalledApps(SelectedAppSortRuleKind, IsIncrease, SearchText);
                }
            }
        }

        /// <summary>
        /// 是否强制卸载
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ToggleSwitch))]
        private void OnForceUninstallToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch && !Equals(ForceUninstall, toggleSwitch.IsOn))
            {
                ForceUninstall = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 应用卸载范围发生更改时触发的事件
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(RadioButtons))]
        private void OnPackageUninstallScopeSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is RadioButtons radioButtons && !Equals(SelectedPackageUninstallScopeIndex, radioButtons.SelectedIndex))
            {
                SelectedPackageUninstallScopeIndex = radioButtons.SelectedIndex;
            }
        }

        /// <summary>
        /// 应用卸载模式发生更改时触发的事件
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(RadioButtons))]
        private void OnPackageUninstallModeSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (sender is RadioButtons radioButtons && !Equals(SelectedPackageUninstallModeIndex, radioButtons.SelectedIndex))
            {
                SelectedPackageUninstallModeIndex = radioButtons.SelectedIndex;
            }
        }

        /// <summary>
        /// 更新已安装应用数据
        /// </summary>
        private async void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            await InitializeInstalledAppsDataAsync(SelectedAppSortRuleKind, IsIncrease);
        }

        /// <summary>
        /// 打开任务管理
        /// </summary>

        private void OnTaskManagerClicked(object sender, RoutedEventArgs args)
        {
            WinGetPageInstance?.ShowTaskManager();
        }

        /// <summary>
        /// 打开控制面板的程序与功能
        /// </summary>
        private void OnControlPanelClicked(object sender, RoutedEventArgs args)
        {
            OpenControlPanel();
        }

        /// <summary>
        /// 了解 WinGet 程序包具体的使用说明
        /// </summary>
        private void OnUseInstructionClicked(object sender, RoutedEventArgs args)
        {
            WinGetPageInstance?.ShowUseInstruction();
        }

        /// <summary>
        /// 根据输入的内容检索应用
        /// </summary>
        private void OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (!string.IsNullOrEmpty(SearchText) && InstalledAppsResultKind is InstalledAppsResultKind.Successfully)
            {
                InitializeMatchedInstalledApps(SelectedAppSortRuleKind, IsIncrease, SearchText);
            }
        }

        /// <summary>
        /// 文本输入框内容为空时，复原原来的内容
        /// </summary>
        private void OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            SearchText = sender.Text;
            if (string.IsNullOrEmpty(SearchText) && InstalledAppsResultKind is InstalledAppsResultKind.SearchResult)
            {
                InitializeMatchedInstalledApps(SelectedAppSortRuleKind, IsIncrease, SearchText);
            }
        }

        /// <summary>
        /// 应用程序退出时触发的事件
        /// </summary>
        private void OnApplicationExit()
        {
            try
            {
                DismountWinGetEvent();
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetInstalledPage), nameof(OnApplicationExit), 1, e);
            }
        }

        /// <summary>
        /// 可卸载项目卸载完成后发生的事件
        /// </summary>
        private void OnInstalledAppsPackageOperationEvent(bool result, bool isCanceled, InstalledAppsModel installedApps, UninstallResult uninstallResult)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                InstalledAppsLock.Enter();

                try
                {
                    if (result && uninstallResult.Status is UninstallResultStatus.Ok)
                    {
                        foreach (InstalledAppsModel installedAppsItem in InstalledAppsList)
                        {
                            if (string.Equals(installedApps.AppID, installedAppsItem.AppID) && string.Equals(installedApps.AppVersion, installedAppsItem.AppVersion))
                            {
                                InstalledAppsList.Remove(installedAppsItem);
                                InstalledAppsCollection.Remove(installedAppsItem);
                                break;
                            }
                        }

                        if (InstalledAppsList.Count is 0)
                        {
                            InstalledAppsResultKind = InstalledAppsResultKind.Failed;
                            InstalledFailedContent = InstalledAppsEmptyDescriptionString;
                        }
                        else
                        {
                            InstalledAppsResultKind = string.IsNullOrEmpty(SearchText) ? InstalledAppsResultKind.Successfully : InstalledAppsResultKind.SearchResult;
                        }
                    }
                    else
                    {
                        foreach (InstalledAppsModel installedAppsItem in InstalledAppsList)
                        {
                            if (string.Equals(installedApps.AppID, installedAppsItem.AppID) && string.Equals(installedApps.AppVersion, installedAppsItem.AppVersion))
                            {
                                installedAppsItem.IsUninstalling = false;
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
                    InstalledAppsLock.Exit();
                }
            });
        }

        #endregion 第六部分：挂载事件处理

        #region 第七部分：数据操作与业务逻辑

        /// <summary>
        /// 创建控制面板接口对象
        /// </summary>
        private void CreateOpenControlPanel()
        {
            Task.Run(() =>
            {
                int createResult = Ole32Library.CoCreateInstance(CLSID_OpenControlPanel, nint.Zero, CLSCTX.CLSCTX_INPROC_SERVER | CLSCTX.CLSCTX_INPROC_HANDLER | CLSCTX.CLSCTX_LOCAL_SERVER | CLSCTX.CLSCTX_REMOTE_SERVER, typeof(IOpenControlPanel).GUID, out nint ppv);

                if (createResult is 0)
                {
                    openControlPanel = (IOpenControlPanel)Program.StrategyBasedComWrappers.GetOrCreateObjectForComInstance(ppv, CreateObjectFlags.None);
                }
            });
        }

        /// <summary>
        /// 打开控制面板
        /// </summary>
        private void OpenControlPanel()
        {
            Task.Run(() =>
            {
                openControlPanel?.Open("Microsoft.ProgramsAndFeatures", null, nint.Zero);
            });
        }

        /// <summary>
        /// 创建卸载选项
        /// </summary>
        private async Task<UninstallOptions> CreateUninstallOptionsAsync(bool forceUninstall, int packageUninstallModeIndex, int packageUninstallScopeIndex)
        {
            return await Task.Run(() =>
            {
                UninstallOptions uninstallOptions = WinGetFactoryHelper.CreateUninstallOptions();
                uninstallOptions.Force = forceUninstall;
                uninstallOptions.PackageUninstallMode = Enum.IsDefined(typeof(PackageUninstallMode), packageUninstallModeIndex) ? (PackageUninstallMode)packageUninstallModeIndex : PackageUninstallMode.Default;
                uninstallOptions.LogOutputPath = LogService.WinGetFolderPath;
                uninstallOptions.PackageUninstallScope = Enum.IsDefined(typeof(PackageUninstallScope), packageUninstallScopeIndex) ? (PackageUninstallScope)packageUninstallScopeIndex : PackageUninstallScope.Any;
                return uninstallOptions;
            });
        }

        /// <summary>
        /// 生成卸载参数
        /// </summary>
        private async Task<string> GenerateUninstallArgumentsAsync(bool hasProcessName, string appId, bool forceInstall, int packageUninstallModeIndex, int packageUninstallScopeIndex)
        {
            return await Task.Run(() =>
            {
                List<string> argsList = [];
                if (hasProcessName)
                {
                    argsList.Add("winget.exe");
                }
                argsList.AddRange((string[])["uninstall", "--id", string.Format(@"""{0}""", appId)]);

                if (forceInstall)
                {
                    argsList.Add("--force");
                }

                if (Enum.IsDefined(typeof(PackageUninstallMode), packageUninstallModeIndex) && (PackageUninstallMode)packageUninstallModeIndex is PackageUninstallMode.Interactive)
                {
                    argsList.Add("--interactive");
                }
                else if (Enum.IsDefined(typeof(PackageUninstallMode), packageUninstallModeIndex) && (PackageUninstallMode)packageUninstallModeIndex is PackageUninstallMode.Silent)
                {
                    argsList.Add("--silent");
                }

                if (Enum.IsDefined(typeof(PackageUninstallScope), packageUninstallScopeIndex) && (PackageUninstallScope)packageUninstallScopeIndex is PackageUninstallScope.User)
                {
                    argsList.Add("--scope");
                    argsList.Add("user");
                }
                else if (Enum.IsDefined(typeof(PackageUninstallScope), packageUninstallScopeIndex) && (PackageUninstallScope)packageUninstallScopeIndex is PackageUninstallScope.System)
                {
                    argsList.Add("--scope");
                    argsList.Add("machine");
                }

                return string.Join(' ', argsList);
            });
        }

        /// <summary>
        /// 运行 WinGet 命令
        /// </summary>
        private void RunWinGetCommand(string arguments)
        {
            if (!string.IsNullOrEmpty(arguments))
            {
                Task.Run(() =>
                {
                    Shell32Library.ShellExecute(nint.Zero, "open", "winget.exe", arguments, null, WindowShowStyle.SW_SHOWNORMAL);
                });
            }
        }

        /// <summary>
        /// 挂载 WinGet 相关事件
        /// </summary>
        private void MountWinGetEvent()
        {
            GlobalNotificationService.ApplicationExit += OnApplicationExit;
            WinGetPageInstance.InstalledAppsPackageOperationEvent += OnInstalledAppsPackageOperationEvent;
        }

        /// <summary>
        /// 卸载 WinGet 相关事件
        /// </summary>
        private void DismountWinGetEvent()
        {
            GlobalNotificationService.ApplicationExit -= OnApplicationExit;
            WinGetPageInstance.InstalledAppsPackageOperationEvent -= OnInstalledAppsPackageOperationEvent;
        }

        /// <summary>
        /// 初始化已安装应用数据
        /// </summary>
        private async Task InitializeInstalledAppsDataAsync(AppSortRuleKind appSortRuleKind, bool isIncrease)
        {
            InstalledAppsResultKind = InstalledAppsResultKind.Querying;

            InstalledAppsLock.Enter();
            try
            {
                InstalledAppsList.Clear();
                InstalledAppsCollection.Clear();
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
            }
            finally
            {
                InstalledAppsLock.Exit();
            }

            PackageCatalogReference packageCatalogReference = await Task.Run(() =>
            {
                PackageManager packageManager = WinGetFactoryHelper.CreatePackageManager();
                return packageManager.GetLocalPackageCatalog(LocalPackageCatalog.InstalledPackages);
            });

            if (packageCatalogReference is not null)
            {
                (ConnectResult connectResult, FindPackagesResult findPackagesResult, List<InstalledAppsModel> upgradableAppsList) = await Task.Run(() =>
                {
                    return InstalledAppsAsync(packageCatalogReference, appSortRuleKind, isIncrease);
                });

                if (connectResult is not null && findPackagesResult is not null)
                {
                    if (connectResult.Status is ConnectResultStatus.Ok)
                    {
                        if (findPackagesResult.Status is FindPackagesResultStatus.Ok)
                        {
                            if (upgradableAppsList is null || upgradableAppsList.Count is 0)
                            {
                                InstalledAppsResultKind = InstalledAppsResultKind.Failed;
                                InstalledFailedContent = InstalledAppsEmptyDescriptionString;
                            }
                            else
                            {
                                InstalledAppsLock.Enter();
                                try
                                {
                                    InstalledAppsList.AddRange(upgradableAppsList);

                                    if (string.IsNullOrEmpty(SearchText))
                                    {
                                        foreach (InstalledAppsModel installedAppsItem in InstalledAppsList)
                                        {
                                            InstalledAppsCollection.Add(installedAppsItem);
                                        }
                                    }
                                    else
                                    {
                                        foreach (InstalledAppsModel installedAppsItem in InstalledAppsList)
                                        {
                                            if (installedAppsItem.AppName.Contains(SearchText) || installedAppsItem.AppPublisher.Contains(SearchText))
                                            {
                                                InstalledAppsCollection.Add(installedAppsItem);
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
                                    InstalledAppsLock.Exit();
                                }

                                InstalledAppsResultKind = string.IsNullOrEmpty(SearchText) ? InstalledAppsResultKind.Successfully : InstalledAppsResultKind.SearchResult;
                            }
                        }
                        else
                        {
                            InstalledAppsResultKind = InstalledAppsResultKind.Failed;
                            InstalledFailedContent = string.Format(InstalledAppsFailedString, InstalledFindAppsFailedString, findPackagesResult.ExtendedErrorCode is not null ? string.Format("0x{0:X8}", findPackagesResult.ExtendedErrorCode.HResult) : NotAvailableString);
                        }
                    }
                    else
                    {
                        InstalledAppsResultKind = InstalledAppsResultKind.Failed;
                        InstalledFailedContent = string.Format(InstalledAppsFailedString, InstalledCatalogReferenceFailedString, findPackagesResult.ExtendedErrorCode is not null ? string.Format("0x{0:X8}", findPackagesResult.ExtendedErrorCode.HResult) : NotAvailableString);
                    }
                }
            }
            else
            {
                InstalledAppsResultKind = InstalledAppsResultKind.Failed;
                InstalledFailedContent = InstalledNotSelectSourceString;
            }
        }

        /// <summary>
        /// 初始化符合的已安装应用结果
        /// </summary>
        private void InitializeMatchedInstalledApps(AppSortRuleKind appSortRuleKind, bool isIncrease, string searchText)
        {
            InstalledAppsResultKind = InstalledAppsResultKind.Querying;
            InstalledAppsLock.Enter();
            try
            {
                InstalledAppsCollection.Clear();
                if (appSortRuleKind is AppSortRuleKind.DisplayName)
                {
                    if (isIncrease)
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
                    if (isIncrease)
                    {
                        InstalledAppsList.Sort((item1, item2) => item1.AppPublisher.CompareTo(item2.AppPublisher));
                    }
                    else
                    {
                        InstalledAppsList.Sort((item1, item2) => item2.AppPublisher.CompareTo(item1.AppPublisher));
                    }
                }

                if (string.IsNullOrEmpty(searchText))
                {
                    foreach (InstalledAppsModel installedAppsItem in InstalledAppsList)
                    {
                        InstalledAppsCollection.Add(installedAppsItem);
                    }
                }
                else
                {
                    foreach (InstalledAppsModel installedAppsItem in InstalledAppsList)
                    {
                        if (installedAppsItem.AppName.Contains(searchText) || installedAppsItem.AppPublisher.Contains(searchText))
                        {
                            InstalledAppsCollection.Add(installedAppsItem);
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
                InstalledAppsLock.Exit();
            }
            InstalledAppsResultKind = string.IsNullOrEmpty(searchText) ? InstalledAppsResultKind.Successfully : InstalledAppsResultKind.SearchResult;
        }

        /// <summary>
        /// 获取已安装应用
        /// </summary>
        private async Task<(ConnectResult, FindPackagesResult, List<InstalledAppsModel>)> InstalledAppsAsync(PackageCatalogReference packageCatalogReference, AppSortRuleKind appSortRuleKind, bool isIncrease)
        {
            (ConnectResult connectResult, FindPackagesResult findPackagesResult, List<InstalledAppsModel> installedAppsList) installedAppsResult = ValueTuple.Create<ConnectResult, FindPackagesResult, List<InstalledAppsModel>>(null, null, null);

            if (packageCatalogReference is not null)
            {
                try
                {
                    ConnectResult connectResult = await packageCatalogReference.ConnectAsync();
                    installedAppsResult.connectResult = connectResult;

                    if (connectResult is not null && connectResult.Status is ConnectResultStatus.Ok)
                    {
                        FindPackagesOptions findPackagesOptions = WinGetFactoryHelper.CreateFindPackagesOptions();
                        FindPackagesResult findPackagesResult = await connectResult.PackageCatalog.FindPackagesAsync(findPackagesOptions);
                        installedAppsResult.findPackagesResult = findPackagesResult;

                        if (findPackagesResult is not null && findPackagesResult.Status is FindPackagesResultStatus.Ok)
                        {
                            List<InstalledAppsModel> installedAppsList = [];

                            for (int index = 0; index < findPackagesResult.Matches.Count; index++)
                            {
                                MatchResult matchItem = findPackagesResult.Matches[index];

                                if (matchItem.CatalogPackage is CatalogPackage catalogPackage && !catalogPackage.InstalledVersion.Id.StartsWith("MSIX", StringComparison.OrdinalIgnoreCase))
                                {
                                    bool isUninstalling = false;
                                    WinGetPageInstance.PackageOperationLock.Enter();
                                    try
                                    {
                                        foreach (PackageOperationModel packageOperationItem in WinGetPageInstance.PackageOperationCollection)
                                        {
                                            if (string.Equals(catalogPackage.InstalledVersion.Id, packageOperationItem.AppID) && string.Equals(catalogPackage.InstalledVersion.Version, packageOperationItem.AppVersion))
                                            {
                                                isUninstalling = true;
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

                                    installedAppsList.Add(new()
                                    {
                                        AppID = matchItem.CatalogPackage.InstalledVersion.Id,
                                        AppName = string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.DisplayName) ? NotAvailableString : matchItem.CatalogPackage.InstalledVersion.DisplayName,
                                        AppPublisher = string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.Publisher) ? NotAvailableString : matchItem.CatalogPackage.InstalledVersion.Publisher,
                                        AppVersion = string.IsNullOrEmpty(matchItem.CatalogPackage.InstalledVersion.Version) ? NotAvailableString : matchItem.CatalogPackage.InstalledVersion.Version,
                                        IsUninstalling = isUninstalling,
                                        CatalogPackage = matchItem.CatalogPackage,
                                    });
                                }
                            }

                            if (appSortRuleKind is AppSortRuleKind.DisplayName)
                            {
                                if (isIncrease)
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
                                if (isIncrease)
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
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(WinGetInstalledPage), nameof(InstalledAppsAsync), 1, e);
                }
            }

            return installedAppsResult;
        }

        /// <summary>
        /// 获取搜索应用是否成功
        /// </summary>
        private Visibility GetInstalledAppsSuccessfullyVisibility(InstalledAppsResultKind installedAppsResultKind, int count, bool isSuccessfully)
        {
            if (isSuccessfully)
            {
                if (installedAppsResultKind is InstalledAppsResultKind.Successfully)
                {
                    return Visibility.Visible;
                }
                else if (installedAppsResultKind is InstalledAppsResultKind.SearchResult)
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
                if (installedAppsResultKind is InstalledAppsResultKind.Successfully)
                {
                    return Visibility.Collapsed;
                }
                else if (installedAppsResultKind is InstalledAppsResultKind.SearchResult)
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
        private Visibility CheckInstalledAppsResultKindVisibility(InstalledAppsResultKind installedAppsResultKind, InstalledAppsResultKind comparedInstalledAppsResultKind)
        {
            return Equals(installedAppsResultKind, comparedInstalledAppsResultKind) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 获取是否正在搜索中
        /// </summary>

        private bool GetIsInstalling(InstalledAppsResultKind installedAppsResultKind)
        {
            return installedAppsResultKind is not InstalledAppsResultKind.Querying;
        }

        #endregion 第七部分：数据操作与业务逻辑
    }
}
