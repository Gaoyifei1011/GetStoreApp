using GetStoreApp.Models.Controls.Home;
using GetStoreApp.Models.Controls.Settings.Advanced;
using GetStoreApp.Models.Controls.Settings.Appearance;
using GetStoreApp.Models.Controls.Settings.Common;
using GetStoreApp.Models.Dialogs.CommonDialogs.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Contracts.Root
{
    public interface IResourceService
    {
        List<TypeModel> TypeList { get; }

        List<ChannelModel> ChannelList { get; }

        List<StatusBarStateModel> StatusBarStateList { get; }

        List<AppExitModel> AppExitList { get; }

        List<BackdropModel> BackdropList { get; }

        List<DownloadModeModel> DownloadModeList { get; }

        List<HistoryLiteNumModel> HistoryLiteNumList { get; }

        List<InstallModeModel> InstallModeList { get; }

        List<ThemeModel> ThemeList { get; }

        List<TraceCleanupModel> TraceCleanupList { get; }

        Task InitializeResourceAsync(LanguageModel defaultAppLanguage, LanguageModel currentAppLanguage);

        string GetLocalized(string resource);
    }
}
