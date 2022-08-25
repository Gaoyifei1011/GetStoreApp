using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Helpers;
using GetStoreApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace GetStoreApp.Services.Download
{
    /// <summary>
    /// 下载调度服务
    /// </summary>
    public class DownloadSchedulerService : IDownloadSchedulerService
    {
        // 临界区资源访问互斥锁
        private readonly object DownloadingListLock = new object();

        private readonly object WaitingListLock = new object();

        private IAria2Service Aria2Service { get; } = IOCHelper.GetService<IAria2Service>();

        private IDownloadDBService DownloadDBService { get; } = IOCHelper.GetService<IDownloadDBService>();

        private IDownloadOptionsService DownloadOptionsService { get; } = IOCHelper.GetService<IDownloadOptionsService>();

        // 下载调度器
        private Timer DownloadSchedulerTimer { get; } = new Timer(1000);

        // 下载中任务列表
        private List<DownloadModel> DownloadingList { get; } = new List<DownloadModel>();

        // 等待下载任务列表
        private List<DownloadModel> WaitingList { get; } = new List<DownloadModel>();

        public List<DownloadModel> GetDownloadingList()
        {
            return DownloadingList;
        }

        public List<DownloadModel> GetWaitingList()
        {
            return WaitingList;
        }

        /// <summary>
        /// 初始化下载监控任务
        /// </summary>
        public async Task InitializeDownloadMonitorAsync()
        {
            DownloadSchedulerTimer.Elapsed += DownloadMonitorTimerElapsed;
            DownloadSchedulerTimer.AutoReset = true;
            DownloadSchedulerTimer.Start();
            await Task.CompletedTask;
        }

        /// <summary>
        /// 关闭下载监控任务
        /// </summary>
        public async Task CloseDownloadMonitorAsync()
        {
            DownloadSchedulerTimer.Stop();
            DownloadSchedulerTimer.Elapsed -= DownloadMonitorTimerElapsed;

            await Task.CompletedTask;
        }

        /// <summary>
        /// 添加下载任务
        /// </summary>
        public async Task AddTaskAsync(DownloadModel downloadItem)
        {
            // 将新添加的下载任务状态标记为等待下载状态
            downloadItem.DownloadFlag = 1;

            // 在数据库中添加下载信息，并获取添加成功的结果
            bool AddResult = await DownloadDBService.AddDataAsync(downloadItem);

            // 数据库添加成功后添加等待下载任务
            if (AddResult)
            {
                // 保证线程安全
                lock (WaitingListLock)
                {
                    WaitingList.Add(downloadItem);
                }
            }
        }

        /// <summary>
        /// 继续下载任务
        /// </summary>
        public async Task ContinueTaskAsync(DownloadModel downloadItem)
        {
            // 将继续下载的任务状态标记为等待下载状态
            downloadItem.DownloadFlag = 1;

            bool UpdateResult = await DownloadDBService.UpdateFlagAsync(downloadItem);

            // 数据库添加成功后添加等待下载任务
            if (UpdateResult)
            {
                // 保证线程安全
                lock (WaitingListLock)
                {
                    WaitingList.Add(downloadItem);
                }
            }
        }

        /// <summary>
        /// 暂停下载任务
        /// </summary>
        public async Task PauseTaskAsync(DownloadModel downloadItem)
        {
            // 处于等待下载状态时，从等待下载列表中移除
            if (downloadItem.DownloadFlag == 1)
            {
                // 保证线程安全
                lock (WaitingListLock)
                {
                    WaitingList.RemoveAll(item => item.DownloadKey == downloadItem.DownloadKey);
                }
            }

            // 处于正在下载状态时，从正在下载列表中移除
            else if (downloadItem.DownloadFlag == 3)
            {
                // 从下载进程中移除正在下载的任务
                string DeleteResult = await Aria2Service.DeleteAsync(downloadItem.GID);

                if (!string.IsNullOrEmpty(DeleteResult))
                {
                    // 保证线程安全
                    lock (DownloadingListLock)
                    {
                        DownloadingList.RemoveAll(item => item.DownloadKey == downloadItem.DownloadKey);
                    }
                }
            }

            // 修改数据库下载任务状态为暂停状态
            downloadItem.DownloadFlag = 2;
            await DownloadDBService.UpdateFlagAsync(downloadItem);
        }

        /// <summary>
        /// 删除下载任务
        /// </summary>
        public async Task DeleteTaskAsync(DownloadModel downloadItem)
        {
            // 处于等待下载状态时，从等待下载列表中移除
            if (downloadItem.DownloadFlag == 1)
            {
                // 保证线程安全
                lock (WaitingListLock)
                {
                    WaitingList.RemoveAll(item => item.DownloadKey == downloadItem.DownloadKey);
                }
            }

            // 处于正在下载状态时，从正在下载列表中移除
            else if (downloadItem.DownloadFlag == 3)
            {
                // 从下载进程中移除正在下载的任务
                string DeleteResult = await Aria2Service.DeleteAsync(downloadItem.GID);

                if (!string.IsNullOrEmpty(DeleteResult))
                {
                    // 保证线程安全
                    lock (DownloadingListLock)
                    {
                        DownloadingList.RemoveAll(item => item.DownloadKey == downloadItem.DownloadKey);
                    }
                }
            }

            // 从数据库中删除任务
            await DownloadDBService.DeleteDownloadDataAsync(downloadItem);
        }

        /// <summary>
        /// 继续下载全部任务
        /// </summary>
        public async Task ContinueAllTaskAsync(List<DownloadModel> downloadItemList)
        {
            foreach (DownloadModel downloadItem in downloadItemList)
            {
                await AddTaskAsync(downloadItem);
            }
        }

        /// <summary>
        /// 暂停下载全部任务
        /// </summary>
        public async Task PauseAllTaskAsync(List<DownloadModel> downloadItemList)
        {
            foreach (DownloadModel downloadItem in downloadItemList)
            {
                await PauseTaskAsync(downloadItem);
            }
        }

        /// <summary>
        /// 删除选定项目的任务
        /// </summary>
        public async Task DeleteSelectedTaskAsync(List<DownloadModel> downloadItemList)
        {
            foreach (DownloadModel downloadItem in downloadItemList)
            {
                await DeleteTaskAsync(downloadItem);
            }
        }

        /// <summary>
        /// 定时计划添加下载任务，更新下载任务信息
        /// </summary>
        private async void DownloadMonitorTimerElapsed(object sender, ElapsedEventArgs e)
        {
            await ScheduledUpdateStatusAsync();

            await ScheduledAddTaskAsync();
        }

        /// <summary>
        /// 定时计划添加下载任务
        /// </summary>
        private async Task ScheduledAddTaskAsync()
        {
            // 如果仍然存在等待下载的任务，并且当前正在下载的数量并未到达阈值时，开始下载
            while (WaitingList.Count > 0 && DownloadingList.Count < DownloadOptionsService.DownloadItem)
            {
                // 获取列表中的第一个元素
                DownloadModel DownloadItem = WaitingList.First();

                bool AddDataResult = await DownloadDBService.AddDataAsync(DownloadItem);

                string TaskGID = await Aria2Service.AddUriAsync(DownloadItem.FileLink, DownloadOptionsService.DownloadFolder.Path, DownloadItem.FileName);

                // 添加下载任务时确保线程安全
                if (AddDataResult && !string.IsNullOrEmpty(TaskGID))
                {
                    lock (DownloadingList)
                    {
                        // 将当前任务的下载状态标记为正在下载状态
                        DownloadItem.DownloadFlag = 3;
                        DownloadItem.GID = TaskGID;
                        DownloadingList.Add(DownloadItem);
                    }

                    // 等待列表中移除已经成功添加的项目
                    WaitingList.RemoveAt(0);
                }
            }
        }

        /// <summary>
        /// 定时计划更新下载队列信息
        /// </summary>
        private async Task ScheduledUpdateStatusAsync()
        {
            // 当下载队列中仍然还存在下载任务时，更新正在下载的任务信息
            if (DownloadingList.Count > 0)
            {
                // 先更新下载的任务信息
                foreach (DownloadModel downloadItem in DownloadingList)
                {
                    Tuple<string, string, int, int, int> DownloadStatus = await Aria2Service.TellStatusAsync(downloadItem.GID);

                    // 当下载任务处于活动状态时，更新下载任务信息
                    if (DownloadStatus.Item2 == "active")
                    {
                        lock (DownloadingList)
                        {
                            downloadItem.TotalSize = DownloadStatus.Item3;
                            downloadItem.FinishedSize = DownloadStatus.Item4;
                            downloadItem.CurrentSpeed = DownloadStatus.Item5;
                        }
                    }

                    // 当下载任务处于完成状态时，将当前任务标记为完成状态
                    else if (DownloadStatus.Item2 == "completed")
                    {
                        lock (DownloadingList)
                        {
                            downloadItem.DownloadFlag = 4;
                            downloadItem.GID = string.Empty;
                            downloadItem.TotalSize = DownloadStatus.Item3;
                            downloadItem.FinishedSize = DownloadStatus.Item4;
                            downloadItem.CurrentSpeed = DownloadStatus.Item5;
                        }

                        await DownloadDBService.UpdateFlagAsync(downloadItem);
                    }

                    // 当下载任务处于错误状态时，将当前任务标记为错误状态
                    else if (DownloadStatus.Item2 == "error")
                    {
                        lock (DownloadingList)
                        {
                            downloadItem.DownloadFlag = 0;
                            downloadItem.GID = string.Empty;
                            downloadItem.TotalSize = DownloadStatus.Item3;
                            downloadItem.FinishedSize = DownloadStatus.Item4;
                            downloadItem.CurrentSpeed = DownloadStatus.Item5;
                        }

                        await DownloadDBService.UpdateFlagAsync(downloadItem);
                    }
                }

                // 保证线程安全
                lock (DownloadingListLock)
                {
                    // 正在下载列表中删除掉不是处于下载状态的任务
                    DownloadingList.RemoveAll(item => item.DownloadFlag != 3);
                }
            }
        }
    }
}
