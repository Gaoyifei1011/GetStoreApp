using GetStoreApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Settings
{
    public interface ILanguageService
    {
        string DefaultAppLanguage { get; }

        string AppLanguage { get; set; }

        List<LanguageModel> LanguageList { get; set; }

        Task InitializeLanguageAsync();

        Task SetLanguageAsync(string language);

        Task SetAppLanguageAsync();
    }
}
