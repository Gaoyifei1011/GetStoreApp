using GetStoreAppWebView.Helpers.Root;
using GetStoreAppWebView.Services.Controls.Settings;
using GetStoreAppWebView.Services.Root;
using GetStoreAppWebView.Views.Windows;
using GetStoreAppWebView.WindowsAPI.PInvoke.User32;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using Windows.Foundation.Diagnostics;
using Windows.Storage;

namespace GetStoreAppWebView.Views.Controls
{
    /// <summary>
    /// WinUI 3 XAML 岛控件
    /// </summary>
    public sealed partial class IslandsControl : Grid, INotifyPropertyChanged
    {
        private bool IsLoadWebView2 = false;

        public System.Windows.Controls.WebBrowser WebBrowser { get; private set; }

        private bool _isWindowMaximized;

        public bool IsWindowMaximized
        {
            get { return _isWindowMaximized; }

            set
            {
                _isWindowMaximized = value;
                OnPropertyChanged();
            }
        }

        private bool _canGoBack = false;

        public bool CanGoBack
        {
            get { return _canGoBack; }

            set
            {
                _canGoBack = value;
                OnPropertyChanged();
            }
        }

        private bool _canGoForward = false;

        public bool CanGoForward
        {
            get { return _canGoForward; }

            set
            {
                _canGoForward = value;
                OnPropertyChanged();
            }
        }

        private bool _isLoading = true;

        public bool IsLoading
        {
            get { return _isLoading; }

            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        private bool _isEnabled = false;

        public bool IsEnabled
        {
            get { return _isEnabled; }

            set
            {
                _isEnabled = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IslandsControl()
        {
            InitializeComponent();

            if (WebKernelService.WebKernel == WebKernelService.WebKernelList[0])
            {
                IsLoadWebView2 = false;
                VerticalAlignment = VerticalAlignment.Top;
            }
            else
            {
                IsLoadWebView2 = true;
                VerticalAlignment = VerticalAlignment.Stretch;
            }
        }

        /// <summary>
        /// 应用主题发生变化时修改应用的背景色
        /// </summary>
        private void OnActualThemeChanged(FrameworkElement sender, object args)
        {
            SetIslandsColor(ActualTheme);
        }

        /// <summary>
        /// 控件加载完成时初始化控件位置
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            SetIslandsColor(ActualTheme);

            if (WebKernelService.WebKernel == WebKernelService.WebKernelList[0])
            {
                IsEnabled = true;
                WebBrowser.Margin = new System.Windows.Thickness(0, ActualHeight, 0, 0);
                User32Library.SetWindowPos((IntPtr)(System.Windows.Application.Current.MainWindow as MainWindow).DesktopWindowXamlSource.SiteBridge.WindowId.Value,
  IntPtr.Zero, 0, 0, (int)(ActualWidth * (System.Windows.Application.Current.MainWindow as MainWindow).WindowDPI), Convert.ToInt32(ActualHeight * (System.Windows.Application.Current.MainWindow as MainWindow).WindowDPI), SetWindowPosFlags.SWP_NOOWNERZORDER | SetWindowPosFlags.SWP_NOZORDER);
            }
        }

        /// <summary>
        /// 窗口关闭
        /// </summary>
        private void OnCloseClicked(object sender, RoutedEventArgs args)
        {
            (System.Windows.Application.Current as WPFApp).Dispose();
        }

        /// <summary>
        /// 窗口最大化
        /// </summary>
        private void OnMaximizeClicked(object sender, RoutedEventArgs args)
        {
            (System.Windows.Application.Current.MainWindow as MainWindow).WindowState = System.Windows.WindowState.Maximized;
        }

        /// <summary>
        /// 窗口最小化
        /// </summary>
        private void OnMinimizeClicked(object sender, RoutedEventArgs args)
        {
            (System.Windows.Application.Current.MainWindow as MainWindow).WindowState = System.Windows.WindowState.Minimized;
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
                User32Library.SendMessage((System.Windows.Application.Current.MainWindow as MainWindow).Handle, WindowMessage.WM_SYSCOMMAND, 0xF010, IntPtr.Zero);
            }
        }

        /// <summary>
        /// 窗口还原
        /// </summary>
        private void OnRestoreClicked(object sender, RoutedEventArgs args)
        {
            (System.Windows.Application.Current.MainWindow as MainWindow).WindowState = System.Windows.WindowState.Normal;
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
                User32Library.SendMessage((System.Windows.Application.Current.MainWindow as MainWindow).Handle, WindowMessage.WM_SYSCOMMAND, 0xF000, IntPtr.Zero);
            }
        }

