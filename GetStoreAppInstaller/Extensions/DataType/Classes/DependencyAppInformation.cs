using System;

namespace GetStoreAppInstaller.Extensions.DataType.Classes
{
    /// <summary>
    /// 依赖应用信息
    /// </summary>
    public class DependencyAppInformation
    {
        /// <summary>
        /// 依赖包全部名称
        /// </summary>
        public string PackageFullName { get; set; }

        /// <summary>
        /// 依赖包开发者名称
        /// </summary>
        public string PublisherDisplayName { get; set; }

        /// <summary>
        /// 依赖包版本
        /// </summary>
        public Version Version { get; set; }
    }
}
