namespace GetStoreApp.Models.Download
{
    public class BackgroundModel
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
        /// 文件SHA1值，用来校验文件是否正确下载
        /// </summary>
        public string FileSHA1 { get; set; }

        /// <summary>
        /// 文件下载标志：0为下载失败，1为等待下载，2为暂停下载，3为正在下载，4为成功下载
        /// </summary>
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
    }
}
