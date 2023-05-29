using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Models.Controls.Settings.Common;
using GetStoreApp.Services.Root;
using Microsoft.Management.Deployment;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Controls.Settings.Common
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

        public static WinGetInstallModeModel DefaultWinGetInstallMode { get; set; }

        public static WinGetInstallModeModel WinGetInstallMode { get; set; }

        public static List<WinGetInstallModeModel> WinGetInstallModeList { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的是否使用开发版本布尔值和 WinGet 程序包安装方式值
        /// </summary>
        public static async Task InitializeWinGetConfigAsync()
        {
            UseDevVersion = await GetUseDevVersionAsync();

            WinGetInstallModeList = ResourceService.WinGetInstallModeList;

            DefaultWinGetInstallMode = WinGetInstallModeList.Find(item => item.InternalName.Equals(Convert.ToString(PackageInstallMode.Interactive), StringComparison.OrdinalIgnoreCase));

            WinGetInstallMode = await GetWinGetInstallModeAsync();
        }

        /// <summary>
        /// 获取设置存储的是否使用开发版本布尔值，如果设置没有存储，使用默认值
        /// </summary>
        private static async Task<bool> GetUseDevVersionAsync()
        {
            bool? useDevVersion = await ConfigService.ReadSettingAsync<bool?>(WinGetConfigSettingsKey);

            if (!useDevVersion.HasValue)
            {
                return DefaultUseDevVersion;
            }

            return Convert.ToBoolean(useDevVersion);
        }

        /// <summary>
        /// 使用说明按钮显示发生修改时修改设置存储的是否使用开发版本布尔值
        /// </summary>
        public static async Task SetUseDevVersionAsync(bool useDevVersion)
        {
            UseDevVersion = useDevVersion;

            await ConfigService.SaveSettingAsync(WinGetConfigSettingsKey, useDevVersion);
        }

        /// <summary>
        /// 获取设置存储的 WinGet 程序包安装方式值，如果设置没有存储，使用默认值
        /// </summary>
        private static async Task<WinGetInstallModeModel> GetWinGetInstallModeAsync()
        {
            string winGetInstallMode = await ConfigService.ReadSettingAsync<string>(WinGetInstallModeSettingsKey);

            if (string.IsNullOrEmpty(winGetInstallMode))
            {
                return WinGetInstallModeList.Find(item => item.InternalName.Equals(DefaultWinGetInstallMode.InternalName, StringComparison.OrdinalIgnoreCase));
            }

            return WinGetInstallModeList.Find(item => item.InternalName.Equals(winGetInstallMode, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 应用安装方式发生修改时修改设置存储的 WinGet 程序包安装方式
        /// </summary>
        public static async Task SetWinGetInstallModeAsync(WinGetInstallModeModel winGetInstallMode)
        {
            WinGetInstallMode = winGetInstallMode;

            await ConfigService.SaveSettingAsync(WinGetInstallModeSettingsKey, winGetInstallMode.InternalName);
        }
    }
}
