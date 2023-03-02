using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace GetStoreAppHelper.Services
{
    /// <summary>
    /// 应用主题设置服务
    /// </summary>
    public static class ThemeService
    {
        private static string SettingsKey { get; } = "AppTheme";

        public static string AppTheme { get; set; }

        /// <summary>
        /// 浮出菜单在打开前获取设置存储的主题值
        /// </summary>
        public static async Task LoadThemeAsync()
        {
            AppTheme = await ConfigService.ReadSettingAsync<string>(SettingsKey);

            if (string.IsNullOrEmpty(AppTheme))
            {
                AppTheme = Convert.ToString(ElementTheme.Default);
            }
        }
    }
}
