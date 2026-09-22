using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;
using System;
using System.Collections.ObjectModel;

namespace GetStoreApp.Services.Settings
{
    /// <summary>
    /// 应用安装方式设置服务
    /// </summary>
    internal static class InstallModeService
    {
        private static readonly string settingsKey = ConfigKey.InstallModeKey;
        private static string defaultInstallMode;

        internal static string InstallMode { get; private set; }

        internal static ReadOnlyCollection<string> InstallModeCollection { get; } = ["AppInstall", "CodeInstall"];

        /// <summary>
        /// 应用在初始化前获取设置存储的应用安装方式值
        /// </summary>
        internal static void InitializeInstallMode()
        {
            foreach (string installModeItem in InstallModeCollection)
            {
                if (string.Equals(installModeItem, "AppInstall", StringComparison.OrdinalIgnoreCase))
                {
                    defaultInstallMode = installModeItem;
                    break;
                }
            }
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
                string result = default;
                foreach (string installModeItem in InstallModeCollection)
                {
                    if (string.Equals(installModeItem, defaultInstallMode, StringComparison.OrdinalIgnoreCase))
                    {
                        result = installModeItem;
                        break;
                    }
                }
                return result;
            }

            string selectedInstallMode = null;
            foreach (string installModeItem in InstallModeCollection)
            {
                if (string.Equals(installModeItem, installMode, StringComparison.OrdinalIgnoreCase))
                {
                    selectedInstallMode = installModeItem;
                    break;
                }
            }
            return string.IsNullOrEmpty(selectedInstallMode) ? defaultInstallMode : selectedInstallMode;
        }

        /// <summary>
        /// 应用安装方式发生修改后修改设置存储的应用安装方式值
        /// </summary>
        internal static void SetInstallMode(string installMode)
        {
            InstallMode = installMode;
            LocalSettingsService.SaveSetting(settingsKey, installMode);
        }
    }
}
