using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Helpers;
using GetStoreApp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Windows.Globalization;

namespace GetStoreApp.Services.Settings
{
    /// <summary>
    /// 应用语言设置服务
    /// </summary>
    public class LanguageService : ILanguageService
    {
        private string SettingsKey { get; init; } = "AppLanguage";

        private IConfigStorageService ConfigStorageService { get; } = IOCHelper.GetService<IConfigStorageService>();

        public LanguageModel DefaultAppLanguage { get; set; }

        public LanguageModel AppLanguage { get; set; }

        private static readonly IReadOnlyList<string> AppLanguages = ApplicationLanguages.ManifestLanguages;

        public List<LanguageModel> LanguageList { get; set; } = new List<LanguageModel>();

        /// <summary>
        /// 初始化应用语言信息列表
        /// </summary>
        private void InitializeLanguageList()
        {
            foreach (var item in AppLanguages)
            {
                CultureInfo culture = CultureInfo.GetCultureInfo(item);

                LanguageList.Add(new LanguageModel()
                {
                    DisplayName = culture.NativeName,
                    InternalName = culture.Name,
                });
            }
        }

        /// <summary>
        /// 当设置中的键值为空时，判断当前系统语言是否存在于LanguageList中
        /// </summary>
        private bool IsExistsInLanguageList(string currentSystemLanguage)
        {
            foreach (LanguageModel item in LanguageList)
            {
                if (item.InternalName == currentSystemLanguage)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 应用在初始化前获取设置存储的语言值
        /// </summary>
        public async Task InitializeLanguageAsync()
        {
            InitializeLanguageList();

            DefaultAppLanguage = LanguageList.Find(item => item.InternalName.Equals("en-US"));

            AppLanguage = await GetLanguageAsync();
        }

        /// <summary>
        /// 获取设置存储的语言值，如果设置没有存储，使用默认值
        /// </summary>
        private async Task<LanguageModel> GetLanguageAsync()
        {
            string language = await ConfigStorageService.GetSettingStringValueAsync(SettingsKey);

            // 当前系统的语言值
            string CurrentSystemLanguage = CultureInfo.CurrentCulture.Parent.Name;

            if (string.IsNullOrEmpty(language))
            {
                // 判断当前系统语言是否存在应用默认添加的语言列表中
                bool result = IsExistsInLanguageList(CurrentSystemLanguage);

                // 如果存在，设置存储值和应用初次设置的语言为当前系统的语言
                if (result)
                {
                    return LanguageList.Find(item => item.InternalName.Equals(CurrentSystemLanguage, StringComparison.OrdinalIgnoreCase));
                }
                // 不存在，设置存储值和应用初次设置的语言为默认语言：English(United States)
                else
                {
                    return LanguageList.Find(item => item.InternalName.Equals(DefaultAppLanguage.InternalName, StringComparison.OrdinalIgnoreCase));
                }
            }

            return LanguageList.Find(item => item.InternalName.Equals(language, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 应语言发生修改时修改设置存储的语言值
        /// </summary>
        public async Task SetLanguageAsync(LanguageModel language)
        {
            AppLanguage = language;

            await ConfigStorageService.SaveSettingStringValueAsync(SettingsKey, language.InternalName);
        }

        /// <summary>
        /// 设置应用使用的语言
        /// </summary>
        public async Task SetAppLanguageAsync()
        {
            ApplicationLanguages.PrimaryLanguageOverride = AppLanguage.InternalName;
            await Task.CompletedTask;
        }
    }
}
