using GetStoreApp.Extensions.DataType.Enums;

namespace GetStoreApp.Extensions.DataType.Classes
{
    /// <summary>
    /// 下载进度
    /// </summary>
    public class DownloadProgress
    {
        /// <summary>
        /// 下载对应的 ID
        /// </summary>
        public string DownloadID { get; set; }

        /// <summary>
        /// 下载文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 下载文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 下载文件状态
        /// </summary>
        public DownloadProgressState DownloadProgressState { get; set; }

        /// <summary>
        /// 已下载完成的大小
        /// </summary>
        public double CompletedSize { get; set; }

        /// <summary>
        /// 文件总大小
        /// </summary>
        public double TotalSize { get; set; }

        /// <summary>
        /// 文件下载速度
        /// </summary>
        public double DownloadSpeed { get; set; }
    }
}
