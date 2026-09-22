using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace GetStoreApp.Services.Settings
{
    /// <summary>
    /// 查询链接方式选择设置服务
    /// </summary>
    internal static class QueryLinksModeService
    {
        private static readonly string queryLinksModeSettingsKey = ConfigKey.QueryLinksModeKey;
        private static string defaultQueryLinksMode;

        private static string _queryLinksMode;

        internal static string QueryLinksMode
        {
            get { return _queryLinksMode; }

            private set
            {
                if (!string.Equals(_queryLinksMode, value))
                {
                    _queryLinksMode = value;
                    PropertyChanged?.Invoke(null, new(nameof(QueryLinksMode)));
                }
            }
        }

        internal static ReadOnlyCollection<string> QueryLinksModeCollection { get; } = ["Official", "ThirdParty"];

        internal static event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 应用在初始化前获取设置存储的查询链接方式选择值
        /// </summary>
        internal static void InitializeQueryLinksMode()
        {
            foreach (string queryLinksModeItem in QueryLinksModeCollection)
            {
                if (queryLinksModeItem is "Official")
                {
                    defaultQueryLinksMode = queryLinksModeItem;
                    break;
                }
            }
            QueryLinksMode = GetQueryLinksMode();
        }

        /// <summary>
        /// 获取设置存储的查询链接方式选择值，如果设置没有存储，使用默认值
        /// </summary>
        private static string GetQueryLinksMode()
        {
            string queryLinksMode = LocalSettingsService.ReadSetting<string>(queryLinksModeSettingsKey);

            if (string.IsNullOrEmpty(queryLinksMode))
            {
                SetQueryLinksMode(defaultQueryLinksMode);
                return defaultQueryLinksMode;
            }

            string selectedQueryLinksMode = null;
            foreach (string queryLinksModeItem in QueryLinksModeCollection)
            {
                if (string.Equals(queryLinksModeItem, queryLinksMode, StringComparison.OrdinalIgnoreCase))
                {
                    selectedQueryLinksMode = queryLinksModeItem;
                    break;
                }
            }
            return string.IsNullOrEmpty(selectedQueryLinksMode) ? defaultQueryLinksMode : selectedQueryLinksMode;
        }

        /// <summary>
        /// 查询链接方式发生修改后修改设置存储的查询链接方式值
        /// </summary>
        internal static void SetQueryLinksMode(string queryLinksMode)
        {
            QueryLinksMode = queryLinksMode;
            LocalSettingsService.SaveSetting(queryLinksModeSettingsKey, queryLinksMode);
        }
    }
}
