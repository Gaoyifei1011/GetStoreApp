using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Helpers;
using GetStoreApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace GetStoreApp.Services.Download
{
    /// <summary>
    /// 下载监控服务
    /// </summary>
    public class DownloadMonitorService : IDownloadMonitorService
    {
        private readonly object DownloadTaskItemLock = new object();
        private int DownloadTaskItem = 0;

        private IAria2Service Aria2Service { get; } = IOCHelper.GetService<IAria2Service>();

        private IDownloadDataService DownloadDataService { get; } = IOCHelper.GetService<IDownloadDataService>();

        private IDownloadOptionsService DownloadOptionsService { get; } = IOCHelper.GetService<IDownloadOptionsService>();

        // 下载监控器
        private Timer DownloadMonitorTimer { get; } = new Timer(2000);

        // 未完成下载任务列表
        private List<DownloadModel> DownloadList { get; } = new List<DownloadModel>();

        /// <summary>
        /// 初始化下载监控任务
        /// </summary>
        public async Task InitializeDownloadMonitorAsync()
        {
            DownloadMonitorTimer.Elapsed += DownloadMonitorTimerElapsed;
            DownloadMonitorTimer.AutoReset = true;
            DownloadMonitorTimer.Start();
            await Task.CompletedTask;
        }

        /// <summary>
        /// 关闭下载监控任务
        /// </summary>
        public async Task CloseDownloadMonitorAsync()
        {
            DownloadMonitorTimer.Stop();
            DownloadMonitorTimer.Elapsed -= DownloadMonitorTimerElapsed;

            await Task.CompletedTask;
        }

        /// <summary>
        /// 添加下载任务，并开始运行下载监控器
        /// </summary>
        public async Task AddTaskAsync(DownloadModel download)
        {
            // 在数据库中添加下载信息，并获取添加成功的结果
            bool AddResult = await DownloadDataService.AddDataAsync(download);

            // 添加成功后添加下载任务
            if (AddResult)
            {
                DownloadList.Add(download);
            }
        }

        /// <summary>
        /// 删除下载任务
        /// </summary>
        public async Task DeleteTaskAsync(DownloadModel download)
        {
            // 先删除下载任务信息
            string Result = await Aria2Service.DeleteAsync(download.GID);

            if (!string.IsNullOrEmpty(Result))
            {
                await DownloadDataService.DeleteDownloadDataAsync(download);
            }

            // 从列表中删除不再下载的任务
            DownloadList.RemoveAll(item => item.DownloadKey == download.DownloadKey);
        }

        /// <summary>
        /// 更新下载状态信息
        /// </summary>
        public async Task<DownloadStatusModel> TellStatusAsync(DownloadModel download)
        {
            return await Aria2Service.TellStatusAsync(download.GID);
        }

        /// <summary>
        /// 定时计划添加下载任务
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
            // 检查是否有处于等待下载的任务
            if (DownloadList.Any(item => item.DownloadFlag == 1))
            {
                // 检查下载任务是否到达阈值，如果已经到达，不再执行添加任务操作
                if (DownloadTaskItem >= DownloadOptionsService.DownloadItem)
                {
                    return;
                }

                // 遍历检查处于等待状态的下载任务
                foreach (DownloadModel downloadItem in DownloadList)
                {
                    if (downloadItem.DownloadFlag == 1)
                    {
                        // 添加Aria2下载任务
                        string GID = await Aria2Service.AddUriAsync(downloadItem.FileLink, DownloadOptionsService.DownloadFolder.Path, downloadItem.FileName);

                        // 检查添加是否成功，如果没有成功返回空值
                        if (!string.IsNullOrEmpty(GID))
                        {
                            downloadItem.GID = GID;

                            // 更改下载状态：正在下载中
                            downloadItem.DownloadFlag = 3;

                            // 更新下载的信息状态
                            await DownloadDataService.UpdateDataAsync(downloadItem);

                            // 下载任务到达阈值时，不再添加下载任务
                            lock (DownloadTaskItemLock)
                            {
                                if (DownloadTaskItem < DownloadOptionsService.DownloadItem)
                                {
                                    DownloadTaskItem++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 定时计划更新下载队列信息
        /// </summary>
        private async Task ScheduledUpdateStatusAsync()
        {
            await Task.CompletedTask;
        }
    }
}
