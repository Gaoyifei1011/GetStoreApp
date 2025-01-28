using System;

namespace GetStoreAppInstaller.Extensions.DataType.Classes
{
    /// <summary>
    /// 应用包依赖信息类
    /// </summary>
    public class DependencyInformation
    {
        /// <summary>
        /// 依赖项最低版本信息
        /// </summary>
        public Version DependencyMinVersion { get; set; }

        /// <summary>
        /// 依赖项名称
        /// </summary>
        public string DependencyName { get; set; }

        /// <summary>
        /// 依赖项发布者名称
        /// </summary>
        public string DependencyPublisher { get; set; }

        /// <summary>
        /// 依赖项最大测试版本
        /// </summary>
        public Version DependencyMaxMajorVersionTested { get; set; }

        /// <summary>
        /// 依赖项对应的版本
        /// </summary>
        public Version DependencyVersion { get; set; }

        /// <summary>
        /// 依赖项对应的架构
        /// </summary>
        public string ProcessorArchitecture { get; set; }

        /// <summary>
        /// 依赖项对应的 URL
        /// </summary>
        public Uri Uri { get; set; }
    }
}
