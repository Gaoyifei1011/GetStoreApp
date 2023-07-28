using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.WinINet
{
    /// <summary>
    /// WinINet.dll 函数库
    /// </summary>
    public static partial class WinINetLibrary
    {
        private const string WinINet = "wininet.dll";

        /// <summary>
        /// 检索本地系统的连接状态。
        /// </summary>
        /// <param name="lpdwFlags">指向接收连接说明的变量的指针。 即使函数返回 FALSE，此参数也可以返回有效的标志。</param>
        /// <param name="dwReserved">此参数是保留的，必须为 0。</param>
        /// <returns>
        /// 如果存在活动调制解调器或 LAN Internet 连接，则返回 TRUE ;如果没有 Internet 连接，或者所有可能的 Internet 连接当前均未处于活动状态，则返回 TRUE 。
        /// </returns>
        [LibraryImport(WinINet, EntryPoint = "InternetGetConnectedState", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool InternetGetConnectedState(ref INTERNET_CONNECTION_FLAGS lpdwFlags, int dwReserved);
    }
}
