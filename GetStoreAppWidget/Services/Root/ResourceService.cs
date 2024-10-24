using System;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation.Diagnostics;
using Windows.Storage.Streams;

namespace GetStoreAppWidget.Services.Root
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

        /// <summary>
        /// 获取嵌入的数据
        /// </summary>
        public static async Task<IBuffer> GetEmbeddedDataAsync(string resource)
        {
            try
            {
                IRandomAccessStream randomAccessStream = await resourceMap.GetValue(resource).GetValueAsStreamAsync();
                DataReader dataReader = new(randomAccessStream);
                await dataReader.LoadAsync((uint)randomAccessStream.Size);
                IBuffer buffer = dataReader.DetachBuffer();
                dataReader.Dispose();
                return buffer;
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}
