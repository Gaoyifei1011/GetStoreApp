using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// <see cref="RECT"> 结构按其左上角和右下角的坐标定义矩形。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct RECT
    {
        /// <summary>
        /// 指定矩形左上角的 x 坐标。
        /// </summary>
        public int left;

        /// <summary>
        /// 指定矩形左上角的 y 坐标。
        /// </summary>
        public int top;

        /// <summary>
        /// 指定矩形右下角的 x 坐标。
        /// </summary>
        public int right;

        /// <summary>
        /// 指定矩形右下角的 y 坐标。
        /// </summary>
        public int bottom;
    }
}
