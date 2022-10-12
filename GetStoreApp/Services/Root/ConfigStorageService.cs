using GetStoreApp.Contracts.Services.Root;
using System.Threading.Tasks;
using Windows.Storage;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 应用配置存储服务
    /// </summary>
    public class ConfigStorageService : IConfigStorageService
    {
        public async Task<T> ReadSettingAsync<T>(string key)
        {
            if (ApplicationData.Current.LocalSettings.Values[key] == null)
            {
                return default;
            }

            return await Task.FromResult((T)ApplicationData.Current.LocalSettings.Values[key]);
        }

        public async Task SaveSettingAsync<T>(string key, T value)
        {
            ApplicationData.Current.LocalSettings.Values[key] = await Task.FromResult(value);
        }
    }
}
