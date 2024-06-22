using GetStoreApp.WindowsAPI.PInvoke.User32;
using GetStoreAppWebView.Helpers.Controls.Extensions;
using GetStoreAppWebView.Services.Controls.Settings;
using GetStoreAppWebView.Services.Root;
using GetStoreAppWebView.UI.Dialogs.Common;
using GetStoreAppWebView.WindowsAPI.PInvoke.Comctl32;
using GetStoreAppWebView.WindowsAPI.PInvoke.User32;
using GetStoreAppWebView.WindowsAPI.PInvoke.Uxtheme;
using Microsoft.UI;
using Microsoft.UI.Content;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;
using Windows.Graphics;
using Windows.Storage;
using Windows.System;
using Windows.UI;
using Windows.Web.UI;
using Windows.Web.UI.Interop;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace GetStoreAppWebView.Windows
{
    /// <summary>
    /// 网页浏览器窗口
    /// </summary>
    public sealed partial class WebWindow : Window, INotifyPropertyChanged
    {
        private readonly SUBCLASSPROC mainWindowSubClassProc;
        private readonly SUBCLASSPROC inputNonClientPointerSourceSubClassProc;
        private readonly ContentCoordinateConverter contentCoordinateConverter;
        private readonly OverlappedPresenter overlappedPresenter;

        private WebViewControlProcess webViewControlProcess;
        private WebViewControl webViewControl;

        public new static WebWindow Current { get; private set; }

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

        private bool _canGoBack = false;

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

        private bool _canGoForward = false;

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

        private bool _isEnabled = false;

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

        public WebWindow()
        {
            Current = this;
            InitializeComponent();

            // 窗口部分初始化
            overlappedPresenter = AppWindow.Presenter as OverlappedPresenter;
            ExtendsContentIntoTitleBar = true;
            AppWindow.Resize(new SizeInt32(1000, 700));
            AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
            AppWindow.TitleBar.InactiveBackgroundColor = Colors.Transparent;
            AppWindow.TitleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;
            IsWindowMaximized = overlappedPresenter.State is OverlappedPresenterState.Maximized;
            contentCoordinateConverter = ContentCoordinateConverter.CreateForWindowId(AppWindow.Id);

            // 标题栏和右键菜单设置
            SetTitleBarTheme((Content as FrameworkElement).ActualTheme);
            SetClassicMenuTheme((Content as FrameworkElement).ActualTheme);

            // 挂载相应的事件
            AppWindow.Changed += OnAppWindowChanged;
            AppWindow.Closing += OnAppWindowClosing;

            // 为应用主窗口添加窗口过程
            mainWindowSubClassProc = new SUBCLASSPROC(MainWindowSubClassProc);
            Comctl32Library.SetWindowSubclass((IntPtr)AppWindow.Id.Value, Marshal.GetFunctionPointerForDelegate(mainWindowSubClassProc), 0, IntPtr.Zero);

            // 为非工作区的窗口设置相应的窗口样式，添加窗口过程
            IntPtr inputNonClientPointerSourceHandle = User32Library.FindWindowEx((IntPtr)AppWindow.Id.Value, IntPtr.Zero, "InputNonClientPointerSource", null);

            if (inputNonClientPointerSourceHandle != IntPtr.Zero)
            {
                inputNonClientPointerSourceSubClassProc = new SUBCLASSPROC(InputNonClientPointerSourceSubClassProc);
                Comctl32Library.SetWindowSubclass((IntPtr)AppWindow.Id.Value, Marshal.GetFunctionPointerForDelegate(inputNonClientPointerSourceSubClassProc), 0, IntPtr.Zero);
            }
        }

        #region 第一部分：窗口类事件

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
        }

        #endregion 第一部分：窗口类事件

        #region 第二部分：窗口辅助类（AppWindow）挂载的事件

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

            if (Content is not null && Content.XamlRoot is not null && webViewControl is not null)
            {
                webViewControl.Bounds = new Rect()
                {
                    Width = sender.ClientSize.Width,
                    Height = sender.ClientSize.Height - 90 * Content.XamlRoot.RasterizationScale,
                    X = 0,
                    Y = 90 * Content.XamlRoot.RasterizationScale
                };
            }

            if (WebKernelService.WebKernel == WebKernelService.WebKernelList[1])
            {
                CloseDownloadDialog();
            }
        }

        /// <summary>
        /// 关闭窗口之后关闭其他服务
        /// </summary>
        private void OnAppWindowClosing(object sender, AppWindowClosingEventArgs args)
        {
            AppWindow.Changed -= OnAppWindowChanged;
            CloseWindow();
            (Application.Current as WebApp).Dispose();
        }

        #endregion 第二部分：窗口辅助类（AppWindow）挂载的事件

        #region 第三部分：窗口右键菜单事件

        /// <summary>
        /// 窗口还原
        /// </summary>
        private void OnRestoreClicked(object sender, RoutedEventArgs args)
        {
            overlappedPresenter.Restore();
        }

        /// <summary>
        /// 窗口移动
        /// </summary>
        private void OnMoveClicked(object sender, RoutedEventArgs args)
        {
            MenuFlyoutItem menuItem = sender as MenuFlyoutItem;
            if (menuItem.Tag is not null)
            {
                ((MenuFlyout)menuItem.Tag).Hide();
                User32Library.SendMessage((IntPtr)AppWindow.Id.Value, WindowMessage.WM_SYSCOMMAND, 0xF010, 0);
            }
        }

        /// <summary>
        /// 窗口大小
        /// </summary>
        private void OnSizeClicked(object sender, RoutedEventArgs args)
        {
            MenuFlyoutItem menuItem = sender as MenuFlyoutItem;
            if (menuItem.Tag is not null)
            {
                ((MenuFlyout)menuItem.Tag).Hide();
                User32Library.SendMessage((IntPtr)AppWindow.Id.Value, WindowMessage.WM_SYSCOMMAND, 0xF000, 0);
            }
        }

        /// <summary>
        /// 窗口最小化
        /// </summary>
        private void OnMinimizeClicked(object sender, RoutedEventArgs args)
        {
            overlappedPresenter.Minimize();
        }

        /// <summary>
        /// 窗口最大化
        /// </summary>
        private void OnMaximizeClicked(object sender, RoutedEventArgs args)
        {
            overlappedPresenter.Maximize();
        }

        /// <summary>
        /// 窗口关闭
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            (Application.Current as WebApp).Dispose();
        }

        #endregion 第三部分：窗口右键菜单事件

        #region 第四部分：窗口内容挂载的事件

        /// <summary>
        /// 应用主题发生变化时修改应用的背景色
        /// </summary>
        private void OnActualThemeChanged(FrameworkElement sender, object args)
        {
            SetTitleBarTheme(sender.ActualTheme);
            SetClassicMenuTheme(sender.ActualTheme);
        }

        #endregion 第四部分：窗口内容挂载的事件

        #region 第五部分：浏览器事件

        /// <summary>
        /// 初始化浏览器需要加载的内容
        /// </summary>
        private async void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (WebKernelService.WebKernel == WebKernelService.WebKernelList[0])
            {
                webViewControlProcess = new WebViewControlProcess();
                webViewControlProcess.ProcessExited += OnProcessExited;
                webViewControl = await webViewControlProcess.CreateWebViewControlAsync((long)AppWindow.Id.Value, new Rect()
                {
                    Width = AppWindow.ClientSize.Width,
                    Height = AppWindow.ClientSize.Height - 90 * Content.XamlRoot.RasterizationScale,
                    X = 0,
                    Y = 90 * Content.XamlRoot.RasterizationScale
                });
                webViewControl.NavigationStarting += OnWebViewNavigationStarting;
                webViewControl.NavigationCompleted += OnWebViewNavigationCompleted;
                webViewControl.NewWindowRequested += OnNewWindowRequested;
                webViewControl.UnviewableContentIdentified += OnUnviewableContentIdentified;
                webViewControl.Source = new Uri("https://store.rg-adguard.net");
                IsEnabled = true;
            }
            else
            {
                if (WebView2Browser is not null)
                {
                    WebView2Browser.Source = new Uri("https://store.rg-adguard.net");
                }
            }
        }

        /// <summary>
        /// WebView 浏览器进程退出触发的事件
        /// </summary>
        private async void OnProcessExited(WebViewControlProcess sender, object args)
        {
            webViewControlProcess = null;
            webViewControl = null;
            await ContentDialogHelper.ShowAsync(new ProcessFailedDialog(), Content as FrameworkElement);
            (Application.Current as WebApp).Dispose();
        }

        /// <summary>
        /// WebView : 页面开始导航
        /// </summary>
        private void OnWebViewNavigationStarting(IWebViewControl sender, WebViewControlNavigationStartingEventArgs args)
        {
            IsLoading = true;
            if (webViewControl is not null && webViewControl.CanGoBack)
            {
                webViewControl.GoBack();
            }

            if (webViewControl is not null && webViewControl.CanGoForward)
            {
                webViewControl.GoForward();
            }
        }

        /// <summary>
        /// WebView : 页面完成导航
        /// </summary>
        private void OnWebViewNavigationCompleted(IWebViewControl sender, WebViewControlNavigationCompletedEventArgs args)
        {
            IsLoading = false;
            Title = string.Format("{0} - {1}", webViewControl.DocumentTitle, ResourceService.GetLocalized("WebView/Title"));
            if (webViewControl is not null && webViewControl.CanGoBack)
            {
                webViewControl.GoBack();
            }

            if (webViewControl is not null && webViewControl.CanGoForward)
            {
                webViewControl.GoForward();
            }
        }

        /// <summary>
        /// WebView : 当前页面对应的链接发生改变时触发这一事件
        /// </summary>
        private void OnNewWindowRequested(IWebViewControl sender, WebViewControlNewWindowRequestedEventArgs args)
        {
            args.Handled = true;
            webViewControl?.Navigate(args.Uri);
        }

        /// <summary>
        /// WebView 下载文件
        /// </summary>
        private async void OnUnviewableContentIdentified(object sender, WebViewControlUnviewableContentIdentifiedEventArgs args)
        {
            await Launcher.LaunchUriAsync(args.Uri);
        }

        /// <summary>
        /// 网页后退
        /// </summary>
        private void OnBrowserBackClicked(object sender, RoutedEventArgs args)
        {
            if (WebKernelService.WebKernel == WebKernelService.WebKernelList[0])
            {
                if (webViewControl is not null && webViewControl.CanGoBack)
                {
                    webViewControl.GoBack();
                }
            }
            else
            {
                if (WebView2Browser is not null && WebView2Browser.CanGoBack)
                {
                    WebView2Browser.GoBack();
                }
            }
        }

        /// <summary>
        /// 网页前进
        /// </summary>
        private void OnBrowserForwardClicked(object sender, RoutedEventArgs args)
        {
            if (WebKernelService.WebKernel == WebKernelService.WebKernelList[0])
            {
                if (webViewControl is not null && webViewControl.CanGoForward)
                {
                    webViewControl.GoForward();
                }
            }
            else
            {
                if (WebView2Browser is not null && WebView2Browser.CanGoForward)
                {
                    WebView2Browser.GoForward();
                }
            }
        }

        /// <summary>
        /// 网页刷新
        /// </summary>
        private void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            if (WebKernelService.WebKernel == WebKernelService.WebKernelList[0])
            {
                webViewControl?.Refresh();
            }
            else
            {
                WebView2Browser?.Reload();
            }
        }

        /// <summary>
        /// WebView2 ：打开下载窗口
        /// WebView ：打开系统下载文件夹
        /// </summary>
        private async void OnDownloadClicked(object sender, RoutedEventArgs args)
        {
            if (WebKernelService.WebKernel == WebKernelService.WebKernelList[0])
            {
                await Launcher.LaunchFolderPathAsync(UserDataPaths.GetDefault().Downloads);
            }
            else
            {
                if (WebView2Browser is not null && WebView2Browser.CoreWebView2 is not null)
                {
                    WebView2Browser.CoreWebView2.OpenDefaultDownloadDialog();
                }
            }
        }

        /// <summary>
        /// 在浏览器中打开
        /// </summary>
        private async void OnOpenWithBrowserClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://store.rg-adguard.net"));
        }

        /// <summary>
        /// 打开缓存文件夹
        /// </summary>
        private void OnOpenCacheFolderClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                if (Directory.Exists(Path.Combine(ApplicationData.Current.LocalFolder.Path, "EbWebView")))
                {
                    await Launcher.LaunchFolderPathAsync(Path.Combine(ApplicationData.Current.LocalFolder.Path, "EbWebView"));
                }
                else
                {
                    await Launcher.LaunchFolderPathAsync(ApplicationData.Current.LocalFolder.Path);
                }
            });
        }

        /// <summary>
        ///  清理网页缓存
        /// </summary>
        private async void OnClearWebCacheClicked(object sender, RoutedEventArgs args)
        {
            if (WebKernelService.WebKernel == WebKernelService.WebKernelList[1])
            {
                if (WebView2Browser is not null && WebView2Browser.CoreWebView2 is not null)
                {
                    await WebView2Browser.CoreWebView2.Profile.ClearBrowsingDataAsync();
                }
            }
        }

        /// <summary>
        /// WebView2 ：进程异常退出时触发的事件
        /// </summary>
        private async void OnCoreProcessFailed(object sender, CoreWebView2ProcessFailedEventArgs args)
        {
            StringBuilder processFailedBuilder = new();
            processFailedBuilder.Append("ProcessFailedKind:");
            processFailedBuilder.Append(args.ProcessFailedKind.ToString());
            processFailedBuilder.Append(Environment.NewLine);
            processFailedBuilder.Append("Reason:");
            processFailedBuilder.Append(args.Reason.ToString());
            processFailedBuilder.Append(Environment.NewLine);
            processFailedBuilder.Append("ExitCode:");
            processFailedBuilder.Append(args.ExitCode);
            processFailedBuilder.Append(Environment.NewLine);
            processFailedBuilder.Append("ProcessDescription:");
            processFailedBuilder.Append(args.ProcessDescription);
            processFailedBuilder.Append(Environment.NewLine);

            Dictionary<string, string> logInformationDict = new()
            {
                { "Process failed kind", args.ProcessFailedKind.ToString() },
                { "Reason", args.Reason.ToString() },
                { "Exit code", args.ExitCode.ToString() },
                { "Process description", args.ProcessDescription },
            };

            LogService.WriteLog(LoggingLevel.Error, "WebView2 process failed", logInformationDict);

            await ContentDialogHelper.ShowAsync(new ProcessFailedDialog(), Content as FrameworkElement);
            (Application.Current as WebApp).Dispose();
        }

        /// <summary>
        /// WebView2 : 初始化 CoreWebView2 对象
        /// </summary>
        private void OnCoreWebView2Initialized(object sender, CoreWebView2InitializedEventArgs args)
        {
            WebView2Browser.CoreWebView2.NewWindowRequested += OnNewWindowRequested;
            WebView2Browser.CoreWebView2.SourceChanged += OnSourceChanged;
            IsEnabled = true;
        }

        /// <summary>
        /// WebView2 : 页面开始导航
        /// </summary>
        private void OnWebView2NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs args)
        {
            IsLoading = true;
        }

        /// <summary>
        /// WebView2 : 页面完成导航
        /// </summary>
        private void OnWebView2NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            IsLoading = false;
            Title = string.Format("{0} - {1}", WebView2Browser.CoreWebView2.DocumentTitle, ResourceService.GetLocalized("WebView/Title"));
        }

        /// <summary>
        /// WebView2 : 当前页面对应的链接发生改变时触发这一事件
        /// </summary>
        private void OnSourceChanged(object sender, CoreWebView2SourceChangedEventArgs args)
        {
            CoreWebView2 coreWebView2 = sender as CoreWebView2;

            if (coreWebView2 is not null)
            {
                CanGoBack = coreWebView2.CanGoBack;
                CanGoForward = coreWebView2.CanGoForward;
            }
        }

        /// <summary>
        /// WebView2 : 捕捉打开新窗口事件，并禁止弹窗
        /// </summary>
        private void OnNewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs args)
        {
            args.Handled = true;

            CoreWebView2 coreWebView2 = sender as CoreWebView2;
            coreWebView2?.Navigate(args.Uri);
        }

        #endregion 第五部分：浏览器事件

        #region 第六部分：窗口属性设置

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

            if (theme is ElementTheme.Light)
            {
                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonForegroundColor = Colors.Black;
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(20, 0, 0, 0);
                titleBar.ButtonHoverForegroundColor = Colors.Black;
                titleBar.ButtonPressedBackgroundColor = Color.FromArgb(30, 0, 0, 0);
                titleBar.ButtonPressedForegroundColor = Colors.Black;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveForegroundColor = Colors.Black;
            }
            else
            {
                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonForegroundColor = Colors.White;
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(20, 255, 255, 255);
                titleBar.ButtonHoverForegroundColor = Colors.White;
                titleBar.ButtonPressedBackgroundColor = Color.FromArgb(30, 255, 255, 255);
                titleBar.ButtonPressedForegroundColor = Colors.White;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveForegroundColor = Colors.White;
            }
        }

        /// <summary>
        /// 设置传统菜单标题栏按钮的主题色
        /// </summary>
        private static void SetClassicMenuTheme(ElementTheme theme)
        {
            if (theme is ElementTheme.Light)
            {
                UxthemeLibrary.SetPreferredAppMode(PreferredAppMode.ForceLight);
            }
            else
            {
                UxthemeLibrary.SetPreferredAppMode(PreferredAppMode.ForceDark);
            }

            UxthemeLibrary.FlushMenuThemes();
        }

        #endregion 第六部分：窗口属性设置

        #region 第七部分：窗口过程

        /// <summary>
        /// 应用主窗口消息处理
        /// </summary>
        private IntPtr MainWindowSubClassProc(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam, uint uIdSubclass, IntPtr dwRefData)
        {
            switch (Msg)
            {
                // 窗口大小发生更改时的消息
                case WindowMessage.WM_GETMINMAXINFO:
                    {
                        if (Content is not null && Content.XamlRoot is not null)
                        {
                            MINMAXINFO minMaxInfo = Marshal.PtrToStructure<MINMAXINFO>(lParam);
                            minMaxInfo.ptMinTrackSize.X = (int)(960 * Content.XamlRoot.RasterizationScale);
                            minMaxInfo.ptMinTrackSize.Y = (int)(600 * Content.XamlRoot.RasterizationScale);
                            Marshal.StructureToPtr(minMaxInfo, lParam, true);
                        }

                        break;
                    }

                // 选择窗口右键菜单的条目时接收到的消息
                case WindowMessage.WM_SYSCOMMAND:
                    {
                        SystemCommand sysCommand = (SystemCommand)(wParam.ToInt32() & 0xFFF0);

                        if (sysCommand is SystemCommand.SC_MOUSEMENU || sysCommand is SystemCommand.SC_KEYMENU)
                        {
                            FlyoutShowOptions options = new()
                            {
                                Position = new Point(0, 45),
                                ShowMode = FlyoutShowMode.Standard
                            };
                            TitlebarMenuFlyout.ShowAt(null, options);
                            return IntPtr.Zero;
                        }
                        break;
                    }
            }

            return Comctl32Library.DefSubclassProc(hWnd, Msg, wParam, lParam);
        }

        /// <summary>
        /// 应用拖拽区域窗口消息处理
        /// </summary>
        private IntPtr InputNonClientPointerSourceSubClassProc(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam, uint uIdSubclass, IntPtr dwRefData)
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
                // 当用户按下鼠标右键时，光标位于窗口的非工作区内的消息
                case WindowMessage.WM_NCRBUTTONUP:
                    {
                        if (Content is not null && Content.XamlRoot is not null)
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
                        return IntPtr.Zero;
                    }
            }
            return Comctl32Library.DefSubclassProc(hWnd, Msg, wParam, lParam);
        }

        #endregion 第七部分：窗口过程

        /// <summary>
        /// 关闭窗口时注销的事件
        /// </summary>
        public void CloseWindow()
        {
            if (WebKernelService.WebKernel == WebKernelService.WebKernelList[0])
            {
                if (webViewControlProcess is not null && webViewControl is not null)
                {
                    try
                    {
                        webViewControl.NavigationStarting -= OnWebViewNavigationStarting;
                        webViewControl.NavigationCompleted -= OnWebViewNavigationCompleted;
                        webViewControl.NewWindowRequested -= OnNewWindowRequested;
                        webViewControl.UnviewableContentIdentified -= OnUnviewableContentIdentified;
                        webViewControl.Close();
                        webViewControl = null;
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "WebView unloaded event failed", e);
                    }
                }
            }
            else
            {
                if (WebView2Browser is not null)
                {
                    try
                    {
                        if (WebView2Browser.CoreWebView2 is not null)
                        {
                            WebView2Browser.CoreWebView2.NewWindowRequested -= OnNewWindowRequested;
                            WebView2Browser.CoreWebView2.SourceChanged -= OnSourceChanged;
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "WebView2 unloaded event failed", e);
                    }
                }
            }
        }

        /// <summary>
        /// 窗口移动或调整大小时关闭下载框
        /// </summary>
        public void CloseDownloadDialog()
        {
            if (WebView2Browser is not null && WebView2Browser.CoreWebView2 is not null && WebView2Browser.CoreWebView2.IsDefaultDownloadDialogOpen)
            {
                WebView2Browser.CoreWebView2.CloseDefaultDownloadDialog();
            }
        }
    }
}
