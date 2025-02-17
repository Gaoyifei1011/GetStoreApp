﻿using GetStoreAppWebView.Helpers.Controls.Extensions;
using GetStoreAppWebView.Helpers.Root;
using GetStoreAppWebView.Services.Controls.Settings;
using GetStoreAppWebView.Services.Root;
using GetStoreAppWebView.UI.Dialogs;
using GetStoreAppWebView.WindowsAPI.ComTypes;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.System;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinRT;

// 抑制 CA1822，IDE0060 警告
#pragma warning disable CA1822,IDE0060

namespace GetStoreAppWebView.Pages
{
    /// <summary>
    /// 显示网页
    /// </summary>
    public sealed partial class WebViewPage : Page, INotifyPropertyChanged
    {
        private readonly ApplicationView applicationView = ApplicationView.GetForCurrentView();
        private readonly CoreApplicationView coreApplicationView = CoreApplication.GetCurrentView();
        private readonly IInternalCoreWindow2 internalCoreWindow2 = Window.Current.CoreWindow.As<IInternalCoreWindow2>();
        private readonly EventHandler windowPositionChangedEventHandler;

        private bool _enableBackdropMaterial;

        public bool EnableBackdropMaterial
        {
            get { return _enableBackdropMaterial; }

            set
            {
                if (!Equals(_enableBackdropMaterial, value))
                {
                    _enableBackdropMaterial = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EnableBackdropMaterial)));
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

        private Uri _source;

