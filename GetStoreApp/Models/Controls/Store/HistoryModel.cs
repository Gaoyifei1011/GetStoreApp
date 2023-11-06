namespace GetStoreApp.Models.Controls.Store
{
    /// <summary>
    /// 历史记录数据模型
    /// </summary>
    public class HistoryModel
    {
        /// <summary>
        /// 历史记录生成时对应的时间戳，本地存储时使用的是格林尼治标准时间（GMT+0）
        /// </summary>
        public long CreateTimeStamp { get; set; }

        /// <summary>
        /// 历史记录的索引键值
        /// </summary>
        public string HistoryKey { get; set; }

        /// <summary>
        /// 历史记录的应用名称
        /// </summary>
        public string HistoryAppName { get; set; }

        /// <summary>
        /// 历史记录的内容
        /// </summary>
        public string HistoryContent { get; set; }

        /// <summary>
        /// 历史记录中包含的类型，数据库存储的原始名称
        /// </summary>
        public string HistoryType { get; set; }

        /// <summary>
        /// 历史记录中包含的通道
        /// </summary>
        public string HistoryChannel { get; set; }

        /// <summary>
        /// 历史记录包含的链接
        /// </summary>
        public string HistoryLink { get; set; }
    }
}
