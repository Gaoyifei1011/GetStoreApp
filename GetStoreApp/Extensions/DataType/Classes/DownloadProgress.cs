using GetStoreApp.Extensions.DataType.Enums;

namespace GetStoreApp.Extensions.DataType.Classes
{
    /// <summary>
    /// 下载进度
    /// </summary>
    internal class DownloadProgress
    {
        /// <summary>
        /// 下载对应的 ID
        /// </summary>
        internal string DownloadID { get; set; }

        /// <summary>
        /// 下载文件名称
        /// </summary>
        internal string FileName { get; set; }

        /// <summary>
        /// 下载文件路径
        /// </summary>
        internal string FilePath { get; set; }

        /// <summary>
        /// 下载文件状态
        /// </summary>
        internal DownloadProgressState DownloadProgressState { get; set; }

        /// <summary>
        /// 已下载完成的大小
        /// </summary>
        internal double CompletedSize { get; set; }

        /// <summary>
        /// 文件总大小
        /// </summary>
        internal double TotalSize { get; set; }

        /// <summary>
        /// 文件下载速度
        /// </summary>
        internal double DownloadSpeed { get; set; }
    }
}
