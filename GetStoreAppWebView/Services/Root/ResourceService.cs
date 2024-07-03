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
        private static bool isInitialized = false;

        private static string _defaultAppLanguage;
        private static string _currentAppLanguage;

        private static readonly ResourceManager resourceManager = new ResourceManager();
        private static readonly ResourceContext defaultResourceContext;
        private static readonly ResourceContext currentResourceContext;
        private static readonly ResourceMap resourceMap;

        static ResourceService()
        {
            defaultResourceContext = resourceManager.CreateResourceContext();
            currentResourceContext = resourceManager.CreateResourceContext();
            resourceMap = resourceManager.MainResourceMap;
        }

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
                catch (NullReferenceException)
                {
                    try
                    {
                        return resourceMap.GetValue(resource, defaultResourceContext).ValueAsString;
                    }
                    catch (NullReferenceException)
                    {
                        return resource;
                    }
                }
            }
            else
            {
                LogService.WriteLog(LoggingLevel.Warning, "Have you forgot to initialize app's resources?", new Exception());
                return resource;
            }
        }
    }
}
