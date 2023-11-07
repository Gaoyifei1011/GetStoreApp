using GetStoreApp.Models.Controls.Store;
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
        private const string SearchStore = "SearchStore";

        private static ApplicationDataContainer localSettingsContainer = ApplicationData.Current.LocalSettings;
        private static ApplicationDataContainer queryLinksContainer;
        private static ApplicationDataContainer searchStoreContainer;

        /// <summary>
        /// 初始化历史记录存储服务
        /// </summary>
        public static void Initialize()
        {
            queryLinksContainer = localSettingsContainer.CreateContainer(QueryLinks, ApplicationDataCreateDisposition.Always);
            searchStoreContainer = localSettingsContainer.CreateContainer(SearchStore, ApplicationDataCreateDisposition.Always);
        }

        /// <summary>
        /// 获取查询链接历史记录数据
        /// </summary>
        public static List<HistoryModel> GetQueryLinksData()
        {
            List<HistoryModel> queryLinksHistoryList = new List<HistoryModel>();

            for (int index = 1; index <= 3; index++)
            {
                string queryLinksKey = QueryLinks + index.ToString();
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
        /// 获取搜索应用历史记录数据
        /// </summary>
        public static List<HistoryModel> GetSearchStoreData()
        {
            List<HistoryModel> searchStoreHistoryList = new List<HistoryModel>();

            for (int index = 1; index <= 3; index++)
            {
                string searchStoreKey = SearchStore + index.ToString();
                if (searchStoreContainer.Values.TryGetValue(searchStoreKey, out object value))
                {
                    try
                    {
                        string[] searchStoreArray = Convert.ToString(value).Split('|');
                        searchStoreHistoryList.Add(new HistoryModel()
                        {
                            CreateTimeStamp = Convert.ToInt64(searchStoreArray[0]),
                            HistoryKey = searchStoreArray[1],
                            HistoryContent = searchStoreArray[2]
                        });
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }

            return searchStoreHistoryList;
        }

        /// <summary>
        /// 存储查询链接历史记录数据
        /// </summary>
        public static void SaveQueryLinksData(List<HistoryModel> queryLinksHistoryList)
        {
            int endIndex = queryLinksHistoryList.Count > 3 ? 3 : queryLinksHistoryList.Count;
            for (int index = 1; index <= endIndex; index++)
            {
                string value = string.Format("{0}|{1}|{2}|{3}|{4}|{5}",
                    queryLinksHistoryList[index - 1].CreateTimeStamp,
                    queryLinksHistoryList[index - 1].HistoryKey,
                    queryLinksHistoryList[index - 1].HistoryAppName,
                    queryLinksHistoryList[index - 1].HistoryType,
                    queryLinksHistoryList[index - 1].HistoryChannel,
                    queryLinksHistoryList[index - 1].HistoryLink
                    );

                string queryLinksKey = QueryLinks + index.ToString();
                queryLinksContainer.Values[queryLinksKey] = value;
            }
        }

        public static void SaveSearchStoreData(List<HistoryModel> searchStoreHistoryList)
        {
            int endIndex = searchStoreHistoryList.Count > 3 ? 3 : searchStoreHistoryList.Count;
            for (int index = 1; index <= endIndex; index++)
            {
                string value = string.Format("{0}|{1}|{2}",
                    searchStoreHistoryList[index - 1].CreateTimeStamp,
                    searchStoreHistoryList[index - 1].HistoryKey,
                    searchStoreHistoryList[index - 1].HistoryContent
                    );

                string searchStoreKey = SearchStore + index.ToString();
                searchStoreContainer.Values[searchStoreKey] = value;
            }
        }

        /// <summary>
        /// 清除历史记录
        /// </summary>
        public static bool ClearData()
        {
            try
            {
                for (int index = 1; index <= 3; index++)
                {
                    queryLinksContainer.Values.Remove(QueryLinks + index.ToString());
                    searchStoreContainer.Values.Remove(SearchStore + index.ToString());
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
