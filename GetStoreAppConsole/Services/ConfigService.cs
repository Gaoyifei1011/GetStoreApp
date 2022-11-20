using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace GetStoreAppConsole.Services
{
    public static class ConfigService
    {
        // 设置选项对应的键值
        public static Dictionary<string, string> ConfigKey = new Dictionary<string, string>()
        {
            {"LanguageKey","AppLanguage" },
        };

        public static async Task<T> ReadSettingAsync<T>(string key)
        {
            if (ApplicationData.Current.LocalSettings.Values[key] == null)
            {
                return default;
            }

            return await Task.FromResult((T)ApplicationData.Current.LocalSettings.Values[key]);
        }
    }
}
