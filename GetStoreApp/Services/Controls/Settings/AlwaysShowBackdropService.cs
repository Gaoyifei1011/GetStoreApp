using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;

namespace GetStoreApp.Services.Controls.Settings
{
    /// <summary>
    /// 始终显示背景色设置服务
    /// </summary>
    public static class AlwaysShowBackdropService
    {
        private static bool DefaultAlwaysShowBackdropValue = false;
        private static string SettingsKey = ConfigKey.AlwaysShowBackdropKey;

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
            bool? alwaysShowBackdropValue = LocalSettingsService.ReadSetting<bool?>(SettingsKey);

            if (!alwaysShowBackdropValue.HasValue)
            {
                SetAlwaysShowBackdrop(DefaultAlwaysShowBackdropValue);
                return DefaultAlwaysShowBackdropValue;
            }

            return alwaysShowBackdropValue.Value;
        }

        /// <summary>
        /// 始终显示背景色发生修改时修改设置存储的始终显示背景色值
        /// </summary>
        public static void SetAlwaysShowBackdrop(bool alwaysShowBackdropValue)
        {
            AlwaysShowBackdropValue = alwaysShowBackdropValue;

            LocalSettingsService.SaveSetting(SettingsKey, alwaysShowBackdropValue);
        }
    }
}
