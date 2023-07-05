using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Models.Controls.Settings.Appearance;
using GetStoreApp.Services.Root;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Windows.Globalization;

namespace GetStoreApp.Services.Controls.Settings.Appearance
{
    /// <summary>
    /// 应用语言设置服务
    /// </summary>
    public static class LanguageService
    {
        private static string SettingsKey { get; } = ConfigKey.LanguageKey;

        public static LanguageModel DefaultAppLanguage { get; set; }

        public static LanguageModel AppLanguage { get; set; }

        private static IReadOnlyList<string> AppLanguagesList { get; } = ApplicationLanguages.ManifestLanguages;

        public static List<LanguageModel> LanguageList { get; set; } = new List<LanguageModel>();

        /// <summary>
        /// 初始化应用语言信息列表
        /// </summary>
        private static void InitializeLanguageList()
        {
            foreach (string applanguage in AppLanguagesList)
            {
                CultureInfo culture = CultureInfo.GetCultureInfo(applanguage);

                LanguageList.Add(new LanguageModel()
                {
                    DisplayName = culture.NativeName,
                    InternalName = culture.Name,
                });
            }
        }

        /// <summary>
        /// 当设置中的键值为空时，判断当前系统语言是否存在于语言列表中
        /// </summary>
        private static bool IsExistsInLanguageList(string currentSystemLanguage)
        {
            foreach (LanguageModel languageItem in LanguageList)
            {
                if (languageItem.InternalName == currentSystemLanguage)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 应用在初始化前获取设置存储的语言值，如果设置值为空，设定默认的应用语言值
        /// </summary>
        public static async Task InitializeLanguageAsync()
        {
            InitializeLanguageList();

            DefaultAppLanguage = LanguageList.Find(item => item.InternalName.Equals("en-US", StringComparison.OrdinalIgnoreCase));

            (bool, LanguageModel) LanguageValueResult = await GetLanguageAsync();

            AppLanguage = LanguageValueResult.Item2;

            if (LanguageValueResult.Item1)
            {
                await SetLanguageAsync(AppLanguage, false);
            }
        }

        /// <summary>
        /// 获取设置存储的语言值，如果设置没有存储，使用默认值
        /// </summary>
        private static async Task<(bool, LanguageModel)> GetLanguageAsync()
        {
            string language = await ConfigService.ReadSettingAsync<string>(SettingsKey);

            // 当前系统的语言值
            string CurrentSystemLanguage = CultureInfo.CurrentCulture.Parent.Name;

            if (string.IsNullOrEmpty(language))
            {
                // 判断当前系统语言是否存在应用默认添加的语言列表中
                bool result = IsExistsInLanguageList(CurrentSystemLanguage);

                // 如果存在，设置存储值和应用初次设置的语言为当前系统的语言
                if (result)
                {
                    return (true, LanguageList.Find(item => item.InternalName.Equals(CurrentSystemLanguage, StringComparison.OrdinalIgnoreCase)));
                }

                // 不存在，设置存储值和应用初次设置的语言为默认语言：English(United States)
                else
                {
                    return (true, LanguageList.Find(item => item.InternalName.Equals(DefaultAppLanguage.InternalName, StringComparison.OrdinalIgnoreCase)));
                }
            }

            return (false, LanguageList.Find(item => item.InternalName.Equals(language, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// 语言发生修改时修改设置存储的语言值
        /// </summary>
        public static async Task SetLanguageAsync(LanguageModel language, bool isNotFirstSet = true)
        {
            if (isNotFirstSet)
            {
                AppLanguage = language;
            }

            await ConfigService.SaveSettingAsync(SettingsKey, language.InternalName);
#if !EXPERIMENTAL
            ApplicationLanguages.PrimaryLanguageOverride = language.InternalName;
#endif
        }
    }
}
