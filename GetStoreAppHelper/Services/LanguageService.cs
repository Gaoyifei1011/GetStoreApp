using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Globalization;

namespace GetStoreAppHelper.Services
{
    public static class LanguageService
    {
        public static string DefaultAppLanguage { get; set; }

        public static string AppLanguage { get; set; }

        private static IReadOnlyList<string> AppLanguagesList { get; } = ApplicationLanguages.ManifestLanguages;

        public static async Task InitializeLanguageAsync()
        {
            DefaultAppLanguage = AppLanguagesList.First(item => item.Equals("en-US", StringComparison.OrdinalIgnoreCase));

            if (string.IsNullOrEmpty(ApplicationLanguages.PrimaryLanguageOverride))
            {
                AppLanguage = "zh-hans";
            }
            else
            {
                AppLanguage = ApplicationLanguages.PrimaryLanguageOverride;
            }

            await Task.CompletedTask;
        }
    }
}
