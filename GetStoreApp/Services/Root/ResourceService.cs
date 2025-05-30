using Microsoft.Windows.ApplicationModel.Resources;
using System;
using System.Collections.Generic;
using Windows.Foundation.Diagnostics;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 应用资源服务
    /// </summary>
    public static class ResourceService
    {
        private static bool isInitialized;

        private static KeyValuePair<string, string> _defaultAppLanguage;
        private static KeyValuePair<string, string> _currentAppLanguage;

        private static readonly ResourceManager resourceManager = new();
        private static ResourceContext defaultResourceContext;
        private static ResourceContext currentResourceContext;
        private static ResourceMap resourceMap;

        /// <summary>
        /// 初始化应用本地化资源
        /// </summary>
        /// <param name="defaultAppLanguage">默认语言名称</param>
        /// <param name="currentAppLanguage">当前语言名称</param>
        public static void InitializeResource(KeyValuePair<string, string> defaultAppLanguage, KeyValuePair<string, string> currentAppLanguage)
        {
            _defaultAppLanguage = defaultAppLanguage;
            _currentAppLanguage = currentAppLanguage;

            defaultResourceContext = resourceManager.CreateResourceContext();
            currentResourceContext = resourceManager.CreateResourceContext();
            resourceMap = resourceManager.MainResourceMap;

            defaultResourceContext.QualifierValues["Language"] = _defaultAppLanguage.Key.ToString();
            currentResourceContext.QualifierValues["Language"] = _currentAppLanguage.Key.ToString();

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
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(ResourceService), nameof(GetLocalized), 1, currentResourceException);
                    try
                    {
                        return resourceMap.GetValue(resource, defaultResourceContext).ValueAsString;
                    }
                    catch (Exception defaultResourceException)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(ResourceService), nameof(GetLocalized), 2, defaultResourceException);
                        return resource;
                    }
                }
            }
            else
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(ResourceService), nameof(GetLocalized), 3, new Exception());
                return resource;
            }
        }

        /// <summary>
        /// 获取嵌入的数据
        /// </summary>
        public static byte[] GetEmbeddedData(string resource)
        {
            try
            {
                return resourceMap.GetValue(resource).ValueAsBytes;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(ResourceService), nameof(GetEmbeddedData), 1, e);
                return default;
            }
        }
    }
}
