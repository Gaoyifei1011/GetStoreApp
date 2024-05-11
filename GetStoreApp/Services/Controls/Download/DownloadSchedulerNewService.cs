using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models.Controls.Download;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.ComTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using Windows.Foundation.Diagnostics;

namespace GetStoreApp.Services.Controls.Download
{
    /// <summary>
    /// 下载调度服务
    /// </summary>
    public static class DownloadSchedulerNewService
    {
        private static int badgeCount = 0;
        private static object downloadSchedulerLock = new object();
        private static DictionaryEntry doEngineMode;

        public static event Action<Guid, DownloadSchedulerModel> DownloadCreated;

        public static event Action<Guid> DownloadContinued;

        public static event Action<Guid> DownloadPaused;

        public static event Action<Guid> DownloadDeleted;

        public static event Action<Guid, DownloadSchedulerModel> DownloadProgressing;

        public static event Action<Guid, DownloadSchedulerModel> DownloadCompleted;

        public static event Action<int> CollectionCountChanged;

        // 下载任务列表
        public static List<DownloadSchedulerModel> DownloadSchedulerList { get; } = new List<DownloadSchedulerModel>();

        #region 第一部分：传递优化服务挂载的事件

        /// <summary>
        /// 传递优化：下载任务已创建事件
        /// </summary>
        private static void OnDeliveryOptimizationCreated(Guid downloadID, string fileName, string filePath, string url, double totalSize)
        {
            lock (downloadSchedulerLock)
            {
                DownloadSchedulerModel downloadSchedulerItem = new DownloadSchedulerModel()
                {
                    DownloadID = downloadID,
                    DownloadStatus = DownloadStatus.Downloading,
                    FileName = fileName,
                    FilePath = filePath,
                    FileLink = url,
                    FinishedSize = 0,
                    TotalSize = totalSize
                };

                DownloadSchedulerList.Add(downloadSchedulerItem);
                DownloadCreated?.Invoke(downloadID, downloadSchedulerItem);
                CollectionCountChanged?.Invoke(DownloadSchedulerList.Count);
            }
        }

