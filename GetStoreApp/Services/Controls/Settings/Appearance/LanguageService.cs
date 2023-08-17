using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Helpers.Converters;
using GetStoreApp.Models.Controls.Settings;
using GetStoreApp.Services.Root;
using System;
using System.Collections.Generic;
using System.Globalization;
using Windows.Globalization;

namespace GetStoreApp.Services.Controls.Settings.Appearance
{
    /// <summary>
    /// 应用语言设置服务
    /// </summary>
    public static class LanguageService
    {
        private static string SettingsKey { get; } = ConfigKey.LanguageKey;

        public static GroupOptionsModel DefaultAppLanguage { get; set; }

        public static GroupOptionsModel AppLanguage { get; set; }

        private static IReadOnlyList<string> AppLanguagesList { get; } = ApplicationLanguages.ManifestLanguages;

        public static List<GroupOptionsModel> LanguageList { get; set; } = new List<GroupOptionsModel>();

        /// <summary>
        /// 初始化应用语言信息列表
        /// </summary>
        private static void InitializeLanguageList()
        {
            foreach (string applanguage in AppLanguagesList)
            {
                CultureInfo culture = CultureInfo.GetCultureInfo(applanguage);

                LanguageList.Add(new GroupOptionsModel()
                {
                    DisplayMember = culture.NativeName,
                    SelectedValue = culture.Name,
                });
            }
        }

        /// <summary>
        /// 当设置中的键值为空时，判断当前系统语言是否存在于语言列表中
        /// </summary>
        private static bool IsExistsInLanguageList(string currentSystemLanguage)
        {
            foreach (GroupOptionsModel languageItem in LanguageList)
            {
                if (languageItem.SelectedValue == currentSystemLanguage)
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

            DefaultAppLanguage = LanguageList.Find(item => item.SelectedValue.Equals("en-US", StringComparison.OrdinalIgnoreCase));

            (bool, GroupOptionsModel) LanguageValueResult = GetLanguage();

            AppLanguage = LanguageValueResult.Item2;

            if (LanguageValueResult.Item1)
            {
                SetLanguage(AppLanguage, false);
            }
        }

        /// <summary>
        /// 获取设置存储的语言值，如果设置没有存储，使用默认值
        /// </summary>
        private static (bool, GroupOptionsModel) GetLanguage()
        {
            string language = ConfigService.ReadSetting<string>(SettingsKey);

            // 当前系统的语言值
            string CurrentSystemLanguage = CultureInfo.CurrentCulture.Parent.Name;

            if (string.IsNullOrEmpty(language))
            {
                // 判断当前系统语言是否存在应用默认添加的语言列表中
                bool result = IsExistsInLanguageList(CurrentSystemLanguage);

                // 如果存在，设置存储值和应用初次设置的语言为当前系统的语言
                if (result)
                {
                    return (true, LanguageList.Find(item => item.SelectedValue.Equals(CurrentSystemLanguage, StringComparison.OrdinalIgnoreCase)));
                }

                // 不存在，设置存储值和应用初次设置的语言为默认语言：English(United States)
                else
                {
                    return (true, LanguageList.Find(item => item.SelectedValue.Equals(DefaultAppLanguage.SelectedValue, StringComparison.OrdinalIgnoreCase)));
                }
            }

            return (false, LanguageList.Find(item => item.SelectedValue.Equals(language, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// 语言发生修改时修改设置存储的语言值
        /// </summary>
        public static void SetLanguage(GroupOptionsModel language, bool isNotFirstSet = true)
        {
            if (isNotFirstSet)
            {
                AppLanguage = language;
            }

            ConfigService.SaveSetting(SettingsKey, language.SelectedValue);
            StringConverterHelper.AppCulture = new CultureInfo(language.SelectedValue);
        }
    }
}
