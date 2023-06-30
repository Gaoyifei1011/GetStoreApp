using GetStoreApp.Helpers.Root;
using GetStoreApp.Helpers.Window;
using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.PInvoke.DwmApi;
using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using Microsoft.UI.Windowing;
using System;
using System.Runtime.InteropServices;
using Windows.Graphics;
using WinRT.Interop;

namespace GetStoreApp.Views.Window
{
    /// <summary>
    /// 应用任务栏右键菜单窗口
    /// </summary>
    public sealed partial class TrayMenuWindow : WinUIWindow
    {
        private WNDPROC newWndProc = null;
        private IntPtr oldWndProc = IntPtr.Zero;

        public TrayMenuWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 获取主窗口的窗口句柄
        /// </summary>
        public IntPtr GetWindowHandle()
        {
            IntPtr MainWindowHandle = WindowNative.GetWindowHandle(this);

            return MainWindowHandle != IntPtr.Zero
                ? MainWindowHandle
                : throw new ApplicationException(ResourceService.GetLocalized("Resources/WindowHandleInitializeFailed"));
        }

        public void InitializeWindow()
        {
            IntPtr MainWindowHandle = GetWindowHandle();
            newWndProc = new WNDPROC(NewWindowProc);
            oldWndProc = SetWindowLongAuto(MainWindowHandle, WindowLongIndexFlags.GWL_WNDPROC, Marshal.GetFunctionPointerForDelegate(newWndProc));

            int setValue = 0;
            int setResult = DwmApiLibrary.DwmSetWindowAttribute(Program.ApplicationRoot.TrayMenuWindow.GetWindowHandle(), DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, ref setValue, Marshal.SizeOf<int>());
            if (setResult is not 0)
            {
                DwmApiLibrary.DwmSetWindowAttribute(Program.ApplicationRoot.TrayMenuWindow.GetWindowHandle(), DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE_OLD, ref setValue, Marshal.SizeOf<int>());
            }

            // 使用重叠的配置显示应用窗口。
            OverlappedPresenter presenter = OverlappedPresenter.CreateForContextMenu();
            presenter.IsAlwaysOnTop = true;
            AppWindow.SetPresenter(presenter);

            // 设置窗口扩展样式为工具窗口
            SetWindowLongAuto(GetWindowHandle(), WindowLongIndexFlags.GWL_EXSTYLE, (IntPtr)WindowStyleEx.WS_EX_TOOLWINDOW);

            AppWindow.MoveAndResize(new RectInt32(0, 0, 0, 0));
            AppWindow.Show();
            AppWindow.Hide();
        }

