using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.ComTypes
{
    /// <summary>
    /// DO_DOWNLOAD_STATUS结构用于获取特定下载的状态。 它是通过调用 IDODownload：：GetStatus 函数获取的。
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DO_DOWNLOAD_STATUS
    {
        /// <summary>
        /// 要下载的字节总数。
        /// </summary>
        public ulong BytesTotal;

        /// <summary>
        /// 已下载的字节数。
        /// </summary>
        public ulong BytesTransferred;

        /// <summary>
        /// DODownloadState 枚举定义的当前下载状态。
        /// </summary>
        public DODownloadState State;

        /// <summary>
        /// 如果存在与当前下载关联的) ，则 (错误信息。
        /// </summary>
        public int Error;

        /// <summary>
        /// 如果存在与当前下载关联的) ，则扩展的错误信息 (。
        /// </summary>
        public int ExtendedError;
    }
}
