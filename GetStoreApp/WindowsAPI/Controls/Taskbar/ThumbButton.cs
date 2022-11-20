using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.Controls.Taskbar
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct ThumbButton
    {
        /// <summary>
        /// 单击按钮的 WPARAM 值
        /// </summary>
        internal const int Clicked = 0x1800;

        [MarshalAs(UnmanagedType.U4)]
        internal ThumbButtonMask Mask;

        internal uint Id;
        internal uint Bitmap;
        internal IntPtr Icon;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        internal string Tip;

        [MarshalAs(UnmanagedType.U4)]
        internal ThumbButtonOptions Flags;
    }
}
