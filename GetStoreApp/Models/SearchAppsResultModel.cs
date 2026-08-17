namespace GetStoreApp.Models
{
    /// <summary>
    /// 查询链接结果数据模型
    /// </summary>
    internal sealed class SearchAppsResultModel
    {
        /// <summary>
        /// 应用包名称
        /// </summary>
        internal string StoreAppName { get; set; }

        /// <summary>
        /// 应用发布者
        /// </summary>
        internal string StoreAppPublisher { get; set; }

        /// <summary>
        /// 应用链接
        /// </summary>
        internal string StoreAppLink { get; set; }
    }
}
