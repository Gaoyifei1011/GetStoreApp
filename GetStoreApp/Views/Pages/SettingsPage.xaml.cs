using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Controls.Extensions;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.Settings;
using GetStoreApp.UI.TeachingTips;
using GetStoreApp.Views.Windows;
using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using WinRT.Interop;

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 设置页面
    /// </summary>
    public sealed partial class SettingsPage : Page, INotifyPropertyChanged
    {
        private AppNaviagtionArgs settingNavigationArgs = AppNaviagtionArgs.None;

        private bool CanUseMicaBackdrop { get; } = InfoHelper.SystemVersion.Build >= 22000;

        private bool IsOfficialVersionExisted { get; } = WinGetService.IsOfficialVersionExisted;

        private bool IsDevVersionExisted { get; } = WinGetService.IsDevVersionExisted;

        private DictionaryEntry _theme = ThemeService.AppTheme;

        public DictionaryEntry Theme
        {
            get { return _theme; }

            set
            {
                _theme = value;
                OnPropertyChanged();
            }
        }

        private DictionaryEntry _backdrop = BackdropService.AppBackdrop;

        public DictionaryEntry Backdrop
        {
            get { return _backdrop; }

            set
            {
                _backdrop = value;
                OnPropertyChanged();
            }
        }

        private bool _alwaysShowBackdropValue = AlwaysShowBackdropService.AlwaysShowBackdropValue;

        public bool AlwaysShowBackdropValue
        {
            get { return _alwaysShowBackdropValue; }

            set
            {
                _alwaysShowBackdropValue = value;
                OnPropertyChanged();
            }
        }

        private DictionaryEntry _appLanguage = LanguageService.AppLanguage;

        public DictionaryEntry AppLanguage
        {
            get { return _appLanguage; }

            set
            {
                _appLanguage = value;
                OnPropertyChanged();
            }
        }

        private bool _topMostValue = TopMostService.TopMostValue;

        public bool TopMostValue
        {
            get { return _topMostValue; }

            set
            {
                _topMostValue = value;
                OnPropertyChanged();
            }
        }

        private bool _notification = NotificationService.AppNotification;

        public bool Notification
        {
            get { return _notification; }

            set
            {
                _notification = value;
                OnPropertyChanged();
            }
        }

        private DictionaryEntry _webKernelItem = WebKernelService.WebKernel;

        public DictionaryEntry WebKernelItem
        {
            get { return _webKernelItem; }

            set
            {
                _webKernelItem = value;
                OnPropertyChanged();
            }
        }

        private bool _encryptedPackageFilterValue = LinkFilterService.EncryptedPackageFilterValue;

        public bool EncryptedPackageFilterValue
        {
            get { return _encryptedPackageFilterValue; }

            set
            {
                _encryptedPackageFilterValue = value;
                OnPropertyChanged();
            }
        }

        private bool _blockMapFilterValue = LinkFilterService.BlockMapFilterValue;

        public bool BlockMapFilterValue
        {
            get { return _blockMapFilterValue; }

            set
            {
                _blockMapFilterValue = value;
                OnPropertyChanged();
            }
        }

        private bool _useDevVersion = WinGetConfigService.UseDevVersion;

        public bool UseDevVersion
        {
            get { return _useDevVersion; }

            set
            {
                _useDevVersion = value;
                OnPropertyChanged();
            }
        }

        private DictionaryEntry _winGetInstallMode = WinGetConfigService.WinGetInstallMode;

        public DictionaryEntry WinGetInstallMode
        {
            get { return _winGetInstallMode; }

            set
            {
                _winGetInstallMode = value;
                OnPropertyChanged();
            }
        }

        private StorageFolder _downloadFolder = DownloadOptionsService.DownloadFolder;

        public StorageFolder DownloadFolder
        {
            get { return _downloadFolder; }

            set
            {
                _downloadFolder = value;
                OnPropertyChanged();
            }
        }

        private int _downloadItem = DownloadOptionsService.DownloadItem;

        public int DownloadItem
        {
            get { return _downloadItem; }

            set
            {
                _downloadItem = value;
                OnPropertyChanged();
            }
        }

        private DictionaryEntry _installMode = InstallModeService.InstallMode;

        public DictionaryEntry InstallMode
        {
            get { return _installMode; }

            set
            {
                _installMode = value;
                OnPropertyChanged();
            }
        }

        private List<DictionaryEntry> ThemeList { get; } = ThemeService.ThemeList;

        private List<DictionaryEntry> BackdropList { get; } = BackdropService.BackdropList;

        private List<DictionaryEntry> LanguageList { get; } = LanguageService.LanguageList;

        private List<DictionaryEntry> HistoryLiteNumList { get; } = WebKernelService.WebKernelList;

        private List<DictionaryEntry> WinGetInstallModeList { get; } = WinGetConfigService.WinGetInstallModeList;

        private List<DictionaryEntry> InstallModeList { get; } = InstallModeService.InstallModeList;

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsPage()
        {
            InitializeComponent();

            for (int index = 0; index < LanguageList.Count; index++)
            {
                DictionaryEntry languageItem = LanguageList[index];
                ToggleMenuFlyoutItem toggleMenuFlyoutItem = new ToggleMenuFlyoutItem()
                {
                    Text = languageItem.Key.ToString(),
                    Height = 32,
                    Padding = new Thickness(11, 0, 11, 0),
                    Tag = index
                };

                if (AppLanguage.Value.Equals(LanguageList[index].Value))
                {
                    toggleMenuFlyoutItem.IsChecked = true;
                }

                toggleMenuFlyoutItem.Click += (sender, args) =>
                {
                    foreach (MenuFlyoutItemBase menuFlyoutItemBase in LanguageFlyout.Items)
                    {
                        ToggleMenuFlyoutItem toggleMenuFlyoutItem = menuFlyoutItemBase as ToggleMenuFlyoutItem;
                        if (toggleMenuFlyoutItem is not null && toggleMenuFlyoutItem.IsChecked)
                        {
                            toggleMenuFlyoutItem.IsChecked = false;
                        }
                    }

                    int selectedIndex = Convert.ToInt32((sender as ToggleMenuFlyoutItem).Tag);
                    (LanguageFlyout.Items[selectedIndex] as ToggleMenuFlyoutItem).IsChecked = true;

                    if (AppLanguage.Value.ToString() != LanguageList[selectedIndex].Value.ToString())
                    {
                        AppLanguage = LanguageList[selectedIndex];
                        LanguageService.SetLanguage(AppLanguage);
                        TeachingTipHelper.Show(new LanguageChangeTip());
                    }
                };
                LanguageFlyout.Items.Add(toggleMenuFlyoutItem);
            }
        }

        #region 第一部分：重写父类事件

        /// <summary>
        /// 导航到该页面触发的事件
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            if (args.Parameter is not null)
            {
                settingNavigationArgs = (AppNaviagtionArgs)Enum.Parse(typeof(AppNaviagtionArgs), Convert.ToString(args.Parameter));
            }
            else
            {
                settingNavigationArgs = AppNaviagtionArgs.None;
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：设置页面——挂载的事件

        /// <summary>
        /// 页面加载完成后如果有具体的要求，将页面滚动到指定位置
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            double currentScrollPosition = SettingsScroll.VerticalOffset;
            Point currentPoint = new Point(0, (int)currentScrollPosition);

            if (settingNavigationArgs is AppNaviagtionArgs.DownloadOptions)
            {
                Point targetPosition = DownloadOptions.TransformToVisual(SettingsScroll).TransformPoint(currentPoint);
                SettingsScroll.ChangeView(null, targetPosition.Y, null);
            }
            else
            {
                SettingsScroll.ChangeView(null, 0, null);
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
                ThemeService.SetWindowTheme();
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
                BackdropService.SetAppBackdrop();
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
                WebKernelItem = HistoryLiteNumList[Convert.ToInt32(toggleMenuFlyoutItem.Tag)];
                WebKernelService.SetWebKernel(WebKernelItem);
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
                            // 先使用 FolderPicker，FolderPicker 打开失败，再尝试使用 SHBrowseForFolder 函数选择文件夹，否则提示自定义文件夹失败
                            bool result = false;

                            // 使用 FolderPicker
                            try
                            {
                                FolderPicker folderPicker = new FolderPicker();
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

                            // 使用 SHBrowseForFolder
                            if (!result)
                            {
                                string selectedPath = string.Empty;
                                unsafe
                                {
                                    char* pszPath = stackalloc char[Kernel32Library.MAX_PATH + 1];

                                    try
                                    {
                                        BROWSEINFO browseInfo = new BROWSEINFO();
                                        browseInfo.hwndOwner = (IntPtr)MainWindow.Current.AppWindow.Id.Value;
                                        browseInfo.lpszTitle = Marshal.StringToHGlobalUni(ResourceService.GetLocalized("Settings/SelectFolder"));
                                        browseInfo.ulFlags = BROWSEINFOFLAGS.BIF_RETURNONLYFSDIRS | BROWSEINFOFLAGS.BIF_NEWDIALOGSTYLE;
                                        IntPtr resultPtr = Shell32Library.SHBrowseForFolder(ref browseInfo);

                                        if (resultPtr != IntPtr.Zero)
                                        {
                                            if (Shell32Library.SHGetPathFromIDList(resultPtr, pszPath))
                                            {
                                                selectedPath = new string(pszPath);
                                            }
                                            Marshal.FreeCoTaskMem(resultPtr);
                                        }
                                        result = true;
                                    }
                                    catch (Exception e)
                                    {
                                        LogService.WriteLog(LoggingLevel.Error, "Open SHBrowseForFolder failed", e);
                                        TeachingTipHelper.Show(new FolderPickerTip());
                                    }
                                };

                                try
                                {
                                    if (result is true && !string.IsNullOrEmpty(selectedPath))
                                    {
                                        DownloadFolder = await StorageFolder.GetFolderFromPathAsync(selectedPath);
                                        DownloadOptionsService.SetFolder(DownloadFolder);
                                    }
                                }
                                catch (Exception e)
                                {
                                    LogService.WriteLog(LoggingLevel.Error, "Failed to read the directory obtained by the SHBrowseForFolder function", e);
                                    result = false;
                                }
                            }

                            // 选取文件夹失败，显示提示
                            if (!result)
                            {
                                TeachingTipHelper.Show(new FolderPickerTip());
                            }

                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 修改同时下载文件数
        /// </summary>
        private void OnDownloadItemSelectClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem toggleMenuFlyoutItem = sender as ToggleMenuFlyoutItem;
            if (toggleMenuFlyoutItem.Tag is not null)
            {
                DownloadItem = Convert.ToInt32(toggleMenuFlyoutItem.Tag);
                DownloadOptionsService.SetItem(DownloadItem);
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
        /// 实验功能设置
        /// </summary>
        private async void OnConfigClicked(object sender, RoutedEventArgs args)
        {
            await ContentDialogHelper.ShowAsync(new ExperimentalConfigDialog(), this);
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
        private void OnClearClicked(object sender, RoutedEventArgs args)
        {
            bool result = LogService.ClearLog();
            TeachingTipHelper.Show(new LogCleanTip(result));
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
        /// 是否开启应用窗口置顶
        /// </summary>
        private void OnTopMostToggled(object sender, RoutedEventArgs args)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch is not null)
            {
                TopMostService.SetTopMostValue(toggleSwitch.IsOn);
                TopMostService.SetAppTopMost();
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

        #endregion 第二部分：设置页面——挂载的事件

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 判断两个版本是否共同存在
        /// </summary>
        private bool IsBothVersionExisted(bool isOfficialVersionExisted, bool isDevVersionExisted)
        {
            return isOfficialVersionExisted && isDevVersionExisted;
        }

        /// <summary>
        /// 判断 WinGet 程序包是否存在
        /// </summary>
        private bool IsWinGetExisted(bool isOfficialVersionExisted, bool isDevVersionExisted)
        {
            return isOfficialVersionExisted || isDevVersionExisted;
        }

        private string LocalizeDisplayNumber(DictionaryEntry selectedBackdrop)
        {
            int index = BackdropList.FindIndex(item => item.Value.Equals(selectedBackdrop.Value));

            if (index is 0)
            {
                return selectedBackdrop.Key.ToString();
            }
            else if (index is 1 || index is 2)
            {
                return ResourceService.GetLocalized("Settings/Mica") + " " + selectedBackdrop.Key.ToString();
            }
            else if (index is 3 || index is 4 || index is 5)
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
