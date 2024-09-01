using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Download;
using GetStoreApp.Services.Root;
using System;
using System.Collections.Generic;
using System.Threading;
using Windows.Foundation.Diagnostics;
using Windows.Storage;

namespace GetStoreApp.Services.Controls.Download
{
    /// <summary>
    /// 已下载完成任务数据存储服务
    /// </summary>
    public static class DownloadStorageService
    {
        private const string DownloadStorage = "DownloadStorage";
        private const string DownloadKey = "DownloadKey";
        private const string FileName = "FileName";
        private const string FileLink = "FileLink";
        private const string FilePath = "FilePath";
        private const string FileSize = "FileSize";

        private static ApplicationDataContainer localSettingsContainer;
        private static ApplicationDataContainer downloadStorageContainer;

        public static SemaphoreSlim DownloadStorageSemaphoreSlim { get; private set; } = new SemaphoreSlim(1, 1);

        public static event Action<DownloadSchedulerModel> StorageDataAdded;

        public static event Action<string> StorageDataDeleted;

        public static event Action StorageDataCleared;

        /// <summary>
        /// 初始化下载记录服务
        /// </summary>
        public static void Initialize()
        {
            localSettingsContainer = ApplicationData.Current.LocalSettings;
            DownloadStorageSemaphoreSlim?.Wait();

            try
            {
                downloadStorageContainer = localSettingsContainer.CreateContainer(DownloadStorage, ApplicationDataCreateDisposition.Always);
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Initialize download storage container failed", e);
            }

            DownloadStorageSemaphoreSlim?.Release();
        }

        /// <summary>
        /// 直接添加已下载完成任务数据
        /// </summary>
        public static void AddDownloadData(DownloadSchedulerModel downloadSchedulerItem)
        {
            if (downloadStorageContainer is not null)
            {
                DownloadStorageSemaphoreSlim?.Wait();

                try
                {
                    ApplicationDataCompositeValue compositeValue = [];

                    downloadSchedulerItem.DownloadKey = string.IsNullOrEmpty(downloadSchedulerItem.DownloadKey)
                        ? HashAlgorithmHelper.GenerateDownloadKey(downloadSchedulerItem.FileName, downloadSchedulerItem.FilePath)
                        : downloadSchedulerItem.DownloadKey;

                    compositeValue[DownloadKey] = downloadSchedulerItem.DownloadKey;
                    compositeValue[FileName] = downloadSchedulerItem.FileName;
                    compositeValue[FileLink] = downloadSchedulerItem.FileLink;
                    compositeValue[FilePath] = downloadSchedulerItem.FilePath;
                    compositeValue[FileSize] = downloadSchedulerItem.TotalSize.ToString();

                    if (downloadStorageContainer.Values.TryAdd(downloadSchedulerItem.DownloadKey, compositeValue))
                    {
                        StorageDataAdded?.Invoke(downloadSchedulerItem);
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "Add download storage container data failed", e);
                }

                DownloadStorageSemaphoreSlim?.Release();
            }
        }

        /// <summary>
        /// 删除已下载完成任务数据
        /// </summary>
        public static void DeleteDownloadData(string downloadKey)
        {
            if (downloadStorageContainer is not null)
            {
                DownloadStorageSemaphoreSlim?.Wait();

                try
                {
                    if (downloadStorageContainer.Values.Remove(downloadKey))
                    {
                        StorageDataDeleted?.Invoke(downloadKey);
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "Delete download storage container data failed", e);
                }

                DownloadStorageSemaphoreSlim?.Release();
            }
        }

        /// <summary>
        /// 获取已下载完成任务数据，为保证安全访问，需要手动对访问的锁进行加锁和释放
        /// </summary>
        public static List<DownloadSchedulerModel> GetDownloadData()
        {
            List<DownloadSchedulerModel> downloadSchedulerList = [];

            if (downloadStorageContainer is not null && DownloadStorageSemaphoreSlim?.CurrentCount is 0)
            {
                try
                {
                    foreach (KeyValuePair<string, object> downloadItemKey in downloadStorageContainer.Values)
                    {
                        if (downloadItemKey.Value is ApplicationDataCompositeValue compositeValue)
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
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "Get download storage container data with status failed", e);
                }
            }

            return downloadSchedulerList;
        }

        /// <summary>
        /// 清除下载记录
        /// </summary>
        public static bool ClearDownloadData()
        {
            bool result = false;

            if (downloadStorageContainer is not null)
            {
                DownloadStorageSemaphoreSlim?.Wait();

                try
                {
                    downloadStorageContainer.Values.Clear();
                    StorageDataCleared?.Invoke();
                    result = true;
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "Clear download storage container data failed", e);
                }

                DownloadStorageSemaphoreSlim?.Release();
            }
            return result;
        }
    }
}
