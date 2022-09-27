using System;
using System.Runtime.InteropServices;
using Windows.Win32.Foundation;
using WinUIEx;

namespace GetStoreApp.Helpers
{
    /// <summary>
    /// 应用窗口设置
    /// </summary>
    public static class WindowHelper
    {
        [DllImport("user32.dll")]
        private static extern bool IsZoomed(IntPtr hWnd);

        /// <summary>
        /// 显示窗口
        /// </summary>
        public static void ShowAppWindow()
        {
            // 将窗口置于前台前首先获取窗口句柄
            HWND hwnd = (HWND)WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);

            // 判断窗口状态是否处于最大化状态
            if (IsZoomed(hwnd))
            {
                Windows.Win32.PInvoke.ShowWindow(hwnd, Windows.Win32.UI.WindowsAndMessaging.SHOW_WINDOW_CMD.SW_MAXIMIZE);
            }

            // 其他状态下窗口还原显示状态
            else
            {
                // 还原窗口（如果最小化）时，需要 Microsoft.Windows.CsWin32 NuGet 包和一个带有 ShowWindow() 方法的 NativeMethods.txt 文件
                Windows.Win32.PInvoke.ShowWindow(hwnd, Windows.Win32.UI.WindowsAndMessaging.SHOW_WINDOW_CMD.SW_RESTORE);
            }

            // 将指定窗口的线程设置到前台时，需要 Microsoft.Windows.CsWin32 NuGet 包和一个具有 SetForegroundWindow() 方法的 NativeMethods.txt 文件
            Windows.Win32.PInvoke.SetForegroundWindow(hwnd);
        }

        /// <summary>
        /// 隐藏窗口
        /// </summary>
        public static void HideAppWindow()
        {
            if (App.MainWindow.Visible)
            {
                App.MainWindow.Hide();
            }
        }
    }
}
