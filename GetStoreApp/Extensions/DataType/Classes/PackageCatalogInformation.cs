using Microsoft.Management.Deployment;
using System;

namespace GetStoreApp.Extensions.DataType.Classes
{
    /// <summary>
    /// WinGet 数据源信息
    /// </summary>
    public class PackageCatalogInformation
    {
        /// <summary>
        /// 数据源名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 数据源参数
        /// </summary>
        public string Arguments { get; set; }

        /// <summary>
        /// 数据源是否是显性的
        /// </summary>
        public bool Explicit { get; set; }

        /// <summary>
        /// 数据源信任等级
        /// </summary>
        public PackageCatalogTrustLevel TrustLevel { get; set; }

        /// <summary>
        /// 数据源 ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 数据源最后一次更新时间
        /// </summary>
        public DateTimeOffset LastUpdateTime { get; set; }

        /// <summary>
        /// 数据源源类型
        /// </summary>
        public PackageCatalogOrigin Origin { get; set; }

        /// <summary>
        /// 数据源类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 数据源是否可接受参数
        /// </summary>
        public bool AcceptSourceAgreements { get; set; }

        /// <summary>
        /// 数据源额外参数
        /// </summary>
        public string AdditionalPackageCatalogArguments { get; set; }

        /// <summary>
        /// 数据源验证类型
        /// </summary>
        public AuthenticationType AuthenticationType { get; set; }

        /// <summary>
        /// 数据源验证参数
        /// </summary>
        public string AuthenticationAccount { get; set; }

        /// <summary>
        /// 数据源后台更新间隔
        /// </summary>
        public TimeSpan PackageCatalogBackgroundUpdateInterval { get; set; }
    }
}
