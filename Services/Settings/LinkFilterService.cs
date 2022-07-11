using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Contracts.Services.Settings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace GetStoreApp.Services.Settings
{
    public class LinkFilterService : ILinkFilterService
    {
        private readonly IConfigService _configService;

        private const string StartWithESettingsKey = "StartsWithEFilterValue";

        private const string BlockMapSettingsKey = "BlockMapFilterValue";

        private bool DefaultLinkFilterValue { get; } = true;

        public bool StartWithEFilterValue { get; set; }

        public bool BlockMapFilterValue { get; set; }

        public LinkFilterService(IConfigService configService)
        {
            _configService = configService;
        }

        /// <summary>
        /// 应用在初始化前获取设置存储的链接过滤值
        /// </summary>
        public async Task InitializeLinkFilterValueAsnyc()
        {
            StartWithEFilterValue = await GetStartWithEFilterValueAsync();

            BlockMapFilterValue = await GetBlockMapFilterValueAsync();
        }

        /// <summary>
        /// 获取设置存储的以".e"开头的文件扩展名的过滤值，如果设置没有存储，使用默认值
        /// </summary>
        private async Task<bool> GetStartWithEFilterValueAsync()
        {
            bool? startWithEFilterValue = await _configService.GetSettingBoolValueAsync(StartWithESettingsKey);

            if (!startWithEFilterValue.HasValue) return DefaultLinkFilterValue;
            
            return Convert.ToBoolean(startWithEFilterValue);
        }

        /// <summary>
        /// 获取设置存储的以".blockmap"的文件扩展名的过滤值，如果设置没有存储，使用默认值
        /// </summary>
        private async Task<bool> GetBlockMapFilterValueAsync()
        {
            bool? blockMapFilterValue = await _configService.GetSettingBoolValueAsync(BlockMapSettingsKey);

            if (!blockMapFilterValue.HasValue) return DefaultLinkFilterValue;

            return Convert.ToBoolean(blockMapFilterValue);
        }

        /// <summary>
        /// 以".e"开头的文件扩展名的过滤值发生修改时修改设置存储的以".e"开头的文件扩展名的过滤值
        /// </summary>
        public async Task SetStartsWithEFilterValueAsync(bool startWithEFilterValue)
        {
            StartWithEFilterValue = startWithEFilterValue;

            await _configService.SaveSettingBoolValueAsync(StartWithESettingsKey,startWithEFilterValue);
        }

        /// <summary>
        /// 以".blockmap"的文件扩展名的过滤值发生修改时修改设置存储的以".blockmap"的文件扩展名的过滤值
        /// </summary>
        public async Task SetBlockMapFilterValueAsync(bool blockMapFilterValue)
        {
            BlockMapFilterValue = blockMapFilterValue;

            await _configService.SaveSettingBoolValueAsync(BlockMapSettingsKey, blockMapFilterValue);
        }
    }
}
