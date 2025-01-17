using System;

namespace GetStoreAppInstaller.Models
{
    /// <summary>
    /// 要安装的依赖项数据模型
    /// </summary>
    public class InstallDependencyModel
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
        /// 依赖项版本
        /// </summary>
        public Version DependencyVersion { get; set; }

        /// <summary>
        /// 依赖项全部名称
        /// </summary>
        public string DependencyFullName { get; set; }

        /// <summary>
        /// 依赖项文件路径
        /// </summary>
        public string DependencyPath { get; set; }
    }
}
