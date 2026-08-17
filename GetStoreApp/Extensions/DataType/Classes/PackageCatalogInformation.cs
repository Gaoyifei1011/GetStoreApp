using Microsoft.Management.Deployment;
using System;

namespace GetStoreApp.Extensions.DataType.Classes
{
    /// <summary>
    /// WinGet 数据源信息
    /// </summary>
    internal class PackageCatalogInformation
    {
        /// <summary>
        /// 数据源名称
        /// </summary>
        internal string Name { get; set; }

        /// <summary>
        /// 数据源参数
        /// </summary>
        internal string Arguments { get; set; }

        /// <summary>
        /// 数据源是否是显性的
        /// </summary>
        internal bool Explicit { get; set; }

        /// <summary>
        /// 数据源信任等级
        /// </summary>
        internal PackageCatalogTrustLevel TrustLevel { get; set; }

        /// <summary>
        /// 数据源 ID
        /// </summary>
        internal string Id { get; set; }

        /// <summary>
        /// 数据源最后一次更新时间
        /// </summary>
        internal DateTimeOffset LastUpdateTime { get; set; }

        /// <summary>
        /// 数据源源类型
        /// </summary>
        internal PackageCatalogOrigin Origin { get; set; }

        /// <summary>
        /// 数据源类型
        /// </summary>
        internal string Type { get; set; }

        /// <summary>
        /// 数据源是否可接受参数
        /// </summary>
        internal bool AcceptSourceAgreements { get; set; }

        /// <summary>
        /// 数据源额外参数
        /// </summary>
        internal string AdditionalPackageCatalogArguments { get; set; }

        /// <summary>
        /// 数据源验证类型
        /// </summary>
        internal AuthenticationType AuthenticationType { get; set; }

        /// <summary>
        /// 数据源验证参数
        /// </summary>
        internal string AuthenticationAccount { get; set; }

        /// <summary>
        /// 数据源后台更新间隔
        /// </summary>
        internal TimeSpan PackageCatalogBackgroundUpdateInterval { get; set; }
    }
}
