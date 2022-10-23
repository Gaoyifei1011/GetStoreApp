using GetStoreApp.Contracts.Extensions;
using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.Extensions.Native
{
    public static class NativeMethods
    {
        [DllImport("shell32.dll", ExactSpelling = true, CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern IShellItem SHCreateItemFromParsingName(
            string pszPath,
            IntPtr pbc,
            [MarshalAs(UnmanagedType.LPStruct)] Guid riid);

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
