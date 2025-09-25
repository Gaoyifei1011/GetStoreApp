using Microsoft.Windows.ApplicationModel.Resources;
using System;
using Windows.Foundation.Diagnostics;

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

        private static readonly ResourceManager resourceManager = new();
        private static ResourceContext defaultResourceContext;
        private static ResourceContext currentResourceContext;
        private static ResourceMap resourceMap;

        /// <summary>
        /// 初始化应用本地化资源
        /// </summary>
        /// <param name="defaultAppLanguage">默认语言名称</param>
        /// <param name="currentAppLanguage">当前语言名称</param>
        public static void InitializeResource(string defaultAppLanguage, string currentAppLanguage)
        {
            _defaultAppLanguage = defaultAppLanguage;
            _currentAppLanguage = currentAppLanguage;

            defaultResourceContext = resourceManager.CreateResourceContext();
            currentResourceContext = resourceManager.CreateResourceContext();
            resourceMap = resourceManager.MainResourceMap;

            defaultResourceContext.QualifierValues["Language"] = Convert.ToString(_defaultAppLanguage);
            currentResourceContext.QualifierValues["Language"] = Convert.ToString(_currentAppLanguage);

            isInitialized = true;
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
