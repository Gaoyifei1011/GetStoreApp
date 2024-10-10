using System;
using System.Runtime.InteropServices.Marshalling;
using Windows.ApplicationModel.Resources.Core;

namespace GetStoreAppShellExtension.Services.Root
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
                catch (Exception currentResourceException)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(currentResourceException);

                    try
                    {
                        return resourceMap.GetValue(resource, defaultResourceContext).ValueAsString;
                    }
                    catch (Exception defaultResourceException)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(defaultResourceException);
                        return resource;
                    }
                }
            }
            else
            {
                return resource;
            }
        }
    }
}
