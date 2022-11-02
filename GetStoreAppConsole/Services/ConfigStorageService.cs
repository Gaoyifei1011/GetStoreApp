using GetStoreAppConsole.Contracts;
using System.Threading.Tasks;
using Windows.Storage;

namespace GetStoreAppConsole.Services
{
    public class ConfigStorageService : IConfigStoreageService
    {
        public async Task<T> ReadSettingAsync<T>(string key)
        {
            if (ApplicationData.Current.LocalSettings.Values[key] == null)
            {
                return default;
            }

            return await Task.FromResult((T)ApplicationData.Current.LocalSettings.Values[key]);
        }
    }
}
