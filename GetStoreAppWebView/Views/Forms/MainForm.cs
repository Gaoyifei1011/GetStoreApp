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
using System.Text;
using System.Windows.Forms;
using Windows.Foundation.Diagnostics;
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

        private IntPtr InputNonClientPointerSourceHandle;
        private IntPtr UWPCoreHandle;
        private IContainer components = null;
        private TitleControl TitleControl = new TitleControl();

        private WNDPROC newInputNonClientPointerSourceWndProc = null;
        private IntPtr oldInputNonClientPointerSourceWndProc = IntPtr.Zero;

        public WindowsXamlHost MileXamlHost { get; } = new WindowsXamlHost();

        public WebBrowser WebBrowser { get; }

        public WebView2 WebView2 { get; }

        public AppWindow AppWindow { get; }

        public MainForm()
        {
            InitializeComponent();

            AllowDrop = false;
            BackColor = Color.Black;
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
                WebView2.AllowExternalDrop = false;
                WebView2.Location = new Point(0, 0);
                WebView2.Size = new Size(ClientSize.Width, ClientSize.Height);
                WebView2.Click += OnClick;
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
                WebBrowser.AllowWebBrowserDrop = false;
                WebBrowser.Location = new Point(0, 0);
                WebBrowser.Size = new Size(ClientSize.Width, ClientSize.Height);
                WebBrowser.Navigating += OnNavigating;
                WebBrowser.Navigated += OnNavigated;
                WebBrowser.NewWindow += OnNewWindow;
            }

            Microsoft.UI.WindowId windowId = new Microsoft.UI.WindowId();
            windowId.Value = (ulong)Handle;
            AppWindow = AppWindow.GetFromWindowId(windowId);
            AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;

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
        }

        /// <summary>
        /// 关闭窗口时恢复默认状态
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs args)
        {
            base.OnFormClosing(args);

            if (RuntimeHelper.IsWebView2Installed)
            {
                if (WebView2 is not null)
                {
                    try
                    {
                        WebView2.Click -= OnClick;
                        WebView2.CoreWebView2InitializationCompleted -= OnCoreWebView2InitializationCompleted;
                        WebView2.NavigationCompleted -= OnNavigationCompleted;
                        WebView2.NavigationStarting -= OnNavigationStarting;
                        WebView2.SourceChanged -= OnWebView2SourceChanged;

                        if (WebView2.CoreWebView2 is not null)
                        {
                            WebView2.CoreWebView2.NewWindowRequested -= OnNewWindowRequested;
                            WebView2.CoreWebView2.ProcessFailed -= OnProcessFailed;
                        }
                        WebView2.Dispose();
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "WebView2 unloaded failed", e);
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
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "Winform WebBrowser unloaded failed", e);
                    }
                }
            }
        }

        /// <summary>
        /// 窗体程序加载时初始化应用程序设置
        /// </summary>
        protected override void OnLoad(EventArgs args)
        {
            base.OnLoad(args);

            SetAppTheme();
            SetWindowBackdrop();

            if (!RuntimeHelper.IsWebView2Installed)
            {
                TitleControl.IsEnabled = true;
            }

            if (InfoHelper.SystemVersion.Build >= 22621)
            {
                Margins FormMargin = new Margins();
                DwmApiLibrary.DwmExtendFrameIntoClientArea(Handle, ref FormMargin);
                Invalidate();
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
                    // 关闭菜单浮出控件
                    if (popup.Child as MenuFlyoutPresenter is not null)
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
                    // 关闭菜单浮出控件
                    if (popup.Child as MenuFlyoutPresenter is not null)
                    {
                        popup.IsOpen = false;
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
                    WebView2.Location = new Point(0, MileXamlHost.Height);
                    WebView2.Size = new Size(ClientSize.Width, ClientSize.Height - MileXamlHost.Height);
                }
            }
            else
            {
                if (WebBrowser is not null)
                {
                    WebBrowser.Location = new Point(0, MileXamlHost.Height);
                    WebBrowser.Size = new Size(ClientSize.Width, ClientSize.Height - MileXamlHost.Height);
                }
            }
        }

        /// <summary>
        /// 点击控件时关闭浮出控件
        /// </summary>
        private void OnClick(object sender, EventArgs args)
        {
            if (TitleControl.XamlRoot is not null)
            {
                IReadOnlyList<Popup> PopupRoot = VisualTreeHelper.GetOpenPopupsForXamlRoot(TitleControl.XamlRoot);
                foreach (Popup popup in PopupRoot)
                {
                    // 关闭菜单浮出控件
                    if (popup.Child as MenuFlyoutPresenter is not null)
                    {
                        popup.IsOpen = false;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 初始化 CoreWebView2 对象
        /// </summary>
        private void OnCoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs args)
        {
            if (WebView2.CoreWebView2 is not null)
            {
                WebView2.CoreWebView2.NewWindowRequested += OnNewWindowRequested;
                WebView2.CoreWebView2.ProcessFailed += OnProcessFailed;
                TitleControl.IsEnabled = true;
            }
        }

        /// <summary>
        /// WebView2 : 页面完成导航
        /// </summary>
        private void OnNavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            TitleControl.IsLoading = false;
            Text = string.Format("{0} - {1}", WebView2.CoreWebView2.DocumentTitle, ResourceService.GetLocalized("WebView/Title"));
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
            args.Handled = true;
            WebBrowser.Navigate(args.Uri);
        }

        /// <summary>
        /// WebView2 ：进程异常退出时发生
        /// </summary>
        private void OnProcessFailed(object sender, CoreWebView2ProcessFailedEventArgs args)
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

            MessageBox.Show(ResourceService.GetLocalized("WebView/WebViewProcessFailedContent"), ResourceService.GetLocalized("WebView/WebViewProcessFailedTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            Program.ApplicationRoot.Dispose();
        }

        /// <summary>
        /// Winform WebBrowser : 当前页面对应的链接发生改变时触发这一事件
        /// </summary>
        private void OnNavigated(object sender, WebBrowserNavigatedEventArgs args)
        {
            Text = string.Format("{0} - {1}", WebBrowser.DocumentTitle, ResourceService.GetLocalized("WebView/Title"));
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
        /// 获取指定窗口的属性
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
        /// 设置应用的主题色
        /// </summary>
        public void SetAppTheme()
        {
            if (InfoHelper.SystemVersion.Build >= 22621)
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
        }

        /// <summary>
        /// 添加窗口背景色
        /// </summary>
        public void SetWindowBackdrop()
        {
            if (InfoHelper.SystemVersion.Build >= 22621)
            {
                int micaBackdrop = 2;
                DwmApiLibrary.DwmSetWindowAttribute(Handle, DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE, ref micaBackdrop, Marshal.SizeOf(typeof(int)));
            }
        }
    }
}
