using GetStoreAppHelper.Extensions.DataType.Constant;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace GetStoreAppHelper.Services
{
    /// <summary>
    /// 应用主题设置服务
    /// </summary>
    public static class ThemeService
    {
        private static string ThemeSettingsKey { get; } = ConfigKey.ThemeKey;

        private static string NotifyIconMenuThemeSettingsKey { get; } = ConfigKey.NotifyIconMenuThemeKey;

        private static string DefaultAppTheme { get; } = Convert.ToString(ElementTheme.Default);

        private static string DefaultNotifyIconMenuTheme { get; set; }

        public static string AppTheme { get; set; }

        public static string NotifyIconMenuTheme { get; set; }

        public static List<string> NotifyIconMenuThemeList { get; } = new List<string>()
        {
            "NotifyIconMenuAppTheme",
            "NotifyIconMenuSystemTheme"
        };

        static ThemeService()
        {
            DefaultNotifyIconMenuTheme = NotifyIconMenuThemeList[1];
        }

        /// <summary>
        /// 浮出菜单在打开前获取设置存储的主题值
        /// </summary>
        public static async Task LoadThemeAsync()
        {
            AppTheme = await ConfigService.ReadSettingAsync<string>(ThemeSettingsKey);

            if (string.IsNullOrEmpty(AppTheme))
            {
                AppTheme = DefaultAppTheme;
            }
        }

        public static async Task LoadNotifyIconMenuThemeAsync()
        {
            NotifyIconMenuTheme = await ConfigService.ReadSettingAsync<string>(NotifyIconMenuThemeSettingsKey);

            if (string.IsNullOrEmpty(NotifyIconMenuTheme))
            {
                NotifyIconMenuTheme = DefaultNotifyIconMenuTheme;
            }
        }
    }
}
