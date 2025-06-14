using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Settings;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;
using Windows.Globalization;
using Windows.System;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 设置商店与更新页面
    /// </summary>
    public sealed partial class SettingsStoreAndUpdatePage : Page, INotifyPropertyChanged
    {
        private readonly string QueryLinksModeOfficialString = ResourceService.GetLocalized("SettingsStoreAndUpdate/QueryLinksModeOfficial");
        private readonly string QueryLinksModeThirdPartyString = ResourceService.GetLocalized("SettingsStoreAndUpdate/QueryLinksModeThirdParty");
        private readonly string InstallModeAppInstallString = ResourceService.GetLocalized("SettingsStoreAndUpdate/InstallModeAppInstall");
        private readonly string InstallModeCodeInstallString = ResourceService.GetLocalized("SettingsStoreAndUpdate/InstallModeCodeInstall");

        private KeyValuePair<string, string> _queryLinksMode;

        public KeyValuePair<string, string> QueryLinksMode
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

        private KeyValuePair<string, string> _installMode;

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

        private List<KeyValuePair<string, string>> QueryLinksModeList { get; } = [];

        private List<KeyValuePair<string, string>> InstallModeList { get; } = [];

        private ObservableCollection<StoreRegionModel> StoreRegionCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsStoreAndUpdatePage()
        {
            InitializeComponent();
            QueryLinksModeList.Add(KeyValuePair.Create(QueryLinksModeService.QueryLinksModeList[0], QueryLinksModeOfficialString));
            QueryLinksModeList.Add(KeyValuePair.Create(QueryLinksModeService.QueryLinksModeList[1], QueryLinksModeThirdPartyString));
            QueryLinksMode = QueryLinksModeList.Find(item => string.Equals(item.Key, QueryLinksModeService.QueryLinksMode, StringComparison.OrdinalIgnoreCase));

            InstallModeList.Add(KeyValuePair.Create(InstallModeService.InstallModeList[0], InstallModeAppInstallString));
            InstallModeList.Add(KeyValuePair.Create(InstallModeService.InstallModeList[1], InstallModeCodeInstallString));
            InstallMode = InstallModeList.Find(item => string.Equals(item.Key, InstallModeService.InstallMode, StringComparison.OrdinalIgnoreCase));

            foreach (GeographicRegion geographicRegionItem in StoreRegionService.StoreRegionList)
            {
                if (string.Equals(StoreRegionService.StoreRegion.CodeTwoLetter, geographicRegionItem.CodeTwoLetter))
                {
                    StoreRegion = geographicRegionItem;
                    StoreRegionCollection.Add(new StoreRegionModel()
                    {
                        StoreRegionInfo = geographicRegionItem,
                        IsChecked = true
                    });
                }
                else
                {
                    StoreRegionCollection.Add(new StoreRegionModel()
                    {
                        StoreRegionInfo = geographicRegionItem,
                        IsChecked = false
                    });
                }
            }

            GlobalNotificationService.ApplicationExit += OnApplicationExit;
            StoreRegionService.PropertyChanged += OnServicePropertyChanged;
        }

        #region 第一部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 修改商店区域
        /// </summary>
        private void OnStoreRegionExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (StoreRegionFlyout.IsOpen)
            {
                StoreRegionFlyout.Hide();
            }

            if (args.Parameter is StoreRegionModel storeRegion)
            {
                foreach (StoreRegionModel storeRegionItem in StoreRegionCollection)
                {
                    storeRegionItem.IsChecked = false;
                    if (string.Equals(storeRegion.StoreRegionInfo.CodeTwoLetter, storeRegionItem.StoreRegionInfo.CodeTwoLetter))
                    {
                        StoreRegion = storeRegionItem.StoreRegionInfo;
                        storeRegionItem.IsChecked = true;
                    }
                }

                StoreRegionService.SetRegion(StoreRegion);
            }
        }

        #endregion 第一部分：XamlUICommand 命令调用时挂载的事件

        #region 第二部分：设置商店与更新页面——挂载的事件

        /// <summary>
        /// 选择查询链接方式
        /// </summary>
        private void OnQueryLinksModeSelectClicked(object sender, RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem radioMenuFlyoutItem && radioMenuFlyoutItem.Tag is string tag)
            {
                QueryLinksMode = QueryLinksModeList[Convert.ToInt32(tag)];
                QueryLinksModeService.SetQueryLinksMode(QueryLinksMode.Key);
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
                InstallModeService.SetInstallMode(InstallMode.Key);
            }
        }

        /// <summary>
        /// 设置是否取消自动更新应用
        /// </summary>
        private void OnCancelAutoUpdateToggled(object sender, RoutedEventArgs args)
        {
            if (sender is ToggleSwitch toggleSwitch)
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
            if (sender is ToggleSwitch toggleSwitch)
            {
                StoreRegionService.SetUseSystemRegionValue(toggleSwitch.IsOn);
                UseSystemRegionValue = toggleSwitch.IsOn;

                if (UseSystemRegionValue)
                {
                    StoreRegion = StoreRegionService.DefaultStoreRegion;
                    StoreRegionService.SetRegion(StoreRegion);

                    foreach (StoreRegionModel storeRegionItem in StoreRegionCollection)
                    {
                        storeRegionItem.IsChecked = false;
                        if (string.Equals(StoreRegion.CodeTwoLetter, storeRegionItem.StoreRegionInfo.CodeTwoLetter))
                        {
                            StoreRegion = storeRegionItem.StoreRegionInfo;
                            storeRegionItem.IsChecked = true;
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
                    StoreRegionListView.ScrollIntoView(StoreRegionCollection[index]);
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

        #endregion 第二部分：设置商店与更新页面——挂载的事件

        #region 第三部分：设置商店与更新页面——自定义事件

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
                        StoreRegion = StoreRegionService.DefaultStoreRegion;

                        foreach (StoreRegionModel storeRegionItem in StoreRegionCollection)
                        {
                            storeRegionItem.IsChecked = false;
                            if (string.Equals(StoreRegion.CodeTwoLetter, storeRegionItem.StoreRegionInfo.CodeTwoLetter))
                            {
                                StoreRegion = storeRegionItem.StoreRegionInfo;
                                storeRegionItem.IsChecked = true;
                            }
                        }
                    }
                });
            }
        }

        #endregion 第三部分：设置商店与更新页面——自定义事件
    }
}
