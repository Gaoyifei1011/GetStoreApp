using Microsoft.Management.Deployment;
using System.ComponentModel;

namespace GetStoreApp.Models.Controls.WinGet
{
    /// <summary>
    /// 可更新版本的数据模型
    /// </summary>
    public partial class AvailableVersionModel
    {
        /// <summary>
        /// 该版本对应的发布者
        /// </summary>
        public string Publisher { get; set; }

        /// <summary>
        /// 该版本对应的版本号
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 该版本对应的通道
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// 该版本所有信息
        /// </summary>
        public PackageVersionId PackageVersionId { get; set; }
    }
}
