using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GetStoreApp.Services.Settings
{
    /// <summary>
    /// 搜索应用方式选择设置服务
    /// </summary>
    public static class SearchAppsModeService
    {
        private static readonly string searchAppsModeSettingsKey = ConfigKey.SearchAppsModeKey;

        private static string defaultSearchAppsMode;

        private static string _searchAppsMode;

        public static string SearchAppsMode
        {
            get { return _searchAppsMode; }

            private set
            {
                if (!string.Equals(_searchAppsMode, value))
                {
                    _searchAppsMode = value;
                    PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(SearchAppsMode)));
                }
            }
        }

        public static List<string> SearchAppsModeList { get; } = ["OfficialAPI", "OfficialConsoleClient"];

        public static event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 应用在初始化前获取设置存储的搜索应用方式选择值
        /// </summary>
        public static void InitializeSearchAppsMode()
        {
            defaultSearchAppsMode = SearchAppsModeList.Find(item => item is "OfficialAPI");
            SearchAppsMode = GetSearchAppsMode();
        }

        /// <summary>
        /// 获取设置存储的搜索应用方式选择值，如果设置没有存储，使用默认值
        /// </summary>
        private static string GetSearchAppsMode()
        {
            string searchAppsModeValue = LocalSettingsService.ReadSetting<string>(searchAppsModeSettingsKey);

            if (string.IsNullOrEmpty(searchAppsModeValue))
            {
                SetSearchAppsMode(defaultSearchAppsMode);
                return defaultSearchAppsMode;
            }

            string selectedSearchAppsMode = SearchAppsModeList.Find(item => string.Equals(item, searchAppsModeValue, StringComparison.OrdinalIgnoreCase));
            return selectedSearchAppsMode is null ? defaultSearchAppsMode : selectedSearchAppsMode;
        }

        /// <summary>
        /// 搜索应用方式发生修改时修改设置存储的搜索应用方式值
        /// </summary>
        public static void SetSearchAppsMode(string searchAppsMode)
        {
            SearchAppsMode = searchAppsMode;
            LocalSettingsService.SaveSetting(searchAppsModeSettingsKey, searchAppsMode);
        }
    }
}
