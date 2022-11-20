using GetStoreApp.Services.Root;
using System;
using System.Threading.Tasks;

namespace GetStoreApp.Services.Controls.Settings.Common
{
    /// <summary>
    /// 链接过滤设置服务
    /// </summary>
    public static class LinkFilterService
    {
        private static string StartWithESettingsKey { get; } = ConfigService.ConfigKey["StartsWithEFilterKey"];

        private static string BlockMapSettingsKey { get; } = ConfigService.ConfigKey["BlockMapFilterKey"];

        private static bool DefaultLinkFilterValue => true;

        public static bool StartWithEFilterValue { get; set; }

        public static bool BlockMapFilterValue { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的链接过滤值
        /// </summary>
        public static async Task InitializeLinkFilterValueAsnyc()
        {
            StartWithEFilterValue = await GetStartWithEFilterValueAsync();

            BlockMapFilterValue = await GetBlockMapFilterValueAsync();
        }

        /// <summary>
        /// 获取设置存储的以".e"开头的文件扩展名的过滤值，如果设置没有存储，使用默认值
        /// </summary>
        private static async Task<bool> GetStartWithEFilterValueAsync()
        {
            bool? startWithEFilterValue = await ConfigService.ReadSettingAsync<bool?>(StartWithESettingsKey);

            if (!startWithEFilterValue.HasValue)
            {
                return DefaultLinkFilterValue;
            }

            return Convert.ToBoolean(startWithEFilterValue);
        }

        /// <summary>
        /// 获取设置存储的以".blockmap"的文件扩展名的过滤值，如果设置没有存储，使用默认值
        /// </summary>
        private static async Task<bool> GetBlockMapFilterValueAsync()
        {
            bool? blockMapFilterValue = await ConfigService.ReadSettingAsync<bool?>(BlockMapSettingsKey);

            if (!blockMapFilterValue.HasValue)
            {
                return DefaultLinkFilterValue;
            }

            return Convert.ToBoolean(blockMapFilterValue);
        }

        /// <summary>
        /// 以".e"开头的文件扩展名的过滤值发生修改时修改设置存储的以".e"开头的文件扩展名的过滤值
        /// </summary>
        public static async Task SetStartsWithEFilterValueAsync(bool startWithEFilterValue)
        {
            StartWithEFilterValue = startWithEFilterValue;

            await ConfigService.SaveSettingAsync(StartWithESettingsKey, startWithEFilterValue);
        }

        /// <summary>
        /// 以".blockmap"的文件扩展名的过滤值发生修改时修改设置存储的以".blockmap"的文件扩展名的过滤值
        /// </summary>
        public static async Task SetBlockMapFilterValueAsync(bool blockMapFilterValue)
        {
            BlockMapFilterValue = blockMapFilterValue;

            await ConfigService.SaveSettingAsync(BlockMapSettingsKey, blockMapFilterValue);
        }
    }
}
