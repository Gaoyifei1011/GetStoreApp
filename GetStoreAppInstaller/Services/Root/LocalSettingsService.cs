using Windows.Storage;

namespace GetStoreAppInstaller.Services.Root
{
    /// <summary>
    /// 设置选项配置服务
    /// </summary>
    public static class LocalSettingsService
    {
        private static readonly ApplicationDataContainer localSettingsContainer = ApplicationData.Current.LocalSettings;

        /// <summary>
        /// 读取设置选项存储信息
        /// </summary>
        public static T ReadSetting<T>(string key)
        {
            return localSettingsContainer.Values[key] is not null ? (T)localSettingsContainer.Values[key] : default;
        }
    }
}
