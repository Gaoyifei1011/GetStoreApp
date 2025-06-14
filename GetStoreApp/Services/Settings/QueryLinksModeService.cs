using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GetStoreApp.Services.Settings
{
    /// <summary>
    /// 查询链接方式选择设置服务
    /// </summary>
    public static class QueryLinksModeService
    {
        private static readonly string queryLinksModeSettingsKey = ConfigKey.QueryLinksModeKey;

        private static string defaultQueryLinksMode;

        private static string _queryLinksMode;

        public static string QueryLinksMode
        {
            get { return _queryLinksMode; }

            private set
            {
                if (!string.Equals(_queryLinksMode, value))
                {
                    _queryLinksMode = value;
                    PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(QueryLinksMode)));
                }
            }
        }

        public static List<string> QueryLinksModeList { get; } = ["Official", "ThirdParty"];

        public static event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 应用在初始化前获取设置存储的查询链接方式选择值
        /// </summary>
        public static void InitializeQueryLinksMode()
        {
            defaultQueryLinksMode = QueryLinksModeList.Find(item => item is "Official");
            QueryLinksMode = GetQueryLinksMode();
        }

        /// <summary>
        /// 获取设置存储的查询链接方式选择值，如果设置没有存储，使用默认值
        /// </summary>
        private static string GetQueryLinksMode()
        {
            string queryLinksModeValue = LocalSettingsService.ReadSetting<string>(queryLinksModeSettingsKey);

            if (string.IsNullOrEmpty(queryLinksModeValue))
            {
                SetQueryLinksMode(defaultQueryLinksMode);
                return defaultQueryLinksMode;
            }

            string selectedQueryLinksMode = QueryLinksModeList.Find(item => string.Equals(item, queryLinksModeValue, StringComparison.OrdinalIgnoreCase));
            return selectedQueryLinksMode is null ? defaultQueryLinksMode : selectedQueryLinksMode;
        }

        /// <summary>
        /// 查询链接方式发生修改时修改设置存储的查询链接方式值
        /// </summary>
        public static void SetQueryLinksMode(string queryLinksMode)
        {
            QueryLinksMode = queryLinksMode;
            LocalSettingsService.SaveSetting(queryLinksModeSettingsKey, queryLinksMode);
        }
    }
}
