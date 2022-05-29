using System;
using System.Collections.Generic;
using Windows.Storage;

namespace GetStoreApp.Services.Settings
{
    /// <summary>
    /// 文件过滤设置服务
    /// File filtering settings service
    /// </summary>
    public static class LinkFilterService
    {
        /// <summary>
        /// 设置存储时需要使用到的键值
        /// The key value that you need to use when setting the store
        /// </summary>
        private static readonly IReadOnlyList<string> SettingsKey = new List<string> { "StartsWithEFilterValue", "BlockMapFilterValue" };

        /// <summary>
        /// 设置按钮的默认状态：全部不过滤
        /// Sets the default state of the button: Display
        /// </summary>
        private static List<bool> DefaultValue = new List<bool> { false, false };

        public static List<bool> LinkFilterValue;

        static LinkFilterService()
        {
            LinkFilterValue = new List<bool>();

            // 从设置存储中加载按钮的设定的值
            LinkFilterValue.Add(GetStartsWithEFilterValue());

            LinkFilterValue.Add(GetBlockMapFilterValue());
        }

        /// <summary>
        /// 应用初始化时，系统关于该键值存储的信息为空，所以需要判断系统存储的键值是否为空
        /// When the application is initialized, the system information about the key value storage is empty, so you need to determine whether the system stored key value is empty
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
        /// Sets the default value
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
        /// 获取当前设置存储的以“.e”开头文件过滤状态布尔值
        /// Gets the Usage Instructions display status Boolean value for the current settings store
        /// </summary>
        /// <returns>显示状态布尔值</returns>
        private static bool GetStartsWithEFilterValue()
        {
            if (IsStartsWithEFilterValueNullOrEmpty())
            {
                InitializeStartsWithEFilterValue();
            }
            return Convert.ToBoolean(ApplicationData.Current.LocalSettings.Values[SettingsKey[0]]);
        }

        private static bool GetBlockMapFilterValue()
        {
            if (IsBlockMapFilterValueNullOrEmpty())
            {
                InitializeBlockMapFilterValue();
            }
            return Convert.ToBoolean(ApplicationData.Current.LocalSettings.Values[SettingsKey[1]]);
        }

        /// <summary>
        /// 设置主页面“使用说明”的显示状态布尔值
        /// Sets the Boolean display state value for the main page Instructions for Use
        /// </summary>
        /// <param name="useInsVisValue">显示状态布尔值</param>
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
