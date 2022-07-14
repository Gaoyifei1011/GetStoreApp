using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.App
{
    public interface IConfigService
    {
        Task<bool?> GetSettingBoolValueAsync(string key);

        Task<int?> GetSettingIntValueAsync(string key);

        Task<string> GetSettingStringValueAsync(string key);

        Task SaveSettingBoolValueAsync(string key, bool value);

        Task SaveSettingIntValueAsync(string key, int value);

        Task SaveSettingStringValueAsync(string key, string value);
    }
}
