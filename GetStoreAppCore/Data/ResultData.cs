namespace GetStoreAppCore.Data
{
    public class ResultData
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件下载链接
        /// </summary>
        public string FileLink { get; set; }

        /// <summary>
        /// 文件下载链接过期时间
        /// </summary>
        public string FileLinkExpireTime { get; set; }

        /// <summary>
        /// 文件SHA1值
        /// </summary>
        public string FileSHA1 { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public string FileSize { get; set; }
    }
}
