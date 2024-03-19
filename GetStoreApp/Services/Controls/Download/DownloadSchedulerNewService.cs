using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models.Controls.Download;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Services.Root;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;
using Windows.Networking.Connectivity;
using Windows.System.Threading;

namespace GetStoreApp.Services.Controls.Download
{
    /// <summary>
    /// 下载调度服务
    /// </summary>
    public static class DownloadSchedulerNewService
    {
        private static bool isUpdatingNow = false;
        private static int badgeCount = 0;
        private static ThreadPoolTimer downloadSchedulerTimer;
        private static SemaphoreSlim semaphoreSlim;

        public static bool IsNetWorkConnected { get; private set; }

        public static event Action<NotifyCollectionChangedAction, DownloadSchedulerModel> CollectionChanged;

        public static event Action<DownloadSchedulerModel> CollectionContentChanged;

        public static event Action<int> CollectionCountChanged;

        // 下载任务列表
        public static List<DownloadSchedulerModel> DownloadSchedulerList { get; } = new List<DownloadSchedulerModel>();

        #region 下载控制服务：自定义事件

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

        /// <summary>
        /// 网络状态发生变化的事件
        /// </summary>
        private static async void OnNetworkStatusChanged(object sender)
        {
            GetNetWorkInformation();

            // 网络未连接，暂停下载所有未完成的任务
            if (!IsNetWorkConnected)
            {
                await PauseAllTaskAsync();
            }
        }

        #endregion 下载控制服务：自定义事件

        /// <summary>
        /// 初始化后台下载调度器
        /// 先检查当前网络状态信息，加载暂停任务信息，然后初始化下载监控任务
        /// </summary>
        public static void InitializeDownloadScheduler()
        {
            GetNetWorkInformation();

            // 加载所有未完成任务下载信息
            List<DownloadSchedulerModel> unfinishedDownloadTaskList = DownloadStorageService.QueryDownloadData(false);

            // 更新当前下载任务数量通知
            BadgeNotificationService.Show(badgeCount);

            // 挂载集合数量发生更改事件
            CollectionCountChanged += OnCollectionCountChanged;

            // 挂载网络状态变化的事件
            NetworkInformation.NetworkStatusChanged += OnNetworkStatusChanged;

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

            // 卸载集合数量发生更改事件
            CollectionCountChanged -= OnCollectionCountChanged;

            // 卸载网络状态变化的事件
            NetworkInformation.NetworkStatusChanged -= OnNetworkStatusChanged;
        }

        /// <summary>
        /// 获取当前网络信息
        /// </summary>
        public static void GetNetWorkInformation()
        {
            // 查看当前网络状态信息
            // 查看是否开启了网络监控服务
            if (NetWorkMonitorService.NetWorkMonitorValue)
            {
                bool isConnected = false;
                bool isChecked = false;

                try
                {
                    ConnectionProfile connectionProfile = NetworkInformation.GetInternetConnectionProfile();
                    isConnected = connectionProfile != null && connectionProfile.GetNetworkConnectivityLevel() is NetworkConnectivityLevel.InternetAccess;
                    isChecked = true;
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Warning, "Network state check failed", e);
                }

                // 开启监控服务时，检查成功和网络处于连接状态时，代表网络已连接
                IsNetWorkConnected = isConnected && isChecked;

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
        }

