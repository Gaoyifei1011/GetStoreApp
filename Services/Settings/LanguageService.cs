using GetStoreApp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using Windows.ApplicationModel.Resources.Core;
using Windows.Globalization;
using Windows.Storage;

namespace GetStoreApp.Services.Settings
{
    /// <summary>
    /// 语言设置服务
    /// Language settings service
    /// </summary>
    public static class LanguageService
    {
        /// <summary>
        /// 设置存储时需要使用到的键值
        /// The key value that you need to use when setting the store
        /// </summary>
        private const string SettingsKey = "AppSelectedLanguage";

        /// <summary>
        /// 默认显示的语言编码
        /// The default displayed language encoding
        /// </summary>
        private static readonly string DefaultLangCodeName;

        /// <summary>
        /// 应用初次打开时的语言编码
        /// The language encoding of the app when it was first opened
        /// </summary>
        public static readonly string PriLangCodeName;

        /// <summary>
        /// 应用程序清单包含的语言列表信息
        /// The application manifest contains language list information
        /// </summary>
        private static readonly IReadOnlyList<string> AppLanguages = ApplicationLanguages.ManifestLanguages;

        /// <summary>
        /// 自定义的语言列表
        /// A list of custom languages
        /// </summary>
        public static List<LanguageModel> LanguageList = new List<LanguageModel>();

        /// <summary>
        /// UI字符串本地化
        /// UI string localization
        /// </summary>
        private static ResourceContext resourceContext;

        private static ResourceMap resourceMap;

        /// <summary>
        /// 静态资源初始化
        /// </summary>
        static LanguageService()
        {
            // 初始化语言列表内容
            InitializeLanguageList();

            // 默认语言编码值:English(United States)
            DefaultLangCodeName = LanguageList.Find(item => item.CodeName.Equals("en-US")).CodeName;

            // 从设置存储中加载当前应用程序初次设定的语言编码值
            PriLangCodeName = GetLanguage();

            // UI字符串本地化
            resourceContext = new ResourceContext();

            resourceContext.QualifierValues["Language"] = PriLangCodeName;

            resourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Resources");
        }

        /// <summary>
        /// 应用初始化时，系统关于该键值存储的信息为空，所以需要判断系统存储的键值是否为空
        /// When the application is initialized, the system information about the key value storage is empty, so you need to determine whether the system stored key value is empty
        /// </summary>
        /// <returns>键值存储的信息是否为空</returns>
        private static bool IsSettingsKeyNullOrEmpty()
        {
            return ApplicationData.Current.LocalSettings.Values[SettingsKey] == null;
        }

        /// <summary>
        /// 当设置中的键值为空时，判断当前系统语言是否存在于LanguageList中
        /// When the key value in the setting is empty, determine whether the current system language exists in the QueueList
        /// </summary>
        /// <param name="currSysLangCodeName">当前系统的语言编码值</param>
        /// <returns>当前系统选定的语言编码值是否存在语言列表LanguageList的布尔值</returns>
        private static bool IsExistsInLanguageList(string currSysLangCodeName)
        {
            foreach (LanguageModel item in LanguageList)
            {
                if (item.CodeName == currSysLangCodeName)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 设置应用展示的语言值
        /// Detects whether the stored key value is empty, and if it does not exist, sets the default value
        /// </summary>
        /// <param name="LangCodeName">选定的语言编码值</param>
        private static void InitializeSettingsKey(string LangCodeName)
        {
            SetLanguage(LangCodeName);
        }

        /// <summary>
        /// 初始化语言列表信息
        /// Initializes the language list information
        /// </summary>
        private static void InitializeLanguageList()
        {
            foreach (var item in AppLanguages)
            {
                CultureInfo culture = CultureInfo.GetCultureInfo(item);
                LanguageList.Add(new LanguageModel(culture.NativeName, culture.Name));
            }
        }

        /// <summary>
        /// 获取设置存储的语言编码
        /// Gets the language encoding for the settings store
        /// </summary>
        /// <returns>返回存储的语言编码信息</returns>
        private static string GetLanguage()
        {
            // 当前系统的语言编码
            string CurrSysLangCodeName = CultureInfo.CurrentCulture.Parent.Name;

            // 先检查默认值是否存在，设定完默认值后然后获取语言编码值
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

            // 获取设置存储的语言编码
            return LoadLanguageFromSettings();
        }

        /// <summary>
        /// UI字符串本地化
        /// UI string localization
        /// </summary>
        /// <param name="resource"></param>
        /// <returns>返回本地化后的信息</returns>
        public static string GetResources(string resource)
        {
            try
            {
                return resourceMap.GetValue(resource, resourceContext).ValueAsString;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取设置存储的语言编码
        /// Gets the language encoding for the settings store
        /// </summary>
        /// <returns>设置存储的语言编码</returns>
        private static string LoadLanguageFromSettings()
        {
            return ApplicationData.Current.LocalSettings.Values[SettingsKey].ToString();
        }

        /// <summary>
        /// 设置当前应用显示的语言和设置存储的语言编码
        /// Sets the language that the current app displays and sets the language encoding stored
        /// </summary>
        /// <param name="LangCodeName">传入的语言编码值</param>
        public static void SetLanguage(string LangCodeName)
        {
            // 修改应用界面显示的语言（重启后生效）
            SetRequestedLanguageCode(LangCodeName);
            // 修改设置存储的值
            ApplicationData.Current.LocalSettings.Values[SettingsKey] = LangCodeName;
        }

        /// <summary>
        /// 修改应用界面显示的语言（重启后生效）
        /// Modify the language displayed in the app interface (effective after reboot)
        /// </summary>
        /// <param name="LangCodeName">传入的语言编码值</param>
        private static void SetRequestedLanguageCode(string LangCodeName)
        {
            ApplicationLanguages.PrimaryLanguageOverride = LangCodeName;
        }
    }
}