using GetStoreApp.Extensions.Backdrop;
using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Download;
using GetStoreApp.Models.Window;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Dialogs.Common;
using GetStoreApp.UI.TeachingTips;
using GetStoreApp.Views.Pages;
using GetStoreApp.WindowsAPI.ComTypes;
using GetStoreApp.WindowsAPI.PInvoke.Comctl32;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using GetStoreApp.WindowsAPI.PInvoke.Uxtheme;
using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Content;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Diagnostics;
using Windows.Graphics;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.System;
using Windows.UI;
using Windows.UI.StartScreen;
using WinRT;
using WinRT.Interop;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace GetStoreApp.Views.Windows
{
    /// <summary>
    /// 应用主窗口
    /// </summary>
    public sealed partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly ContentCoordinateConverter contentCoordinateConverter;
        private readonly OverlappedPresenter overlappedPresenter;
        private readonly SUBCLASSPROC mainWindowSubClassProc;
        private bool isDialogOpening;
        private ContentIsland contentIsland;
        private DesktopSiteBridge desktopSiteBridge;
        private InputKeyboardSource inputKeyboardSource;

        public new static MainWindow Current { get; private set; }

        private string _windowTitle;

        public string WindowTitle
        {
            get { return _windowTitle; }

            set
            {
                if (!Equals(_windowTitle, value))
                {
                    _windowTitle = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WindowTitle)));
                }
            }
        }

        private SystemBackdrop _windowSystemBackdrop;

        public SystemBackdrop WindowSystemBackdrop
        {
            get { return _windowSystemBackdrop; }

            set
            {
                if (!Equals(_windowSystemBackdrop, value))
                {
                    _windowSystemBackdrop = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WindowSystemBackdrop)));
                }
            }
        }

        private ElementTheme _windowTheme;

        public ElementTheme WindowTheme
        {
            get { return _windowTheme; }

            set
            {
                if (!Equals(_windowTheme, value))
                {
                    _windowTheme = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WindowTheme)));
                }
            }
        }

        private bool _isWindowMaximized;

        public bool IsWindowMaximized
        {
            get { return _isWindowMaximized; }

            set
            {
                if (!Equals(_isWindowMaximized, value))
                {
                    _isWindowMaximized = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsWindowMaximized)));
                }
            }
        }

        private bool _isBackEnabled;

        public bool IsBackEnabled
        {
            get { return _isBackEnabled; }

            set
            {
                if (!Equals(_isBackEnabled, value))
                {
                    _isBackEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsBackEnabled)));
                }
            }
        }

        private NavigationViewItem _selectedItem;

        public NavigationViewItem SelectedItem
        {
            get { return _selectedItem; }

            set
            {
                if (!Equals(_selectedItem, value))
                {
                    _selectedItem = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedItem)));
                }
            }
        }

        private List<KeyValuePair<string, Type>> PageList { get; } =
        [
            KeyValuePair.Create("Store",typeof(StorePage)),
            KeyValuePair.Create("AppUpdate", typeof(AppUpdatePage)),
            KeyValuePair.Create("WinGet", typeof(WinGetPage)),
            KeyValuePair.Create("AppManager", typeof(AppManagerPage)),
            KeyValuePair.Create("Download", typeof(DownloadPage)),
            KeyValuePair.Create<string, Type>("Web",null),
            KeyValuePair.Create("Settings", typeof(SettingsPage))
        ];

        public List<NavigationModel> NavigationItemList { get; } = [];

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            Current = this;
            InitializeComponent();

            // 窗口部分初始化
            WindowTitle = RuntimeHelper.IsElevated ? ResourceService.GetLocalized("Window/WindowTitle") + ResourceService.GetLocalized("Window/RunningAdministrator") : ResourceService.GetLocalized("Window/WindowTitle");
            overlappedPresenter = AppWindow.Presenter as OverlappedPresenter;
            ExtendsContentIntoTitleBar = true;
            AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
            AppWindow.TitleBar.InactiveBackgroundColor = Colors.Transparent;
            AppWindow.TitleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;
            IsWindowMaximized = overlappedPresenter.State is OverlappedPresenterState.Maximized;
            contentCoordinateConverter = ContentCoordinateConverter.CreateForWindowId(AppWindow.Id);

            // 标题栏和右键菜单设置
            SetClassicMenuTheme((Content as FrameworkElement).ActualTheme);

            // 挂载相应的事件
            AppWindow.Changed += OnAppWindowChanged;
            AppWindow.Closing += OnAppWindowClosing;
            NetworkInformation.NetworkStatusChanged += OnNetworkStatusChanged;
            ApplicationData.Current.DataChanged += OnDataChanged;
            ThemeService.PropertyChanged += OnServicePropertyChanged;
            BackdropService.PropertyChanged += OnServicePropertyChanged;
            TopMostService.PropertyChanged += OnServicePropertyChanged;

            // 为应用主窗口添加窗口过程
            mainWindowSubClassProc = new SUBCLASSPROC(MainWindowSubClassProc);
            Comctl32Library.SetWindowSubclass(Win32Interop.GetWindowFromWindowId(AppWindow.Id), mainWindowSubClassProc, 0, IntPtr.Zero);

            SetWindowTheme();
            SetSystemBackdrop();
            SetTopMost();
            CheckNetwork();
        }

        #region 第一部分：窗口类事件

        /// <summary>
        /// 窗口激活状态发生变化的事件
        /// </summary>
        private void OnActivated(object sender, WindowActivatedEventArgs args)
        {
            try
            {
                if (contentIsland is not null && !contentIsland.IsClosed && WindowSystemBackdrop is MaterialBackdrop materialBackdrop && materialBackdrop.BackdropConfiguration is not null)
                {
                    materialBackdrop.BackdropConfiguration.IsInputActive = AlwaysShowBackdropService.AlwaysShowBackdropValue || args.WindowActivationState is not WindowActivationState.Deactivated;
                    Task.Run(NotificationService.UpdateNotificationSetting);
                }
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
            }
        }

        /// <summary>
        /// 窗口大小发生改变时的事件
        /// </summary>
        private void OnSizeChanged(object sender, WindowSizeChangedEventArgs args)
        {
            if (TitlebarMenuFlyout.IsOpen)
            {
                TitlebarMenuFlyout.Hide();
            }

            if (overlappedPresenter is not null)
            {
                IsWindowMaximized = overlappedPresenter.State is OverlappedPresenterState.Maximized;
            }

            if (Content is not null && Content.XamlRoot is not null)
            {
                overlappedPresenter.PreferredMinimumWidth = Convert.ToInt32(1280 * Content.XamlRoot.RasterizationScale);
                overlappedPresenter.PreferredMinimumHeight = Convert.ToInt32(720 * Content.XamlRoot.RasterizationScale);
            }
        }

        #endregion 第一部分：窗口类事件

        #region 第二部分：窗口辅助类挂载的事件

        /// <summary>
        /// 窗口位置变化发生的事件
        /// </summary>
        private void OnAppWindowChanged(AppWindow sender, AppWindowChangedEventArgs args)
        {
            if (desktopSiteBridge is not null && !desktopSiteBridge.IsClosed)
            {
                desktopSiteBridge.MoveAndResize(new RectInt32(0, 0, AppWindow.ClientSize.Width, AppWindow.ClientSize.Height));
            }

            // 窗口位置发生变化
            if (args.DidPositionChange)
            {
                if (TitlebarMenuFlyout.IsOpen)
                {
                    TitlebarMenuFlyout.Hide();
                }

                if (overlappedPresenter is not null)
                {
                    IsWindowMaximized = overlappedPresenter.State is OverlappedPresenterState.Maximized;
                }

                if (Content is not null && Content.XamlRoot is not null)
                {
                    // 窗口移动时，校对并纠正弹出窗口位置错误的问题
                    foreach (Popup popup in VisualTreeHelper.GetOpenPopupsForXamlRoot(Content.XamlRoot))
                    {
                        ElementCompositeMode compositeMode = popup.CompositeMode;
                        popup.CompositeMode = compositeMode is ElementCompositeMode.SourceOver ? ElementCompositeMode.MinBlend : ElementCompositeMode.SourceOver;
                        popup.CompositeMode = compositeMode;
                    }
                }
            }
        }

        /// <summary>
        /// 关闭窗口之后关闭其他服务
        /// </summary>
        private async void OnAppWindowClosing(object sender, AppWindowClosingEventArgs args)
        {
            args.Cancel = true;

            List<DownloadSchedulerModel> downloadSchedulerList = null;
            DownloadSchedulerService.DownloadSchedulerSemaphoreSlim?.Wait();
            try
            {
                downloadSchedulerList = DownloadSchedulerService.GetDownloadSchedulerList();
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Query download scheduler list failed", e);
            }
            finally
            {
                DownloadSchedulerService.DownloadSchedulerSemaphoreSlim?.Release();
            }

            // 下载队列存在任务时，弹出对话窗口确认是否要关闭窗口
            if (downloadSchedulerList is not null && downloadSchedulerList.Count > 0)
            {
                Show();

                // 关闭窗口提示对话框是否已经处于打开状态，如果是，不再弹出
                ContentDialogResult result = await ShowDialogAsync(new ClosingWindowDialog());

                if (result is ContentDialogResult.Primary)
                {
                    AppWindow.Changed -= OnAppWindowChanged;
                    contentIsland.Environment.SettingChanged -= OnSettingChanged;
                    inputKeyboardSource.SystemKeyDown -= OnSystemKeyDown;
                    ApplicationData.Current.DataChanged -= OnDataChanged;
                    ThemeService.PropertyChanged -= OnServicePropertyChanged;
                    BackdropService.PropertyChanged -= OnServicePropertyChanged;
                    TopMostService.PropertyChanged -= OnServicePropertyChanged;
                    DownloadSchedulerService.TerminateDownload();
                    Comctl32Library.RemoveWindowSubclass(Win32Interop.GetWindowFromWindowId(AppWindow.Id), mainWindowSubClassProc, 0);
                    (Application.Current as WinUIApp).Dispose();
                }
                else if (result is ContentDialogResult.Secondary)
                {
                    if (GetCurrentPageType() != typeof(DownloadPage))
                    {
                        NavigateTo(typeof(DownloadPage));
                    }
                }
            }
            else
            {
                AppWindow.Changed -= OnAppWindowChanged;
                contentIsland.Environment.SettingChanged -= OnSettingChanged;
                inputKeyboardSource.SystemKeyDown -= OnSystemKeyDown;
                ApplicationData.Current.DataChanged -= OnDataChanged;
                ThemeService.PropertyChanged -= OnServicePropertyChanged;
                BackdropService.PropertyChanged -= OnServicePropertyChanged;
                TopMostService.PropertyChanged -= OnServicePropertyChanged;
                Comctl32Library.RemoveWindowSubclass(Win32Interop.GetWindowFromWindowId(AppWindow.Id), mainWindowSubClassProc, 0);
                (Application.Current as WinUIApp).Dispose();
            }
        }

        /// <summary>
        /// 内容岛设置发生更改时触发的事件
        /// </summary>
        private void OnSettingChanged(ContentIslandEnvironment sender, ContentEnvironmentSettingChangedEventArgs args)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                if (Equals(ThemeService.AppTheme, ThemeService.ThemeList[0]))
                {
                    WindowTheme = Application.Current.RequestedTheme is ApplicationTheme.Light ? ElementTheme.Light : ElementTheme.Dark;
                }

                StoreRegionService.UpdateDefaultRegion();
            });
        }

        /// <summary>
        /// 处理键盘 Alt + Space 键弹出窗口右键菜单事件
        /// </summary>
        private void OnSystemKeyDown(InputKeyboardSource sender, KeyEventArgs args)
        {
            if (args.VirtualKey is VirtualKey.Space)
            {
                args.Handled = true;
                FlyoutShowOptions options = new()
                {
                    Position = new Point(0, 45),
                    ShowMode = FlyoutShowMode.Standard
                };
                TitlebarMenuFlyout.ShowAt(null, options);
            }
        }

        #endregion 第二部分：窗口辅助类挂载的事件

        #region 第三部分：窗口右键菜单事件

        /// <summary>
        /// 窗口还原
        /// </summary>
        private void OnRestoreClicked(object sender, RoutedEventArgs args)
        {
            User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(AppWindow.Id), WindowMessage.WM_SYSCOMMAND, (UIntPtr)SYSTEMCOMMAND.SC_RESTORE, 0);
        }

        /// <summary>
        /// 窗口移动
        /// </summary>
        private void OnMoveClicked(object sender, RoutedEventArgs args)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem && menuFlyoutItem.Tag is not null)
            {
                ((MenuFlyout)menuFlyoutItem.Tag).Hide();
                User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(AppWindow.Id), WindowMessage.WM_SYSCOMMAND, (UIntPtr)SYSTEMCOMMAND.SC_MOVE, 0);
            }
        }

        /// <summary>
        /// 窗口大小
        /// </summary>
        private void OnSizeClicked(object sender, RoutedEventArgs args)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem && menuFlyoutItem.Tag is not null)
            {
                ((MenuFlyout)menuFlyoutItem.Tag).Hide();
                User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(AppWindow.Id), WindowMessage.WM_SYSCOMMAND, (UIntPtr)SYSTEMCOMMAND.SC_SIZE, 0);
            }
        }

        /// <summary>
        /// 窗口最小化
        /// </summary>
        private void OnMinimizeClicked(object sender, RoutedEventArgs args)
        {
            User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(AppWindow.Id), WindowMessage.WM_SYSCOMMAND, (UIntPtr)SYSTEMCOMMAND.SC_MINIMIZE, 0);
        }

        /// <summary>
        /// 窗口最大化
        /// </summary>
        private void OnMaximizeClicked(object sender, RoutedEventArgs args)
        {
            User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(AppWindow.Id), WindowMessage.WM_SYSCOMMAND, (UIntPtr)SYSTEMCOMMAND.SC_MAXIMIZE, 0);
        }

        /// <summary>
        /// 窗口关闭
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(AppWindow.Id), WindowMessage.WM_SYSCOMMAND, (UIntPtr)SYSTEMCOMMAND.SC_CLOSE, 0);
        }

        #endregion 第三部分：窗口右键菜单事件

        #region 第四部分：窗口内容挂载的事件

        /// <summary>
        /// 应用主题变化时设置标题栏按钮的颜色
        /// </summary>
        private void OnActualThemeChanged(FrameworkElement sender, object args)
        {
            SetTitleBarTheme(sender.ActualTheme);
            SetClassicMenuTheme(sender.ActualTheme);
        }

        /// <summary>
        /// 按下 Alt + BackSpace 键时，导航控件返回到上一页
        /// </summary>
        private void OnKeyDown(object sender, KeyRoutedEventArgs args)
        {
            if (args.Key is VirtualKey.Back && args.KeyStatus.IsMenuKeyDown)
            {
                if (GetFrameContent() is AppManagerPage appManagerPage && appManagerPage.BreadCollection.Count is 2)
                {
                    appManagerPage.NavigateTo(appManagerPage.PageList[0], null, false);
                }
                else if (GetFrameContent() is SettingsPage settingsPage && settingsPage.BreadCollection.Count is 2)
                {
                    settingsPage.NavigateTo(settingsPage.PageList[0], null, false);
                }
                else
                {
                    NavigationFrom();
                }
            }
        }

        /// <summary>
        /// 固定到开始屏幕
        /// </summary>
        private async void OnPinToStartScreenClicked(object sender, RoutedEventArgs args)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem && menuFlyoutItem.Tag is not null)
            {
                string tag = Convert.ToString(menuFlyoutItem.Tag);
                string displayName = string.Empty;

                switch (tag)
                {
                    case "Store": displayName = WindowStoreText.Text; break;
                    case "AppUpdate": displayName = WindowAppUpdateText.Text; break;
                    case "WinGet": displayName = WindowWinGetText.Text; break;
                    case "AppManager": displayName = WindowAppManagerText.Text; break;
                    case "Download": displayName = WindowDownloadText.Text; break;
                    case "Web": displayName = WindowWebText.Text; break;
                    case "Settings": displayName = WindowSettingsText.Text; break;
                }

                if (RuntimeHelper.IsElevated)
                {
                    await Launcher.LaunchUriAsync(new Uri("getstoreapppinner:"), new LauncherOptions() { TargetApplicationPackageFamilyName = Package.Current.Id.FamilyName }, new ValueSet()
                    {
                        {"Type", nameof(SecondaryTile) },
                        { "DisplayName", displayName },
                        { "Tag", tag },
                    });
                }
                else
                {
                    bool isPinnedSuccessfully = false;

                    try
                    {
                        SecondaryTile secondaryTile = new("GetStoreApp" + tag)
                        {
                            DisplayName = displayName,
                            Arguments = "SecondaryTile " + tag
                        };

                        secondaryTile.VisualElements.BackgroundColor = Colors.Transparent;
                        secondaryTile.VisualElements.Square150x150Logo = new Uri(string.Format("ms-appx:///Assets/Icon/Control/{0}.png", tag));
                        secondaryTile.VisualElements.Square71x71Logo = new Uri(string.Format("ms-appx:///Assets/Icon/Control/{0}.png", tag));
                        secondaryTile.VisualElements.Square44x44Logo = new Uri(string.Format("ms-appx:///Assets/Icon/Control/{0}.png", tag));

                        secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;

                        InitializeWithWindow.Initialize(secondaryTile, Win32Interop.GetWindowFromWindowId(AppWindow.Id));
                        isPinnedSuccessfully = await secondaryTile.RequestCreateAsync();
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "Use SecondaryTile api to pin app to startscreen failed.", e);
                    }
                    finally
                    {
                        await ShowNotificationAsync(new QuickOperationTip(QuickOperationKind.StartScreen, isPinnedSuccessfully));
                    }
                }
            }
        }

        #endregion 第四部分：窗口内容挂载的事件

        #region 第五部分：导航控件及其内容挂载的事件

        /// <summary>
        /// 导航控件加载完成后初始化内容，初始化导航控件属性、屏幕缩放比例值和应用的背景色
        /// </summary>
        private async void OnLoaded(object sender, RoutedEventArgs args)
        {
            contentIsland = Content.XamlRoot.ContentIsland;
            contentIsland.As<IContentIslandPartner>().Get_TEMP_DesktopSiteBridge(out IntPtr desktopSiteBridgePtr);
            desktopSiteBridge = DesktopSiteBridge.FromAbi(desktopSiteBridgePtr);
            contentIsland.Environment.SettingChanged += OnSettingChanged;
            inputKeyboardSource = InputKeyboardSource.GetForIsland(contentIsland);
            inputKeyboardSource.SystemKeyDown += OnSystemKeyDown;

            // 设置标题栏主题
            SetTitleBarTheme((Content as FrameworkElement).ActualTheme);

            // 导航控件加载完成后初始化内容
            if (sender is NavigationView navigationView)
            {
                foreach (object menuItem in navigationView.MenuItems)
                {
                    if (menuItem is NavigationViewItem navigationViewItem)
                    {
                        int tagIndex = PageList.FindIndex(item => Equals(item.Key, navigationViewItem.Tag));

                        NavigationItemList.Add(new NavigationModel()
                        {
                            NavigationTag = PageList[tagIndex].Key,
                            NavigationItem = navigationViewItem,
                            NavigationPage = PageList[tagIndex].Value,
                        });
                    }
                }

                foreach (object footerMenuItem in navigationView.FooterMenuItems)
                {
                    if (footerMenuItem is NavigationViewItem navigationViewItem)
                    {
                        int tagIndex = PageList.FindIndex(item => Equals(item.Key, navigationViewItem.Tag));

                        NavigationItemList.Add(new NavigationModel()
                        {
                            NavigationTag = PageList[tagIndex].Key,
                            NavigationItem = navigationViewItem,
                            NavigationPage = PageList[tagIndex].Value,
                        });
                    }
                }
            }

            // 初始化启动信息
            StorageDataKind dataKind = ResultService.GetStorageDataKind();
            List<string> dataContentList = ResultService.ReadResult(dataKind);
            ResultService.SaveResult(StorageDataKind.None, null);

            if (dataKind is StorageDataKind.Launch)
            {
                // 正常启动重定向获得的内容
                if (dataContentList.Count is 1 || dataContentList.Count is 2)
                {
                    NavigateTo(typeof(StorePage));

                    // 正常启动
                    if (dataContentList[0] is "Launch")
                    {
                        Show();

                        // 应用已经启动
                        if (dataContentList.Count is 2 && dataContentList[1] is "IsRunning")
                        {
                            await ShowDialogAsync(new AppRunningDialog());
                        }
                    }
                    // 跳转列表或辅助磁贴启动重定向获得的内容
                    else if (dataContentList[0] is "JumpList" || dataContentList[0] is "SecondaryTile")
                    {
                        if (dataContentList[1] is "Store" && GetCurrentPageType() != typeof(StorePage))
                        {
                            NavigateTo(typeof(StorePage));
                        }
                        else if (dataContentList[1] is "AppUpdate" && GetCurrentPageType() != typeof(AppUpdatePage))
                        {
                            NavigateTo(typeof(AppUpdatePage));
                        }
                        else if (dataContentList[1] is "WinGet" && GetCurrentPageType() != typeof(WinGetPage))
                        {
                            NavigateTo(typeof(WinGetPage));
                        }
                        else if (dataContentList[1] is "AppManager" && GetCurrentPageType() != typeof(AppManagerPage))
                        {
                            NavigateTo(typeof(AppManagerPage));
                        }
                        else if (dataContentList[1] is "Download" && GetCurrentPageType() != typeof(DownloadPage))
                        {
                            NavigateTo(typeof(DownloadPage));
                        }
                    }
                }
                // 带有命令参数启动重定向获得的内容
                else if (dataContentList.Count is 4 && dataContentList[0] is "Console")
                {
                    if (Equals(GetCurrentPageType(), typeof(StorePage)))
                    {
                        (GetFrameContent() as StorePage).InitializeQueryLinksContent(dataContentList[1..4]);
                    }
                    else
                    {
                        NavigateTo(typeof(StorePage), dataContentList[1..4]);
                    }
                }
            }
            // 从通知协议启动重定向获得的内容
            else if (dataKind is StorageDataKind.Protocol)
            {
                NavigateTo(typeof(StorePage));

                // 正常启动重定向获得的内容
                if (dataContentList.Count is 1 || dataContentList.Count is 2)
                {
                    // 正常启动
                    if (dataContentList[0] is "Launch")
                    {
                        Show();

                        // 应用已经启动
                        if (dataContentList.Count is 2 && dataContentList[1] is "IsRunning")
                        {
                            await ShowDialogAsync(new AppRunningDialog());
                        }
                    }
                    // 打开设置
                    else if (dataContentList[0] is "Settings")
                    {
                        if (GetCurrentPageType() != typeof(SettingsPage))
                        {
                            NavigateTo(typeof(SettingsPage));
                        }
                    }
                }
            }
            // 从共享目标启动重定向获得的内容
            else if (dataKind is StorageDataKind.ShareTarget)
            {
                if (dataContentList.Count is 3)
                {
                    if (Equals(GetCurrentPageType(), typeof(StorePage)))
                    {
                        (GetFrameContent() as StorePage).InitializeQueryLinksContent(dataContentList);
                    }
                    else
                    {
                        NavigateTo(typeof(StorePage), dataContentList);
                    }
                }

                Show();
            }
            // 从 Toast 通知启动重定向获得的内容
            else if (dataKind is StorageDataKind.ToastNotification)
            {
                if (dataContentList.Count is 1 && dataContentList[0] is "OpenApp")
                {
                    if (GetCurrentPageType() != typeof(StorePage))
                    {
                        NavigateTo(typeof(StorePage));
                    }
                }
                else if (dataContentList.Count is 1 && dataContentList[0] is "ViewDownloadPage")
                {
                    if (GetCurrentPageType() != typeof(DownloadPage))
                    {
                        NavigateTo(typeof(DownloadPage));
                    }
                }

                Show();
            }

            if (Content is not null && Content.XamlRoot is not null)
            {
                overlappedPresenter.PreferredMinimumWidth = Convert.ToInt32(1280 * Content.XamlRoot.RasterizationScale);
                overlappedPresenter.PreferredMinimumHeight = Convert.ToInt32(720 * Content.XamlRoot.RasterizationScale);
            }
        }

        /// <summary>
        /// 当后退按钮收到交互（如单击或点击）时发生
        /// </summary>
        private void OnBackRequested(object sender, NavigationViewBackRequestedEventArgs args)
        {
            if (GetFrameContent() is AppManagerPage appManagerPage && appManagerPage.BreadCollection.Count is 2)
            {
                appManagerPage.NavigateTo(appManagerPage.PageList[0], null, false);
            }
            else if (GetFrameContent() is SettingsPage settingsPage && settingsPage.BreadCollection.Count is 2)
            {
                settingsPage.NavigateTo(settingsPage.PageList[0], null, false);
            }
            else
            {
                NavigationFrom();
            }
        }

        /// <summary>
        /// 在当前导航控件所选项更改时发生
        /// </summary>
        private async void OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItemContainer is NavigationViewItemBase navigationViewItem && navigationViewItem.Tag is not null)
            {
                NavigationModel navigationItem = NavigationItemList.Find(item => Equals(item.NavigationTag, PageList[PageList.FindIndex(item => Equals(item.Key, navigationViewItem.Tag))].Key));

                if (SelectedItem != navigationItem.NavigationItem)
                {
                    if (PageList[PageList.FindIndex(item => Equals(item.Key, navigationViewItem.Tag))].Key is "Web")
                    {
                        await Launcher.LaunchUriAsync(new Uri("getstoreappwebbrowser:"));
                        sender.SelectedItem = SelectedItem;
                    }
                    else
                    {
                        NavigateTo(navigationItem.NavigationPage);
                    }
                }
            }
        }

        /// <summary>
        /// 导航完成后发生
        /// </summary>
        private void OnNavigated(object sender, NavigationEventArgs args)
        {
            SelectedItem = NavigationItemList.Find(item => Equals(item.NavigationPage, GetCurrentPageType())).NavigationItem;
            IsBackEnabled = CanGoBack();
        }

        /// <summary>
        /// 导航失败时发生
        /// </summary>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs args)
        {
            args.Handled = true;
            LogService.WriteLog(LoggingLevel.Warning, string.Format(ResourceService.GetLocalized("Window/NavigationFailed"), args.SourcePageType.FullName), args.Exception);
            (Application.Current as WinUIApp).Dispose();
        }

        #endregion 第五部分：导航控件及其内容挂载的事件

        #region 第六部分：自定义事件

        /// <summary>
        /// 网络状态发生变化时触发的事件
        /// </summary>
        private void OnNetworkStatusChanged(object sender)
        {
            DispatcherQueue.TryEnqueue(CheckNetwork);
        }

        /// <summary>
        /// 同步漫游应用程序数据时发生的事件
        /// </summary>
        private void OnDataChanged(ApplicationData sender, object args)
        {
            StorageDataKind dataKind = ResultService.GetStorageDataKind();
            List<string> dataList = ResultService.ReadResult(dataKind);

            ResultService.SaveResult(StorageDataKind.None, null);

            if (dataKind is StorageDataKind.Launch)
            {
                // 正常启动重定向获得的内容
                if (dataList.Count is 1 || dataList.Count is 2)
                {
                    // 正常启动
                    if (dataList[0] is "Launch")
                    {
                        DispatcherQueue.TryEnqueue(async () =>
                        {
                            Show();

                            // 应用已经启动
                            if (dataList.Count is 2 && dataList[1] is "IsRunning")
                            {
                                await ShowDialogAsync(new AppRunningDialog());
                            }
                        });
                    }
                    // 跳转列表或辅助磁贴启动重定向获得的内容
                    else if (dataList[0] is "JumpList" || dataList[0] is "SecondaryTile")
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            if (dataList[1] is "Store" && GetCurrentPageType() != typeof(StorePage))
                            {
                                NavigateTo(typeof(StorePage));
                            }
                            else if (dataList[1] is "AppUpdate" && GetCurrentPageType() != typeof(AppUpdatePage))
                            {
                                NavigateTo(typeof(AppUpdatePage));
                            }
                            else if (dataList[1] is "WinGet" && GetCurrentPageType() != typeof(WinGetPage))
                            {
                                NavigateTo(typeof(WinGetPage));
                            }
                            else if (dataList[1] is "AppManager" && GetCurrentPageType() != typeof(AppManagerPage))
                            {
                                NavigateTo(typeof(AppManagerPage));
                            }
                            else if (dataList[1] is "Download" && GetCurrentPageType() != typeof(DownloadPage))
                            {
                                NavigateTo(typeof(DownloadPage));
                            }
                        });
                    }
                }
                // 带有命令参数启动重定向获得的内容
                else if (dataList.Count is 4 && dataList[0] is "Console")
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        if (dataList.Count is 4)
                        {
                            if (Equals(GetCurrentPageType(), typeof(StorePage)))
                            {
                                (GetFrameContent() as StorePage).InitializeQueryLinksContent(dataList[1..4]);
                            }
                            else
                            {
                                NavigateTo(typeof(StorePage), dataList[1..4]);
                            }
                        }
                    });
                }
            }
            // 从共享目标启动重定向获得的内容
            else if (dataKind is StorageDataKind.ShareTarget)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    if (dataList.Count is 3)
                    {
                        if (Equals(GetCurrentPageType(), typeof(StorePage)))
                        {
                            (GetFrameContent() as StorePage).InitializeQueryLinksContent(dataList);
                        }
                        else
                        {
                            NavigateTo(typeof(StorePage), dataList);
                        }
                    }

                    Show();
                });
            }
            // 从通知协议启动重定向获得的内容
            else if (dataKind is StorageDataKind.Protocol)
            {
                // 正常启动重定向获得的内容
                if (dataList.Count is 1 || dataList.Count is 2)
                {
                    // 正常启动
                    if (dataList[0] is "Launch")
                    {
                        DispatcherQueue.TryEnqueue(async () =>
                        {
                            Show();

                            // 应用已经启动
                            if (dataList.Count is 2 && dataList[1] is "IsRunning")
                            {
                                await ShowDialogAsync(new AppRunningDialog());
                            }
                        });
                    }
                    // 打开设置
                    else if (dataList[0] is "Settings")
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            if (GetCurrentPageType() != typeof(SettingsPage))
                            {
                                NavigateTo(typeof(SettingsPage));
                            }
                        });
                    }
                }
            }
            // 从 Toast 通知启动重定向获得的内容
            else if (dataKind is StorageDataKind.ToastNotification)
            {
                DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
                {
                    if (dataList.Count is 1 && dataList[0] is "OpenApp")
                    {
                        if (GetCurrentPageType() != typeof(StorePage))
                        {
                            NavigateTo(typeof(StorePage));
                        }
                    }
                    else if (dataList.Count is 1 && dataList[0] is "ViewDownloadPage")
                    {
                        if (GetCurrentPageType() != typeof(DownloadPage))
                        {
                            NavigateTo(typeof(DownloadPage));
                        }
                    }

                    Show();
                });
            }
            // 开始屏幕“辅助磁贴”固定结果
            else if (dataKind is StorageDataKind.SecondaryTile)
            {
                if (dataList.Count is 1)
                {
                    DispatcherQueue.TryEnqueue(async () =>
                    {
                        await ShowNotificationAsync(new QuickOperationTip(QuickOperationKind.StartScreen, Convert.ToBoolean(dataList[0])));
                    });
                }
            }
            // 任务栏应用固定结果
            else if (dataKind is StorageDataKind.TaskbarManager)
            {
                if (dataList.Count is 1)
                {
                    DispatcherQueue.TryEnqueue(async () =>
                    {
                        await ShowNotificationAsync(new QuickOperationTip(QuickOperationKind.Taskbar, Convert.ToBoolean(dataList[0])));
                    });
                }
            }
        }

        /// <summary>
        /// 设置选项发生变化时触发的事件
        /// </summary>
        private void OnServicePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                if (Equals(args.PropertyName, nameof(ThemeService.AppTheme)))
                {
                    SetWindowTheme();
                }
                if (Equals(args.PropertyName, nameof(BackdropService.AppBackdrop)))
                {
                    SetSystemBackdrop();
                }
                if (Equals(args.PropertyName, nameof(TopMostService.TopMostValue)))
                {
                    SetTopMost();
                }
            });
        }

        #endregion 第六部分：自定义事件

        #region 第七部分：窗口属性设置

        /// <summary>
        /// 设置应用显示的主题
        /// </summary>
        public void SetWindowTheme()
        {
            WindowTheme = Equals(ThemeService.AppTheme, ThemeService.ThemeList[0])
                ? Application.Current.RequestedTheme is ApplicationTheme.Light ? ElementTheme.Light : ElementTheme.Dark
                : Enum.TryParse(ThemeService.AppTheme.Key, out ElementTheme elementTheme) ? elementTheme : ElementTheme.Default;
        }

        /// <summary>
        /// 设置应用的背景色
        /// </summary>
        private void SetSystemBackdrop()
        {
            if (Equals(BackdropService.AppBackdrop, BackdropService.BackdropList[1]))
            {
                WindowSystemBackdrop = new MaterialBackdrop(MicaKind.Base);
                VisualStateManager.GoToState(MainPage, "BackgroundTransparent", false);
            }
            else if (Equals(BackdropService.AppBackdrop, BackdropService.BackdropList[2]))
            {
                WindowSystemBackdrop = new MaterialBackdrop(MicaKind.BaseAlt);
                VisualStateManager.GoToState(MainPage, "BackgroundTransparent", false);
            }
            else if (Equals(BackdropService.AppBackdrop, BackdropService.BackdropList[3]))
            {
                WindowSystemBackdrop = new MaterialBackdrop(DesktopAcrylicKind.Default);
                VisualStateManager.GoToState(MainPage, "BackgroundTransparent", false);
            }
            else if (Equals(BackdropService.AppBackdrop, BackdropService.BackdropList[4]))
            {
                WindowSystemBackdrop = new MaterialBackdrop(DesktopAcrylicKind.Base);
                VisualStateManager.GoToState(MainPage, "BackgroundTransparent", false);
            }
            else if (Equals(BackdropService.AppBackdrop, BackdropService.BackdropList[5]))
            {
                WindowSystemBackdrop = new MaterialBackdrop(DesktopAcrylicKind.Thin);
                VisualStateManager.GoToState(MainPage, "BackgroundTransparent", false);
            }
            else
            {
                WindowSystemBackdrop = null;
                VisualStateManager.GoToState(MainPage, "BackgroundDefault", false);
            }
        }

        /// <summary>
        /// 设置标题栏按钮的主题色
        /// </summary>
        private void SetTitleBarTheme(ElementTheme theme)
        {
            AppWindowTitleBar titleBar = AppWindow.TitleBar;

            titleBar.BackgroundColor = Colors.Transparent;
            titleBar.ForegroundColor = Colors.Transparent;
            titleBar.InactiveBackgroundColor = Colors.Transparent;
            titleBar.InactiveForegroundColor = Colors.Transparent;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            if (theme is ElementTheme.Light)
            {
                titleBar.ButtonForegroundColor = Color.FromArgb(255, 23, 23, 23);
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(25, 0, 0, 0);
                titleBar.ButtonHoverForegroundColor = Colors.Black;
                titleBar.ButtonPressedBackgroundColor = Color.FromArgb(51, 0, 0, 0);
                titleBar.ButtonPressedForegroundColor = Colors.Black;
                titleBar.ButtonInactiveForegroundColor = Color.FromArgb(255, 153, 153, 153);
            }
            else
            {
                titleBar.ButtonForegroundColor = Color.FromArgb(255, 242, 242, 242);
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(25, 255, 255, 255);
                titleBar.ButtonHoverForegroundColor = Colors.White;
                titleBar.ButtonPressedBackgroundColor = Color.FromArgb(51, 255, 255, 255);
                titleBar.ButtonPressedForegroundColor = Colors.White;
                titleBar.ButtonInactiveForegroundColor = Color.FromArgb(255, 102, 102, 102);
            }
        }

        /// <summary>
        /// 设置传统菜单标题栏按钮的主题色
        /// </summary>
        private void SetClassicMenuTheme(ElementTheme theme)
        {
            AppWindowTitleBar titleBar = AppWindow.TitleBar;

            if (theme is ElementTheme.Light)
            {
                titleBar.PreferredTheme = TitleBarTheme.Light;
                UxthemeLibrary.SetPreferredAppMode(PreferredAppMode.ForceLight);
            }
            else
            {
                titleBar.PreferredTheme = TitleBarTheme.Dark;
                UxthemeLibrary.SetPreferredAppMode(PreferredAppMode.ForceDark);
            }

            UxthemeLibrary.FlushMenuThemes();
        }

        /// <summary>
        /// 显示窗口
        /// </summary>
        public void Show(bool isFirstActivate = false)
        {
            if (isFirstActivate)
            {
                bool? isWindowMaximized = LocalSettingsService.ReadSetting<bool?>(ConfigKey.IsWindowMaximizedKey);
                int? windowWidth = LocalSettingsService.ReadSetting<int?>(ConfigKey.WindowWidthKey);
                int? windowHeight = LocalSettingsService.ReadSetting<int?>(ConfigKey.WindowHeightKey);
                int? windowPositionXAxis = LocalSettingsService.ReadSetting<int?>(ConfigKey.WindowPositionXAxisKey);
                int? windowPositionYAxis = LocalSettingsService.ReadSetting<int?>(ConfigKey.WindowPositionYAxisKey);

                if (isWindowMaximized.HasValue && isWindowMaximized.Value)
                {
                    overlappedPresenter.Maximize();
                }
                else
                {
                    if (windowWidth.HasValue && windowHeight.HasValue && windowPositionXAxis.HasValue && windowPositionYAxis.HasValue)
                    {
                        AppWindow.MoveAndResize(new RectInt32(windowPositionXAxis.Value, windowPositionYAxis.Value, windowWidth.Value, windowHeight.Value));
                    }
                }
            }
            else
            {
                if (overlappedPresenter.IsMinimizable)
                {
                    overlappedPresenter.Restore();
                }
            }

            Activate();
        }

        /// <summary>
        /// 设置窗口的置顶状态
        /// </summary>
        private void SetTopMost()
        {
            overlappedPresenter.IsAlwaysOnTop = TopMostService.TopMostValue;
        }

        /// <summary>
        /// 关闭窗口时保存窗口的大小和位置信息
        /// </summary>
        public void SaveWindowInformation()
        {
            LocalSettingsService.SaveSetting(ConfigKey.IsWindowMaximizedKey, IsWindowMaximized);
            LocalSettingsService.SaveSetting(ConfigKey.WindowWidthKey, AppWindow.Size.Width);
            LocalSettingsService.SaveSetting(ConfigKey.WindowHeightKey, AppWindow.Size.Height);
            LocalSettingsService.SaveSetting(ConfigKey.WindowPositionXAxisKey, AppWindow.Position.X);
            LocalSettingsService.SaveSetting(ConfigKey.WindowPositionYAxisKey, AppWindow.Position.Y);
        }

        #endregion 第七部分：窗口属性设置

        #region 第八部分：窗口过程

        /// <summary>
        /// 应用主窗口消息处理
        /// </summary>
        private IntPtr MainWindowSubClassProc(IntPtr hWnd, WindowMessage Msg, UIntPtr wParam, IntPtr lParam, uint uIdSubclass, IntPtr dwRefData)
        {
            switch (Msg)
            {
                // 当用户按下鼠标左键时，光标位于窗口的非工作区内的消息
                case WindowMessage.WM_NCLBUTTONDOWN:
                    {
                        if (TitlebarMenuFlyout.IsOpen)
                        {
                            TitlebarMenuFlyout.Hide();
                        }
                        break;
                    }
                // 当用户按下鼠标右键并释放时，光标位于窗口的非工作区内的消息
                case WindowMessage.WM_NCRBUTTONUP:
                    {
                        if (wParam is 2 && Content is not null && Content.XamlRoot is not null)
                        {
                            PointInt32 screenPoint = new(lParam.ToInt32() & 0xFFFF, lParam.ToInt32() >> 16);
                            Point localPoint = contentCoordinateConverter.ConvertScreenToLocal(screenPoint);

                            FlyoutShowOptions options = new()
                            {
                                ShowMode = FlyoutShowMode.Standard,
                                Position = new Point(localPoint.X / Content.XamlRoot.RasterizationScale, localPoint.Y / Content.XamlRoot.RasterizationScale)
                            };

                            TitlebarMenuFlyout.ShowAt(Content, options);
                        }
                        return 0;
                    }
                // 选择窗口右键菜单的条目时接收到的消息
                case WindowMessage.WM_SYSCOMMAND:
                    {
                        SYSTEMCOMMAND sysCommand = (SYSTEMCOMMAND)(wParam & 0xFFF0);

                        if (sysCommand is SYSTEMCOMMAND.SC_KEYMENU)
                        {
                            return 0;
                        }
                        break;
                    }
            }
            return Comctl32Library.DefSubclassProc(hWnd, Msg, wParam, lParam);
        }

        #endregion 第八部分：窗口过程

        #region 第九部分：窗口导航方法

        /// <summary>
        /// 页面向前导航
        /// </summary>
        public void NavigateTo(Type navigationPageType, object parameter = null)
        {
            foreach (NavigationModel navigationItem in NavigationItemList)
            {
                if (Equals(navigationItem.NavigationPage, navigationPageType))
                {
                    WindowFrame.Navigate(navigationItem.NavigationPage, parameter);
                    break;
                }
            }
        }

        /// <summary>
        /// 页面向后导航
        /// </summary>
        public void NavigationFrom()
        {
            if (WindowFrame.CanGoBack)
            {
                WindowFrame.GoBack();
            }
        }

        /// <summary>
        /// 获取当前导航到的页
        /// </summary>
        public Type GetCurrentPageType()
        {
            return WindowFrame.CurrentSourcePageType;
        }

        /// <summary>
        /// 检查当前页面是否能向后导航
        /// </summary>
        public bool CanGoBack()
        {
            return WindowFrame.CanGoBack;
        }

        /// <summary>
        /// 获取当前导航控件内容对应的页面
        /// </summary>
        public object GetFrameContent()
        {
            return WindowFrame.Content;
        }

        #endregion 第九部分：窗口导航方法

        #region 第十部分：显示对话框和应用通知

        /// <summary>
        /// 显示内容对话框
        /// </summary>
        public async Task<ContentDialogResult> ShowDialogAsync(ContentDialog contentDialog)
        {
            ContentDialogResult dialogResult = ContentDialogResult.None;
            if (!isDialogOpening && contentDialog is not null && Content is not null)
            {
                isDialogOpening = true;

                try
                {
                    contentDialog.XamlRoot = Content.XamlRoot;
                    dialogResult = await contentDialog.ShowAsync();
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }

                isDialogOpening = false;
            }

            return dialogResult;
        }

        /// <summary>
        /// 使用教学提示显示应用内通知
        /// </summary>
        public async Task ShowNotificationAsync(TeachingTip teachingTip, int duration = 2000)
        {
            if (teachingTip is not null && MainPage.Content is Grid grid)
            {
                try
                {
                    grid.Children.Add(teachingTip);

                    teachingTip.IsOpen = true;
                    await Task.Delay(duration);
                    teachingTip.IsOpen = false;

                    // 应用内通知关闭动画显示耗费 300 ms
                    await Task.Delay(300);
                    grid.Children.Remove(teachingTip);
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            }
        }

        #endregion 第十部分：显示对话框和应用通知

        /// <summary>
        /// 检查网络状态
        /// </summary>
        private void CheckNetwork()
        {
            try
            {
                ConnectionProfile connectionProfile = NetworkInformation.GetInternetConnectionProfile();
                bool isConnected = connectionProfile is not null && connectionProfile.GetNetworkConnectivityLevel() is NetworkConnectivityLevel.InternetAccess;

                if (!isConnected)
                {
                    Task.Run(() =>
                    {
                        // 显示网络连接异常通知
                        AppNotificationBuilder appNotificationBuilder = new();
                        appNotificationBuilder.AddArgument("action", "OpenApp");
                        appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/NetworkError1")));
                        appNotificationBuilder.AddText(string.Format(ResourceService.GetLocalized("Notification/NetworkError2")));
                        AppNotificationButton checkNetWorkConnectionButton = new(ResourceService.GetLocalized("Notification/CheckNetWorkConnection"));
                        checkNetWorkConnectionButton.Arguments.Add("action", "CheckNetWorkConnection");
                        appNotificationBuilder.AddButton(checkNetWorkConnectionButton);
                        AppNotification appNotification = appNotificationBuilder.BuildNotification();
                        ToastNotificationService.Show(appNotification);
                    });
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Network state check failed", e);
            }
        }
    }
}
