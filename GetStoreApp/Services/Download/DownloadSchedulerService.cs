using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Extensions.Collection;
using GetStoreApp.Helpers;
using GetStoreApp.Models.Download;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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

        // 下载中任务列表（带通知）
        public NotifyList<BackgroundModel> DownloadingList { get; } = new NotifyList<BackgroundModel>();

        // 等待下载任务列表（带通知）
        public NotifyList<BackgroundModel> WaitingList { get; } = new NotifyList<BackgroundModel>();

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
        public async Task<int> AddTaskAsync(string fileName, string fileLink, string fileSHA1)
        {
            BackgroundModel downloadItem = new BackgroundModel
            {
                DownloadKey = GenerateUniqueKey(fileName, fileLink, fileSHA1),
                FileName = fileName,
                FileLink = fileLink,
                FilePath = string.Format("{0}\\{1}", DownloadOptionsService.DownloadFolder.Path, fileName),
                TotalSize = 0,
                FileSHA1 = fileSHA1,
                DownloadFlag = 1
            };

            // 检查是否存在相同的任务记录
            bool SearchResult = await DownloadDBService.CheckDuplicatedAsync(downloadItem.DownloadKey);

            if (!SearchResult)
            {
                // 在数据库中添加下载信息，并获取添加成功的结果
                bool AddResult = await DownloadDBService.AddAsync(downloadItem);

                // 数据库添加成功后添加等待下载任务
                if (AddResult)
                {
                    // 保证线程安全
                    lock (WaitingListLock)
                    {
                        WaitingList.Add(downloadItem);
                    }
                    return 0;
                }
                // 下载记录添加发生异常
                else
                {
                    return 1;
                }
            }
            // 存在重复的下载记录
            else
            {
                return 2;
            }
        }

        /// <summary>
        /// 继续下载任务
        /// </summary>
        public async Task<bool> ContinueTaskAsync(BackgroundModel downloadItem)
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

            return UpdateResult;
        }

        /// <summary>
        /// 暂停下载任务
        /// </summary>
        public async Task<bool> PauseTaskAsync(BackgroundModel downloadItem)
        {
            int DownloadFlag = downloadItem.DownloadFlag;

            // 处于等待下载状态时，从等待下载列表中移除
            if (downloadItem.DownloadFlag == 1)
            {
                // 保证线程安全
                lock (WaitingListLock)
                {
                    WaitingList.Remove(WaitingList.Find(item => item.DownloadKey == downloadItem.DownloadKey));
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
                        DownloadingList.Remove(WaitingList.Find(item => item.DownloadKey == downloadItem.DownloadKey));
                    }
                }
            }

            // 修改数据库下载任务状态为暂停状态
            downloadItem.DownloadFlag = 2;
            return await DownloadDBService.UpdateFlagAsync(downloadItem);
        }

        /// <summary>
        /// 删除下载任务
        /// </summary>
        public async Task<bool> DeleteTaskAsync(BackgroundModel downloadItem)
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
            return await DownloadDBService.DeleteAsync(downloadItem.DownloadKey);
        }

        /// <summary>
        /// 定时计划添加下载任务，更新下载任务信息
        /// </summary>
        private async void DownloadMonitorTimerElapsed(object sender, ElapsedEventArgs args)
        {
            await ScheduledUpdateStatusAsync();

            await ScheduledAddTaskAsync();
        }

        /// <summary>
        /// 定时计划添加下载任务
        /// </summary>
        private async Task ScheduledAddTaskAsync()
        {
            // 在添加任务时先暂停计时器操作
            DownloadSchedulerTimer.Enabled = false;

            // 如果仍然存在等待下载的任务，并且当前正在下载的数量并未到达阈值时，开始下载
            while (WaitingList.Count > 0 && (DownloadingList.Count < DownloadOptionsService.DownloadItem))
            {
                // 获取列表中的第一个元素
                BackgroundModel DownloadItem = WaitingList.First();

                string TaskGID = await Aria2Service.AddUriAsync(DownloadItem.FileLink, DownloadOptionsService.DownloadFolder.Path, DownloadItem.FileName);
                // 添加下载任务时确保线程安全
                if (!string.IsNullOrEmpty(TaskGID))
                {
                    lock (DownloadingListLock)
                    {
                        // 将当前任务的下载状态标记为正在下载状态
                        DownloadItem.DownloadFlag = 3;
                        DownloadItem.GID = TaskGID;
                        DownloadingList.Add(DownloadItem);
                    }

                    // 等待列表中移除已经成功添加的项目
                    lock (WaitingListLock)
                    {
                        WaitingList.Remove(WaitingList.First());
                    }

                    await DownloadDBService.UpdateFlagAsync(DownloadItem);
                }
            }

            // 添加完成后，再打开计时器
            DownloadSchedulerTimer.Enabled = true;
        }

        /// <summary>
        /// 定时计划更新下载队列信息
        /// </summary>
        private async Task ScheduledUpdateStatusAsync()
        {
            // 在更新任务信息时先暂停计时器操作
            DownloadSchedulerTimer.Enabled = false;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            // 当下载队列中仍然还存在下载任务时，更新正在下载的任务信息
            if (DownloadingList.Count > 0)
            {
                // 先更新下载的任务信息
                foreach (BackgroundModel downloadItem in DownloadingList)
                {
                    Tuple<string, string, int, int, int> DownloadStatus = await Aria2Service.TellStatusAsync(downloadItem.GID);

                    if (DownloadStatus is null)
                    {
                        continue;
                    }

                    // 当下载任务处于活动状态时，更新下载任务信息
                    if (DownloadStatus.Item2 == "active")
                    {
                        lock (DownloadingListLock)
                        {
                            downloadItem.TotalSize = DownloadStatus.Item3;
                            downloadItem.FinishedSize = DownloadStatus.Item4;
                            downloadItem.CurrentSpeed = DownloadStatus.Item5;
                        }
                    }

                    // 当下载任务处于完成状态时，将当前任务标记为完成状态
                    else if (DownloadStatus.Item2 == "completed")
                    {
                        lock (DownloadingListLock)
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
                        lock (DownloadingListLock)
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
            stopwatch.Stop();
            Debug.WriteLine(stopwatch.ElapsedMilliseconds);
            DownloadSchedulerTimer.Enabled = true;
        }

        /// <summary>
        /// 生成唯一的下载键值
        /// </summary>
        private string GenerateUniqueKey(string fileName, string fileLink, string fileSHA1)
        {
            string Content = string.Format("{0} {1} {2}", fileName, fileLink, fileSHA1);

            MD5 md5Hash = MD5.Create();

            // 将输入字符串转换为字节数组并计算哈希数据
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(Content));

            // 创建一个 Stringbuilder 来收集字节并创建字符串
            StringBuilder str = new StringBuilder();

            // 循环遍历哈希数据的每一个字节并格式化为十六进制字符串
            for (int i = 0; i < data.Length; i++) str.Append(data[i].ToString("x2"));//加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位

            // 返回十六进制字符串
            return str.ToString();
        }
    }
}
