using System;

namespace GetStoreAppInstaller.Extensions.DataType.Classes
{
    /// <summary>
    /// 依赖应用信息
    /// </summary>
    internal class DependencyAppInformation
    {
        /// <summary>
        /// 依赖包全部名称
        /// </summary>
        internal string PackageFullName { get; set; }

        /// <summary>
        /// 依赖包开发者名称
        /// </summary>
        internal string PublisherDisplayName { get; set; }

        /// <summary>
        /// 依赖包版本
        /// </summary>
        internal Version Version { get; set; }
    }
}
