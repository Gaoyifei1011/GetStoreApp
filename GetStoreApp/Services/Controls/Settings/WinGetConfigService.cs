using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;
using Microsoft.Management.Deployment;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GetStoreApp.Services.Controls.Settings
{
    /// <summary>
    /// WinGet 程序包配置服务
    /// </summary>
    public static class WinGetConfigService
    {
        private static string WinGetConfigSettingsKey = ConfigKey.WinGetConfigKey;
        private static string WinGetInstallModeSettingsKey = ConfigKey.WinGetInstallModeKey;

        private static bool DefaultUseDevVersion = false;

        public static bool UseDevVersion { get; private set; }

        public static DictionaryEntry DefaultWinGetInstallMode { get; set; }

        public static DictionaryEntry WinGetInstallMode { get; set; }

        public static List<DictionaryEntry> WinGetInstallModeList { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的是否使用开发版本布尔值和 WinGet 程序包安装方式值
        /// </summary>
        public static void InitializeWinGetConfig()
        {
            UseDevVersion = GetUseDevVersion();

            WinGetInstallModeList = ResourceService.WinGetInstallModeList;

            DefaultWinGetInstallMode = WinGetInstallModeList.Find(item => item.Value.ToString().Equals(PackageInstallMode.Interactive.ToString(), StringComparison.OrdinalIgnoreCase));

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
        private static DictionaryEntry GetWinGetInstallMode()
        {
            object winGetInstallMode = ConfigService.ReadSetting<object>(WinGetInstallModeSettingsKey);

            if (winGetInstallMode is null)
            {
                SetWinGetInstallMode(DefaultWinGetInstallMode);
                return WinGetInstallModeList.Find(item => item.Value.Equals(DefaultWinGetInstallMode.Value));
            }

            return WinGetInstallModeList.Find(item => item.Value.Equals(winGetInstallMode));
        }

        /// <summary>
        /// 应用安装方式发生修改时修改设置存储的 WinGet 程序包安装方式
        /// </summary>
        public static void SetWinGetInstallMode(DictionaryEntry winGetInstallMode)
        {
            WinGetInstallMode = winGetInstallMode;

            ConfigService.SaveSetting(WinGetInstallModeSettingsKey, winGetInstallMode.Value);
        }
    }
}
