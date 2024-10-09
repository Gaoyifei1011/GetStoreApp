using GetStoreApp.Extensions.DataType.Constant;
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
        private static readonly string winGetConfigSettingsKey = ConfigKey.WinGetConfigKey;
        private static readonly string winGetInstallModeSettingsKey = ConfigKey.WinGetInstallModeKey;

        private static readonly bool defaultUseDevVersion;

        public static bool UseDevVersion { get; private set; }

        public static KeyValuePair<string, string> DefaultWinGetInstallMode { get; set; }

        public static KeyValuePair<string, string> WinGetInstallMode { get; set; }

        public static List<KeyValuePair<string, string>> WinGetInstallModeList { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的是否使用开发版本布尔值和 WinGet 程序包安装方式值
        /// </summary>
        public static void InitializeWinGetConfig()
        {
            UseDevVersion = GetUseDevVersion();

            WinGetInstallModeList = ResourceService.WinGetInstallModeList;

            DefaultWinGetInstallMode = WinGetInstallModeList.Find(item => item.Key.Equals(PackageInstallMode.Interactive.ToString(), StringComparison.OrdinalIgnoreCase));

            WinGetInstallMode = GetWinGetInstallMode();
        }

        /// <summary>
        /// 获取设置存储的是否使用开发版本布尔值，如果设置没有存储，使用默认值
        /// </summary>
        private static bool GetUseDevVersion()
        {
            bool? useDevVersion = LocalSettingsService.ReadSetting<bool?>(winGetConfigSettingsKey);

            if (!useDevVersion.HasValue)
            {
                SetUseDevVersion(defaultUseDevVersion);
                return defaultUseDevVersion;
            }

            return Convert.ToBoolean(useDevVersion);
        }

        /// <summary>
        /// 使用说明按钮显示发生修改时修改设置存储的是否使用开发版本布尔值
        /// </summary>
        public static void SetUseDevVersion(bool useDevVersion)
        {
            UseDevVersion = useDevVersion;

            LocalSettingsService.SaveSetting(winGetConfigSettingsKey, useDevVersion);
        }

        /// <summary>
        /// 获取设置存储的 WinGet 程序包安装方式值，如果设置没有存储，使用默认值
        /// </summary>
        private static KeyValuePair<string, string> GetWinGetInstallMode()
        {
            object winGetInstallMode = LocalSettingsService.ReadSetting<object>(winGetInstallModeSettingsKey);

            if (winGetInstallMode is null)
            {
                SetWinGetInstallMode(DefaultWinGetInstallMode);
                return WinGetInstallModeList.Find(item => item.Key.Equals(DefaultWinGetInstallMode.Key));
            }

            KeyValuePair<string, string> selectedWinGetInstallMode = WinGetInstallModeList.Find(item => item.Key.Equals(winGetInstallMode));

            return selectedWinGetInstallMode.Key is null ? DefaultWinGetInstallMode : selectedWinGetInstallMode;
        }

        /// <summary>
        /// 应用安装方式发生修改时修改设置存储的 WinGet 程序包安装方式
        /// </summary>
        public static void SetWinGetInstallMode(KeyValuePair<string, string> winGetInstallMode)
        {
            WinGetInstallMode = winGetInstallMode;

            LocalSettingsService.SaveSetting(winGetInstallModeSettingsKey, winGetInstallMode.Key);
        }
    }
}
