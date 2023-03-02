using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Globalization;

namespace GetStoreAppHelper.Services
{
    /// <summary>
    /// 应用语言设置服务
    /// </summary>
    public static class LanguageService
    {
        private static string SettingsKey { get; } = "AppLanguage";

        public static string DefaultAppLanguage { get; set; }

        public static string AppLanguage { get; set; }

        private static IReadOnlyList<string> AppLanguagesList { get; } = ApplicationLanguages.ManifestLanguages;

        /// <summary>
        /// 应用在初始化前获取设置存储的语言值，如果设置值为空，设定默认的应用语言值
        /// </summary>
        public static async Task InitializeLanguageAsync()
        {
            DefaultAppLanguage = AppLanguagesList.First(item => item.Equals("en-US", StringComparison.OrdinalIgnoreCase));

            AppLanguage = await ConfigService.ReadSettingAsync<string>(SettingsKey);

            if (string.IsNullOrEmpty(AppLanguage))
            {
                AppLanguage = DefaultAppLanguage;
            }

            await Task.CompletedTask;
        }
    }
}
