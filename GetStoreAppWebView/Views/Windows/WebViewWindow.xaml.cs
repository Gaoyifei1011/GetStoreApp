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
using Microsoft.Windows.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Diagnostics;
using Windows.Graphics;
using Windows.System;
using Windows.UI;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreAppWebView.Views.Windows
{
    /// <summary>
    /// 网页浏览器窗口
    /// </summary>
    public sealed partial class WebViewWindow : Window, INotifyPropertyChanged
    {
        private readonly string TitleString = ResourceService.GetLocalized("WebView/Title");
        private readonly string RunningAdministratorString = ResourceService.GetLocalized("WebView/RunningAdministrator");

        private readonly ContentIsland contentIsland;
        private readonly InputKeyboardSource inputKeyboardSource;
        private readonly InputPointerSource inputPointerSource;
        private readonly ContentCoordinateConverter contentCoordinateConverter;
        private readonly OverlappedPresenter overlappedPresenter;
        private readonly SUBCLASSPROC webViewWindowSubClassProc;

        private bool isDialogOpening;

        private string _windowTitle;

        public string WindowTitle
        {
            get { return _windowTitle; }

            set
            {
                if (!string.Equals(_windowTitle, value))
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

        private bool _canGoBack;

        public bool CanGoBack
        {
            get { return _canGoBack; }

            set
            {
                if (!Equals(_canGoBack, value))
                {
                    _canGoBack = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanGoBack)));
                }
            }
        }

        private bool _canGoForward;

        public bool CanGoForward
        {
            get { return _canGoForward; }

            set
            {
                if (!Equals(_canGoForward, value))
                {
                    _canGoForward = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanGoForward)));
                }
            }
        }

        private bool _isLoading = true;

        public bool IsLoading
        {
            get { return _isLoading; }

            set
            {
                if (!Equals(_isLoading, value))
                {
                    _isLoading = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLoading)));
                }
            }
        }

        private bool _isDownloadClickEnabled;

        public bool IsDownloadClickEnabled
        {
            get { return _isDownloadClickEnabled; }

            set
            {
                if (!Equals(_isDownloadClickEnabled, value))
                {
                    _isDownloadClickEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDownloadClickEnabled)));
                }
            }
        }

        private bool _isEnabled;

        public bool IsEnabled
        {
            get { return _isEnabled; }

            set
            {
                if (!Equals(_isEnabled, value))
                {
                    _isEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnabled)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public WebViewWindow()
        {
            InitializeComponent();

            // 窗口部分初始化
            WindowTitle = RuntimeHelper.IsElevated ? TitleString + RunningAdministratorString : TitleString;
            overlappedPresenter = AppWindow.Presenter as OverlappedPresenter;
            ExtendsContentIntoTitleBar = true;
            AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
            AppWindow.TitleBar.InactiveBackgroundColor = Colors.Transparent;
            AppWindow.TitleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;
            IsWindowMaximized = overlappedPresenter.State is OverlappedPresenterState.Maximized;
            contentCoordinateConverter = ContentCoordinateConverter.CreateForWindowId(AppWindow.Id);
            contentIsland = ContentIsland.FindAllForCompositor(Compositor)[0];
            inputKeyboardSource = InputKeyboardSource.GetForIsland(contentIsland);
            inputPointerSource = InputPointerSource.GetForIsland(contentIsland);

            AppWindow.Changed += OnAppWindowChanged;
            AppWindow.Closing += OnAppWindowClosing;
            contentIsland.StateChanged += OnStateChanged;
            contentIsland.Environment.SettingChanged += OnSettingChanged;
            inputKeyboardSource.SystemKeyDown += OnSystemKeyDown;
            inputPointerSource.PointerReleased += OnPointerReleased;

            // 标题栏和右键菜单设置
            SetClassicMenuTheme((Content as FrameworkElement).ActualTheme);

            // 为应用主窗口添加窗口过程
            webViewWindowSubClassProc = new SUBCLASSPROC(WebViewWindowSubClassProc);
            Comctl32Library.SetWindowSubclass(Win32Interop.GetWindowFromWindowId(AppWindow.Id), webViewWindowSubClassProc, 0, nint.Zero);

            SetWindowTheme();
            SetSystemBackdrop();

            AppWindow.Resize(new SizeInt32(Convert.ToInt32(1000 * contentIsland.RasterizationScale), Convert.ToInt32(700 * contentIsland.RasterizationScale)));

            // 默认直接显示到窗口中间
            if (DisplayArea.GetFromWindowId(AppWindow.Id, DisplayAreaFallback.Nearest) is DisplayArea displayArea && contentIsland is not null)
            {
                RectInt32 workArea = displayArea.WorkArea;
                AppWindow.Move(new PointInt32((workArea.Width - AppWindow.Size.Width) / 2, (workArea.Height - AppWindow.Size.Height) / 2));
            }
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

        #endregion 第一部分：窗口类事件

        #region 第二部分：窗口辅助类挂载的事件

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
            AppWindow.Changed -= OnAppWindowChanged;
            contentIsland.Environment.SettingChanged -= OnSettingChanged;
            inputKeyboardSource.SystemKeyDown -= OnSystemKeyDown;
            inputPointerSource.PointerReleased -= OnPointerReleased;
            Comctl32Library.RemoveWindowSubclass(Win32Interop.GetWindowFromWindowId(AppWindow.Id), webViewWindowSubClassProc, 0);
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
            DispatcherQueue.TryEnqueue(() =>
            {
                if (string.Equals(ThemeService.AppTheme, ThemeService.ThemeList[0]))
                {
                    WindowTheme = Application.Current.RequestedTheme is ApplicationTheme.Light ? ElementTheme.Light : ElementTheme.Dark;
                }

                SetPopupControlTheme(WindowTheme);
            });
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
                    Position = new Point(0, 45),
                    ShowMode = FlyoutShowMode.Standard
                };
                TitlebarMenuFlyout.ShowAt(null, options);
            }
            else if (args.VirtualKey is VirtualKey.F10 && Content is not null && Content.XamlRoot is not null)
            {
                await Task.Delay(50);
                SetPopupControlTheme(WindowTheme);
            }
        }

        /// <summary>
        /// 处理鼠标事件
        /// </summary>
        private async void OnPointerReleased(InputPointerSource sender, PointerEventArgs args)
        {
            if (args.CurrentPoint.Properties.PointerUpdateKind is PointerUpdateKind.RightButtonReleased && Content is not null && Content.XamlRoot is not null)
            {
                await Task.Delay(50);
                SetPopupControlTheme(WindowTheme);
            }
        }

        #endregion 第二部分：窗口辅助类挂载的事件

        #region 第三部分：窗口右键菜单事件

        /// <summary>
        /// 窗口还原
        /// </summary>
        private void OnRestoreClicked(object sender, RoutedEventArgs args)
        {
            User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(AppWindow.Id), WindowMessage.WM_SYSCOMMAND, (nuint)SYSTEMCOMMAND.SC_RESTORE, 0);
        }

        /// <summary>
        /// 窗口移动
        /// </summary>
        private void OnMoveClicked(object sender, RoutedEventArgs args)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem && menuFlyoutItem.Tag is MenuFlyout menuFlyout)
            {
                menuFlyout.Hide();
                User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(AppWindow.Id), WindowMessage.WM_SYSCOMMAND, (nuint)SYSTEMCOMMAND.SC_MOVE, 0);
            }
        }

        /// <summary>
        /// 窗口大小
        /// </summary>
        private void OnSizeClicked(object sender, RoutedEventArgs args)
        {
            if (sender is MenuFlyoutItem menuFlyoutItem && menuFlyoutItem.Tag is MenuFlyout menuFlyout)
            {
                menuFlyout.Hide();
                User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(AppWindow.Id), WindowMessage.WM_SYSCOMMAND, (nuint)SYSTEMCOMMAND.SC_SIZE, 0);
            }
        }

        /// <summary>
        /// 窗口最小化
        /// </summary>
        private void OnMinimizeClicked(object sender, RoutedEventArgs args)
        {
            User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(AppWindow.Id), WindowMessage.WM_SYSCOMMAND, (nuint)SYSTEMCOMMAND.SC_MINIMIZE, 0);
        }

        /// <summary>
        /// 窗口最大化
        /// </summary>
        private void OnMaximizeClicked(object sender, RoutedEventArgs args)
        {
            User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(AppWindow.Id), WindowMessage.WM_SYSCOMMAND, (nuint)SYSTEMCOMMAND.SC_MAXIMIZE, 0);
        }

        /// <summary>
        /// 窗口关闭
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(AppWindow.Id), WindowMessage.WM_SYSCOMMAND, (nuint)SYSTEMCOMMAND.SC_CLOSE, 0);
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
        /// 内容加载完成后触发的事件
        /// </summary>
        private async void OnLoaded(object sender, RoutedEventArgs args)
        {
            CoreWebView2Environment coreWebView2Environment = null;
            if (RuntimeHelper.WebView2Type is WebView2Type.User)
            {
                coreWebView2Environment = await CoreWebView2Environment.CreateWithOptionsAsync(null, null, new()
                {
                    Language = LanguageService.AppLanguage,
                });
            }
            else if (RuntimeHelper.WebView2Type is WebView2Type.System)
            {
                coreWebView2Environment = await CoreWebView2Environment.CreateWithOptionsAsync(Path.Combine(global::Windows.Storage.SystemDataPaths.GetDefault().System, "Microsoft-Edge-WebView"), null, new()
                {
                    Language = LanguageService.AppLanguage,
                });
            }

            if (coreWebView2Environment is not null)
            {
                await WebViewBrowser.EnsureCoreWebView2Async(coreWebView2Environment);
                WebViewBrowser.CoreWebView2.Navigate("https://apps.microsoft.com");
            }
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
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new Uri("https://apps.microsoft.com"));
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
        }

        /// <summary>
        /// 打开缓存文件夹
        /// </summary>
        private void OnOpenCacheFolderClicked(object sender, RoutedEventArgs args)
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
        ///  清理网页缓存
        /// </summary>
        private async void OnClearWebCacheClicked(object sender, RoutedEventArgs args)
        {
            if (WebViewBrowser is not null && WebViewBrowser.CoreWebView2 is not null)
            {
                WebViewBrowser.CoreWebView2.CookieManager.DeleteAllCookies();
                await WebViewBrowser.CoreWebView2.Profile.ClearBrowsingDataAsync();
                await WebViewBrowser.CoreWebView2.ClearServerCertificateErrorActionsAsync();
            }
        }

        /// <summary>
        /// 打开设置
        /// </summary>
        private void OnOpenSettingsClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new Uri("getstoreapp:"), new LauncherOptions() { TargetApplicationPackageFamilyName = Package.Current.Id.FamilyName }, new ValueSet()
                    {
                        { "Parameter", "Settings" }
                    });
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            });
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
            Dictionary<string, string> logInformationDict = new()
            {
                { "Process failed kind", Convert.ToString(args.ProcessFailedKind) },
                { "Reason", Convert.ToString(args.Reason) },
                { "Exit code", Convert.ToString(args.ExitCode) },
                { "Process description", args.ProcessDescription },
            };

            LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreAppWebView), nameof(WebViewWindow), nameof(OnCoreProcessFailed), 3, logInformationDict);
            await ShowDialogAsync(new ProcessFailedDialog());
            (Application.Current as WebViewApp).Dispose();
        }

        /// <summary>
        /// 初始化 CoreWebView2 对象
        /// </summary>
        private void OnCoreWebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            if (WebViewBrowser.CoreWebView2 is not null)
            {
                WebViewBrowser.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = false;
                WebViewBrowser.CoreWebView2.Settings.AreDevToolsEnabled = false;
                WebViewBrowser.CoreWebView2.NewWindowRequested += OnCoreWebViewNewWindowRequested;
                WebViewBrowser.CoreWebView2.SourceChanged += OnSourceChanged;
                IsEnabled = true;
            }
        }

        /// <summary>
        /// 页面开始导航
        /// </summary>
        private void OnWebView2NavigationStarting(WebView2 sender, CoreWebView2NavigationStartingEventArgs args)
        {
            IsLoading = true;
        }

        /// <summary>
        /// 页面完成导航
        /// </summary>
        private void OnWebView2NavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            IsLoading = false;
            WindowTitle = string.Format("{0} - {1}", WebViewBrowser.CoreWebView2.DocumentTitle, TitleString);
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

        #endregion 第四部分：窗口内容挂载的事件

        #region 第五部分：窗口及内容属性设置

        /// <summary>
        /// 设置应用显示的主题
        /// </summary>
        public void SetWindowTheme()
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
        /// 设置所有弹出控件主题
        /// </summary>
        private void SetPopupControlTheme(ElementTheme elementTheme)
        {
            foreach (Popup popup in VisualTreeHelper.GetOpenPopupsForXamlRoot(Content.XamlRoot))
            {
                popup.RequestedTheme = elementTheme;

                if (popup.Child is FlyoutPresenter flyoutPresenter)
                {
                    flyoutPresenter.RequestedTheme = elementTheme;
                }

                if (popup.Child is Grid grid && grid.Name is "OuterOverflowContentRootV2")
                {
                    grid.RequestedTheme = elementTheme;
                }
            }
        }

        #endregion 第五部分：窗口及内容属性设置

        #region 第六部分：窗口过程

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
                                Position = Environment.OSVersion.Version.Build > 22000 ? new Point(localPoint.X / Content.XamlRoot.RasterizationScale, localPoint.Y / Content.XamlRoot.RasterizationScale) : new Point(localPoint.X, localPoint.Y)
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

        #endregion 第六部分：窗口过程

        #region 第七部分：显示对话框和应用通知

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

        #endregion 第七部分：显示对话框和应用通知
    }
}
