using System;

namespace GetStoreApp.WindowsAPI.Dialogs.FileDialog
{
    public static class NativeMethods
    {
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
