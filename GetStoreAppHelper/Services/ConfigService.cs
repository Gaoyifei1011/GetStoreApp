using System.Threading.Tasks;
using Windows.Storage;

namespace GetStoreAppHelper.Services
{
    /// <summary>
    /// 设置选项配置服务
    /// </summary>
    public static class ConfigService
    {
        /// <summary>
        /// 读取设置选项存储信息
        /// </summary>
        public static async Task<T> ReadSettingAsync<T>(string key)
        {
            if (ApplicationData.Current.LocalSettings.Values[key] is null)
            {
                return default;
            }

            return await Task.FromResult((T)ApplicationData.Current.LocalSettings.Values[key]);
        }
    }
}
