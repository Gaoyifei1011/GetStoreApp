using GetStoreApp.Models;
using GetStoreApp.Services.Download;
using GetStoreApp.Services.Root;
using Microsoft.Windows.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using Windows.Foundation.Diagnostics;
using WinRT;

namespace GetStoreApp.Services.History
{
    /// <summary>
    /// 历史记录存储服务
    /// </summary>
    internal static class HistoryStorageService
    {
        private const string QueryLinks = "QueryLinks";
        private const string SearchApps = "SearchApps";
        private const string CreateTimeStamp = "CreateTimeStamp";
        private const string HistoryAppName = "HistoryAppName";
        private const string HistoryContent = "HistoryContent";
        private const string HistoryType = "HistoryType";
        private const string HistoryChannel = "HistoryChannel";
        private const string HistoryLink = "HistoryLink";

        private static readonly Lock historyStorageLock = new();
        private static readonly ApplicationDataContainer localSettingsContainer = ApplicationData.GetDefault().LocalSettings;
        private static ApplicationDataContainer queryLinksContainer;
        private static ApplicationDataContainer searchAppsContainer;

        private static ReadOnlyCollection<TypeModel> TypeCollection { get; } =
        [
            new() { InternalName = "url", ShortName = "url" },
            new() { InternalName = "ProductId", ShortName = "pid" }
        ];

        private static ReadOnlyCollection<ChannelModel> ChannelCollection { get; } =
        [
            new() { InternalName = "WIF", ShortName = "wif" },
            new() { InternalName = "WIS", ShortName = "wis" },
            new() { InternalName = "RP", ShortName = "rp" },
            new() { InternalName = "Retail", ShortName = "rt" }
        ];

        internal static event Action QueryLinksCleared;

        internal static event Action SearchAppsCleared;

        /// <summary>
        /// 初始化历史记录存储服务
        /// </summary>
        internal static void Initialize()
        {
            queryLinksContainer = localSettingsContainer.CreateContainer(QueryLinks, ApplicationDataCreateDisposition.Always);
            searchAppsContainer = localSettingsContainer.CreateContainer(SearchApps, ApplicationDataCreateDisposition.Always);
        }

