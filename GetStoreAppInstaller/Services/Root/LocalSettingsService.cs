using Microsoft.Windows.Storage;

namespace GetStoreAppInstaller.Services.Root
{
    /// <summary>
    /// 设置选项配置服务
    /// </summary>
    internal static class LocalSettingsService
    {
        private static readonly ApplicationDataContainer localSettingsContainer = ApplicationData.GetDefault().LocalSettings;

        /// <summary>
        /// 读取设置选项存储信息
        /// </summary>
        internal static T ReadSetting<T>(string key)
        {
            return localSettingsContainer.Values.TryGetValue(key, out object value) ? (T)value : default;
        }
    }
}
