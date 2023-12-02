using System.Runtime.InteropServices;

namespace GetStoreAppWebView.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// RECT 结构通过矩形的左上角和右下角的坐标来定义矩形。
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        /// <summary>
        /// 指定矩形左上角的 x 坐标。
        /// </summary>
        public int Left;

        /// <summary>
        /// 指定矩形左上角的 y 坐标。
        /// </summary>
        public int Top;

        /// <summary>
        /// 指定矩形右下角的 x 坐标。
        /// </summary>
        public int Right;

        /// <summary>
        /// 指定矩形右下角的 y 坐标。
        /// </summary>
        public int Bottom;
    }
}
