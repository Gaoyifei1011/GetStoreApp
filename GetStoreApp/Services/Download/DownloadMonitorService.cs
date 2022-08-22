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
        private Timer DownloadMonitorTimer { get; } = new Timer(1000);

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
        /// 添加下载任务
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
            await Aria2Service.DeleteAsync(download.GID);
        }

        /// <summary>
        /// 定时添加下载任务
        /// </summary>
        private async void DownloadMonitorTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // 存在等待下载的任务时，更新下载信息
            if (DownloadList.Any(item => item.DownloadFlag == 1))
            {
                foreach (DownloadModel downloadItem in DownloadList)
                {
                    // 添加Aria2下载任务
                    downloadItem.GID = await Aria2Service.AddUriAsync(downloadItem.FileLink, DownloadOptionsService.DownloadFolder.Path, downloadItem.FileName);
                    downloadItem.DownloadFlag = 1;

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
