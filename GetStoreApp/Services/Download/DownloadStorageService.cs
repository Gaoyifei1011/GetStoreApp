using GetStoreApp.Helpers.Root;
using GetStoreApp.Models;
using GetStoreApp.Services.Root;
using Microsoft.Windows.Storage;
using System;
using System.Collections.Generic;
using System.Threading;
using Windows.Foundation.Diagnostics;

namespace GetStoreApp.Services.Download
{
    /// <summary>
    /// 已下载完成任务数据存储服务
    /// </summary>
    public static class DownloadStorageService
    {
        private const string DownloadStorage = "DownloadStorage";
        private const string DownloadKey = "DownloadKey";
        private const string FileName = "FileName";
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
            localSettingsContainer = ApplicationData.GetDefault().LocalSettings;
            DownloadStorageSemaphoreSlim?.Wait();

            try
            {
                downloadStorageContainer = localSettingsContainer.CreateContainer(DownloadStorage, ApplicationDataCreateDisposition.Always);
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(DownloadStorageService), nameof(Initialize), 1, e);
            }

            DownloadStorageSemaphoreSlim?.Release();
        }

        /// <summary>
        /// 直接添加已下载完成任务数据
        /// </summary>
        public static void AddDownloadData(DownloadSchedulerModel downloadScheduler)
        {
            if (downloadStorageContainer is not null)
            {
                DownloadStorageSemaphoreSlim?.Wait();

                try
                {
                    Windows.Storage.ApplicationDataCompositeValue compositeValue = [];

                    downloadScheduler.DownloadKey = string.IsNullOrEmpty(downloadScheduler.DownloadKey)
                        ? HashAlgorithmHelper.GenerateDownloadKey(downloadScheduler.FileName, downloadScheduler.FilePath)
                        : downloadScheduler.DownloadKey;

                    compositeValue[DownloadKey] = downloadScheduler.DownloadKey;
                    compositeValue[FileName] = downloadScheduler.FileName;
                    compositeValue[FilePath] = downloadScheduler.FilePath;
                    compositeValue[FileSize] = downloadScheduler.TotalSize;

                    if (downloadStorageContainer.Values.TryAdd(downloadScheduler.DownloadKey, compositeValue))
                    {
                        StorageDataAdded?.Invoke(downloadScheduler);
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(DownloadStorageService), nameof(AddDownloadData), 1, e);
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
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(DownloadStorageService), nameof(DeleteDownloadData), 1, e);
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

            if (downloadStorageContainer is not null)
            {
                try
                {
                    foreach (KeyValuePair<string, object> downloadStorageItem in downloadStorageContainer.Values)
                    {
                        if (downloadStorageItem.Value is Windows.Storage.ApplicationDataCompositeValue compositeValue)
                        {
                            downloadSchedulerList.Add(new DownloadSchedulerModel()
                            {
                                DownloadKey = downloadStorageItem.Key,
                                FileName = Convert.ToString(compositeValue[FileName]),
                                FilePath = Convert.ToString(compositeValue[FilePath]),
                                TotalSize = Convert.ToDouble(compositeValue[FileSize])
                            });
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(DownloadStorageService), nameof(GetDownloadData), 1, e);
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
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(DownloadStorageService), nameof(ClearDownloadData), 1, e);
                }

                DownloadStorageSemaphoreSlim?.Release();
            }
            return result;
        }
    }
}
