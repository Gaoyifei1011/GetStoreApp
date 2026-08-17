using GetStoreAppInstaller.Extensions.DataType.Enums;
using GetStoreAppInstaller.Models;
using GetStoreAppInstaller.WindowsAPI.ComTypes;
using System;
using System.Collections.Generic;

namespace GetStoreAppInstaller.Extensions.DataType.Classes
{
    /// <summary>
    /// 应用信息类
    /// </summary>
    internal class PackageInformation
    {
        /// <summary>
        /// 解析的应用类型
        /// </summary>
        internal PackageFileType PackageFileType { get; set; }

        /// <summary>
        /// 应用包架构
        /// </summary>
        internal string ProcessorArchitecture { get; set; }

        /// <summary>
        /// 应用包系列名称
        /// </summary>
        internal string PackageFamilyName { get; set; }

        /// <summary>
        /// 应用包全部名称
        /// </summary>
        internal string PackageFullName { get; set; }

        /// <summary>
        /// 应用包版本
        /// </summary>
        internal Version Version { get; set; }

        /// <summary>
        /// 是否为框架包
        /// </summary>
        internal bool? IsFramework { get; set; }

        /// <summary>
        /// 应用包描述
        /// </summary>
        internal string Description { get; set; }

        /// <summary>
        /// 应用包名称
        /// </summary>
        internal string DisplayName { get; set; }

        /// <summary>
        /// 应用包图标名称
        /// </summary>
        internal string Logo { get; set; }

        /// <summary>
        /// 应用包开发者显示名称
        /// </summary>
        internal string PublisherDisplayName { get; set; }

        /// <summary>
        /// 应用安装状态
        /// </summary>
        internal string AppInstalledState { get; set; }

        /// <summary>
        /// 应用是否已安装
        /// </summary>
        internal bool IsAppInstalled { get; set; }

        /// <summary>
        /// 更新设置是否存在
        /// </summary>
        internal bool IsUpdateSettingsExisted { get; set; }

        /// <summary>
        /// 更新检查最小间隔
        /// </summary>
        internal int HoursBetweenUpdateChecks { get; set; }

        /// <summary>
        /// 更新时禁止启动
        /// </summary>
        internal bool UpdateBlocksActivation { get; set; }

        /// <summary>
        /// 启用自动更新
        /// </summary>
        internal bool AutomaticBackgroundTask { get; set; }

        /// <summary>
        /// 安装更新时显示窗口
        /// </summary>
        internal bool ShowPrompt { get; set; }

        /// <summary>
        /// 从任何版本更新应用
        /// </summary>
        internal bool ForceUpdateFromAnyVersion { get; set; }

        /// <summary>
        /// 应用包图标资源
        /// </summary>
        internal IStream ImageLogo { get; set; }

        /// <summary>
        /// 应用安装程序源链接
        /// </summary>
        internal string AppInstallerSourceLink { get; set; }

        /// <summary>
        /// 应用安装程序源链接是否存在
        /// </summary>
        internal bool IsAppInstallerSourceLinkExisted { get; set; }

        /// <summary>
        /// 应用包源链接
        /// </summary>
        internal string PackageSourceLink { get; set; }

        /// <summary>
        /// 应用包源链接是否存在
        /// </summary>
        internal bool IsPackageSourceLinkExisted { get; set; }

        /// <summary>
        /// 应用包类型
        /// </summary>
        internal string PackageType { get; set; }

        /// <summary>
        /// 应用功能列表
        /// </summary>
        internal List<string> CapabilitiesList { get; set; }

        /// <summary>
        /// 依赖项列表
        /// </summary>
        internal List<DependencyInformation> DependencyList { get; set; }

        /// <summary>
        /// 应用包目标设备信息
        /// </summary>
        internal List<TargetDeviceFamilyModel> TargetDeviceFamilyList { get; set; }

        /// <summary>
        /// 应用包应用信息
        /// </summary>
        internal List<ApplicationModel> ApplicationList { get; set; }

        /// <summary>
        /// 应用包语言信息
        /// </summary>
        internal List<string> LanguageList { get; set; }
    }
}
