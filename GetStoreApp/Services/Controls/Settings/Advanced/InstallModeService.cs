using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Models.Controls.Settings;
using GetStoreApp.Services.Root;
using System;
using System.Collections.Generic;

namespace GetStoreApp.Services.Controls.Settings.Advanced
{
    /// <summary>
    /// 应用安装方式设置服务
    /// </summary>
    public static class InstallModeService
    {
        private static string SettingsKey { get; } = ConfigKey.InstallModeKey;

        public static GroupOptionsModel DefaultInstallMode { get; set; }

        public static GroupOptionsModel InstallMode { get; set; }

        public static List<GroupOptionsModel> InstallModeList { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的应用安装方式值
        /// </summary>
        public static void InitializeInstallMode()
        {
            InstallModeList = ResourceService.InstallModeList;

            DefaultInstallMode = InstallModeList.Find(item => item.SelectedValue.Equals("AppInstall", StringComparison.OrdinalIgnoreCase));

            InstallMode = GetInstallMode();
        }

        /// <summary>
        /// 获取设置存储的应用安装方式值，如果设置没有存储，使用默认值
        /// </summary>
        private static GroupOptionsModel GetInstallMode()
        {
            string installMode = ConfigService.ReadSetting<string>(SettingsKey);

            if (string.IsNullOrEmpty(installMode))
            {
                return InstallModeList.Find(item => item.SelectedValue.Equals(DefaultInstallMode.SelectedValue, StringComparison.OrdinalIgnoreCase));
            }

            return InstallModeList.Find(item => item.SelectedValue.Equals(installMode, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 应用安装方式发生修改时修改设置存储的应用安装方式值
        /// </summary>
        public static void SetInstallMode(GroupOptionsModel installMode)
        {
            InstallMode = installMode;

            ConfigService.SaveSetting(SettingsKey, installMode.SelectedValue);
        }
    }
}
