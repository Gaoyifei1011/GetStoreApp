using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Windows.Globalization;

namespace GetStoreApp.Services.Controls.Settings
{
    /// <summary>
    /// 应用语言设置服务
    /// </summary>
    public static class LanguageService
    {
        private static readonly string settingsKey = ConfigKey.LanguageKey;

        public static DictionaryEntry DefaultAppLanguage { get; private set; }

        public static DictionaryEntry AppLanguage { get; private set; }

        public static FlowDirection FlowDirection { get; private set; }

        private static IReadOnlyList<string> AppLanguagesList { get; } = ApplicationLanguages.ManifestLanguages;

        public static List<DictionaryEntry> LanguageList { get; } = [];

        /// <summary>
        /// 初始化应用语言信息列表
        /// </summary>
        private static void InitializeLanguageList()
        {
            foreach (string applanguage in AppLanguagesList)
            {
                CultureInfo culture = CultureInfo.GetCultureInfo(applanguage);
                LanguageList.Add(new DictionaryEntry(culture.NativeName, culture.Name));
            }
        }

        /// <summary>
        /// 当设置中的键值为空时，判断当前系统语言是否存在于语言列表中
        /// </summary>
        private static bool IsExistsInLanguageList(CultureInfo currentCulture, out DictionaryEntry language)
        {
            foreach (DictionaryEntry languageItem in LanguageList)
            {
                if (languageItem.Value.ToString().Contains(currentCulture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    language = languageItem;
                    return true;
                }
            }

            language = new();
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

            // 当前系统语言和当前系统语言的父区域性的语言
            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            CultureInfo currentParentCulture = CultureInfo.CurrentCulture.Parent;
            bool existResult = false;

            if (language is null)
            {
                // 判断当前系统语言是否存在应用默认添加的语言列表中
                existResult = IsExistsInLanguageList(currentCulture, out DictionaryEntry currentLanguage);

                // 如果存在，设置存储值和应用初次设置的语言为当前系统的语言
                if (existResult)
                {
                    SetLanguage(currentLanguage);
                    FlowDirection = currentCulture.TextInfo.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
                    System.IO.File.AppendAllText("D:\\01.txt", currentCulture.TextInfo.IsRightToLeft.ToString() + Environment.NewLine);
                    return currentLanguage;
                }
                else
                {
                    existResult = IsExistsInLanguageList(currentParentCulture, out DictionaryEntry currentParentLanguage);

                    // 如果存在，设置存储值和应用初次设置的语言为当前系统语言的父区域性的语言
                    if (existResult)
                    {
                        SetLanguage(currentParentLanguage);
                        FlowDirection = currentParentCulture.TextInfo.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
                        System.IO.File.AppendAllText("D:\\01.txt", currentCulture.TextInfo.IsRightToLeft.ToString() + Environment.NewLine);
                        return currentParentLanguage;
                    }

                    // 不存在，设置存储值和应用初次设置的语言为默认语言：English(United States)
                    else
                    {
                        SetLanguage(DefaultAppLanguage);
                        FlowDirection = CultureInfo.GetCultureInfo(DefaultAppLanguage.Value.ToString()).TextInfo.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
                        System.IO.File.AppendAllText("D:\\01.txt", currentCulture.TextInfo.IsRightToLeft.ToString() + Environment.NewLine);
                        return DefaultAppLanguage;
                    }
                }
            }
            else
            {
                CultureInfo savedCultureInfo = CultureInfo.GetCultureInfo(language.ToString());
                FlowDirection = savedCultureInfo.TextInfo.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
                return LanguageList.Find(item => item.Value.ToString().Contains(language.ToString(), StringComparison.OrdinalIgnoreCase));
            }
        }

        /// <summary>
        /// 语言发生修改时修改设置存储的语言值
        /// </summary>
        public static void SetLanguage(DictionaryEntry language)
        {
            AppLanguage = language;
            LocalSettingsService.SaveSetting(settingsKey, language.Value);
            ApplicationLanguages.PrimaryLanguageOverride = language.Value.ToString();
        }
    }
}
