using System;

namespace GetStoreAppInstaller.Models
{
    /// <summary>
    /// 依赖项数据模型
    /// </summary>
    public class DependencyModel
    {
        /// <summary>
        /// 依赖项应用名称
        /// </summary>
        public string DependencyName { get; set; }

        /// <summary>
        /// 依赖项发布者名称
        /// </summary>
        public string DependencyPublisher { get; set; }

        /// <summary>
        /// 依赖项最低版本
        /// </summary>
        public Version DependencyMinVersion { get; set; }
    }
}
