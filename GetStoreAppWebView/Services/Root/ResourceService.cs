using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation.Diagnostics;
using Windows.UI.Xaml;

namespace GetStoreAppWebView.Services.Root
{
    /// <summary>
    /// 应用资源服务
    /// </summary>
    public static class ResourceService
    {
        private static bool isInitialized;

        private static string _defaultAppLanguage;
        private static string _currentAppLanguage;

        private static readonly ResourceContext defaultResourceContext = new();
        private static readonly ResourceContext currentResourceContext = new();
        private static readonly ResourceMap resourceMap = ResourceManager.Current.MainResourceMap;

        public static List<string> ThemeList { get; } = [];

        /// <summary>
        /// 初始化应用本地化资源
        /// </summary>
        /// <param name="defaultAppLanguage">默认语言名称</param>
        /// <param name="currentAppLanguage">当前语言名称</param>
        public static void InitializeResource(string defaultAppLanguage, string currentAppLanguage)
        {
            _defaultAppLanguage = defaultAppLanguage;
            _currentAppLanguage = currentAppLanguage;

            defaultResourceContext.QualifierValues["Language"] = _defaultAppLanguage;
            currentResourceContext.QualifierValues["Language"] = _currentAppLanguage;

            isInitialized = true;
        }

        /// <summary>
        /// 初始化应用本地化信息
        /// </summary>
        public static void LocalizeReosurce()
        {
            InitializeThemeList();
        }

        /// <summary>
        /// 初始化应用主题信息列表
        /// </summary>
        private static void InitializeThemeList()
        {
            ThemeList.Add(nameof(ElementTheme.Default));
            ThemeList.Add(nameof(ElementTheme.Light));
            ThemeList.Add(nameof(ElementTheme.Dark));
        }

        /// <summary>
        /// 字符串本地化
        /// </summary>
        public static string GetLocalized(string resource)
        {
            if (isInitialized)
            {
                try
                {
                    return resourceMap.GetValue(resource, currentResourceContext).ValueAsString;
                }
                catch (Exception currentResourceException)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreAppWebView), nameof(ResourceService), nameof(GetLocalized), 1, currentResourceException);
                    try
                    {
                        return resourceMap.GetValue(resource, defaultResourceContext).ValueAsString;
                    }
                    catch (Exception defaultResourceException)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreAppWebView), nameof(ResourceService), nameof(GetLocalized), 2, defaultResourceException);
                        return resource;
                    }
                }
            }
            else
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreAppWebView), nameof(ResourceService), nameof(GetLocalized), 3, new Exception());
                return resource;
            }
        }
    }
}
