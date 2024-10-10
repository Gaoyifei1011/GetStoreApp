using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.ComTypes;
using GetStoreApp.WindowsAPI.PInvoke.Ole32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;

namespace GetStoreApp.Services.Controls.Download
{
    /// <summary>
    /// 后台智能传输服务
    /// </summary>
    public static class BitsService
    {
        private static readonly string displayName = "GetStoreApp";
        private static readonly Lock bitsLock = new();
        private static readonly Guid CLSID_BackgroundCopyManager = new("4991D34B-80A1-4291-83B6-3328366B9097");

        private static IBackgroundCopyManager backgroundCopyManager;

        private static Dictionary<Guid, Tuple<IBackgroundCopyJob, BackgroundCopyCallback>> BitsDict { get; } = [];

        public static event Action<Guid, string, string, string, double> DownloadCreated;

        public static event Action<Guid> DownloadContinued;

        public static event Action<Guid> DownloadPaused;

        public static event Action<Guid> DownloadDeleted;

        public static event Action<Guid, BG_JOB_PROGRESS> DownloadProgressing;

        public static event Action<Guid, BG_JOB_PROGRESS> DownloadCompleted;

        /// <summary>
        /// 初始化后台智能传输服务
        /// </summary>
        public static unsafe void Initialize()
        {
            if (backgroundCopyManager is null)
            {
                Task.Factory.StartNew((param) =>
                {
                    try
                    {
                        int createResult = Ole32Library.CoCreateInstance(CLSID_BackgroundCopyManager, IntPtr.Zero, CLSCTX.CLSCTX_LOCAL_SERVER, typeof(IBackgroundCopyManager).GUID, out IntPtr ppv);

                        if (createResult is 0)
                        {
                            backgroundCopyManager = ComInterfaceMarshaller<IBackgroundCopyManager>.ConvertToManaged((void*)ppv);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "Initialize background intelligent transfer service failed", e);
                    }
                }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
            }
        }

        /// <summary>
        /// 获取下载任务的数量
        /// </summary>
        public static int GetDownloadCount()
        {
            int count = 0;
            bitsLock.Enter();

            try
            {
                count = BitsDict.Count;
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
            }
            finally
            {
                bitsLock.Exit();
            }

            return count;
        }

        /// <summary>
        /// 终止所有下载任务，仅用于应用关闭时
        /// </summary>
        public static void TerminateDownload()
        {
            Task.Factory.StartNew((param) =>
            {
                try
                {
                    if (GetDownloadCount() > 0)
                    {
                        bitsLock.Enter();

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
                        finally
                        {
                            bitsLock.Exit();
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "Terminate background transfer intelligent service download all task failed", e);
                }
            }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        /// <summary>
        /// 使用下载链接创建下载
        /// </summary>
        public static unsafe void CreateDownload(string url, string saveFilePath)
        {
            Task.Factory.StartNew((param) =>
            {
                try
                {
                    if (backgroundCopyManager is not null)
                    {
                        backgroundCopyManager.CreateJob(displayName, BG_JOB_TYPE.BG_JOB_TYPE_DOWNLOAD, out Guid downloadID, out IBackgroundCopyJob downloadJob);
                        downloadJob.AddFile(url, saveFilePath);
                        downloadJob.SetNotifyFlags(BG_JOB_NOTIFICATION_TYPE.BG_NOTIFY_FILE_RANGES_TRANSFERRED | BG_JOB_NOTIFICATION_TYPE.BG_NOTIFY_JOB_ERROR | BG_JOB_NOTIFICATION_TYPE.BG_NOTIFY_JOB_MODIFICATION);
                        BackgroundCopyCallback backgroundCopyCallback = new()
                        {
                            DownloadID = downloadID
                        };
                        backgroundCopyCallback.StatusChanged += OnStatusChanged;
                        downloadJob.SetNotifyInterface((IntPtr)ComInterfaceMarshaller<object>.ConvertToUnmanaged(new UnknownWrapper(backgroundCopyCallback).WrappedObject));

                        downloadJob.GetProgress(out BG_JOB_PROGRESS progress);
                        DownloadCreated?.Invoke(backgroundCopyCallback.DownloadID, Path.GetFileName(saveFilePath), saveFilePath, url, progress.BytesTotal is ulong.MaxValue ? 0 : progress.BytesTotal);

                        bitsLock.Enter();

                        try
                        {
                            BitsDict.TryAdd(downloadID, Tuple.Create(downloadJob, backgroundCopyCallback));
                        }
                        catch (Exception e)
                        {
                            ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                        }
                        finally
                        {
                            bitsLock.Exit();
                        }

                        downloadJob.Resume();
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "Create background intelligent transfer service download failed", e);
                }
            }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        /// <summary>
        /// 继续下载
        /// </summary>
        public static void ContinueDownload(Guid downloadID)
        {
            Task.Factory.StartNew((param) =>
            {
                bitsLock.Enter();

                try
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
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "Continue background intelligent transfer service download failed", e);
                }
                finally
                {
                    bitsLock.Exit();
                }
            }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        /// <summary>
        /// 暂停下载
        /// </summary>
        public static void PauseDownload(Guid downloadID)
        {
            Task.Factory.StartNew((param) =>
            {
                bitsLock.Enter();

                try
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
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "Pause background intelligent transfer service download failed", e);
                }
                finally
                {
                    bitsLock.Exit();
                }
            }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        /// <summary>
        /// 删除下载
        /// </summary>
        public static void DeleteDownload(Guid downloadID)
        {
            Task.Factory.StartNew((param) =>
            {
                bitsLock.Enter();

                try
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
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "Delete background intelligent transfer service download failed", e);
                }
                finally
                {
                    bitsLock.Exit();
                }
            }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
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

                    bitsLock.Enter();

                    try
                    {
                        if (BitsDict.ContainsKey(callback.DownloadID))
                        {
                            BitsDict.Remove(callback.DownloadID);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                    }
                    finally
                    {
                        bitsLock.Exit();
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
