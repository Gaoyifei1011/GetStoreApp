using GetStoreApp.Contracts.Services.App;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace GetStoreApp.Services.App
{
    public class ConfigService : IConfigService
    {
        public async Task<string> GetSettingStringValueAsync(string key)
        {
            if (ApplicationData.Current.LocalSettings.Values[key] == null) return null;

            return await Task.FromResult(Convert.ToString(ApplicationData.Current.LocalSettings.Values[key]));
        }

        public async Task<bool?> GetSettingBoolValueAsync(string key)
        {
            if (ApplicationData.Current.LocalSettings.Values[key] == null) return null;

            return await Task.FromResult(Convert.ToBoolean(ApplicationData.Current.LocalSettings.Values[key]));
        }

        public async Task SaveSettingStringValueAsync(string key, string value)
        {
            ApplicationData.Current.LocalSettings.Values[key] = await Task.FromResult(value);
        }

        public async Task SaveSettingBoolValueAsync(string key, bool value)
        {
            ApplicationData.Current.LocalSettings.Values[key] = await Task.FromResult(value);
        }
    }
}
