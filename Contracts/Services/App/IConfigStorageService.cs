using System.Threading.Tasks;
using Windows.Storage;

namespace GetStoreApp.Contracts.Services.App
{
    public interface IConfigStorageService
    {
        Task<bool?> GetSettingBoolValueAsync(string key);

        Task<int?> GetSettingIntValueAsync(string key);

        Task<string> GetSettingStringValueAsync(string key);

        Task<StorageFolder> GetSettingStorageFolderValueAsync(string key);

        Task SaveSettingBoolValueAsync(string key, bool value);

        Task SaveSettingIntValueAsync(string key, int value);

        Task SaveSettingStringValueAsync(string key, string value);

        Task SaveSettingStorageFolderValueAsync(string key, StorageFolder value);
    }
}
