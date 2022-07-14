using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Contracts.Services.Settings;
using System;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Settings
{
    /// <summary>
    /// 链接过滤设置服务
    /// </summary>
    public class LinkFilterService : ILinkFilterService
    {
        private readonly IConfigStorageService ConfigStorageService;

        private const string StartWithESettingsKey = "StartsWithEFilterValue";

        private const string BlockMapSettingsKey = "BlockMapFilterValue";

        private bool DefaultLinkFilterValue { get; } = true;

        public bool StartWithEFilterValue { get; set; }

        public bool BlockMapFilterValue { get; set; }

        public LinkFilterService(IConfigStorageService configStorageService)
        {
            ConfigStorageService = configStorageService;
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
            bool? startWithEFilterValue = await ConfigStorageService.GetSettingBoolValueAsync(StartWithESettingsKey);

            if (!startWithEFilterValue.HasValue) return DefaultLinkFilterValue;

            return Convert.ToBoolean(startWithEFilterValue);
        }

        /// <summary>
        /// 获取设置存储的以".blockmap"的文件扩展名的过滤值，如果设置没有存储，使用默认值
        /// </summary>
        private async Task<bool> GetBlockMapFilterValueAsync()
        {
            bool? blockMapFilterValue = await ConfigStorageService.GetSettingBoolValueAsync(BlockMapSettingsKey);

            if (!blockMapFilterValue.HasValue) return DefaultLinkFilterValue;

            return Convert.ToBoolean(blockMapFilterValue);
        }

        /// <summary>
        /// 以".e"开头的文件扩展名的过滤值发生修改时修改设置存储的以".e"开头的文件扩展名的过滤值
        /// </summary>
        public async Task SetStartsWithEFilterValueAsync(bool startWithEFilterValue)
        {
            StartWithEFilterValue = startWithEFilterValue;

            await ConfigStorageService.SaveSettingBoolValueAsync(StartWithESettingsKey, startWithEFilterValue);
        }

        /// <summary>
        /// 以".blockmap"的文件扩展名的过滤值发生修改时修改设置存储的以".blockmap"的文件扩展名的过滤值
        /// </summary>
        public async Task SetBlockMapFilterValueAsync(bool blockMapFilterValue)
        {
            BlockMapFilterValue = blockMapFilterValue;

            await ConfigStorageService.SaveSettingBoolValueAsync(BlockMapSettingsKey, blockMapFilterValue);
        }
    }
}
