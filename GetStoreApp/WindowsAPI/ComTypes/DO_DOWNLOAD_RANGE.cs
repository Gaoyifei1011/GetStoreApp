using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.ComTypes
{
    /// <summary>
    /// DO_DOWNLOAD_RANGE 结构标识要从文件下载的单个字节范围。 DO_DOWNLOAD_RANGE 结构包含在 DO_DOWNLOAD_RANGES_INFO 结构中，以提供要下载的范围数组。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DO_DOWNLOAD_RANGE
    {
        /// <summary>
        /// 从零开始的偏移量到要从文件下载的字节范围开头。
        /// </summary>
        public ulong Offset;

        /// <summary>
        /// 范围的长度（以字节为单位）。 请勿指定零字节长度。 若要指示范围扩展到文件末尾，请指定 DO_LENGTH_TO_EOF。
        /// </summary>
        public ulong Length;
    }
}
