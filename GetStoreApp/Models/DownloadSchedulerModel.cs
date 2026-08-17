using GetStoreApp.Extensions.DataType.Enums;

namespace GetStoreApp.Models
{
    /// <summary>
    /// 下载调度服务数据模型
    /// </summary>
    internal sealed class DownloadSchedulerModel
    {
        /// <summary>
        /// 任务下载时创建的下载唯一标识符
        /// </summary>
        internal string DownloadKey { get; set; }

        /// <summary>
        /// 任务下载时创建下载 ID
        /// </summary>
        internal string DownloadID { get; set; }

        /// <summary>
        /// 下载文件名称
        /// </summary>
        internal string FileName { get; set; }

        /// <summary>
        /// 文件下载保存的路径
        /// </summary>
        internal string FilePath { get; set; }

        /// <summary>
        /// 文件下载状态
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
