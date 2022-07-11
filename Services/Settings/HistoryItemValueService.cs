using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace GetStoreApp.Services.Settings
{
    public class HistoryItemValueService : IHistoryItemValueService
    {
        private readonly IConfigService _configService;

        private const string SettingsKey = "HistoryItemValue";

        private string DefaultHistoryItemValue { get; } = "3";

        public string HistoryItemValue { get; set; }

        public List<HistoryItemValueModel> HistoryItemValueList { get; set; } = new List<HistoryItemValueModel>();

        public HistoryItemValueService(IConfigService configService)
        {
            _configService = configService;

            InitializeHistoryItemValueList();
        }

        /// <summary>
        /// 初始化历史记录显示数量信息列表
        /// </summary>
        private void InitializeHistoryItemValueList()
        {
            HistoryItemValueList.Add(new HistoryItemValueModel
            {
                HistoryItemName = LanguageService.GetResources("/Settings/HistoryItemValueMin"),
                HistoryItemValue = "3"
            });
            HistoryItemValueList.Add(new HistoryItemValueModel
            {
                HistoryItemName = LanguageService.GetResources("/Settings/HistoryItemValueMax"),
                HistoryItemValue = "5"
            });
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
        private async Task<string> GetHistoryItemValueAsync()
        {
            string historyItemValue = await _configService.GetSettingStringValueAsync(SettingsKey);

            if (string.IsNullOrEmpty(historyItemValue))
            {
                return HistoryItemValueList.Find(item => item.HistoryItemValue.Equals(DefaultHistoryItemValue, StringComparison.OrdinalIgnoreCase)).HistoryItemValue;
            }

            return HistoryItemValueList.Find(item => item.HistoryItemValue.Equals(historyItemValue, StringComparison.OrdinalIgnoreCase)).HistoryItemValue;
        }

        /// <summary>
        /// 历史记录显示数量发生修改时修改设置存储的历史记录显示数量值
        /// </summary>
        public async Task SetHistoryItemValueAsync(string historyItemValue)
        {
            HistoryItemValue = historyItemValue;

            await _configService.SaveSettingStringValueAsync(SettingsKey, historyItemValue);
        }
    }
}
