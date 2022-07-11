using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Helpers;
using GetStoreApp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Settings
{
    public class RegionService : IRegionService
    {
        private readonly IConfigService _configService;

        private const string SettingsKey = "AppRegion";

        private string DefaultAppRegion { get; }

        public string AppRegion { get; set; }

        public List<RegionModel> RegionList { get; set; } = GeographicalLocationHelper.GetGeographicalLocations().OrderBy(item => item.FriendlyName).ToList();

        public RegionService(IConfigService configService)
        {
            _configService = configService;

            DefaultAppRegion = RegionList.Find(item => item.ISO2 == RegionInfo.CurrentRegion.TwoLetterISORegionName).ISO2;
        }

        /// <summary>
        /// 应用在初始化前获取设置存储的区域值
        /// </summary>
        public async Task InitializeRegionAsync()
        {
            AppRegion = await GetRegionAsync();
        }

        /// <summary>
        /// 获取设置存储的区域值，如果设置没有存储，使用默认值
        /// </summary>
        private async Task<string> GetRegionAsync()
        {
            string region = await _configService.GetSettingStringValueAsync(SettingsKey);

            if (string.IsNullOrEmpty(region))
            {
                return RegionList.Find(item => item.ISO2.Equals(DefaultAppRegion, StringComparison.OrdinalIgnoreCase)).ISO2;
            }

            return RegionList.Find(item => item.ISO2.Equals(region, StringComparison.OrdinalIgnoreCase)).ISO2;
        }

        /// <summary>
        /// 应用区域发生修改时修改设置存储的主题值
        /// </summary>
        public async Task SetRegionAsync(string region)
        {
            AppRegion = region;

            await _configService.SaveSettingStringValueAsync(SettingsKey, region);
        }
    }
}
