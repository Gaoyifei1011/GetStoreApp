using System;

namespace GetStoreAppInstaller.Models
{
    /// <summary>
    /// 依赖项数据模型
    /// </summary>
    internal class DependencyModel
    {
        /// <summary>
        /// 依赖项应用名称
        /// </summary>
        internal string DependencyName { get; set; }

        /// <summary>
        /// 依赖项发布者名称
        /// </summary>
        internal string DependencyPublisher { get; set; }

        /// <summary>
        /// 依赖项最低版本
        /// </summary>
        internal Version DependencyMinVersion { get; set; }

        /// <summary>
        /// 依赖项对应的版本
        /// </summary>
        internal Version DependencyVersion { get; set; }

        /// <summary>
        /// 依赖项对应的架构
        /// </summary>
        internal string ProcessorArchitecture { get; set; }

        /// <summary>
        /// 依赖项对应的 URL
        /// </summary>
        internal Uri Uri { get; set; }
    }
}
