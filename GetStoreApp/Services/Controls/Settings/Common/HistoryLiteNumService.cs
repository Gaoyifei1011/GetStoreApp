using GetStoreApp.Models.Controls.Settings.Common;
using GetStoreApp.Services.Root;
using GetStoreAppCore.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Controls.Settings.Common
{
    /// <summary>
    /// 应用历史记录显示数量设置服务
    /// </summary>
    public static class HistoryLiteNumService
    {
        private static string SettingsKey { get; } = ConfigStorage.ConfigKey["HistoryLiteNumKey"];

        private static HistoryLiteNumModel DefaultHistoryLiteNum { get; set; }

        public static HistoryLiteNumModel HistoryLiteNum { get; set; }

        public static List<HistoryLiteNumModel> HistoryLiteNumList { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的历史记录显示数量值
        /// </summary>
        public static async Task InitializeHistoryLiteNumAsync()
        {
            HistoryLiteNumList = ResourceService.HistoryLiteNumList;

            DefaultHistoryLiteNum = HistoryLiteNumList.Find(item => item.HistoryLiteNumValue == 3);

            HistoryLiteNum = await GetHistoryLiteNumAsync();
        }

        /// <summary>
        /// 获取设置存储的主题值，如果设置没有存储，使用默认值
        /// </summary>
        private static async Task<HistoryLiteNumModel> GetHistoryLiteNumAsync()
        {
            int? historyLiteNumValue = await ConfigStorage.ReadSettingAsync<int?>(SettingsKey);

            if (!historyLiteNumValue.HasValue)
            {
                return HistoryLiteNumList.Find(item => item.HistoryLiteNumValue == DefaultHistoryLiteNum.HistoryLiteNumValue);
            }

            return HistoryLiteNumList.Find(item => item.HistoryLiteNumValue == historyLiteNumValue);
        }

        /// <summary>
        /// 历史记录显示数量发生修改时修改设置存储的历史记录显示数量值
        /// </summary>
        public static async Task SetHistoryLiteNumAsync(HistoryLiteNumModel historyLiteNum)
        {
            HistoryLiteNum = historyLiteNum;

            await ConfigStorage.SaveSettingAsync(SettingsKey, historyLiteNum.HistoryLiteNumValue);
        }
    }
}
