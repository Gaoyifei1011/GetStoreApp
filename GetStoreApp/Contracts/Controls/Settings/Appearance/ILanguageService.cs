using GetStoreApp.Models.Controls.Settings.Appearance;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Controls.Settings.Appearance
{
    public interface ILanguageService
    {
        LanguageModel DefaultAppLanguage { get; }

        LanguageModel AppLanguage { get; set; }

        List<LanguageModel> LanguageList { get; set; }

        Task InitializeLanguageAsync();

        Task SetLanguageAsync(LanguageModel language);
    }
}
