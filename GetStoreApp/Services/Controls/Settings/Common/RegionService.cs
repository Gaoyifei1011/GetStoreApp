using GetStoreApp.Contracts.Controls.Settings.Common;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Settings.Common;
using GetStoreAppCore.Settings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Controls.Settings.Common
{
    /// <summary>
    /// 区域设置服务
    /// </summary>
    public class RegionService : IRegionService
    {
        private string SettingsKey { get; init; } = ConfigStorage.ConfigKey["RegionKey"];

        public List<RegionModel> RegionList => GeographicalLocationHelper.GetGeographicalLocations().OrderBy(item => item.FriendlyName).ToList();

        private RegionModel DefaultAppRegion { get; set; }

        public RegionModel AppRegion { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的区域值
        /// </summary>
        public async Task InitializeRegionAsync()
        {
            DefaultAppRegion = RegionList.Find(item => item.ISO2 == RegionInfo.CurrentRegion.TwoLetterISORegionName);

            AppRegion = await GetRegionAsync();
        }

        /// <summary>
        /// 获取设置存储的区域值，如果设置没有存储，使用默认值
        /// </summary>
        private async Task<RegionModel> GetRegionAsync()
        {
            string region = await ConfigStorage.ReadSettingAsync<string>(SettingsKey);

            if (string.IsNullOrEmpty(region))
            {
                return RegionList.Find(item => item.ISO2.Equals(DefaultAppRegion.ISO2, StringComparison.OrdinalIgnoreCase));
            }

            return RegionList.Find(item => item.ISO2.Equals(region, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 应用区域发生修改时修改设置存储的主题值
        /// </summary>
        public async Task SetRegionAsync(RegionModel region)
        {
            AppRegion = region;

            await ConfigStorage.SaveSettingAsync(SettingsKey, region.ISO2);
        }
    }
}
