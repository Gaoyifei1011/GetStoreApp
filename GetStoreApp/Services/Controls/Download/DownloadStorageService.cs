using GetStoreApp.Models.Controls.Download;
using GetStoreApp.Services.Root;
using System;
using System.Collections.Generic;
using Windows.Foundation.Diagnostics;
using Windows.Storage;

namespace GetStoreApp.Services.Controls.Download
{
    /// <summary>
    /// 已下载完成任务数据存储服务
    /// </summary>
    public static class DownloadStorageService
    {
        private static readonly object downloadStorageLock = new object();

        private const string DownloadStorage = "DownloadStorage";
        private const string DownloadKey = "DownloadKey";
        private const string FileName = "FileName";
        private const string FileLink = "FileLink";
        private const string FilePath = "FilePath";
        private const string FileSize = "FileSize";

        private static ApplicationDataContainer localSettingsContainer = ApplicationData.Current.LocalSettings;
        private static ApplicationDataContainer downloadStorageContainer;

        /// <summary>
        /// 初始化下载记录服务
        /// </summary>
        public static void InitializeStorage()
        {
            lock (downloadStorageLock)
            {
                try
                {
                    downloadStorageContainer = localSettingsContainer.CreateContainer(DownloadStorage, ApplicationDataCreateDisposition.Always);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "Initialize download storage container failed", e);
                }
            }
        }

        /// <summary>
        /// 直接添加已下载完成任务数据
        /// </summary>
        public static bool AddDownloadData(DownloadSchedulerModel downloadSchedulerItem)
        {
            if (downloadStorageContainer is not null)
            {
                lock (downloadStorageLock)
                {
                    try
                    {
                        ApplicationDataCompositeValue compositeValue = new ApplicationDataCompositeValue();

                        compositeValue[FileName] = downloadSchedulerItem.FileName;
                        compositeValue[FileLink] = downloadSchedulerItem.FileLink;
                        compositeValue[FilePath] = downloadSchedulerItem.FilePath;
                        compositeValue[FileSize] = downloadSchedulerItem.TotalSize.ToString();

                        downloadStorageContainer.Values.Add(DownloadKey, compositeValue);

                        return true;
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "Add download storage container data failed", e);
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 检查是否已下载完成任务的数据
        /// </summary>
        public static bool CheckDuplicatedDownloadData(DownloadSchedulerModel downloadSchedulerItem)
        {
            if (downloadStorageContainer is not null)
            {
                lock (downloadStorageLock)
                {
                    try
                    {
                        return downloadStorageContainer.Values.ContainsKey(downloadSchedulerItem.DownloadKey);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "Check duplicated download storage container data failed", e);
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 删除已下载完成任务数据
        /// </summary>
        public static bool DeleteDownloadData(DownloadSchedulerModel downloadSchedulerItem)
        {
            if (downloadStorageContainer is not null)
            {
                lock (downloadStorageLock)
                {
                    try
                    {
                        return downloadStorageContainer.Values.Remove(downloadSchedulerItem.DownloadKey);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "Delete download storage container data failed", e);
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 删除已下载完成任务数据，并返回下载记录是否删除成功的结果
        /// </summary>
        public static bool DeleteSelectedDownloadData(List<DownloadSchedulerModel> downloadSchedulerList)
        {
            if (downloadStorageContainer is not null)
            {
                lock (downloadStorageLock)
                {
                    try
                    {
                        bool deleteResult = false;

                        // 遍历选定的下载记录数据，尝试删除
                        foreach (DownloadSchedulerModel downloadSchedulerItem in downloadSchedulerList)
                        {
                            bool result = downloadStorageContainer.Values.Remove(downloadSchedulerItem.DownloadKey);

                            if (result)
                            {
                                deleteResult = result;
                            }
                        }

                        return deleteResult;
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "Delete selected download storage container data failed", e);
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取已下载完成任务数据
        /// </summary>
        public static List<DownloadSchedulerModel> QueryDownloadData()
        {
            List<DownloadSchedulerModel> downloadSchedulerList = new List<DownloadSchedulerModel>();

            lock (downloadStorageLock)
            {
                try
                {
                    foreach (KeyValuePair<string, object> downloadItemKey in downloadStorageContainer.Values)
                    {
                        ApplicationDataCompositeValue compositeValue = downloadItemKey.Value as ApplicationDataCompositeValue;

                        if (compositeValue is not null)
                        {
                            downloadSchedulerList.Add(new DownloadSchedulerModel()
                            {
                                DownloadKey = downloadItemKey.Key,
                                FileName = Convert.ToString(compositeValue[FileName]),
                                FileLink = Convert.ToString(compositeValue[FileLink]),
                                FilePath = Convert.ToString(compositeValue[FilePath]),
                                TotalSize = Convert.ToInt32(compositeValue[FileSize])
                            });
                        }
                    }

                    return downloadSchedulerList;
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "Query download storage container data with status failed", e);
                    return downloadSchedulerList;
                }
            }
        }

        /// <summary>
        /// 获取指定下载键值的已下载完成任务数据，并返回指定下载键值的查询结果
        /// </summary>
        public static DownloadSchedulerModel QueryWithKeyDownloadData(string downloadKey)
        {
            DownloadSchedulerModel downloadSchedulerItem = new DownloadSchedulerModel();

            lock (downloadStorageLock)
            {
                try
                {
                    // 获取指定下载键值的下载记录数据
                    if (downloadStorageContainer.Values.TryGetValue(downloadKey, out object downloadItemValue))
                    {
                        ApplicationDataCompositeValue compositeValue = downloadItemValue as ApplicationDataCompositeValue;

                        if (compositeValue is not null)
                        {
                            downloadSchedulerItem.DownloadKey = downloadKey;
                            downloadSchedulerItem.FileName = Convert.ToString(compositeValue[FileName]);
                            downloadSchedulerItem.FileLink = Convert.ToString(compositeValue[FileLink]);
                            downloadSchedulerItem.FilePath = Convert.ToString(compositeValue[FilePath]);
                            downloadSchedulerItem.TotalSize = Convert.ToInt32(compositeValue[FileSize]);
                        }
                    }
                    return downloadSchedulerItem;
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "Query download storage container data with key failed", e);
                    return downloadSchedulerItem;
                }
            }
        }

        /// <summary>
        /// 清除下载记录
        /// </summary>
        private static bool RemoveDownloadData()
        {
            if (downloadStorageContainer is not null)
            {
                lock (downloadStorageLock)
                {
                    try
                    {
                        downloadStorageContainer.Values.Clear();
                        return true;
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "Clear download storage container data failed", e);
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }
    }
}
