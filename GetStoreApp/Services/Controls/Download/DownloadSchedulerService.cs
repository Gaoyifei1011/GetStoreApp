using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Download;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;
using Windows.System.Threading;

namespace GetStoreApp.Services.Controls.Download
{
    /// <summary>
    /// 下载调度服务
    /// </summary>
    public static class DownloadSchedulerService
    {
        private static readonly object DownloadSchedulerLock = new object();
        private static readonly object IsNetWorkConnectedLock = new object();

        private static bool IsUpdatingNow = false;
        private static bool IsNetWorkConnected = true;

        private static ThreadPoolTimer DownloadSchedulerTimer;

        // 下载中任务列表（带通知）
        public static ObservableCollection<BackgroundModel> DownloadingCollection { get; } = new ObservableCollection<BackgroundModel>();

        // 等待下载任务列表（带通知）
        public static ObservableCollection<BackgroundModel> WaitingCollection { get; } = new ObservableCollection<BackgroundModel>();

        /// <summary>
        /// 先获取当前网络状态信息，然后初始化下载监控任务
        /// </summary>
        public static void InitializeDownloadScheduler()
        {
            // 查看是否开启了网络监控服务
            if (NetWorkMonitorService.NetWorkMonitorValue)
            {
                if (!NetWorkHelper.IsNetworkConnected(out bool checkFailed))
                {
                    if (!checkFailed)
                    {
                        IsNetWorkConnected = false;
                        ToastNotificationService.Show(NotificationKind.DownloadAborted, "NotDownload");
                    }
                }
            }

            DownloadSchedulerTimer = ThreadPoolTimer.CreatePeriodicTimer(DownloadSchedulerTimerElapsed, TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// 关闭下载监控任务
        /// </summary>
        public static void CloseDownloadScheduler()
        {
            DownloadSchedulerTimer.Cancel();
        }

        /// <summary>
        /// 添加下载任务
        /// </summary>
        public static async Task<bool> AddTaskAsync(BackgroundModel backgroundItem, string operation)
        {
            // 尝试进入写模式，该模式必须等待
            while (IsUpdatingNow) await Task.Delay(50);
            lock (DownloadSchedulerLock) IsUpdatingNow = true;

            bool Result = false;

            if (operation is "Add")
            {
                // 在数据库中添加下载信息，并获取添加成功的结果
                bool AddResult = await DownloadXmlService.AddAsync(backgroundItem);

                // 数据库添加成功后添加等待下载任务
                if (AddResult)
                {
                    WaitingCollection.Add(backgroundItem);
                    Result = true;
                }
            }
            // 存在重复的下载记录
            else if (operation is "Update")
            {
                bool UpdateResult = await DownloadXmlService.UpdateFlagAsync(backgroundItem.DownloadKey, 1);

                // 数据库更新成功后添加等待下载任务
                if (UpdateResult)
                {
                    WaitingCollection.Add(backgroundItem);
                    Result = true;
                }
            }

            // 信息更新完毕时，退出写模式，让其他线程执行操作
            lock (DownloadSchedulerLock) IsUpdatingNow = false;

            return Result;
        }

        /// <summary>
        /// 继续下载任务
        /// </summary>
        public static async Task<bool> ContinueTaskAsync(BackgroundModel downloadItem)
        {
            // 尝试进入写模式，该模式必须等待
            while (IsUpdatingNow) await Task.Delay(50);
            lock (DownloadSchedulerLock) IsUpdatingNow = true;

            // 将继续下载的任务状态标记为等待下载状态
            bool UpdateResult = await DownloadXmlService.UpdateFlagAsync(downloadItem.DownloadKey, 1);

            // 数据库添加成功后添加等待下载任务
            if (UpdateResult)
            {
                WaitingCollection.Add(downloadItem);
            }

            // 信息更新完毕时，退出写模式，让其他线程执行操作
            lock (DownloadSchedulerLock) IsUpdatingNow = false;

            return UpdateResult;
        }

        /// <summary>
        /// 暂停下载任务
        /// </summary>
        public static async Task<bool> PauseTaskAsync(string downloadKey, string gID, int downloadFlag)
        {
            // 尝试进入写模式，该模式必须等待
            while (IsUpdatingNow) await Task.Delay(50);
            lock (DownloadSchedulerLock) IsUpdatingNow = true;

            bool Result = true;

            // 处于等待下载状态时，从等待下载列表中移除
            if (downloadFlag is 1)
            {
                try
                {
                    WaitingCollection.Remove(WaitingCollection.First(item => item.DownloadKey == downloadKey));
                    Result = await DownloadXmlService.UpdateFlagAsync(downloadKey, 2);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Warning, "Pause waiting list task failed.", e);
                    Result = false;
                }
            }

            // 处于正在下载状态时，从正在下载列表中移除
            else if (downloadFlag is 3)
            {
                // 从下载进程中移除正在下载的任务
                (bool, string) DeleteResult = await Aria2Service.PauseAsync(gID);

                if (DeleteResult.Item1)
                {
                    try
                    {
                        DownloadingCollection.Remove(DownloadingCollection.First(item => item.DownloadKey == downloadKey));
                        Result = await DownloadXmlService.UpdateFlagAsync(downloadKey, 2);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Warning, "Pause downloading list task failed.", e);
                        Result = false;
                    }
                }
                else
                {
                    Result = false;
                }
            }

            // 信息更新完毕时，退出写模式，让其他线程执行操作
            lock (DownloadSchedulerLock) IsUpdatingNow = false;

            return Result;
        }

