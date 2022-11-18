using GetStoreAppWindowsAPI.PInvoke.User32;
using WinRT.Interop;

namespace GetStoreApp.Helpers.Window
{
    /// <summary>
    /// 应用窗口设置
    /// </summary>
    public static class WindowHelper
    {
        /// <summary>
        /// 显示窗口
        /// </summary>
        public static void ShowAppWindow()
        {
            // 判断窗口状态是否处于最大化状态，如果是，直接最大化窗口
            if (User32Library.IsZoomed(WindowNative.GetWindowHandle(App.MainWindow)))
            {
                User32Library.ShowWindow(WindowNative.GetWindowHandle(App.MainWindow), WindowShowStyle.SW_MAXIMIZE);
            }

            // 其他状态下窗口还原显示状态
            else
            {
                // 还原窗口（如果最小化）时
                User32Library.ShowWindow(WindowNative.GetWindowHandle(App.MainWindow), WindowShowStyle.SW_RESTORE);
            }

            // 将应用窗口设置到前台
            User32Library.SwitchToThisWindow(WindowNative.GetWindowHandle(App.MainWindow), true);
        }

        /// <summary>
        /// 隐藏窗口
        /// </summary>
        public static void HideAppWindow()
        {
            if (App.MainWindow.Visible)
            {
                User32Library.ShowWindow(WindowNative.GetWindowHandle(App.MainWindow), WindowShowStyle.SW_HIDE);
            }
        }

        /// <summary>
        /// 设置应用窗口置顶
        /// </summary>
        public static void SetAppTopMost(bool topMostValue)
        {
            if (topMostValue)
            {
                User32Library.SetWindowPos(WindowNative.GetWindowHandle(App.MainWindow), SpecialWindowHandles.HWND_TOPMOST, 0, 0, 0, 0,
                SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE);
            }
            else
            {
                User32Library.SetWindowPos(WindowNative.GetWindowHandle(App.MainWindow), SpecialWindowHandles.HWND_NOTOPMOST, 0, 0, 0, 0,
                SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE);
            }
        }
    }
}
