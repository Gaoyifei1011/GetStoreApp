using GetStoreApp.Extensions.DataType.Classes;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;
using System.Threading;

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

        public static bool IsDownloadingPageInitialized { get; set; }

        public static SemaphoreSlim DownloadSchedulerSemaphoreSlim { get; private set; } = new(1, 1);

        public static List<DownloadSchedulerModel> DownloadSchedulerList { get; } = [];

        public static List<DownloadSchedulerModel> DownloadFailedList { get; } = [];

        public static event Action<DownloadSchedulerModel> DownloadProgress;

        /// <summary>
        /// 下载状态发生改变时触发的事件
        /// </summary>
        private static void OnDownloadProgress(DownloadProgress downloadProgress)
        {
            // 处于等待中（新添加下载任务或者已经恢复下载）
            if (downloadProgress.DownloadProgressState is DownloadProgressState.Queued)
            {
                DownloadSchedulerSemaphoreSlim?.Wait();

                try
                {
                    // 下载任务已经存在，更新下载状态
                    foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerList)
                    {
                        if (Equals(downloadSchedulerItem.DownloadID, downloadProgress.DownloadID))
                        {
                            downloadSchedulerItem.DownloadProgressState = downloadProgress.DownloadProgressState;
                            DownloadProgress?.Invoke(new DownloadSchedulerModel()
                            {
                                DownloadID = downloadSchedulerItem.DownloadID,
                                FileName = downloadSchedulerItem.FileName,
                                FilePath = downloadSchedulerItem.FilePath,
                                DownloadProgressState = downloadSchedulerItem.DownloadProgressState,
                                CompletedSize = downloadSchedulerItem.CompletedSize,
                                TotalSize = downloadSchedulerItem.TotalSize,
                                DownloadSpeed = downloadSchedulerItem.DownloadSpeed,
                            });
                            return;
                        }
                    }

                    // 不存在则添加任务
                    DownloadSchedulerModel downloadScheduler = new()
                    {
                        DownloadID = downloadProgress.DownloadID,
                        DownloadProgressState = downloadProgress.DownloadProgressState,
                        FileName = downloadProgress.FileName,
                        FilePath = downloadProgress.FilePath,
                        CompletedSize = downloadProgress.CompletedSize,
                        TotalSize = downloadProgress.TotalSize,
                        DownloadSpeed = downloadProgress.DownloadSpeed,
                    };

                    DownloadSchedulerList.Add(downloadScheduler);
                    UpdateBadgeNotification(DownloadSchedulerList.Count, true);
                    DownloadProgress?.Invoke(new DownloadSchedulerModel()
                    {
                        DownloadID = downloadScheduler.DownloadID,
                        FileName = downloadScheduler.FileName,
                        FilePath = downloadScheduler.FilePath,
                        DownloadProgressState = downloadScheduler.DownloadProgressState,
                        CompletedSize = downloadScheduler.CompletedSize,
                        TotalSize = downloadScheduler.TotalSize,
                        DownloadSpeed = downloadScheduler.DownloadSpeed,
                    });
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
                finally
                {
                    DownloadSchedulerSemaphoreSlim?.Release();
                }
            }
            // 下载任务正在下载中
            else if (downloadProgress.DownloadProgressState is DownloadProgressState.Downloading)
            {
                DownloadSchedulerSemaphoreSlim?.Wait();

                try
                {
                    foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerList)
                    {
                        if (Equals(downloadSchedulerItem.DownloadID, downloadProgress.DownloadID))
                        {
                            downloadSchedulerItem.DownloadProgressState = downloadProgress.DownloadProgressState;
                            downloadSchedulerItem.DownloadSpeed = downloadProgress.CompletedSize - downloadSchedulerItem.CompletedSize;
                            downloadSchedulerItem.CompletedSize = downloadProgress.CompletedSize;
                            downloadSchedulerItem.TotalSize = downloadProgress.TotalSize;
                            DownloadProgress?.Invoke(new DownloadSchedulerModel()
                            {
                                DownloadID = downloadSchedulerItem.DownloadID,
                                FileName = downloadSchedulerItem.FileName,
                                FilePath = downloadSchedulerItem.FilePath,
                                DownloadProgressState = downloadSchedulerItem.DownloadProgressState,
                                CompletedSize = downloadSchedulerItem.CompletedSize,
                                TotalSize = downloadSchedulerItem.TotalSize,
                                DownloadSpeed = downloadSchedulerItem.DownloadSpeed,
                            });
                            return;
                        }
                    }
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
                finally
                {
                    DownloadSchedulerSemaphoreSlim?.Release();
                }
            }
            // 下载任务已暂停
            else if (downloadProgress.DownloadProgressState is DownloadProgressState.Paused)
            {
                DownloadSchedulerSemaphoreSlim?.Wait();

                try
                {
                    foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerList)
                    {
                        if (Equals(downloadSchedulerItem.DownloadID, downloadProgress.DownloadID))
                        {
                            downloadSchedulerItem.DownloadProgressState = downloadProgress.DownloadProgressState;
                            DownloadProgress?.Invoke(new DownloadSchedulerModel()
                            {
                                DownloadID = downloadSchedulerItem.DownloadID,
                                FileName = downloadSchedulerItem.FileName,
                                FilePath = downloadSchedulerItem.FilePath,
                                DownloadProgressState = downloadSchedulerItem.DownloadProgressState,
                                CompletedSize = downloadSchedulerItem.CompletedSize,
                                TotalSize = downloadSchedulerItem.TotalSize,
                                DownloadSpeed = downloadSchedulerItem.DownloadSpeed,
                            });
                            return;
                        }
                    }
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
                finally
                {
                    DownloadSchedulerSemaphoreSlim?.Release();
                }
            }
            // 下载任务已失败
            else if (downloadProgress.DownloadProgressState is DownloadProgressState.Failed)
            {
                DownloadSchedulerSemaphoreSlim?.Wait();

                try
                {
                    foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerList)
                    {
                        if (Equals(downloadSchedulerItem.DownloadID, downloadProgress.DownloadID))
                        {
                            downloadSchedulerItem.DownloadProgressState = downloadProgress.DownloadProgressState;
                            downloadSchedulerItem.CompletedSize = 1;
                            downloadProgress.TotalSize = 1;
                            DownloadProgress?.Invoke(new DownloadSchedulerModel()
                            {
                                DownloadID = downloadSchedulerItem.DownloadID,
                                FileName = downloadSchedulerItem.FileName,
                                FilePath = downloadSchedulerItem.FilePath,
                                DownloadProgressState = downloadSchedulerItem.DownloadProgressState,
                                CompletedSize = downloadSchedulerItem.CompletedSize,
                                TotalSize = downloadSchedulerItem.TotalSize,
                                DownloadSpeed = downloadSchedulerItem.DownloadSpeed,
                            });

                            // 下载页面未初始化前将下载内容存放到下载失败记录列表中
                            if (!IsDownloadingPageInitialized)
                            {
                                DownloadFailedList.Add(new DownloadSchedulerModel()
                                {
                                    DownloadID = downloadSchedulerItem.DownloadID,
                                    FileName = downloadSchedulerItem.FileName,
                                    FilePath = downloadSchedulerItem.FilePath,
                                    DownloadProgressState = downloadSchedulerItem.DownloadProgressState,
                                    CompletedSize = downloadSchedulerItem.CompletedSize,
                                    TotalSize = downloadSchedulerItem.TotalSize,
                                    DownloadSpeed = downloadSchedulerItem.DownloadSpeed,
                                });
                            }
                            DownloadSchedulerList.Remove(downloadSchedulerItem);
                            UpdateBadgeNotification(DownloadSchedulerList.Count, true);
                            return;
                        }
                    }
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
                finally
                {
                    DownloadSchedulerSemaphoreSlim?.Release();
                }
            }
            // 下载任务已完成
            else if (downloadProgress.DownloadProgressState is DownloadProgressState.Finished)
            {
                DownloadSchedulerSemaphoreSlim?.Wait();

                try
                {
                    foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerList)
                    {
                        if (Equals(downloadSchedulerItem.DownloadID, downloadProgress.DownloadID))
                        {
                            downloadSchedulerItem.DownloadProgressState = downloadProgress.DownloadProgressState;
                            downloadSchedulerItem.DownloadSpeed = downloadProgress.CompletedSize - downloadSchedulerItem.CompletedSize;
                            downloadSchedulerItem.CompletedSize = downloadProgress.CompletedSize;
                            downloadSchedulerItem.TotalSize = downloadProgress.TotalSize;
                            DownloadProgress?.Invoke(new DownloadSchedulerModel()
                            {
                                DownloadID = downloadSchedulerItem.DownloadID,
                                FileName = downloadSchedulerItem.FileName,
                                FilePath = downloadSchedulerItem.FilePath,
                                DownloadProgressState = downloadSchedulerItem.DownloadProgressState,
                                CompletedSize = downloadSchedulerItem.CompletedSize,
                                TotalSize = downloadSchedulerItem.TotalSize,
                                DownloadSpeed = downloadSchedulerItem.DownloadSpeed,
                            });
                            DownloadStorageService.AddDownloadData(downloadSchedulerItem);
                            DownloadSchedulerList.Remove(downloadSchedulerItem);
                            UpdateBadgeNotification(DownloadSchedulerList.Count, true);
                            return;
                        }
                    }
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
                finally
                {
                    DownloadSchedulerSemaphoreSlim?.Release();
                }
            }
            // 下载任务已删除
            else if (downloadProgress.DownloadProgressState is DownloadProgressState.Deleted)
            {
                DownloadSchedulerSemaphoreSlim?.Wait();

                try
                {
                    foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerList)
                    {
                        if (Equals(downloadSchedulerItem.DownloadID, downloadProgress.DownloadID))
                        {
                            downloadSchedulerItem.DownloadProgressState = downloadProgress.DownloadProgressState;
                            DownloadProgress?.Invoke(new DownloadSchedulerModel()
                            {
                                DownloadID = downloadSchedulerItem.DownloadID,
                                FileName = downloadSchedulerItem.FileName,
                                FilePath = downloadSchedulerItem.FilePath,
                                DownloadProgressState = downloadSchedulerItem.DownloadProgressState,
                                CompletedSize = downloadSchedulerItem.CompletedSize,
                                TotalSize = downloadSchedulerItem.TotalSize,
                                DownloadSpeed = downloadSchedulerItem.DownloadSpeed,
                            });
                            DownloadSchedulerList.Remove(downloadSchedulerItem);
                            UpdateBadgeNotification(DownloadSchedulerList.Count, false);
                            return;
                        }
                    }
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
                finally
                {
                    DownloadSchedulerSemaphoreSlim?.Release();
                }
            }
        }

        /// <summary>
        /// 集合的数量发生变化时修改任务栏徽标下载调度任务数量
        /// </summary>
        private static void UpdateBadgeNotification(int count, bool needNotification)
        {
            // 当前下载任务数量发生变化时，更新当前下载任务数量通知
            if (badgeCount != count)
            {
                BadgeNotificationService.Show(count);
                badgeCount = count;
            }

            if (DownloadSchedulerList.Count is 0 && needNotification)
            {
                // 显示下载文件完成通知
                AppNotificationBuilder appNotificationBuilder = new();
                appNotificationBuilder.AddArgument("action", "OpenApp");
                appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/DownloadCompleted1"));
                appNotificationBuilder.AddText(ResourceService.GetLocalized("Notification/DownloadCompleted2"));
                AppNotificationButton viewDownloadPageButton = new(ResourceService.GetLocalized("Notification/ViewDownloadPage"));
                viewDownloadPageButton.Arguments.Add("action", "ViewDownloadPage");
                appNotificationBuilder.AddButton(viewDownloadPageButton);
                AppNotification appNotification = appNotificationBuilder.BuildNotification();
                ToastNotificationService.Show(appNotification);
            }
        }

        /// <summary>
        /// 初始化后台下载调度器
        /// </summary>
        public static void InitializeDownloadScheduler()
        {
            if (!isInitialized)
            {
                isInitialized = true;

                // 获取当前下载引擎
                doEngineMode = DownloadOptionsService.DoEngineMode;

                // 更新当前下载任务数量通知
                BadgeNotificationService.Show(badgeCount);

                // 初始化下载服务
                if (Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[0]))
                {
                    DeliveryOptimizationService.DownloadProgress += OnDownloadProgress;
                }
                else if (Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[1]))
                {
                    BitsService.Initialize();
                    BitsService.DownloadProgress += OnDownloadProgress;
                }
                else if (Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[2]))
                {
                    Aria2Service.InitializeAria2Conf();
                    Aria2Service.Initialize();
                    Aria2Service.DownloadProgress += OnDownloadProgress;
                }
            }
        }

        /// <summary>
        /// 关闭下载监控任务
        /// </summary>
        public static void CloseDownloadScheduler()
        {
            if (isInitialized)
            {
                isInitialized = false;

                DownloadSchedulerSemaphoreSlim?.Dispose();
                DownloadSchedulerSemaphoreSlim = null;

                // 注销下载服务
                if (Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[0]))
                {
                    DeliveryOptimizationService.TerminateDownload();
                    DeliveryOptimizationService.DownloadProgress -= OnDownloadProgress;
                }
                else if (Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[1]))
                {
                    BitsService.TerminateDownload();
                    BitsService.DownloadProgress -= OnDownloadProgress;
                }
                else if (Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[2]))
                {
                    Aria2Service.Release();
                    Aria2Service.DownloadProgress -= OnDownloadProgress;
                }
            }
        }

        /// <summary>
        /// 创建下载任务
        /// </summary>
        public static void CreateDownload(string fileLink, string filePath)
        {
            if (Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[0]))
            {
                DeliveryOptimizationService.CreateDownload(fileLink, filePath);
            }
            else if (Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[1]))
            {
                BitsService.CreateDownload(fileLink, filePath);
            }
            else if (Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[2]))
            {
                Aria2Service.CreateDownload(fileLink, filePath);
            }
        }

        /// <summary>
        /// 继续下载任务
        /// </summary>
        public static void ContinueDownload(string downloadID)
        {
            if (Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[0]))
            {
                DeliveryOptimizationService.ContinueDownload(downloadID);
            }
            else if (Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[1]))
            {
                BitsService.ContinueDownload(downloadID);
            }
            else if (Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[2]))
            {
                Aria2Service.ContinueDownload(downloadID);
            }
        }

        /// <summary>
        /// 暂停下载任务
        /// </summary>
        public static void PauseDownload(string downloadID)
        {
            if (Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[0]))
            {
                DeliveryOptimizationService.PauseDownload(downloadID);
            }
            else if (Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[1]))
            {
                BitsService.PauseDownload(downloadID);
            }
            else if (Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[2]))
            {
                Aria2Service.PauseDownload(downloadID);
            }
        }

        /// <summary>
        /// 删除下载任务
        /// </summary>
        public static void DeleteDownload(string downloadID)
        {
            if (Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[0]))
            {
                DeliveryOptimizationService.DeleteDownload(downloadID);
            }
            else if (Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[1]))
            {
                BitsService.DeleteDownload(downloadID);
            }
            else if (Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[2]))
            {
                Aria2Service.DeleteDownload(downloadID);
            }
        }

        /// <summary>
        /// 应用关闭时终止所有下载任务
        /// </summary>
        public static void TerminateDownload()
        {
            if (Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[0]))
            {
                DeliveryOptimizationService.TerminateDownload();
            }
            else if (Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[1]))
            {
                BitsService.TerminateDownload();
            }
            else if (Equals(doEngineMode, DownloadOptionsService.DoEngineModeList[2]))
            {
                Aria2Service.Release();
            }
        }
    }
}
