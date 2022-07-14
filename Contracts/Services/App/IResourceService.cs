using GetStoreApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Services.App
{
    public interface IResourceService
    {
        List<ThemeModel> ThemeList { get; set; }

        List<BackdropModel> BackdropList { get; set; }

        List<HistoryItemValueModel> HistoryItemValueList { get; set; }

        List<GetAppTypeModel> TypeList { get; set; }

        List<GetAppChannelModel> ChannelList { get; set; }

        List<StatusBarStateModel> StatusBarStateList { get; set; }

        Task InitializeResourceAsync(string defaultAppLanguage, string currentAppLanguage);

        string GetLocalized(string resource);
    }
}
