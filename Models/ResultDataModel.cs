namespace GetStoreApp.Models
{
    /// <summary>
    /// 向API发送请求，正确获得消息并成功解析后得到的结果列表数据模型
    /// The result list data model obtained after sending a request to the API, getting the message correctly and parsing it successfully
    /// </summary>
    public class ResultDataModel
    {
        /// <summary>
        /// 列表条目中对应的序号
        /// </summary>
        public string Index { get; set; }

        /// <summary>
        /// 文件名称
        /// File name
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 链接名称
        /// File Link
        /// </summary>
        public string FileLink { get; set; }

        /// <summary>
        /// 链接有效期
        /// Link validity period 
        /// </summary>
        public string FileLinkExpireTime { get; set; }

        /// <summary>
        /// 文件的SHA-1值
        /// The SHA-1 value of the file
        /// </summary>
        public string FileSHA1 { get; set; }

        /// <summary>
        /// 文件大小
        /// File size
        /// </summary>
        public string FileSize { get; set; }

        public ResultDataModel(string fileName, string fileLink, string fileLinkExpireTime, string fileSHA1, string fileSize)
        {
            FileName = fileName;
            FileLink = fileLink;
            FileLinkExpireTime = fileLinkExpireTime;
            FileSHA1 = fileSHA1;
            FileSize = fileSize;
        }
    }
}
