using GetStoreApp.Extensions.DataType.Classes;
using GetStoreApp.Extensions.DataType.Enums;
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

namespace GetStoreApp.Services.Download
{
    /// <summary>
    /// 后台智能传输服务
    /// </summary>
    public static class BitsService
    {
        private static readonly string displayName = nameof(GetStoreApp);
        private static readonly Lock bitsLock = new();
        private static readonly Guid CLSID_BackgroundCopyManager = new("4991D34B-80A1-4291-83B6-3328366B9097");

        private static IBackgroundCopyManager backgroundCopyManager;

        private static Dictionary<string, (string saveFilePath, IBackgroundCopyJob backgroundCopyJob, BackgroundCopyCallback backgroundCopyCallback)> BitsDict { get; } = [];

        public static event Action<DownloadProgress> DownloadProgress;

        /// <summary>
        /// 初始化后台智能传输服务
        /// </summary>
        public static void Initialize()
        {
            if (backgroundCopyManager is null)
            {
                Task.Factory.StartNew((param) =>
                {
                    try
                    {
                        int createResult = Ole32Library.CoCreateInstance(CLSID_BackgroundCopyManager, nint.Zero, CLSCTX.CLSCTX_LOCAL_SERVER, typeof(IBackgroundCopyManager).GUID, out nint ppv);

                        if (createResult is 0)
                        {
                            backgroundCopyManager = (IBackgroundCopyManager)Program.StrategyBasedComWrappers.GetOrCreateObjectForComInstance(ppv, CreateObjectFlags.None);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(BitsService), nameof(Initialize), 1, e);
                    }
                }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
            }
        }

        /// <summary>
        /// 终止所有下载任务，仅用于应用关闭时
        /// </summary>
        public static void TerminateDownload()
        {
            Task.Factory.StartNew((param) =>
            {
                bitsLock.Enter();

                try
                {
                    foreach (KeyValuePair<string, (string saveFilePath, IBackgroundCopyJob backgroundCopyJob, BackgroundCopyCallback backgroundCopyCallback)> bits in BitsDict)
                    {
                        bits.Value.backgroundCopyJob.Cancel();
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(BitsService), nameof(TerminateDownload), 1, e);
                }
                finally
                {
                    bitsLock.Exit();
                }
            }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        /// <summary>
        /// 使用下载链接创建下载
        /// </summary>
        public static void CreateDownload(string url, string saveFilePath)
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
                            DownloadID = Convert.ToString(downloadID)
                        };
                        backgroundCopyCallback.StatusChanged += OnStatusChanged;
                        downloadJob.SetNotifyInterface(Program.StrategyBasedComWrappers.GetOrCreateComInterfaceForObject(new UnknownWrapper(backgroundCopyCallback).WrappedObject, CreateComInterfaceFlags.None));
                        downloadJob.GetProgress(out BG_JOB_PROGRESS progress);

                        bitsLock.Enter();

                        try
                        {
                            BitsDict.TryAdd(backgroundCopyCallback.DownloadID, ValueTuple.Create(saveFilePath, downloadJob, backgroundCopyCallback));
                        }
                        catch (Exception e)
                        {
                            ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                        }
                        finally
                        {
                            bitsLock.Exit();
                        }

                        DownloadProgress?.Invoke(new DownloadProgress()
                        {
                            DownloadID = backgroundCopyCallback.DownloadID,
                            DownloadProgressState = DownloadProgressState.Queued,
                            FileName = Path.GetFileName(saveFilePath),
                            FilePath = saveFilePath,
                            DownloadSpeed = 0,
                            CompletedSize = 0,
                            TotalSize = 0,
                        });

                        downloadJob.Resume();
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(BitsService), nameof(CreateDownload), 1, e);
                }
            }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        /// <summary>
        /// 继续下载
        /// </summary>
        public static void ContinueDownload(string downloadID)
        {
            Task.Factory.StartNew((param) =>
            {
                bitsLock.Enter();

                try
                {
                    if (BitsDict.TryGetValue(downloadID, out (string saveFilePath, IBackgroundCopyJob backgroundCopyJob, BackgroundCopyCallback backgroundCopyCallback) downloadValue))
                    {
                        int continueResult = downloadValue.backgroundCopyJob.Resume();

                        if (continueResult is 0)
                        {
                            DownloadProgress?.Invoke(new DownloadProgress()
                            {
                                DownloadID = downloadID,
                                DownloadProgressState = DownloadProgressState.Queued,
                                FileName = Path.GetFileName(downloadValue.saveFilePath),
                                FilePath = downloadValue.saveFilePath,
                                DownloadSpeed = 0,
                                CompletedSize = 0,
                                TotalSize = 0,
                            });
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(BitsService), nameof(ContinueDownload), 1, e);
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
        public static void PauseDownload(string downloadID)
        {
            Task.Factory.StartNew((param) =>
            {
                bitsLock.Enter();

                try
                {
                    if (BitsDict.TryGetValue(downloadID, out (string saveFilePath, IBackgroundCopyJob backgroundCopyJob, BackgroundCopyCallback backgroundCopyCallback) downloadValue))
                    {
                        int pauseResult = downloadValue.backgroundCopyJob.Suspend();

                        if (pauseResult is 0)
                        {
                            DownloadProgress?.Invoke(new DownloadProgress()
                            {
                                DownloadID = downloadID,
                                DownloadProgressState = DownloadProgressState.Paused,
                                FileName = Path.GetFileName(downloadValue.saveFilePath),
                                FilePath = downloadValue.saveFilePath,
                                DownloadSpeed = 0,
                                CompletedSize = 0,
                                TotalSize = 0,
                            });
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(BitsService), nameof(PauseDownload), 1, e);
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
        public static void DeleteDownload(string downloadID)
        {
            Task.Factory.StartNew((param) =>
            {
                bitsLock.Enter();

                try
                {
                    if (BitsDict.TryGetValue(downloadID, out (string saveFilePath, IBackgroundCopyJob backgroundCopyJob, BackgroundCopyCallback backgroundCopyCallback) downloadValue))
                    {
                        int deleteResult = downloadValue.backgroundCopyJob.Cancel();

                        if (deleteResult is 0)
                        {
                            downloadValue.backgroundCopyCallback.StatusChanged -= OnStatusChanged;
                            BitsDict.Remove(downloadID);
                            DownloadProgress?.Invoke(new DownloadProgress()
                            {
                                DownloadID = downloadID,
                                DownloadProgressState = DownloadProgressState.Deleted,
                                FileName = Path.GetFileName(downloadValue.saveFilePath),
                                FilePath = downloadValue.saveFilePath,
                                DownloadSpeed = 0,
                                CompletedSize = 0,
                                TotalSize = 0,
                            });
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(BitsService), nameof(DeleteDownload), 1, e);
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
            // 下载文件中
            if (state is BG_JOB_STATE.BG_JOB_STATE_TRANSFERRING)
            {
                downloadJob.GetProgress(out BG_JOB_PROGRESS progress);

                if (BitsDict.TryGetValue(callback.DownloadID, out (string saveFilePath, IBackgroundCopyJob backgroundCopyJob, BackgroundCopyCallback backgroundCopyCallback) downloadValue))
                {
                    DownloadProgress?.Invoke(new DownloadProgress()
                    {
                        DownloadID = callback.DownloadID,
                        DownloadProgressState = DownloadProgressState.Downloading,
                        FileName = Path.GetFileName(downloadValue.saveFilePath),
                        FilePath = downloadValue.saveFilePath,
                        DownloadSpeed = 0,
                        CompletedSize = progress.BytesTransferred,
                        TotalSize = progress.BytesTotal,
                    });
                }
            }
            // 下载完成
            else if (state is BG_JOB_STATE.BG_JOB_STATE_TRANSFERRED)
            {
                downloadJob.GetProgress(out BG_JOB_PROGRESS progress);

                try
                {
                    callback.StatusChanged -= OnStatusChanged;
                    downloadJob.Complete();

                    bitsLock.Enter();

                    try
                    {
                        if (BitsDict.TryGetValue(callback.DownloadID, out (string saveFilePath, IBackgroundCopyJob backgroundCopyJob, BackgroundCopyCallback backgroundCopyCallback) downloadValue))
                        {
                            DownloadProgress?.Invoke(new DownloadProgress()
                            {
                                DownloadID = callback.DownloadID,
                                DownloadProgressState = DownloadProgressState.Finished,
                                FileName = Path.GetFileName(downloadValue.saveFilePath),
                                FilePath = downloadValue.saveFilePath,
                                DownloadSpeed = 0,
                                CompletedSize = progress.BytesTransferred,
                                TotalSize = progress.BytesTotal,
                            });
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
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(BitsService), nameof(OnStatusChanged), 1, e);
                }
            }
            // 下载错误
            else if (state is BG_JOB_STATE.BG_JOB_STATE_ERROR || state is BG_JOB_STATE.BG_JOB_STATE_TRANSIENT_ERROR)
            {
                downloadJob.GetProgress(out BG_JOB_PROGRESS progress);

                try
                {
                    callback.StatusChanged -= OnStatusChanged;

                    bitsLock.Enter();

                    try
                    {
                        if (BitsDict.TryGetValue(callback.DownloadID, out (string saveFilePath, IBackgroundCopyJob backgroundCopyJob, BackgroundCopyCallback backgroundCopyCallback) downloadValue))
                        {
                            DownloadProgress?.Invoke(new DownloadProgress()
                            {
                                DownloadID = callback.DownloadID,
                                DownloadProgressState = DownloadProgressState.Failed,
                                FileName = Path.GetFileName(downloadValue.saveFilePath),
                                FilePath = downloadValue.saveFilePath,
                                DownloadSpeed = 0,
                                CompletedSize = progress.BytesTransferred,
                                TotalSize = progress.BytesTotal,
                            });
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
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(BitsService), nameof(OnStatusChanged), 2, e);
                }
            }
        }
    }
}
