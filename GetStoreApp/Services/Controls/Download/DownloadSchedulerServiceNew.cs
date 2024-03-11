using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Download;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;
using Windows.System.Threading;

namespace GetStoreApp.Services.Controls.Download
{
    /// <summary>
    /// 下载调度服务
    /// </summary>
    public static class DownloadSchedulerServiceNew
    {
        private static readonly object isNetWorkConnectedLock = new object();

        private static bool isUpdatingNow = false;
        private static int badgeCount = 0;
        private static ThreadPoolTimer downloadSchedulerTimer;
        private static SemaphoreSlim semaphoreSlim;

        private static bool _isNetWorkConnected = false;

        public static bool IsNetWorkConnected
        {
            get { return _isNetWorkConnected; }

            set
            {
                if (_isNetWorkConnected != value)
                {
                    _isNetWorkConnected = value;
                    PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(IsNetWorkConnected)));
                }
            }
        }

        public static event PropertyChangedEventHandler PropertyChanged;

        public static event Action<NotifyCollectionChangedAction, DownloadSchedulerModel> CollectionChanged;

        public static event Action<DownloadSchedulerModel> CollectionContentChanged;

        public static event Action<int> CollectionCountChanged;

        // 下载任务列表
        public static List<DownloadSchedulerModel> DownloadSchedulerList { get; } = new List<DownloadSchedulerModel>();

        #region 下载控制服务：自定义事件

        /// <summary>
        /// 网络状态发生变化时发送通知
        /// </summary>
        private static void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName.Equals(nameof(IsNetWorkConnected)))
            {
                if (!IsNetWorkConnected)
                {
                    ToastNotificationService.Show(NotificationKind.DownloadAborted, "NotDownload");
                }
            }
        }

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

        #endregion 下载控制服务：自定义事件

        /// <summary>
        /// 初始化后台下载调度器
        /// 先检查当前网络状态信息，加载暂停任务信息，然后初始化下载监控任务
        /// </summary>
        public static void InitializeDownloadScheduler()
        {
            // 查看当前网络状态信息
            // 查看是否开启了网络监控服务
            if (NetWorkMonitorService.NetWorkMonitorValue)
            {
                Tuple<bool, bool> checkResult = NetWorkHelper.IsNetworkConnected();

                // 开启监控服务时，检查成功和网络处于连接状态时，代表网络已连接
                lock (isNetWorkConnectedLock)
                {
                    IsNetWorkConnected = checkResult.Item1 && checkResult.Item2;
                }

                // 网络未连接，显示网络异常通知
                if (!IsNetWorkConnected)
                {
                    ToastNotificationService.Show(NotificationKind.DownloadAborted, "NotDownload");
                }
            }
            else
            {
                IsNetWorkConnected = true;
            }

            // 加载暂停任务下载信息
            // TODO

            // 更新当前下载任务数量通知
            BadgeNotificationService.Show(badgeCount);

            // 挂载网络状态变化事件
            PropertyChanged += OnPropertyChanged;

            // 挂载集合数量发生更改事件
            CollectionCountChanged += OnCollectionCountChanged;

            semaphoreSlim = new SemaphoreSlim(1, 1);

            downloadSchedulerTimer ??= ThreadPoolTimer.CreatePeriodicTimer(DownloadSchedulerTimerElapsed, TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// 关闭下载监控任务
        /// </summary>
        public static void CloseDownloadScheduler()
        {
            downloadSchedulerTimer.Cancel();
            semaphoreSlim.Dispose();
            semaphoreSlim = null;

            // 卸载网络状态变化事件
            PropertyChanged -= OnPropertyChanged;

            // 卸载集合数量发生更改事件
            CollectionCountChanged -= OnCollectionCountChanged;
        }

        /// <summary>
        /// 添加下载任务
        /// </summary>
        public static async Task AddTaskAsync(DownloadSchedulerModel addItem, string operation)
        {
            if (semaphoreSlim is not null)
            {
                await semaphoreSlim.WaitAsync();
            }
            addItem.DownloadStatus = DownloadStatus.Unknown;

            try
            {
                // 添加下载任务
                if (operation is "Add")
                {
                    // 添加下载记录
                    // 本地存储下载信息文件中添加下载信息
                    if (await DownloadXmlService.AddAsync(addItem))
                    {
                        // Mile.Aria2 添加下载信息
                        (bool, string) addResult = await Aria2Service.AddUriAsync(addItem.FileName, addItem.FileLink, DownloadOptionsService.DownloadFolder.Path);

                        // 下载任务添加成功
                        if (addResult.Item1)
                        {
                            addItem.DownloadStatus = DownloadStatus.Unknown;
                            addItem.GID = addResult.Item2;
                        }
                        // 下载任务添加失败
                        else
                        {
                            addItem.DownloadStatus = DownloadStatus.Error;
                        }

                        // 下载调度列表添加下载信息，触发列表数量变化和集合更改的事件
                        DownloadSchedulerList.Add(addItem);
                        CollectionChanged.Invoke(NotifyCollectionChangedAction.Add, addItem);
                        CollectionCountChanged?.Invoke(DownloadSchedulerList.Count);

                        // 本地存储下载信息文件中更新下载信息
                        await DownloadXmlService.UpdateFlagAsync(addItem.DownloadKey, (int)addItem.DownloadStatus);
                    }
                }
                // 更新下载记录
                else if (operation is "Update")
                {
                    // Mile.Aria2 添加下载信息
                    (bool, string) addResult = await Aria2Service.AddUriAsync(addItem.FileName, addItem.FileLink, DownloadOptionsService.DownloadFolder.Path);

                    // 下载任务添加成功
                    if (addResult.Item1)
                    {
                        addItem.DownloadStatus = DownloadStatus.Unknown;
                        addItem.GID = addResult.Item2;
                    }
                    // 下载任务添加失败
                    else
                    {
                        addItem.DownloadStatus = DownloadStatus.Error;
                    }

                    bool isPaused = false;

                    // 下载列表如果存在当前信息则修改，没有直接添加下载信息
                    foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerList)
                    {
                        if (downloadSchedulerItem.DownloadKey.Equals(addItem.DownloadKey, StringComparison.OrdinalIgnoreCase))
                        {
                            downloadSchedulerItem.DownloadStatus = addItem.DownloadStatus;
                            downloadSchedulerItem.GID = addItem.GID;
                            isPaused = true;

                            // 下载调度列表修改下载信息，触发列表内容变化的事件
                            CollectionContentChanged?.Invoke(downloadSchedulerItem);
                            break;
                        }
                    }

                    if (!isPaused)
                    {
                        // 下载调度列表添加下载信息，触发列表数量变化和集合更改的事件
                        DownloadSchedulerList.Add(addItem);
                        CollectionChanged.Invoke(NotifyCollectionChangedAction.Add, addItem);
                        CollectionCountChanged?.Invoke(DownloadSchedulerList.Count);
                    }

                    // 本地存储下载信息文件中更新下载信息
                    await DownloadXmlService.UpdateFlagAsync(addItem.DownloadKey, (int)addItem.DownloadStatus);
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Add download task failed", e);
            }
            finally
            {
                semaphoreSlim?.Release();
            }
        }

        /// <summary>
        /// 继续下载任务
        /// </summary>
        public static async Task ContinueTaskAsync(DownloadSchedulerModel continueItem)
        {
            if (semaphoreSlim is not null)
            {
                await semaphoreSlim.WaitAsync();
            }

            continueItem.DownloadStatus = DownloadStatus.Unknown;

            try
            {
                // Mile.Aria2 添加下载信息
                (bool, string) continueResult = await Aria2Service.AddUriAsync(continueItem.FileName, continueItem.FileLink, DownloadOptionsService.DownloadFolder.Path);

                // 下载任务添加成功
                if (continueResult.Item1)
                {
                    continueItem.DownloadStatus = DownloadStatus.Unknown;
                    continueItem.GID = continueResult.Item2;
                }
                // 下载任务添加失败
                else
                {
                    continueItem.DownloadStatus = DownloadStatus.Error;
                }

                // 下载列表存在当前信息则修改下载信息
                foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerList)
                {
                    if (downloadSchedulerItem.DownloadKey.Equals(downloadSchedulerItem.DownloadKey, StringComparison.OrdinalIgnoreCase))
                    {
                        downloadSchedulerItem.DownloadStatus = continueItem.DownloadStatus;
                        downloadSchedulerItem.GID = continueItem.GID;

                        // 下载调度列表修改下载信息，触发列表内容变化的事件
                        CollectionContentChanged?.Invoke(downloadSchedulerItem);
                        break;
                    }
                }

                // 本地存储下载信息文件中更新下载信息
                await DownloadXmlService.UpdateFlagAsync(continueItem.DownloadKey, (int)continueItem.DownloadStatus);
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Continue download task failed", e);
            }
            finally
            {
                semaphoreSlim?.Release();
            }
        }

        /// <summary>
        /// 暂停下载任务
        /// </summary>
        public static async Task PauseTaskAsync(string downloadKey, string gID, DownloadStatus downloadStatus)
        {
            if (semaphoreSlim is not null)
            {
                await semaphoreSlim.WaitAsync();
            }

            try
            {
                // 遍历当前下载列表，存在当前下载任务，尝试暂停下载，并修改下载信息
                foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerList)
                {
                    if (downloadSchedulerItem.DownloadKey.Equals(downloadKey, StringComparison.OrdinalIgnoreCase))
                    {
                        // 处于正在下载状态或等待状态时，将任务停止，更改为暂停状态
                        if (downloadStatus is DownloadStatus.Download || downloadStatus is DownloadStatus.Wait)
                        {
                            // 从下载进程中暂停正在下载或等待下载的任务
                            (bool, string) pauseResult = await Aria2Service.PauseAsync(gID);

                            if (pauseResult.Item1)
                            {
                                downloadSchedulerItem.DownloadStatus = DownloadStatus.Pause;
                                // 下载调度列表修改下载信息，触发列表内容变化的事件
                                CollectionContentChanged?.Invoke(downloadSchedulerItem);
                                await DownloadXmlService.UpdateFlagAsync(downloadSchedulerItem.DownloadKey, (int)downloadSchedulerItem.DownloadStatus);

                                break;
                            }
                        }
                        // 处于未知状态时，将任务停止，更改为暂停状态
                        else if (downloadStatus is DownloadStatus.Unknown)
                        {
                            downloadSchedulerItem.DownloadStatus = DownloadStatus.Pause;

                            // 下载调度列表修改下载信息，触发列表内容变化的事件
                            CollectionContentChanged?.Invoke(downloadSchedulerItem);
                            await DownloadXmlService.UpdateFlagAsync(downloadSchedulerItem.DownloadKey, (int)downloadSchedulerItem.DownloadStatus);

                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Pause download task failed", e);
            }
            finally
            {
                semaphoreSlim?.Release();
            }
        }

        /// <summary>
        /// 暂停全部下载任务
        /// </summary>
        public static async Task PauseAllTaskAsync()
        {
            if (semaphoreSlim is not null)
            {
                await semaphoreSlim.WaitAsync();
            }

            try
            {
                // 遍历所有的任务，暂停等待下载、正在下载和未知状态的任务
                foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerList)
                {
                    try
                    {
                        // 处于正在下载状态或等待状态时，将任务停止，更改为暂停状态
                        if (downloadSchedulerItem.DownloadStatus is DownloadStatus.Download || downloadSchedulerItem.DownloadStatus is DownloadStatus.Wait)
                        {
                            // 从下载进程中暂停正在下载或等待下载的任务
                            (bool, string) pauseResult = await Aria2Service.PauseAsync(downloadSchedulerItem.GID);

                            if (pauseResult.Item1)
                            {
                                downloadSchedulerItem.DownloadStatus = DownloadStatus.Pause;
                                // 下载调度列表修改下载信息，触发列表内容变化的事件
                                CollectionContentChanged?.Invoke(downloadSchedulerItem);
                                await DownloadXmlService.UpdateFlagAsync(downloadSchedulerItem.DownloadKey, (int)downloadSchedulerItem.DownloadStatus);
                            }
                        }
                        // 处于未知状态时，将任务停止，更改为暂停状态
                        else if (downloadSchedulerItem.DownloadStatus is DownloadStatus.Unknown)
                        {
                            downloadSchedulerItem.DownloadStatus = DownloadStatus.Pause;
                            // 下载调度列表修改下载信息，触发列表内容变化的事件
                            CollectionContentChanged?.Invoke(downloadSchedulerItem);
                            await DownloadXmlService.UpdateFlagAsync(downloadSchedulerItem.DownloadKey, (int)downloadSchedulerItem.DownloadStatus);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogService.WriteLog(LoggingLevel.Warning, "Pause all downloading list task failed.", ex);
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Warning, "Pause all downloading list task failed.", e);
            }
            finally
            {
                semaphoreSlim?.Release();
            }
        }

        /// <summary>
        /// 定时更新下载任务信息
        /// </summary>
        private static void DownloadSchedulerTimerElapsed(ThreadPoolTimer timer)
        {
            // need To do
        }

        /// <summary>
        /// 获取当前下载调度的所有任务信息，可能会进行其他操作，需要等待锁对象释放
        /// 该任务需要从调用该方法的同一线程中调用释放锁的方法
        /// </summary>
        public static List<DownloadSchedulerModel> GetDownloadSchedulerList()
        {
            semaphoreSlim?.Wait();
            List<DownloadSchedulerModel> downloadSchedulerList = new List<DownloadSchedulerModel>();

            try
            {
                foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerList)
                {
                    downloadSchedulerList.Add(downloadSchedulerItem);
                }
                return downloadSchedulerList;
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Get download information failed", e);
                return downloadSchedulerList;
            }
        }

        /// <summary>
        /// 在获取当前下载调度的所有任务信息后可能会进行其他操作，完成其他操作后，手动释放锁（需要再同一线程中执行，否则会引发异常）
        /// </summary>
        /// <returns>释放锁对象成功，返回 true，释放锁对象失败，返回 false，以便通知调用方以进行在释放失败时进行第二次调用</returns>
        public static bool FinishGetDownloadSchedulerList()
        {
            try
            {
                semaphoreSlim?.Release();
                return true;
            }
            catch (SynchronizationLockException e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Finish get download scheduler list failed", e);
                return false;
            }
        }
    }
}
