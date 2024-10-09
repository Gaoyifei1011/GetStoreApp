using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using Microsoft.UI.Xaml;
using System;
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

        public static KeyValuePair<string, string> DefaultAppLanguage { get; private set; }

        public static KeyValuePair<string, string> AppLanguage { get; private set; }

        public static FlowDirection FlowDirection { get; private set; }

        private static IReadOnlyList<string> AppLanguagesList { get; } = ApplicationLanguages.ManifestLanguages;

        public static List<KeyValuePair<string, string>> LanguageList { get; } = [];

        /// <summary>
        /// 应用在初始化前获取设置存储的语言值，如果设置值为空，设定默认的应用语言值
        /// </summary>
        public static void InitializeLanguage()
        {
            InitializeLanguageList();

            DefaultAppLanguage = LanguageList.Find(item => item.Key.Equals("en-US", StringComparison.OrdinalIgnoreCase));

            AppLanguage = GetLanguage();
        }

        /// <summary>
        /// 初始化应用语言信息列表
        /// </summary>
        private static void InitializeLanguageList()
        {
            foreach (string applanguage in AppLanguagesList)
            {
                CultureInfo culture = CultureInfo.GetCultureInfo(applanguage);
                LanguageList.Add(KeyValuePair.Create(culture.Name, culture.NativeName));
            }
            LanguageList.Sort((item1, item2) => item1.Key.CompareTo(item2.Key));
        }

        /// <summary>
        /// 当设置中的键值为空时，判断当前系统语言是否存在于语言列表中
        /// </summary>
        private static bool IsExistsInLanguageList(CultureInfo currentCulture, out KeyValuePair<string, string> language)
        {
            foreach (KeyValuePair<string, string> languageItem in LanguageList)
            {
                if (languageItem.Key.Contains(currentCulture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    language = languageItem;
                    return true;
                }
            }

            language = new();
            return false;
        }

        /// <summary>
        /// 获取设置存储的语言值，如果设置没有存储，使用默认值
        /// </summary>
        private static KeyValuePair<string, string> GetLanguage()
        {
            object language = LocalSettingsService.ReadSetting<object>(settingsKey);

            // 当前系统语言和当前系统语言的父区域性的语言
            CultureInfo currentCultureInfo = CultureInfo.CurrentCulture;
            CultureInfo currentParentCultureInfo = CultureInfo.CurrentCulture.Parent;
            bool existResult = false;

            if (language is null)
            {
                // 判断当前系统语言是否存在应用默认添加的语言列表中
                existResult = IsExistsInLanguageList(currentCultureInfo, out KeyValuePair<string, string> currentLanguage);

                // 如果存在，设置存储值和应用初次设置的语言为当前系统的语言
                if (existResult)
                {
                    SetLanguage(currentLanguage);
                    FlowDirection = currentCultureInfo.TextInfo.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
                    User32Library.SetProcessDefaultLayout(Convert.ToUInt32(currentCultureInfo.TextInfo.IsRightToLeft));
                    return currentLanguage;
                }
                else
                {
                    existResult = IsExistsInLanguageList(currentParentCultureInfo, out KeyValuePair<string, string> currentParentLanguage);

                    // 如果存在，设置存储值和应用初次设置的语言为当前系统语言的父区域性的语言
                    if (existResult)
                    {
                        SetLanguage(currentParentLanguage);
                        FlowDirection = currentParentCultureInfo.TextInfo.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
                        User32Library.SetProcessDefaultLayout(Convert.ToUInt32(currentParentCultureInfo.TextInfo.IsRightToLeft));
                        return currentParentLanguage;
                    }

                    // 不存在，设置存储值和应用初次设置的语言为默认语言：English(United States)
                    else
                    {
                        SetLanguage(DefaultAppLanguage);
                        CultureInfo defaultCultureInfo = CultureInfo.GetCultureInfo(DefaultAppLanguage.Key);
                        FlowDirection = defaultCultureInfo.TextInfo.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
                        User32Library.SetProcessDefaultLayout(Convert.ToUInt32(defaultCultureInfo.TextInfo.IsRightToLeft));
                        return DefaultAppLanguage;
                    }
                }
            }
            else
            {
                CultureInfo savedCultureInfo = CultureInfo.GetCultureInfo(language.ToString());
                FlowDirection = savedCultureInfo.TextInfo.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
                User32Library.SetProcessDefaultLayout(Convert.ToUInt32(savedCultureInfo.TextInfo.IsRightToLeft));
                return LanguageList.Find(item => language.ToString().Contains(item.Key, StringComparison.OrdinalIgnoreCase));
            }
        }

        /// <summary>
        /// 语言发生修改时修改设置存储的语言值
        /// </summary>
        public static void SetLanguage(KeyValuePair<string, string> language)
        {
            AppLanguage = language;
            LocalSettingsService.SaveSetting(settingsKey, language.Key);
            ApplicationLanguages.PrimaryLanguageOverride = language.Key;
        }
    }
}
