using GetStoreApp.WindowsAPI.PInvoke.User32;
using System;

namespace GetStoreApp.Extensions.SystemTray
{
    public class NotifyIconManager
    {
        private static RECT rect;

        /// <summary>
        /// 程序关闭后刷新托盘区域的图标
        /// </summary>
        public static void RefreshNotification()
        {
            IntPtr NotifyAreaHandle = GetNotifyAreaHandle();
            if (NotifyAreaHandle != IntPtr.Zero)
            {
                RefreshWindow(NotifyAreaHandle);
            }

            IntPtr NotifyOverHandle = GetNotifyOverHandle();
            if (NotifyOverHandle != IntPtr.Zero)
            {
                RefreshWindow(NotifyOverHandle);
            }
        }

        /// <summary>
        /// 刷新窗口
        /// </summary>
        private static void RefreshWindow(IntPtr windowHandle)
        {
            User32Library.GetClientRect(windowHandle, out rect);
            for (int x = 0; x < rect.right; x += 5)
            {
                for (int y = 0; y < rect.bottom; y += 5)
                {
                    User32Library.PostMessage(windowHandle, WindowMessage.WM_MOUSEMOVE, 0, (y << 16) + x);
                }
            }
        }

        /// <summary>
        /// 获取显示通知区域的窗口句柄
        /// </summary>
        private static IntPtr GetNotifyAreaHandle()
        {
            IntPtr NotifyAreaHandle = IntPtr.Zero;
            IntPtr TrayWndHandle = User32Library.FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Shell_TrayWnd", string.Empty);
            if (User32Library.FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Shell_TrayWnd", string.Empty) != IntPtr.Zero)
            {
                IntPtr TrayNotifyWndHandle = User32Library.FindWindowEx(TrayWndHandle, IntPtr.Zero, "TrayNotifyWnd", string.Empty);
                if (TrayNotifyWndHandle != IntPtr.Zero)
                {
                    IntPtr SysPagerHandle = User32Library.FindWindowEx(TrayNotifyWndHandle, IntPtr.Zero, "SysPager", string.Empty);
                    if (SysPagerHandle != IntPtr.Zero)
                    {
                        NotifyAreaHandle = User32Library.FindWindowEx(SysPagerHandle, IntPtr.Zero, "ToolbarWindow32", string.Empty);
                    }
                }
            }
            return NotifyAreaHandle;
        }

        /// <summary>
        /// 获取溢出通知区域的窗口句柄
        /// </summary>
        private static IntPtr GetNotifyOverHandle()
        {
            IntPtr NotifyOverHandle = IntPtr.Zero;
            IntPtr OverHandle = User32Library.FindWindowEx(IntPtr.Zero, IntPtr.Zero, "NotifyIconOverflowWindow", string.Empty);
            if (OverHandle != IntPtr.Zero)
            {
                NotifyOverHandle = User32Library.FindWindowEx(OverHandle, IntPtr.Zero, "ToolbarWindow32", string.Empty);
            }

            return NotifyOverHandle;
        }
    }
}
