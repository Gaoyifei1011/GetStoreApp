using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Settings;
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

        private bool _cancelAutoUpdateValue = CancelAutoUpdateService.CancelAutoUpdateValue;

        public bool CancelAutoUpdateValue
        {
            get { return _cancelAutoUpdateValue; }

            set
            {
                if (!Equals(_cancelAutoUpdateValue, value))
                {
                    _cancelAutoUpdateValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CancelAutoUpdateValue)));
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
            }
        }

        /// <summary>
        /// 设置是否取消自动更新应用
        /// </summary>
        private void OnCancelAutoUpdateToggled(object sender, RoutedEventArgs args)
        {
            if (sender.As<ToggleSwitch>() is ToggleSwitch toggleSwitch)
            {
                CancelAutoUpdateService.SetCancelAutoUpdateValue(toggleSwitch.IsOn);
                CancelAutoUpdateValue = toggleSwitch.IsOn;
            }
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
        private void OnUseSystemRegionToggled(object sender, RoutedEventArgs args)
        {
            if (sender.As<ToggleSwitch>() is ToggleSwitch toggleSwitch)
            {
                StoreRegionService.SetUseSystemRegionValue(toggleSwitch.IsOn);
                UseSystemRegionValue = toggleSwitch.IsOn;

                if (UseSystemRegionValue)
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
            }
        }

        /// <summary>
        /// 设置是否过滤加密包文件
        /// </summary>
        private void OnEncryptedPackageToggled(object sender, RoutedEventArgs args)
        {
            if (sender.As<ToggleSwitch>() is ToggleSwitch toggleSwitch)
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
            if (sender.As<ToggleSwitch>() is ToggleSwitch toggleSwitch)
            {
                LinkFilterService.SetBlockMapFilterValue(toggleSwitch.IsOn);
                BlockMapFilterValue = toggleSwitch.IsOn;
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

                    if (UseSystemRegionValue)
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