        /// <summary>
        /// 获取查询链接历史记录数据
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(Windows.Storage.ApplicationDataCompositeValue))]
        internal static ReadOnlyCollection<HistoryModel> GetQueryLinksDataCollection()
        {
            List<HistoryModel> queryLinksHistoryList = [];
            historyStorageLock.Enter();

            try
            {
                if (queryLinksContainer is not null)
                {
                    foreach (KeyValuePair<string, object> queryLinksContainerItem in queryLinksContainer.Values)
                    {
                        if (queryLinksContainerItem.Key.Length is 32 && queryLinksContainerItem.Value is Windows.Storage.ApplicationDataCompositeValue compositeValue)
                        {
                            TypeModel type = null;
                            string historyType = compositeValue[HistoryType] as string;
                            foreach (TypeModel typeItem in TypeCollection)
                            {
                                if (string.Equals(typeItem.InternalName, historyType, StringComparison.OrdinalIgnoreCase))
                                {
                                    type = typeItem;
                                    break;
                                }
                            }

                            ChannelModel channel = null;
                            string historyChannel = compositeValue[HistoryChannel] as string;
                            foreach (ChannelModel channelItem in ChannelCollection)
                            {
                                if (string.Equals(channelItem.InternalName, historyChannel, StringComparison.OrdinalIgnoreCase))
                                {
                                    channel = channelItem;
                                    break;
                                }
                            }

                            queryLinksHistoryList.Add(new()
                            {
                                HistoryKey = queryLinksContainerItem.Key,
                                CreateTimeStamp = Convert.ToInt64(compositeValue[CreateTimeStamp]),
                                HistoryAppName = Convert.ToString(compositeValue[HistoryAppName]),
                                HistoryType = type.InternalName,
                                HistoryChannel = channel.InternalName,
                                HistoryLink = Convert.ToString(compositeValue[HistoryLink])
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(DownloadStorageService), nameof(GetQueryLinksDataCollection), 1, e);
            }
            finally
            {
                historyStorageLock.Exit();
            }

            queryLinksHistoryList.Sort((item1, item2) => item2.CreateTimeStamp.CompareTo(item1.CreateTimeStamp));
            return queryLinksHistoryList.AsReadOnly();
        }

        /// <summary>
        /// 获取搜索应用历史记录数据
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(Windows.Storage.ApplicationDataCompositeValue))]
        internal static ReadOnlyCollection<HistoryModel> GetSearchAppsDataCollection()
        {
            List<HistoryModel> searchAppsHistoryList = [];
            historyStorageLock.Enter();

            try
            {
                if (searchAppsContainer is not null)
                {
                    foreach (KeyValuePair<string, object> searchAppsContainerItem in searchAppsContainer.Values)
                    {
                        if (searchAppsContainerItem.Key.Length is 32 && searchAppsContainerItem.Value is Windows.Storage.ApplicationDataCompositeValue compositeValue)
                        {
                            TypeModel type = null;
                            string historyType = compositeValue[HistoryType] as string;
                            foreach (TypeModel typeItem in TypeCollection)
                            {
                                if (string.Equals(typeItem.InternalName, historyType, StringComparison.OrdinalIgnoreCase))
                                {
                                    type = typeItem;
                                    break;
                                }
                            }

                            ChannelModel channel = null;
                            string historyChannel = compositeValue[HistoryChannel] as string;
                            foreach (ChannelModel channelItem in ChannelCollection)
                            {
                                if (string.Equals(channelItem.InternalName, historyChannel, StringComparison.OrdinalIgnoreCase))
                                {
                                    channel = channelItem;
                                    break;
                                }
                            }
                            searchAppsHistoryList.Add(new()
                            {
                                HistoryKey = searchAppsContainerItem.Key,
                                CreateTimeStamp = Convert.ToInt64(compositeValue[CreateTimeStamp]),
                                HistoryContent = Convert.ToString(compositeValue[HistoryContent]),
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(DownloadStorageService), nameof(GetSearchAppsDataCollection), 1, e);
            }
            finally
            {
                historyStorageLock.Exit();
            }

            searchAppsHistoryList.Sort((item1, item2) => item2.CreateTimeStamp.CompareTo(item1.CreateTimeStamp));
            return searchAppsHistoryList.AsReadOnly();
        }

        /// <summary>
        /// 存储查询链接历史记录数据
        /// </summary>
        internal static void SaveQueryLinksData(IReadOnlyList<HistoryModel> queryLinksHistoryList)
        {
            if (queryLinksHistoryList is null || queryLinksHistoryList.Count is 0)
            {
                return;
            }

            historyStorageLock.Enter();

            try
            {
                foreach (HistoryModel historyItem in queryLinksHistoryList)
                {
                    Windows.Storage.ApplicationDataCompositeValue compositeValue = new()
                    {
                        [CreateTimeStamp] = historyItem.CreateTimeStamp,
                        [HistoryAppName] = historyItem.HistoryAppName,
                        [HistoryType] = historyItem.HistoryType,
                        [HistoryChannel] = historyItem.HistoryChannel,
                        [HistoryLink] = historyItem.HistoryLink
                    };

                    queryLinksContainer.Values[historyItem.HistoryKey] = compositeValue;
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(DownloadStorageService), nameof(SaveQueryLinksData), 1, e);
            }
            finally
            {
                historyStorageLock.Exit();
            }
        }

        /// <summary>
        /// 存储搜索应用历史记录数据
        /// </summary>
        internal static void SaveSearchAppsData(IReadOnlyList<HistoryModel> searchAppsHistoryList)
        {
            if (searchAppsHistoryList is null || searchAppsHistoryList.Count is 0)
            {
                return;
            }

            historyStorageLock.Enter();

            try
            {
                foreach (HistoryModel historyItem in searchAppsHistoryList)
                {
                    Windows.Storage.ApplicationDataCompositeValue compositeValue = new()
                    {
                        [CreateTimeStamp] = historyItem.CreateTimeStamp,
                        [HistoryContent] = historyItem.HistoryContent
                    };

                    searchAppsContainer.Values[historyItem.HistoryKey] = compositeValue;
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(DownloadStorageService), nameof(SaveSearchAppsData), 1, e);
            }
            finally
            {
                historyStorageLock.Exit();
            }
        }

        /// <summary>
        /// 更新查询链接历史记录数据
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(Windows.Storage.ApplicationDataCompositeValue))]
        internal static void UpdateQueryLinksData(HistoryModel historyItem)
        {
            if (historyItem is not null && queryLinksContainer.Values.TryGetValue(historyItem.HistoryKey, out object compositeValueObj) && compositeValueObj is Windows.Storage.ApplicationDataCompositeValue compositeValue)
            {
                compositeValue[CreateTimeStamp] = historyItem.CreateTimeStamp;
                compositeValue[HistoryAppName] = historyItem.HistoryAppName;
            }
        }

        /// <summary>
        /// 更新搜索应用历史记录数据
        /// </summary>
        [DynamicWindowsRuntimeCast(typeof(Windows.Storage.ApplicationDataCompositeValue))]
        internal static void UpdateSearchAppsData(HistoryModel historyItem)
        {
            if (historyItem is not null && searchAppsContainer.Values.TryGetValue(historyItem.HistoryKey, out object compositeValueObj) && compositeValueObj is Windows.Storage.ApplicationDataCompositeValue compositeValue)
            {
                compositeValue[CreateTimeStamp] = historyItem.CreateTimeStamp;
            }
        }

        /// <summary>
        /// 移除查询链接历史记录数据
        /// </summary>
        internal static void RemoveQueryLinksData(string historyKey)
        {
            if (string.IsNullOrEmpty(historyKey))
            {
                return;
            }

            historyStorageLock.Enter();

            try
            {
                queryLinksContainer.Values.Remove(historyKey);
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(DownloadStorageService), nameof(RemoveQueryLinksData), 1, e);
            }
            finally
            {
                historyStorageLock.Exit();
            }
        }

        /// <summary>
        /// 移除搜索应用记录数据
        /// </summary>
        internal static void RemoveSearchAppsData(string historyKey)
        {
            if (string.IsNullOrEmpty(historyKey))
            {
                return;
            }

            historyStorageLock.Enter();

            try
            {
                searchAppsContainer.Values.Remove(historyKey);
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(DownloadStorageService), nameof(RemoveSearchAppsData), 1, e);
            }
            finally
            {
                historyStorageLock.Exit();
            }
        }

        /// <summary>
        /// 清除历史记录
        /// </summary>
        internal static bool ClearData()
        {
            historyStorageLock.Enter();

            try
            {
                queryLinksContainer.Values.Clear();
                QueryLinksCleared?.Invoke();
                searchAppsContainer.Values.Clear();
                SearchAppsCleared?.Invoke();
                return true;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(DownloadStorageService), nameof(ClearData), 1, e);
                return false;
            }
            finally
            {
                historyStorageLock.Exit();
            }
        }
    }
}
