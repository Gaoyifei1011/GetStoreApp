using TaskbarPinner.Properties;
using System;
using Windows.ApplicationModel.Resources.Core;

namespace TaskbarPinner.Services.Root
{
    /// <summary>
    /// 应用资源服务
    /// </summary>
    public static class ResourceService
    {
        private static bool IsInitialized = false;

        private static string DefaultAppLanguage;
        private static string CurrentAppLanguage;

        private static ResourceContext DefaultResourceContext = new ResourceContext();
        private static ResourceContext CurrentResourceContext = new ResourceContext();
        private static ResourceMap ResourceMap = ResourceManager.Current.MainResourceMap;

        /// <summary>
        /// 初始化应用本地化资源
        /// </summary>
        /// <param name="defaultAppLanguage">默认语言名称</param>
        /// <param name="currentAppLanguage">当前语言名称</param>
        public static void InitializeResource(string defaultAppLanguage, string currentAppLanguage)
        {
            DefaultAppLanguage = defaultAppLanguage;
            CurrentAppLanguage = currentAppLanguage;

            DefaultResourceContext.QualifierValues["Language"] = DefaultAppLanguage;
            CurrentResourceContext.QualifierValues["Language"] = CurrentAppLanguage;

            IsInitialized = true;
        }

        /// <summary>
        /// 字符串本地化
        /// </summary>
        public static string GetLocalized(string resource)
        {
            if (IsInitialized)
            {
                try
                {
                    return ResourceMap.GetValue(resource, CurrentResourceContext).ValueAsString;
                }
                catch (NullReferenceException)
                {
                    try
                    {
                        return ResourceMap.GetValue(resource, DefaultResourceContext).ValueAsString;
                    }
                    catch (NullReferenceException)
                    {
                        return resource;
                    }
                }
            }
            else
            {
                throw new ApplicationException(Resources.ResourcesInitializeFailed);
            }
        }
    }
}