        /// <summary>
        /// 暂停全部下载任务
        /// </summary>
        public static async Task PauseAllTaskAsync()
        {
            // 尝试进入写模式，该模式必须等待
            while (IsUpdatingNow) await Task.Delay(50);
            lock (DownloadSchedulerLock) IsUpdatingNow = true;

            // 从正在下载列表中暂停所有正在下载任务
            foreach (BackgroundModel backgroundItem in DownloadingCollection)
            {
                // 从下载进程中移除正在下载的任务
                (bool, string) DeleteResult = await Aria2Service.PauseAsync(backgroundItem.GID);

                if (DeleteResult.Item1)
                {
                    try
                    {
                        await DownloadXmlService.UpdateFlagAsync(backgroundItem.DownloadKey, 2);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Warning, "Pause all downloading list task failed.", e);
                        continue;
                    }
                }
            }

            // 从等待下载列表中暂停所有等待下载任务
            foreach (BackgroundModel backgroundItem in WaitingCollection)
            {
                try
                {
                    await DownloadXmlService.UpdateFlagAsync(backgroundItem.DownloadKey, 2);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Warning, "Pause all waiting list task failed.", e);
                    continue;
                }
            }

            // 清空所有正在下载和等待下载的列表内容
            while (DownloadingCollection.Count > 0) DownloadingCollection.RemoveAt(0);
            while (WaitingCollection.Count > 0) WaitingCollection.RemoveAt(0);

