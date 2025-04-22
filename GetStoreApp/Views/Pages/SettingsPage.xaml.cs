using GetStoreApp.Extensions.DataType.Classes;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Settings;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.Settings;
using GetStoreApp.UI.TeachingTips;
using GetStoreApp.Views.Windows;
using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using GetStoreApp.WindowsAPI.PInvoke.Rstrtmgr;
using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using Microsoft.Management.Deployment;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.Storage.Pickers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;
using Windows.Globalization;
using Windows.Management.Core;
using Windows.Storage;
using Windows.System;
using Windows.UI.Notifications;
using Windows.UI.ViewManagement;

// 抑制 CA1822，CS8305，IDE0060 警告
#pragma warning disable CA1822,CS8305,IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 设置页面
    /// </summary>
    public sealed partial class SettingsPage : Page, INotifyPropertyChanged
    {
        private readonly UISettings uiSettings = new();
        private readonly string None = ResourceService.GetLocalized("Settings/None");
        private readonly string Yes = ResourceService.GetLocalized("Settings/Yes");
        private readonly string No = ResourceService.GetLocalized("Settings/No");
        private readonly string Trusted = ResourceService.GetLocalized("Settings/Trusted");
        private readonly string Distrusted = ResourceService.GetLocalized("Settings/Distrusted");
        private readonly string Predefined = ResourceService.GetLocalized("Settings/Predefined");
        private readonly string User = ResourceService.GetLocalized("Settings/User");
        private readonly string Unknown = ResourceService.GetLocalized("Settings/Unknown");
        private readonly string MicrosoftEntraId = ResourceService.GetLocalized("Settings/MicrosoftEntraId");
        private readonly string MicrosoftEntraIdForAzureBlobStorage = ResourceService.GetLocalized("Settings/MicrosoftEntraIdForAzureBlobStorage");
        private readonly string WinGetDataSourceRemoveFailed = ResourceService.GetLocalized("Settings/WinGetDataSourceRemoveFailed");
        private readonly string WinGetDataSourceRemoveGroupPolicyError = ResourceService.GetLocalized("Settings/WinGetDataSourceRemoveGroupPolicyError");
        private readonly string WinGetDataSourceRemoveCatalogError = ResourceService.GetLocalized("Settings/WinGetDataSourceRemoveCatalogError");
        private readonly string WinGetDataSourceRemoveInternalError = ResourceService.GetLocalized("Settings/WinGetDataSourceRemoveInternalError");
        private readonly string WinGetDataSourceRemoveInvalidOptions = ResourceService.GetLocalized("Settings/WinGetDataSourceRemoveInvalidOptions");
        private readonly string WinGetDataSourceRemoveAccessDenied = ResourceService.GetLocalized("Settings/WinGetDataSourceRemoveAccessDenied");
        private readonly string WinGetDataSourceRemoveSuccess = ResourceService.GetLocalized("Settings/WinGetDataSourceRemoveSuccess");
        private AppNaviagtionArgs settingNavigationArgs = AppNaviagtionArgs.None;

        private KeyValuePair<string, string> _theme = ThemeService.AppTheme;

        public KeyValuePair<string, string> Theme
        {
            get { return _theme; }

            set
            {
                if (!Equals(_theme, value))
                {
                    _theme = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Theme)));
                }
            }
        }

        private KeyValuePair<string, string> _backdrop = BackdropService.AppBackdrop;

        public KeyValuePair<string, string> Backdrop
        {
            get { return _backdrop; }

            set
            {
                if (!Equals(_backdrop, value))
                {
                    _backdrop = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Backdrop)));
                }
            }
        }

        private bool _alwaysShowBackdropValue = AlwaysShowBackdropService.AlwaysShowBackdropValue;

        public bool AlwaysShowBackdropValue
        {
            get { return _alwaysShowBackdropValue; }

            set
            {
                if (!Equals(_alwaysShowBackdropValue, value))
                {
                    _alwaysShowBackdropValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AlwaysShowBackdropValue)));
                }
            }
        }

        private bool _alwaysShowBackdropEnabled;

        public bool AlwaysShowBackdropEnabled
        {
            get { return _alwaysShowBackdropEnabled; }

            set
            {
                if (!Equals(_alwaysShowBackdropEnabled, value))
                {
                    _alwaysShowBackdropEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AlwaysShowBackdropEnabled)));
                }
            }
        }

        private bool _advancedEffectsEnabled;

        public bool AdvancedEffectsEnabled
        {
            get { return _advancedEffectsEnabled; }

            set
            {
                if (!Equals(_advancedEffectsEnabled, value))
                {
                    _advancedEffectsEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AdvancedEffectsEnabled)));
                }
            }
        }

        private KeyValuePair<string, string> _appLanguage;

        public KeyValuePair<string, string> AppLanguage
        {
            get { return _appLanguage; }

            set
            {
                if (!Equals(_appLanguage, value))
                {
                    _appLanguage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AppLanguage)));
                }
            }
        }

        private bool _topMostValue = TopMostService.TopMostValue;

        public bool TopMostValue
        {
            get { return _topMostValue; }

            set
            {
                if (!Equals(_topMostValue, value))
                {
                    _topMostValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TopMostValue)));
                }
            }
        }

        private bool _notificationValue = NotificationService.AppNotification;

        public bool NotificationValue
        {
            get { return _notificationValue; }

            set
            {
                if (!Equals(_notificationValue, value))
                {
                    _notificationValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NotificationValue)));
                }
            }
        }

        private bool _notificationEnabled = NotificationService.NotificationSetting is NotificationSetting.Enabled;

        public bool NotificationEnabled
        {
            get { return _notificationEnabled; }

            private set
            {
                if (!Equals(_notificationEnabled, value))
                {
                    _notificationEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NotificationEnabled)));
                }
            }
        }

        private bool _isWinGetConfigMode;

        public bool IsWinGetConfigMode
        {
            get { return _isWinGetConfigMode; }

            set
            {
                if (!Equals(_isWinGetConfigMode, value))
                {
                    _isWinGetConfigMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsWinGetConfigMode)));
                }
            }
        }

        private bool _isLoadedCompleted;

        public bool IsLoadedCompleted
        {
            get { return _isLoadedCompleted; }

            private set
            {
                if (!Equals(_isLoadedCompleted, value))
                {
                    _isLoadedCompleted = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoadedCompleted)));
                }
            }
        }

        private KeyValuePair<string, string> _webKernelItem = WebKernelService.WebKernel;

        public KeyValuePair<string, string> WebKernelItem
        {
            get { return _webKernelItem; }

            set
            {
                if (!Equals(_webKernelItem, value))
                {
                    _webKernelItem = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WebKernelItem)));
                }
            }
        }

        private KeyValuePair<string, string> _queryLinksModeItem = QueryLinksModeService.QueryLinksMode;

        public KeyValuePair<string, string> QueryLinksModeItem
        {
            get { return _queryLinksModeItem; }

            set
            {
                if (!Equals(_queryLinksModeItem, value))
                {
                    _queryLinksModeItem = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QueryLinksModeItem)));
                }
            }
        }

        private bool _isRestarting = false;

        public bool IsRestarting
        {
            get { return _isRestarting; }

            set
            {
                if (!Equals(_isRestarting, value))
                {
                    _isRestarting = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRestarting)));
                }
            }
        }

        private bool _shellMenuValue = ShellMenuService.ShellMenuValue;

        public bool ShellMenuValue
        {
            get { return _shellMenuValue; }

            set
            {
                if (!Equals(_shellMenuValue, value))
                {
                    _shellMenuValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShellMenuValue)));
                }
            }
        }

        private bool _useSystemRegionValue = StoreRegionService.UseSystemRegionValue;

        public bool UseSystemRegionValue
        {
            get { return _useSystemRegionValue; }

            set
            {
                if (!Equals(_useSystemRegionValue, value))
                {
                    _useSystemRegionValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UseSystemRegionValue)));
                }
            }
        }

        private GeographicRegion _currentCountryOrRegion = StoreRegionService.DefaultStoreRegion;

        public GeographicRegion CurrentCountryOrRegion
        {
            get { return _currentCountryOrRegion; }

            set
            {
                if (!Equals(_currentCountryOrRegion, value))
                {
                    _currentCountryOrRegion = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentCountryOrRegion)));
                }
            }
        }

        private GeographicRegion _storeRegion;

        public GeographicRegion StoreRegion
        {
            get { return _storeRegion; }

            set
            {
                if (!Equals(_storeRegion, value))
                {
                    _storeRegion = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StoreRegion)));
                }
            }
        }

        private bool _encryptedPackageFilterValue = LinkFilterService.EncryptedPackageFilterValue;

        public bool EncryptedPackageFilterValue
        {
            get { return _encryptedPackageFilterValue; }

            set
            {
                if (!Equals(_encryptedPackageFilterValue, value))
                {
                    _encryptedPackageFilterValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EncryptedPackageFilterValue)));
                }
            }
        }

        private bool _blockMapFilterValue = LinkFilterService.BlockMapFilterValue;

        public bool BlockMapFilterValue
        {
            get { return _blockMapFilterValue; }

            set
            {
                if (!Equals(_blockMapFilterValue, value))
                {
                    _blockMapFilterValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BlockMapFilterValue)));
                }
            }
        }

        private StorageFolder _winGetPackageFolder = WinGetConfigService.DownloadFolder;

        public StorageFolder WinGetPackageFolder
        {
            get { return _winGetPackageFolder; }

            set
            {
                if (!Equals(_winGetPackageFolder, value))
                {
                    _winGetPackageFolder = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WinGetPackageFolder)));
                }
            }
        }

        private KeyValuePair<string, string> _winGetInstallMode = WinGetConfigService.WinGetInstallMode;

        public KeyValuePair<string, string> WinGetInstallMode
        {
            get { return _winGetInstallMode; }

            set
            {
                if (!Equals(_winGetInstallMode, value))
                {
                    _winGetInstallMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WinGetInstallMode)));
                }
            }
        }

        private StorageFolder _downloadFolder = DownloadOptionsService.DownloadFolder;

        public StorageFolder DownloadFolder
        {
            get { return _downloadFolder; }

            set
            {
                if (!Equals(_downloadFolder, value))
                {
                    _downloadFolder = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DownloadFolder)));
                }
            }
        }

        private KeyValuePair<string, string> _doEngineMode = DownloadOptionsService.DoEngineMode;

        public KeyValuePair<string, string> DoEngineMode
        {
            get { return _doEngineMode; }

            set
            {
                if (!Equals(_doEngineMode, value))
                {
                    _doEngineMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DoEngineMode)));
                }
            }
        }

        private bool _allowUnsignedPackageValue = AppInstallService.AllowUnsignedPackageValue;

        public bool AllowUnsignedPackageValue
        {
            get { return _allowUnsignedPackageValue; }

            set
            {
                if (!Equals(_allowUnsignedPackageValue, value))
                {
                    _allowUnsignedPackageValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AllowUnsignedPackageValue)));
                }
            }
        }

        private bool _forceAppShutdownValue = AppInstallService.ForceAppShutdownValue;

        public bool ForceAppShutdownValue
        {
            get { return _forceAppShutdownValue; }

            set
            {
                if (!Equals(_forceAppShutdownValue, value))
                {
                    _forceAppShutdownValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ForceAppShutdownValue)));
                }
            }
        }

        private bool _forceTargetAppShutdownValue = AppInstallService.ForceTargetAppShutdownValue;

        public bool ForceTargetAppShutdownValue
        {
            get { return _forceTargetAppShutdownValue; }

            set
            {
                if (!Equals(_forceTargetAppShutdownValue, value))
                {
                    _forceTargetAppShutdownValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ForceTargetAppShutdownValue)));
                }
            }
        }

        private KeyValuePair<string, string> _installMode = InstallModeService.InstallMode;

        public KeyValuePair<string, string> InstallMode
        {
            get { return _installMode; }

            set
            {
                if (!Equals(_installMode, value))
                {
                    _installMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InstallMode)));
                }
            }
        }

        private List<KeyValuePair<string, string>> ThemeList { get; } = ThemeService.ThemeList;

        private List<KeyValuePair<string, string>> BackdropList { get; } = BackdropService.BackdropList;

        private List<KeyValuePair<string, string>> WebKernelList { get; } = WebKernelService.WebKernelList;

        private List<KeyValuePair<string, string>> QueryLinksModeList { get; } = QueryLinksModeService.QueryLinksModeList;

        private List<KeyValuePair<string, string>> WinGetInstallModeList { get; } = WinGetConfigService.WinGetInstallModeList;

        private List<KeyValuePair<string, string>> DoEngineModeList { get; } = DownloadOptionsService.DoEngineModeList;

        private List<KeyValuePair<string, string>> InstallModeList { get; } = InstallModeService.InstallModeList;

        private ObservableCollection<LanguageModel> LanguageCollection { get; } = [];

        private ObservableCollection<StoreRegionModel> StoreRegionCollection { get; } = [];

        private ObservableCollection<WinGetSourceModel> WinGetSourceInternalCollection { get; } = [];

        private ObservableCollection<WinGetSourceModel> WinGetSourceCustomCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsPage()
        {
            InitializeComponent();

            AdvancedEffectsEnabled = uiSettings.AdvancedEffectsEnabled;

            foreach (KeyValuePair<string, string> languageItem in LanguageService.LanguageList)
            {
                if (LanguageService.AppLanguage.Key.Equals(languageItem.Key))
                {
                    AppLanguage = languageItem;
                    LanguageCollection.Add(new LanguageModel()
                    {
                        LangaugeInfo = languageItem,
                        IsChecked = true
                    });
                }
                else
                {
                    LanguageCollection.Add(new LanguageModel()
                    {
                        LangaugeInfo = languageItem,
                        IsChecked = false
                    });
                }
            }

            foreach (GeographicRegion geographicItem in StoreRegionService.StoreRegionList)
            {
                if (StoreRegionService.StoreRegion.CodeTwoLetter.Equals(geographicItem.CodeTwoLetter))
                {
                    StoreRegion = geographicItem;
                    StoreRegionCollection.Add(new StoreRegionModel()
                    {
                        StoreRegionInfo = geographicItem,
                        IsChecked = true
                    });
                }
                else
                {
                    StoreRegionCollection.Add(new StoreRegionModel()
                    {
                        StoreRegionInfo = geographicItem,
                        IsChecked = false
                    });
                }
            }

            AlwaysShowBackdropEnabled = uiSettings.AdvancedEffectsEnabled && !Backdrop.Key.Equals(BackdropList[0].Key);
            uiSettings.AdvancedEffectsEnabledChanged += OnAdvancedEffectsEnabledChanged;
            GlobalNotificationService.ApplicationExit += OnApplicationExit;
            NotificationService.PropertyChanged += OnServicePropertyChanged;
            StoreRegionService.PropertyChanged += OnServicePropertyChanged;
        }

        #region 第一部分：重写父类事件

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            settingNavigationArgs = args.Parameter is not null && Enum.TryParse(Convert.ToString(args.Parameter), out AppNaviagtionArgs appNaviagtionArgs) ? appNaviagtionArgs : AppNaviagtionArgs.None;

            if (settingNavigationArgs is AppNaviagtionArgs.WinGetDataSource)
            {
                IsWinGetConfigMode = true;
                IsLoadedCompleted = false;
                await InitializeWinGetSourceDataAsync();
                IsLoadedCompleted = true;
            }
            else
            {
                IsWinGetConfigMode = false;
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 搜索时是否使用本数据源
        /// </summary>
        private void OnCheckBoxClickExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (args.Parameter is WinGetSourceModel winGetSourceItem)
            {
                foreach (WinGetSourceModel internalSourceItem in WinGetSourceInternalCollection)
                {
                    if (!internalSourceItem.Equals(winGetSourceItem))
                    {
                        internalSourceItem.IsSelected = false;
                    }
                }

                foreach (WinGetSourceModel customSourceItem in WinGetSourceCustomCollection)
                {
                    if (!customSourceItem.Equals(winGetSourceItem))
                    {
                        customSourceItem.IsSelected = false;
                    }
                }

                Task.Run(() =>
                {
                    KeyValuePair<string, bool> winGetDataSourceName = KeyValuePair.Create(winGetSourceItem.Name, winGetSourceItem.IsInternal);

                    if (winGetSourceItem.IsSelected)
                    {
                        WinGetConfigService.SetWinGetDataSourceName(winGetDataSourceName);
                    }
                    else
                    {
                        WinGetConfigService.RemoveWinGetDataSourceName(winGetDataSourceName);
                    }
                });
            }
        }

        /// <summary>
        /// 编辑 WinGet 数据源
        /// </summary>
        private async void OnEditExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (RuntimeHelper.IsElevated && args.Parameter is WinGetSourceModel winGetSourceItem)
            {
                WinGetSourceEditDialog winGetSourceEditDialog = new(WinGetSourceEditKind.Edit, winGetSourceItem);
                await MainWindow.Current.ShowDialogAsync(winGetSourceEditDialog);

                if (winGetSourceEditDialog.AddPackageCatalogStatusResult.HasValue && winGetSourceEditDialog.AddPackageCatalogStatusResult is AddPackageCatalogStatus.Ok)
                {
                    IsLoadedCompleted = false;
                    await InitializeWinGetSourceDataAsync();
                    IsLoadedCompleted = true;
                }
            }
            else
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.NotElevated));
            }
        }

        /// <summary>
        /// 移除数据源
        /// </summary>
        private async void OnRemoveExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (RuntimeHelper.IsElevated && args.Parameter is WinGetSourceModel winGetSourceItem)
            {
                foreach (WinGetSourceModel customSourceItem in WinGetSourceCustomCollection)
                {
                    if (customSourceItem.Name.Equals(winGetSourceItem.Name))
                    {
                        customSourceItem.IsOperating = true;
                        break;
                    }
                }

                RemovePackageCatalogResult removePackageCatalogResult = await Task.Run(async () =>
                {
                    PackageManager packageManager = new();
                    RemovePackageCatalogOptions removePackageCatalogOptions = new()
                    {
                        Name = winGetSourceItem.Name,
                        PreserveData = false
                    };

                    return await packageManager.RemovePackageCatalogAsync(removePackageCatalogOptions);
                });

                switch (removePackageCatalogResult.Status)
                {
                    case RemovePackageCatalogStatus.Ok:
                        {
                            await Task.Run(() =>
                            {
                                WinGetConfigService.RemoveWinGetDataSourceName(KeyValuePair.Create(winGetSourceItem.Name, winGetSourceItem.IsInternal));
                            });

                            foreach (WinGetSourceModel customSourceItem in WinGetSourceCustomCollection)
                            {
                                if (customSourceItem.Name.Equals(winGetSourceItem.Name))
                                {
                                    WinGetSourceCustomCollection.Remove(customSourceItem);
                                    break;
                                }
                            }

                            await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.WinGetSource, true, WinGetDataSourceRemoveSuccess));
                            break;
                        }
                    case RemovePackageCatalogStatus.GroupPolicyError:
                        {
                            foreach (WinGetSourceModel item in WinGetSourceCustomCollection)
                            {
                                if (item.Name.Equals(winGetSourceItem.Name))
                                {
                                    item.IsOperating = false;
                                    break;
                                }
                            }

                            await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceRemoveFailed, WinGetDataSourceRemoveGroupPolicyError, removePackageCatalogResult.ExtendedErrorCode is not null ? removePackageCatalogResult.ExtendedErrorCode.HResult : Unknown)));
                            break;
                        }
                    case RemovePackageCatalogStatus.CatalogError:
                        {
                            foreach (WinGetSourceModel customSourceItem in WinGetSourceCustomCollection)
                            {
                                if (customSourceItem.Name.Equals(winGetSourceItem.Name))
                                {
                                    customSourceItem.IsOperating = false;
                                    break;
                                }
                            }

                            await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceRemoveFailed, WinGetDataSourceRemoveCatalogError, removePackageCatalogResult.ExtendedErrorCode is not null ? removePackageCatalogResult.ExtendedErrorCode.HResult : Unknown)));
                            break;
                        }
                    case RemovePackageCatalogStatus.InternalError:
                        {
                            foreach (WinGetSourceModel customSourceItem in WinGetSourceCustomCollection)
                            {
                                if (customSourceItem.Name.Equals(winGetSourceItem.Name))
                                {
                                    customSourceItem.IsOperating = false;
                                    break;
                                }
                            }

                            await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceRemoveFailed, WinGetDataSourceRemoveInternalError, removePackageCatalogResult.ExtendedErrorCode is not null ? removePackageCatalogResult.ExtendedErrorCode.HResult : Unknown)));
                            break;
                        }
                    case RemovePackageCatalogStatus.InvalidOptions:
                        {
                            foreach (WinGetSourceModel customSourceItem in WinGetSourceCustomCollection)
                            {
                                if (customSourceItem.Name.Equals(winGetSourceItem.Name))
                                {
                                    customSourceItem.IsOperating = false;
                                    break;
                                }
                            }

                            await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceRemoveFailed, WinGetDataSourceRemoveInvalidOptions, removePackageCatalogResult.ExtendedErrorCode is not null ? removePackageCatalogResult.ExtendedErrorCode.HResult : Unknown)));
                            break;
                        }
                    case RemovePackageCatalogStatus.AccessDenied:
                        {
                            foreach (WinGetSourceModel customSourceItem in WinGetSourceCustomCollection)
                            {
                                if (customSourceItem.Name.Equals(winGetSourceItem.Name))
                                {
                                    customSourceItem.IsOperating = false;
                                    break;
                                }
                            }

                            await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceRemoveFailed, WinGetDataSourceRemoveAccessDenied, removePackageCatalogResult.ExtendedErrorCode is not null ? removePackageCatalogResult.ExtendedErrorCode.HResult : Unknown)));
                            break;
                        }
                }
            }
            else
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.NotElevated));
            }
        }

        /// <summary>
        /// 移除数据源
        /// </summary>
        private async void OnRemovePreserveDataExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (RuntimeHelper.IsElevated && args.Parameter is WinGetSourceModel winGetSourceItem)
            {
                foreach (WinGetSourceModel customSourceItem in WinGetSourceCustomCollection)
                {
                    if (customSourceItem.Name.Equals(winGetSourceItem.Name))
                    {
                        customSourceItem.IsOperating = true;
                        break;
                    }
                }

                RemovePackageCatalogResult removePackageCatalogResult = await Task.Run(async () =>
                {
                    PackageManager packageManager = new();
                    RemovePackageCatalogOptions removePackageCatalogOptions = new()
                    {
                        Name = winGetSourceItem.Name,
                        PreserveData = true
                    };

                    return await packageManager.RemovePackageCatalogAsync(removePackageCatalogOptions);
                });

                switch (removePackageCatalogResult.Status)
                {
                    case RemovePackageCatalogStatus.Ok:
                        {
                            await Task.Run(() =>
                            {
                                WinGetConfigService.RemoveWinGetDataSourceName(KeyValuePair.Create(winGetSourceItem.Name, winGetSourceItem.IsInternal));
                            });

                            foreach (WinGetSourceModel customSourceItem in WinGetSourceCustomCollection)
                            {
                                if (customSourceItem.Name.Equals(winGetSourceItem.Name))
                                {
                                    WinGetSourceCustomCollection.Remove(customSourceItem);
                                    break;
                                }
                            }

                            await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.WinGetSource, true, WinGetDataSourceRemoveSuccess));
                            break;
                        }
                    case RemovePackageCatalogStatus.GroupPolicyError:
                        {
                            foreach (WinGetSourceModel customSourceItem in WinGetSourceCustomCollection)
                            {
                                if (customSourceItem.Name.Equals(winGetSourceItem.Name))
                                {
                                    customSourceItem.IsOperating = false;
                                    break;
                                }
                            }

                            await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceRemoveFailed, WinGetDataSourceRemoveGroupPolicyError, removePackageCatalogResult.ExtendedErrorCode is not null ? removePackageCatalogResult.ExtendedErrorCode.HResult : Unknown)));
                            break;
                        }
                    case RemovePackageCatalogStatus.CatalogError:
                        {
                            foreach (WinGetSourceModel customSourceItem in WinGetSourceCustomCollection)
                            {
                                if (customSourceItem.Name.Equals(winGetSourceItem.Name))
                                {
                                    customSourceItem.IsOperating = false;
                                    break;
                                }
                            }

                            await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceRemoveFailed, WinGetDataSourceRemoveGroupPolicyError, removePackageCatalogResult.ExtendedErrorCode is not null ? removePackageCatalogResult.ExtendedErrorCode.HResult : Unknown)));
                            break;
                        }
                    case RemovePackageCatalogStatus.InternalError:
                        {
                            foreach (WinGetSourceModel customSourceItem in WinGetSourceCustomCollection)
                            {
                                if (customSourceItem.Name.Equals(winGetSourceItem.Name))
                                {
                                    customSourceItem.IsOperating = false;
                                    break;
                                }
                            }

                            await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceRemoveFailed, WinGetDataSourceRemoveGroupPolicyError, removePackageCatalogResult.ExtendedErrorCode is not null ? removePackageCatalogResult.ExtendedErrorCode.HResult : Unknown)));
                            break;
                        }
                    case RemovePackageCatalogStatus.InvalidOptions:
                        {
                            foreach (WinGetSourceModel customSourceItem in WinGetSourceCustomCollection)
                            {
                                if (customSourceItem.Name.Equals(winGetSourceItem.Name))
                                {
                                    customSourceItem.IsOperating = false;
                                    break;
                                }
                            }

                            await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceRemoveFailed, WinGetDataSourceRemoveGroupPolicyError, removePackageCatalogResult.ExtendedErrorCode is not null ? removePackageCatalogResult.ExtendedErrorCode.HResult : Unknown)));
                            break;
                        }
                    case RemovePackageCatalogStatus.AccessDenied:
                        {
                            foreach (WinGetSourceModel customSourceItem in WinGetSourceCustomCollection)
                            {
                                if (customSourceItem.Name.Equals(winGetSourceItem.Name))
                                {
                                    customSourceItem.IsOperating = false;
                                    break;
                                }
                            }

                            await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.WinGetSource, false, string.Format(WinGetDataSourceRemoveFailed, WinGetDataSourceRemoveGroupPolicyError, removePackageCatalogResult.ExtendedErrorCode is not null ? removePackageCatalogResult.ExtendedErrorCode.HResult : Unknown)));
                            break;
                        }
                }
            }
            else
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.NotElevated));
            }
        }

        /// <summary>
        /// 修改应用语言
        /// </summary>
        private async void OnLanguageExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (LanguageFlyout.IsOpen)
            {
                LanguageFlyout.Hide();
            }

            if (args.Parameter is LanguageModel languageItem)
            {
                foreach (LanguageModel item in LanguageCollection)
                {
                    item.IsChecked = false;
                    if (languageItem.LangaugeInfo.Key.Equals(item.LangaugeInfo.Key))
                    {
                        AppLanguage = item.LangaugeInfo;
                        item.IsChecked = true;
                    }
                }

                LanguageService.SetLanguage(AppLanguage);
                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.LanguageChange));
            }
        }

        /// <summary>
        /// 修改商店区域
        /// </summary>
        private void OnStoreRegionExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (StoreRegionFlyout.IsOpen)
            {
                StoreRegionFlyout.Hide();
            }

            if (args.Parameter is StoreRegionModel regionItem)
            {
                foreach (StoreRegionModel item in StoreRegionCollection)
                {
                    item.IsChecked = false;
                    if (regionItem.StoreRegionInfo.CodeTwoLetter.Equals(item.StoreRegionInfo.CodeTwoLetter))
                    {
                        StoreRegion = item.StoreRegionInfo;
                        item.IsChecked = true;
                    }
                }

                StoreRegionService.SetRegion(StoreRegion);
            }
        }

        #endregion 第二部分：XamlUICommand 命令调用时挂载的事件

        #region 第三部分：设置页面——挂载的事件

        /// <summary>
        /// 页面加载完成后如果有具体的要求，将页面滚动到指定位置
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            double currentScrollPosition = SettingsScroll.VerticalOffset;
            Point currentPoint = new(0, (int)currentScrollPosition);

            if (settingNavigationArgs is AppNaviagtionArgs.DownloadOptions)
            {
                Point targetPosition = DownloadOptions.TransformToVisual(SettingsScroll).TransformPoint(currentPoint);
                SettingsScroll.ScrollTo(0, targetPosition.Y, new ScrollingScrollOptions(ScrollingAnimationMode.Disabled));
            }
            else
            {
                SettingsScroll.ScrollTo(0, 0, new ScrollingScrollOptions(ScrollingAnimationMode.Disabled));
            }
        }

        /// <summary>
        /// 打开重启应用确认的窗口对话框
        /// </summary>
        private async void OnRestartAppsClicked(object sender, RoutedEventArgs args)
        {
            await MainWindow.Current.ShowDialogAsync(new RestartAppsDialog());
        }

        /// <summary>
        /// 设置说明
        /// </summary>
        private void OnSettingsInstructionClicked(object sender, RoutedEventArgs args)
        {
            MainWindow.Current.NavigateTo(typeof(AboutPage), AppNaviagtionArgs.SettingsHelp);
        }

        /// <summary>
        /// 打开系统主题设置
        /// </summary>
        private async void OnSystemThemeSettingsClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:colors"));
        }

        /// <summary>
        /// 主题修改设置
        /// </summary>
        private void OnThemeSelectClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is string tag)
            {
                Theme = ThemeList[Convert.ToInt32(tag)];
                ThemeService.SetTheme(Theme);
            }
        }

        /// <summary>
        /// 背景色修改设置
        /// </summary>
        private void OnBackdropSelectClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is string tag)
            {
                Backdrop = BackdropList[Convert.ToInt32(tag)];
                BackdropService.SetBackdrop(Backdrop);
                AlwaysShowBackdropEnabled = uiSettings.AdvancedEffectsEnabled && !Backdrop.Key.Equals(BackdropList[0].Key);

                if (Backdrop.Equals(BackdropList[0]))
                {
                    AlwaysShowBackdropService.SetAlwaysShowBackdropValue(false);
                    AlwaysShowBackdropValue = false;
                }
            }
        }

        /// <summary>
        /// 打开系统背景色设置
        /// </summary>
        private async void OnSystemBackdropSettingsClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:easeofaccess-visualeffects"));
        }

        /// <summary>
        /// 打开系统通知设置
        /// </summary>
        private async void OnSystemNotificationSettingsClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:notifications"));
        }

        /// <summary>
        /// 打开系统语言设置
        /// </summary>
        private async void OnSystemLanguageSettingsClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:regionlanguage-languageoptions"));
        }

        /// <summary>
        /// 选择网页浏览器渲染网页使用的内核
        /// </summary>
        private void OnWebKernelSelectClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is string tag)
            {
                WebKernelItem = WebKernelList[Convert.ToInt32(tag)];
                WebKernelService.SetWebKernel(WebKernelItem);
            }
        }

        /// <summary>
        /// 选择查询链接方式
        /// </summary>
        private void OnQueryLinksModeSelectClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is string tag)
            {
                QueryLinksModeItem = QueryLinksModeList[Convert.ToInt32(tag)];
                QueryLinksModeService.SetQueryLinksMode(QueryLinksModeItem);
            }
        }

        /// <summary>
        /// 重新启动资源管理器
        /// </summary>
        private async void OnRestartExplorerClicked(object sender, RoutedEventArgs args)
        {
            IsRestarting = true;

            await Task.Run(() =>
            {
                try
                {
                    int dwRmStatus = RstrtmgrLibrary.RmStartSession(out uint dwSessionHandle, 0, GuidHelper.Empty.ToString());

                    if (dwRmStatus is 0)
                    {
                        List<uint> processPIDList = ProcessHelper.GetProcessPIDByName("explorer.exe");
                        RM_UNIQUE_PROCESS[] lpRmProcList = new RM_UNIQUE_PROCESS[processPIDList.Count];

                        for (int index = 0; index < processPIDList.Count; index++)
                        {
                            lpRmProcList[index].dwProcessId = (int)processPIDList[index];
                            IntPtr hProcess = Kernel32Library.OpenProcess(EDesiredAccess.PROCESS_QUERY_LIMITED_INFORMATION, false, (int)processPIDList[index]);
                            lpRmProcList[index].ProcessStartTime = hProcess != IntPtr.Zero && Kernel32Library.GetProcessTimes(hProcess, out FILETIME creationTime, out FILETIME exitTime, out FILETIME kernelTime, out FILETIME userTime) ? creationTime : new();
                        }

                        dwRmStatus = RstrtmgrLibrary.RmRegisterResources(dwSessionHandle, 0, null, (uint)processPIDList.Count, lpRmProcList, 0, null);

                        if (dwRmStatus is 0)
                        {
                            dwRmStatus = RstrtmgrLibrary.RmShutdown(dwSessionHandle, RM_SHUTDOWN_TYPE.RmForceShutdown, null);

                            if (dwRmStatus is 0)
                            {
                                dwRmStatus = RstrtmgrLibrary.RmRestart(dwSessionHandle, 0, null);

                                if (dwRmStatus is 0)
                                {
                                    dwRmStatus = RstrtmgrLibrary.RmEndSession(dwSessionHandle);
                                }
                                else
                                {
                                    LogService.WriteLog(LoggingLevel.Error, "Restart explorer restart failed", new Exception());
                                }
                            }
                            else
                            {
                                LogService.WriteLog(LoggingLevel.Error, "Restart explorer shutdown failed", new Exception());
                            }
                        }
                        else
                        {
                            LogService.WriteLog(LoggingLevel.Error, "Restart explorer register resources failed", new Exception());
                        }
                    }
                    else
                    {
                        LogService.WriteLog(LoggingLevel.Error, "Restart explorer start session failed", new Exception());
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "Restart explorer failed", e);
                }
            });

            IsRestarting = false;
        }

        /// <summary>
        /// 是否开启显示文件右键菜单
        /// </summary>
        private void OnShellMenuToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                ShellMenuService.SetShellMenuValue(toggleSwitch.IsOn);
                ShellMenuValue = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 配置 WinGet 数据源
        /// </summary>
        private async void OnConfigurationClicked(object sender, RoutedEventArgs args)
        {
            if (!IsWinGetConfigMode && WinGetConfigService.IsWinGetInstalled)
            {
                IsWinGetConfigMode = true;
                IsLoadedCompleted = false;
                await InitializeWinGetSourceDataAsync();
                IsLoadedCompleted = true;
            }
        }

        /// <summary>
        /// WinGet 程序包安装方式设置
        /// </summary>
        private void OnWinGetInstallModeSelectClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is string tag)
            {
                WinGetInstallMode = WinGetInstallModeList[Convert.ToInt32(tag)];
                WinGetConfigService.SetWinGetInstallMode(WinGetInstallMode);
            }
        }

        /// <summary>
        /// 打开 WinGet 程序包设置
        /// </summary>
        private void OnOpenWinGetSettingsClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                if (ApplicationDataManager.CreateForPackageFamily("Microsoft.DesktopAppInstaller_8wekyb3d8bbwe") is ApplicationData applicationData)
                {
                    string wingetConfigFilePath = Path.Combine(applicationData.LocalFolder.Path, "settings.json");

                    if (File.Exists(wingetConfigFilePath))
                    {
                        await Launcher.LaunchFileAsync(await StorageFile.GetFileFromPathAsync(wingetConfigFilePath));
                    }
                    else
                    {
                        Shell32Library.ShellExecute(IntPtr.Zero, "open", "winget.exe", "settings", null, WindowShowStyle.SW_HIDE);
                    }
                }
            });
        }

        /// <summary>
        /// 打开下载文件存放目录
        /// </summary>
        private async void OnDownloadOpenFolderClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            await DownloadOptionsService.OpenFolderAsync(DownloadFolder);
        }

        /// <summary>
        /// 修改下载文件存放目录
        /// </summary>
        private async void OnDownloadChangeFolderClicked(object sender, RoutedEventArgs args)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem && menuFlyoutItem.Tag is string tag)
            {
                switch (tag)
                {
                    case "AppCache":
                        {
                            DownloadFolder = DownloadOptionsService.DefaultDownloadFolder;
                            DownloadOptionsService.SetFolder(DownloadFolder);
                            break;
                        }
                    case "Download":
                        {
                            DownloadFolder = await StorageFolder.GetFolderFromPathAsync(InfoHelper.UserDataPath.Downloads);
                            DownloadOptionsService.SetFolder(DownloadFolder);
                            break;
                        }
                    case "Desktop":
                        {
                            DownloadFolder = await StorageFolder.GetFolderFromPathAsync(InfoHelper.UserDataPath.Desktop);
                            DownloadOptionsService.SetFolder(DownloadFolder);
                            break;
                        }
                    case "Custom":
                        {
                            try
                            {
                                FolderPicker folderPicker = new(MainWindow.Current.AppWindow.Id)
                                {
                                    SuggestedStartLocation = PickerLocationId.Downloads
                                };

                                if (await folderPicker.PickSingleFolderAsync() is PickFolderResult pickFolderResult)
                                {
                                    DownloadFolder = await StorageFolder.GetFolderFromPathAsync(pickFolderResult.Path);
                                    DownloadOptionsService.SetFolder(DownloadFolder);
                                }
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(LoggingLevel.Error, "Open folderPicker failed", e);
                                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.FolderPicker));
                            }

                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 打开传递优化设置
        /// </summary>
        private async void OnOpenDeliveryOptimizationClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:delivery-optimization"));
        }

        /// <summary>
        /// 下载引擎说明
        /// </summary>
        private void OnLearnDoEngineClicked(object sender, RoutedEventArgs args)
        {
            MainWindow.Current.NavigateTo(typeof(AboutPage), AppNaviagtionArgs.SettingsHelp);
        }

        /// <summary>
        /// 下载引擎方式设置
        /// </summary>

        private void OnDoEngineModeSelectClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is string tag)
            {
                DoEngineMode = DoEngineModeList[Convert.ToInt32(tag)];
                DownloadOptionsService.SetDoEngineMode(DoEngineMode);
            }
        }

        /// <summary>
        /// 打开开发者选项
        /// </summary>
        private async void OnOpenDevelopersClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:developers"));
        }

        /// <summary>
        /// 是否允许安装未签名的安装包
        /// </summary>
        private void OnAllowUnsignedPackageToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                AppInstallService.SetAllowUnsignedPackageValue(toggleSwitch.IsOn);
                AllowUnsignedPackageValue = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 是否在安装应用时强制关闭与包关联的进程
        /// </summary>
        private void OnForceAppShutdownToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                AppInstallService.SetForceAppShutdownValue(toggleSwitch.IsOn);
                ForceAppShutdownValue = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 是否在安装应用时强制关闭与包关联的进程
        /// </summary>
        private void OnForceTargetAppShutdownToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                AppInstallService.SetForceTargetAppShutdownValue(toggleSwitch.IsOn);
                ForceTargetAppShutdownValue = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 应用安装方式设置
        /// </summary>
        private void OnInstallModeSelectClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is string tag)
            {
                InstallMode = InstallModeList[Convert.ToInt32(tag)];
                InstallModeService.SetInstallMode(InstallMode);
            }
        }

        /// <summary>
        /// 清理应用内使用的所有痕迹
        /// </summary>
        private async void OnTraceCleanupClicked(object sender, RoutedEventArgs args)
        {
            await MainWindow.Current.ShowDialogAsync(new TraceCleanupPromptDialog());
        }

        /// <summary>
        /// 打开日志文件夹
        /// </summary>
        private async void OnOpenLogFolderClicked(object sender, RoutedEventArgs args)
        {
            await LogService.OpenLogFolderAsync();
        }

        /// <summary>
        /// 清除所有日志记录
        /// </summary>
        private async void OnClearClicked(object sender, RoutedEventArgs args)
        {
            bool result = await LogService.ClearLogAsync();
            await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.LogClean, result));
        }

        /// <summary>
        /// 开关按钮切换时修改相应设置
        /// </summary>
        private void OnAlwaysShowBackdropToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                AlwaysShowBackdropService.SetAlwaysShowBackdropValue(toggleSwitch.IsOn);
                AlwaysShowBackdropValue = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 语言设置菜单打开时自动定位到选中项
        /// </summary>
        private void OnLanguageFlyoutOpened(object sender, object args)
        {
            for (int index = 0; index < LanguageCollection.Count; index++)
            {
                if (LanguageCollection[index].IsChecked)
                {
                    LanguageItemsView.ScrollView.ScrollTo(0, index * 32 - 134, new ScrollingScrollOptions(ScrollingAnimationMode.Disabled));
                    break;
                }
            }
        }

        /// <summary>
        /// 是否开启应用窗口置顶
        /// </summary>
        private void OnTopMostToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                TopMostService.SetTopMostValue(toggleSwitch.IsOn);
                TopMostValue = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 设置是否开启应用通知
        /// </summary>
        private void OnNotificationToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                NotificationService.SetNotification(toggleSwitch.IsOn);
                NotificationValue = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 打开系统区域设置
        /// </summary>
        private async void OnSystemRegionSettingsClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:regionformatting"));
        }

        /// <summary>
        /// 设置是否使用系统默认区域
        /// </summary>
        private void OnUseSystemRegionToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                StoreRegionService.SetUseSystemRegionValue(toggleSwitch.IsOn);
                UseSystemRegionValue = toggleSwitch.IsOn;

                if (UseSystemRegionValue)
                {
                    StoreRegion = StoreRegionService.DefaultStoreRegion;
                    StoreRegionService.SetRegion(StoreRegion);

                    foreach (StoreRegionModel item in StoreRegionCollection)
                    {
                        item.IsChecked = false;
                        if (StoreRegion.CodeTwoLetter.Equals(item.StoreRegionInfo.CodeTwoLetter))
                        {
                            StoreRegion = item.StoreRegionInfo;
                            item.IsChecked = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 区域设置菜单打开时自动定位到选中项
        /// </summary>
        private void OnStoreRegionFlyoutOpened(object sender, object args)
        {
            for (int index = 0; index < StoreRegionCollection.Count; index++)
            {
                if (StoreRegionCollection[index].IsChecked)
                {
                    StoreRegionItemsView.ScrollView.ScrollTo(0, index * 32 - 134, new ScrollingScrollOptions(ScrollingAnimationMode.Disabled));
                    break;
                }
            }
        }

        /// <summary>
        /// 设置是否过滤加密包文件
        /// </summary>
        private void OnEncryptedPackageToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                LinkFilterService.SetEncryptedPackageFilterValue(toggleSwitch.IsOn);
                EncryptedPackageFilterValue = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 设置是否过滤包块映射文件
        /// </summary>
        private void OnBlockMapToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                LinkFilterService.SetBlockMapFilterValue(toggleSwitch.IsOn);
                BlockMapFilterValue = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 添加数据源
        /// </summary>
        private async void OnAddNewSourceClicked(object sender, RoutedEventArgs args)
        {
            if (RuntimeHelper.IsElevated)
            {
                WinGetSourceEditDialog winGetSourceEditDialog = new(WinGetSourceEditKind.Add, null);
                await MainWindow.Current.ShowDialogAsync(winGetSourceEditDialog);

                if (winGetSourceEditDialog.AddPackageCatalogStatusResult.HasValue && winGetSourceEditDialog.AddPackageCatalogStatusResult is AddPackageCatalogStatus.Ok)
                {
                    IsLoadedCompleted = false;
                    await InitializeWinGetSourceDataAsync();
                    IsLoadedCompleted = true;
                }
            }
            else
            {
                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.NotElevated));
            }
        }

        /// <summary>
        /// 打开下载文件存放目录
        /// </summary>
        private async void OnWinGetOpenFolderClicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            await WinGetConfigService.OpenFolderAsync(WinGetPackageFolder);
        }

        /// <summary>
        /// 修改下载文件存放目录
        /// </summary>
        private async void OnWinGetChangeFolderClicked(object sender, RoutedEventArgs args)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem && menuFlyoutItem.Tag is string tag)
            {
                switch (tag)
                {
                    case "AppCache":
                        {
                            WinGetPackageFolder = WinGetConfigService.DefaultDownloadFolder;
                            WinGetConfigService.SetFolder(WinGetPackageFolder);
                            break;
                        }
                    case "Download":
                        {
                            WinGetPackageFolder = await StorageFolder.GetFolderFromPathAsync(InfoHelper.UserDataPath.Downloads);
                            WinGetConfigService.SetFolder(WinGetPackageFolder);
                            break;
                        }
                    case "Desktop":
                        {
                            WinGetPackageFolder = await StorageFolder.GetFolderFromPathAsync(InfoHelper.UserDataPath.Desktop);
                            WinGetConfigService.SetFolder(WinGetPackageFolder);
                            break;
                        }
                    case "Custom":
                        {
                            try
                            {
                                FolderPicker folderPicker = new(MainWindow.Current.AppWindow.Id)
                                {
                                    SuggestedStartLocation = PickerLocationId.Downloads
                                };

                                if (await folderPicker.PickSingleFolderAsync() is PickFolderResult pickFolderResult)
                                {
                                    WinGetPackageFolder = await StorageFolder.GetFolderFromPathAsync(pickFolderResult.Path);
                                    WinGetConfigService.SetFolder(WinGetPackageFolder);
                                }
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(LoggingLevel.Error, "Open folderPicker failed", e);
                                await MainWindow.Current.ShowNotificationAsync(new OperationResultTip(OperationKind.FolderPicker));
                            }

                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 刷新
        /// </summary>
        private async void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            IsLoadedCompleted = false;
            await InitializeWinGetSourceDataAsync();
            IsLoadedCompleted = true;
        }

        #endregion 第三部分：设置页面——挂载的事件

        #region 第四部分：自定义事件

        /// <summary>
        /// 应用程序退出时触发的事件
        /// </summary>
        private void OnApplicationExit(object sender, EventArgs args)
        {
            try
            {
                GlobalNotificationService.ApplicationExit -= OnApplicationExit;
                uiSettings.ColorValuesChanged -= OnAdvancedEffectsEnabledChanged;
                NotificationService.PropertyChanged -= OnServicePropertyChanged;
                StoreRegionService.PropertyChanged -= OnServicePropertyChanged;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Unregister store region service event failed", e);
            }
        }

        /// <summary>
        /// 在启用或禁用系统高级 UI 效果设置时发生的事件
        /// </summary>
        private void OnAdvancedEffectsEnabledChanged(UISettings sender, object args)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                AdvancedEffectsEnabled = uiSettings.AdvancedEffectsEnabled;
                AlwaysShowBackdropEnabled = uiSettings.AdvancedEffectsEnabled && !Backdrop.Key.Equals(BackdropList[0].Key);
            });
        }

        /// <summary>
        /// 设置选项发生变化时触发的事件
        /// </summary>
        private void OnServicePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName.Equals(nameof(NotificationService.NotificationSetting)))
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    NotificationEnabled = NotificationService.NotificationSetting is NotificationSetting.Enabled;
                });
            }
            else if (args.PropertyName.Equals(nameof(StoreRegionService.StoreRegion)))
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    if (!CurrentCountryOrRegion.CodeTwoLetter.Equals(StoreRegionService.DefaultStoreRegion.CodeTwoLetter))
                    {
                        CurrentCountryOrRegion = StoreRegionService.DefaultStoreRegion;
                    }

                    if (UseSystemRegionValue)
                    {
                        StoreRegion = StoreRegionService.DefaultStoreRegion;

                        foreach (StoreRegionModel item in StoreRegionCollection)
                        {
                            item.IsChecked = false;
                            if (StoreRegion.CodeTwoLetter.Equals(item.StoreRegionInfo.CodeTwoLetter))
                            {
                                StoreRegion = item.StoreRegionInfo;
                                item.IsChecked = true;
                            }
                        }
                    }
                });
            }
        }

        #endregion 第四部分：自定义事件

        /// <summary>
        /// 初始化 WinGet 数据源信息
        /// </summary>
        private async Task InitializeWinGetSourceDataAsync()
        {
            WinGetSourceInternalCollection.Clear();
            WinGetSourceCustomCollection.Clear();

            List<WinGetSourceModel> winGetSourceInternalList = await Task.Run(() =>
            {
                PackageManager packageManager = new();
                List<WinGetSourceModel> winGetSourceInternalList = [];
                KeyValuePair<string, bool> winGetDataSourceName = WinGetConfigService.GetWinGetDataSourceName();

                List<PackageCatalogReference> predefinedPackageCatalogReferenceList = [];
                foreach (PredefinedPackageCatalog predefinedPackageCatalog in Enum.GetValues<PredefinedPackageCatalog>())
                {
                    PackageCatalogReference packageCatalogReference = packageManager.GetPredefinedPackageCatalog(predefinedPackageCatalog);

                    PackageCatalogInformation packageCatalogInformation = new()
                    {
                        Name = packageCatalogReference.Info.Name,
                        Arguments = packageCatalogReference.Info.Argument,
                        Explicit = packageCatalogReference.Info.Explicit,
                        TrustLevel = packageCatalogReference.Info.TrustLevel,
                        Id = packageCatalogReference.Info.Id,
                        LastUpdateTime = packageCatalogReference.Info.LastUpdateTime,
                        Origin = packageCatalogReference.Info.Origin,
                        Type = packageCatalogReference.Info.Type,
                        AcceptSourceAgreements = packageCatalogReference.AcceptSourceAgreements,
                        AdditionalPackageCatalogArguments = packageCatalogReference.AdditionalPackageCatalogArguments,
                        AuthenticationType = packageCatalogReference.AuthenticationInfo.AuthenticationType,
                        AuthenticationAccount = packageCatalogReference.AuthenticationArguments is not null && !string.IsNullOrEmpty(packageCatalogReference.AuthenticationArguments.AuthenticationAccount) ? packageCatalogReference.AuthenticationArguments.AuthenticationAccount : string.Empty,
                        PackageCatalogBackgroundUpdateInterval = packageCatalogReference.PackageCatalogBackgroundUpdateInterval,
                    };

                    WinGetSourceModel winGetSourceItem = new()
                    {
                        IsOperating = false,
                        IsSelected = winGetDataSourceName.Equals(KeyValuePair.Create(packageCatalogInformation.Name, true)),
                        PackageCatalogInformation = packageCatalogInformation,
                        Name = packageCatalogInformation.Name,
                        Arguments = string.IsNullOrEmpty(packageCatalogInformation.Arguments) ? None : packageCatalogReference.Info.Argument,
                        Explicit = packageCatalogInformation.Explicit ? Yes : No,
                        TrustLevel = packageCatalogInformation.TrustLevel is PackageCatalogTrustLevel.Trusted ? Trusted : Distrusted,
                        Id = packageCatalogInformation.Id,
                        LastUpdateTime = packageCatalogInformation.LastUpdateTime.ToString("yyyy/MM/dd HH:mm"),
                        Origin = packageCatalogInformation.Origin is PackageCatalogOrigin.Predefined ? Predefined : User,
                        Type = packageCatalogInformation.Type,
                        AcceptSourceAgreements = packageCatalogInformation.AcceptSourceAgreements ? Yes : No,
                        AuthenticationType = packageCatalogInformation.AuthenticationType switch
                        {
                            AuthenticationType.None => None,
                            AuthenticationType.Unknown => Unknown,
                            AuthenticationType.MicrosoftEntraId => MicrosoftEntraId,
                            AuthenticationType.MicrosoftEntraIdForAzureBlobStorage => MicrosoftEntraIdForAzureBlobStorage,
                            _ => Unknown
                        },
                        AdditionalPackageCatalogArguments = string.IsNullOrEmpty(packageCatalogInformation.AdditionalPackageCatalogArguments) ? None : packageCatalogInformation.AdditionalPackageCatalogArguments,
                        AuthenticationAccount = string.IsNullOrEmpty(packageCatalogInformation.AuthenticationAccount) ? None : packageCatalogInformation.AuthenticationAccount,
                        PackageCatalogBackgroundUpdateInterval = packageCatalogInformation.PackageCatalogBackgroundUpdateInterval.ToString(),
                        IsInternal = true,
                        PredefinedPackageCatalog = predefinedPackageCatalog
                    };

                    winGetSourceInternalList.Add(winGetSourceItem);
                }

                return winGetSourceInternalList;
            });

            List<WinGetSourceModel> wingetSourceCustomList = await Task.Run(() =>
            {
                PackageManager packageManager = new();
                List<WinGetSourceModel> wingetSourceCustomList = [];
                KeyValuePair<string, bool> winGetDataSourceName = WinGetConfigService.GetWinGetDataSourceName();

                IReadOnlyList<PackageCatalogReference> packageCatalogReferenceList = packageManager.GetPackageCatalogs();

                for (int index = 0; index < packageCatalogReferenceList.Count; index++)
                {
                    PackageCatalogReference packageCatalogReference = packageCatalogReferenceList[index];

                    PackageCatalogInformation packageCatalogInformation = new()
                    {
                        Name = packageCatalogReference.Info.Name,
                        Arguments = packageCatalogReference.Info.Argument,
                        Explicit = packageCatalogReference.Info.Explicit,
                        TrustLevel = packageCatalogReference.Info.TrustLevel,
                        Id = packageCatalogReference.Info.Id,
                        LastUpdateTime = packageCatalogReference.Info.LastUpdateTime,
                        Origin = packageCatalogReference.Info.Origin,
                        Type = packageCatalogReference.Info.Type,
                        AcceptSourceAgreements = packageCatalogReference.AcceptSourceAgreements,
                        AdditionalPackageCatalogArguments = packageCatalogReference.AdditionalPackageCatalogArguments,
                        AuthenticationType = packageCatalogReference.AuthenticationInfo.AuthenticationType,
                        AuthenticationAccount = packageCatalogReference.AuthenticationArguments is not null && !string.IsNullOrEmpty(packageCatalogReference.AuthenticationArguments.AuthenticationAccount) ? packageCatalogReference.AuthenticationArguments.AuthenticationAccount : string.Empty,
                        PackageCatalogBackgroundUpdateInterval = packageCatalogReference.PackageCatalogBackgroundUpdateInterval,
                    };

                    WinGetSourceModel winGetSourceItem = new()
                    {
                        IsOperating = false,
                        IsSelected = winGetDataSourceName.Equals(KeyValuePair.Create(packageCatalogInformation.Name, false)),
                        PackageCatalogInformation = packageCatalogInformation,
                        Name = packageCatalogInformation.Name,
                        Arguments = string.IsNullOrEmpty(packageCatalogInformation.Arguments) ? None : packageCatalogReference.Info.Argument,
                        Explicit = packageCatalogInformation.Explicit ? Yes : No,
                        TrustLevel = packageCatalogInformation.TrustLevel is PackageCatalogTrustLevel.Trusted ? Trusted : Distrusted,
                        Id = packageCatalogInformation.Id,
                        LastUpdateTime = packageCatalogInformation.LastUpdateTime.ToString("yyyy/MM/dd HH:mm"),
                        Origin = packageCatalogInformation.Origin is PackageCatalogOrigin.Predefined ? Predefined : User,
                        Type = packageCatalogInformation.Type,
                        AcceptSourceAgreements = packageCatalogInformation.AcceptSourceAgreements ? Yes : No,
                        AuthenticationType = packageCatalogInformation.AuthenticationType switch
                        {
                            AuthenticationType.None => None,
                            AuthenticationType.Unknown => Unknown,
                            AuthenticationType.MicrosoftEntraId => MicrosoftEntraId,
                            AuthenticationType.MicrosoftEntraIdForAzureBlobStorage => MicrosoftEntraIdForAzureBlobStorage,
                            _ => Unknown
                        },
                        AdditionalPackageCatalogArguments = string.IsNullOrEmpty(packageCatalogInformation.AdditionalPackageCatalogArguments) ? None : packageCatalogInformation.AdditionalPackageCatalogArguments,
                        AuthenticationAccount = string.IsNullOrEmpty(packageCatalogInformation.AuthenticationAccount) ? None : packageCatalogInformation.AuthenticationAccount,
                        PackageCatalogBackgroundUpdateInterval = packageCatalogInformation.PackageCatalogBackgroundUpdateInterval.ToString(),
                        IsInternal = false
                    };

                    wingetSourceCustomList.Add(winGetSourceItem);
                }

                return wingetSourceCustomList;
            });

            foreach (WinGetSourceModel wingetSourceItem in winGetSourceInternalList)
            {
                WinGetSourceInternalCollection.Add(wingetSourceItem);
            }

            foreach (WinGetSourceModel wingetSourceItem in wingetSourceCustomList)
            {
                WinGetSourceCustomCollection.Add(wingetSourceItem);
            }

            await Task.Delay(500);
        }

        private string LocalizeDisplayNumber(KeyValuePair<string, string> selectedBackdrop)
        {
            int index = BackdropList.FindIndex(item => item.Key.Equals(selectedBackdrop.Key));

            if (index is 0)
            {
                return selectedBackdrop.Value;
            }
            else if (index is 1 or 2)
            {
                return string.Join(' ', ResourceService.GetLocalized("Settings/Mica"), selectedBackdrop.Value);
            }
            else if (index is 3 or 4 or 5)
            {
                return string.Join(' ', ResourceService.GetLocalized("Settings/DesktopAcrylic"), selectedBackdrop.Value);
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
