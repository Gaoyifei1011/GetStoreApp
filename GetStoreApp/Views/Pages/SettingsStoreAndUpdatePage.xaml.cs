using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Settings;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.TeachingTips;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.Windows.Storage.Pickers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;
using Windows.Globalization;
using Windows.Storage;
using Windows.System;

// 抑制 CS8305，IDE0060 警告
#pragma warning disable CS8305,IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 设置商店与更新页面
    /// </summary>
    public sealed partial class SettingsStoreAndUpdatePage : Page, INotifyPropertyChanged
    {
        private KeyValuePair<string, string> _queryLinksMode = QueryLinksModeService.QueryLinksMode;

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

        private List<KeyValuePair<string, string>> QueryLinksModeList { get; } = QueryLinksModeService.QueryLinksModeList;

        private List<KeyValuePair<string, string>> InstallModeList { get; } = InstallModeService.InstallModeList;

        private List<KeyValuePair<string, string>> DoEngineModeList { get; } = DownloadOptionsService.DoEngineModeList;

        private ObservableCollection<StoreRegionModel> StoreRegionCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsStoreAndUpdatePage()
        {
            InitializeComponent();

            foreach (GeographicRegion geographicItem in StoreRegionService.StoreRegionList)
            {
                if (Equals(StoreRegionService.StoreRegion.CodeTwoLetter, geographicItem.CodeTwoLetter))
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
                    if (Equals(storeRegion.StoreRegionInfo.CodeTwoLetter, storeRegionItem.StoreRegionInfo.CodeTwoLetter))
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
                QueryLinksModeService.SetQueryLinksMode(QueryLinksMode);
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

                    foreach (StoreRegionModel item in StoreRegionCollection)
                    {
                        item.IsChecked = false;
                        if (Equals(StoreRegion.CodeTwoLetter, item.StoreRegionInfo.CodeTwoLetter))
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
        private void OnOpenDeliveryOptimizationClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new Uri("ms-settings:delivery-optimization"));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 下载引擎说明
        /// </summary>
        private void OnLearnDoEngineClicked(object sender, RoutedEventArgs args)
        {
            if (MainWindow.Current.GetFrameContent() is SettingsPage settingsPage)
            {
                settingsPage.ShowSettingsInstruction();
            }
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

        #endregion 第二部分：设置商店与更新页面——挂载的事件

        #region 第三部分：设置商店与更新页面——自定义事件

        /// <summary>
        /// 应用程序退出时触发的事件
        /// </summary>
        private void OnApplicationExit(object sender, EventArgs args)
        {
            try
            {
                GlobalNotificationService.ApplicationExit -= OnApplicationExit;
                StoreRegionService.PropertyChanged -= OnServicePropertyChanged;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Unregister application exit event failed", e);
            }
        }

        /// <summary>
        /// 设置选项发生变化时触发的事件
        /// </summary>
        private void OnServicePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (Equals(args.PropertyName, nameof(StoreRegionService.StoreRegion)))
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    if (!Equals(CurrentCountryOrRegion.CodeTwoLetter, StoreRegionService.DefaultStoreRegion.CodeTwoLetter))
                    {
                        CurrentCountryOrRegion = StoreRegionService.DefaultStoreRegion;
                    }

                    if (UseSystemRegionValue)
                    {
                        StoreRegion = StoreRegionService.DefaultStoreRegion;

                        foreach (StoreRegionModel item in StoreRegionCollection)
                        {
                            item.IsChecked = false;
                            if (Equals(StoreRegion.CodeTwoLetter, item.StoreRegionInfo.CodeTwoLetter))
                            {
                                StoreRegion = item.StoreRegionInfo;
                                item.IsChecked = true;
                            }
                        }
                    }
                });
            }
        }

        #endregion 第三部分：设置商店与更新页面——自定义事件
    }
}
