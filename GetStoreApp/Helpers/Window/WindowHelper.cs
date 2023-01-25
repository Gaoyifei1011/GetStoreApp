using GetStoreApp.WindowsAPI.PInvoke.User32;

namespace GetStoreApp.Helpers.Window
{
    /// <summary>
    /// 应用窗口辅助类
    /// </summary>
    public static class WindowHelper
    {
        /// <summary>
        /// 判断窗口是否最大化
        /// </summary>
        public static bool IsWindowMaximized()
        {
            return User32Library.IsZoomed(Program.ApplicationRoot.MainWindow.GetMainWindowHandle());
        }

        /// <summary>
        /// 显示窗口
        /// </summary>
        public static void ShowAppWindow()
        {
            // 判断窗口状态是否处于最大化状态，如果是，直接最大化窗口
            if (IsWindowMaximized())
            {
                User32Library.ShowWindow(Program.ApplicationRoot.MainWindow.GetMainWindowHandle(), WindowShowStyle.SW_MAXIMIZE);
            }

            // 其他状态下窗口还原显示状态
            else
            {
                // 还原窗口（如果最小化）时
                Program.ApplicationRoot.AppWindow.Show();
            }

            // 将应用窗口设置到前台
            User32Library.SetForegroundWindow(Program.ApplicationRoot.MainWindow.GetMainWindowHandle());
        }

        /// <summary>
        /// 隐藏窗口
        /// </summary>
        public static void HideAppWindow()
        {
            if (Program.ApplicationRoot.AppWindow.IsVisible)
            {
                Program.ApplicationRoot.AppWindow.Hide();
            }
        }

        /// <summary>
        /// 设置应用窗口置顶
        /// </summary>
        public static void SetAppTopMost(bool topMostValue)
        {
            if (topMostValue)
            {
                User32Library.SetWindowPos(Program.ApplicationRoot.MainWindow.GetMainWindowHandle(), SpecialWindowHandles.HWND_TOPMOST, 0, 0, 0, 0,
                SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE);
            }
            else
            {
                User32Library.SetWindowPos(Program.ApplicationRoot.MainWindow.GetMainWindowHandle(), SpecialWindowHandles.HWND_NOTOPMOST, 0, 0, 0, 0,
                SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE);
            }
        }
    }
}
