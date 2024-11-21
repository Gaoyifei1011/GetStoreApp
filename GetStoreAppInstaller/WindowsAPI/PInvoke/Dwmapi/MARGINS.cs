using System.Runtime.InteropServices;

namespace GetStoreAppInstaller.WindowsAPI.PInvoke.Dwmapi
{
    /// <summary>
    /// 由 GetThemeMargins 函数返回，用于定义应用了视觉样式的窗口边距。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct MARGINS
    {
        /// <summary>
        /// 保留其大小的左边框的宽度。
        /// </summary>
        public int cxLeftWidth;

        /// <summary>
        /// 保留其大小的右边框的宽度。
        /// </summary>
        public int cxRightWidth;

        /// <summary>
        /// 保留其大小的上边框的高度。
        /// </summary>
        public int cyTopHeight;

        /// <summary>
        /// 保留其大小的下边框的高度。
        /// </summary>
        public int cyBottomHeight;
    }
}
