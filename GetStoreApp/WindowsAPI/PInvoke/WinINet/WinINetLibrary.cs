using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.WinINet
{
    /// <summary>
    /// WinINet.dll 函数库
    /// </summary>
    public static partial class WinINetLibrary
    {
        private const string WinINet = "wininet.dll";

        [LibraryImport(WinINet, EntryPoint = "InternetGetConnectedState", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool InternetGetConnectedState(ref INTERNET_CONNECTION_FLAGS dwFlag, int dwReserved);
    }
}
