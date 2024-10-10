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
    /// 传递优化服务（仅支持 Winodws 11 22621 及更高版本）
    /// </summary>
    public static class DeliveryOptimizationService
    {
        private static readonly string displayName = "GetStoreApp";
        private static readonly Lock deliveryOptimizationLock = new();
        private static readonly Guid CLSID_DeliveryOptimization = new("5B99FA76-721C-423C-ADAC-56D03C8A8007");

        private static Dictionary<Guid, Tuple<IDODownload, DODownloadStatusCallback>> DeliveryOptimizationDict { get; } = [];

        public static event Action<Guid, string, string, string, double> DownloadCreated;

        public static event Action<Guid> DownloadContinued;

        public static event Action<Guid> DownloadPaused;

        public static event Action<Guid> DownloadDeleted;

        public static event Action<Guid, DO_DOWNLOAD_STATUS> DownloadProgressing;

        public static event Action<Guid, DO_DOWNLOAD_STATUS> DownloadCompleted;

        /// <summary>
        /// 获取下载任务的数量
        /// </summary>
        public static int GetDownloadCount()
        {
            int count = 0;
            deliveryOptimizationLock.Enter();

            try
            {
                count = DeliveryOptimizationDict.Count;
            }
            catch (Exception e)
            {
                ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
            }
            finally
            {
                deliveryOptimizationLock.Exit();
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
                if (GetDownloadCount() > 0)
                {
                    deliveryOptimizationLock.Enter();

                    try
                    {
                        foreach (KeyValuePair<Guid, Tuple<IDODownload, DODownloadStatusCallback>> deliveryOptimizationKeyValue in DeliveryOptimizationDict)
                        {
                            deliveryOptimizationKeyValue.Value.Item1.Abort();
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "Terminate all task failed", e);
                    }
                    finally
                    {
                        deliveryOptimizationLock.Exit();
                    }
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
                    IDOManager doManager = null;
                    IDODownload doDownload = null;

                    // 创建 IDoManager
                    int createResult = Ole32Library.CoCreateInstance(CLSID_DeliveryOptimization, IntPtr.Zero, CLSCTX.CLSCTX_LOCAL_SERVER, typeof(IDOManager).GUID, out IntPtr doManagerPointer);

                    if (createResult is 0)
                    {
                        doManager = ComInterfaceMarshaller<IDOManager>.ConvertToManaged((void*)doManagerPointer);
                        doManager.CreateDownload(out doDownload);
                        ComWrappers.TryGetComInstance(doDownload, out IntPtr doDownloadPointer);
                        _ = Ole32Library.CoSetProxyBlanket(doDownloadPointer, uint.MaxValue, uint.MaxValue, new IntPtr(-1), 0, 3, IntPtr.Zero, 32);
                        _ = Ole32Library.CoSetProxyBlanket(doDownloadPointer, uint.MaxValue, uint.MaxValue, new IntPtr(-1), 0, 3, IntPtr.Zero, 32);

                        // 添加下载信息
                        ComVariant displayNameVariant = ComVariant.Create(displayName);
                        doDownload.SetProperty(DODownloadProperty.DODownloadProperty_DisplayName, displayNameVariant);
                        ComVariant urlVariant = ComVariant.Create(url);
                        doDownload.SetProperty(DODownloadProperty.DODownloadProperty_Uri, urlVariant);
                        ComVariant filePathVariant = ComVariant.Create(saveFilePath);
                        doDownload.SetProperty(DODownloadProperty.DODownloadProperty_LocalPath, filePathVariant);

                        DODownloadStatusCallback doDownloadStatusCallback = new();
                        doDownloadStatusCallback.StatusChanged += OnStatusChanged;

                        ComVariant callbackInterfaceVariant = ComVariant.CreateRaw(VarEnum.VT_UNKNOWN, (IntPtr)ComInterfaceMarshaller<object>.ConvertToUnmanaged(new UnknownWrapper(doDownloadStatusCallback).WrappedObject));
                        doDownload.SetProperty(DODownloadProperty.DODownloadProperty_CallbackInterface, callbackInterfaceVariant);
                        ComVariant foregroundVariant = ComVariant.Create(true);
                        doDownload.SetProperty(DODownloadProperty.DODownloadProperty_ForegroundPriority, foregroundVariant);

                        ComVariant idVariant = ComVariant.Null;
                        doDownload.GetProperty(DODownloadProperty.DODownloadProperty_Id, out idVariant);
                        ComVariant totalSizeVariant = ComVariant.Null;
                        doDownload.GetProperty(DODownloadProperty.DODownloadProperty_TotalSizeBytes, out totalSizeVariant);
                        doDownloadStatusCallback.DownloadID = new(idVariant.As<string>());
                        double size = Convert.ToDouble(totalSizeVariant.As<ulong>());
                        DownloadCreated?.Invoke(doDownloadStatusCallback.DownloadID, Path.GetFileName(saveFilePath), saveFilePath, url, Convert.ToDouble(totalSizeVariant.As<ulong>()));

                        deliveryOptimizationLock.Enter();

                        try
                        {
                            DeliveryOptimizationDict.TryAdd(doDownloadStatusCallback.DownloadID, Tuple.Create(doDownload, doDownloadStatusCallback));
                        }
                        catch (Exception e)
                        {
                            ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                        }
                        finally
                        {
                            deliveryOptimizationLock.Exit();
                        }

                        doDownload.Start(IntPtr.Zero);
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "Create delivery optimization download failed", e);
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
                deliveryOptimizationLock.Enter();

                try
                {
                    if (DeliveryOptimizationDict.TryGetValue(downloadID, out Tuple<IDODownload, DODownloadStatusCallback> downloadValue))
                    {
                        int continueResult = downloadValue.Item1.Start(IntPtr.Zero);

                        if (continueResult is 0)
                        {
                            DownloadContinued?.Invoke(downloadID);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "Continue delivery optimization download failed", e);
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
        public static void PauseDownload(Guid downloadID)
        {
            Task.Factory.StartNew((param) =>
            {
                deliveryOptimizationLock.Enter();

                try
                {
                    if (DeliveryOptimizationDict.TryGetValue(downloadID, out Tuple<IDODownload, DODownloadStatusCallback> downloadValue))
                    {
                        int pauseResult = downloadValue.Item1.Pause();

                        if (pauseResult is 0)
                        {
                            DownloadPaused?.Invoke(downloadID);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "Pause delivery optimization download failed", e);
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
        public static void DeleteDownload(Guid downloadID)
        {
            Task.Factory.StartNew((param) =>
            {
                deliveryOptimizationLock.Enter();

                try
                {
                    if (DeliveryOptimizationDict.TryGetValue(downloadID, out Tuple<IDODownload, DODownloadStatusCallback> downloadValue))
                    {
                        int deleteResult = downloadValue.Item1.Abort();

                        if (deleteResult is 0)
                        {
                            downloadValue.Item2.StatusChanged -= OnStatusChanged;
                            DownloadDeleted?.Invoke(downloadID);
                            DeliveryOptimizationDict.Remove(downloadID);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "Delete delivery optimization download failed", e);
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
            if (status.State is DODownloadState.DODownloadState_Transferring)
            {
                DownloadProgressing?.Invoke(callback.DownloadID, status);
            }
            else if (status.State is DODownloadState.DODownloadState_Transferred)
            {
                DownloadCompleted?.Invoke(callback.DownloadID, status);
                try
                {
                    callback.StatusChanged -= OnStatusChanged;
                    doDownload.Finalize();

                    deliveryOptimizationLock.Enter();

                    try
                    {
                        if (DeliveryOptimizationDict.ContainsKey(callback.DownloadID))
                        {
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
                    LogService.WriteLog(LoggingLevel.Warning, "Finalize delivery optimization download task failed", e);
                }
            }
            else if (status.State is DODownloadState.DODownloadState_Paused)
            {
                DownloadPaused?.Invoke(callback.DownloadID);
            }
        }
    }
}
