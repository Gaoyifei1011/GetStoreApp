using GetStoreAppHelper.Helpers;
using GetStoreAppHelper.Helpers.Root;
using GetStoreAppHelper.WindowsAPI.PInvoke.User32;
using System;
using Windows.Foundation;
using Windows.Graphics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GetStoreAppHelper.UI.Controls
{
    /// <summary>
    /// 任务栏辅助部分：任务栏右键菜单浮出控件视图
    /// </summary>
    public sealed partial class TrayMenuControl : ContentControl
    {
        public TrayMenuControl()
        {
            InitializeComponent();
        }

        public void SetXamlRoot(XamlRoot xamlRoot)
        {
            TrayMenuFlyout.XamlRoot = xamlRoot;
        }

        public void ShowMenuFlyout(PointInt32 point)
        {
            IntPtr hwnd = User32Library.FindWindow("NotifyIconOverflowWindow", null);

            if (hwnd != IntPtr.Zero)
            {
                User32Library.SetWindowPos(hwnd, new IntPtr(-2), 0, 0, 0, 0, SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOMOVE);

                do
                {
                    hwnd = User32Library.FindWindowEx(hwnd, IntPtr.Zero, "ToolbarWindow32", null);

                    if (hwnd != IntPtr.Zero)
                    {
                        User32Library.SetWindowPos(hwnd, new IntPtr(-2), 0, 0, 0, 0, SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOMOVE);
                    }
                } while (hwnd != IntPtr.Zero);
            }

            int x = DPICalcHelper.ConvertPixelToEpx(Program.ApplicationRoot.MainWindow.Handle, point.X);
            int y = DPICalcHelper.ConvertPixelToEpx(Program.ApplicationRoot.MainWindow.Handle, point.Y);
            User32Library.SetWindowPos(Program.ApplicationRoot.MainWindow.Handle, new IntPtr(-2), 0, 0, 0, 0, SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOMOVE);

            if (InfoHelper.GetSystemVersion().BuildNumber >= 22000)
            {
                TrayMenuFlyout.ShowAt(null, new Point(x, y));
            }
            else
            {
                TrayMenuFlyout.ShowAt(null, new Point(point.X, point.Y));
            }
        }
    }
}
