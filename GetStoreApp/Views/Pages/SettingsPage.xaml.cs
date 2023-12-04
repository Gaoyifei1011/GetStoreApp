using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Controls.Extensions;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Window;
using GetStoreApp.UI.Dialogs.Settings;
using GetStoreApp.UI.Notifications;
using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using GetStoreApp.WindowsAPI.PInvoke.User32;
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
        private bool CanUseMicaBackdrop = InfoHelper.SystemVersion.Build >= 22000;
        private bool IsOfficialVersionExisted = WinGetService.IsOfficialVersionExisted;
        private bool IsDevVersionExisted = WinGetService.IsDevVersionExisted;

        private AppNaviagtionArgs SettingNavigationArgs = AppNaviagtionArgs.None;

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

        private DictionaryEntry _downloadMode = DownloadOptionsService.DownloadMode;

        public DictionaryEntry DownloadMode
        {
            get { return _downloadMode; }

            set
            {
                _downloadMode = value;
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

        private List<DictionaryEntry> DownloadModeList { get; } = DownloadOptionsService.DownloadModeList;

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
                        new LanguageChangeNotification(this).Show();
                    }
                };
                LanguageFlyout.Items.Add(toggleMenuFlyoutItem);
            }
        }

        #region 第一部分：重写父类事件

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            if (args.Parameter is not null)
            {
                SettingNavigationArgs = (AppNaviagtionArgs)Enum.Parse(typeof(AppNaviagtionArgs), Convert.ToString(args.Parameter));
            }
            else
            {
                SettingNavigationArgs = AppNaviagtionArgs.None;
            }
        }

        #endregion 第一部分：重写父类事件

        #region 第二部分：设置页面——挂载的事件

        /// <summary>
        /// 页面加载完成后如果有具体的要求，将页面滚动到指定位置
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            double CurrentScrollPosition = SettingsScroll.VerticalOffset;
            Point CurrentPoint = new Point(0, (int)CurrentScrollPosition);

            if (SettingNavigationArgs is AppNaviagtionArgs.DownloadOptions)
            {
                Point TargetPosition = DownloadOptions.TransformToVisual(SettingsScroll).TransformPoint(CurrentPoint);
                SettingsScroll.ChangeView(null, TargetPosition.Y, null);
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
            NavigationService.NavigateTo(typeof(AboutPage), AppNaviagtionArgs.SettingsHelp);
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
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.Tag is not null)
            {
                Theme = ThemeList[Convert.ToInt32(item.Tag)];
                ThemeService.SetTheme(Theme);
                ThemeService.SetWindowTheme();
            }
        }

        /// <summary>
        /// 背景色修改设置
        /// </summary>
        private void OnBackdropSelectClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.Tag is not null)
            {
                Backdrop = BackdropList[Convert.ToInt32(item.Tag)];
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
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.Tag is not null)
            {
                WebKernelItem = HistoryLiteNumList[Convert.ToInt32(item.Tag)];
                WebKernelService.SetWebKernel(WebKernelItem);
            }
        }

        /// <summary>
        /// WinGet 程序包安装方式设置
        /// </summary>
        private void OnWinGetInstallModeSelectClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.Tag is not null)
            {
                WinGetInstallMode = WinGetInstallModeList[Convert.ToInt32(item.Tag)];
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
                Kernel32Library.GetStartupInfo(out STARTUPINFO WinGetSettingsStartupInfo);
                WinGetSettingsStartupInfo.lpReserved = IntPtr.Zero;
                WinGetSettingsStartupInfo.lpDesktop = IntPtr.Zero;
                WinGetSettingsStartupInfo.lpTitle = IntPtr.Zero;
                WinGetSettingsStartupInfo.dwX = 0;
                WinGetSettingsStartupInfo.dwY = 0;
                WinGetSettingsStartupInfo.dwXSize = 0;
                WinGetSettingsStartupInfo.dwYSize = 0;
                WinGetSettingsStartupInfo.dwXCountChars = 500;
                WinGetSettingsStartupInfo.dwYCountChars = 500;
                WinGetSettingsStartupInfo.dwFlags = STARTF.STARTF_USESHOWWINDOW;
                WinGetSettingsStartupInfo.wShowWindow = WindowShowStyle.SW_HIDE;
                WinGetSettingsStartupInfo.cbReserved2 = 0;
                WinGetSettingsStartupInfo.lpReserved2 = IntPtr.Zero;

                WinGetSettingsStartupInfo.cb = Marshal.SizeOf(typeof(STARTUPINFO));
                bool createResult = Kernel32Library.CreateProcess(null, string.Format("{0} {1}", "winget.exe", "settings"), IntPtr.Zero, IntPtr.Zero, false, CreateProcessFlags.CREATE_NO_WINDOW, IntPtr.Zero, null, ref WinGetSettingsStartupInfo, out PROCESS_INFORMATION WinGetSettingsProcessInformation);

                if (createResult)
                {
                    if (WinGetSettingsProcessInformation.hProcess != IntPtr.Zero) Kernel32Library.CloseHandle(WinGetSettingsProcessInformation.hProcess);
                    if (WinGetSettingsProcessInformation.hThread != IntPtr.Zero) Kernel32Library.CloseHandle(WinGetSettingsProcessInformation.hThread);
                }
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
            MenuFlyoutItem item = sender as MenuFlyoutItem;
            if (item.Tag is not null)
            {
                switch ((string)item.Tag)
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
                                FolderPicker folderPicker = new FolderPicker();
                                InitializeWithWindow.Initialize(folderPicker, Program.ApplicationRoot.MainWindow.Handle);
                                folderPicker.SuggestedStartLocation = PickerLocationId.Downloads;

                                StorageFolder downloadFolder = await folderPicker.PickSingleFolderAsync();

                                if (downloadFolder is not null)
                                {
                                    DownloadFolder = downloadFolder;
                                    DownloadOptionsService.SetFolder(downloadFolder);
                                }
                            }
                            catch (Exception)
                            {
                                new FolderPickerNotification(this).Show();
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
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.Tag is not null)
            {
                DownloadItem = Convert.ToInt32(item.Tag);
                DownloadOptionsService.SetItem(DownloadItem);
            }
        }

        /// <summary>
        /// 修改下载文件的方式
        /// </summary>
        private void OnDownloadModeSelectClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.Tag is not null)
            {
                DownloadMode = DownloadModeList[Convert.ToInt32(item.Tag)];
                DownloadOptionsService.SetMode(DownloadMode);
            }
        }

        /// <summary>
        /// 应用安装方式设置
        /// </summary>
        private void OnInstallModeSelectClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem item = sender as ToggleMenuFlyoutItem;
            if (item.Tag is not null)
            {
                InstallMode = InstallModeList[Convert.ToInt32(item.Tag)];
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
            new LogCleanNotification(this, result).Show();
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
