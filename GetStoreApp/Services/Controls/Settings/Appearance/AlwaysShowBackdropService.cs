using GetStoreAppCore.Settings;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Controls.Settings.Appearance
{
    public static class AlwaysShowBackdropService
    {
        private static string SettingsKey { get; } = ConfigStorage.ConfigKey["AlwaysShowBackdropKey"];

        private static bool DefaultAlwaysShowBackdropValue = false;

        public static bool AlwaysShowBackdropValue { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的始终显示背景色值
        /// </summary>
        public static async Task InitializeAlwaysShowBackdropAsync()
        {
            AlwaysShowBackdropValue = await GetAlwaysShowBackdropValueAsync();
        }

        /// <summary>
        /// 获取设置存储的始终显示背景色值，如果设置没有存储，使用默认值
        /// </summary>
        private static async Task<bool> GetAlwaysShowBackdropValueAsync()
        {
            bool? alwaysShowBackdropValue = await ConfigStorage.ReadSettingAsync<bool?>(SettingsKey);

            if (!alwaysShowBackdropValue.HasValue)
            {
                return DefaultAlwaysShowBackdropValue;
            }

            return alwaysShowBackdropValue.Value;
        }

        /// <summary>
        /// 始终显示背景色发生修改时修改设置存储的始终显示背景色值
        /// </summary>
        public static async Task SetAlwaysShowBackdropAsync(bool alwaysShowBackdropValue)
        {
            AlwaysShowBackdropValue = alwaysShowBackdropValue;

            await ConfigStorage.SaveSettingAsync(SettingsKey, alwaysShowBackdropValue);
        }
    }
}
