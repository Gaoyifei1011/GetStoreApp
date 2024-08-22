using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Controls.Extensions;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Settings;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.Settings;
using GetStoreApp.UI.TeachingTips;
using GetStoreApp.Views.Windows;
using GetStoreApp.WindowsAPI.ComTypes;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using WinRT.Interop;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 设置页面
    /// </summary>
    public sealed partial class SettingsPage : Page, INotifyPropertyChanged
    {
        private AppNaviagtionArgs settingNavigationArgs = AppNaviagtionArgs.None;

        private bool IsOfficialVersionExisted { get; } = WinGetService.IsOfficialVersionExisted;

        private bool IsDevVersionExisted { get; } = WinGetService.IsDevVersionExisted;

        private DictionaryEntry _theme = ThemeService.AppTheme;

        public DictionaryEntry Theme
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

        private DictionaryEntry _backdrop = BackdropService.AppBackdrop;

        public DictionaryEntry Backdrop
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

        private DictionaryEntry _appLanguage;

        public DictionaryEntry AppLanguage
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

        private bool _notification = NotificationService.AppNotification;

        public bool Notification
        {
            get { return _notification; }

            set
            {
                if (!Equals(_notification, value))
                {
                    _notification = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Notification)));
                }
            }
        }

        private DictionaryEntry _webKernelItem = WebKernelService.WebKernel;

        public DictionaryEntry WebKernelItem
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

        private DictionaryEntry _queryLinksModeItem = QueryLinksModeService.QueryLinksMode;

        public DictionaryEntry QueryLinksModeItem
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

        private bool _useDevVersion = WinGetConfigService.UseDevVersion;

        public bool UseDevVersion
        {
            get { return _useDevVersion; }

            set
            {
                if (!Equals(_useDevVersion, value))
                {
                    _useDevVersion = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UseDevVersion)));
                }
            }
        }

        private DictionaryEntry _winGetInstallMode = WinGetConfigService.WinGetInstallMode;

        public DictionaryEntry WinGetInstallMode
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

        private DictionaryEntry _doEngineMode = DownloadOptionsService.DoEngineMode;

        public DictionaryEntry DoEngineMode
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

        private DictionaryEntry _installMode = InstallModeService.InstallMode;

        public DictionaryEntry InstallMode
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

        private List<DictionaryEntry> ThemeList { get; } = ThemeService.ThemeList;

        private List<DictionaryEntry> BackdropList { get; } = BackdropService.BackdropList;

        private List<DictionaryEntry> WebKernelList { get; } = WebKernelService.WebKernelList;

        private List<DictionaryEntry> QueryLinksModeList { get; } = QueryLinksModeService.QueryLinksModeList;

        private List<DictionaryEntry> WinGetInstallModeList { get; } = WinGetConfigService.WinGetInstallModeList;

        private List<DictionaryEntry> DoEngineModeList { get; } = DownloadOptionsService.DoEngineModeList;

        private List<DictionaryEntry> InstallModeList { get; } = InstallModeService.InstallModeList;

        private ObservableCollection<LanguageModel> LanguageCollection { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsPage()
        {
            InitializeComponent();

            foreach (DictionaryEntry languageItem in LanguageService.LanguageList)
            {
                if (LanguageService.AppLanguage.Value.Equals(languageItem.Value))
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
        }

        #region 第一部分：重写父类事件

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            settingNavigationArgs = args.Parameter is not null && Enum.TryParse(Convert.ToString(args.Parameter), out AppNaviagtionArgs appNaviagtionArgs) ? appNaviagtionArgs : AppNaviagtionArgs.None;
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：XamlUICommand 命令调用时挂载的事件

        /// <summary>
        /// 修改应用语言
        /// </summary>
        private async void OnLanguageExecuteRequested(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            if (LanguageFlyout.IsOpen)
            {
                LanguageFlyout.Hide();
            }

            LanguageModel languageItem = args.Parameter as LanguageModel;

            if (languageItem is not null)
            {
                foreach (LanguageModel item in LanguageCollection)
                {
                    item.IsChecked = false;
                    if (languageItem.LangaugeInfo.Value.Equals(item.LangaugeInfo.Value))
                    {
                        AppLanguage = item.LangaugeInfo;
                        item.IsChecked = true;
                    }
                }

                LanguageService.SetLanguage(AppLanguage);
                await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.LanguageChange));
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
                SettingsScroll.ChangeView(null, targetPosition.Y, null, true);
            }
            else
            {
                SettingsScroll.ChangeView(null, 0, null, true);
            }
        }

        /// <summary>
        /// 打开重启应用确认的窗口对话框
        /// </summary>
        private async void OnRestartAppsClicked(object sender, RoutedEventArgs args)
        {
            await ContentDialogHelper.ShowAsync(new RestartAppsDialog(), this);
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
            ToggleMenuFlyoutItem toggleMenuFlyoutItem = sender as ToggleMenuFlyoutItem;
            if (toggleMenuFlyoutItem.Tag is not null)
            {
                Theme = ThemeList[Convert.ToInt32(toggleMenuFlyoutItem.Tag)];
                ThemeService.SetTheme(Theme);
            }
        }

        /// <summary>
        /// 背景色修改设置
        /// </summary>
        private void OnBackdropSelectClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem toggleMenuFlyoutItem = sender as ToggleMenuFlyoutItem;
            if (toggleMenuFlyoutItem.Tag is not null)
            {
                Backdrop = BackdropList[Convert.ToInt32(toggleMenuFlyoutItem.Tag)];
                BackdropService.SetBackdrop(Backdrop);
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
            ToggleMenuFlyoutItem toggleMenuFlyoutItem = sender as ToggleMenuFlyoutItem;
            if (toggleMenuFlyoutItem.Tag is not null)
            {
                WebKernelItem = WebKernelList[Convert.ToInt32(toggleMenuFlyoutItem.Tag)];
                WebKernelService.SetWebKernel(WebKernelItem);
            }
        }

        /// <summary>
        /// 选择查询链接方式
        /// </summary>
        private void OnQueryLinksModeSelectClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem toggleMenuFlyoutItem = sender as ToggleMenuFlyoutItem;
            if (toggleMenuFlyoutItem.Tag is not null)
            {
                QueryLinksModeItem = QueryLinksModeList[Convert.ToInt32(toggleMenuFlyoutItem.Tag)];
                QueryLinksModeService.SetQueryLinksMode(QueryLinksModeItem);
            }
        }

        /// <summary>
        /// WinGet 程序包安装方式设置
        /// </summary>
        private void OnWinGetInstallModeSelectClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem toggleMenuFlyoutItem = sender as ToggleMenuFlyoutItem;
            if (toggleMenuFlyoutItem.Tag is not null)
            {
                WinGetInstallMode = WinGetInstallModeList[Convert.ToInt32(toggleMenuFlyoutItem.Tag)];
                WinGetConfigService.SetWinGetInstallMode(WinGetInstallMode);
            }
        }

        /// <summary>
        /// 打开 WinGet 程序包设置
        /// </summary>
        private void OnOpenWinGetSettingsClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                ProcessHelper.StartProcess("winget.exe", "settings", out _);
            });
        }

        /// <summary>
        /// 打开文件存放目录
        /// </summary>
        private async void OnOpenFolderClicked(object sender, RoutedEventArgs args)
        {
            await DownloadOptionsService.OpenFolderAsync(DownloadFolder);
        }

        /// <summary>
        /// 修改下载目录
        /// </summary>
        private async void OnChangeFolderClicked(object sender, RoutedEventArgs args)
        {
            MenuFlyoutItem menuFlyoutItem = sender as MenuFlyoutItem;
            if (menuFlyoutItem.Tag is not null)
            {
                switch ((string)menuFlyoutItem.Tag)
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
                            // 先使用 FolderPicker，FolderPicker 打开失败，再尝试使用 IFileDialog COM 接口选择文件夹，否则提示自定义文件夹失败
                            bool result = false;

                            // 使用 FolderPicker
                            try
                            {
                                FolderPicker folderPicker = new();
                                InitializeWithWindow.Initialize(folderPicker, (IntPtr)MainWindow.Current.AppWindow.Id.Value);
                                folderPicker.SuggestedStartLocation = PickerLocationId.Downloads;

                                StorageFolder downloadFolder = await folderPicker.PickSingleFolderAsync();

                                if (downloadFolder is not null)
                                {
                                    DownloadFolder = downloadFolder;
                                    DownloadOptionsService.SetFolder(downloadFolder);
                                }
                                result = true;
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(LoggingLevel.Error, "Open folderPicker failed", e);
                            }

                            // 使用 IFileDialog
                            if (!result)
                            {
                                try
                                {
                                    OpenFolderDialog openFolderDialog = new(MainWindow.Current.AppWindow.Id)
                                    {
                                        Description = ResourceService.GetLocalized("Settings/SelectFolder"),
                                        RootFolder = DownloadOptionsService.DownloadFolder.Path,
                                    };

                                    if (openFolderDialog.ShowDialog())
                                    {
                                        DownloadFolder = await StorageFolder.GetFolderFromPathAsync(openFolderDialog.SelectedPath);
                                        DownloadOptionsService.SetFolder(DownloadFolder);
                                    }

                                    result = true;
                                    openFolderDialog.Dispose();
                                }
                                catch (Exception e)
                                {
                                    LogService.WriteLog(LoggingLevel.Error, "OpenFolderDialog(IFileOpenDialog) initialize failed.", e);
                                }
                            }

                            // 选取文件夹失败，显示提示
                            if (!result)
                            {
                                await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.FolderPicker));
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
            ToggleMenuFlyoutItem toggleMenuFlyoutItem = sender as ToggleMenuFlyoutItem;
            if (toggleMenuFlyoutItem.Tag is not null)
            {
                DoEngineMode = DoEngineModeList[Convert.ToInt32(toggleMenuFlyoutItem.Tag)];
                DownloadOptionsService.SetDoEngineMode(DoEngineMode);
            }
        }

        /// <summary>
        /// 应用安装方式设置
        /// </summary>
        private void OnInstallModeSelectClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem toggleMenuFlyoutItem = sender as ToggleMenuFlyoutItem;
            if (toggleMenuFlyoutItem.Tag is not null)
            {
                InstallMode = InstallModeList[Convert.ToInt32(toggleMenuFlyoutItem.Tag)];
                InstallModeService.SetInstallMode(InstallMode);
            }
        }

        /// <summary>
        /// 清理应用内使用的所有痕迹
        /// </summary>
        private async void OnTraceCleanupClicked(object sender, RoutedEventArgs args)
        {
            await ContentDialogHelper.ShowAsync(new TraceCleanupPromptDialog(), this);
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
            bool result = LogService.ClearLog();
            await TeachingTipHelper.ShowAsync(new OperationResultTip(OperationKind.LogClean, result));
        }

        /// <summary>
        /// 开关按钮切换时修改相应设置
        /// </summary>
        private void OnAlwaysShowBackdropToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch is not null)
            {
                AlwaysShowBackdropService.SetAlwaysShowBackdrop(toggleSwitch.IsOn);
                AlwaysShowBackdropValue = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 语言设置菜单打开时自动定位到选中项
        /// </summary>
        private void OnOpened(object sender, object args)
        {
            foreach (LanguageModel languageItem in LanguageCollection)
            {
                if (languageItem.IsChecked)
                {
                    LanguageListView.ScrollIntoView(languageItem);
                    break;
                }
            }
        }

        /// <summary>
        /// 是否开启应用窗口置顶
        /// </summary>
        private void OnTopMostToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch is not null)
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
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch is not null)
            {
                NotificationService.SetNotification(toggleSwitch.IsOn);
                Notification = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 设置是否过滤加密包文件
        /// </summary>
        private void OnEncryptedPackageToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch is not null)
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
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch is not null)
            {
                LinkFilterService.SetBlockMapFilterValue(toggleSwitch.IsOn);
                BlockMapFilterValue = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 当两个版本共存时，设置是否优先使用开发版本
        /// </summary>
        private void OnWinGetConfigToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch is not null)
            {
                WinGetConfigService.SetUseDevVersion(toggleSwitch.IsOn);
                UseDevVersion = toggleSwitch.IsOn;
            }
        }

        /// <summary>
        /// 当应用未启用背景色设置时，自动关闭始终显示背景色设置
        /// </summary>
        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch is not null)
            {
                AlwaysShowBackdropService.SetAlwaysShowBackdrop(false);
                AlwaysShowBackdropValue = false;
            }
        }

        #endregion 第三部分：设置页面——挂载的事件

        private string LocalizeDisplayNumber(DictionaryEntry selectedBackdrop)
        {
            int index = BackdropList.FindIndex(item => item.Value.Equals(selectedBackdrop.Value));

            if (index is 0)
            {
                return selectedBackdrop.Key.ToString();
            }
            else if (index is 1 or 2)
            {
                return ResourceService.GetLocalized("Settings/Mica") + " " + selectedBackdrop.Key.ToString();
            }
            else if (index is 3 or 4 or 5)
            {
                return ResourceService.GetLocalized("Settings/DesktopAcrylic") + " " + selectedBackdrop.Key.ToString();
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
