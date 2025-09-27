using Microsoft.Windows.Storage;

namespace GetStoreAppShellExtension.Services.Root
{
    /// <summary>
    /// 应用本地设置服务
    /// </summary>
    public static class LocalSettingsService
    {
        private static readonly ApplicationDataContainer localSettingsContainer = ApplicationData.GetDefault().LocalSettings;

        /// <summary>
        /// 读取设置选项存储信息
        /// </summary>
        public static T ReadSetting<T>(string key)
        {
            return localSettingsContainer.Values.TryGetValue(key, out object value) ? (T)value : default;
        }
    }
}
