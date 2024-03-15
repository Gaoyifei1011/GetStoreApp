using GetStoreApp.Services.Root;
using System;
using System.Collections.Generic;
using System.Threading;
using Windows.Foundation.Diagnostics;
using Windows.Storage;

namespace GetStoreApp.Services.Controls.Download
{
    /// <summary>
    /// 下载数据存储服务
    /// </summary>
    public static class DownloadStorageService
    {
        private static readonly object downloadStorageLock = new object();

        private const string DownloadStorage = "DownloadStorage";
        private const string DownloadKey = "DownloadKey";
        private const string FileName = "FileName";
        private const string FileLink = "FileLink";
        private const string FilePath = "FilePath";
        private const string FileSHA1 = "FileSHA1";
        private const string FileSize = "FileSize";
        private const string DownloadStatus = "DownloadStatus";

        private static ApplicationDataContainer localSettingsContainer = ApplicationData.Current.LocalSettings;
        private static ApplicationDataContainer downloadStorageContainer;

        /// <summary>
        /// 初始化下载记录服务，包括下载记录项和已有的下载记录
        /// </summary>
        public static void InitializeService()
        {
            InitializeContainer();
            InitializeDownloadData();
        }

        /// <summary>
        /// 初始化下载记录存储容器
        /// </summary>
        private static void InitializeContainer()
        {
            Monitor.Enter(downloadStorageLock);

            try
            {
                downloadStorageContainer = localSettingsContainer.CreateContainer(DownloadStorage, ApplicationDataCreateDisposition.Always);
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Initialize download storage container failed", e);
            }
            finally
            {
                Monitor.Exit(downloadStorageLock);
            }
        }

        /// <summary>
        /// 初始化已有的下载记录，将已有异常的下载记录进行复原
        /// </summary>
        private static void InitializeDownloadData()
        {
            if (downloadStorageContainer is not null)
            {
                Monitor.Enter(downloadStorageLock);

                try
                {
                    // 打开每一个下载记录子项，检查是否有下载异常的记录，并将对应的下载状态值修改为暂停下载状态
                    foreach (KeyValuePair<string, object> downloadItemKey in downloadStorageContainer.Values)
                    {
                        ApplicationDataCompositeValue compositeValue = downloadItemKey.Value as ApplicationDataCompositeValue;

                        if (compositeValue is not null)
                        {
                            if (compositeValue.TryGetValue(DownloadStatus, out object value))
                            {
                                int status = Convert.ToInt32(value);

                                if (status is 1 || status is 3)
                                {
                                    compositeValue[DownloadStatus] = 2;
                                }
                            }
                            else
                            {
                                compositeValue[DownloadStatus] = 2;
                            }

                            // 保存修改发生变化后的值
                            downloadStorageContainer.Values[downloadItemKey.Key] = compositeValue;
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "Moidfy download storage container data failed", e);
                }
                finally
                {
                    Monitor.Exit(downloadStorageLock);
                }
            }
        }

        /// <summary>
        /// 清除下载记录
        /// </summary>
        private static bool ClearData()
        {
            Monitor.Enter(downloadStorageLock);

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
            finally
            {
                Monitor.Exit(downloadStorageLock);
            }
        }
    }
}
