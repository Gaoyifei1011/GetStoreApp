using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;
using System;

namespace GetStoreApp.Services.Settings
{
    /// <summary>
    /// 链接过滤设置服务
    /// </summary>
    public static class LinkFilterService
    {
        private static readonly string encryptedPackageSettingsKey = ConfigKey.EncryptedPackageFilterKey;
        private static readonly string blockMapSettingsKey = ConfigKey.BlockMapFilterKey;
        private static readonly bool defaultLinkFilter = true;

        public static bool EncryptedPackageFilter { get; private set; }

        public static bool BlockMapFilter { get; private set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的链接过滤值
        /// </summary>
        public static void InitializeLinkFilter()
        {
            EncryptedPackageFilter = GetEncryptedPackageFilter();
            BlockMapFilter = GetBlockMapFilter();
        }

        /// <summary>
        /// 获取设置存储的加密包显示设置的过滤值，如果设置没有存储，使用默认值
        /// </summary>
        private static bool GetEncryptedPackageFilter()
        {
            bool? encryptedPackageFilter = LocalSettingsService.ReadSetting<bool?>(encryptedPackageSettingsKey);

            if (!encryptedPackageFilter.HasValue)
            {
                SetEncryptedPackageFilter(defaultLinkFilter);
                return defaultLinkFilter;
            }

            return Convert.ToBoolean(encryptedPackageFilter);
        }

        /// <summary>
        /// 获取设置存储的以".blockmap"的文件扩展名的过滤值，如果设置没有存储，使用默认值
        /// </summary>
        private static bool GetBlockMapFilter()
        {
            bool? blockMapFilter = LocalSettingsService.ReadSetting<bool?>(blockMapSettingsKey);

            if (!blockMapFilter.HasValue)
            {
                SetBlockMapFilter(defaultLinkFilter);
                return defaultLinkFilter;
            }

            return Convert.ToBoolean(blockMapFilter);
        }

        /// <summary>
        /// 加密包显示设置的过滤值发生修改时修改设置存储的加密包显示设置的过滤值
        /// </summary>
        public static void SetEncryptedPackageFilter(bool encryptedPackageFilter)
        {
            EncryptedPackageFilter = encryptedPackageFilter;
            LocalSettingsService.SaveSetting(encryptedPackageSettingsKey, encryptedPackageFilter);
        }

        /// <summary>
        /// 以".blockmap"的文件扩展名的过滤值发生修改时修改设置存储的以".blockmap"的文件扩展名的过滤值
        /// </summary>
        public static void SetBlockMapFilter(bool blockMapFilter)
        {
            BlockMapFilter = blockMapFilter;
            LocalSettingsService.SaveSetting(blockMapSettingsKey, blockMapFilter);
        }
    }
}
