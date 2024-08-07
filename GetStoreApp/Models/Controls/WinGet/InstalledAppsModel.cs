﻿namespace GetStoreApp.Models.Controls.WinGet
{
    /// <summary>
    /// 已安装应用数据模型
    /// </summary>
    public sealed class InstalledAppsModel
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// 应用的发布者
        /// </summary>
        public string AppPublisher { get; set; }

        /// <summary>
        /// 应用版本
        /// </summary>
        public string AppVersion { get; set; }
    }
}
