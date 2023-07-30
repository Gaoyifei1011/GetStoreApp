using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;
using System;

namespace GetStoreApp.Services.Controls.Settings.Common
{
    /// <summary>
    /// 链接过滤设置服务
    /// </summary>
    public static class LinkFilterService
    {
        private static string StartWithESettingsKey { get; } = ConfigKey.StartsWithEFilterKey;

        private static string BlockMapSettingsKey { get; } = ConfigKey.BlockMapFilterKey;

        private static bool DefaultLinkFilterValue { get; } = true;

        public static bool StartWithEFilterValue { get; set; }

        public static bool BlockMapFilterValue { get; set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的链接过滤值
        /// </summary>
        public static void InitializeLinkFilterValue()
        {
            StartWithEFilterValue = GetStartWithEFilterValue();

            BlockMapFilterValue = GetBlockMapFilterValue();
        }

        /// <summary>
        /// 获取设置存储的以".e"开头的文件扩展名的过滤值，如果设置没有存储，使用默认值
        /// </summary>
        private static bool GetStartWithEFilterValue()
        {
            bool? startWithEFilterValue = ConfigService.ReadSetting<bool?>(StartWithESettingsKey);

            if (!startWithEFilterValue.HasValue)
            {
                return DefaultLinkFilterValue;
            }

            return Convert.ToBoolean(startWithEFilterValue);
        }

        /// <summary>
        /// 获取设置存储的以".blockmap"的文件扩展名的过滤值，如果设置没有存储，使用默认值
        /// </summary>
        private static bool GetBlockMapFilterValue()
        {
            bool? blockMapFilterValue = ConfigService.ReadSetting<bool?>(BlockMapSettingsKey);

            if (!blockMapFilterValue.HasValue)
            {
                return DefaultLinkFilterValue;
            }

            return Convert.ToBoolean(blockMapFilterValue);
        }

        /// <summary>
        /// 以".e"开头的文件扩展名的过滤值发生修改时修改设置存储的以".e"开头的文件扩展名的过滤值
        /// </summary>
        public static void SetStartsWithEFilterValue(bool startWithEFilterValue)
        {
            StartWithEFilterValue = startWithEFilterValue;

            ConfigService.SaveSetting(StartWithESettingsKey, startWithEFilterValue);
        }

        /// <summary>
        /// 以".blockmap"的文件扩展名的过滤值发生修改时修改设置存储的以".blockmap"的文件扩展名的过滤值
        /// </summary>
        public static void SetBlockMapFilterValue(bool blockMapFilterValue)
        {
            BlockMapFilterValue = blockMapFilterValue;

            ConfigService.SaveSetting(BlockMapSettingsKey, blockMapFilterValue);
        }
    }
}
