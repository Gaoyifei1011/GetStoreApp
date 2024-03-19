using GetStoreApp.Extensions.DataType.Enums;
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
                            // 尝试获取下载状态值
                            if (compositeValue.TryGetValue(DownloadStatus, out object downloadStatusValue))
                            {
                                // 解析下载状态值
                                DownloadStatus status = (DownloadStatus)downloadStatusValue;

                                if (status is Extensions.DataType.Enums.DownloadStatus.Download || status is Extensions.DataType.Enums.DownloadStatus.Wait || status is Extensions.DataType.Enums.DownloadStatus.Unknown)
                                {
                                    compositeValue[DownloadStatus] = Convert.ToInt32(Extensions.DataType.Enums.DownloadStatus.Pause);
                                }
                            }
                            else
                            {
                                compositeValue[DownloadStatus] = Convert.ToInt32(Extensions.DataType.Enums.DownloadStatus.Pause);
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
        /// 直接添加下载记录数据，并返回下载记录是否添加成功的结果
        /// </summary>
        public static bool AddDownloadData(DownloadSchedulerModel downloadSchedulerItem)
        {
            if (downloadStorageContainer is not null)
            {
                Monitor.Enter(downloadStorageLock);

                try
                {
                    // 添加下载记录内容
                    ApplicationDataCompositeValue compositeValue = new ApplicationDataCompositeValue();

                    compositeValue[FileName] = downloadSchedulerItem.FileName;
                    compositeValue[FileLink] = downloadSchedulerItem.FileLink;
                    compositeValue[FilePath] = downloadSchedulerItem.FilePath;
                    compositeValue[FileSize] = downloadSchedulerItem.TotalSize.ToString();
                    compositeValue[DownloadStatus] = Convert.ToInt32(downloadSchedulerItem.DownloadStatus);

                    downloadStorageContainer.Values.Add(DownloadKey, compositeValue);

                    return true;
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "Add download storage container data failed", e);
                    return false;
                }
                finally
                {
                    Monitor.Exit(downloadStorageLock);
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 检查是否存在相同键值的数据，并返回下载记录是否存在相同键值的结果
        /// </summary>
        public static DuplicatedDataKind CheckDuplicatedDownloadData(DownloadSchedulerModel downloadSchedulerItem)
        {
            DuplicatedDataKind duplicatedDataKind = DuplicatedDataKind.None;
            Monitor.Enter(downloadStorageLock);

            try
            {
                // 判断当前下载容器是否存在当前键值，存在获取当前键值对应的下载状态
                if (downloadStorageContainer.Values.TryGetValue(downloadSchedulerItem.DownloadKey, out object downloadItemValue))
                {
                    ApplicationDataCompositeValue compositeValue = downloadItemValue as ApplicationDataCompositeValue;

                    if (compositeValue is not null)
                    {
                        // 尝试获取下载状态值
                        if (compositeValue.TryGetValue(DownloadStatus, out object downloadStatusValue))
                        {
                            // 解析下载状态值
                            duplicatedDataKind = (DownloadStatus)downloadStatusValue is Extensions.DataType.Enums.DownloadStatus.Completed ? DuplicatedDataKind.Completed : DuplicatedDataKind.Unfinished;
                        }
                    }
                }

                return duplicatedDataKind;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Check duplicated download storage container data failed", e);
                return duplicatedDataKind;
            }
            finally
            {
                Monitor.Exit(downloadStorageLock);
            }
        }

        /// <summary>
        /// 删除下载记录数据，并返回下载记录是否删除成功的结果
        /// </summary>
        public static bool DeleteDownloadData(DownloadSchedulerModel downloadSchedulerItem)
        {
            if (downloadStorageContainer is not null)
            {
                Monitor.Enter(downloadStorageLock);

                try
                {
                    // 尝试进行删除，并返回删除的结果
                    return downloadStorageContainer.Values.Remove(downloadSchedulerItem.DownloadKey);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "Delete download storage container data failed", e);
                    return false;
                }
                finally
                {
                    Monitor.Exit(downloadStorageLock);
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 删除选定的下载记录数据，并返回下载记录是否删除成功的结果
        /// </summary>
        public static bool DeleteSelectedDownloadData(List<DownloadSchedulerModel> downloadSchedulerList)
        {
            if (downloadStorageContainer is not null)
            {
                Monitor.Enter(downloadStorageLock);

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
                finally
                {
                    Monitor.Exit(downloadStorageLock);
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取指定下载状态值的下载记录数据，并返回指定下载状态值的查询列表
        /// 指定状态值：下载成功，下载失败
        /// </summary>
        public static List<DownloadSchedulerModel> QueryDownloadData(bool isSuccessDownload)
        {
            List<DownloadSchedulerModel> downloadSchedulerList = new List<DownloadSchedulerModel>();
            Monitor.Enter(downloadStorageLock);

            try
            {
                // 打开每一个下载记录子项，检查该下载记录子项的下载状态是否符合查询的下载记录子项的下载状态
                foreach (KeyValuePair<string, object> downloadItemKey in downloadStorageContainer.Values)
                {
                    ApplicationDataCompositeValue compositeValue = downloadItemKey.Value as ApplicationDataCompositeValue;

                    if (compositeValue is not null)
                    {
                        // 尝试获取下载状态值
                        if (compositeValue.TryGetValue(DownloadStatus, out object downloadStatusValue))
                        {
                            // 解析下载状态值
                            DownloadStatus status = (DownloadStatus)downloadStatusValue;

                            // 获取下载完成的信息列表
                            if (isSuccessDownload && status is Extensions.DataType.Enums.DownloadStatus.Completed)
                            {
                                downloadSchedulerList.Add(new DownloadSchedulerModel()
                                {
                                    DownloadKey = downloadItemKey.Key,
                                    FileName = Convert.ToString(compositeValue[FileName]),
                                    FileLink = Convert.ToString(compositeValue[FileLink]),
                                    FilePath = Convert.ToString(compositeValue[FilePath]),
                                    TotalSize = Convert.ToInt32(compositeValue[FileSize]),
                                    DownloadStatus = (DownloadStatus)compositeValue[DownloadStatus]
                                });
                            }
                            // 获取下载未完成的信息列表
                            else
                            {
                                downloadSchedulerList.Add(new DownloadSchedulerModel()
                                {
                                    DownloadKey = downloadItemKey.Key,
                                    FileName = Convert.ToString(compositeValue[FileName]),
                                    FileLink = Convert.ToString(compositeValue[FileLink]),
                                    FilePath = Convert.ToString(compositeValue[FilePath]),
                                    TotalSize = Convert.ToInt32(compositeValue[FileSize]),
                                    DownloadStatus = (DownloadStatus)compositeValue[DownloadStatus]
                                });
                            }
                        }
                    }
                }

                return downloadSchedulerList;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Query download storage container data with status failed", e);
                return downloadSchedulerList;
            }
            finally
            {
                Monitor.Exit(downloadStorageLock);
            }
        }

        /// <summary>
        /// 获取指定下载键值的下载记录数据，并返回指定下载键值的查询结果
        /// </summary>
        public static DownloadSchedulerModel QueryWithKeyDownloadData(string downloadKey)
        {
            DownloadSchedulerModel downloadSchedulerItem = new DownloadSchedulerModel();
            Monitor.Enter(downloadStorageLock);

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
                        downloadSchedulerItem.DownloadStatus = (DownloadStatus)compositeValue[DownloadStatus];
                    }
                }
                return downloadSchedulerItem;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Query download storage container data with key failed", e);
                return downloadSchedulerItem;
            }
            finally
            {
                Monitor.Exit(downloadStorageLock);
            }
        }

        /// <summary>
        /// 更新指定键值的下载状态值，并返回下载记录状态值是否更新成功的结果
        /// </summary>
        public static bool UpdateDownloadStatusData(string downloadKey, DownloadStatus status)
        {
            if (downloadStorageContainer is not null)
            {
                Monitor.Enter(downloadStorageLock);

                try
                {
                    // 获取并更新指定键值的下载记录状态值
                    if (downloadStorageContainer.Values.TryGetValue(downloadKey, out object downloadItemValue))
                    {
                        ApplicationDataCompositeValue compositeValue = downloadItemValue as ApplicationDataCompositeValue;

                        if (compositeValue is not null)
                        {
                            compositeValue[DownloadStatus] = Convert.ToInt32(status);
                            downloadStorageContainer.Values[downloadKey] = compositeValue;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "Update download storage container flag data failed", e);
                    return false;
                }
                finally
                {
                    Monitor.Exit(downloadStorageLock);
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 清除下载记录
        /// </summary>
        private static bool RemoveDownloadData()
        {
            if (downloadStorageContainer is not null)
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
            else
            {
                return false;
            }
        }
    }
}
