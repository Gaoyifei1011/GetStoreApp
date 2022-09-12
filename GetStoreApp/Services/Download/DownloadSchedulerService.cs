using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Extensions.Collection;
using GetStoreApp.Helpers;
using GetStoreApp.Models.Download;
using System;
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
        private readonly object IsUpdatingNowLock = new object();

        // 标志信息是否在更新中
        private bool IsUpdatingNow = false;

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
            // 有信息在更新时，等待操作
            while (IsUpdatingNow)
            {
                await Task.Delay(300);
                continue;
            }

            // 当有信息处于更新状态中时，暂停其他操作
            lock (IsUpdatingNowLock)
            {
                IsUpdatingNow = true;
            }

            int Result = 0;

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
                    WaitingList.Add(downloadItem);
                }
                // 下载记录发生了异常
                else
                {
                    Result = 1;
                }
            }
            // 存在重复的下载记录
            else
            {
                Result = 2;
            }

            // 信息更新完毕时，允许其他操作开始执行
            lock (IsUpdatingNowLock)
            {
                IsUpdatingNow = false;
            }

            return Result;
        }

        /// <summary>
        /// 继续下载任务
        /// </summary>
        public async Task<bool> ContinueTaskAsync(BackgroundModel downloadItem)
        {
            // 有信息在更新时，等待操作
            while (IsUpdatingNow)
            {
                await Task.Delay(300);
                continue;
            }

            // 当有信息处于更新状态中时，暂停其他操作
            lock (IsUpdatingNowLock)
            {
                IsUpdatingNow = true;
            }

            // 将继续下载的任务状态标记为等待下载状态
            bool UpdateResult = await DownloadDBService.UpdateFlagAsync(downloadItem.DownloadKey, 1);

            // 数据库添加成功后添加等待下载任务
            if (UpdateResult)
            {
                WaitingList.Add(downloadItem);
            }

            // 信息更新完毕时，允许其他操作开始执行
            lock (IsUpdatingNowLock)
            {
                IsUpdatingNow = false;
            }

            return UpdateResult;
        }

        /// <summary>
        /// 暂停下载任务
        /// </summary>
        public async Task<bool> PauseTaskAsync(string downloadKey, string gID, int downloadFlag)
        {
            // 有信息在更新时，等待操作
            while (IsUpdatingNow)
            {
                await Task.Delay(300);
                continue;
            }

            // 当有信息处于更新状态中时，暂停其他操作
            lock (IsUpdatingNowLock)
            {
                IsUpdatingNow = true;
            }

            bool Result = true;

            // 处于等待下载状态时，从等待下载列表中移除
            if (downloadFlag == 1)
            {
                WaitingList.Remove(WaitingList.Find(item => item.DownloadKey == downloadKey));

                Result = await DownloadDBService.UpdateFlagAsync(downloadKey, 2);
            }

            // 处于正在下载状态时，从正在下载列表中移除
            else if (downloadFlag == 3)
            {
                // 从下载进程中移除正在下载的任务
                (bool, string) DeleteResult = await Aria2Service.PauseAsync(gID);

                if (DeleteResult.Item1)
                {
                    DownloadingList.Remove(DownloadingList.Find(item => item.DownloadKey == downloadKey));

                    Result = await DownloadDBService.UpdateFlagAsync(downloadKey, 2);
                }
            }

            // 信息更新完毕时，允许其他操作开始执行
            lock (IsUpdatingNowLock)
            {
                IsUpdatingNow = false;
            }

            return Result;
        }

        /// <summary>
        /// 删除下载任务
        /// </summary>
        public async Task<bool> DeleteTaskAsync(string downloadKey, string gID, int downloadFlag)
        {
            // 有信息在更新时，等待操作
            while (IsUpdatingNow)
            {
                await Task.Delay(300);
                continue;
            }

            // 当有信息处于更新状态中时，暂停其他操作
            lock (IsUpdatingNowLock)
            {
                IsUpdatingNow = true;
            }

            bool Result = true;

            // 处于等待下载状态时，从等待下载列表中移除
            if (downloadFlag == 1)
            {
                WaitingList.RemoveAll(item => item.DownloadKey == downloadKey);

                Result = await DownloadDBService.DeleteAsync(downloadKey);
            }

            // 处于正在下载状态时，从正在下载列表中移除
            else if (downloadFlag == 3)
            {
                // 从下载进程中移除正在下载的任务
                (bool, string) DeleteResult = await Aria2Service.DeleteAsync(gID);

                if (DeleteResult.Item1)
                {
                    DownloadingList.RemoveAll(item => item.DownloadKey == downloadKey);

                    Result = await DownloadDBService.DeleteAsync(downloadKey);
                }
            }

            // 信息更新完毕时，允许其他操作开始执行
            lock (IsUpdatingNowLock)
            {
                IsUpdatingNow = false;
            }

            return Result;
        }

        /// <summary>
        /// 定时计划添加下载任务，更新下载任务信息
        /// </summary>
        private async void DownloadMonitorTimerElapsed(object sender, ElapsedEventArgs args)
        {
            // 有信息在更新时，不再操作，等待下一秒尝试更新内容
            if (IsUpdatingNow)
            {
                return;
            }

            // 当有信息处于更新状态中时，暂停其他操作
            lock (IsUpdatingNowLock)
            {
                IsUpdatingNow = true;
            }

            await ScheduledUpdateStatusAsync();

            await ScheduledAddTaskAsync();

            // 信息更新完毕时，允许其他操作开始执行
            lock (IsUpdatingNowLock)
            {
                IsUpdatingNow = false;
            }
        }

        /// <summary>
        /// 定时计划添加下载任务
        /// </summary>
        private async Task ScheduledAddTaskAsync()
        {
            // 如果仍然存在等待下载的任务，并且当前正在下载的数量并未到达阈值时，开始下载
            while (WaitingList.Count > 0 && (DownloadingList.Count < DownloadOptionsService.DownloadItem))
            {
                // 获取列表中的第一个元素
                BackgroundModel DownloadItem = WaitingList.First();

                (bool, string) AddResult = await Aria2Service.AddUriAsync(DownloadItem.FileLink, DownloadOptionsService.DownloadFolder.Path);

                if (AddResult.Item1)
                {
                    // 将当前任务的下载状态标记为正在下载状态
                    DownloadItem.DownloadFlag = 3;
                    DownloadItem.GID = AddResult.Item2;

                    (bool, long) GetFileSizeResult = await Aria2Service.GetFileSizeAsync(AddResult.Item2);

                    if (GetFileSizeResult.Item1)
                    {
                        DownloadItem.TotalSize = GetFileSizeResult.Item2;
                    }

                    WaitingList.Remove(WaitingList.First());
                    DownloadingList.Add(DownloadItem);

                    await DownloadDBService.UpdateFlagAsync(DownloadItem.DownloadKey, DownloadItem.DownloadFlag);
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
                foreach (BackgroundModel downloadItem in DownloadingList)
                {
                    (bool, string, long, long, long) TellStatusResult = await Aria2Service.TellStatusAsync(downloadItem.GID);

                    if (TellStatusResult.Item1)
                    {
                        // 当下载任务处于活动状态时，更新下载任务信息
                        if (TellStatusResult.Item2 == "active")
                        {
                            downloadItem.FinishedSize = TellStatusResult.Item3;
                            downloadItem.TotalSize = TellStatusResult.Item4;
                            downloadItem.CurrentSpeed = TellStatusResult.Item5;
                        }

                        // 当下载任务处于完成状态时，将当前任务标记为完成状态
                        else if (TellStatusResult.Item2 == "complete")
                        {
                            downloadItem.DownloadFlag = 4;
                            downloadItem.GID = string.Empty;
                            downloadItem.FinishedSize = TellStatusResult.Item3;
                            downloadItem.TotalSize = TellStatusResult.Item4;
                            downloadItem.CurrentSpeed = TellStatusResult.Item5;

                            await DownloadDBService.UpdateFlagAsync(downloadItem.DownloadKey, downloadItem.DownloadFlag);
                            await DownloadDBService.UpdateFileSizeAsync(downloadItem.DownloadKey, downloadItem.TotalSize);
                        }

                        // 当下载任务处于错误状态时，将当前任务标记为错误状态
                        else if (TellStatusResult.Item2 == "error")
                        {
                            downloadItem.DownloadFlag = 0;
                            downloadItem.GID = string.Empty;
                            downloadItem.FinishedSize = TellStatusResult.Item3;
                            downloadItem.TotalSize = TellStatusResult.Item4;
                            downloadItem.CurrentSpeed = TellStatusResult.Item5;

                            await DownloadDBService.UpdateFlagAsync(downloadItem.DownloadKey, downloadItem.DownloadFlag);
                            await DownloadDBService.UpdateFileSizeAsync(downloadItem.DownloadKey, downloadItem.TotalSize);
                        }
                    }
                }

                // 正在下载列表中删除掉不是处于下载状态的任务
                DownloadingList.RemoveAll(item => item.DownloadFlag != 3);
            }
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
