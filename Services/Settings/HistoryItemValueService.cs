using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Settings
{
    public class HistoryItemValueService : IHistoryItemValueService
    {
        private readonly IConfigService ConfigService;
        private readonly IResourceService ResourceService;

        private const string SettingsKey = "HistoryItemValue";

        private int DefaultHistoryItemValue { get; } = 3;

        public int HistoryItemValue { get; set; }

        public List<HistoryItemValueModel> HistoryItemValueList { get; set; }

        public HistoryItemValueService(IConfigService configService, IResourceService resourceService)
        {
            ConfigService = configService;
            ResourceService = resourceService;

            HistoryItemValueList = ResourceService.HistoryItemValueList;
        }

        /// <summary>
        /// 应用在初始化前获取设置存储的历史记录显示数量值
        /// </summary>
        public async Task InitializeHistoryItemValueAsync()
        {
            HistoryItemValue = await GetHistoryItemValueAsync();
        }

        /// <summary>
        /// 获取设置存储的主题值，如果设置没有存储，使用默认值
        /// </summary>
        private async Task<int> GetHistoryItemValueAsync()
        {
            int? historyItemValue = await ConfigService.GetSettingIntValueAsync(SettingsKey);

            if (!historyItemValue.HasValue)
            {
                return HistoryItemValueList.Find(item => item.HistoryItemValue == DefaultHistoryItemValue).HistoryItemValue;
            }

            return HistoryItemValueList.Find(item => item.HistoryItemValue == historyItemValue).HistoryItemValue;
        }

        /// <summary>
        /// 历史记录显示数量发生修改时修改设置存储的历史记录显示数量值
        /// </summary>
        public async Task SetHistoryItemValueAsync(int historyItemValue)
        {
            HistoryItemValue = historyItemValue;

            await ConfigService.SaveSettingIntValueAsync(SettingsKey, historyItemValue);
        }
    }
}