        /// <summary>
        /// 设置窗口的位置
        /// </summary>
        public void SetWindowPosition(APPBARDATA appbarData, MONITORINFO monitorInfo, PointInt32 windowPos)
        {
            switch (appbarData.uEdge)
            {
                // 任务栏位置在屏幕左侧
                // 此时只需要判断窗口底部是否超过屏幕的底部边界
                case AppBarEdge.ABE_LEFT:
                    {
                        bool outofScreen = windowPos.Y + AppWindow.Size.Height > monitorInfo.rcMonitor.bottom;
                        if (outofScreen)
                        {
                            AppWindow.Move(new PointInt32(windowPos.X, windowPos.Y - AppWindow.Size.Height));
                        }
                        else
                        {
                            AppWindow.Move(new PointInt32(windowPos.X, windowPos.Y));
                        }
                        break;
                    }
                // 任务栏位置在屏幕顶部
                // 此时只需判断窗口右侧是否超过屏幕的右侧边界
                case AppBarEdge.ABE_TOP:
                    {
                        bool outofScreen = windowPos.X + AppWindow.Size.Width > monitorInfo.rcMonitor.right;
                        if (outofScreen)
                        {
                            AppWindow.Move(new PointInt32(windowPos.X - AppWindow.Size.Width, windowPos.Y));
                        }
                        else
                        {
                            AppWindow.Move(new PointInt32(windowPos.X, windowPos.Y));
                        }
                        break;
                    }
                // 任务栏位置在屏幕右侧
                // 此时只需要判断窗口底部是否超过屏幕的底部边界
                case AppBarEdge.ABE_RIGHT:
                    {
                        bool outofScreen = windowPos.Y + AppWindow.Size.Height > monitorInfo.rcMonitor.bottom;
                        if (outofScreen)
                        {
                            AppWindow.Move(new PointInt32(windowPos.X - AppWindow.Size.Width, windowPos.Y - AppWindow.Size.Height));
                        }
                        else
                        {
                            AppWindow.Move(new PointInt32(windowPos.X - AppWindow.Size.Width, windowPos.Y));
                        }
                        break;
                    }
                // 任务栏位置在屏幕底部
                // 此时只需判断窗口右侧是否超过屏幕的右侧边界
                case AppBarEdge.ABE_BOTTOM:
                    {
                        bool outofScreen = windowPos.X + AppWindow.Size.Width > monitorInfo.rcMonitor.right;
                        if (outofScreen)
                        {
                            AppWindow.Move(new PointInt32(windowPos.X - AppWindow.Size.Width, windowPos.Y - AppWindow.Size.Height));
                        }
                        else
                        {
                            AppWindow.Move(new PointInt32(windowPos.X, windowPos.Y - AppWindow.Size.Height));
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 调整窗口的大小
        /// </summary>
        public void SetWindowSize()
        {
            AppWindow.Resize(new SizeInt32(
                DPICalcHelper.ConvertEpxToPixel(Program.ApplicationRoot.TrayIcon.Handle, Convert.ToInt32(TrayMenuFlyout.ActualWidth) + 2),
                DPICalcHelper.ConvertEpxToPixel(Program.ApplicationRoot.TrayIcon.Handle, Convert.ToInt32(TrayMenuFlyout.ActualHeight) + 2)
                ));
        }

        /// <summary>
        /// 窗口消息处理
        /// </summary>
        private IntPtr NewWindowProc(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam)
        {
            switch (Msg)
            {
                // 窗口大小发生更改时的消息
                case WindowMessage.WM_GETMINMAXINFO:
                    {
                        MINMAXINFO minMaxInfo = Marshal.PtrToStructure<MINMAXINFO>(lParam);
                        if (MinWidth >= 0)
                        {
                            minMaxInfo.ptMinTrackSize.X = DPICalcHelper.ConvertEpxToPixel(hWnd, MinWidth);
                        }
                        if (MinHeight >= 0)
                        {
                            minMaxInfo.ptMinTrackSize.Y = DPICalcHelper.ConvertEpxToPixel(hWnd, MinHeight);
                        }
                        if (MaxWidth > 0)
                        {
                            minMaxInfo.ptMaxTrackSize.X = DPICalcHelper.ConvertEpxToPixel(hWnd, MaxWidth);
                        }
                        if (MaxHeight > 0)
                        {
                            minMaxInfo.ptMaxTrackSize.Y = DPICalcHelper.ConvertEpxToPixel(hWnd, MaxHeight);
                        }

                        minMaxInfo.ptMinTrackSize.Y = 0;
                        Marshal.StructureToPtr(minMaxInfo, lParam, true);
                        break;
                    }
                // 系统设置选项发生更改时的消息
                case WindowMessage.WM_SETTINGCHANGE:
                    {
                        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
                        {
                            if (ThemeService.NotifyIconMenuTheme == ThemeService.NotifyIconMenuThemeList[1])
                            {
                                ViewModel.WindowTheme = RegistryHelper.GetSystemUsesTheme();
                            }
                        });
                        break;
                    }
            }
            return User32Library.CallWindowProc(oldWndProc, hWnd, Msg, wParam, lParam);
        }
    }
}
