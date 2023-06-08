using GetStoreApp.WindowsAPI.PInvoke.User32;
using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.Shell32
{
    /// <summary>
    /// 包含有关系统应用栏消息的信息。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct APPBARDATA
    {
        /// <summary>
        /// 结构大小（以字节为单位）。
        /// </summary>
        public int cbSize;

        /// <summary>
        /// 应用栏窗口的句柄。 并非所有消息都使用此成员。 请参阅单个消息页，了解是否需要提供 hWind 值。
        /// </summary>
        public IntPtr hWnd;

        /// <summary>
        /// 应用程序定义的消息标识符。 应用程序将指定的标识符用于发送到 由 hWnd 成员标识的应用栏的通知消息。 发送 ABM_NEW 消息时使用此成员。
        /// </summary>
        public int uCallbackMessage;

        /// <summary>
        /// 一个指定屏幕边缘的值。 发送以下消息之一时使用此成员：
        /// <see cref="AppBarMessage.ABM_GETAUTOHIDEBAR">
        /// <see cref="AppBarMessage.ABM_SETAUTOHIDEBAR">
        /// <see cref="AppBarMessage.ABM_GETAUTOHIDEBAREX">
        /// <see cref="AppBarMessage.ABM_SETAUTOHIDEBAREX">
        /// <see cref="AppBarMessage.ABM_QUERYPOS">
        /// <see cref="AppBarMessage.ABM_SETPOS">
        /// </summary>
        public AppBarEdge uEdge;

        /// <summary>
        /// RECT 结构，其用法因消息而异：
        /// <see cref="AppBarMessage.ABM_GETTASKBARPOS">、 <see cref="AppBarMessage.ABM_QUERYPOS">、<see cref="AppBarMessage.ABM_SETPOS">：应用栏或 Windows 任务栏的边框（以屏幕坐标表示）。
        /// <see cref="AppBarMessage.ABM_GETAUTOHIDEBAREX">，<see cref="AppBarMessage.ABM_SETAUTOHIDEBAREX">：正在其上执行操作的监视器。 可以通过 GetMonitorInfo 函数检索此信息。
        /// </summary>
        public RECT rc;

        /// <summary>
        /// 依赖于消息的值。 此成员用于以下消息：
        /// <see cref="AppBarMessage.ABM_SETAUTOHIDEBAR">
        /// <see cref="AppBarMessage.ABM_SETAUTOHIDEBAREX">
        /// <see cref="AppBarMessage.ABM_SETSTATE">
        /// </summary>
        public IntPtr lParam;
    }
}
