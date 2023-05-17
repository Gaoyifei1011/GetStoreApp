using GetStoreApp.Extensions.DataType.Collections;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Download;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.Services.Controls.Settings.Experiment;
using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.PInvoke.WinINet;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace GetStoreApp.Services.Controls.Download
{
    /// <summary>
    /// 下载调度服务
    /// </summary>
    public static class DownloadSchedulerService
    {
        // 临界区资源访问互斥锁
        private static readonly object DownloadSchedulerLock = new object();

        private static readonly object IsNetWorkConnectedLock = new object();

        // 标志信息是否在更新中
        private static bool IsUpdatingNow = false;

        private static bool IsNetWorkConnected = true;

        // 下载调度器
        private static Timer DownloadSchedulerTimer { get; } = new Timer(1000);

        // 下载中任务列表（带通知）
        public static NotifyList<BackgroundModel> DownloadingList { get; } = new NotifyList<BackgroundModel>();

        // 等待下载任务列表（带通知）
        public static NotifyList<BackgroundModel> WaitingList { get; } = new NotifyList<BackgroundModel>();

        /// <summary>
        /// 先获取当前网络状态信息，然后初始化下载监控任务
        /// </summary>
        public static async Task InitializeDownloadSchedulerAsync()
        {
            // 查看是否开启了网络监控服务
            if (NetWorkMonitorService.NetWorkMonitorValue)
            {
                INTERNET_CONNECTION_FLAGS flags = INTERNET_CONNECTION_FLAGS.INTERNET_CONNECTION_OFFLINE;
                if (!WinINetLibrary.InternetGetConnectedState(ref flags, 0))
                {
                    IsNetWorkConnected = false;
                    AppNotificationService.Show("DownloadAborted", "NotDownload");
                }
            }

            DownloadSchedulerTimer.Elapsed += DownloadSchedulerTimerElapsed;
            DownloadSchedulerTimer.AutoReset = true;
            DownloadSchedulerTimer.Start();
            await Task.CompletedTask;
        }

        /// <summary>
        /// 关闭下载监控任务
        /// </summary>
        public static async Task CloseDownloadSchedulerAsync()
        {
            DownloadSchedulerTimer.Stop();
            DownloadSchedulerTimer.Elapsed -= DownloadSchedulerTimerElapsed;
            DownloadSchedulerTimer.Dispose();

            await Task.CompletedTask;
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
                    WaitingList.Add(backgroundItem);
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
                    WaitingList.Add(backgroundItem);
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
                WaitingList.Add(downloadItem);
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
                    WaitingList.Remove(WaitingList.Find(item => item.DownloadKey == downloadKey));
                    Result = await DownloadXmlService.UpdateFlagAsync(downloadKey, 2);
                }
                catch (Exception)
                {
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
                        DownloadingList.Remove(DownloadingList.Find(item => item.DownloadKey == downloadKey));
                        Result = await DownloadXmlService.UpdateFlagAsync(downloadKey, 2);
                    }
                    catch (Exception)
                    {
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
            foreach (BackgroundModel backgroundItem in DownloadingList)
            {
                // 从下载进程中移除正在下载的任务
                (bool, string) DeleteResult = await Aria2Service.PauseAsync(backgroundItem.GID);

                if (DeleteResult.Item1)
                {
                    try
                    {
                        await DownloadXmlService.UpdateFlagAsync(backgroundItem.DownloadKey, 2);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }

            // 从等待下载列表中暂停所有等待下载任务
            foreach (BackgroundModel backgroundItem in WaitingList)
            {
                try
                {
                    await DownloadXmlService.UpdateFlagAsync(backgroundItem.DownloadKey, 2);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            // 清空所有正在下载和等待下载的列表内容
            DownloadingList.Clear();
            WaitingList.Clear();

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
                    WaitingList.RemoveAll(item => item.DownloadKey == downloadKey);
                    Result = await DownloadXmlService.DeleteAsync(downloadKey);
                }
                catch (Exception)
                {
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
                        DownloadingList.RemoveAll(item => item.DownloadKey == downloadKey);

                        Result = await DownloadXmlService.DeleteAsync(downloadKey);
                    }
                    catch (Exception)
                    {
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
        private static async void DownloadSchedulerTimerElapsed(object sender, ElapsedEventArgs args)
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
            INTERNET_CONNECTION_FLAGS flags = INTERNET_CONNECTION_FLAGS.INTERNET_CONNECTION_OFFLINE;
            if (!WinINetLibrary.InternetGetConnectedState(ref flags, 0))
            {
                // 如果网络处于正在连接状态，修改当前网络状态并发送通知
                if (IsNetWorkConnected)
                {
                    lock (IsNetWorkConnectedLock)
                    {
                        IsNetWorkConnected = false;
                    }

                    // 发送通知
                    if (DownloadingList.Any() || WaitingList.Any())
                    {
                        AppNotificationService.Show("DownloadAborted", "DownloadingNow");
                    }
                    else
                    {
                        AppNotificationService.Show("DownloadAborted", "NotDownload");
                    }
                }

                // 暂停所有下载任务
                await PauseAllTaskAsync();
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
            while (WaitingList.Count > 0 && DownloadingList.Count < DownloadOptionsService.DownloadItem)
            {
                // 获取列表中的第一个元素
                BackgroundModel DownloadItem = WaitingList.FirstOrDefault();

                (bool, string) AddResult = await Aria2Service.AddUriAsync(DownloadItem.FileLink, DownloadOptionsService.DownloadFolder.Path);

                if (AddResult.Item1 && DownloadItem is not null)
                {
                    // 将当前任务的下载状态标记为正在下载状态
                    DownloadItem.DownloadFlag = 3;
                    DownloadItem.GID = AddResult.Item2;

                    try
                    {
                        WaitingList.RemoveAll(item => item.DownloadKey == DownloadItem.DownloadKey);

                        DownloadingList.Add(DownloadItem);

                        await DownloadXmlService.UpdateFlagAsync(DownloadItem.DownloadKey, DownloadItem.DownloadFlag);
                    }
                    catch (Exception)
                    {
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
            if (DownloadingList.Count > 0)
            {
                // 先更新下载的任务信息
                foreach (BackgroundModel downloadItem in DownloadingList)
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
                        else if (TellStatusResult.Item2 is "error")
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
                DownloadingList.RemoveAll(item => item.DownloadFlag is not 3);

                // 下载完成后发送通知
                if (DownloadingList.Count is 0 && WaitingList.Count is 0)
                {
                    AppNotificationService.Show("DownloadCompleted");
                }
            }
        }
    }
}
