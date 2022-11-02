using System.Threading.Tasks;

namespace GetStoreAppConsole.Contracts
{
    public interface IConfigStoreageService
    {
        Task<T> ReadSettingAsync<T>(string key);
    }
}
