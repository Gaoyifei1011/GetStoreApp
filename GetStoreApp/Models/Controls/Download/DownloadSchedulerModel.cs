using GetStoreApp.Extensions.DataType.Enums;
using System;

namespace GetStoreApp.Models.Controls.Download
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
        public Guid DownloadID { get; set; }

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
    }
}
