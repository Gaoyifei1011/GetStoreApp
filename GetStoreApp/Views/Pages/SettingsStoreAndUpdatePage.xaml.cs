using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Settings;
using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;
using Windows.Globalization;
using Windows.System;
using WinRT;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 设置商店与更新页面
    /// </summary>
    public sealed partial class SettingsStoreAndUpdatePage : Page, INotifyPropertyChanged
    {
        private readonly string AppLinkOpenModeBuiltInAppString = ResourceService.GetLocalized("SettingsStoreAndUpdate/AppLinkOpenModeBuiltInApp");
        private readonly string AppLinkOpenModeSystemBrowserString = ResourceService.GetLocalized("SettingsStoreAndUpdate/AppLinkOpenModeSystemBrowser");
        private readonly string QueryLinksModeOfficialString = ResourceService.GetLocalized("SettingsStoreAndUpdate/QueryLinksModeOfficial");
        private readonly string QueryLinksModeThirdPartyString = ResourceService.GetLocalized("SettingsStoreAndUpdate/QueryLinksModeThirdParty");
        private readonly string InstallModeAppInstallString = ResourceService.GetLocalized("SettingsStoreAndUpdate/InstallModeAppInstall");
        private readonly string InstallModeCodeInstallString = ResourceService.GetLocalized("SettingsStoreAndUpdate/InstallModeCodeInstall");

        private ComboBoxItemModel _queryLinksMode;

        public ComboBoxItemModel QueryLinksMode
        {
            get { return _queryLinksMode; }

            set
            {
                if (!Equals(_queryLinksMode, value))
                {
                    _queryLinksMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(QueryLinksMode)));
                }
            }
        }

        private ComboBoxItemModel _searchAppsMode;

        public ComboBoxItemModel SearchAppsMode
        {
            get { return _searchAppsMode; }

            set
            {
                if (!Equals(_searchAppsMode, value))
                {
                    _searchAppsMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SearchAppsMode)));
                }
            }
        }

        private ComboBoxItemModel _appLinkOpenMode;

        public ComboBoxItemModel AppLinkOpenMode
        {
            get { return _appLinkOpenMode; }

            set
            {
                if (!Equals(_appLinkOpenMode, value))
                {
                    _appLinkOpenMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AppLinkOpenMode)));
                }
            }
        }

        private ComboBoxItemModel _installMode;

        public ComboBoxItemModel InstallMode
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

        private bool _cancelAutoUpdate = CancelAutoUpdateService.CancelAutoUpdate;

        public bool CancelAutoUpdate
        {
            get { return _cancelAutoUpdate; }

            set
            {
                if (!Equals(_cancelAutoUpdate, value))
                {
                    _cancelAutoUpdate = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CancelAutoUpdate)));
                }
            }
        }

        private bool _useSystemRegion = StoreRegionService.UseSystemRegion;

        public bool UseSystemRegion
        {
            get { return _useSystemRegion; }

            set
            {
                if (!Equals(_useSystemRegion, value))
                {
                    _useSystemRegion = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UseSystemRegion)));
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

        private StoreRegionModel _storeRegion;

        public StoreRegionModel StoreRegion
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

        private bool _encryptedPackageFilter = LinkFilterService.EncryptedPackageFilter;

        public bool EncryptedPackageFilter
        {
            get { return _encryptedPackageFilter; }

            set
            {
                if (!Equals(_encryptedPackageFilter, value))
                {
                    _encryptedPackageFilter = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EncryptedPackageFilter)));
                }
            }
        }

        private bool _blockMapFilter = LinkFilterService.BlockMapFilter;

        public bool BlockMapFilter
        {
            get { return _blockMapFilter; }

            set
            {
                if (!Equals(_blockMapFilter, value))
                {
                    _blockMapFilter = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BlockMapFilter)));
                }
            }
        }

        private List<ComboBoxItemModel> QueryLinksModeList { get; } = [];

        private List<ComboBoxItemModel> AppLinkOpenModeList { get; } = [];

        private List<ComboBoxItemModel> InstallModeList { get; } = [];

        private ObservableCollection<StoreRegionModel> StoreRegionCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsStoreAndUpdatePage()
        {
            InitializeComponent();
            QueryLinksModeList.Add(new ComboBoxItemModel() { SelectedValue = QueryLinksModeService.QueryLinksModeList[0], DisplayMember = QueryLinksModeOfficialString });
            QueryLinksModeList.Add(new ComboBoxItemModel() { SelectedValue = QueryLinksModeService.QueryLinksModeList[1], DisplayMember = QueryLinksModeThirdPartyString });
            QueryLinksMode = QueryLinksModeList.Find(item => string.Equals(Convert.ToString(item.SelectedValue), QueryLinksModeService.QueryLinksMode, StringComparison.OrdinalIgnoreCase));

            AppLinkOpenModeList.Add(new ComboBoxItemModel() { SelectedValue = AppLinkOpenModeService.AppLinkOpenModeList[0], DisplayMember = AppLinkOpenModeBuiltInAppString });
            AppLinkOpenModeList.Add(new ComboBoxItemModel() { SelectedValue = AppLinkOpenModeService.AppLinkOpenModeList[1], DisplayMember = AppLinkOpenModeSystemBrowserString });
            AppLinkOpenMode = AppLinkOpenModeList.Find(item => string.Equals(Convert.ToString(item.SelectedValue), AppLinkOpenModeService.AppLinkOpenMode, StringComparison.OrdinalIgnoreCase));

            InstallModeList.Add(new ComboBoxItemModel() { SelectedValue = InstallModeService.InstallModeList[0], DisplayMember = InstallModeAppInstallString });
            InstallModeList.Add(new ComboBoxItemModel() { SelectedValue = InstallModeService.InstallModeList[1], DisplayMember = InstallModeCodeInstallString });
            InstallMode = InstallModeList.Find(item => string.Equals(Convert.ToString(item.SelectedValue), InstallModeService.InstallMode, StringComparison.OrdinalIgnoreCase));

            foreach (GeographicRegion geographicRegionItem in StoreRegionService.StoreRegionList)
            {
                StoreRegionCollection.Add(new StoreRegionModel()
                {
                    DisplayMember = geographicRegionItem.DisplayName,
                    CodeTwoLetter = geographicRegionItem.CodeTwoLetter,
                    GeographicRegion = geographicRegionItem
                });
            }

            foreach (StoreRegionModel storeRegion in StoreRegionCollection)
            {
                if (string.Equals(StoreRegionService.StoreRegion.CodeTwoLetter, storeRegion.CodeTwoLetter))
                {
                    StoreRegion = storeRegion;
                    break;
                }
            }

            GlobalNotificationService.ApplicationExit += OnApplicationExit;
            StoreRegionService.PropertyChanged += OnServicePropertyChanged;
        }

        #region 第一部分：设置商店与更新页面——挂载的事件

        /// <summary>
        /// 选择查询链接方式
        /// </summary>
        private void OnQueryLinksModeSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (args.AddedItems.Count > 0 && args.AddedItems[0] is ComboBoxItemModel queryLinksMode && !Equals(QueryLinksMode, queryLinksMode))
            {
                QueryLinksMode = queryLinksMode;
                QueryLinksModeService.SetQueryLinksMode(Convert.ToString(QueryLinksMode.SelectedValue));
                QueryLinksMode = QueryLinksModeList.Find(item => string.Equals(Convert.ToString(item.SelectedValue), QueryLinksModeService.QueryLinksMode, StringComparison.OrdinalIgnoreCase));
            }
        }

        /// <summary>
        /// 选择应用链接打开方式
        /// </summary>
        private void OnAppLinkOpenModeSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (args.AddedItems.Count > 0 && args.AddedItems[0] is ComboBoxItemModel appLinkOpenMode && !Equals(AppLinkOpenMode, appLinkOpenMode))
            {
                AppLinkOpenMode = appLinkOpenMode;
                AppLinkOpenModeService.SetAppLinkOpenMode(Convert.ToString(AppLinkOpenMode.SelectedValue));
                AppLinkOpenMode = AppLinkOpenModeList.Find(item => string.Equals(Convert.ToString(item.SelectedValue), AppLinkOpenModeService.AppLinkOpenMode, StringComparison.OrdinalIgnoreCase));
            }
        }

        /// <summary>
        /// 应用安装方式设置
        /// </summary>
        private void OnInstallModeSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (args.AddedItems.Count > 0 && args.AddedItems[0] is ComboBoxItemModel installMode && !Equals(InstallMode, installMode))
            {
                InstallMode = installMode;
                InstallModeService.SetInstallMode(Convert.ToString(InstallMode.SelectedValue));
                InstallMode = InstallModeList.Find(item => string.Equals(Convert.ToString(item.SelectedValue), InstallModeService.InstallMode, StringComparison.OrdinalIgnoreCase));
            }
        }

        /// <summary>
        /// 设置是否取消自动更新应用
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ToggleSwitch))]
        private void OnCancelAutoUpdateToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch && !Equals(CancelAutoUpdate, toggleSwitch.IsOn))
            {
                CancelAutoUpdate = toggleSwitch.IsOn;
                CancelAutoUpdateService.SetCancelAutoUpdate(toggleSwitch.IsOn);
                CancelAutoUpdate = CancelAutoUpdateService.CancelAutoUpdate;
            }
        }

        /// <summary>
        /// 重置商店缓存
        /// </summary>
        private void OnResetStoreCacheClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                Shell32Library.ShellExecute(nint.Zero, "open", "wsreset.exe", string.Empty, null, WindowShowStyle.SW_SHOWNORMAL);
            });
        }

        /// <summary>
        /// 打开系统区域设置
        /// </summary>
        private void OnSystemRegionSettingsClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new Uri("ms-settings:regionformatting"));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 设置是否使用系统默认区域
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ToggleSwitch))]
        private void OnUseSystemRegionToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch && !Equals(UseSystemRegion, toggleSwitch.IsOn))
            {
                UseSystemRegion = toggleSwitch.IsOn;
                StoreRegionService.SetUseSystemRegion(toggleSwitch.IsOn);
                UseSystemRegion = StoreRegionService.UseSystemRegion;

                if (UseSystemRegion)
                {
                    foreach (StoreRegionModel storeRegion in StoreRegionCollection)
                    {
                        if (Equals(storeRegion.CodeTwoLetter, StoreRegionService.DefaultStoreRegion.CodeTwoLetter))
                        {
                            StoreRegion = storeRegion;
                            break;
                        }
                    }

                    StoreRegionService.SetRegion(StoreRegion.GeographicRegion);
                }
            }
        }

        /// <summary>
        /// 区域设置菜单打开时自动定位到选中项
        /// </summary>
        private void OnStoreRegionSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (args.AddedItems.Count > 0 && args.AddedItems[0] is StoreRegionModel storeRegion && !Equals(StoreRegion, storeRegion))
            {
                StoreRegion = storeRegion;
                StoreRegionService.SetRegion(StoreRegion.GeographicRegion);
                foreach (StoreRegionModel storeRegionItem in StoreRegionCollection)
                {
                    if (string.Equals(StoreRegionService.StoreRegion.CodeTwoLetter, storeRegionItem.CodeTwoLetter))
                    {
                        StoreRegion = storeRegionItem;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 设置是否过滤加密包文件
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ToggleSwitch))]
        private void OnEncryptedPackageToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch && !Equals(EncryptedPackageFilter, toggleSwitch.IsOn))
            {
                EncryptedPackageFilter = toggleSwitch.IsOn;
                LinkFilterService.SetEncryptedPackageFilter(toggleSwitch.IsOn);
                EncryptedPackageFilter = LinkFilterService.EncryptedPackageFilter;
            }
        }

        /// <summary>
        /// 设置是否过滤包块映射文件
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ToggleSwitch))]
        private void OnBlockMapToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch && !Equals(BlockMapFilter, toggleSwitch.IsOn))
            {
                BlockMapFilter = toggleSwitch.IsOn;
                LinkFilterService.SetBlockMapFilter(toggleSwitch.IsOn);
                BlockMapFilter = LinkFilterService.BlockMapFilter;
            }
        }

        #endregion 第一部分：设置商店与更新页面——挂载的事件

        #region 第二部分：设置商店与更新页面——自定义事件

        /// <summary>
        /// 应用程序退出时触发的事件
        /// </summary>
        private void OnApplicationExit()
        {
            try
            {
                GlobalNotificationService.ApplicationExit -= OnApplicationExit;
                StoreRegionService.PropertyChanged -= OnServicePropertyChanged;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(SettingsStoreAndUpdatePage), nameof(OnApplicationExit), 1, e);
            }
        }

        /// <summary>
        /// 设置选项发生变化时触发的事件
        /// </summary>
        private void OnServicePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (string.Equals(args.PropertyName, nameof(StoreRegionService.StoreRegion)))
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    if (!string.Equals(CurrentCountryOrRegion.CodeTwoLetter, StoreRegionService.DefaultStoreRegion.CodeTwoLetter))
                    {
                        CurrentCountryOrRegion = StoreRegionService.DefaultStoreRegion;
                    }

                    if (UseSystemRegion)
                    {
                        foreach (StoreRegionModel storeRegion in StoreRegionCollection)
                        {
                            if (Equals(storeRegion.CodeTwoLetter, StoreRegionService.DefaultStoreRegion.CodeTwoLetter))
                            {
                                StoreRegion = storeRegion;
                                break;
                            }
                        }
                    }
                });
            }
        }

        #endregion 第二部分：设置商店与更新页面——自定义事件
    }
}
