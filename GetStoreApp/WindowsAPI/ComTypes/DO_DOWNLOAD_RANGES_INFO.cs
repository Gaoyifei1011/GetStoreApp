using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.ComTypes
{
    /// <summary>
    /// DO_DOWNLOAD_RANGES_INFO 结构标识要从文件下载的字节范围数组。 它通常作为可选参数传递给 IDODownload：：Start 函数。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DO_DOWNLOAD_RANGES_INFO
    {
        /// <summary>
        /// Ranges 中的元素数。
        /// </summary>
        public uint RangeCount;

        /// <summary>
        ///一个或多个 DO_DOWNLOAD_RANGE 结构的数组，这些结构指定要下载的范围。
        /// </summary>
        [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)]
        public DO_DOWNLOAD_RANGE[] Ranges;
    }
}
