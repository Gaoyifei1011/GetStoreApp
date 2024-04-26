using GetStoreApp.Extensions.DataType.Constant;
using GetStoreApp.Services.Root;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace GetStoreApp.Services.Controls.Settings
{
    /// <summary>
    /// 查询链接方式选择设置服务
    /// </summary>
    public static class QueryLinksModeService
    {
        private static string queryLinksModeSettingsKey = ConfigKey.QueryLinksModeKey;

        private static DictionaryEntry defaultQueryLinksMode;

        private static DictionaryEntry _queryLinksMode;

        public static DictionaryEntry QueryLinksMode
        {
            get { return _queryLinksMode; }

            private set
            {
                if (!Equals(_queryLinksMode, value))
                {
                    _queryLinksMode = value;
                    PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(QueryLinksMode)));
                }
            }
        }

        public static List<DictionaryEntry> QueryLinksModeList { get; private set; }

        public static event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 应用在初始化前获取设置存储的查询链接方式选择值
        /// </summary>
        public static void InitializeQueryLinksMode()
        {
            QueryLinksModeList = ResourceService.QueryLinksModeList;

            defaultQueryLinksMode = QueryLinksModeList.Find(item => item.Value.ToString() is "Official");

            QueryLinksMode = GetQueryLinksMode();
        }

        /// <summary>
        /// 获取设置存储的查询链接方式选择值，如果设置没有存储，使用默认值
        /// </summary>
        private static DictionaryEntry GetQueryLinksMode()
        {
            object queryLinksModeValue = LocalSettingsService.ReadSetting<object>(queryLinksModeSettingsKey);

            if (queryLinksModeValue is null)
            {
                SetQueryLinksMode(defaultQueryLinksMode);
                return defaultQueryLinksMode;
            }

            DictionaryEntry selectedQueryLinksMode = QueryLinksModeList.Find(item => item.Value.Equals(queryLinksModeValue));

            return selectedQueryLinksMode.Key is null ? defaultQueryLinksMode : selectedQueryLinksMode;
        }

        /// <summary>
        /// 查询链接方式发生修改时修改设置存储的查询链接方式值
        /// </summary>
        public static void SetQueryLinksMode(DictionaryEntry queryLinksMode)
        {
            QueryLinksMode = queryLinksMode;

            LocalSettingsService.SaveSetting(queryLinksModeSettingsKey, queryLinksMode.Value);
        }
    }
}
