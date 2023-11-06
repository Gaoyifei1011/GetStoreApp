using GetStoreApp.Models.Controls.History;
using System;
using System.Collections.Generic;
using Windows.Storage;

namespace GetStoreApp.Services.Controls.History
{
    /// <summary>
    /// 历史记录存储服务
    /// </summary>
    public static class HistoryService
    {
        private const string QueryLinks = "QueryLinks";
        private const string Search = "Search";

        private static ApplicationDataContainer localSettingsContainer = ApplicationData.Current.LocalSettings;
        private static ApplicationDataContainer queryLinksContainer;
        private static ApplicationDataContainer searchContainer;

        /// <summary>
        /// 初始化历史理解存储服务
        /// </summary>
        public static void Initialize()
        {
            queryLinksContainer = localSettingsContainer.CreateContainer(QueryLinks, ApplicationDataCreateDisposition.Always);
            searchContainer = localSettingsContainer.CreateContainer(Search, ApplicationDataCreateDisposition.Always);
        }

        /// <summary>
        /// 获取查询链接历史记录数据
        /// </summary>
        public static List<HistoryModel> GetQueryLinksData()
        {
            List<HistoryModel> queryLinksHistoryList = new List<HistoryModel>();

            for (int index = 1; index <= 3; index++)
            {
                string queryLinksKey = queryLinksContainer + index.ToString();
                if (queryLinksContainer.Values.TryGetValue(queryLinksKey, out object value))
                {
                    try
                    {
                        string[] queryLinksArray = Convert.ToString(value).Split('|');
                        queryLinksHistoryList.Add(new HistoryModel()
                        {
                            CreateTimeStamp = Convert.ToInt64(queryLinksArray[0]),
                            HistoryKey = queryLinksArray[1],
                            HistoryAppName = queryLinksArray[2],
                            HistoryType = queryLinksArray[3],
                            HistoryChannel = queryLinksArray[4],
                            HistoryLink = queryLinksArray[5]
                        });
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }

            return queryLinksHistoryList;
        }

        /// <summary>
        /// 存储查询链接历史记录数据
        /// </summary>
        public static void SaveQueryLinksData(List<HistoryModel> queryLinksHistoryList)
        {
            for (int index = 1; index <= 3; index++)
            {
                string value = string.Format("{0}|{1}|{2}|{3}|{4}|{5}",
                    queryLinksHistoryList[index - 1].CreateTimeStamp,
                    queryLinksHistoryList[index].HistoryKey,
                    queryLinksHistoryList[index].HistoryAppName,
                    queryLinksHistoryList[index].HistoryType,
                    queryLinksHistoryList[index].HistoryChannel,
                    queryLinksHistoryList[index - 1].HistoryLink
                    );

                string queryLinksKey = queryLinksContainer + index.ToString();
                queryLinksContainer.Values[queryLinksKey] = value;
            }
        }

        /// <summary>
        /// 清除历史记录
        /// </summary>
        public static void ClearData()
        {
            for (int index = 1; index <= 3; index++)
            {
                queryLinksContainer.Values.Remove(QueryLinks + index.ToString());
                searchContainer.Values.Remove(Search + index.ToString());
            }
        }
    }
}
