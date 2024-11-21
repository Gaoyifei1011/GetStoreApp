using System;
using System.Runtime.InteropServices;

namespace GetStoreAppInstaller.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 包含应用程序在处理 WM_NCCALCSIZE 消息时可以使用的信息，以计算窗口工作区的大小、位置和有效内容。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct NCCALCSIZE_PARAMS
    {
        /// <summary>
        /// 矩形数组。 矩形数组的含义在处理 WM_NCCALCSIZE 消息期间发生更改。
        /// 当窗口过程收到 WM_NCCALCSIZE 消息时，第一个矩形包含已移动或调整大小的窗口的新坐标，即建议的新窗口坐标。 第二个包含移动或调整窗口大小之前窗口的坐标。 第三个包含移动或调整窗口大小之前窗口工作区的坐标。 如果窗口是子窗口，则坐标相对于父窗口的工作区。 如果窗口是顶级窗口，则坐标相对于屏幕原点。
        /// 当窗口过程返回时，第一个矩形包含移动或调整大小所产生的新客户端矩形的坐标。 第二个矩形包含有效的目标矩形，第三个矩形包含有效的源矩形。 最后两个矩形与 WM_NCCALCSIZE 消息的返回值结合使用，以确定要保留的窗口区域。
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public RECT[] rgrc;

        /// <summary>
        /// 指向 WINDOWPOS 结构的指针，该结构包含移动窗口或调整窗口大小的操作中指定的大小和位置值。
        /// </summary>
        public IntPtr lppos;
    }
}
