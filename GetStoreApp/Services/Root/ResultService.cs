using Windows.Storage;

namespace GetStoreApp.Services.Root
{
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

        /// <summary>
        /// 保存结果存储信息
        /// </summary>
        public static void SaveResult<T>(string key, T value)
        {
            resultContainer.Values[key] = value;
        }
    }
}
