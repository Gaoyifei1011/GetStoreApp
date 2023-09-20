using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Models.Controls.Settings;
using GetStoreApp.Services.Root;
using Microsoft.Management.Deployment;
using System;
using System.Collections.Generic;

namespace GetStoreApp.Services.Controls.Settings
{
    /// <summary>
    /// WinGet 程序包配置服务
    /// </summary>
    public static class WinGetConfigService
    {
        private static string WinGetConfigSettingsKey { get; } = ConfigKey.WinGetConfigKey;

        private static string WinGetInstallModeSettingsKey { get; } = ConfigKey.WinGetInstallModeKey;

        private static bool DefaultUseDevVersion { get; } = false;

        public static bool UseDevVersion { get; set; }

        public static GroupOptionsModel DefaultWinGetInstallMode { get; set; }

        public static GroupOptionsModel WinGetInstallMode { get; set; }

        public static List<GroupOptionsModel> WinGetInstallModeList { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的是否使用开发版本布尔值和 WinGet 程序包安装方式值
        /// </summary>
        public static void InitializeWinGetConfig()
        {
            UseDevVersion = GetUseDevVersion();

            WinGetInstallModeList = ResourceService.WinGetInstallModeList;

            DefaultWinGetInstallMode = WinGetInstallModeList.Find(item => item.SelectedValue.Equals(Convert.ToString(PackageInstallMode.Interactive), StringComparison.OrdinalIgnoreCase));

            WinGetInstallMode = GetWinGetInstallMode();
        }

        /// <summary>
        /// 获取设置存储的是否使用开发版本布尔值，如果设置没有存储，使用默认值
        /// </summary>
        private static bool GetUseDevVersion()
        {
            bool? useDevVersion = ConfigService.ReadSetting<bool?>(WinGetConfigSettingsKey);

            if (!useDevVersion.HasValue)
            {
                SetUseDevVersion(DefaultUseDevVersion);
                return DefaultUseDevVersion;
            }

            return Convert.ToBoolean(useDevVersion);
        }

        /// <summary>
        /// 使用说明按钮显示发生修改时修改设置存储的是否使用开发版本布尔值
        /// </summary>
        public static void SetUseDevVersion(bool useDevVersion)
        {
            UseDevVersion = useDevVersion;

            ConfigService.SaveSetting(WinGetConfigSettingsKey, useDevVersion);
        }

        /// <summary>
        /// 获取设置存储的 WinGet 程序包安装方式值，如果设置没有存储，使用默认值
        /// </summary>
        private static GroupOptionsModel GetWinGetInstallMode()
        {
            string winGetInstallMode = ConfigService.ReadSetting<string>(WinGetInstallModeSettingsKey);

            if (string.IsNullOrEmpty(winGetInstallMode))
            {
                SetWinGetInstallMode(DefaultWinGetInstallMode);
                return WinGetInstallModeList.Find(item => item.SelectedValue.Equals(DefaultWinGetInstallMode.SelectedValue, StringComparison.OrdinalIgnoreCase));
            }

            return WinGetInstallModeList.Find(item => item.SelectedValue.Equals(winGetInstallMode, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 应用安装方式发生修改时修改设置存储的 WinGet 程序包安装方式
        /// </summary>
        public static void SetWinGetInstallMode(GroupOptionsModel winGetInstallMode)
        {
            WinGetInstallMode = winGetInstallMode;

            ConfigService.SaveSetting(WinGetInstallModeSettingsKey, winGetInstallMode.SelectedValue);
        }
    }
}
