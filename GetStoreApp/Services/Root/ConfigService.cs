using Windows.Storage;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 设置选项配置服务
    /// </summary>
    public static class ConfigService
    {
        /// <summary>
        /// 读取设置选项存储信息
        /// </summary>
        public static T ReadSetting<T>(string key)
        {
            if (ApplicationData.Current.LocalSettings.Values[key] is null)
            {
                return default;
            }

            return (T)ApplicationData.Current.LocalSettings.Values[key];
        }

        /// <summary>
        /// 保存设置选项存储信息
        /// </summary>
        public static void SaveSetting<T>(string key, T value)
        {
            ApplicationData.Current.LocalSettings.Values[key] = value;
        }
    }
}
