using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;
using System;
using System.Collections.Generic;

namespace GetStoreApp.Services.Settings
{
    /// <summary>
    /// 应用安装方式设置服务
    /// </summary>
    public static class InstallModeService
    {
        private static readonly string settingsKey = ConfigKey.InstallModeKey;

        private static string defaultInstallMode;

        public static string InstallMode { get; private set; }

        public static List<string> InstallModeList { get; } = ["AppInstall", "CodeInstall"];

        /// <summary>
        /// 应用在初始化前获取设置存储的应用安装方式值
        /// </summary>
        public static void InitializeInstallMode()
        {
            defaultInstallMode = InstallModeList.Find(item => string.Equals(item, "AppInstall", StringComparison.OrdinalIgnoreCase));
            InstallMode = GetInstallMode();
        }

        /// <summary>
        /// 获取设置存储的应用安装方式值，如果设置没有存储，使用默认值
        /// </summary>
        private static string GetInstallMode()
        {
            string installMode = LocalSettingsService.ReadSetting<string>(settingsKey);

            if (string.IsNullOrEmpty(installMode))
            {
                SetInstallMode(defaultInstallMode);
                return InstallModeList.Find(item => string.Equals(item, defaultInstallMode, StringComparison.OrdinalIgnoreCase));
            }

            string selectedInstallMode = InstallModeList.Find(item => string.Equals(item, installMode, StringComparison.OrdinalIgnoreCase));
            return string.IsNullOrEmpty(selectedInstallMode) ? defaultInstallMode : selectedInstallMode;
        }

        /// <summary>
        /// 应用安装方式发生修改时修改设置存储的应用安装方式值
        /// </summary>
        public static void SetInstallMode(string installMode)
        {
            InstallMode = installMode;
            LocalSettingsService.SaveSetting(settingsKey, installMode);
        }
    }
}
