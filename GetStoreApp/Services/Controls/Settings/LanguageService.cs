using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;
using System;
using System.Collections;
using System.Collections.Generic;
using Windows.Globalization;

namespace GetStoreApp.Services.Controls.Settings
{
    /// <summary>
    /// 应用语言设置服务
    /// </summary>
    public static class LanguageService
    {
        private static string settingsKey = ConfigKey.LanguageKey;

        public static DictionaryEntry DefaultAppLanguage { get; private set; }

        public static DictionaryEntry AppLanguage { get; private set; }

        private static IReadOnlyList<string> AppLanguagesList { get; } = ApplicationLanguages.ManifestLanguages;

        public static List<DictionaryEntry> LanguageList { get; } = new List<DictionaryEntry>();

        /// <summary>
        /// 初始化应用语言信息列表
        /// </summary>
        private static void InitializeLanguageList()
        {
            foreach (string applanguage in AppLanguagesList)
            {
                LanguageList.Add(new DictionaryEntry(new Language(applanguage).NativeName, applanguage));
            }
        }

        /// <summary>
        /// 当设置中的键值为空时，判断当前系统语言是否存在于语言列表中
        /// </summary>
        private static bool IsExistsInLanguageList(string currentSystemLanguage)
        {
            foreach (DictionaryEntry languageItem in LanguageList)
            {
                if (languageItem.Value.ToString().Contains(currentSystemLanguage, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 应用在初始化前获取设置存储的语言值，如果设置值为空，设定默认的应用语言值
        /// </summary>
        public static void InitializeLanguage()
        {
            InitializeLanguageList();

            DefaultAppLanguage = LanguageList.Find(item => item.Value.ToString().Equals("en-US", StringComparison.OrdinalIgnoreCase));

            AppLanguage = GetLanguage();
        }

        /// <summary>
        /// 获取设置存储的语言值，如果设置没有存储，使用默认值
        /// </summary>
        private static DictionaryEntry GetLanguage()
        {
            object language = LocalSettingsService.ReadSetting<object>(settingsKey);

            // 当前系统的语言值
            string currentSystemLanguage = ApplicationLanguages.Languages[0];

            if (language is null)
            {
                // 判断当前系统语言是否存在应用默认添加的语言列表中
                bool result = IsExistsInLanguageList(currentSystemLanguage);

                // 如果存在，设置存储值和应用初次设置的语言为当前系统的语言
                if (result)
                {
                    DictionaryEntry currentLanguage = LanguageList.Find(item => item.Value.ToString().Contains(currentSystemLanguage, StringComparison.OrdinalIgnoreCase));
                    SetLanguage(currentLanguage);
                    return currentLanguage;
                }

                // 不存在，设置存储值和应用初次设置的语言为默认语言：English(United States)
                else
                {
                    SetLanguage(DefaultAppLanguage);
                    return DefaultAppLanguage;
                }
            }

            return LanguageList.Find(item => item.Value.ToString().Contains(language.ToString(), StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 语言发生修改时修改设置存储的语言值
        /// </summary>
        public static void SetLanguage(DictionaryEntry language)
        {
            AppLanguage = language;

            LocalSettingsService.SaveSetting(settingsKey, language.Value);
        }
    }
}
