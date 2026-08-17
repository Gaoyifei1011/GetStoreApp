using GetStoreApp.Models;
using System.Collections.Generic;

namespace GetStoreApp.Extensions.DataType.Classes
{
    /// <summary>
    /// 应用信息类
    /// </summary>
    internal class AppInformation
    {
        /// <summary>
        /// 应用显示名称
        /// </summary>
        internal string DisplayName { get; set; }

        /// <summary>
        /// 应用包系列名称
        /// </summary>
        internal string PackageFamilyName { get; set; }

        /// <summary>
        /// 应用包全部名称
        /// </summary>
        internal string PackageFullName { get; set; }

        /// <summary>
        /// 应用包描述
        /// </summary>
        internal string Description { get; set; }

        /// <summary>
        /// 应用包开发者名称
        /// </summary>
        internal string PublisherDisplayName { get; set; }

        /// <summary>
        /// 应用包开发者 ID
        /// </summary>
        internal string PublisherId { get; set; }

        /// <summary>
        /// 应用包版本
        /// </summary>
        internal string Version { get; set; }

        /// <summary>
        /// 应用包安装日期
        /// </summary>
        internal string InstallDate { get; set; }

        /// <summary>
        /// 应用包架构
        /// </summary>
        internal string Architecture { get; set; }

        /// <summary>
        /// 应用包签名类型
        /// </summary>
        internal string SignatureKind { get; set; }

        /// <summary>
        /// 应用包资源 ID
        /// </summary>
        internal string ResourceId { get; set; }

        /// <summary>
        /// 是否为捆绑包
        /// </summary>
        internal string IsBundle { get; set; }

        /// <summary>
        /// 是否处于开发者模式
        /// </summary>
        internal string IsDevelopmentMode { get; set; }

        /// <summary>
        /// 是否是框架包
        /// </summary>
        internal string IsFramework { get; set; }

        /// <summary>
        /// 是否是可选包
        /// </summary>
        internal string IsOptional { get; set; }

        /// <summary>
        /// 是否是资源包
        /// </summary>
        internal string IsResourcePackage { get; set; }

        /// <summary>
        /// 是否是存根应用
        /// </summary>
        internal string IsStub { get; set; }

        /// <summary>
        /// 应用状态是否良好
        /// </summary>
        internal string VerifyIsOK { get; set; }

        /// <summary>
        /// 应用入口信息
        /// </summary>
        internal List<AppListEntryModel> AppListEntryList { get; set; } = [];

        /// <summary>
        /// 应用依赖信息
        /// </summary>
        internal List<PackageModel> DependenciesList { get; set; } = [];
    }
}
