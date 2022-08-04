using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Settings
{
    /// <summary>
    /// 应用历史记录显示数量设置服务
    /// </summary>
    public class HistoryItemValueService : IHistoryItemValueService
    {
        private string SettingsKey { get; init; } = "HistoryItem";

        private IConfigStorageService ConfigStorageService { get; } = GetStoreApp.App.GetService<IConfigStorageService>();

        private IResourceService ResourceService { get; } = GetStoreApp.App.GetService<IResourceService>();

        private HistoryItemValueModel DefaultHistoryItem { get; set; }

        public HistoryItemValueModel HistoryItem { get; set; }

        public List<HistoryItemValueModel> HistoryItemValueList { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的历史记录显示数量值
        /// </summary>
        public async Task InitializeHistoryItemValueAsync()
        {
            HistoryItemValueList = ResourceService.HistoryItemValueList;

            DefaultHistoryItem = HistoryItemValueList.Find(item => item.HistoryItemValue == 3);

            HistoryItem = await GetHistoryItemValueAsync();
        }

        /// <summary>
        /// 获取设置存储的主题值，如果设置没有存储，使用默认值
        /// </summary>
        private async Task<HistoryItemValueModel> GetHistoryItemValueAsync()
        {
            int? historyItemValue = await ConfigStorageService.GetSettingIntValueAsync(SettingsKey);

            if (!historyItemValue.HasValue)
            {
                return HistoryItemValueList.Find(item => item.HistoryItemValue == DefaultHistoryItem.HistoryItemValue);
            }

            return HistoryItemValueList.Find(item => item.HistoryItemValue == historyItemValue);
        }

        /// <summary>
        /// 历史记录显示数量发生修改时修改设置存储的历史记录显示数量值
        /// </summary>
        public async Task SetHistoryItemValueAsync(HistoryItemValueModel historyItem)
        {
            HistoryItem = historyItem;

            await ConfigStorageService.SaveSettingIntValueAsync(SettingsKey, historyItem.HistoryItemValue);
        }
    }
}
