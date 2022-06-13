using GetStoreApp.Helpers;
using Microsoft.UI.Xaml;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace GetStoreApp.Services.Settings
{
    public static class ThemeSelectorService
    {
        private const string SettingsKey = "AppBackgroundRequestedTheme";

        public static ElementTheme Theme { get; set; }

        static ThemeSelectorService()
        {
            Theme = GetTheme();
        }

        public static async Task<ElementTheme> InitializeAsync()
        {
            Theme = await LoadThemeFromSettingsAsync();
            return await LoadThemeFromSettingsAsync();
        }

        public static async Task SetThemeAsync(ElementTheme theme)
        {
            Theme = theme;

            await SetRequestedThemeAsync();
            await SaveThemeInSettingsAsync(Theme);
        }

        public static async Task SetRequestedThemeAsync()
        {
            if (GetStoreApp.App.MainWindow.Content is FrameworkElement frameworkElement)
            {
                frameworkElement.RequestedTheme = Theme;
            }
            await Task.CompletedTask;
        }

        private static async Task SaveThemeInSettingsAsync(ElementTheme theme)
        {
            await ApplicationData.Current.LocalSettings.SaveAsync(SettingsKey, theme.ToString());
        }

        private static async Task<ElementTheme> LoadThemeFromSettingsAsync()
        {
            ElementTheme cacheTheme = ElementTheme.Default;
            string themeName = await ApplicationData.Current.LocalSettings.ReadAsync<string>(SettingsKey);

            if (!string.IsNullOrEmpty(themeName))
            {
                Enum.TryParse(themeName, out cacheTheme);
            }

            return cacheTheme;
        }

        /// <summary>
        /// 获取设置中存储的主题值，非异步方法
        /// </summary>
        public static ElementTheme GetTheme()
        {
            ElementTheme cacheTheme = ElementTheme.Default;
            string themeName = (string)ApplicationData.Current.LocalSettings.Values[SettingsKey];

            if (!string.IsNullOrEmpty(themeName))
            {
                Enum.TryParse(themeName, out cacheTheme);
            }

            return cacheTheme;
        }
    }
}
