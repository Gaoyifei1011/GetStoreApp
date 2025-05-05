using Microsoft.Management.Deployment;

namespace GetStoreApp.Models.Controls.WinGet
{
    /// <summary>
    /// 可更新版本的数据模型
    /// </summary>
    public partial class AvailableVersionModel
    {
        /// <summary>
        /// 是否是默认版本
        /// </summary>
        public bool IsDefaultVersion { get; set; }

        /// <summary>
        /// 该版本对应的版本号
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 该版本所有信息
        /// </summary>
        public PackageVersionInfo PackageVersionInfo { get; set; }
    }
}
