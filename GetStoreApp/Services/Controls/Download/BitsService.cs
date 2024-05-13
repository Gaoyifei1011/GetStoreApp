using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.ComTypes;
using GetStoreApp.WindowsAPI.PInvoke.Ole32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Windows.Foundation.Diagnostics;

namespace GetStoreApp.Services.Controls.Download
{
    /// <summary>
    /// 后台智能传输服务
    /// </summary>
    public static class BitsService
    {
        private static string displayName = "GetStoreApp";
        private static object bitsLock = new object();
        private static Guid CLSID_BackgroundCopyManager = new Guid("4991D34B-80A1-4291-83B6-3328366B9097");
        private static Guid IID_IBackgroundCopyManager = new Guid("5CE34C0D-0DC9-4C1F-897C-DAA1B78CEE7C");
        private static BackgroundWorker backgroundWorker = new BackgroundWorker();
        private static StrategyBasedComWrappers strategyBasedComWrappers = new StrategyBasedComWrappers();
        private static IBackgroundCopyManager backgroundCopyManager;

        private static Dictionary<Guid, Tuple<IBackgroundCopyJob, BackgroundCopyCallback>> BitsDict { get; } = new Dictionary<Guid, Tuple<IBackgroundCopyJob, BackgroundCopyCallback>>();

        public static event Action<Guid, string, string, string, double> DownloadCreated;

        public static event Action<Guid> DownloadContinued;

        public static event Action<Guid> DownloadPaused;

        public static event Action<Guid> DownloadDeleted;

        public static event Action<Guid, BG_JOB_PROGRESS> DownloadProgressing;

        public static event Action<Guid, BG_JOB_PROGRESS> DownloadCompleted;

        /// <summary>
        /// 初始化后台智能传输服务
        /// </summary>
        public static void Initialize()
        {
            if (backgroundCopyManager is null && backgroundWorker is not null)
            {
                backgroundWorker.DoWork += OnDoWork;

                try
                {
                    while (backgroundWorker.IsBusy) continue;
                    backgroundWorker.RunWorkerAsync(new object[] { "Initialize" });
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "Initialize background intelligent transfer service failed", e);
                }
            }
        }

        /// <summary>
        /// 注销后台智能传输服务
        /// </summary>
        public static void UnInitialize()
        {
            if (backgroundWorker is not null)
            {
                backgroundWorker.DoWork -= OnDoWork;
                backgroundWorker.Dispose();
                backgroundWorker = null;
            }
        }

