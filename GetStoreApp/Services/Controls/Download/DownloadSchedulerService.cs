using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models.Controls.Download;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using GetStoreApp.Views.Windows;
using GetStoreApp.WindowsAPI.ComTypes;
using System;
using System.Collections.Generic;
using System.Threading;
using Windows.Foundation.Diagnostics;

namespace GetStoreApp.Services.Controls.Download
{
    /// <summary>
    /// 下载调度服务
    /// </summary>
    public static class DownloadSchedulerService
    {
        private static bool isInitialized;
        private static int badgeCount;
        private static KeyValuePair<string, string> doEngineMode;

        public static SemaphoreSlim DownloadSchedulerSemaphoreSlim { get; private set; } = new(1, 1);

        private static List<DownloadSchedulerModel> DownloadSchedulerList { get; } = [];

        public static event Action<Guid, DownloadSchedulerModel> DownloadCreated;

        public static event Action<Guid> DownloadContinued;

        public static event Action<Guid> DownloadPaused;

        public static event Action<Guid> DownloadDeleted;

        public static event Action<Guid, DownloadSchedulerModel> DownloadProgressing;

        public static event Action<Guid, DownloadSchedulerModel> DownloadCompleted;

        public static event Action<int> CollectionCountChanged;

        #region 第一部分：传递优化服务挂载的事件

        /// <summary>
        /// 传递优化：下载任务已创建事件
        /// </summary>
        private static void OnDeliveryOptimizationCreated(Guid downloadID, string fileName, string filePath, string url, double totalSize)
        {
            DownloadSchedulerSemaphoreSlim?.Wait();

            try
            {
                DownloadSchedulerModel downloadSchedulerItem = new()
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
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Deal OnDeliveryOptimizationCreated event failed", e);
            }
            finally
            {
                DownloadSchedulerSemaphoreSlim?.Release();
            }
        }

        /// <summary>
        /// 传递优化：下载任务已继续下载事件
        /// </summary>
        private static void OnDeliveryOptimizationContinued(Guid downloadID)
        {
            DownloadSchedulerSemaphoreSlim?.Wait();

            try
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
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Deal OnDeliveryOptimizationContinued event failed", e);
            }
            finally
            {
                DownloadSchedulerSemaphoreSlim?.Release();
            }
        }

