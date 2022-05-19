using System;

using Windows.Storage;

namespace GetStoreApp.Services.Settings
{
    /// <summary>
    /// 修改主页面历史记录显示的条目数量
    /// Modify the number of entries displayed in the main page history
    /// </summary>
    public static class SimpleHistoryItemSelectorService
    {
        private const string SettingsKey = "SimpleHistoryItemValue";

        // 默认设置显示较少的历史记录条目数量:3条
        private static readonly int DefaultHisItemValue = 3;

        public static int SimpleHisItemValue;

        static SimpleHistoryItemSelectorService()
        {
            SimpleHisItemValue = GetSimpleHisItem();
        }

        /// <summary>
        /// 应用初始化时，系统关于该键值存储的信息为空，所以需要判断系统存储的键值是否为空
        /// When the application is initialized, the system information about the key value storage is empty, so you need to determine whether the system stored key value is empty
        /// </summary>
        /// <returns>键值存储的信息是否为空</returns>
        private static bool IsSettingsKeyNullOrEmpty()
        {
            return ApplicationData.Current.LocalSettings.Values[SettingsKey] == null;
        }

        /// <summary>
        /// 设置默认值
        /// Sets the default value
        /// </summary>
        private static void InitializeSettingsKey()
        {
            ApplicationData.Current.LocalSettings.Values[SettingsKey] = DefaultHisItemValue;
        }

        /// <summary>
        /// 获取设置存储的主页面历史记录显示的条目数量值
        /// Gets the value of the number of entries displayed by the main page history displayed by the settings store
        /// </summary>
        /// <returns>主页面历史记录显示的条目数量值</returns>
        private static int GetSimpleHisItem()
        {
            if (IsSettingsKeyNullOrEmpty())
            {
                InitializeSettingsKey();
            }

            return Convert.ToInt32(ApplicationData.Current.LocalSettings.Values[SettingsKey]);
        }

        /// <summary>
        /// 设置主页面历史记录显示的条目数量值
        /// Sets the number of entries displayed in the main page history value
        /// </summary>
        /// <param name="SimpleHisItemValue">主页面历史记录显示的条目数量值</param>
        public static void SetSimpleHisItem(int simpleHisItemValue)
        {
            SimpleHisItemValue = simpleHisItemValue;

            ApplicationData.Current.LocalSettings.Values[SettingsKey] = simpleHisItemValue;
        }
    }
}
