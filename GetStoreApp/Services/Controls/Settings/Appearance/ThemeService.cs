using GetStoreApp.Models.Controls.Settings.Appearance;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.Window;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Controls.Settings.Appearance
{
    /// <summary>
    /// 应用主题设置服务
    /// </summary>
    public static class ThemeService
    {
        private static string SettingsKey { get; } = ConfigService.ConfigKey["ThemeKey"];

        private static ThemeModel DefaultAppTheme { get; set; }

        public static ThemeModel AppTheme { get; set; }

        public static List<ThemeModel> ThemeList { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的主题值
        /// </summary>
        public static async Task InitializeThemeAsync()
        {
            ThemeList = ResourceService.ThemeList;

            DefaultAppTheme = ThemeList.Find(item => item.InternalName == Convert.ToString(ElementTheme.Default));

            AppTheme = await GetThemeAsync();
        }

        /// <summary>
        /// 获取设置存储的主题值，如果设置没有存储，使用默认值
        /// </summary>
        private static async Task<ThemeModel> GetThemeAsync()
        {
            string theme = await ConfigService.ReadSettingAsync<string>(SettingsKey);

            if (string.IsNullOrEmpty(theme))
            {
                return ThemeList.Find(item => item.InternalName.Equals(DefaultAppTheme.InternalName, StringComparison.OrdinalIgnoreCase));
            }

            return ThemeList.Find(item => item.InternalName.Equals(theme, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 应用主题发生修改时修改设置存储的主题值
        /// </summary>
        public static async Task SetThemeAsync(ThemeModel theme)
        {
            AppTheme = theme;

            await ConfigService.SaveSettingAsync(SettingsKey, theme.InternalName);
        }

        /// <summary>
        /// 设置应用显示的主题
        /// </summary>
        public static async Task SetAppThemeAsync()
        {
            if (Program.ApplicationRoot.MainWindow.Content is FrameworkElement frameworkElement)
            {
                frameworkElement.RequestedTheme = (ElementTheme)Enum.Parse(typeof(ElementTheme), AppTheme.InternalName);
            }
            await Task.CompletedTask;
        }
    }
}