        /// <summary>
        /// 网页后退
        /// </summary>
        private void OnBrowserBackClicked(object sender, RoutedEventArgs args)
        {
            if (WebKernelService.WebKernel == WebKernelService.WebKernelList[0])
            {
                if (WebBrowser is not null && WebBrowser.CanGoBack)
                {
                    WebBrowser.GoBack();
                }
            }
            else
            {
                if (webView2 is not null && webView2.CanGoBack)
                {
                    webView2.GoBack();
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
                if (WebBrowser is not null && WebBrowser.CanGoForward)
                {
                    WebBrowser.GoForward();
                }
            }
            else
            {
                if (webView2 is not null && webView2.CanGoForward)
                {
                    webView2.GoForward();
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
                WebBrowser?.Refresh();
            }
            else
            {
                webView2?.Reload();
            }
        }

        /// <summary>
        /// WebView2 ：打开下载窗口
        /// WPF WebBrowser ：打开系统下载文件夹
        /// </summary>
        private void OnDownloadClicked(object sender, RoutedEventArgs args)
        {
            if (WebKernelService.WebKernel == WebKernelService.WebKernelList[1])
            {
                if (webView2 is not null && webView2.CoreWebView2 is not null)
                {
                    webView2.CoreWebView2.OpenDefaultDownloadDialog();
                }
            }
            else
            {
                if (WebBrowser is not null)
                {
                    Task.Run(() =>
                    {
                        Process.Start(InfoHelper.UserDataPath.Downloads);
                    });
                }
            }
        }

        /// <summary>
        /// 在浏览器中打开
        /// </summary>
        private void OnOpenWithBrowserClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                Process.Start("https://store.rg-adguard.net");
            });
        }

