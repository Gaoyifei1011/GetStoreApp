﻿using GetStoreAppShellExtension.Extensions.DataType.Constant;
using GetStoreAppShellExtension.Services.Root;
using System;
using System.Collections.Generic;
using Windows.Globalization;

namespace GetStoreAppShellExtension.Services.Settings
{
    /// <summary>
    /// 应用语言设置服务
    /// </summary>
    public static class LanguageService
    {
        private static readonly string settingsKey = ConfigKey.LanguageKey;

        public static string DefaultAppLanguage { get; private set; }

        public static string AppLanguage { get; private set; }

        private static IReadOnlyList<string> AppLanguagesList { get; } = ApplicationLanguages.ManifestLanguages;

        /// <summary>
        /// 应用在初始化前获取设置存储的语言值，如果设置值为空，设定默认的应用语言值
        /// </summary>
        public static void InitializeLanguage()
        {
            foreach (string language in AppLanguagesList)
            {
                if (string.Equals(language, "en-US", StringComparison.OrdinalIgnoreCase))
                {
                    DefaultAppLanguage = language;
                }
            }

            AppLanguage = LocalSettingsService.ReadSetting<string>(settingsKey);

            if (string.IsNullOrEmpty(AppLanguage))
            {
                AppLanguage = DefaultAppLanguage;
            }

            ApplicationLanguages.PrimaryLanguageOverride = AppLanguage;
        }
    }
}
