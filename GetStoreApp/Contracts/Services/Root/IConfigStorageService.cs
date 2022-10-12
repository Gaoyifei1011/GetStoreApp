using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Root
{
    public interface IConfigStorageService
    {
        Task<T> ReadSettingAsync<T>(string key);

        Task SaveSettingAsync<T>(string key, T value);
    }
}