        /// <summary>
        /// 添加下载任务
        /// </summary>
        public static async Task AddTaskAsync(DownloadSchedulerModel addItem, string operation)
        {
            if (semaphoreSlim is null)
            {
                return;
            }

            await semaphoreSlim.WaitAsync();

            addItem.DownloadStatus = DownloadStatus.Unknown;

            try
            {
                // 添加下载任务
                if (operation is "Add")
                {
                    // 添加下载记录
                    // 本地存储下载信息文件中添加下载信息
                    if (DownloadStorageService.AddDownloadData(addItem))
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
                        DownloadStorageService.UpdateDownloadStatusData(addItem.DownloadKey, addItem.DownloadStatus);
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

                    // 下载列表存在当前信息，修改
                    foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerList)
                    {
                        if (downloadSchedulerItem.DownloadKey.Equals(addItem.DownloadKey, StringComparison.OrdinalIgnoreCase))
                        {
                            downloadSchedulerItem.DownloadStatus = addItem.DownloadStatus;
                            downloadSchedulerItem.GID = addItem.GID;

                            // 下载调度列表修改下载信息，触发列表内容变化的事件
                            CollectionContentChanged?.Invoke(downloadSchedulerItem);
                            break;
                        }
                    }

                    // 本地存储下载信息文件中更新下载信息
                    DownloadStorageService.UpdateDownloadStatusData(addItem.DownloadKey, addItem.DownloadStatus);
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
            if (semaphoreSlim is null)
            {
                return;
            }

            await semaphoreSlim.WaitAsync();

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
                DownloadStorageService.UpdateDownloadStatusData(continueItem.DownloadKey, continueItem.DownloadStatus);
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
        public static async Task PauseTaskAsync(DownloadSchedulerModel pauseItem)
        {
            if (semaphoreSlim is null)
            {
                return;
            }

            await semaphoreSlim.WaitAsync();

            try
            {
                // 遍历当前下载列表，存在当前下载任务，尝试暂停下载，并修改下载信息
                foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerList)
                {
                    if (downloadSchedulerItem.DownloadKey.Equals(pauseItem.DownloadKey, StringComparison.OrdinalIgnoreCase))
                    {
                        // 处于正在下载状态或等待状态时，将任务停止，更改为暂停状态
                        if (pauseItem.DownloadStatus is DownloadStatus.Download || pauseItem.DownloadStatus is DownloadStatus.Wait)
                        {
                            // 从下载进程中暂停正在下载或等待下载的任务
                            (bool, string) pauseResult = await Aria2Service.PauseAsync(pauseItem.GID);

                            if (pauseResult.Item1)
                            {
                                downloadSchedulerItem.DownloadStatus = DownloadStatus.Pause;

                                // 下载调度列表修改下载信息，触发列表内容变化的事件
                                CollectionContentChanged?.Invoke(downloadSchedulerItem);
                                DownloadStorageService.UpdateDownloadStatusData(downloadSchedulerItem.DownloadKey, downloadSchedulerItem.DownloadStatus);

                                break;
                            }
                        }
                        // 处于未知状态时，将任务停止，更改为暂停状态
                        else if (pauseItem.DownloadStatus is DownloadStatus.Unknown)
                        {
                            downloadSchedulerItem.DownloadStatus = DownloadStatus.Pause;

                            // 下载调度列表修改下载信息，触发列表内容变化的事件
                            CollectionContentChanged?.Invoke(downloadSchedulerItem);
                            DownloadStorageService.UpdateDownloadStatusData(downloadSchedulerItem.DownloadKey, downloadSchedulerItem.DownloadStatus);

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
            if (semaphoreSlim is null)
            {
                return;
            }

            await semaphoreSlim.WaitAsync();

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
                                DownloadStorageService.UpdateDownloadStatusData(downloadSchedulerItem.DownloadKey, downloadSchedulerItem.DownloadStatus);
                            }
                        }
                        // 处于未知状态时，将任务停止，更改为暂停状态
                        else if (downloadSchedulerItem.DownloadStatus is DownloadStatus.Unknown)
                        {
                            downloadSchedulerItem.DownloadStatus = DownloadStatus.Pause;

                            // 下载调度列表修改下载信息，触发列表内容变化的事件
                            CollectionContentChanged?.Invoke(downloadSchedulerItem);
                            DownloadStorageService.UpdateDownloadStatusData(downloadSchedulerItem.DownloadKey, downloadSchedulerItem.DownloadStatus);
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
        /// 删除下载任务
        /// </summary>
        public static async Task DeleteTaskAsync(DownloadSchedulerModel deleteItem)
        {
            if (semaphoreSlim is null)
            {
                return;
            }

            await semaphoreSlim.WaitAsync();

            try
            {
                // 遍历当前下载列表，存在当前下载任务，尝试删除下载，并修改下载信息
                for (int index = DownloadSchedulerList.Count - 1; index >= 0; index--)
                {
                    DownloadSchedulerModel downloadSchedulerItem = DownloadSchedulerList[index];
                    if (downloadSchedulerItem.DownloadKey.Equals(deleteItem.DownloadKey, StringComparison.OrdinalIgnoreCase))
                    {
                        // 处于正在下载状态或等待状态时，将任务停止，并删除当前下载任务
                        if (deleteItem.DownloadStatus is DownloadStatus.Download || deleteItem.DownloadStatus is DownloadStatus.Wait)
                        {
                            // 从下载进程中删除正在下载或等待下载的任务
                            (bool, string) deleteResult = await Aria2Service.DeleteAsync(deleteItem.GID);

                            if (deleteResult.Item1)
                            {
                                DownloadSchedulerList.Remove(downloadSchedulerItem);

                                // 下载调度列表删除下载信息，触发列表删除的事件
                                CollectionChanged?.Invoke(NotifyCollectionChangedAction.Remove, downloadSchedulerItem);
                                CollectionCountChanged.Invoke(DownloadSchedulerList.Count);
                                DownloadStorageService.DeleteDownloadData(downloadSchedulerItem);

                                break;
                            }
                        }
                        // 处于未知状态时，将任务停止，更改为暂停状态
                        else if (deleteItem.DownloadStatus is DownloadStatus.Unknown)
                        {
                            DownloadSchedulerList.Remove(downloadSchedulerItem);

                            // 下载调度列表删除下载信息，触发列表删除的事件
                            CollectionChanged?.Invoke(NotifyCollectionChangedAction.Remove, downloadSchedulerItem);
                            CollectionCountChanged.Invoke(DownloadSchedulerList.Count);
                            DownloadStorageService.DeleteDownloadData(downloadSchedulerItem);

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
        /// 定时更新下载任务信息
        /// </summary>
        private static async void DownloadSchedulerTimerElapsed(ThreadPoolTimer timer)
        {
            if (semaphoreSlim is null)
            {
                return;
            }

            await semaphoreSlim.WaitAsync();

            try
            {
                foreach (DownloadSchedulerModel downloadSchedulerItem in DownloadSchedulerList)
                {
                    (bool, string, double, double, double) tellStatusResult = await Aria2Service.TellStatusAsync(downloadSchedulerItem.GID);

                    if (tellStatusResult.Item1)
                    {
                        // 当下载任务处于活动状态时，更新下载任务信息
                        if (tellStatusResult.Item2.Equals("active", StringComparison.OrdinalIgnoreCase))
                        {
                            downloadSchedulerItem.FinishedSize = tellStatusResult.Item3;
                            downloadSchedulerItem.TotalSize = tellStatusResult.Item4;
                            downloadSchedulerItem.CurrentSpeed = tellStatusResult.Item5;

                            // 下载调度列表修改下载信息，触发列表内容变化的事件
                            CollectionContentChanged?.Invoke(downloadSchedulerItem);
                        }
                        // 当下载任务处于等待状态时，将当前任务标记为等待状态
                        else if (tellStatusResult.Item2.Equals("waiting", StringComparison.OrdinalIgnoreCase))
                        {
                            downloadSchedulerItem.DownloadStatus = DownloadStatus.Wait;
                            downloadSchedulerItem.GID = string.Empty;
                            downloadSchedulerItem.FinishedSize = tellStatusResult.Item3;
                            downloadSchedulerItem.TotalSize = tellStatusResult.Item4;
                            downloadSchedulerItem.CurrentSpeed = tellStatusResult.Item5;

                            DownloadStorageService.UpdateDownloadStatusData(downloadSchedulerItem.DownloadKey, downloadSchedulerItem.DownloadStatus);

                            // 下载调度列表修改下载信息，触发列表内容变化的事件
                            CollectionContentChanged?.Invoke(downloadSchedulerItem);
                        }
                        // 当下载任务处于完成状态时，将当前任务标记为完成状态
                        else if (tellStatusResult.Item2.Equals("complete", StringComparison.OrdinalIgnoreCase))
                        {
                            downloadSchedulerItem.DownloadStatus = DownloadStatus.Completed;
                            downloadSchedulerItem.GID = string.Empty;
                            downloadSchedulerItem.FinishedSize = tellStatusResult.Item3;
                            downloadSchedulerItem.TotalSize = tellStatusResult.Item4;
                            downloadSchedulerItem.CurrentSpeed = tellStatusResult.Item5;

                            DownloadStorageService.UpdateDownloadStatusData(downloadSchedulerItem.DownloadKey, downloadSchedulerItem.DownloadStatus);
                            DownloadSchedulerList.Remove(downloadSchedulerItem);

                            // 下载调度列表删除下载信息，触发列表删除的事件
                            CollectionChanged?.Invoke(NotifyCollectionChangedAction.Remove, downloadSchedulerItem);
                            CollectionCountChanged.Invoke(DownloadSchedulerList.Count);
                        }
                        // 当下载任务处于错误状态时，将当前任务标记为错误状态
                        else if (tellStatusResult.Item2.Equals("error", StringComparison.OrdinalIgnoreCase))
                        {
                            downloadSchedulerItem.DownloadStatus = DownloadStatus.Error;
                            downloadSchedulerItem.GID = string.Empty;
                            downloadSchedulerItem.FinishedSize = tellStatusResult.Item3;
                            downloadSchedulerItem.TotalSize = tellStatusResult.Item4;
                            downloadSchedulerItem.CurrentSpeed = tellStatusResult.Item5;

                            DownloadStorageService.UpdateDownloadStatusData(downloadSchedulerItem.DownloadKey, downloadSchedulerItem.DownloadStatus);

                            DownloadSchedulerList.Remove(downloadSchedulerItem);

                            // 下载调度列表删除下载信息，触发列表删除的事件
                            CollectionChanged?.Invoke(NotifyCollectionChangedAction.Remove, downloadSchedulerItem);
                            CollectionCountChanged.Invoke(DownloadSchedulerList.Count);
                        }
                        // 其他状态暂停任务下载
                        else
                        {
                            downloadSchedulerItem.DownloadStatus = DownloadStatus.Pause;
                            downloadSchedulerItem.GID = string.Empty;

                            DownloadStorageService.UpdateDownloadStatusData(downloadSchedulerItem.DownloadKey, downloadSchedulerItem.DownloadStatus);

                            DownloadSchedulerList.Remove(downloadSchedulerItem);

                            // 下载调度列表删除下载信息，触发列表删除的事件
                            CollectionChanged?.Invoke(NotifyCollectionChangedAction.Remove, downloadSchedulerItem);
                            CollectionCountChanged.Invoke(DownloadSchedulerList.Count);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Schedule update download status failed", e);
            }
            finally
            {
                semaphoreSlim?.Release();
            }
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