            // 信息更新完毕时，退出写模式，让其他线程执行操作
            lock (DownloadSchedulerLock) IsUpdatingNow = false;
        }

        /// <summary>
        /// 删除下载任务
        /// </summary>
        public static async Task<bool> DeleteTaskAsync(string downloadKey, string gID, int downloadFlag)
        {
            // 尝试进入写模式，该模式必须等待
            while (IsUpdatingNow) await Task.Delay(50);
            lock (DownloadSchedulerLock) IsUpdatingNow = true;

            bool Result = true;

            // 处于等待下载状态时，从等待下载列表中移除
            if (downloadFlag is 1)
            {
                try
                {
                    List<BackgroundModel> waitingList = WaitingCollection.Where(item => item.DownloadKey == downloadKey).ToList();
                    foreach (BackgroundModel backgroundItem in waitingList)
                    {
                        WaitingCollection.Remove(backgroundItem);
                    }

                    Result = await DownloadXmlService.DeleteAsync(downloadKey);
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Warning, "Delete waiting list task failed.", e);
                    Result = false;
                }
            }

            // 处于正在下载状态时，从正在下载列表中移除
            else if (downloadFlag is 3)
            {
                // 从下载进程中移除正在下载的任务
                (bool, string) DeleteResult = await Aria2Service.DeleteAsync(gID);

                if (DeleteResult.Item1)
                {
                    try
                    {
                        List<BackgroundModel> downloadingList = DownloadingCollection.Where(item => item.DownloadKey == downloadKey).ToList();
                        foreach (BackgroundModel backgroundItem in downloadingList)
                        {
                            DownloadingCollection.Remove(backgroundItem);
                        }

                        Result = await DownloadXmlService.DeleteAsync(downloadKey);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Warning, "Delete downloading list task failed.", e);
                        Result = false;
                    }
                }
                else
                {
                    Result = false;
                }
            }

            // 信息更新完毕时，退出写模式，让其他线程执行操作
            lock (DownloadSchedulerLock) IsUpdatingNow = false;

            return Result;
        }

        /// <summary>
        /// 定时计划添加下载任务，更新下载任务信息
        /// </summary>
        private static async void DownloadSchedulerTimerElapsed(ThreadPoolTimer timer)
        {
            // 查看是否开启了网络监控服务
            if (NetWorkMonitorService.NetWorkMonitorValue)
            {
                await ScheduledGetNetWorkAsync();
            }

            // 尝试进入写模式，该模式必须等待
            while (IsUpdatingNow) await Task.Delay(50);
            lock (DownloadSchedulerLock) IsUpdatingNow = true;

            await ScheduledUpdateStatusAsync();

            await ScheduledAddTaskAsync();

            // 信息更新完毕时，退出写模式，让其他线程执行操作
            lock (DownloadSchedulerLock) IsUpdatingNow = false;
        }

        /// <summary>
        /// 定时检查网络是否处于连接状态
        /// </summary>
        private static async Task ScheduledGetNetWorkAsync()
        {
            // 网络处于未连接状态，暂停所有任务
            if (!NetWorkHelper.IsNetworkConnected(out bool checkFailed))
            {
                if (!checkFailed)
                {
                    // 如果网络处于正在连接状态，修改当前网络状态并发送通知
                    if (IsNetWorkConnected)
                    {
                        lock (IsNetWorkConnectedLock)
                        {
                            IsNetWorkConnected = false;
                        }

                        // 发送通知
                        if (DownloadingCollection.Any() || WaitingCollection.Any())
                        {
                            ToastNotificationService.Show(NotificationKind.DownloadAborted, "DownloadingNow");
                        }
                        else
                        {
                            ToastNotificationService.Show(NotificationKind.DownloadAborted, "NotDownload");
                        }
                    }

                    // 暂停所有下载任务
                    await PauseAllTaskAsync();
                }
            }
            else
            {
                // 如果网络处于断线状态，修改当前网络状态
                if (!IsNetWorkConnected)
                {
                    lock (IsNetWorkConnectedLock)
                    {
                        IsNetWorkConnected = true;
                    }
                }
            }
        }

        /// <summary>
        /// 定时计划添加下载任务
        /// </summary>
        private static async Task ScheduledAddTaskAsync()
        {
            // 如果仍然存在等待下载的任务，并且当前正在下载的数量并未到达阈值时，开始下载
            while (WaitingCollection.Count > 0 && DownloadingCollection.Count < DownloadOptionsService.DownloadItem)
            {
                // 获取列表中的第一个元素
                BackgroundModel DownloadItem = WaitingCollection.FirstOrDefault();

                (bool, string) AddResult = await Aria2Service.AddUriAsync(DownloadItem.FileLink, DownloadOptionsService.DownloadFolder.Path);

                if (AddResult.Item1 && DownloadItem is not null)
                {
                    try
                    {
                        List<BackgroundModel> downloadingList = WaitingCollection.Where(item => item.DownloadKey == DownloadItem.DownloadKey).ToList();
                        foreach (BackgroundModel backgroundItem in downloadingList)
                        {
                            WaitingCollection.Remove(backgroundItem);
                        }

                        // 将当前任务的下载状态标记为正在下载状态
                        DownloadItem.DownloadFlag = 3;
                        DownloadItem.GID = AddResult.Item2;

                        DownloadingCollection.Add(DownloadItem);

                        await DownloadXmlService.UpdateFlagAsync(DownloadItem.DownloadKey, DownloadItem.DownloadFlag);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Warning, "Schedule add task failed.", e);
                        continue;
                    }
                }
                else
                {
                    try
                    {
                        DownloadItem.DownloadFlag = 0;
                        for (int index = 0; index < WaitingCollection.Count; index++)
                        {
                            if (DownloadItem.DownloadKey == WaitingCollection[index].DownloadKey)
                            {
                                WaitingCollection.RemoveAt(index);
                                break;
                            }
                        }

                        await DownloadXmlService.UpdateFlagAsync(DownloadItem.DownloadKey, DownloadItem.DownloadFlag);
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Warning, "Schedule add task failed.", e);
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// 定时计划更新下载队列信息
        /// </summary>
        private static async Task ScheduledUpdateStatusAsync()
        {
            // 当下载队列中仍然还存在下载任务时，更新正在下载的任务信息
            if (DownloadingCollection.Count > 0)
            {
                // 先更新下载的任务信息
                foreach (BackgroundModel downloadItem in DownloadingCollection)
                {
                    (bool, string, double, double, double) TellStatusResult = await Aria2Service.TellStatusAsync(downloadItem.GID);

                    if (TellStatusResult.Item1)
                    {
                        // 当下载任务处于活动状态时，更新下载任务信息
                        if (TellStatusResult.Item2 is "active")
                        {
                            downloadItem.FinishedSize = TellStatusResult.Item3;
                            downloadItem.TotalSize = TellStatusResult.Item4;
                            downloadItem.CurrentSpeed = TellStatusResult.Item5;
                        }

                        // 当下载任务处于完成状态时，将当前任务标记为完成状态
                        else if (TellStatusResult.Item2 is "complete")
                        {
                            downloadItem.DownloadFlag = 4;
                            downloadItem.GID = string.Empty;
                            downloadItem.FinishedSize = TellStatusResult.Item3;
                            downloadItem.TotalSize = TellStatusResult.Item4;
                            downloadItem.CurrentSpeed = TellStatusResult.Item5;

                            await DownloadXmlService.UpdateFlagAsync(downloadItem.DownloadKey, downloadItem.DownloadFlag);
                            await DownloadXmlService.UpdateFileSizeAsync(downloadItem.DownloadKey, downloadItem.TotalSize);
                        }

                        // 当下载任务处于错误状态时，将当前任务标记为错误状态
                        else if (TellStatusResult.Item2 is "WARNING")
                        {
                            downloadItem.DownloadFlag = 0;
                            downloadItem.GID = string.Empty;
                            downloadItem.FinishedSize = TellStatusResult.Item3;
                            downloadItem.TotalSize = TellStatusResult.Item4;
                            downloadItem.CurrentSpeed = TellStatusResult.Item5;

                            await DownloadXmlService.UpdateFlagAsync(downloadItem.DownloadKey, downloadItem.DownloadFlag);
                            await DownloadXmlService.UpdateFileSizeAsync(downloadItem.DownloadKey, downloadItem.TotalSize);
                        }
                    }
                    else
                    {
                        downloadItem.DownloadFlag = 2;
                        downloadItem.GID = string.Empty;

                        await DownloadXmlService.UpdateFlagAsync(downloadItem.DownloadKey, downloadItem.DownloadFlag);
                        await DownloadXmlService.UpdateFileSizeAsync(downloadItem.DownloadKey, downloadItem.TotalSize);
                    }
                }

                // 正在下载列表中删除掉不是处于下载状态的任务
                List<BackgroundModel> downloadingList = DownloadingCollection.Where(item => item.DownloadFlag is not 3).ToList();
                foreach (BackgroundModel backgroundItem in downloadingList)
                {
                    DownloadingCollection.Remove(backgroundItem);
                }

                BadgeNotificationService.Show(DownloadingCollection.Count + WaitingCollection.Count);

                // 下载完成后发送通知
                if (DownloadingCollection.Count is 0 && WaitingCollection.Count is 0)
                {
                    ToastNotificationService.Show(NotificationKind.DownloadCompleted);
                }
            }
        }
    }
}
