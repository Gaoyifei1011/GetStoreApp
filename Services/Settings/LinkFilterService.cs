using System;
using System.Collections.Generic;
using Windows.Storage;

namespace GetStoreApp.Services.Settings
{
    public static class LinkFilterService
    {
        private static readonly IReadOnlyList<string> SettingsKey = new List<string> { "StartsWithEFilterValue", "BlockMapFilterValue" };

        private static List<bool> DefaultValue { get; } = new List<bool> { true, true };

        public static List<bool> LinkFilterValue { get; set; }

        static LinkFilterService()
        {
            LinkFilterValue = new List<bool>();

            LinkFilterValue.Add(GetStartsWithEFilterValue());
            LinkFilterValue.Add(GetBlockMapFilterValue());
        }

        /// <summary>
        /// 应用初始化时，系统关于该键值存储的信息为空，所以需要判断系统存储的键值是否为空
        /// </summary>
        private static bool IsStartsWithEFilterValueNullOrEmpty()
        {
            return ApplicationData.Current.LocalSettings.Values[SettingsKey[0]] == null;
        }

        private static bool IsBlockMapFilterValueNullOrEmpty()
        {
            return ApplicationData.Current.LocalSettings.Values[SettingsKey[1]] == null;
        }

        /// <summary>
        /// 设置默认值
        /// </summary>
        private static void InitializeStartsWithEFilterValue()
        {
            ApplicationData.Current.LocalSettings.Values[SettingsKey[0]] = DefaultValue[0];
        }

        private static void InitializeBlockMapFilterValue()
        {
            ApplicationData.Current.LocalSettings.Values[SettingsKey[1]] = DefaultValue[1];
        }

        /// <summary>
        ///  获取当前设置存储的扩展名以“.e”开头的文件过滤状态布尔值
        /// </summary>
        private static bool GetStartsWithEFilterValue()
        {
            if (IsStartsWithEFilterValueNullOrEmpty())
            {
                InitializeStartsWithEFilterValue();
            }
            return Convert.ToBoolean(ApplicationData.Current.LocalSettings.Values[SettingsKey[0]]);
        }

        /// <summary>
        /// 获取当前设置存储的扩展名以“.blockmap”开头的文件过滤状态布尔值
        /// </summary>
        private static bool GetBlockMapFilterValue()
        {
            if (IsBlockMapFilterValueNullOrEmpty())
            {
                InitializeBlockMapFilterValue();
            }
            return Convert.ToBoolean(ApplicationData.Current.LocalSettings.Values[SettingsKey[1]]);
        }

        /// <summary>
        /// 修改设置
        /// </summary>
        /// <param name="startsWithEFilterValue"></param>
        public static void SetStartsWithEFilterValue(bool startsWithEFilterValue)
        {
            LinkFilterValue[0] = startsWithEFilterValue;

            ApplicationData.Current.LocalSettings.Values[SettingsKey[0]] = LinkFilterValue[0];
        }

        public static void SetBlockMapFilterValue(bool blockMapFilterValue)
        {
            LinkFilterValue[1] = blockMapFilterValue;

            ApplicationData.Current.LocalSettings.Values[SettingsKey[1]] = LinkFilterValue[1];
        }
    }
}