        /// <summary>
        /// 打开缓存文件夹
        /// </summary>
        private void OnOpenCacheFolderClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                if (WebKernelService.WebKernel == WebKernelService.WebKernelList[0])
                {
                    Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache));
                }
                else
                {
                    if (Directory.Exists(Path.Combine(ApplicationData.Current.LocalFolder.Path, "EbWebView")))
                    {
                        Process.Start(Path.Combine(ApplicationData.Current.LocalFolder.Path, "EbWebView"));
                    }
                    else
                    {
                        Process.Start(Path.Combine(ApplicationData.Current.LocalFolder.Path));
                    }
                }
            });
        }

        /// <summary>
        ///  清理网页缓存
        /// </summary>
        private void OnClearWebCacheClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(async () =>
            {
                if (WebKernelService.WebKernel == WebKernelService.WebKernelList[0])
                {
                    Process.Start("RunDll32.exe", "InetCpl.cpl,ClearMyTracksByProcess 255");
                }
                else
                {
                    if (webView2 is not null && webView2.CoreWebView2 is not null)
                    {
                        await webView2.CoreWebView2.Profile.ClearBrowsingDataAsync();
                    }
                }
            });
        }

        /// <summary>
        /// WebView2 ：进程异常退出时触发的事件
        /// </summary>
        private void OnCoreProcessFailed(object sender, CoreWebView2ProcessFailedEventArgs args)
        {
            StringBuilder processFailedBuilder = new StringBuilder();
            processFailedBuilder.Append("ProcessFailedKind:");
            processFailedBuilder.Append(args.ProcessFailedKind.ToString());
            processFailedBuilder.Append(Environment.NewLine);
            processFailedBuilder.Append("Reason:");
            processFailedBuilder.Append(args.Reason.ToString());
            processFailedBuilder.Append(Environment.NewLine);
            processFailedBuilder.Append("ExitCode:");
            processFailedBuilder.Append(args.ExitCode.ToString());
            processFailedBuilder.Append(Environment.NewLine);
            processFailedBuilder.Append("ProcessDescription:");
            processFailedBuilder.Append(args.ProcessDescription);
            processFailedBuilder.Append(Environment.NewLine);

            LogService.WriteLog(LoggingLevel.Error, "WebView2 process failed", processFailedBuilder);

            System.Windows.MessageBox.Show(ResourceService.GetLocalized("WebView/WebViewProcessFailedContent"), ResourceService.GetLocalized("WebView/WebViewProcessFailedTitle"), System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            (System.Windows.Application.Current as WPFApp).Dispose();
        }

        /// <summary>
        /// WebView2 : 初始化 CoreWebView2 对象
        /// </summary>
        private void OnCoreWebView2Initialized(object sender, CoreWebView2InitializedEventArgs args)
        {
            webView2.CoreWebView2.NewWindowRequested += OnNewWindowRequested;
            webView2.CoreWebView2.SourceChanged += OnSourceChanged;
            IsEnabled = true;
        }

        /// <summary>
        /// WebView2 : 页面开始导航
        /// </summary>
        private void OnNavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs args)
        {
            IsLoading = true;
        }

        /// <summary>
        /// WebView2 : 页面完成导航
        /// </summary>
        private void OnNavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            IsLoading = false;
            System.Windows.Application.Current.MainWindow.Title = string.Format("{0} - {1}", webView2.CoreWebView2.DocumentTitle, ResourceService.GetLocalized("WebView/Title"));
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

        /// <summary>
        /// WPF WebBrowser : 当前页面对应的链接发生改变时触发这一事件
        /// </summary>
        private void OnNavigated(object sender, NavigationEventArgs args)
        {
            System.Windows.Application.Current.MainWindow.Title = string.Format("{0} - {1}", WebBrowser.InvokeScript("eval", "document.title").ToString(), ResourceService.GetLocalized("WebView/Title"));
            CanGoBack = WebBrowser.CanGoBack;
            CanGoForward = WebBrowser.CanGoForward;
            IsLoading = false;
        }

        /// <summary>
        /// WPF WebBrowser : 当前页面对应的链接发生改变时触发这一事件
        /// </summary>
        private void OnNavigating(object sender, NavigatingCancelEventArgs args)
        {
            IsLoading = true;
        }

        /// <summary>
        /// 设置应用窗口和标题栏的颜色
        /// </summary>
        private void SetIslandsColor(ElementTheme theme)
        {
            AppWindowTitleBar titleBar = (System.Windows.Application.Current.MainWindow as MainWindow).AppWindow.TitleBar;

            titleBar.BackgroundColor = Colors.Transparent;
            titleBar.ForegroundColor = Colors.Transparent;
            titleBar.InactiveBackgroundColor = Colors.Transparent;
            titleBar.InactiveForegroundColor = Colors.Transparent;

            if (theme is ElementTheme.Light)
            {
                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonForegroundColor = Colors.Black;
                titleBar.ButtonHoverBackgroundColor = global::Windows.UI.Color.FromArgb(20, 0, 0, 0);
                titleBar.ButtonHoverForegroundColor = Colors.Black;
                titleBar.ButtonPressedBackgroundColor = global::Windows.UI.Color.FromArgb(30, 0, 0, 0);
                titleBar.ButtonPressedForegroundColor = Colors.Black;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveForegroundColor = Colors.Black;
            }
            else
            {
                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonForegroundColor = Colors.White;
                titleBar.ButtonHoverBackgroundColor = global::Windows.UI.Color.FromArgb(20, 255, 255, 255);
                titleBar.ButtonHoverForegroundColor = Colors.White;
                titleBar.ButtonPressedBackgroundColor = global::Windows.UI.Color.FromArgb(30, 255, 255, 255);
                titleBar.ButtonPressedForegroundColor = Colors.White;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveForegroundColor = Colors.White;
            }
        }

        /// <summary>
        /// 关闭窗口时注销的事件
        /// </summary>
        public void CloseWindow()
        {
            if (WebKernelService.WebKernel == WebKernelService.WebKernelList[0])
            {
                if (WebBrowser is not null)
                {
                    try
                    {
                        WebBrowser.Navigated -= OnNavigated;
                        WebBrowser.Navigating -= OnNavigating;
                        WebBrowser.Dispose();
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "WPF WebBrowser unloaded failed", e);
                    }
                }
            }
            else
            {
                if (webView2 is not null)
                {
                    try
                    {
                        if (webView2.CoreWebView2 is not null)
                        {
                            webView2.CoreWebView2.NewWindowRequested -= OnNewWindowRequested;
                            webView2.CoreWebView2.SourceChanged -= OnSourceChanged;
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
            if (webView2 is not null && webView2.CoreWebView2 is not null && webView2.CoreWebView2.IsDefaultDownloadDialogOpen)
            {
                webView2.CoreWebView2.CloseDefaultDownloadDialog();
            }
        }

        /// <summary>
        /// 初始化 WPF WebBrowser控件
        /// </summary>
        public void InitializeWebBrowser()
        {
            WebBrowser = new System.Windows.Controls.WebBrowser();
            WebBrowser.Source = new Uri("https://store.rg-adguard.net");
            System.Windows.Application.Current.MainWindow.Content = WebBrowser;
            SuppressScriptErrors(WebBrowser, true);
            WebBrowser.AllowDrop = false;
            WebBrowser.Navigating += OnNavigating;
            WebBrowser.Navigated += OnNavigated;
        }

        /// <summary>
        /// 禁止 WebBrowser控件显示脚本错误消息对话框
        /// </summary>
        public void SuppressScriptErrors(System.Windows.Controls.WebBrowser webBrowser, bool Hide)
        {
            FieldInfo fiComWebBrowser = typeof(System.Windows.Controls.WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiComWebBrowser == null) return;

            object objComWebBrowser = fiComWebBrowser.GetValue(webBrowser);
            if (objComWebBrowser == null) return;

            objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, [Hide]);
        }

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
