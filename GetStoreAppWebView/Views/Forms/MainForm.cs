using GetStoreAppWebView.Helpers.Root;
using GetStoreAppWebView.Services.Controls.Settings;
using GetStoreAppWebView.Services.Root;
using GetStoreAppWebView.Views.Controls;
using GetStoreAppWebView.WindowsAPI.PInvoke.User32;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GetStoreAppWebView.Views.Forms
{
    /// <summary>
    /// 应用主界面
    /// </summary>
    public class MainForm : Form
    {
        private int windowWidth = 1000;
        private int windowHeight = 700;

        private IContainer components = new Container();
        private IntPtr InputNonClientPointerSourceHandle;
        private IslandsControl IslandsControl = new IslandsControl();

        private WNDPROC newInputNonClientPointerSourceWndProc = null;
        private IntPtr oldInputNonClientPointerSourceWndProc = IntPtr.Zero;

        public double WindowDPI { get; private set; }

        public AppWindow AppWindow { get; private set; }

        public DesktopWindowXamlSource DesktopWindowXamlSource { get; private set; } = new DesktopWindowXamlSource();

        public MainForm()
        {
            AllowDrop = false;
            AutoScaleMode = AutoScaleMode.Font;
            Icon = Icon.ExtractAssociatedIcon(Process.GetCurrentProcess().MainModule.FileName);
            WindowDPI = ((double)DeviceDpi) / 96;
            MinimumSize = new Size(Convert.ToInt32(windowWidth * WindowDPI), Convert.ToInt32(windowHeight * WindowDPI));
            Size = new Size(Convert.ToInt32(windowWidth * WindowDPI), Convert.ToInt32(windowHeight * WindowDPI));
            StartPosition = FormStartPosition.CenterParent;
            Text = ResourceService.GetLocalized("WebView/Title");
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
        /// 关闭窗口时释放资源
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs args)
        {
            base.OnFormClosing(args);
            IslandsControl.CloseWindow();
        }

        /// <summary>
        /// 窗口句柄创建完成后引发的事件
        /// </summary>

        protected override void OnHandleCreated(EventArgs args)
        {
            base.OnHandleCreated(args);

            WindowId windowId = new WindowId() { Value = (ulong)Handle };
            AppWindow = AppWindow.GetFromWindowId(windowId);
            AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;

            DesktopWindowXamlSource.Initialize(windowId);
            DesktopWindowXamlSource.Content = IslandsControl;
            DesktopWindowXamlSource.SystemBackdrop = new MicaBackdrop();

            subClassProc = new SUBCLASSPROC(OnSubClassProc);
            SetWindowSubclass((IntPtr)DesktopWindowXamlSource.SiteBridge.WindowId.Value, subClassProc, 100, IntPtr.Zero);

            InputNonClientPointerSourceHandle = User32Library.FindWindowEx(Handle, IntPtr.Zero, "InputNonClientPointerSource", null);

            if (InputNonClientPointerSourceHandle != IntPtr.Zero)
            {
                int style = GetWindowLongAuto(Handle, WindowLongIndexFlags.GWL_STYLE);
                SetWindowLongAuto(Handle, WindowLongIndexFlags.GWL_STYLE, (IntPtr)(style & ~(int)WindowStyle.WS_SYSMENU));

                newInputNonClientPointerSourceWndProc = new WNDPROC(InputNonClientPointerSourceWndProc);
                oldInputNonClientPointerSourceWndProc = SetWindowLongAuto(InputNonClientPointerSourceHandle, WindowLongIndexFlags.GWL_WNDPROC, Marshal.GetFunctionPointerForDelegate(newInputNonClientPointerSourceWndProc));
            }

            if (WebKernelService.WebKernel == WebKernelService.WebKernelList[0])
            {
                IslandsControl.InitializeWebBrowser();
            }

            DesktopWindowXamlSource.SiteBridge.Show();
        }

        /// <summary>
        /// 窗体移动时关闭浮出窗口
        /// </summary>
        protected override void OnMove(EventArgs args)
        {
            base.OnMove(args);
            if (IslandsControl is not null && IslandsControl.XamlRoot is not null)
            {
                IReadOnlyList<Popup> PopupRoot = VisualTreeHelper.GetOpenPopupsForXamlRoot(IslandsControl.XamlRoot);
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
            IslandsControl.IsWindowMaximized = WindowState == FormWindowState.Maximized;

            if (DesktopWindowXamlSource.SiteBridge is not null)
            {
                User32Library.SetWindowPos((IntPtr)Program.MainWindow.DesktopWindowXamlSource.SiteBridge.WindowId.Value,
                  IntPtr.Zero, 0, 0, ClientSize.Width, Convert.ToInt32(IslandsControl.ActualHeight * Program.MainWindow.WindowDPI), SetWindowPosFlags.SWP_NOOWNERZORDER | SetWindowPosFlags.SWP_NOZORDER);
            }

            IslandsControl.ChangeSize(ClientSize.Width, ClientSize.Height);
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
                // 选择窗口右键菜单的条目时接收到的消息
                case (int)WindowMessage.WM_SYSCOMMAND:
                    {
                        SystemCommand sysCommand = (SystemCommand)(m.WParam.ToInt32() & 0xFFF0);

                        if (sysCommand is SystemCommand.SC_MOUSEMENU || sysCommand is SystemCommand.SC_KEYMENU)
                        {
                            FlyoutShowOptions options = new FlyoutShowOptions();
                            options.Position = new Windows.Foundation.Point(0, 0);
                            options.ShowMode = FlyoutShowMode.Standard;
                            IslandsControl.TitlebarMenuFlyout.ShowAt(null, options);
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
                        if (IslandsControl.TitlebarMenuFlyout.IsOpen)
                        {
                            IslandsControl.TitlebarMenuFlyout.Hide();
                        }
                        break;
                    }
                // 当用户按下鼠标右键时，光标位于窗口的非工作区内的消息
                case WindowMessage.WM_NCRBUTTONDOWN:
                    {
                        if (IslandsControl is not null && IslandsControl.XamlRoot is not null)
                        {
                            Point ms = MousePosition;
                            FlyoutShowOptions options = new FlyoutShowOptions();
                            options.Placement = FlyoutPlacementMode.BottomEdgeAlignedLeft;
                            options.ShowMode = FlyoutShowMode.Standard;
                            options.Position = InfoHelper.SystemVersion.Build >= 22000 ?
                                new Windows.Foundation.Point((ms.X - Location.X - 8) / WindowDPI, (ms.Y - Location.Y) / WindowDPI) :
                                new Windows.Foundation.Point(ms.X - Location.X - 8, ms.Y - Location.Y);
                            IslandsControl.TitlebarMenuFlyout.ShowAt(null, options);
                        }
                        return IntPtr.Zero;
                    }
            }
            return User32Library.CallWindowProc(oldInputNonClientPointerSourceWndProc, hWnd, Msg, wParam, lParam);
        }

        private const uint WM_ERASEBKGND = 0x14;
        private const uint WM_NCPAINT = 0x85;

        [DllImport("comctl32.dll")]
        private static extern bool SetWindowSubclass(IntPtr hWnd, SUBCLASSPROC pfnSubclass, uint uIdSubclass, IntPtr dwRefData);

        [DllImport("comctl32.dll")]
        private static extern IntPtr DefSubclassProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        private delegate IntPtr SUBCLASSPROC(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, uint uIdSubclass, IntPtr dwRefData);

        SUBCLASSPROC subClassProc;

        private IntPtr OnSubClassProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, uint uIdSubclass, IntPtr dwRefData)
        {
            if (uMsg == WM_ERASEBKGND || uMsg == WM_NCPAINT) return IntPtr.Zero;

            return DefSubclassProc(hWnd, uMsg, wParam, lParam);
        }
    }
}