        /// <summary>
        /// 传递优化：下载任务已暂停下载事件
        /// </summary>
        private static void OnDeliveryOptimizationPaused(Guid downloadID)
        {
            DownloadSchedulerSemaphoreSlim?.Wait();

            try
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
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Deal OnDeliveryOptimizationPaused event failed", e);
            }
            finally
            {
                DownloadSchedulerSemaphoreSlim?.Release();
            }
        }

        /// <summary>
        /// 传递优化：下载任务已删除事件
        /// </summary>
        private static void OnDeliveryOptimizationDeleted(Guid downloadID)
        {
            DownloadSchedulerSemaphoreSlim?.Wait();

            try
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
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Deal OnDeliveryOptimizationDeleted event failed", e);
            }
            finally
            {
                DownloadSchedulerSemaphoreSlim?.Release();
            }
        }

        /// <summary>
        /// 传递优化：下载任务下载进度发生变化事件
        /// </summary>
        private static void OnDeliveryOptimizationProgressing(Guid downloadID, DO_DOWNLOAD_STATUS status)
        {
            DownloadSchedulerSemaphoreSlim?.Wait();

            try
            {
                foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerList)
                {
                    if (downloadSchedulerItem.DownloadID.Equals(downloadID))
                    {
                        downloadSchedulerItem.DownloadStatus = DownloadStatus.Downloading;
                        downloadSchedulerItem.CurrentSpeed = Convert.ToDouble(status.BytesTransferred) - downloadSchedulerItem.FinishedSize;
                        downloadSchedulerItem.FinishedSize = status.BytesTransferred;
                        downloadSchedulerItem.TotalSize = status.BytesTotal;

                        DownloadProgressing?.Invoke(downloadID, downloadSchedulerItem);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Deal OnDeliveryOptimizationProgressing event failed", e);
            }
            finally
            {
                DownloadSchedulerSemaphoreSlim?.Release();
            }
        }

        /// <summary>
        /// 传递优化：下载任务已下载完成事件
        /// </summary>
        private static void OnDeliveryOptimizationCompleted(Guid downloadID, DO_DOWNLOAD_STATUS status)
        {
            DownloadSchedulerSemaphoreSlim?.Wait();

            try
            {
                foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerList)
                {
                    if (downloadSchedulerItem.DownloadID.Equals(downloadID))
                    {
                        downloadSchedulerItem.DownloadStatus = DownloadStatus.Completed;
                        downloadSchedulerItem.CurrentSpeed = Convert.ToDouble(status.BytesTransferred) - downloadSchedulerItem.FinishedSize;
                        downloadSchedulerItem.FinishedSize = status.BytesTransferred;
                        downloadSchedulerItem.TotalSize = status.BytesTotal;

                        DownloadCompleted?.Invoke(downloadID, downloadSchedulerItem);
                        DownloadStorageService.AddDownloadData(downloadSchedulerItem);
                        DownloadSchedulerList.Remove(downloadSchedulerItem);
                        CollectionCountChanged?.Invoke(DownloadSchedulerList.Count);
                        break;
                    }
                }

                if (DownloadSchedulerList.Count is 0)
                {
                    MainWindow.Current?.DispatcherQueue.TryEnqueue(() =>
                        {
                            ToastNotificationService.Show(NotificationKind.DownloadCompleted);
                        });
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Deal OnDeliveryOptimizationCompleted event failed", e);
            }
            finally
            {
                DownloadSchedulerSemaphoreSlim?.Release();
            }
        }

        #endregion 第一部分：传递优化服务挂载的事件

        #region 第二部分：后台智能传输服务挂载的事件

        /// <summary>
        /// 后台智能传输任务：下载任务已创建事件
        /// </summary>
        private static void OnBitsCreated(Guid downloadID, string fileName, string filePath, string url, double totalSize)
        {
            DownloadSchedulerSemaphoreSlim?.Wait();

            try
            {
                DownloadSchedulerModel downloadSchedulerItem = new()
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
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Deal OnBitsCreated event failed", e);
            }
            finally
            {
                DownloadSchedulerSemaphoreSlim?.Release();
            }
        }

        /// <summary>
        /// 后台智能传输任务：下载任务已继续下载事件
        /// </summary>
        private static void OnBitsContinued(Guid downloadID)
        {
            DownloadSchedulerSemaphoreSlim?.Wait();

            try
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
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Deal OnBitsContinued event failed", e);
            }
            finally
            {
                DownloadSchedulerSemaphoreSlim?.Release();
            }
        }

        /// <summary>
        /// 后台智能传输任务：下载任务已暂停下载事件
        /// </summary>
        private static void OnBitsPaused(Guid downloadID)
        {
            DownloadSchedulerSemaphoreSlim?.Wait();

            try
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
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Deal OnBitsPaused event failed", e);
            }
            finally
            {
                DownloadSchedulerSemaphoreSlim?.Release();
            }
        }

        /// <summary>
        /// 后台智能传输任务：下载任务已删除事件
        /// </summary>
        private static void OnBitsDeleted(Guid downloadID)
        {
            DownloadSchedulerSemaphoreSlim?.Wait();

            try
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
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Deal OnBitsDeleted event failed", e);
            }
            finally
            {
                DownloadSchedulerSemaphoreSlim?.Release();
            }
        }

        /// <summary>
        /// 后台智能传输任务：下载任务下载进度发生变化事件
        /// </summary>
        private static void OnBitsProgressing(Guid downloadID, BG_JOB_PROGRESS progress)
        {
            DownloadSchedulerSemaphoreSlim?.Wait();

            try
            {
                foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerList)
                {
                    if (downloadSchedulerItem.DownloadID.Equals(downloadID))
                    {
                        downloadSchedulerItem.DownloadStatus = DownloadStatus.Downloading;
                        downloadSchedulerItem.CurrentSpeed = Convert.ToDouble(progress.BytesTransferred) - downloadSchedulerItem.FinishedSize;
                        downloadSchedulerItem.FinishedSize = progress.BytesTransferred;
                        downloadSchedulerItem.TotalSize = progress.BytesTotal;

                        DownloadProgressing?.Invoke(downloadID, downloadSchedulerItem);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Deal OnBitsProgressing event failed", e);
            }
            finally
            {
                DownloadSchedulerSemaphoreSlim?.Release();
            }
        }

        /// <summary>
        /// 后台智能传输任务：下载任务已下载完成事件
        /// </summary>
        private static void OnBitsCompleted(Guid downloadID, BG_JOB_PROGRESS progress)
        {
            DownloadSchedulerSemaphoreSlim?.Wait();

            try
            {
                foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerList)
                {
                    if (downloadSchedulerItem.DownloadID.Equals(downloadID))
                    {
                        downloadSchedulerItem.DownloadStatus = DownloadStatus.Completed;
                        downloadSchedulerItem.CurrentSpeed = Convert.ToDouble(progress.BytesTransferred) - downloadSchedulerItem.FinishedSize;
                        downloadSchedulerItem.FinishedSize = progress.BytesTransferred;
                        downloadSchedulerItem.TotalSize = progress.BytesTotal;

                        DownloadCompleted?.Invoke(downloadID, downloadSchedulerItem);
                        DownloadStorageService.AddDownloadData(downloadSchedulerItem);
                        DownloadSchedulerList.Remove(downloadSchedulerItem);
                        CollectionCountChanged?.Invoke(DownloadSchedulerList.Count);
                        break;
                    }
                }

                if (DownloadSchedulerList.Count is 0)
                {
                    MainWindow.Current?.DispatcherQueue.TryEnqueue(() =>
                    {
                        ToastNotificationService.Show(NotificationKind.DownloadCompleted);
                    });
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Deal OnBitsCompleted event failed", e);
            }
            finally
            {
                DownloadSchedulerSemaphoreSlim?.Release();
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
        public static void InitializeDownloadScheduler(bool isDesktopProgram)
        {
            if (!isInitialized)
            {
                isInitialized = true;

                // 获取当前下载引擎
                doEngineMode = DownloadOptionsService.DoEngineMode;

                // 更新当前下载任务数量通知
                BadgeNotificationService.Show(badgeCount);

                // 挂载集合数量发生更改事件
                if (isDesktopProgram)
                {
                    CollectionCountChanged += OnCollectionCountChanged;
                }

                // 初始化下载服务
                if (doEngineMode.Equals(DownloadOptionsService.DoEngineModeList[0]))
                {
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
        }

        /// <summary>
        /// 关闭下载监控任务
        /// </summary>
        public static void CloseDownloadScheduler(bool isDesktopProgram)
        {
            if (isInitialized)
            {
                isInitialized = false;

                DownloadSchedulerSemaphoreSlim?.Dispose();
                DownloadSchedulerSemaphoreSlim = null;

                // 卸载集合数量发生更改事件
                if (isDesktopProgram)
                {
                    CollectionCountChanged -= OnCollectionCountChanged;
                }

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
        }

        /// <summary>
        /// 创建下载任务
        /// </summary>
        public static void CreateDownload(string fileLink, string filePath)
        {
            if (doEngineMode.Equals(DownloadOptionsService.DoEngineModeList[0]))
            {
                DeliveryOptimizationService.CreateDownload(fileLink, filePath);
            }
            else
            {
                BitsService.CreateDownload(fileLink, filePath);
            }
        }

        /// <summary>
        /// 继续下载任务
        /// </summary>
        public static void ContinueDownload(Guid downloadID)
        {
            if (doEngineMode.Equals(DownloadOptionsService.DoEngineModeList[0]))
            {
                DeliveryOptimizationService.ContinueDownload(downloadID);
            }
            else
            {
                BitsService.ContinueDownload(downloadID);
            }
        }

        /// <summary>
        /// 暂停下载任务
        /// </summary>
        public static void PauseDownload(Guid downloadID)
        {
            if (doEngineMode.Equals(DownloadOptionsService.DoEngineModeList[0]))
            {
                DeliveryOptimizationService.PauseDownload(downloadID);
            }
            else
            {
                BitsService.PauseDownload(downloadID);
            }
        }

        /// <summary>
        /// 删除下载任务
        /// </summary>
        public static void DeleteDownload(Guid downloadID)
        {
            if (doEngineMode.Equals(DownloadOptionsService.DoEngineModeList[0]))
            {
                DeliveryOptimizationService.DeleteDownload(downloadID);
            }
            else
            {
                BitsService.DeleteDownload(downloadID);
            }
        }

        /// <summary>
        /// 终止所有下载任务，仅用于应用关闭时
        /// </summary>
        public static void TerminateDownload()
        {
            if (doEngineMode.Equals(DownloadOptionsService.DoEngineModeList[0]))
            {
                DeliveryOptimizationService.TerminateDownload();
            }
            else
            {
                BitsService.TerminateDownload();
            }
        }

        /// <summary>
        /// 获取当前下载调度的所有任务列表信息，为保证安全访问，需要手动对访问的锁进行加锁和释放
        /// </summary>
        public static List<DownloadSchedulerModel> GetDownloadSchedulerList()
        {
            List<DownloadSchedulerModel> downloadSchedulerList = [];

            if (DownloadSchedulerSemaphoreSlim?.CurrentCount is 0)
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
