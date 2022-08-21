using GetStoreApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Settings
{
    public interface ILanguageService
    {
        LanguageModel DefaultAppLanguage { get; }

        LanguageModel AppLanguage { get; set; }

        List<LanguageModel> LanguageList { get; set; }

        Task InitializeLanguageAsync();

        Task SetLanguageAsync(LanguageModel language);

        Task SetAppLanguageAsync();
    }
}
