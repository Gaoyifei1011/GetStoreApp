using GetStoreAppWebView.Extensions.Backdrop;
using GetStoreAppWebView.Extensions.DataType.Enums;
using GetStoreAppWebView.Helpers.Root;
using GetStoreAppWebView.Services.Root;
using GetStoreAppWebView.Services.Settings;
using GetStoreAppWebView.Views.Dialogs;
using GetStoreAppWebView.WindowsAPI.PInvoke.Comctl32;
using GetStoreAppWebView.WindowsAPI.PInvoke.User32;
using GetStoreAppWebView.WindowsAPI.PInvoke.Uxtheme;
using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Content;
using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.Web.WebView2.Core;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Diagnostics;
using Windows.Graphics;
using Windows.System;
using Windows.UI;
using WinRT;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreAppWebView.Views.Windows
{
    /// <summary>
    /// 网页浏览器窗口
    /// </summary>
    internal sealed partial class WebViewWindow : Window, INotifyPropertyChanged
    {
        #region 第一部分：常量、资源与状态字段

        private readonly string TitleString = ResourceService.GetLocalized("WebView/Title");
        private readonly string WebTitleString = ResourceService.GetLocalized("WebView/WebTitle");
        private readonly string RunningAdministratorString = ResourceService.GetLocalized("WebView/RunningAdministrator");
        private ContentIsland contentIsland;
        private InputKeyboardSource inputKeyboardSource;
        private ContentCoordinateConverter contentCoordinateConverter;
        private OverlappedPresenter overlappedPresenter;
        private SUBCLASSPROC webViewWindowSubClassProc;

        #endregion 第一部分：常量、资源与状态字段

        #region 第二部分：属性、集合与事件

        private string _windowTitle;

        private string WindowTitle
        {
            get { return _windowTitle; }

            set
            {
                if (!string.Equals(_windowTitle, value))
                {
                    _windowTitle = value;
                    PropertyChanged?.Invoke(this, new(nameof(WindowTitle)));
                }
            }
        }

        private SystemBackdrop _windowSystemBackdrop;

        private SystemBackdrop WindowSystemBackdrop
        {
            get { return _windowSystemBackdrop; }

            set
            {
                if (!Equals(_windowSystemBackdrop, value))
                {
                    _windowSystemBackdrop = value;
                    PropertyChanged?.Invoke(this, new(nameof(WindowSystemBackdrop)));
                }
            }
        }

        private ElementTheme _windowTheme;

        private ElementTheme WindowTheme
        {
            get { return _windowTheme; }

            set
            {
                if (!Equals(_windowTheme, value))
                {
                    _windowTheme = value;
                    PropertyChanged?.Invoke(this, new(nameof(WindowTheme)));
                }
            }
        }

        private bool _isWindowMaximized;

        private bool IsWindowMaximized
        {
            get { return _isWindowMaximized; }

            set
            {
                if (!Equals(_isWindowMaximized, value))
                {
                    _isWindowMaximized = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsWindowMaximized)));
                }
            }
        }

        private bool _canGoBack;

        private bool CanGoBack
        {
            get { return _canGoBack; }

            set
            {
                if (!Equals(_canGoBack, value))
                {
                    _canGoBack = value;
                    PropertyChanged?.Invoke(this, new(nameof(CanGoBack)));
                }
            }
        }

        private bool _canGoForward;

        private bool CanGoForward
        {
            get { return _canGoForward; }

            set
            {
                if (!Equals(_canGoForward, value))
                {
                    _canGoForward = value;
                    PropertyChanged?.Invoke(this, new(nameof(CanGoForward)));
                }
            }
        }

        private bool _isLoading;

        private bool IsLoading
        {
            get { return _isLoading; }

            set
            {
                if (!Equals(_isLoading, value))
                {
                    _isLoading = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsLoading)));
                }
            }
        }

        private string _webTitle;

        private string WebTitle
        {
            get { return _webTitle; }

            set
            {
                if (!string.Equals(_webTitle, value))
                {
                    _webTitle = value;
                    PropertyChanged?.Invoke(this, new(nameof(WebTitle)));
                }
            }
        }

        private bool _isDownloadClickEnabled;

        private bool IsDownloadClickEnabled
        {
            get { return _isDownloadClickEnabled; }

            set
            {
                if (!Equals(_isDownloadClickEnabled, value))
                {
                    _isDownloadClickEnabled = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsDownloadClickEnabled)));
                }
            }
        }

        private bool _isEnabled;

        private bool IsEnabled
        {
            get { return _isEnabled; }

            set
            {
                if (!Equals(_isEnabled, value))
                {
                    _isEnabled = value;
                    PropertyChanged?.Invoke(this, new(nameof(IsEnabled)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion 第二部分：属性、集合与事件

        #region 第三部分：构造函数

        [DynamicWindowsRuntimeCast(typeof(FrameworkElement))]
        internal WebViewWindow()
        {
            InitializeComponent();
            InitializeWindowData(AppWindow);
            MountWindowEvent();
            MountWindowWndProc(AppWindow.Id);
            SetWindowTheme();
            SetSystemBackdrop();
            SetWindowSize(AppWindow);
            SetWindowPosition(AppWindow);
            SetClassicMenuTheme(AppWindow.TitleBar, (Content as FrameworkElement).ActualTheme);
            InitializeWebView();
        }

        #endregion 第三部分：构造函数

        #region 第四部分：父类虚方法重写

        /// <summary>
        /// 窗口激活状态发生变化的事件
        /// </summary>
        private void OnActivated(object sender, WindowActivatedEventArgs args)
        {
            try
            {
                if (contentIsland is not null && !contentIsland.IsClosed && WindowSystemBackdrop is MaterialBackdrop materialBackdrop && materialBackdrop.BackdropConfiguration is not null)
                {
                    materialBackdrop.BackdropConfiguration.IsInputActive = AlwaysShowBackdropService.AlwaysShowBackdrop || args.WindowActivationState is not WindowActivationState.Deactivated;
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

            if (contentIsland is not null)
            {
                overlappedPresenter.PreferredMinimumWidth = Convert.ToInt32(800 * contentIsland.RasterizationScale);
                overlappedPresenter.PreferredMinimumHeight = Convert.ToInt32(560 * contentIsland.RasterizationScale);
            }
        }

        /// <summary>
        /// 窗口位置变化发生的事件
        /// </summary>
        private void OnAppWindowChanged(AppWindow sender, AppWindowChangedEventArgs args)
        {
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
            }
        }

        /// <summary>
        /// 关闭窗口之后关闭其他服务
        /// </summary>
        private void OnAppWindowClosing(AppWindow sender, AppWindowClosingEventArgs args)
        {
            WebViewBrowser?.Close();
            DismountWindowEvent();
            DismountWindowWndProc(sender.Id);
            (Application.Current as WebViewApp).Dispose();
        }

        /// <summary>
        /// 内容岛状态发生更改时触发的事件
        /// </summary>
        private void OnStateChanged(ContentIsland sender, ContentIslandStateChangedEventArgs args)
        {
            if (args.DidRasterizationScaleChange)
            {
                overlappedPresenter.PreferredMinimumWidth = Convert.ToInt32(1000 * contentIsland.RasterizationScale);
                overlappedPresenter.PreferredMinimumHeight = Convert.ToInt32(600 * contentIsland.RasterizationScale);
            }
        }

        /// <summary>
        /// 内容岛设置发生更改时触发的事件
        /// </summary>
        private void OnSettingChanged(ContentIslandEnvironment sender, ContentEnvironmentSettingChangedEventArgs args)
        {
            DispatcherQueue.TryEnqueue(SetWindowTheme);
        }

        /// <summary>
        /// 处理键盘系统按键事件
        /// </summary>
        private async void OnSystemKeyDown(InputKeyboardSource sender, KeyEventArgs args)
        {
            if (args.KeyStatus.IsMenuKeyDown && args.VirtualKey is VirtualKey.Space)
            {
                args.Handled = true;
                FlyoutShowOptions options = new()
                {
                    Position = new(0, 45),
                    ShowMode = FlyoutShowMode.Standard
                };
                TitlebarMenuFlyout.ShowAt(null, options);
            }
        }

        /// <summary>
        /// 窗口还原
        /// </summary>
        private void OnRestoreClicked(object sender, RoutedEventArgs args)
        {
            SendWindowMessage(AppWindow.Id, WindowMessageKind.Restore);
        }

        /// <summary>
        /// 窗口移动
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(MenuFlyout)), DynamicWindowsRuntimeCast(typeof(MenuFlyoutItem))]
        private void OnMoveClicked(object sender, RoutedEventArgs args)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem && menuFlyoutItem.Tag is MenuFlyout menuFlyout)
            {
                menuFlyout.Hide();
                SendWindowMessage(AppWindow.Id, WindowMessageKind.Move);
            }
        }

        /// <summary>
        /// 窗口大小
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(MenuFlyout)), DynamicWindowsRuntimeCast(typeof(MenuFlyoutItem))]
        private void OnSizeClicked(object sender, RoutedEventArgs args)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem && menuFlyoutItem.Tag is MenuFlyout menuFlyout)
            {
                menuFlyout.Hide();
                SendWindowMessage(AppWindow.Id, WindowMessageKind.Size);
            }
        }

        /// <summary>
        /// 窗口最小化
        /// </summary>
        private void OnMinimizeClicked(object sender, RoutedEventArgs args)
        {
            SendWindowMessage(AppWindow.Id, WindowMessageKind.Minimize);
        }

        /// <summary>
        /// 窗口最大化
        /// </summary>
        private void OnMaximizeClicked(object sender, RoutedEventArgs args)
        {
            SendWindowMessage(AppWindow.Id, WindowMessageKind.Maximize);
        }

        /// <summary>
        /// 窗口关闭
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            SendWindowMessage(AppWindow.Id, WindowMessageKind.Close);
        }

        /// <summary>
        /// 应用主题变化时设置标题栏按钮的颜色
        /// </summary>
        private void OnActualThemeChanged(FrameworkElement sender, object args)
        {
            SetTitleBarTheme(AppWindow.TitleBar, sender.ActualTheme);
            SetClassicMenuTheme(AppWindow.TitleBar, sender.ActualTheme);
        }

        /// <summary>
        /// 内容加载完成后触发的事件
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(FrameworkElement))]
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            // 设置标题栏主题
            SetTitleBarTheme(AppWindow.TitleBar, (Content as FrameworkElement).ActualTheme);
        }

        /// <summary>
        /// 网页后退
        /// </summary>
        private void OnBrowserBackClicked(object sender, RoutedEventArgs args)
        {
            if (WebViewBrowser is not null && WebViewBrowser.CanGoBack)
            {
                WebViewBrowser.GoBack();
            }
        }

        /// <summary>
        /// 网页前进
        /// </summary>
        private void OnBrowserForwardClicked(object sender, RoutedEventArgs args)
        {
            if (WebViewBrowser is not null && WebViewBrowser.CanGoForward)
            {
                WebViewBrowser.GoForward();
            }
        }

        /// <summary>
        /// 网页刷新
        /// </summary>
        private void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            WebViewBrowser?.Reload();
        }

        /// <summary>
        /// 导航到主页
        /// </summary>
        private void OnHomeClicked(object sender, RoutedEventArgs args)
        {
            WebViewBrowser?.CoreWebView2.Navigate("https://apps.microsoft.com");
        }

        /// <summary>
        /// 打开下载窗口
        /// </summary>
        private void OnDownloadClicked(object sender, RoutedEventArgs args)
        {
            if (WebViewBrowser is not null && WebViewBrowser.CoreWebView2 is not null)
            {
                WebViewBrowser.CoreWebView2.OpenDefaultDownloadDialog();
            }
        }

        /// <summary>
        /// 在浏览器中打开
        /// </summary>
        private void OnOpenWithBrowserClicked(object sender, RoutedEventArgs args)
        {
            OpenWithBrowser();
        }

        /// <summary>
        /// 打开缓存文件夹
        /// </summary>
        private void OnOpenCacheFolderClicked(object sender, RoutedEventArgs args)
        {
            OpenCacheFolder();
        }

        /// <summary>
        ///  清理网页缓存
        /// </summary>
        private async void OnClearWebCacheClicked(object sender, RoutedEventArgs args)
        {
            if (WebViewBrowser is not null && WebViewBrowser.CoreWebView2 is not null)
            {
                await ClearWebViewCacheAsync(WebViewBrowser.CoreWebView2);
            }
        }

        /// <summary>
        /// 打开设置
        /// </summary>
        private void OnOpenSettingsClicked(object sender, RoutedEventArgs args)
        {
            OpenSettings();
        }

        /// <summary>
        /// 查看浏览器内核信息
        /// </summary>
        private async void OnBrowserInformationClicked(object sender, RoutedEventArgs args)
        {
            if (WebViewBrowser is not null && WebViewBrowser.CoreWebView2 is not null)
            {
                await ShowDialogAsync(new BrowserInformationDialog(WebViewBrowser.CoreWebView2.Environment.BrowserVersionString));
            }
        }

        /// <summary>
        /// 进程异常退出时触发的事件
        /// </summary>
        private async void OnCoreProcessFailed(WebView2 sender, CoreWebView2ProcessFailedEventArgs args)
        {
            Dictionary<string, string> logInformationDict = GetLogInformationDict(args.ProcessDescription, args.Reason, args.ExitCode, args.ProcessDescription);
            if (logInformationDict is null)
            {
                return;
            }

            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreAppWebView), nameof(WebViewWindow), nameof(OnCoreProcessFailed), 3, logInformationDict);
            await ShowDialogAsync(new ProcessFailedDialog());
            (Application.Current as WebViewApp).Dispose();
        }

        /// <summary>
        /// 初始化 CoreWebView2 对象
        /// </summary>
        private void OnCoreWebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            if (WebViewBrowser.CoreWebView2 is null)
            {
                return;
            }

            WebViewBrowser.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = false;
            WebViewBrowser.CoreWebView2.Settings.AreDevToolsEnabled = false;
            WebViewBrowser.CoreWebView2.NewWindowRequested += OnCoreWebViewNewWindowRequested;
            WebViewBrowser.CoreWebView2.SourceChanged += OnSourceChanged;
            IsEnabled = true;
        }

        /// <summary>
        /// 页面开始导航
        /// </summary>
        private void OnWebView2NavigationStarting(WebView2 sender, CoreWebView2NavigationStartingEventArgs args)
        {
            IsLoading = true;
            if (string.IsNullOrEmpty(sender.CoreWebView2.DocumentTitle))
            {
                WebTitle = WebTitleString;
                WindowTitle = TitleString;
            }
            else
            {
                WebTitle = sender.CoreWebView2.DocumentTitle;
                WindowTitle = string.Format("{0} - {1}", sender.CoreWebView2.DocumentTitle, TitleString);
            }
        }

        /// <summary>
        /// 页面完成导航
        /// </summary>
        private void OnWebView2NavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            IsLoading = false;
            if (string.IsNullOrEmpty(sender.CoreWebView2.DocumentTitle))
            {
                WebTitle = WebTitleString;
                WindowTitle = TitleString;
            }
            else
            {
                WebTitle = sender.CoreWebView2.DocumentTitle;
                WindowTitle = string.Format("{0} - {1}", sender.CoreWebView2.DocumentTitle, TitleString);
            }
        }

        /// <summary>
        /// 当前页面对应的链接发生改变时触发这一事件
        /// </summary>
        private void OnSourceChanged(CoreWebView2 sender, CoreWebView2SourceChangedEventArgs args)
        {
            CanGoBack = sender.CanGoBack;
            CanGoForward = sender.CanGoForward;
        }

        /// <summary>
        /// 捕捉打开新窗口事件，并禁止弹窗
        /// </summary>
        private void OnCoreWebViewNewWindowRequested(CoreWebView2 sender, CoreWebView2NewWindowRequestedEventArgs args)
        {
            args.Handled = true;
            sender.Navigate(args.Uri);
        }

        #endregion 第四部分：父类虚方法重写

        #region 第五部分：数据操作与业务逻辑

        /// <summary>
        /// 发送窗口消息类型
        /// </summary>
        private void SendWindowMessage(Microsoft.UI.WindowId windowId, WindowMessageKind windowMessageKind)
        {
            switch (windowMessageKind)
            {
                case WindowMessageKind.Move:
                    {
                        User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(windowId), WindowMessage.WM_SYSCOMMAND, (nuint)SYSTEMCOMMAND.SC_MOVE, 0);
                        break;
                    }
                case WindowMessageKind.Size:
                    {
                        User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(windowId), WindowMessage.WM_SYSCOMMAND, (nuint)SYSTEMCOMMAND.SC_SIZE, 0);
                        break;
                    }
                case WindowMessageKind.Minimize:
                    {
                        User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(windowId), WindowMessage.WM_SYSCOMMAND, (nuint)SYSTEMCOMMAND.SC_MINIMIZE, 0);
                        break;
                    }
                case WindowMessageKind.Maximize:
                    {
                        User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(windowId), WindowMessage.WM_SYSCOMMAND, (nuint)SYSTEMCOMMAND.SC_MAXIMIZE, 0);
                        break;
                    }
                case WindowMessageKind.Restore:
                    {
                        User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(windowId), WindowMessage.WM_SYSCOMMAND, (nuint)SYSTEMCOMMAND.SC_RESTORE, 0);
                        break;
                    }
                case WindowMessageKind.Close:
                    {
                        User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(windowId), WindowMessage.WM_SYSCOMMAND, (nuint)SYSTEMCOMMAND.SC_CLOSE, 0);
                        break;
                    }
                default:
                    break;
            }
        }

        /// <summary>
        /// 初始化窗口数据
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(OverlappedPresenter))]
        private void InitializeWindowData(AppWindow appWindow)
        {
            WebTitle = WebTitleString;
            WindowTitle = RuntimeHelper.IsElevated ? TitleString + RunningAdministratorString : TitleString;
            overlappedPresenter = appWindow.Presenter as OverlappedPresenter;
            ExtendsContentIntoTitleBar = true;
            appWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
            appWindow.TitleBar.InactiveBackgroundColor = Colors.Transparent;
            appWindow.TitleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;
            IsWindowMaximized = overlappedPresenter.State is OverlappedPresenterState.Maximized;
            contentCoordinateConverter = ContentCoordinateConverter.CreateForWindowId(appWindow.Id);
            contentIsland = ContentIsland.FindAllForCompositor(Compositor)[0];
            inputKeyboardSource = InputKeyboardSource.GetForIsland(contentIsland);
        }

        /// <summary>
        /// 挂载窗口事件
        /// </summary>
        private void MountWindowEvent()
        {
            AppWindow.Changed += OnAppWindowChanged;
            AppWindow.Closing += OnAppWindowClosing;
            contentIsland.StateChanged += OnStateChanged;
            contentIsland.Environment.SettingChanged += OnSettingChanged;
            inputKeyboardSource.SystemKeyDown += OnSystemKeyDown;
        }

        /// <summary>
        /// 卸载窗口事件
        /// </summary>
        private void DismountWindowEvent()
        {
            AppWindow.Changed -= OnAppWindowChanged;
            contentIsland.Environment.SettingChanged -= OnSettingChanged;
            inputKeyboardSource.SystemKeyDown -= OnSystemKeyDown;
        }

        /// <summary>
        /// 挂载窗口进程
        /// </summary>
        private void MountWindowWndProc(Microsoft.UI.WindowId windowId)
        {
            webViewWindowSubClassProc = new(WebViewWindowSubClassProc);
            Comctl32Library.SetWindowSubclass(Win32Interop.GetWindowFromWindowId(windowId), webViewWindowSubClassProc, 0, nint.Zero);
        }

        /// <summary>
        /// 卸载窗口进程
        /// </summary>
        private void DismountWindowWndProc(Microsoft.UI.WindowId windowId)
        {
            Comctl32Library.RemoveWindowSubclass(Win32Interop.GetWindowFromWindowId(windowId), webViewWindowSubClassProc, 0);
        }

        /// <summary>
        /// 设置应用显示的主题
        /// </summary>
        private void SetWindowTheme()
        {
            WindowTheme = string.Equals(ThemeService.AppTheme, ThemeService.ThemeList[0]) ? Application.Current.RequestedTheme is ApplicationTheme.Light ? ElementTheme.Light : ElementTheme.Dark : Enum.TryParse(ThemeService.AppTheme, out ElementTheme elementTheme) ? elementTheme : ElementTheme.Default;
        }

        /// <summary>
        /// 设置应用的背景色
        /// </summary>
        private void SetSystemBackdrop()
        {
            if (string.Equals(BackdropService.AppBackdrop, BackdropService.BackdropList[1]))
            {
                WindowSystemBackdrop = new MaterialBackdrop(MicaKind.Base);
                VisualStateManager.GoToState(WebViewPage, "BackgroundTransparent", false);
            }
            else if (string.Equals(BackdropService.AppBackdrop, BackdropService.BackdropList[2]))
            {
                WindowSystemBackdrop = new MaterialBackdrop(MicaKind.BaseAlt);
                VisualStateManager.GoToState(WebViewPage, "BackgroundTransparent", false);
            }
            else if (string.Equals(BackdropService.AppBackdrop, BackdropService.BackdropList[3]))
            {
                WindowSystemBackdrop = new MaterialBackdrop(DesktopAcrylicKind.Default);
                VisualStateManager.GoToState(WebViewPage, "BackgroundTransparent", false);
            }
            else if (string.Equals(BackdropService.AppBackdrop, BackdropService.BackdropList[4]))
            {
                WindowSystemBackdrop = new MaterialBackdrop(DesktopAcrylicKind.Base);
                VisualStateManager.GoToState(WebViewPage, "BackgroundTransparent", false);
            }
            else if (string.Equals(BackdropService.AppBackdrop, BackdropService.BackdropList[5]))
            {
                WindowSystemBackdrop = new MaterialBackdrop(DesktopAcrylicKind.Thin);
                VisualStateManager.GoToState(WebViewPage, "BackgroundTransparent", false);
            }
            else
            {
                WindowSystemBackdrop = null;
                VisualStateManager.GoToState(WebViewPage, "BackgroundDefault", false);
            }
        }

        /// <summary>
        /// 设置窗口大小
        /// </summary>
        private void SetWindowSize(AppWindow appWindow)
        {
            appWindow.Resize(new(Convert.ToInt32(1000 * contentIsland.RasterizationScale), Convert.ToInt32(700 * contentIsland.RasterizationScale)));
        }

        /// <summary>
        /// 设置窗口大小和位置
        /// </summary>
        private void SetWindowPosition(AppWindow appWindow)
        {
            // 默认直接显示到窗口中间
            if (DisplayArea.GetFromWindowId(appWindow.Id, DisplayAreaFallback.Nearest) is DisplayArea displayArea && contentIsland is not null)
            {
                RectInt32 workArea = displayArea.WorkArea;
                appWindow.Move(new((workArea.Width - appWindow.Size.Width) / 2, (workArea.Height - appWindow.Size.Height) / 2));
            }
        }

        /// <summary>
        /// 设置标题栏按钮的主题色
        /// </summary>
        private void SetTitleBarTheme(AppWindowTitleBar appWindowTitleBar, ElementTheme theme)
        {
            if (appWindowTitleBar is null)
            {
                return;
            }

            appWindowTitleBar.BackgroundColor = Colors.Transparent;
            appWindowTitleBar.ForegroundColor = Colors.Transparent;
            appWindowTitleBar.InactiveBackgroundColor = Colors.Transparent;
            appWindowTitleBar.InactiveForegroundColor = Colors.Transparent;
            appWindowTitleBar.ButtonBackgroundColor = Colors.Transparent;
            appWindowTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            if (theme is ElementTheme.Light)
            {
                appWindowTitleBar.ButtonForegroundColor = Color.FromArgb(255, 23, 23, 23);
                appWindowTitleBar.ButtonHoverBackgroundColor = Color.FromArgb(25, 0, 0, 0);
                appWindowTitleBar.ButtonHoverForegroundColor = Colors.Black;
                appWindowTitleBar.ButtonPressedBackgroundColor = Color.FromArgb(51, 0, 0, 0);
                appWindowTitleBar.ButtonPressedForegroundColor = Colors.Black;
                appWindowTitleBar.ButtonInactiveForegroundColor = Color.FromArgb(255, 153, 153, 153);
            }
            else
            {
                appWindowTitleBar.ButtonForegroundColor = Color.FromArgb(255, 242, 242, 242);
                appWindowTitleBar.ButtonHoverBackgroundColor = Color.FromArgb(25, 255, 255, 255);
                appWindowTitleBar.ButtonHoverForegroundColor = Colors.White;
                appWindowTitleBar.ButtonPressedBackgroundColor = Color.FromArgb(51, 255, 255, 255);
                appWindowTitleBar.ButtonPressedForegroundColor = Colors.White;
                appWindowTitleBar.ButtonInactiveForegroundColor = Color.FromArgb(255, 102, 102, 102);
            }
        }

        /// <summary>
        /// 初始化窗口浏览器
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ProtocolActivatedEventArgs))]
        private void InitializeWebView()
        {
            DispatcherQueue.TryEnqueue(async () =>
            {
                IsLoading = true;
                CoreWebView2Environment coreWebView2Environment = null;
                if (RuntimeHelper.WebView2Type is WebView2Type.User)
                {
                    coreWebView2Environment = await CoreWebView2Environment.CreateWithOptionsAsync(null, null, new()
                    {
                        Language = LanguageService.AppLanguage,
                        ScrollBarStyle = CoreWebView2ScrollbarStyle.FluentOverlay
                    });
                }
                else if (RuntimeHelper.WebView2Type is WebView2Type.System)
                {
                    coreWebView2Environment = await CoreWebView2Environment.CreateWithOptionsAsync(Path.Combine(global::Windows.Storage.SystemDataPaths.GetDefault().System, "Microsoft-Edge-WebView"), null, new()
                    {
                        Language = LanguageService.AppLanguage,
                        ScrollBarStyle = CoreWebView2ScrollbarStyle.FluentOverlay
                    });
                }

                if (coreWebView2Environment is null)
                {
                    return;
                }

                await WebViewBrowser.EnsureCoreWebView2Async(coreWebView2Environment);
                if (Program.AppActivationArguments.Kind is ExtendedActivationKind.Protocol)
                {
                    ProtocolActivatedEventArgs protocolActivatedEventArgs = Program.AppActivationArguments.Data as ProtocolActivatedEventArgs;
                    if (protocolActivatedEventArgs.Data is ValueSet protocolData && protocolData.TryGetValue("AppLink", out object appLinkObj) && appLinkObj is string appLink && !string.IsNullOrEmpty(appLink))
                    {
                        WebViewBrowser.CoreWebView2.Navigate(appLink);
                    }
                    else
                    {
                        WebViewBrowser.CoreWebView2.Navigate("https://apps.microsoft.com");
                    }
                }
                else
                {
                    WebViewBrowser.CoreWebView2.Navigate("https://apps.microsoft.com");
                }

                CoreWebView2Profile coreWebView2Profile = WebViewBrowser.CoreWebView2.Profile;
                coreWebView2Profile.DefaultDownloadFolderPath = DownloadOptionsService.DownloadFolder;
            });
        }

        /// <summary>
        /// 设置传统菜单标题栏按钮的主题色
        /// </summary>
        private void SetClassicMenuTheme(AppWindowTitleBar appWindowTitleBar, ElementTheme theme)
        {
            if (appWindowTitleBar is null)
            {
                return;
            }

            if (theme is ElementTheme.Light)
            {
                appWindowTitleBar.PreferredTheme = TitleBarTheme.Light;
                UxthemeLibrary.SetPreferredAppMode(PreferredAppMode.ForceLight);
            }
            else
            {
                appWindowTitleBar.PreferredTheme = TitleBarTheme.Dark;
                UxthemeLibrary.SetPreferredAppMode(PreferredAppMode.ForceDark);
            }

            UxthemeLibrary.FlushMenuThemes();
        }

        /// <summary>
        /// 应用主窗口消息处理
        /// </summary>
        private nint WebViewWindowSubClassProc(nint hWnd, WindowMessage Msg, nuint wParam, nint lParam, uint uIdSubclass, nint dwRefData)
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
                                Position = Environment.OSVersion.Version.Build > 22000 ? new(localPoint.X / Content.XamlRoot.RasterizationScale, localPoint.Y / Content.XamlRoot.RasterizationScale) : new(localPoint.X, localPoint.Y)
                            };

                            TitlebarMenuFlyout.ShowAt(Content, options);
                        }
                        return 0;
                    }
                // 选择窗口右键菜单的条目时接收到的消息
                case WindowMessage.WM_SYSCOMMAND:
                    {
                        SYSTEMCOMMAND sysCommand = (SYSTEMCOMMAND)(wParam & 0xFFF0);

                        if (sysCommand is SYSTEMCOMMAND.SC_KEYMENU && lParam is (nint)VirtualKey.Space)
                        {
                            return 0;
                        }
                        break;
                    }
            }

            return Comctl32Library.DefSubclassProc(hWnd, Msg, wParam, lParam);
        }

        /// <summary>
        /// 显示内容对话框
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(ContentDialog))]
        internal async Task<ContentDialogResult> ShowDialogAsync(ContentDialog contentDialog)
        {
            ContentDialogResult dialogResult = ContentDialogResult.None;
            bool isDialogOpening = false;
            if (contentDialog is not null && Content is not null)
            {
                foreach (Popup popup in VisualTreeHelper.GetOpenPopupsForXamlRoot(Content.XamlRoot))
                {
                    if (popup.Child is ContentDialog)
                    {
                        isDialogOpening = true;
                        break;
                    }
                }

                if (!isDialogOpening)
                {
                    try
                    {
                        contentDialog.XamlRoot = Content.XamlRoot;
                        dialogResult = await contentDialog.ShowAsync();
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreAppWebView), nameof(WebViewWindow), nameof(ShowDialogAsync), 1, e);
                    }
                }
            }

            return dialogResult;
        }

        /// <summary>
        /// 使用浏览器打开
        /// </summary>
        private void OpenWithBrowser()
        {
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new("https://apps.microsoft.com"));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 打开缓存目录
        /// </summary>
        private void OpenCacheFolder()
        {
            Task.Run(async () =>
            {
                try
                {
                    if (Directory.Exists(Path.Combine(ApplicationData.GetDefault().LocalFolder.Path, "EbWebView")))
                    {
                        await Launcher.LaunchFolderPathAsync(Path.Combine(ApplicationData.GetDefault().LocalFolder.Path, "EbWebView"));
                    }
                    else
                    {
                        await Launcher.LaunchFolderAsync(ApplicationData.GetDefault().LocalFolder);
                    }
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 清空浏览器缓存
        /// </summary>
        private async Task ClearWebViewCacheAsync(CoreWebView2 coreWebView2)
        {
            if (coreWebView2 is null)
            {
                return;
            }

            coreWebView2.CookieManager.DeleteAllCookies();
            await coreWebView2.Profile.ClearBrowsingDataAsync(CoreWebView2BrowsingDataKinds.AllProfile | CoreWebView2BrowsingDataKinds.AllSite | CoreWebView2BrowsingDataKinds.AllDomStorage);
            await coreWebView2.ClearServerCertificateErrorActionsAsync();
        }

        /// <summary>
        /// 打开设置
        /// </summary>
        private void OpenSettings()
        {
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new("getstoreapp:"), new() { TargetApplicationPackageFamilyName = Package.Current.Id.FamilyName }, new()
                    {
                        { "Parameter", "DownloadSettings" }
                    });
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 获取异常信息
        /// </summary>
        private Dictionary<string, string> GetLogInformationDict(string processFailedKind, CoreWebView2ProcessFailedReason reason, int exitCode, string processDescription)
        {
            return new()
            {
                { "Process failed kind", processFailedKind },
                { "Reason", Convert.ToString(reason) },
                { "Exit code", Convert.ToString(exitCode) },
                { "Process description", processDescription },
            };
        }

        #endregion 第五部分：数据操作与业务逻辑
    }
}
