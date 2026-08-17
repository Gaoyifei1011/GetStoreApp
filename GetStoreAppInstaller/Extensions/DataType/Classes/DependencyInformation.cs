using System;

namespace GetStoreAppInstaller.Extensions.DataType.Classes
{
    /// <summary>
    /// 应用包依赖信息类
    /// </summary>
    internal class DependencyInformation
    {
        /// <summary>
        /// 依赖项最低版本信息
        /// </summary>
        internal Version DependencyMinVersion { get; set; }

        /// <summary>
        /// 依赖项名称
        /// </summary>
        internal string DependencyName { get; set; }

        /// <summary>
        /// 依赖项发布者名称
        /// </summary>
        internal string DependencyPublisher { get; set; }

        /// <summary>
        /// 依赖项最大测试版本
        /// </summary>
        internal Version DependencyMaxMajorVersionTested { get; set; }

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
