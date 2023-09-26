using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 包含要通过 WM_COPYDATA 消息传递到另一个应用程序的数据。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct COPYDATASTRUCT
    {
        /// <summary>
        /// 要传递给接收应用程序的数据类型。 接收应用程序定义有效类型。
        /// </summary>
        public IntPtr dwData;

        /// <summary>
        /// lpData 成员指向的数据的大小（以字节为单位）。
        /// </summary>
        public int cbData;

        /// <summary>
        /// 要传递给接收应用程序的数据。 此成员可以为 NULL。
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr, SizeConst = 512)]
        public string lpData;
    }
}
