using GetStoreApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.App
{
    public interface IResourceService
    {
        List<ThemeModel> ThemeList { get; }

        List<BackdropModel> BackdropList { get; }

        List<HistoryLiteNumModel> HistoryLiteNumList { get; }

        List<GetAppTypeModel> TypeList { get; }

        List<GetAppChannelModel> ChannelList { get; }

        List<StatusBarStateModel> StatusBarStateList { get; }

        Task InitializeResourceAsync(LanguageModel defaultAppLanguage, LanguageModel currentAppLanguage);

        string GetLocalized(string resource);
    }
}
