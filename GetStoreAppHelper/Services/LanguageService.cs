using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Globalization;

namespace GetStoreAppHelper.Services
{
    public static class LanguageService
    {
        private static string SettingsKey { get; } = "AppLanguage";

        public static string DefaultAppLanguage { get; set; }

        public static string AppLanguage { get; set; }

        private static IReadOnlyList<string> AppLanguagesList { get; } = ApplicationLanguages.ManifestLanguages;

        public static async Task InitializeLanguageAsync()
        {
            DefaultAppLanguage = AppLanguagesList.First(item => item.Equals("en-US", StringComparison.OrdinalIgnoreCase));

            AppLanguage = await ConfigService.ReadSettingAsync<string>(SettingsKey);

            if (string.IsNullOrEmpty(AppLanguage))
            {
                AppLanguage = "zh-hans";
            }

            await Task.CompletedTask;
        }
    }
}
