using System.Runtime.InteropServices;

namespace GetStoreAppInstaller.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// RECT 结构通过矩形的左上角和右下角的坐标来定义矩形。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct RECT
    {
        /// <summary>
        /// 指定矩形左上角的 x 坐标。
        /// </summary>
        internal int left;

        /// <summary>
        /// 指定矩形左上角的 y 坐标。
        /// </summary>
        internal int top;

        /// <summary>
        /// 指定矩形右下角的 x 坐标。
        /// </summary>
        internal int right;

        /// <summary>
        /// 指定矩形右下角的 y 坐标。
        /// </summary>
        internal int bottom;
    }
}
