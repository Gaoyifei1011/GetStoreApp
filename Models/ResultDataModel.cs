namespace GetStoreApp.Models
{
    public class ResultDataModel
    {
        // 序号索引
        public int Index { get; set; }

        // 文件名称
        public string FileName { get; set; }

        // 链接名称
        public string FileLink { get; set; }

        // 链接过期时间
        public string FileLinkExpireTime { get; set; }

        // 文件SHA-1值
        public string FileSHA1 { get; set; }

        // 文件大小
        public string FileSize { get; set; }

        // 不带Index索引的构造器
        public ResultDataModel(string fileName, string fileLink, string fileLinkExpireTime, string fileSHA1, string fileSize)
        {
            Index = 0;
            FileName = fileName;
            FileLink = fileLink;
            FileLinkExpireTime = fileLinkExpireTime;
            FileSHA1 = fileSHA1;
            FileSize = fileSize;
        }

        // 带Index索引的构造器
        public ResultDataModel(int index, string fileName, string fileLink, string fileLinkExpireTime, string fileSHA1, string fileSize)
        {
            Index = index;
            FileName = fileName;
            FileLink = fileLink;
            FileLinkExpireTime = fileLinkExpireTime;
            FileSHA1 = fileSHA1;
            FileSize = fileSize;
        }
    }
}