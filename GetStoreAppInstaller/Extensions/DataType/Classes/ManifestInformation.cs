﻿using GetStoreAppInstaller.Models;
using System;
using System.Collections.Generic;

namespace GetStoreAppInstaller.Extensions.DataType.Classes
{
    /// <summary>
    /// 应用清单信息
    /// </summary>
    public class ManifestInformation
    {
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
