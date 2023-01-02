using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace GetStoreApp.Services.Root
{
    public static class ConfigService
    {
        // 设置选项对应的键值
        public static Dictionary<string, string> ConfigKey { get; } = new Dictionary<string, string>()
        {
            {"AlwaysShowBackdropKey","AlwaysShowBackdrop" },
            {"BackdropKey","AppBackdrop" },
            {"BlockMapFilterKey","BlockMapFilterValue" },
            {"DownloadFolderKey","DownloadFolder" },
            {"DownloadItemKey","DownloadItemKey" },
            {"DownloadModeKey","DownloadMode" },
            {"ExitKey","AppExit" },
            {"HistoryLiteNumKey","HistoryLiteNum" },
            {"InstallModeKey","InstallMode" },
            {"LanguageKey","AppLanguage" },
            {"NetWorkMonitorKey","NetWorkMonitorValue" },
            {"NotificationKey","AppNotification" },
            {"RegionKey","AppRegion" },
            {"StartsWithEFilterKey","StartsWithEFilterValue" },
            {"ThemeKey","AppTheme" },
            {"TopMostKey","TopMostValue" },
            {"UseInstructionKey","UseInsVisValue" },
        };

        public static async Task<T> ReadSettingAsync<T>(string key)
        {
            if (ApplicationData.Current.LocalSettings.Values[key] == null)
            {
                return default;
            }

            return await Task.FromResult((T)ApplicationData.Current.LocalSettings.Values[key]);
        }

        public static async Task SaveSettingAsync<T>(string key, T value)
        {
            ApplicationData.Current.LocalSettings.Values[key] = await Task.FromResult(value);
        }
    }
}
