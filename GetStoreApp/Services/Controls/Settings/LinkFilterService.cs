using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;
using System;

namespace GetStoreApp.Services.Controls.Settings
{
    /// <summary>
    /// 链接过滤设置服务
    /// </summary>
    public static class LinkFilterService
    {
        private static readonly string encryptedPackageSettingsKey = ConfigKey.EncryptedPackageFilterKey;
        private static readonly string blockMapSettingsKey = ConfigKey.BlockMapFilterKey;
        private static readonly bool defaultLinkFilterValue = true;

        public static bool EncryptedPackageFilterValue { get; private set; }

        public static bool BlockMapFilterValue { get; private set; }

        /// <summary>
        /// 应用在初始化前获取设置存储的链接过滤值
        /// </summary>
        public static void InitializeLinkFilter()
        {
            EncryptedPackageFilterValue = GetEncryptedPackageFilterValue();
            BlockMapFilterValue = GetBlockMapFilterValue();
        }

        /// <summary>
        /// 获取设置存储的加密包显示设置的过滤值，如果设置没有存储，使用默认值
        /// </summary>
        private static bool GetEncryptedPackageFilterValue()
        {
            bool? encryptedPackageFilterValue = LocalSettingsService.ReadSetting<bool?>(encryptedPackageSettingsKey);

            if (!encryptedPackageFilterValue.HasValue)
            {
                SetEncryptedPackageFilterValue(defaultLinkFilterValue);
                return defaultLinkFilterValue;
            }

            return Convert.ToBoolean(encryptedPackageFilterValue);
        }

        /// <summary>
        /// 获取设置存储的以".blockmap"的文件扩展名的过滤值，如果设置没有存储，使用默认值
        /// </summary>
        private static bool GetBlockMapFilterValue()
        {
            bool? blockMapFilterValue = LocalSettingsService.ReadSetting<bool?>(blockMapSettingsKey);

            if (!blockMapFilterValue.HasValue)
            {
                SetBlockMapFilterValue(defaultLinkFilterValue);
                return defaultLinkFilterValue;
            }

            return Convert.ToBoolean(blockMapFilterValue);
        }

        /// <summary>
        /// 加密包显示设置的过滤值发生修改时修改设置存储的加密包显示设置的过滤值
        /// </summary>
        public static void SetEncryptedPackageFilterValue(bool encryptedPackageFilterValue)
        {
            EncryptedPackageFilterValue = encryptedPackageFilterValue;

            LocalSettingsService.SaveSetting(encryptedPackageSettingsKey, encryptedPackageFilterValue);
        }

        /// <summary>
        /// 以".blockmap"的文件扩展名的过滤值发生修改时修改设置存储的以".blockmap"的文件扩展名的过滤值
        /// </summary>
        public static void SetBlockMapFilterValue(bool blockMapFilterValue)
        {
            BlockMapFilterValue = blockMapFilterValue;

            LocalSettingsService.SaveSetting(blockMapSettingsKey, blockMapFilterValue);
        }
    }
}
