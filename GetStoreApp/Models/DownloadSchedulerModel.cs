using GetStoreApp.Extensions.DataType.Enums;

namespace GetStoreApp.Models
{
    /// <summary>
    /// 下载调度服务数据模型
    /// </summary>
    public sealed class DownloadSchedulerModel
    {
        /// <summary>
        /// 任务下载时创建的下载唯一标识符
        /// </summary>
        public string DownloadKey { get; set; }

        /// <summary>
        /// 任务下载时创建下载 ID
        /// </summary>
        public string DownloadID { get; set; }

        /// <summary>
        /// 下载文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件下载保存的路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 文件下载状态
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
