using Windows.Storage;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 存储运行结果服务
    /// </summary>
    public static class ResultService
    {
        private const string result = "Result";

        private static readonly ApplicationDataContainer localSettingsContainer = ApplicationData.Current.LocalSettings;
        private static ApplicationDataContainer resultContainer;

        /// <summary>
        /// 初始化结果记录存储服务
        /// </summary>
        public static void Initialize()
        {
            resultContainer = localSettingsContainer.CreateContainer(result, ApplicationDataCreateDisposition.Always);
        }

        /// <summary>
        /// 读取结果存储信息
        /// </summary>
        public static T ReadResult<T>(string key)
        {
            if (resultContainer.Values[key] is null)
            {
                return default;
            }

            return (T)resultContainer.Values[key];
        }
    }
}