        /// <summary>
        /// 在单独的线程中执行后台智能传输服务
        /// </summary>
        private static void OnDoWork(object sender, DoWorkEventArgs args)
        {
            object[] parameter = args.Argument as object[];

            if (parameter.Length > 0)
            {
                if (parameter[0] is "Initialize")
                {
                    int createResult = Ole32Library.CoCreateInstance(ref CLSID_BackgroundCopyManager, IntPtr.Zero, CLSCTX.CLSCTX_LOCAL_SERVER, ref IID_IBackgroundCopyManager, out IntPtr ppv);

                    if (createResult is 0)
                    {
                        backgroundCopyManager = (IBackgroundCopyManager)strategyBasedComWrappers.GetOrCreateObjectForComInstance(ppv, CreateObjectFlags.None);
                    }
                }
                else if (parameter[0] is "CreateDownload")
                {
                    string url = Convert.ToString(parameter[1]);
                    string saveFilePath = Convert.ToString(parameter[2]);

                    try
                    {
                        if (backgroundCopyManager is not null)
                        {
                            backgroundCopyManager.CreateJob(displayName, BG_JOB_TYPE.BG_JOB_TYPE_DOWNLOAD, out Guid downloadID, out IBackgroundCopyJob downloadJob);
                            downloadJob.AddFile(url, saveFilePath);
                            downloadJob.SetNotifyFlags(BG_JOB_NOTIFICATION_TYPE.BG_NOTIFY_FILE_RANGES_TRANSFERRED | BG_JOB_NOTIFICATION_TYPE.BG_NOTIFY_JOB_ERROR | BG_JOB_NOTIFICATION_TYPE.BG_NOTIFY_JOB_MODIFICATION);
                            BackgroundCopyCallback backgroundCopyCallback = new BackgroundCopyCallback();
                            backgroundCopyCallback.DownloadID = downloadID;
                            backgroundCopyCallback.StatusChanged += OnStatusChanged;
                            downloadJob.SetNotifyInterface(strategyBasedComWrappers.GetOrCreateComInterfaceForObject(new UnknownWrapper(backgroundCopyCallback).WrappedObject, CreateComInterfaceFlags.None));

                            downloadJob.GetProgress(out BG_JOB_PROGRESS progress);
                            DownloadCreated?.Invoke(backgroundCopyCallback.DownloadID, Path.GetFileName(saveFilePath), saveFilePath, url, progress.BytesTotal);

                            lock (bitsLock)
                            {
                                BitsDict.TryAdd(downloadID, Tuple.Create(downloadJob, backgroundCopyCallback));
                            }

                            downloadJob.Resume();
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "Create background intelligent transfer service download failed", e);
                    }
                }
                else if (parameter[0] is "ContinueDownload")
                {
                    Guid downloadID = (Guid)parameter[1];

                    try
                    {
                        lock (bitsLock)
                        {
                            if (BitsDict.TryGetValue(downloadID, out Tuple<IBackgroundCopyJob, BackgroundCopyCallback> downloadValue))
                            {
                                int continueResult = downloadValue.Item1.Resume();

                                if (continueResult is 0)
                                {
                                    DownloadContinued?.Invoke(downloadID);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "Continue background intelligent transfer service download failed", e);
                    }
                }
                else if (parameter[0] is "PauseDownload")
                {
                    Guid downloadID = (Guid)parameter[1];

                    try
                    {
                        lock (bitsLock)
                        {
                            if (BitsDict.TryGetValue(downloadID, out Tuple<IBackgroundCopyJob, BackgroundCopyCallback> downloadValue))
                            {
                                int pauseResult = downloadValue.Item1.Suspend();

                                if (pauseResult is 0)
                                {
                                    DownloadPaused?.Invoke(downloadID);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "Pause background intelligent transfer service download failed", e);
                    }
                }
                else if (parameter[0] is "DeleteDownload")
                {
                    Guid downloadID = (Guid)parameter[1];

                    try
                    {
                        lock (bitsLock)
                        {
                            if (BitsDict.TryGetValue(downloadID, out Tuple<IBackgroundCopyJob, BackgroundCopyCallback> downloadValue))
                            {
                                int deleteResult = downloadValue.Item1.Cancel();

                                if (deleteResult is 0)
                                {
                                    downloadValue.Item2.StatusChanged -= OnStatusChanged;
                                    DownloadDeleted?.Invoke(downloadID);
                                    BitsDict.Remove(downloadID);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "Delete background intelligent transfer service download failed", e);
                    }
                }
                else if (parameter[0] is "TerminateDownload")
                {
                    if (GetDownloadCount() > 0)
                    {
                        lock (bitsLock)
                        {
                            try
                            {
                                foreach (KeyValuePair<Guid, Tuple<IBackgroundCopyJob, BackgroundCopyCallback>> bitsKeyValue in BitsDict)
                                {
                                    bitsKeyValue.Value.Item1.Cancel();
                                }
                            }
                            catch (Exception e)
                            {
                                LogService.WriteLog(LoggingLevel.Error, "Terminate all task failed", e);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取下载任务的数量
        /// </summary>
        public static int GetDownloadCount()
        {
            int count = 0;
            lock (bitsLock)
            {
                count = BitsDict.Count;
            }
            return count;
        }

        /// <summary>
        /// 终止所有下载任务，仅用于应用关闭时
        /// </summary>
        public static void TerminateDownload()
        {
            try
            {
                while (backgroundWorker.IsBusy) continue;
                backgroundWorker.RunWorkerAsync(new object[1] { "TerminateDownload" });
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Terminate background transfer intelligent service download all task failed", e);
            }
        }

        /// <summary>
        /// 使用下载链接创建下载
        /// </summary>
        public static unsafe void CreateDownload(string url, string saveFilePath)
        {
            try
            {
                while (backgroundWorker.IsBusy) continue;
                backgroundWorker.RunWorkerAsync(new object[3] { "CreateDownload", url, saveFilePath });
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Create background transfer intelligent service download task failed", e);
            }
        }

        /// <summary>
        /// 继续下载
        /// </summary>
        public static void ContinueDownload(Guid downloadID)
        {
            try
            {
                while (backgroundWorker.IsBusy) continue;
                backgroundWorker.RunWorkerAsync(new object[2] { "ContinueDownload", downloadID });
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Continue background transfer intelligent service download task failed", e);
            }
        }

        /// <summary>
        /// 暂停下载
        /// </summary>
        public static void PauseDownload(Guid downloadID)
        {
            try
            {
                while (backgroundWorker.IsBusy) continue;
                backgroundWorker.RunWorkerAsync(new object[2] { "PauseDownload", downloadID });
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Pause background transfer intelligent service download task failed", e);
            }
        }

        /// <summary>
        /// 删除下载
        /// </summary>
        public static void DeleteDownload(Guid downloadID)
        {
            try
            {
                while (backgroundWorker.IsBusy) continue;
                backgroundWorker.RunWorkerAsync(new object[2] { "DeleteDownload", downloadID });
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Delete background transfer intelligent service download task failed", e);
            }
        }

        /// <summary>
        /// 下载状态发生变化触发的事件
        /// </summary>
        private static void OnStatusChanged(BackgroundCopyCallback callback, IBackgroundCopyJob downloadJob, BG_JOB_STATE state)
        {
            if (state is BG_JOB_STATE.BG_JOB_STATE_TRANSFERRING)
            {
                downloadJob.GetProgress(out BG_JOB_PROGRESS progress);
                DownloadProgressing?.Invoke(callback.DownloadID, progress);
            }
            else if (state is BG_JOB_STATE.BG_JOB_STATE_TRANSFERRED)
            {
                downloadJob.GetProgress(out BG_JOB_PROGRESS progress);
                DownloadCompleted?.Invoke(callback.DownloadID, progress);

                try
                {
                    callback.StatusChanged -= OnStatusChanged;
                    downloadJob.Complete();

                    lock (bitsLock)
                    {
                        if (BitsDict.ContainsKey(callback.DownloadID))
                        {
                            BitsDict.Remove(callback.DownloadID);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Warning, "Finalize background intelligent transfer service download task failed", e);
                }
            }
            else if (state is BG_JOB_STATE.BG_JOB_STATE_SUSPENDED)
            {
                DownloadPaused?.Invoke(callback.DownloadID);
            }
        }
    }
}
