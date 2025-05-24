using Microsoft.Management.Deployment;

namespace GetStoreApp.Models
{
    /// <summary>
    /// 搜索应用数据模型
    /// </summary>
    public sealed partial class SearchAppsModel
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// 匹配到的应用包
        /// </summary>
        public CatalogPackage CatalogPackage { get; set; }
    }
}
