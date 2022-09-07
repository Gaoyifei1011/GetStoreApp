using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Helpers;
using GetStoreApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Settings
{
    /// <summary>
    /// 应用历史记录显示数量设置服务
    /// </summary>
    public class HistoryLiteNumService : IHistoryLiteNumService
    {
        private IConfigStorageService ConfigStorageService { get; } = IOCHelper.GetService<IConfigStorageService>();

        private string SettingsKey { get; init; } = "HistoryLiteNum";

        private IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        private HistoryLiteNumModel DefaultHistoryLiteNum { get; set; }

        public HistoryLiteNumModel HistoryLiteNum { get; set; }

        public List<HistoryLiteNumModel> HistoryLiteNumList { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的历史记录显示数量值
        /// </summary>
        public async Task InitializeHistoryLiteNumAsync()
        {
            HistoryLiteNumList = ResourceService.HistoryLiteNumList;

            DefaultHistoryLiteNum = HistoryLiteNumList.Find(item => item.HistoryLiteNumValue == 3);

            HistoryLiteNum = await GetHistoryLiteNumAsync();
        }

        /// <summary>
        /// 获取设置存储的主题值，如果设置没有存储，使用默认值
        /// </summary>
        private async Task<HistoryLiteNumModel> GetHistoryLiteNumAsync()
        {
            int? historyLiteNumValue = await ConfigStorageService.GetSettingIntValueAsync(SettingsKey);

            if (!historyLiteNumValue.HasValue)
            {
                return HistoryLiteNumList.Find(item => item.HistoryLiteNumValue == DefaultHistoryLiteNum.HistoryLiteNumValue);
            }

            return HistoryLiteNumList.Find(item => item.HistoryLiteNumValue == historyLiteNumValue);
        }

        /// <summary>
        /// 历史记录显示数量发生修改时修改设置存储的历史记录显示数量值
        /// </summary>
        public async Task SetHistoryLiteNumAsync(HistoryLiteNumModel historyLiteNum)
        {
            HistoryLiteNum = historyLiteNum;

            await ConfigStorageService.SaveSettingIntValueAsync(SettingsKey, historyLiteNum.HistoryLiteNumValue);
        }
    }
}
