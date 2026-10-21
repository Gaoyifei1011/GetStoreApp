using System.Runtime.InteropServices;
using Windows.Graphics;

namespace GetStoreApp.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 包含有关窗口在屏幕上的位置的信息。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct WINDOWPLACEMENT
    {
        /// <summary>
        /// 结构的长度（以字节为单位）。 在调用 GetWindowPlacement 或 SetWindowPlacement 函数之前，请将此成员设置为 sizeof(WINDOWPLACEMENT)。如果未正确设置此成员，则 GetWindowPlacement 和 SetWindowPlacement 将失败。
        /// </summary>
        internal int length;

        /// <summary>
        /// 控制最小化窗口的位置以及还原窗口的方法的标志。
        /// </summary>
        internal WINDOWPLACEMENTFLAGS flags;

        /// <summary>
        /// 窗口的当前显示状态。 它可以是在 ShowWindow 函数的 nCmdShow 参数中指定的任何值。
        /// </summary>
        internal SHOWWINDOWCMD showCmd;

        /// <summary>
        /// 窗口最小化时窗口左上角的坐标。
        /// </summary>
        internal PointInt32 ptMinPosition;

        /// <summary>
        /// 窗口最大化时窗口左上角的坐标。
        /// </summary>
        internal PointInt32 ptMaxPosition;

        /// <summary>
        /// 窗口处于还原位置时的窗口坐标。
        /// </summary>
        internal RECT rcNormalPosition;
    }
}
