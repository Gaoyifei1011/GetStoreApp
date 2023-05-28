using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;
using System;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Controls.Settings.Common
{
    /// <summary>
    /// WinGet 程序包配置服务
    /// </summary>
    public static class WinGetConfigService
    {
        private static string SettingsKey { get; } = ConfigKey.WinGetConfigKey;

        private static bool DefaultUseDevVersion { get; } = false;

        public static bool UseDevVersion { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的是否使用开发版本布尔值
        /// </summary>
        public static async Task InitializeUseDevVersionAsync()
        {
            UseDevVersion = await GetUseDevVersionAsync();
        }

        /// <summary>
        /// 获取设置存储的是否使用开发版本布尔值，如果设置没有存储，使用默认值
        /// </summary>
        private static async Task<bool> GetUseDevVersionAsync()
        {
            bool? useDevVersion = await ConfigService.ReadSettingAsync<bool?>(SettingsKey);

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

            await ConfigService.SaveSettingAsync(SettingsKey, useDevVersion);
        }
    }
}
