using GetStoreAppConsole.Contracts;
using GetStoreAppConsole.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Windows.Globalization;

namespace GetStoreAppConsole.Services
{
    /// <summary>
    /// 控制台语言服务
    /// </summary>
    public class LanguageService : ILanguageService
    {
        private IConfigStoreageService ConfigStorageService { get; } = IOCHelper.GetService<IConfigStoreageService>();

        private string SettingsKey { get; init; } = "AppLanguage";

        public string DefaultConsoleLanguage { get; set; }

        public string ConsoleLanguage { get; set; }

        private readonly IReadOnlyList<string> AppLanguagesList = ApplicationLanguages.ManifestLanguages;

        public List<string> LanguageList { get; set; } = new List<string>();

        /// <summary>
        /// 初始化控制台语言信息列表
        /// </summary>
        private void InitializeLanguageList()
        {
            foreach (string applanguage in AppLanguagesList)
            {
                CultureInfo culture = CultureInfo.GetCultureInfo(applanguage);

                LanguageList.Add(culture.Name);
            }
        }

        /// <summary>
        /// 当设置中的键值为空时，判断当前系统语言是否存在于语言列表中
        /// </summary>
        private bool IsExistsInLanguageList(string currentSystemLanguage)
        {
            foreach (string appLanguageItem in AppLanguagesList)
            {
                if (CultureInfo.GetCultureInfo(appLanguageItem).Name == currentSystemLanguage)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 控制台在初始化前获取设置存储的语言值，如果设置值为空，设定默认的应用语言值
        /// </summary>
        public async Task InitializeLanguageAsync()
        {
            InitializeLanguageList();

            DefaultConsoleLanguage = LanguageList.Find(item => item.Equals("en-US", StringComparison.OrdinalIgnoreCase));

            ConsoleLanguage = await GetLanguageAsync();
        }

        /// <summary>
        /// 获取设置存储的语言值，如果设置没有存储，使用默认值
        /// </summary>
        public async Task<string> GetLanguageAsync()
        {
            string language = await ConfigStorageService.ReadSettingAsync<string>(SettingsKey);

            // 当前系统的语言值
            string CurrentSystemLanguage = CultureInfo.CurrentCulture.Parent.Name;

            if (string.IsNullOrEmpty(language))
            {
                // 判断当前系统语言是否存在应用默认添加的语言列表中
                bool result = IsExistsInLanguageList(CurrentSystemLanguage);

                // 如果存在，设置存储值和控制台初次设置的语言为当前系统的语言
                if (result)
                {
                    return LanguageList.Find(item => item.Equals(CurrentSystemLanguage, StringComparison.OrdinalIgnoreCase));
                }

                // 不存在，设置存储值和控制台初次设置的语言为默认语言：English(United States)
                else
                {
                    return LanguageList.Find(item => item.Equals(DefaultConsoleLanguage, StringComparison.OrdinalIgnoreCase));
                }
            }

            return LanguageList.Find(item => item.Equals(language, StringComparison.OrdinalIgnoreCase));
        }
    }
}
