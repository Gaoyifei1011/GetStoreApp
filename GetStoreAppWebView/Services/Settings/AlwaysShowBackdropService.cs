using GetStoreAppWebView.Extensions.DataType.Constant;
using GetStoreAppWebView.Services.Root;

namespace GetStoreAppWebView.Services.Settings
{
    /// <summary>
    /// 始终显示背景色设置服务
    /// </summary>
    public static class AlwaysShowBackdropService
    {
        private static readonly string settingsKey = ConfigKey.AlwaysShowBackdropKey;

        private static readonly bool defaultAlwaysShowBackdrop = false;

        public static bool AlwaysShowBackdrop { get; private set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的始终显示背景色值
        /// </summary>
        public static void InitializeAlwaysShowBackdrop()
        {
            AlwaysShowBackdrop = GetAlwaysShowBackdrop();
        }

        /// <summary>
        /// 获取设置存储的始终显示背景色值，如果设置没有存储，使用默认值
        /// </summary>
        private static bool GetAlwaysShowBackdrop()
        {
            bool? alwaysShowBackdrop = LocalSettingsService.ReadSetting<bool?>(settingsKey);

            if (!alwaysShowBackdrop.HasValue)
            {
                return defaultAlwaysShowBackdrop;
            }

            return alwaysShowBackdrop.Value;
        }
    }
}
