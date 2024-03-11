using GetStoreApp.Extensions.DataType.Enums;

namespace GetStoreApp.Models.Controls.Download
{
    /// <summary>
    /// 下载调度服务数据模型
    /// </summary>
    public class DownloadSchedulerModel
    {
        /// <summary>
        /// 任务在下载状态时，获取的GID码。该值唯一
        /// </summary>
        public string GID { get; set; }

        /// <summary>
        /// 下载任务的唯一标识码，该值唯一
        /// </summary>
        public string DownloadKey { get; set; }

        /// <summary>
        /// 下载文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件下载链接
        /// </summary>
        public string FileLink { get; set; }

        /// <summary>
        /// 文件下载保存的路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 文件下载状态
        /// </summary>
        public DownloadStatus DownloadStatus { get; set; }

        public int DownloadFlag { get; set; }

        /// <summary>
        /// 下载文件的总大小
        /// </summary>
        public double TotalSize { get; set; }

        /// <summary>
        /// 下载文件已完成的进度
        /// </summary>
        public double FinishedSize { get; set; }

        /// <summary>
        /// 文件下载速度
        /// </summary>
        public double CurrentSpeed { get; set; }

        /// <summary>
        /// 文件是否处于正在安装状态
        /// </summary>
        public bool IsInstalling { get; set; }
    }
}
