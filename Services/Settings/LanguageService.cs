using GetStoreApp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using Windows.ApplicationModel.Resources.Core;
using Windows.Globalization;
using Windows.Storage;

namespace GetStoreApp.Services.Settings
{
    public static class LanguageService
    {
        private const string SettingsKey = "AppSelectedLanguage";

        private static readonly string DefaultLangCodeName;

        // 应用初次打开时的语言编码
        public static readonly string PriLangCodeName;

        private static readonly IReadOnlyList<string> AppLanguages = ApplicationLanguages.ManifestLanguages;

        public static List<LanguageModel> LanguageList { get; set; } = new List<LanguageModel>();

        private static readonly ResourceContext PriResourceContext = new ResourceContext();
        private static readonly ResourceContext DefaultResourceContext = new ResourceContext();

        private static readonly ResourceMap resourceMap;

        static LanguageService()
        {
            InitializeLanguageList();

            // 默认语言编码值:English(United States)
            DefaultLangCodeName = LanguageList.Find(item => item.InternalName.Equals("en-US")).InternalName;

            // 从设置存储中加载当前应用程序初次设定的语言编码值
            PriLangCodeName = GetLanguage();

            PriResourceContext.QualifierValues["Language"] = PriLangCodeName;
            DefaultResourceContext.QualifierValues["Language"] = DefaultLangCodeName;

            resourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Resources");
        }

        /// <summary>
        /// 应用初始化时，系统关于该键值存储的信息为空，所以需要判断系统存储的键值是否为空
        /// </summary>
        private static bool IsSettingsKeyNullOrEmpty()
        {
            return ApplicationData.Current.LocalSettings.Values[SettingsKey] == null;
        }

        /// <summary>
        /// 当设置中的键值为空时，判断当前系统语言是否存在于LanguageList中
        /// </summary>
        private static bool IsExistsInLanguageList(string currSysLangCodeName)
        {
            foreach (LanguageModel item in LanguageList)
            {
                if (item.InternalName == currSysLangCodeName)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 设置应用展示的语言值
        /// </summary>
        private static void InitializeSettingsKey(string LangCodeName)
        {
            SetLanguage(LangCodeName);
        }

        /// <summary>
        /// 初始化语言列表信息
        /// </summary>
        private static void InitializeLanguageList()
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
        /// 获取设置存储的语言编码
        /// </summary>
        private static string GetLanguage()
        {
            // 当前系统的语言编码
            string CurrSysLangCodeName = CultureInfo.CurrentCulture.Parent.Name;

            // 检测存储的键值是否为空,如果不存在,设置默认值
            if (IsSettingsKeyNullOrEmpty())
            {
                // 判断当前语言是否存在应用默认添加的语言列表中
                bool result = IsExistsInLanguageList(CurrSysLangCodeName);

                // 如果存在，设置存储值和应用初次设置的语言为当前系统的语言
                if (result)
                {
                    InitializeSettingsKey(CurrSysLangCodeName);
                }
                // 不存在，设置存储值和应用初次设置的语言为默认语言：English(United States)
                else
                {
                    InitializeSettingsKey(DefaultLangCodeName);
                }
            }

            return ApplicationData.Current.LocalSettings.Values[SettingsKey].ToString();
        }

        public static string GetResources(string resource)
        {
            try
            {
                return resourceMap.GetValue(resource, PriResourceContext).ValueAsString;
            }
            catch (NullReferenceException)
            {
                try
                {
                    return resourceMap.GetValue(resource, DefaultResourceContext).ValueAsString;
                }
                catch (NullReferenceException)
                {
                    return resource;
                }
            }
        }

        /// <summary>
        /// 修改设置
        /// </summary>
        public static void SetLanguage(string LangCodeName)
        {
            ApplicationLanguages.PrimaryLanguageOverride = LangCodeName;
            ApplicationData.Current.LocalSettings.Values[SettingsKey] = LangCodeName;
        }
    }
}
