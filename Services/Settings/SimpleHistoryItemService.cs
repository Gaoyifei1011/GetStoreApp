using System;
using Windows.Storage;

namespace GetStoreApp.Services.Settings
{
    public static class HistoryItemValueService
    {
        private const string SettingsKey = "HistoryItemNum";

        private static readonly int DefaultHistoryItemValue = 3;

        /// <summary>
        /// 主页面历史记录显示的条目数量
        /// </summary>
        public static int HistoryItemValue;

        static HistoryItemValueService()
        {
            // 从设置存储中加载当前主页面历史记录显示的条目数量值
            HistoryItemValue = GetHistoryItemValue();
        }

        /// <summary>
        /// 应用初始化时，系统关于该键值存储的信息为空，所以需要判断系统存储的键值是否为空
        /// </summary>
        private static bool IsSettingsKeyNullOrEmpty()
        {
            return ApplicationData.Current.LocalSettings.Values[SettingsKey] == null;
        }

        /// <summary>
        /// 设置默认值
        /// </summary>
        private static void InitializeSettingsKey()
        {
            ApplicationData.Current.LocalSettings.Values[SettingsKey] = DefaultHistoryItemValue;
        }

        /// <summary>
        /// 获取设置存储的主页面历史记录显示的条目数量值
        /// </summary>
        private static int GetHistoryItemValue()
        {
            if (IsSettingsKeyNullOrEmpty())
            {
                InitializeSettingsKey();
            }

            return Convert.ToInt32(ApplicationData.Current.LocalSettings.Values[SettingsKey]);
        }

        /// <summary>
        /// 修改设置
        /// Sets the number of entries displayed in the main page history value
        /// </summary>
        public static void SetHistoryItemValue(int historyItemValue)
        {
            HistoryItemValue = historyItemValue;

            ApplicationData.Current.LocalSettings.Values[SettingsKey] = historyItemValue;
        }
    }
}
