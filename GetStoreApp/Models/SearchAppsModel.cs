using Microsoft.Management.Deployment;

namespace GetStoreApp.Models
{
    /// <summary>
    /// 搜索应用数据模型
    /// </summary>
    internal sealed partial class SearchAppsModel
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        internal string AppID { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        internal string AppName { get; set; }

        /// <summary>
        /// 匹配到的应用包
        /// </summary>
        internal CatalogPackage CatalogPackage { get; set; }
    }
}
