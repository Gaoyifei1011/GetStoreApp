using System;
using System.Runtime.InteropServices;

namespace GetStoreAppWindowsAPI.Dialogs.FileDialog.Native
{
    internal static class NativeMethods
    {
        [DllImport("shell32.dll", ExactSpelling = true, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern IShellItem SHCreateItemFromParsingName(
            string pszPath,
            IntPtr pbc,
            [MarshalAs(UnmanagedType.LPStruct)] Guid riid);

        /// <summary>
        /// 获取一个前台窗口的句柄（用户当前工作的窗口）
        /// </summary>
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// 0x800704C7
        /// </summary>
        public static int ERROR_CANCELLED { get; } = BitConverter.ToInt32(BitConverter.GetBytes(0x800704C7), 0);

        /// <summary>
        /// 0
        /// </summary>
        public static int OK { get; } = 0;
    }
}
