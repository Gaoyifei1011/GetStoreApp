using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace GetStoreAppHelper.Services
{
    public static class ThemeService
    {
        private static string SettingsKey { get; } = "AppTheme";

        public static string AppTheme { get; set; }

        public static async Task InitializeThemeAsync()
        {
            AppTheme = await ConfigService.ReadSettingAsync<string>(SettingsKey);

            if (string.IsNullOrEmpty(AppTheme))
            {
                AppTheme = ElementTheme.Default.ToString();
            }
        }
    }
}
