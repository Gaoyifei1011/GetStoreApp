using GetStoreApp.Contracts.Services.App;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace GetStoreApp.Services.App
{
    /// <summary>
    /// 应用配置存储服务
    /// </summary>
    public class ConfigStorageService : IConfigStorageService
    {
        public async Task<bool?> GetSettingBoolValueAsync(string key)
        {
            if (ApplicationData.Current.LocalSettings.Values[key] == null) return null;

            return await Task.FromResult(Convert.ToBoolean(ApplicationData.Current.LocalSettings.Values[key]));
        }

        public async Task<int?> GetSettingIntValueAsync(string key)
        {
            if (ApplicationData.Current.LocalSettings.Values[key] == null) return null;

            return await Task.FromResult(Convert.ToInt32(ApplicationData.Current.LocalSettings.Values[key]));
        }

        public async Task<string> GetSettingStringValueAsync(string key)
        {
            if (ApplicationData.Current.LocalSettings.Values[key] == null) return null;

            return await Task.FromResult(Convert.ToString(ApplicationData.Current.LocalSettings.Values[key]));
        }

        public async Task<StorageFolder> GetSettingStorageFolderValueAsync(string key)
        {
            if (ApplicationData.Current.LocalSettings.Values[key] == null) return null;

            string FolderPath = Convert.ToString(ApplicationData.Current.LocalSettings.Values[key]);

            if (!Directory.Exists(FolderPath)) return null;

            return await StorageFolder.GetFolderFromPathAsync(FolderPath);
        }

        public async Task SaveSettingBoolValueAsync(string key, bool value)
        {
            ApplicationData.Current.LocalSettings.Values[key] = await Task.FromResult(value);
        }

        public async Task SaveSettingIntValueAsync(string key, int value)
        {
            ApplicationData.Current.LocalSettings.Values[key] = await Task.FromResult(value);
        }

        public async Task SaveSettingStringValueAsync(string key, string value)
        {
            ApplicationData.Current.LocalSettings.Values[key] = await Task.FromResult(value);
        }

        public async Task SaveSettingStorageFolderValueAsync(string key, StorageFolder value)
        {
            ApplicationData.Current.LocalSettings.Values[key] = await Task.FromResult(value.Path);
        }
    }
}
