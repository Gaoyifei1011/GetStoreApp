using GetStoreApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.Root
{
    public interface IResourceService
    {
        List<GetAppTypeModel> TypeList { get; }

        List<GetAppChannelModel> ChannelList { get; }

        List<StatusBarStateModel> StatusBarStateList { get; }

        List<BackdropModel> BackdropList { get; }

        List<DownloadModeModel> DownloadModeList { get; }

        List<HistoryLiteNumModel> HistoryLiteNumList { get; }

        List<ThemeModel> ThemeList { get; }

        Task InitializeResourceAsync(LanguageModel defaultAppLanguage, LanguageModel currentAppLanguage);

        string GetLocalized(string resource);
    }
}
