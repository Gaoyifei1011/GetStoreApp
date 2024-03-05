using GetStoreAppWebView.Helpers.Root;
using GetStoreAppWebView.Services.Controls.Settings;
using GetStoreAppWebView.Services.Root;
using GetStoreAppWebView.Views.Controls;
using GetStoreAppWebView.WindowsAPI.PInvoke.Comctl32;
using GetStoreAppWebView.WindowsAPI.PInvoke.User32;
using Microsoft.UI;
using Microsoft.UI.Content;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Media;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;

namespace GetStoreAppWebView.Views.Windows
{
    /// <summary>
    /// 应用主窗口
    /// </summary>
    public class MainWindow : Window
    {
        private IntPtr inputNonClientPointerSourceHandle;
        private IslandsControl islandsControl = new IslandsControl();

        private SUBCLASSPROC mainWindowSubClassProc;
        private SUBCLASSPROC desktopSourceSubClassProc;
        private SUBCLASSPROC inputNonClientPointerSourceSubClassProc;

        public double WindowDPI { get; private set; }

        public IntPtr Handle { get; private set; }

        public AppWindow AppWindow { get; private set; }

        public DesktopWindowXamlSource DesktopWindowXamlSource { get; private set; } = new DesktopWindowXamlSource();

        public MainWindow()
        {
            AllowDrop = false;
            Height = MinHeight = 700;
            Width = MinWidth = 1000;
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;
            Title = ResourceService.GetLocalized("WebView/Title");
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        /// <summary>
        /// 设备的每英寸像素 (PPI) 显示更改修改后触发的事件
        /// </summary>
        protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
        {
            base.OnDpiChanged(oldDpi, newDpi);
            WindowDPI = newDpi.PixelsPerDip;
        }

        /// <summary>
        /// 关闭窗口时释放资源
        /// </summary>
        protected override void OnClosing(CancelEventArgs args)
        {
            base.OnClosing(args);
            islandsControl.CloseWindow();
        }

        /// <summary>
        /// 引发此事件以支持与 Win32 的互操作
        /// </summary>
        protected override void OnSourceInitialized(EventArgs args)
        {
            base.OnSourceInitialized(args);

            Handle = new WindowInteropHelper(this).Handle;
            WindowDPI = System.Drawing.Graphics.FromHwnd(Handle).DpiX / 96;

            mainWindowSubClassProc = new SUBCLASSPROC(MainWindowSubClassProc);
            Comctl32Library.SetWindowSubclass(Handle, mainWindowSubClassProc, 0, IntPtr.Zero);

            WindowId windowId = new WindowId() { Value = (ulong)Handle };
            AppWindow = AppWindow.GetFromWindowId(windowId);
            AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;

            DesktopWindowXamlSource.Initialize(windowId);
            DesktopWindowXamlSource.Content = islandsControl;
            DesktopWindowXamlSource.SystemBackdrop = InfoHelper.SystemVersion.Build >= 22000 ? new MicaBackdrop() : new DesktopAcrylicBackdrop();

            desktopSourceSubClassProc = new SUBCLASSPROC(OnDesktopSourceSubClassProc);
            Comctl32Library.SetWindowSubclass((IntPtr)DesktopWindowXamlSource.SiteBridge.WindowId.Value, desktopSourceSubClassProc, 0, IntPtr.Zero);

            inputNonClientPointerSourceHandle = User32Library.FindWindowEx(Handle, IntPtr.Zero, "InputNonClientPointerSource", null);

            if (inputNonClientPointerSourceHandle != IntPtr.Zero)
            {
                inputNonClientPointerSourceSubClassProc = new SUBCLASSPROC(InputNonClientPointerSourceSubClassProc);
                Comctl32Library.SetWindowSubclass(inputNonClientPointerSourceHandle, inputNonClientPointerSourceSubClassProc, 0, IntPtr.Zero);
            }

            if (WebKernelService.WebKernel == WebKernelService.WebKernelList[0])
            {
                islandsControl.InitializeWebBrowser();
            }
            else
            {
                DesktopWindowXamlSource.SiteBridge.ResizePolicy = ContentSizePolicy.ResizeContentToParentWindow;
            }

            DesktopWindowXamlSource.SiteBridge.Show();
        }

        /// <summary>
        /// 窗口状态发生改变时触发的事件
        /// </summary>
        protected override void OnStateChanged(EventArgs args)
        {
            base.OnStateChanged(args);
            islandsControl.IsWindowMaximized = WindowState is WindowState.Maximized;
        }

        /// <summary>
        /// 应用主窗口消息处理
        /// </summary>
        private IntPtr MainWindowSubClassProc(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam, uint uIdSubclass, IntPtr dwRefData)
        {
            switch (Msg)
            {
                case WindowMessage.WM_SIZE:
                    {
                        if (WebKernelService.WebKernel == WebKernelService.WebKernelList[0] && DesktopWindowXamlSource.SiteBridge is not null)
                        {
                            User32Library.SetWindowPos((IntPtr)DesktopWindowXamlSource.SiteBridge.WindowId.Value,
                              IntPtr.Zero, 0, 0, lParam.ToInt32() & 0xFFFF, Convert.ToInt32(islandsControl.ActualHeight * WindowDPI), SetWindowPosFlags.SWP_NOOWNERZORDER | SetWindowPosFlags.SWP_NOZORDER);
                        }
                        else
                        {
                            islandsControl.CloseDownloadDialog();
                        }
                        break;
                    }
                case WindowMessage.WM_MOVE:
                    {
                        islandsControl?.CloseDownloadDialog();
                        break;
                    }
                // 选择窗口右键菜单的条目时接收到的消息
                case WindowMessage.WM_SYSCOMMAND:
                    {
                        SystemCommand sysCommand = (SystemCommand)(wParam.ToInt32() & 0xFFF0);

                        if (sysCommand is SystemCommand.SC_MOUSEMENU || sysCommand is SystemCommand.SC_KEYMENU)
                        {
                            FlyoutShowOptions options = new FlyoutShowOptions();
                            options.Position = new global::Windows.Foundation.Point(0, 45);
                            options.ShowMode = FlyoutShowMode.Standard;
                            islandsControl.TitlebarMenuFlyout.ShowAt(null, options);
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
                        if (islandsControl.TitlebarMenuFlyout.IsOpen)
                        {
                            islandsControl.TitlebarMenuFlyout.Hide();
                        }
                        break;
                    }
                // 当用户按下鼠标右键时，光标位于窗口的非工作区内的消息
                case WindowMessage.WM_NCRBUTTONUP:
                    {
                        if (islandsControl is not null && islandsControl.XamlRoot is not null)
                        {
                            Point screenPoint = new Point(lParam.ToInt32() & 0xFFFF, lParam.ToInt32() >> 16);
                            RECT rect = new RECT();
                            User32Library.GetWindowRect(Handle, ref rect);

                            FlyoutShowOptions options = new FlyoutShowOptions();
                            options.Placement = FlyoutPlacementMode.BottomEdgeAlignedLeft;
                            options.ShowMode = FlyoutShowMode.Standard;
                            if (InfoHelper.SystemVersion.Build >= 22000)
                            {
                                if (WindowState is WindowState.Maximized)
                                {
                                    options.Position = new global::Windows.Foundation.Point((screenPoint.X - rect.Left - 8) / WindowDPI, (screenPoint.Y - rect.Top - 8) / WindowDPI);
                                }
                                else
                                {
                                    options.Position = new global::Windows.Foundation.Point((screenPoint.X - rect.Left - 8) / WindowDPI, (screenPoint.Y - rect.Top) / WindowDPI);
                                }
                            }
                            else
                            {
                                if (WindowState is WindowState.Maximized)
                                {
                                    options.Position = new global::Windows.Foundation.Point(screenPoint.X - rect.Left - 8, screenPoint.Y - rect.Top);
                                }
                                else
                                {
                                    options.Position = new global::Windows.Foundation.Point(screenPoint.X - rect.Left - 8, screenPoint.Y - rect.Top - 8);
                                }
                            }

                            islandsControl.TitlebarMenuFlyout.ShowAt(null, options);
                        }
                        return IntPtr.Zero;
                    }
            }
            return Comctl32Library.DefSubclassProc(hWnd, Msg, wParam, lParam);
        }

        /// <summary>
        /// WinUI 3 Islands 子窗口消息处理
        /// </summary>
        private IntPtr OnDesktopSourceSubClassProc(IntPtr hWnd, WindowMessage uMsg, IntPtr wParam, IntPtr lParam, uint uIdSubclass, IntPtr dwRefData)
        {
            if (uMsg is WindowMessage.WM_ERASEBKGND || uMsg is WindowMessage.WM_NCPAINT)
            {
                return IntPtr.Zero;
            }

            return Comctl32Library.DefSubclassProc(hWnd, uMsg, wParam, lParam);
        }
    }
}
