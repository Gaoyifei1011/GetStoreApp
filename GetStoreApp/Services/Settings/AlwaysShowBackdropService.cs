using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;

namespace GetStoreApp.Services.Settings
{
    /// <summary>
    /// 始终显示背景色设置服务
    /// </summary>
    public static class AlwaysShowBackdropService
    {
        private static readonly string settingsKey = ConfigKey.AlwaysShowBackdropKey;

        private static readonly bool defaultAlwaysShowBackdropValue = false;

        public static bool AlwaysShowBackdropValue { get; private set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的始终显示背景色值
        /// </summary>
        public static void InitializeAlwaysShowBackdrop()
        {
            AlwaysShowBackdropValue = GetAlwaysShowBackdropValue();
        }

        /// <summary>
        /// 获取设置存储的始终显示背景色值，如果设置没有存储，使用默认值
        /// </summary>
        private static bool GetAlwaysShowBackdropValue()
        {
            bool? alwaysShowBackdropValue = LocalSettingsService.ReadSetting<bool?>(settingsKey);

            if (!alwaysShowBackdropValue.HasValue)
            {
                SetAlwaysShowBackdropValue(defaultAlwaysShowBackdropValue);
                return defaultAlwaysShowBackdropValue;
            }

            return alwaysShowBackdropValue.Value;
        }

        /// <summary>
        /// 始终显示背景色发生修改时修改设置存储的始终显示背景色值
        /// </summary>
        public static void SetAlwaysShowBackdropValue(bool alwaysShowBackdropValue)
        {
            AlwaysShowBackdropValue = alwaysShowBackdropValue;
            LocalSettingsService.SaveSetting(settingsKey, alwaysShowBackdropValue);
        }
    }
}
