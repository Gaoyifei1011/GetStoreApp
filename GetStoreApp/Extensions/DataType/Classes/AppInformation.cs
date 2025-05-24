using GetStoreApp.Models;
using System.Collections.Generic;

namespace GetStoreApp.Extensions.DataType.Classes
{
    /// <summary>
    /// 应用信息类
    /// </summary>
    public class AppInformation
    {
        /// <summary>
        /// 应用显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 应用包系列名称
        /// </summary>
        public string PackageFamilyName { get; set; }

        /// <summary>
        /// 应用包全部名称
        /// </summary>
        public string PackageFullName { get; set; }

        /// <summary>
        /// 应用包描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 应用包开发者名称
        /// </summary>
        public string PublisherDisplayName { get; set; }

        /// <summary>
        /// 应用包开发者 ID
        /// </summary>
        public string PublisherId { get; set; }

        /// <summary>
        /// 应用包版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 应用包安装日期
        /// </summary>
        public string InstallDate { get; set; }

        /// <summary>
        /// 应用包架构
        /// </summary>
        public string Architecture { get; set; }

        /// <summary>
        /// 应用包签名类型
        /// </summary>
        public string SignatureKind { get; set; }

        /// <summary>
        /// 应用包资源 ID
        /// </summary>
        public string ResourceId { get; set; }

        /// <summary>
        /// 是否为捆绑包
        /// </summary>
        public string IsBundle { get; set; }

        /// <summary>
        /// 是否处于开发者模式
        /// </summary>
        public string IsDevelopmentMode { get; set; }

        /// <summary>
        /// 是否是框架包
        /// </summary>
        public string IsFramework { get; set; }

        /// <summary>
        /// 是否是可选包
        /// </summary>
        public string IsOptional { get; set; }

        /// <summary>
        /// 是否是资源包
        /// </summary>
        public string IsResourcePackage { get; set; }

        /// <summary>
        /// 是否是存根应用
        /// </summary>
        public string IsStub { get; set; }

        /// <summary>
        /// 应用状态是否良好
        /// </summary>
        public string VertifyIsOK { get; set; }

        /// <summary>
        /// 应用入口信息
        /// </summary>
        public List<AppListEntryModel> AppListEntryList { get; set; } = [];

        /// <summary>
        /// 应用依赖信息
        /// </summary>
        public List<PackageModel> DependenciesList { get; set; } = [];
    }
}
