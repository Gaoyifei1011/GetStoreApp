using GetStoreAppWebView.Helpers.Root;
using GetStoreAppWebView.Services.Root;
using GetStoreAppWebView.Views.Controls;
using GetStoreAppWebView.WindowsAPI.PInvoke.DwmApi;
using GetStoreAppWebView.WindowsAPI.PInvoke.User32;
using Microsoft.UI.Windowing;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Mile.Xaml;
using Mile.Xaml.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace GetStoreAppWebView.Views.Forms
{
    /// <summary>
    /// 应用主界面
    /// </summary>
    public class MainForm : Form
    {
        private int windowWidth = 1000;
        private int windowHeight = 700;

        private double WindowDPI;

        private IContainer components = null;

        private WindowsXamlHost MileXamlHost = new WindowsXamlHost();

        public WebBrowser WebBrowser { get; }

        public WebView2 WebView2 { get; }

        public TitleControl TitleControl { get; } = new TitleControl();

        private WNDPROC newInputNonClientPointerSourceWndProc = null;
        private IntPtr oldInputNonClientPointerSourceWndProc = IntPtr.Zero;

        public AppWindow AppWindow { get; }

        private IntPtr InputNonClientPointerSourceHandle { get; }

        public IntPtr UWPCoreHandle { get; }

        public MainForm()
        {
            InitializeComponent();

            BackColor = System.Drawing.Color.Black;
            Icon = Icon.ExtractAssociatedIcon(Process.GetCurrentProcess().MainModule.FileName);
            WindowDPI = ((double)DeviceDpi) / 96;
            MinimumSize = new Size(Convert.ToInt32(windowWidth * WindowDPI), Convert.ToInt32(windowHeight * WindowDPI));
            Size = new Size(Convert.ToInt32(windowWidth * WindowDPI), Convert.ToInt32(windowHeight * WindowDPI));
            StartPosition = FormStartPosition.CenterParent;
            Text = ResourceService.GetLocalized("WebView/Title");

            MileXamlHost.AutoSize = true;
            MileXamlHost.Child = TitleControl;
            MileXamlHost.Dock = DockStyle.Top;
            Controls.Add(MileXamlHost);

            if (RuntimeHelper.IsWebView2Installed)
            {
                WebView2 = new WebView2();
                WebView2.Source = new Uri("https://store.rg-adguard.net");
                Controls.Add(WebView2);
                WebView2.Location = new Point(0, (int)(MileXamlHost.Height * WindowDPI));
                WebView2.Size = new Size(ClientSize.Width, (int)(ClientSize.Height - MileXamlHost.Height * WindowDPI));
                WebView2.CoreWebView2InitializationCompleted += OnCoreWebView2InitializationCompleted;
                WebView2.NavigationStarting += OnNavigationStarting;
                WebView2.NavigationCompleted += OnNavigationCompleted;
                WebView2.SourceChanged += OnWebView2SourceChanged;
            }
            else
            {
                WebBrowser = new WebBrowser();
                WebBrowser.Url = new Uri("https://store.rg-adguard.net");
                WebBrowser.ScriptErrorsSuppressed = true;
                Controls.Add(WebBrowser);
                WebBrowser.Location = new Point(0, (int)(MileXamlHost.Height * WindowDPI));
                WebBrowser.Size = new Size(ClientSize.Width, (int)(ClientSize.Height - MileXamlHost.Height * WindowDPI));
                WebBrowser.Navigating += OnNavigating;
                WebBrowser.Navigated += OnNavigated;
                WebBrowser.NewWindow += OnNewWindow;
            }

            Microsoft.UI.WindowId windowId = new Microsoft.UI.WindowId();
            windowId.Value = (ulong)Handle;
            AppWindow = AppWindow.GetFromWindowId(windowId);
            AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            SetTitleBarColor(TitleControl.ActualTheme);

            InputNonClientPointerSourceHandle = User32Library.FindWindowEx(Handle, IntPtr.Zero, "InputNonClientPointerSource", null);

            if (InputNonClientPointerSourceHandle != IntPtr.Zero)
            {
                int style = GetWindowLongAuto(Handle, WindowLongIndexFlags.GWL_STYLE);
                SetWindowLongAuto(Handle, WindowLongIndexFlags.GWL_STYLE, (IntPtr)(style & ~(int)WindowStyle.WS_SYSMENU));

                newInputNonClientPointerSourceWndProc = new WNDPROC(InputNonClientPointerSourceWndProc);
                oldInputNonClientPointerSourceWndProc = SetWindowLongAuto(InputNonClientPointerSourceHandle, WindowLongIndexFlags.GWL_WNDPROC, Marshal.GetFunctionPointerForDelegate(newInputNonClientPointerSourceWndProc));

                User32Library.SetWindowPos(InputNonClientPointerSourceHandle, IntPtr.Zero, 0, 0, MileXamlHost.Width, MileXamlHost.Height, SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOOWNERZORDER | SetWindowPosFlags.SWP_NOREDRAW | SetWindowPosFlags.SWP_NOZORDER);
            }

            UWPCoreHandle = InteropExtensions.GetInterop(Window.Current.CoreWindow).WindowHandle;
            if (UWPCoreHandle != IntPtr.Zero)
            {
                User32Library.SetWindowPos(UWPCoreHandle, IntPtr.Zero, 0, 0, Size.Width, Size.Height, SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOOWNERZORDER | SetWindowPosFlags.SWP_NOREDRAW | SetWindowPosFlags.SWP_NOZORDER);
            }

            TitleControl.ActualThemeChanged += OnActualThemeChanged;
        }

        private void InitializeComponent()
        {
            components = new Container();
            AutoScaleMode = AutoScaleMode.Font;
        }

        /// <summary>
        /// 处置由主窗体占用的资源
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components is not null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// 设备的每英寸像素 (PPI) 显示更改修改后触发的事件
        /// </summary>
        protected override void OnDpiChanged(DpiChangedEventArgs args)
        {
            base.OnDpiChanged(args);
            WindowDPI = ((double)args.DeviceDpiNew) / 96;

            if (RuntimeHelper.IsWebView2Installed)
            {
                if (WebView2 is not null)
                {
                    WebView2.Location = new Point(0, (int)(MileXamlHost.Height * WindowDPI));
                    WebView2.Size = new Size(ClientSize.Width, (int)(ClientSize.Height - MileXamlHost.Height * WindowDPI));
                }
            }
            else
            {
                if (WebBrowser is not null)
                {
                    WebBrowser.Location = new Point(0, (int)(MileXamlHost.Height * WindowDPI));
                    WebBrowser.Size = new Size(ClientSize.Width, (int)(ClientSize.Height - MileXamlHost.Height * WindowDPI));
                }
            }
        }

        /// <summary>
        /// 关闭窗口时恢复默认状态
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs args)
        {
            base.OnFormClosing(args);
            TitleControl.ActualThemeChanged -= OnActualThemeChanged;

            if (RuntimeHelper.IsWebView2Installed)
            {
                if (WebView2 is not null)
                {
                    try
                    {
                        WebView2.CoreWebView2InitializationCompleted -= OnCoreWebView2InitializationCompleted;
                        WebView2.NavigationCompleted -= OnNavigationCompleted;
                        WebView2.NavigationStarting -= OnNavigationStarting;
                        WebView2.SourceChanged -= OnWebView2SourceChanged;

                        if (WebView2.CoreWebView2 is not null)
                        {
                            WebView2.CoreWebView2.NewWindowRequested -= OnNewWindowRequested;
                        }
                        WebView2.Dispose();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            else
            {
                if (WebBrowser is not null)
                {
                    try
                    {
                        WebBrowser.Navigated -= OnNavigated;
                        WebBrowser.Navigating -= OnNavigating;
                        WebBrowser.NewWindow -= OnNewWindow;
                        WebBrowser.Dispose();
                    }
                    catch (Exception) { }
                }
            }
        }

        /// <summary>
        /// 窗体程序加载时初始化应用程序设置
        /// </summary>
        protected override void OnLoad(EventArgs args)
        {
            base.OnLoad(args);
            Margins FormMargin = new Margins();
            DwmApiLibrary.DwmExtendFrameIntoClientArea(Handle, ref FormMargin);
            SetAppTheme();
            SetWindowBackdrop();
            Invalidate();

            if (!RuntimeHelper.IsWebView2Installed)
            {
                TitleControl.IsEnabled = true;
            }
        }

        /// <summary>
        /// 窗体移动时关闭浮出窗口
        /// </summary>
        protected override void OnMove(EventArgs args)
        {
            base.OnMove(args);
            if (TitleControl.XamlRoot is not null)
            {
                IReadOnlyList<Popup> PopupRoot = VisualTreeHelper.GetOpenPopupsForXamlRoot(TitleControl.XamlRoot);
                foreach (Popup popup in PopupRoot)
                {
                    // 关闭浮出控件
                    if (popup.Child as FlyoutPresenter is not null)
                    {
                        popup.IsOpen = false;
                        break;
                    }

                    // 关闭菜单浮出控件
                    if (popup.Child as MenuFlyoutPresenter is not null)
                    {
                        popup.IsOpen = false;
                        break;
                    }

                    // 关闭日期选择器浮出控件
                    if (popup.Child as DatePickerFlyoutPresenter is not null)
                    {
                        popup.IsOpen = false;
                        break;
                    }

                    // 关闭时间选择器浮出控件
                    if (popup.Child as TimePickerFlyoutPresenter is not null)
                    {
                        popup.IsOpen = false;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 窗口大小改变时发生的事件
        /// </summary>
        protected override void OnSizeChanged(EventArgs args)
        {
            base.OnSizeChanged(args);
            TitleControl.IsWindowMaximized = WindowState == FormWindowState.Maximized;

            if (TitleControl.XamlRoot is not null)
            {
                IReadOnlyList<Popup> PopupRoot = VisualTreeHelper.GetOpenPopupsForXamlRoot(TitleControl.XamlRoot);
                foreach (Popup popup in PopupRoot)
                {
                    // 关闭内容对话框
                    if (popup.Child as ContentDialog is not null)
                    {
                        (popup.Child as ContentDialog).Hide();
                        break;
                    }
                }
            }

            if (InputNonClientPointerSourceHandle != IntPtr.Zero)
            {
                User32Library.SetWindowPos(InputNonClientPointerSourceHandle, IntPtr.Zero, 0, 0, MileXamlHost.Width, MileXamlHost.Height, SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOOWNERZORDER | SetWindowPosFlags.SWP_NOREDRAW | SetWindowPosFlags.SWP_NOZORDER);
            }

            if (UWPCoreHandle != IntPtr.Zero)
            {
                User32Library.SetWindowPos(UWPCoreHandle, IntPtr.Zero, 0, 0, Size.Width, Size.Height, SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOOWNERZORDER | SetWindowPosFlags.SWP_NOREDRAW | SetWindowPosFlags.SWP_NOZORDER);
            }

            if (RuntimeHelper.IsWebView2Installed)
            {
                if (WebView2 is not null)
                {
                    WebView2.Location = new Point(0, (int)(MileXamlHost.Height * WindowDPI));
                    WebView2.Size = new Size(ClientSize.Width, (int)(ClientSize.Height - MileXamlHost.Height * WindowDPI));
                }
            }
            else
            {
                if (WebBrowser is not null)
                {
                    WebBrowser.Location = new Point(0, (int)(MileXamlHost.Height * WindowDPI));
                    WebBrowser.Size = new Size(ClientSize.Width, (int)(ClientSize.Height - MileXamlHost.Height * WindowDPI));
                }
            }
        }

        /// <summary>
        /// 应用主题发生变化时修改应用的背景色
        /// </summary>
        private void OnActualThemeChanged(FrameworkElement sender, object args)
        {
            SetTitleBarColor(sender.ActualTheme);
        }

        /// <summary>
        /// 初始化 CoreWebView2 对象
        /// </summary>
        private void OnCoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs args)
        {
            if (WebView2.CoreWebView2 is not null)
            {
                WebView2.CoreWebView2.NewWindowRequested += OnNewWindowRequested;
                TitleControl.IsEnabled = true;
            }
        }

        /// <summary>
        /// WebView2 : 页面完成导航
        /// </summary>
        private void OnNavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            TitleControl.IsLoading = false;
        }

        /// <summary>
        /// WebView2 : 页面开始导航
        /// </summary>
        private void OnNavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs args)
        {
            TitleControl.IsLoading = true;
        }

        /// <summary>
        /// WebView2 : 当前页面对应的链接发生改变时触发这一事件
        /// </summary>
        private void OnWebView2SourceChanged(object sender, CoreWebView2SourceChangedEventArgs args)
        {
            TitleControl.CanGoBack = WebView2.CanGoBack;
            TitleControl.CanGoForward = WebView2.CanGoForward;
        }

        /// <summary>
        /// WebView2 : 捕捉打开新窗口事件，并禁止弹窗
        /// </summary>
        private void OnNewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs args)
        {
            WebBrowser.Navigate(args.Uri);
            args.Handled = true;
        }

        /// <summary>
        /// Winform WebBrowser : 当前页面对应的链接发生改变时触发这一事件
        /// </summary>
        private void OnNavigated(object sender, WebBrowserNavigatedEventArgs args)
        {
            TitleControl.CanGoBack = WebBrowser.CanGoBack;
            TitleControl.CanGoForward = WebBrowser.CanGoForward;
            TitleControl.IsLoading = false;
        }

        /// <summary>
        /// Winform WebBrowser : 当前页面对应的链接发生改变时触发这一事件
        /// </summary>
        private void OnNavigating(object sender, WebBrowserNavigatingEventArgs args)
        {
            TitleControl.IsLoading = true;
        }

        /// <summary>
        /// Winform WebBrowser : 捕捉打开新窗口事件，并禁止弹窗
        /// </summary>
        private void OnNewWindow(object sender, CancelEventArgs args)
        {
            args.Cancel = true;
            WebBrowser.Navigate(WebBrowser.StatusText.ToString());
        }

        /// <summary>
        /// 更改指定窗口的属性
        /// </summary>
        private int GetWindowLongAuto(IntPtr hWnd, WindowLongIndexFlags nIndex)
        {
            if (IntPtr.Size is 8)
            {
                return User32Library.GetWindowLongPtr(hWnd, nIndex);
            }
            else
            {
                return User32Library.GetWindowLong(hWnd, nIndex);
            }
        }

        /// <summary>
        /// 更改指定窗口的窗口属性
        /// </summary>
        private IntPtr SetWindowLongAuto(IntPtr hWnd, WindowLongIndexFlags nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size is 8)
            {
                return User32Library.SetWindowLongPtr(hWnd, nIndex, dwNewLong);
            }
            else
            {
                return User32Library.SetWindowLong(hWnd, nIndex, dwNewLong);
            }
        }

        /// <summary>
        /// 处理 Windows 消息
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                // 系统设置发生变化时的消息
                case (int)WindowMessage.WM_SETTINGCHANGE:
                    {
                        SetAppTheme();
                        break;
                    }
                // 当用户按下鼠标左键时，光标位于窗口的非工作区内的消息
                case (int)WindowMessage.WM_NCLBUTTONDOWN:
                    {
                        if (TitleControl.TitlebarMenuFlyout.IsOpen)
                        {
                            TitleControl.TitlebarMenuFlyout.Hide();
                        }
                        break;
                    }
                // 选择窗口右键菜单的条目时接收到的消息
                case (int)WindowMessage.WM_SYSCOMMAND:
                    {
                        SystemCommand sysCommand = (SystemCommand)(m.WParam.ToInt32() & 0xFFF0);

                        if (sysCommand is SystemCommand.SC_MOUSEMENU || sysCommand is SystemCommand.SC_KEYMENU)
                        {
                            FlyoutShowOptions options = new FlyoutShowOptions();
                            options.Position = new Windows.Foundation.Point(0, 0);
                            options.ShowMode = FlyoutShowMode.Standard;
                            TitleControl.TitlebarMenuFlyout.ShowAt(null, options);
                            return;
                        }
                        break;
                    }
                // 任务栏窗口右键点击后的消息
                case (int)WindowMessage.WM_SYSMENU:
                    {
                        if (WindowState is FormWindowState.Minimized)
                        {
                            WindowState = FormWindowState.Normal;
                        }
                        break;
                    }
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// 应用拖拽区域窗口消息处理
        /// </summary>
        private IntPtr InputNonClientPointerSourceWndProc(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam)
        {
            switch (Msg)
            {
                // 当用户按下鼠标左键时，光标位于窗口的非工作区内的消息
                case WindowMessage.WM_NCLBUTTONDOWN:
                    {
                        if (TitleControl.TitlebarMenuFlyout.IsOpen)
                        {
                            TitleControl.TitlebarMenuFlyout.Hide();
                        }
                        break;
                    }
                // 当用户按下鼠标右键时，光标位于窗口的非工作区内的消息
                case WindowMessage.WM_NCRBUTTONDOWN:
                    {
                        if (TitleControl is not null && TitleControl.XamlRoot is not null)
                        {
                            Point ms = MousePosition;
                            FlyoutShowOptions options = new FlyoutShowOptions();
                            options.Placement = FlyoutPlacementMode.BottomEdgeAlignedLeft;
                            options.ShowMode = FlyoutShowMode.Standard;
                            options.Position = InfoHelper.SystemVersion.Build >= 22000 ?
                                new Windows.Foundation.Point((ms.X - Location.X - 8) / WindowDPI, (ms.Y - Location.Y) / WindowDPI) :
                                new Windows.Foundation.Point(ms.X - Location.X - 8, ms.Y - Location.Y);
                            TitleControl.TitlebarMenuFlyout.ShowAt(null, options);
                        }
                        return IntPtr.Zero;
                    }
            }
            return User32Library.CallWindowProc(oldInputNonClientPointerSourceWndProc, hWnd, Msg, wParam, lParam);
        }

        /// <summary>
        /// 设置标题栏按钮的颜色
        /// </summary>
        private void SetTitleBarColor(ElementTheme theme)
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
                titleBar.ButtonHoverBackgroundColor = Windows.UI.Color.FromArgb(20, 0, 0, 0);
                titleBar.ButtonHoverForegroundColor = Colors.Black;
                titleBar.ButtonPressedBackgroundColor = Windows.UI.Color.FromArgb(30, 0, 0, 0);
                titleBar.ButtonPressedForegroundColor = Colors.Black;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveForegroundColor = Colors.Black;
            }
            else
            {
                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonForegroundColor = Colors.White;
                titleBar.ButtonHoverBackgroundColor = Windows.UI.Color.FromArgb(20, 255, 255, 255);
                titleBar.ButtonHoverForegroundColor = Colors.White;
                titleBar.ButtonPressedBackgroundColor = Windows.UI.Color.FromArgb(30, 255, 255, 255);
                titleBar.ButtonPressedForegroundColor = Colors.White;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveForegroundColor = Colors.White;
            }
        }

        /// <summary>
        /// 设置应用的主题色
        /// </summary>
        public void SetAppTheme()
        {
            if (Windows.UI.Xaml.Application.Current.RequestedTheme is ApplicationTheme.Light)
            {
                int useLightMode = 0;
                DwmApiLibrary.DwmSetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, ref useLightMode, Marshal.SizeOf(typeof(int)));
            }
            else
            {
                int useDarkMode = 1;
                DwmApiLibrary.DwmSetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDarkMode, Marshal.SizeOf(typeof(int)));
            }
        }

        /// <summary>
        /// 添加窗口背景色
        /// </summary>
        public void SetWindowBackdrop()
        {
            int micaBackdrop = 2;
            DwmApiLibrary.DwmSetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE, ref micaBackdrop, Marshal.SizeOf(typeof(int)));
        }
    }
}
