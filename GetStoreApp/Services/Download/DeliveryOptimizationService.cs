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
    /// 传递优化服务
    /// </summary>
    public static class DeliveryOptimizationService
    {
        private static readonly string displayName = "GetStoreApp";
        private static readonly Lock deliveryOptimizationLock = new();
        private static readonly Guid CLSID_DeliveryOptimization = new("5B99FA76-721C-423C-ADAC-56D03C8A8007");

        private static Dictionary<string, (string saveFilePath, IDODownload doDownload, DODownloadStatusCallback doDownloadStatusCallback)> DeliveryOptimizationDict { get; } = [];

        public static event Action<DownloadProgress> DownloadProgress;

        /// <summary>
        /// 应用关闭时终止所有下载任务
        /// </summary>
        public static void TerminateDownload()
        {
            Task.Factory.StartNew((param) =>
            {
                deliveryOptimizationLock.Enter();

                try
                {
                    foreach (KeyValuePair<string, (string saveFilePath, IDODownload doDownload, DODownloadStatusCallback doDownloadStatusCallback)> deliveryOptimization in DeliveryOptimizationDict)
                    {
                        deliveryOptimization.Value.doDownload.Abort();
                    }

                    DeliveryOptimizationDict.Clear();
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(DeliveryOptimizationService), nameof(TerminateDownload), 1, e);
                }
                finally
                {
                    deliveryOptimizationLock.Exit();
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
                    IDOManager doManager = null;
                    IDODownload doDownload = null;

                    // 创建 IDoManager
                    int createResult = Ole32Library.CoCreateInstance(CLSID_DeliveryOptimization, nint.Zero, CLSCTX.CLSCTX_LOCAL_SERVER, typeof(IDOManager).GUID, out nint ppv);

                    if (createResult is 0)
                    {
                        doManager = (IDOManager)Program.StrategyBasedComWrappers.GetOrCreateObjectForComInstance(ppv, CreateObjectFlags.None);
                        doManager.CreateDownload(out doDownload);
                        ComWrappers.TryGetComInstance(doDownload, out nint doDownloadPointer);
                        _ = Ole32Library.CoSetProxyBlanket(doDownloadPointer, uint.MaxValue, uint.MaxValue, new nint(-1), 0, 3, nint.Zero, 32);
                        _ = Ole32Library.CoSetProxyBlanket(doDownloadPointer, uint.MaxValue, uint.MaxValue, new nint(-1), 0, 3, nint.Zero, 32);

                        // 添加下载信息
                        ComVariant displayNameVariant = ComVariant.Create(displayName);
                        doDownload.SetProperty(DODownloadProperty.DODownloadProperty_DisplayName, displayNameVariant);
                        ComVariant urlVariant = ComVariant.Create(url);
                        doDownload.SetProperty(DODownloadProperty.DODownloadProperty_Uri, urlVariant);
                        ComVariant filePathVariant = ComVariant.Create(saveFilePath);
                        doDownload.SetProperty(DODownloadProperty.DODownloadProperty_LocalPath, filePathVariant);
                        doDownload.GetProperty(DODownloadProperty.DODownloadProperty_Id, out ComVariant idVarient);
                        string downloadID = idVarient.As<string>();
                        DODownloadStatusCallback doDownloadStatusCallback = new()
                        {
                            DownloadID = downloadID
                        };
                        doDownloadStatusCallback.StatusChanged += OnStatusChanged;

                        ComVariant callbackInterfaceVariant = ComVariant.CreateRaw(VarEnum.VT_UNKNOWN, Program.StrategyBasedComWrappers.GetOrCreateComInterfaceForObject(new UnknownWrapper(doDownloadStatusCallback).WrappedObject, CreateComInterfaceFlags.None));
                        doDownload.SetProperty(DODownloadProperty.DODownloadProperty_CallbackInterface, callbackInterfaceVariant);
                        ComVariant foregroundVariant = ComVariant.Create(true);
                        doDownload.SetProperty(DODownloadProperty.DODownloadProperty_ForegroundPriority, foregroundVariant);

                        deliveryOptimizationLock.Enter();

                        try
                        {
                            DeliveryOptimizationDict.TryAdd(downloadID, ValueTuple.Create(saveFilePath, doDownload, doDownloadStatusCallback));
                        }
                        catch (Exception e)
                        {
                            ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                        }
                        finally
                        {
                            deliveryOptimizationLock.Exit();
                        }

                        DownloadProgress?.Invoke(new DownloadProgress()
                        {
                            DownloadID = doDownloadStatusCallback.DownloadID,
                            DownloadProgressState = DownloadProgressState.Queued,
                            FileName = Path.GetFileName(saveFilePath),
                            FilePath = saveFilePath,
                            DownloadSpeed = 0,
                            CompletedSize = 0,
                            TotalSize = 0,
                        });

                        doDownload.Start(nint.Zero);
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(DeliveryOptimizationService), nameof(CreateDownload), 1, e);
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
                deliveryOptimizationLock.Enter();

                try
                {
                    if (DeliveryOptimizationDict.TryGetValue(downloadID, out (string saveFilePath, IDODownload doDownload, DODownloadStatusCallback doDownloadStatusCallback) downloadValue))
                    {
                        int continueResult = downloadValue.doDownload.Start(nint.Zero);

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
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(DeliveryOptimizationService), nameof(ContinueDownload), 1, e);
                }
                finally
                {
                    deliveryOptimizationLock.Exit();
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
                deliveryOptimizationLock.Enter();

                try
                {
                    if (DeliveryOptimizationDict.TryGetValue(downloadID, out (string saveFilePath, IDODownload doDownload, DODownloadStatusCallback doDownloadStatusCallback) downloadValue))
                    {
                        int pauseResult = downloadValue.doDownload.Pause();

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
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(DeliveryOptimizationService), nameof(PauseDownload), 1, e);
                }
                finally
                {
                    deliveryOptimizationLock.Exit();
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
                deliveryOptimizationLock.Enter();

                try
                {
                    if (DeliveryOptimizationDict.TryGetValue(downloadID, out (string saveFilePath, IDODownload doDownload, DODownloadStatusCallback doDownloadStatusCallback) downloadValue))
                    {
                        int deleteResult = downloadValue.doDownload.Abort();

                        if (deleteResult is 0)
                        {
                            downloadValue.doDownloadStatusCallback.StatusChanged -= OnStatusChanged;
                            DeliveryOptimizationDict.Remove(downloadID);
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
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(DeliveryOptimizationService), nameof(DeleteDownload), 1, e);
                }
                finally
                {
                    deliveryOptimizationLock.Exit();
                }
            }, null, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        /// <summary>
        /// 下载状态发生变化触发的事件
        /// </summary>
        private static void OnStatusChanged(DODownloadStatusCallback callback, IDODownload doDownload, DO_DOWNLOAD_STATUS status)
        {
            // 下载文件中
            if (status.State is DODownloadState.DODownloadState_Transferring)
            {
                if (DeliveryOptimizationDict.TryGetValue(callback.DownloadID, out (string saveFilePath, IDODownload doDownload, DODownloadStatusCallback doDownloadStatusCallback) downloadValue))
                {
                    DownloadProgress?.Invoke(new DownloadProgress()
                    {
                        DownloadID = callback.DownloadID,
                        DownloadProgressState = DownloadProgressState.Downloading,
                        FileName = Path.GetFileName(downloadValue.saveFilePath),
                        FilePath = downloadValue.saveFilePath,
                        DownloadSpeed = 0,
                        CompletedSize = status.BytesTransferred,
                        TotalSize = status.BytesTotal,
                    });
                }
            }
            // 下载完成
            else if (status.State is DODownloadState.DODownloadState_Transferred)
            {
                try
                {
                    callback.StatusChanged -= OnStatusChanged;
                    doDownload.Finalize();

                    deliveryOptimizationLock.Enter();

                    try
                    {
                        if (DeliveryOptimizationDict.TryGetValue(callback.DownloadID, out (string saveFilePath, IDODownload doDownload, DODownloadStatusCallback doDownloadStatusCallback) downloadValue))
                        {
                            DownloadProgress?.Invoke(new DownloadProgress()
                            {
                                DownloadID = callback.DownloadID,
                                DownloadProgressState = DownloadProgressState.Finished,
                                FileName = Path.GetFileName(downloadValue.saveFilePath),
                                FilePath = downloadValue.saveFilePath,
                                DownloadSpeed = 0,
                                CompletedSize = status.BytesTransferred,
                                TotalSize = status.BytesTotal,
                            });

                            DeliveryOptimizationDict.Remove(callback.DownloadID);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                    }
                    finally
                    {
                        deliveryOptimizationLock.Exit();
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(DeliveryOptimizationService), nameof(OnStatusChanged), 1, e);
                }
            }

            // 下载错误
            if (status.Error is not 0 || status.ExtendedError is not 0)
            {
                try
                {
                    callback.StatusChanged -= OnStatusChanged;

                    deliveryOptimizationLock.Enter();

                    try
                    {
                        if (DeliveryOptimizationDict.TryGetValue(callback.DownloadID, out (string saveFilePath, IDODownload doDownload, DODownloadStatusCallback doDownloadStatusCallback) downloadValue))
                        {
                            DownloadProgress?.Invoke(new DownloadProgress()
                            {
                                DownloadID = callback.DownloadID,
                                DownloadProgressState = DownloadProgressState.Failed,
                                FileName = Path.GetFileName(downloadValue.saveFilePath),
                                FilePath = downloadValue.saveFilePath,
                                DownloadSpeed = 0,
                                CompletedSize = status.BytesTransferred,
                                TotalSize = status.BytesTotal,
                            });

                            DeliveryOptimizationDict.Remove(callback.DownloadID);
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                    }
                    finally
                    {
                        deliveryOptimizationLock.Exit();
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(DeliveryOptimizationService), nameof(OnStatusChanged), 2, e);
                }
            }
        }
    }
}
