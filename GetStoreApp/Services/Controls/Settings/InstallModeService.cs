using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GetStoreApp.Services.Controls.Settings
{
    /// <summary>
    /// 应用安装方式设置服务
    /// </summary>
    public static class InstallModeService
    {
        private static string settingsKey = ConfigKey.InstallModeKey;
        private static DictionaryEntry defaultInstallMode;

        public static DictionaryEntry InstallMode { get; private set; }

        public static List<DictionaryEntry> InstallModeList { get; private set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的应用安装方式值
        /// </summary>
        public static void InitializeInstallMode()
        {
            InstallModeList = ResourceService.InstallModeList;

            defaultInstallMode = InstallModeList.Find(item => item.Value.ToString().Equals("AppInstall", StringComparison.OrdinalIgnoreCase));

            InstallMode = GetInstallMode();
        }

        /// <summary>
        /// 获取设置存储的应用安装方式值，如果设置没有存储，使用默认值
        /// </summary>
        private static DictionaryEntry GetInstallMode()
        {
            object installMode = LocalSettingsService.ReadSetting<object>(settingsKey);

            if (installMode is null)
            {
                SetInstallMode(defaultInstallMode);
                return InstallModeList.Find(item => item.Value.Equals(defaultInstallMode.Value));
            }

            return InstallModeList.Find(item => item.Value.Equals(installMode));
        }

        /// <summary>
        /// 应用安装方式发生修改时修改设置存储的应用安装方式值
        /// </summary>
        public static void SetInstallMode(DictionaryEntry installMode)
        {
            InstallMode = installMode;

            LocalSettingsService.SaveSetting(settingsKey, installMode.Value);
        }
    }
}
