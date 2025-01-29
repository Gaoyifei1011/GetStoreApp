using GetStoreAppInstaller.Models;
using GetStoreAppInstaller.WindowsAPI.ComTypes;
using System.Collections.Generic;
using System;
using GetStoreAppInstaller.Extensions.DataType.Enums;

namespace GetStoreAppInstaller.Extensions.DataType.Classes
{
    /// <summary>
    /// 应用信息类
    /// </summary>
    public class PackageInformation
    {
        /// <summary>
        /// 解析的应用类型
        /// </summary>
        public PackageFileType PackageFileType { get; set; }

        /// <summary>
        /// 应用包架构
        /// </summary>
        public string ProcessorArchitecture { get; set; }

        /// <summary>
        /// 应用包系列名称
        /// </summary>
        public string PackageFamilyName { get; set; }

        /// <summary>
        /// 应用包全部名称
        /// </summary>
        public string PackageFullName { get; set; }

        /// <summary>
        /// 应用包版本
        /// </summary>
        public Version Version { get; set; }

        /// <summary>
        /// 是否为框架包
        /// </summary>
        public bool? IsFramework { get; set; }

        /// <summary>
        /// 应用包描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 应用包名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 应用包图标名称
        /// </summary>
        public string Logo { get; set; }

        /// <summary>
        /// 应用包开发者显示名称
        /// </summary>
        public string PublisherDisplayName { get; set; }

        /// <summary>
        /// 应用安装状态
        /// </summary>
        public string AppInstalledState { get; set; }

        /// <summary>
        /// 应用是否已安装
        /// </summary>
        public bool IsAppInstalled { get; set; }

        /// <summary>
        /// 更新设置是否存在
        /// </summary>
        public bool IsUpdateSettingsExisted { get; set; }

        /// <summary>
        /// 更新检查最小间隔
        /// </summary>
        public int HoursBetweenUpdateChecks { get; set; }

        /// <summary>
        /// 更新时禁止启动
        /// </summary>
        public bool UpdateBlocksActivation { get; set; }

        /// <summary>
        /// 启用自动更新
        /// </summary>
        public bool AutomaticBackgroundTask { get; set; }

        /// <summary>
        /// 安装更新时显示窗口
        /// </summary>
        public bool ShowPrompt { get; set; }

        /// <summary>
        /// 从任何版本更新应用
        /// </summary>
        public bool ForceUpdateFromAnyVersion { get; set; }

        /// <summary>
        /// 应用包图标资源
        /// </summary>
        public IStream ImageLogo { get; set; }

        /// <summary>
        /// 应用安装程序源链接
        /// </summary>
        public string AppInstallerSourceLink { get; set; }

        /// <summary>
        /// 应用安装程序源链接是否存在
        /// </summary>
        public bool IsAppInstallerSourceLinkExisted { get; set; }

        /// <summary>
        /// 应用包源链接
        /// </summary>
        public string PackageSourceLink { get; set; }

        /// <summary>
        /// 应用包源链接是否存在
        /// </summary>
        public bool IsPackageSourceLinkExisted { get; set; }

        /// <summary>
        /// 应用包类型
        /// </summary>
        public string PackageType { get; set; }

        /// <summary>
        /// 应用功能列表
        /// </summary>
        public List<string> CapabilitiesList { get; set; }

        /// <summary>
        /// 依赖项列表
        /// </summary>
        public List<DependencyInformation> DependencyList { get; set; }

        /// <summary>
        /// 应用包目标设备信息
        /// </summary>
        public List<TargetDeviceFamilyModel> TargetDeviceFamilyList { get; set; }

        /// <summary>
        /// 应用包应用信息
        /// </summary>
        public List<ApplicationModel> ApplicationList { get; set; }

        /// <summary>
        /// 应用包语言信息
        /// </summary>
        public List<string> LanguageList { get; set; }
    }
}