        /// <summary>
        /// 传递优化：下载任务已继续下载事件
        /// </summary>
        private static void OnDeliveryOptimizationContinued(Guid downloadID)
        {
            lock (downloadSchedulerLock)
            {
                foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerList)
                {
                    if (downloadSchedulerItem.DownloadID.Equals(downloadID))
                    {
                        downloadSchedulerItem.DownloadStatus = DownloadStatus.Downloading;

                        DownloadContinued?.Invoke(downloadID);
                        CollectionCountChanged?.Invoke(DownloadSchedulerList.Count);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 传递优化：下载任务已暂停事件
        /// </summary>
        private static void OnDeliveryOptimizationPaused(Guid downloadID)
        {
            lock (downloadSchedulerLock)
            {
                foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerList)
                {
                    if (downloadSchedulerItem.DownloadID.Equals(downloadID))
                    {
                        downloadSchedulerItem.DownloadStatus = DownloadStatus.Pause;

                        DownloadPaused?.Invoke(downloadID);
                        CollectionCountChanged?.Invoke(DownloadSchedulerList.Count);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 传递优化：下载任务已删除事件
        /// </summary>
        private static void OnDeliveryOptimizationDeleted(Guid downloadID)
        {
            lock (downloadSchedulerLock)
            {
                foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerList)
                {
                    if (downloadSchedulerItem.DownloadID.Equals(downloadID))
                    {
                        DownloadSchedulerList.Remove(downloadSchedulerItem);

                        DownloadDeleted?.Invoke(downloadID);
                        CollectionCountChanged?.Invoke(DownloadSchedulerList.Count);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 传递优化：下载任务下载进度发生变化事件
        /// </summary>
        private static void OnDeliveryOptimizationProgressing(Guid downloadID, DO_DOWNLOAD_STATUS status)
        {
            lock (downloadSchedulerLock)
            {
                foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerList)
                {
                    if (downloadSchedulerItem.DownloadID.Equals(downloadID))
                    {
                        downloadSchedulerItem.DownloadStatus = DownloadStatus.Downloading;
                        downloadSchedulerItem.FinishedSize = status.BytesTransferred;
                        downloadSchedulerItem.TotalSize = status.BytesTotal;

                        DownloadProgressing?.Invoke(downloadID, downloadSchedulerItem);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 传递优化：下载任务已下载完成事件
        /// </summary>
        private static void OnDeliveryOptimizationCompleted(Guid downloadID, DO_DOWNLOAD_STATUS status)
        {
            lock (downloadSchedulerLock)
            {
                foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerList)
                {
                    if (downloadSchedulerItem.DownloadID.Equals(downloadID))
                    {
                        downloadSchedulerItem.DownloadStatus = DownloadStatus.Completed;
                        downloadSchedulerItem.FinishedSize = status.BytesTransferred;
                        downloadSchedulerItem.TotalSize = status.BytesTotal;

                        DownloadCompleted?.Invoke(downloadID, downloadSchedulerItem);
                        CollectionCountChanged?.Invoke(DownloadSchedulerList.Count);
                        DownloadStorageService.AddDownloadData(downloadSchedulerItem);
                        break;
                    }
                }
            }
        }

        #endregion 第一部分：传递优化服务挂载的事件

        #region 第二部分：后台智能传输服务挂载的事件

        /// <summary>
        /// 后台智能传输任务：下载任务已创建事件
        /// </summary>
        private static void OnBitsCreated(Guid downloadID, string fileName, string filePath, string url, double totalSize)
        {
            lock (downloadSchedulerLock)
            {
                DownloadSchedulerModel downloadSchedulerItem = new DownloadSchedulerModel()
                {
                    DownloadID = downloadID,
                    DownloadStatus = DownloadStatus.Downloading,
                    FileName = fileName,
                    FilePath = filePath,
                    FileLink = url,
                    FinishedSize = 0,
                    TotalSize = totalSize
                };

                DownloadSchedulerList.Add(downloadSchedulerItem);
                DownloadCreated?.Invoke(downloadID, downloadSchedulerItem);
                CollectionCountChanged?.Invoke(DownloadSchedulerList.Count);
            }
        }

        /// <summary>
        /// 后台智能传输任务：下载任务已继续下载事件
        /// </summary>
        private static void OnBitsContinued(Guid downloadID)
        {
            lock (downloadSchedulerLock)
            {
                foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerList)
                {
                    if (downloadSchedulerItem.DownloadID.Equals(downloadID))
                    {
                        downloadSchedulerItem.DownloadStatus = DownloadStatus.Downloading;
                        DownloadContinued?.Invoke(downloadID);
                        CollectionCountChanged?.Invoke(DownloadSchedulerList.Count);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 后台智能传输任务：下载任务已暂停事件
        /// </summary>
        private static void OnBitsPaused(Guid downloadID)
        {
            lock (downloadSchedulerLock)
            {
                foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerList)
                {
                    if (downloadSchedulerItem.DownloadID.Equals(downloadID))
                    {
                        downloadSchedulerItem.DownloadStatus = DownloadStatus.Pause;
                        DownloadPaused?.Invoke(downloadID);
                        CollectionCountChanged?.Invoke(DownloadSchedulerList.Count);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 后台智能传输任务：下载任务已删除事件
        /// </summary>
        private static void OnBitsDeleted(Guid downloadID)
        {
            lock (downloadSchedulerLock)
            {
                foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerList)
                {
                    if (downloadSchedulerItem.DownloadID.Equals(downloadID))
                    {
                        DownloadSchedulerList.Remove(downloadSchedulerItem);

                        DownloadDeleted?.Invoke(downloadID);
                        CollectionCountChanged?.Invoke(DownloadSchedulerList.Count);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 后台智能传输任务：下载任务下载进度发生变化事件
        /// </summary>
        private static void OnBitsProgressing(Guid downloadID, BG_JOB_PROGRESS progress)
        {
            lock (downloadSchedulerLock)
            {
                foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerList)
                {
                    if (downloadSchedulerItem.DownloadID.Equals(downloadID))
                    {
                        downloadSchedulerItem.DownloadStatus = DownloadStatus.Downloading;
                        downloadSchedulerItem.FinishedSize = progress.BytesTransferred;
                        downloadSchedulerItem.TotalSize = progress.BytesTotal;

                        DownloadProgressing?.Invoke(downloadID, downloadSchedulerItem);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 后台智能传输任务：下载任务已下载完成事件
        /// </summary>
        private static void OnBitsCompleted(Guid downloadID, BG_JOB_PROGRESS progress)
        {
            lock (downloadSchedulerLock)
            {
                foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerList)
                {
                    if (downloadSchedulerItem.DownloadID.Equals(downloadID))
                    {
                        downloadSchedulerItem.DownloadStatus = DownloadStatus.Completed;
                        downloadSchedulerItem.FinishedSize = progress.BytesTransferred;
                        downloadSchedulerItem.TotalSize = progress.BytesTotal;

                        DownloadCompleted?.Invoke(downloadID, downloadSchedulerItem);
                        CollectionCountChanged?.Invoke(DownloadSchedulerList.Count);
                        DownloadStorageService.AddDownloadData(downloadSchedulerItem);
                        break;
                    }
                }
            }
        }

        #endregion 第二部分：后台智能传输服务挂载的事件

        #region 第三部分：下载控制服务：自定义事件

        /// <summary>
        /// 集合的数量发生变化时修改任务栏徽标下载调度任务数量
        /// </summary>
        private static void OnCollectionCountChanged(int count)
        {
            // 当前下载任务数量发生变化时，更新当前下载任务数量通知
            if (badgeCount != count)
            {
                BadgeNotificationService.Show(count);
                badgeCount = count;
            }
        }

        #endregion 第三部分：下载控制服务：自定义事件

        /// <summary>
        /// 初始化后台下载调度器
        /// 先检查当前网络状态信息，加载暂停任务信息，然后初始化下载监控任务
        /// </summary>
        public static void InitializeDownloadScheduler()
        {
            // 获取当前下载引擎
            doEngineMode = DownloadOptionsService.DoEngineMode;

            // 加载所有未完成任务下载信息
            List<DownloadSchedulerModel> unfinishedDownloadTaskList = DownloadStorageService.QueryDownloadData();

            // 更新当前下载任务数量通知
            BadgeNotificationService.Show(badgeCount);

            // 挂载集合数量发生更改事件
            CollectionCountChanged += OnCollectionCountChanged;

            // 初始化下载服务
            if (doEngineMode.Equals(DownloadOptionsService.DoEngineModeList[0]))
            {
                DeliveryOptimizationService.InItialize();
                DeliveryOptimizationService.DownloadCreated += OnDeliveryOptimizationCreated;
                DeliveryOptimizationService.DownloadContinued += OnDeliveryOptimizationContinued;
                DeliveryOptimizationService.DownloadPaused += OnDeliveryOptimizationPaused;
                DeliveryOptimizationService.DownloadDeleted += OnDeliveryOptimizationDeleted;
                DeliveryOptimizationService.DownloadProgressing += OnDeliveryOptimizationProgressing;
                DeliveryOptimizationService.DownloadCompleted += OnDeliveryOptimizationCompleted;
            }
            else
            {
                BitsService.Initialize();
                BitsService.DownloadCreated += OnBitsCreated;
                BitsService.DownloadContinued += OnBitsContinued;
                BitsService.DownloadPaused += OnBitsPaused;
                BitsService.DownloadDeleted += OnBitsDeleted;
                BitsService.DownloadProgressing += OnBitsProgressing;
                BitsService.DownloadCompleted += OnBitsCompleted;
            }
        }

        /// <summary>
        /// 关闭下载监控任务
        /// </summary>
        public static void CloseDownloadScheduler()
        {
            // 卸载集合数量发生更改事件
            CollectionCountChanged -= OnCollectionCountChanged;

            // 注销下载服务
            if (doEngineMode.Equals(DownloadOptionsService.DoEngineModeList[0]))
            {
                DeliveryOptimizationService.DownloadCreated -= OnDeliveryOptimizationCreated;
                DeliveryOptimizationService.DownloadContinued -= OnDeliveryOptimizationContinued;
                DeliveryOptimizationService.DownloadPaused -= OnDeliveryOptimizationPaused;
                DeliveryOptimizationService.DownloadDeleted -= OnDeliveryOptimizationDeleted;
                DeliveryOptimizationService.DownloadProgressing -= OnDeliveryOptimizationProgressing;
                DeliveryOptimizationService.DownloadCompleted -= OnDeliveryOptimizationCompleted;
            }
            else
            {
                BitsService.DownloadCreated -= OnBitsCreated;
                BitsService.DownloadContinued -= OnBitsContinued;
                BitsService.DownloadPaused -= OnBitsPaused;
                BitsService.DownloadDeleted -= OnBitsDeleted;
                BitsService.DownloadProgressing -= OnBitsProgressing;
                BitsService.DownloadCompleted -= OnBitsCompleted;
            }
        }

        /// <summary>
        /// 添加下载任务
        /// </summary>
        public static void AddTask(DownloadSchedulerModel downloadSchedulerItem)
        {
            if (doEngineMode.Equals(DownloadOptionsService.DoEngineModeList[0]))
            {
                DeliveryOptimizationService.CreateDownload(downloadSchedulerItem.FileLink, downloadSchedulerItem.FilePath);
            }
            else
            {
                BitsService.CreateDownload(downloadSchedulerItem.FileLink, downloadSchedulerItem.FilePath);
            }
        }

        /// <summary>
        /// 继续下载任务
        /// </summary>
        public static void ContinueTask(DownloadSchedulerModel downloadSchedulerItem)
        {
            if (doEngineMode.Equals(DownloadOptionsService.DoEngineModeList[0]))
            {
                DeliveryOptimizationService.ContinueDownload(downloadSchedulerItem.DownloadID);
            }
            else
            {
                BitsService.ContinueDownload(downloadSchedulerItem.DownloadID);
            }
        }

        /// <summary>
        /// 暂停下载任务
        /// </summary>
        public static void PauseTask(DownloadSchedulerModel downloadSchedulerItem)
        {
            if (doEngineMode.Equals(DownloadOptionsService.DoEngineModeList[0]))
            {
                DeliveryOptimizationService.PauseDownload(downloadSchedulerItem.DownloadID);
            }
            else
            {
                BitsService.PauseDownload(downloadSchedulerItem.DownloadID);
            }
        }

        /// <summary>
        /// 暂停全部下载任务
        /// </summary>
        public static void PauseAllTask()
        {
            if (doEngineMode.Equals(DownloadOptionsService.DoEngineModeList[0]))
            {
                foreach (DownloadSchedulerModel downloadScheduleItem in DownloadSchedulerList)
                {
                    DeliveryOptimizationService.PauseDownload(downloadScheduleItem.DownloadID);
                }
            }
            else
            {
                foreach (DownloadSchedulerModel downloadScheduleItem in DownloadSchedulerList)
                {
                    BitsService.PauseDownload(downloadScheduleItem.DownloadID);
                }
            }
        }

        /// <summary>
        /// 删除下载任务
        /// </summary>
        public static void DeleteTask(DownloadSchedulerModel downloadSchedulerItem)
        {
            if (doEngineMode.Equals(DownloadOptionsService.DoEngineModeList[0]))
            {
                DeliveryOptimizationService.DeleteDownload(downloadSchedulerItem.DownloadID);
            }
            else
            {
                BitsService.DeleteDownload(downloadSchedulerItem.DownloadID);
            }
        }

        /// <summary>
        /// 获取当前下载调度的所有任务列表信息
        /// </summary>
        public static List<DownloadSchedulerModel> GetDownloadSchedulerLIst()
        {
            List<DownloadSchedulerModel> downloadSchedulerList = new List<DownloadSchedulerModel>();

            lock (downloadSchedulerLock)
            {
                try
                {
                    foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerList)
                    {
                        downloadSchedulerList.Add(downloadSchedulerItem);
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "Get download information failed", e);
                }
            }

            return downloadSchedulerList;
        }
    }
}
