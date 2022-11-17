using GetStoreApp.Contracts.Controls.Settings.Common;
using GetStoreAppCore.Settings;
using System;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Controls.Settings.Common
{
    /// <summary>
    /// 链接过滤设置服务
    /// </summary>
    public class LinkFilterService : ILinkFilterService
    {
        private string StartWithESettingsKey { get; init; } = ConfigStorage.ConfigKey["StartsWithEFilterKey"];

        private string BlockMapSettingsKey { get; init; } = ConfigStorage.ConfigKey["BlockMapFilterKey"];

        private bool DefaultLinkFilterValue => true;

        public bool StartWithEFilterValue { get; set; }

        public bool BlockMapFilterValue { get; set; }

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
            bool? startWithEFilterValue = await ConfigStorage.ReadSettingAsync<bool?>(StartWithESettingsKey);

            if (!startWithEFilterValue.HasValue)
            {
                return DefaultLinkFilterValue;
            }

            return Convert.ToBoolean(startWithEFilterValue);
        }

        /// <summary>
        /// 获取设置存储的以".blockmap"的文件扩展名的过滤值，如果设置没有存储，使用默认值
        /// </summary>
        private async Task<bool> GetBlockMapFilterValueAsync()
        {
            bool? blockMapFilterValue = await ConfigStorage.ReadSettingAsync<bool?>(BlockMapSettingsKey);

            if (!blockMapFilterValue.HasValue)
            {
                return DefaultLinkFilterValue;
            }

            return Convert.ToBoolean(blockMapFilterValue);
        }

        /// <summary>
        /// 以".e"开头的文件扩展名的过滤值发生修改时修改设置存储的以".e"开头的文件扩展名的过滤值
        /// </summary>
        public async Task SetStartsWithEFilterValueAsync(bool startWithEFilterValue)
        {
            StartWithEFilterValue = startWithEFilterValue;

            await ConfigStorage.SaveSettingAsync(StartWithESettingsKey, startWithEFilterValue);
        }

        /// <summary>
        /// 以".blockmap"的文件扩展名的过滤值发生修改时修改设置存储的以".blockmap"的文件扩展名的过滤值
        /// </summary>
        public async Task SetBlockMapFilterValueAsync(bool blockMapFilterValue)
        {
            BlockMapFilterValue = blockMapFilterValue;

            await ConfigStorage.SaveSettingAsync(BlockMapSettingsKey, blockMapFilterValue);
        }
    }
}
