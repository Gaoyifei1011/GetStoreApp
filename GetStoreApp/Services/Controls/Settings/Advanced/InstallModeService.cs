using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Models.Controls.Settings.Advanced;
using GetStoreApp.Services.Root;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Controls.Settings.Advanced
{
    /// <summary>
    /// 应用安装方式设置服务
    /// </summary>
    public static class InstallModeService
    {
        private static string SettingsKey { get; } = ConfigKey.InstallModeKey;

        public static InstallModeModel DefaultInstallMode { get; set; }

        public static InstallModeModel InstallMode { get; set; }

        public static List<InstallModeModel> InstallModeList { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的应用安装方式值
        /// </summary>
        public static async Task InitializeInstallModeAsync()
        {
            InstallModeList = ResourceService.InstallModeList;

            DefaultInstallMode = InstallModeList.Find(item => item.InternalName.Equals("AppInstall", StringComparison.OrdinalIgnoreCase));

            InstallMode = await GetInstallModeAsync();
        }

        /// <summary>
        /// 获取设置存储的应用安装方式值，如果设置没有存储，使用默认值
        /// </summary>
        private static async Task<InstallModeModel> GetInstallModeAsync()
        {
            string installMode = await ConfigService.ReadSettingAsync<string>(SettingsKey);

            if (string.IsNullOrEmpty(installMode))
            {
                return InstallModeList.Find(item => item.InternalName.Equals(DefaultInstallMode.InternalName, StringComparison.OrdinalIgnoreCase));
            }

            return InstallModeList.Find(item => item.InternalName.Equals(installMode, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 应用安装方式发生修改时修改设置存储的背景色值
        /// </summary>
        public static async Task SetInstallModeAsync(InstallModeModel installMode)
        {
            InstallMode = installMode;

            await ConfigService.SaveSettingAsync(SettingsKey, installMode.InternalName);
        }
    }
}