        public Uri Source
        {
            get { return _source; }

            set
            {
                if (!Equals(_source, value))
                {
                    _source = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDownloadClickEnabled)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public WebViewPage()
        {
            InitializeComponent();

            // 窗口部分初始化
            coreApplicationView.TitleBar.ExtendViewIntoTitleBar = true;
            applicationView.TryResizeView(new Size(1000, 700));
            applicationView.TitleBar.ButtonBackgroundColor = Colors.Transparent;
            applicationView.TitleBar.InactiveBackgroundColor = Colors.Transparent;
            WindowTheme = Enum.TryParse(ThemeService.AppTheme, out ElementTheme elementTheme) ? elementTheme : ElementTheme.Default;

            // 标题栏设置
            Window.Current.SetTitleBar(AppTitlebar);
            SetTitleBarTheme(ActualTheme);

            // 设置网页
            Source = new Uri("https://store.rg-adguard.net");

            if (ApiInformation.IsMethodPresent(typeof(Compositor).FullName, nameof(Compositor.TryCreateBlurredWallpaperBackdropBrush)))
            {
                EnableBackdropMaterial = true;
                VisualStateManager.GoToState(WebViewPageRoot, "MicaBackdrop", false);
            }
            else
            {
                EnableBackdropMaterial = false;
                VisualStateManager.GoToState(WebViewPageRoot, "DesktopAcrylicBackdrop", false);
            }

            windowPositionChangedEventHandler = new EventHandler(OnWindowPositionChanged);
            internalCoreWindow2.AddWindowPositionChanged(MarshalInterface<EventHandler>.CreateMarshaler(windowPositionChangedEventHandler).ThisPtr, out EventRegistrationToken token);
        }

        #region 第一部分：窗口内容挂载的事件

        /// <summary>
        /// 应用主题发生变化时修改应用的背景色
        /// </summary>
        private void OnActualThemeChanged(FrameworkElement sender, object args)
        {
            SetTitleBarTheme(sender.ActualTheme);
        }

        /// <summary>
        /// 窗口位置和大小发生变化关闭 WebView 2 的下载对话框
        /// </summary>
        private void OnWindowPositionChanged(object sender, EventArgs args)
        {
            if (WebKernelService.WebKernel == WebKernelService.WebKernelList[1] && WebView2Browser is not null && WebView2Browser.CoreWebView2 is not null && WebView2Browser.CoreWebView2.IsDefaultDownloadDialogOpen)
            {
                WebView2Browser.CoreWebView2.CloseDefaultDownloadDialog();
            }
        }

        #endregion 第一部分：窗口内容挂载的事件

        #region 第二部分：浏览器窗口挂载的事件

        /// <summary>
        /// 网页后退
        /// </summary>
        private void OnBrowserBackClicked(object sender, RoutedEventArgs args)
        {
            if (WebKernelService.WebKernel == WebKernelService.WebKernelList[0])
            {
                if (WebViewBrowser is not null && WebViewBrowser.CanGoBack)
                {
                    WebViewBrowser.GoBack();
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
                if (WebViewBrowser is not null && WebViewBrowser.CanGoForward)
                {
                    WebViewBrowser.GoForward();
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
                WebViewBrowser?.Refresh();
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
                await Launcher.LaunchFolderPathAsync(InfoHelper.UserDataPath.Downloads);
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
        private async void OnOpenCacheFolderClicked(object sender, RoutedEventArgs args)
        {
            if (Directory.Exists(Path.Combine(ApplicationData.Current.LocalFolder.Path, "EbWebView")))
            {
                await Launcher.LaunchFolderPathAsync(Path.Combine(ApplicationData.Current.LocalFolder.Path, "EbWebView"));
            }
            else
            {
                await Launcher.LaunchFolderAsync(ApplicationData.Current.LocalFolder);
            }
        }

        /// <summary>
        ///  清理网页缓存
        /// </summary>
        private async void OnClearWebCacheClicked(object sender, RoutedEventArgs args)
        {
            if (WebKernelService.WebKernel == WebKernelService.WebKernelList[0])
            {
                await WebView.ClearTemporaryWebDataAsync();
            }
            else
            {
                if (WebView2Browser is not null && WebView2Browser.CoreWebView2 is not null)
                {
                    WebView2Browser.CoreWebView2.CookieManager.DeleteAllCookies();
                    await WebView2Browser.CoreWebView2.Profile.ClearBrowsingDataAsync();
                    await WebView2Browser.CoreWebView2.ClearServerCertificateErrorActionsAsync();
                }
            }
        }

        #endregion 第二部分：浏览器窗口挂载的事件

        #region 第三部分 WebView 浏览器事件

        /// <summary>
        /// 页面开始导航
        /// </summary>
        private void OnNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            IsLoading = true;
            if (sender.CanGoBack)
            {
                sender.GoBack();
            }

            if (sender.CanGoForward)
            {
                sender.GoForward();
            }
        }

        /// <summary>
        /// 页面完成导航
        /// </summary>
        private void OnNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            IsLoading = false;
            applicationView.Title = string.Format("{0} - {1}", sender.DocumentTitle, ResourceService.GetLocalized("WebView/Title"));
            if (sender.CanGoBack)
            {
                sender.GoBack();
            }

            if (sender.CanGoForward)
            {
                sender.GoForward();
            }
        }

        /// <summary>
        /// 当前页面对应的链接发生改变时触发这一事件
        /// </summary>
        private void OnNewWindowRequested(WebView sender, WebViewNewWindowRequestedEventArgs args)
        {
            args.Handled = true;
            sender.Navigate(args.Uri);
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        private async void OnUnviewableContentIdentified(WebView sender, WebViewUnviewableContentIdentifiedEventArgs args)
        {
            await Launcher.LaunchUriAsync(args.Uri);
        }

        #endregion 第三部分 WebView 浏览器事件

        #region 第四部分：WebView2 浏览器事件

        /// <summary>
        /// 进程异常退出时触发的事件
        /// </summary>
        private async void OnCoreProcessFailed(object sender, CoreWebView2ProcessFailedEventArgs args)
        {
            Dictionary<string, string> logInformationDict = new()
            {
                { "Process failed kind", args.ProcessFailedKind.ToString() },
                { "Reason", args.Reason.ToString() },
                { "Exit code", args.ExitCode.ToString() },
                { "Process description", args.ProcessDescription },
            };

            LogService.WriteLog(LoggingLevel.Error, "WebView2 process failed", logInformationDict);

            await ContentDialogHelper.ShowAsync(new ProcessFailedDialog(), Content as FrameworkElement);
            (Application.Current as App).Dispose();
        }

        /// <summary>
        /// 初始化 CoreWebView2 对象
        /// </summary>
        private void OnCoreWebView2Initialized(object sender, CoreWebView2InitializedEventArgs args)
        {
            if (WebView2Browser.CoreWebView2 is not null)
            {
                WebView2Browser.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = false;
                WebView2Browser.CoreWebView2.Settings.AreDevToolsEnabled = false;
                WebView2Browser.CoreWebView2.NewWindowRequested += OnCoreWebViewNewWindowRequested;
                WebView2Browser.CoreWebView2.SourceChanged += OnSourceChanged;
                IsEnabled = true;
            }
        }

        /// <summary>
        /// 页面开始导航
        /// </summary>
        private void OnWebView2NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs args)
        {
            IsLoading = true;
        }

        /// <summary>
        /// 页面完成导航
        /// </summary>
        private void OnWebView2NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            IsLoading = false;
            applicationView.Title = string.Format("{0} - {1}", WebView2Browser.CoreWebView2.DocumentTitle, ResourceService.GetLocalized("WebView/Title"));
        }

        /// <summary>
        /// 当前页面对应的链接发生改变时触发这一事件
        /// </summary>
        private void OnSourceChanged(object sender, CoreWebView2SourceChangedEventArgs args)
        {
            if (sender is CoreWebView2 coreWebView2)
            {
                CanGoBack = coreWebView2.CanGoBack;
                CanGoForward = coreWebView2.CanGoForward;
            }
        }

        /// <summary>
        /// 捕捉打开新窗口事件，并禁止弹窗
        /// </summary>
        private void OnCoreWebViewNewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs args)
        {
            args.Handled = true;
            (sender as CoreWebView2)?.Navigate(args.Uri);
        }

        #endregion 第四部分：WebView2 浏览器事件

        #region 第五部分：窗口属性设置

        /// <summary>
        /// 设置标题栏按钮的主题色
        /// </summary>
        private void SetTitleBarTheme(ElementTheme theme)
        {
            ApplicationViewTitleBar titleBar = applicationView.TitleBar;

            titleBar.BackgroundColor = Colors.Transparent;
            titleBar.ForegroundColor = Colors.Transparent;
            titleBar.InactiveBackgroundColor = Colors.Transparent;
            titleBar.InactiveForegroundColor = Colors.Transparent;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            if (theme is ElementTheme.Light)
            {
                titleBar.ButtonForegroundColor = Colors.Black;
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(9, 0, 0, 0);
                titleBar.ButtonHoverForegroundColor = Colors.Black;
                titleBar.ButtonPressedBackgroundColor = Color.FromArgb(6, 0, 0, 0);
                titleBar.ButtonPressedForegroundColor = Colors.Black;
                titleBar.ButtonInactiveForegroundColor = Colors.Black;
            }
            else
            {
                titleBar.ButtonForegroundColor = Colors.White;
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(15, 255, 255, 255);
                titleBar.ButtonHoverForegroundColor = Colors.White;
                titleBar.ButtonPressedBackgroundColor = Color.FromArgb(10, 255, 255, 255);
                titleBar.ButtonPressedForegroundColor = Colors.White;
                titleBar.ButtonInactiveForegroundColor = Colors.White;
            }
        }

        #endregion 第五部分：窗口属性设置
    }
}
