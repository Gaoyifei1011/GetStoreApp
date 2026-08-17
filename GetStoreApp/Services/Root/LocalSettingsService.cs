using Microsoft.Windows.Storage;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 应用本地设置服务
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

        /// <summary>
        /// 保存设置选项存储信息
        /// </summary>
        internal static void SaveSetting<T>(string key, T value)
        {
            localSettingsContainer.Values[key] = value;
        }
    }
}
