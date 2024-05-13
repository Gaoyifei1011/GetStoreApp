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
    /// 传递优化服务（仅支持 Winodws 11 22621 及更高版本）
    /// </summary>
    public static class DeliveryOptimizationService
    {
        private static string displayName = "GetStoreApp";
        private static object deliveryOptimizationLock = new object();
        private static Guid CLSID_DeliveryOptimization = new Guid("5B99FA76-721C-423C-ADAC-56D03C8A8007");
        private static Guid IID_DOManager = new Guid("400E2D4A-1431-4C1A-A748-39CA472CFDB1");
        private static BackgroundWorker backgroundWorker = new BackgroundWorker();
        private static StrategyBasedComWrappers strategyBasedComWrappers = new StrategyBasedComWrappers();

        private static Dictionary<Guid, Tuple<IDODownload, DODownloadStatusCallback>> DeliveryOptimizationDict { get; } = new Dictionary<Guid, Tuple<IDODownload, DODownloadStatusCallback>>();

        public static event Action<Guid, string, string, string, double> DownloadCreated;

        public static event Action<Guid> DownloadContinued;

        public static event Action<Guid> DownloadPaused;

        public static event Action<Guid> DownloadDeleted;

        public static event Action<Guid, DO_DOWNLOAD_STATUS> DownloadProgressing;

        public static event Action<Guid, DO_DOWNLOAD_STATUS> DownloadCompleted;

        /// <summary>
        /// 初始化传递优化服务
        /// </summary>
        public static void InItialize()
        {
            if (backgroundWorker is not null)
            {
                backgroundWorker.DoWork += OnDoWork;
            }

            try
            {
                while (backgroundWorker.IsBusy) continue;
                backgroundWorker.RunWorkerAsync(new object[] { "Initialize" });
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Initialize delivery optimization failed", e);
            }
        }

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
        /// 在单独的线程中执行传递优化服务
        /// </summary>
        private static unsafe void OnDoWork(object sender, DoWorkEventArgs args)
        {
            object[] parameter = args.Argument as object[];

            if (parameter.Length > 0)
            {
                if (parameter[0] is "Initialize")
                {
                    Ole32Library.CoInitializeSecurity(0, -1, 0, 0, 0, 3, 0, 0x20, 0);
                }
                else if (parameter[0] is "CreateDownload")
                {
                    string url = Convert.ToString(parameter[1]);
                    string saveFilePath = Convert.ToString(parameter[2]);

                    try
                    {
                        IDOManager doManager = null;
                        IDODownload doDownload = null;

                        // 创建 IDoManager
                        int createResult = Ole32Library.CoCreateInstance(ref CLSID_DeliveryOptimization, IntPtr.Zero, CLSCTX.CLSCTX_LOCAL_SERVER, ref IID_DOManager, out IntPtr doManagerPointer);

                        if (createResult is 0)
                        {
                            doManager = (IDOManager)strategyBasedComWrappers.GetOrCreateObjectForComInstance(doManagerPointer, CreateObjectFlags.None);
                            doManager.CreateDownload(out doDownload);
                            ComWrappers.TryGetComInstance(doDownload, out IntPtr doDownloadPointer);
                            int proxyResult = Ole32Library.CoSetProxyBlanket(doDownloadPointer, uint.MaxValue, uint.MaxValue, new IntPtr(-1), 0, 3, IntPtr.Zero, 32);

                            // 添加下载信息
                            ComVariant displayNameVarient = ComVariant.Create(displayName);
                            doDownload.SetProperty(DODownloadProperty.DODownloadProperty_DisplayName, &displayNameVarient);
                            ComVariant urlVarient = ComVariant.Create(url);
                            doDownload.SetProperty(DODownloadProperty.DODownloadProperty_Uri, &urlVarient);
                            ComVariant filePathVarient = ComVariant.Create(saveFilePath);
                            int result2 = doDownload.SetProperty(DODownloadProperty.DODownloadProperty_LocalPath, &filePathVarient);

                            DODownloadStatusCallback doDownloadStatusCallback = new DODownloadStatusCallback();
                            doDownloadStatusCallback.StatusChanged += OnStatusChanged;

                            ComVariant callbackInterfaceVarient = ComVariant.CreateRaw(VarEnum.VT_UNKNOWN, strategyBasedComWrappers.GetOrCreateComInterfaceForObject(new UnknownWrapper(doDownloadStatusCallback).WrappedObject, CreateComInterfaceFlags.None));
                            ComVariant foregroundVarient = ComVariant.Create(true);
                            doDownload.SetProperty(DODownloadProperty.DODownloadProperty_ForegroundPriority, &foregroundVarient);

                            ComVariant idVarient = ComVariant.Null;
                            doDownload.GetProperty(DODownloadProperty.DODownloadProperty_Id, &idVarient);
                            ComVariant totalSizeVarient = ComVariant.Null;
                            doDownload.GetProperty(DODownloadProperty.DODownloadProperty_TotalSizeBytes, &totalSizeVarient);
                            doDownloadStatusCallback.DownloadID = new Guid(idVarient.As<string>());
                            double size = Convert.ToDouble(totalSizeVarient.As<ulong>());
                            DownloadCreated?.Invoke(doDownloadStatusCallback.DownloadID, Path.GetFileName(saveFilePath), saveFilePath, url, Convert.ToDouble(totalSizeVarient.As<ulong>()));

                            lock (deliveryOptimizationLock)
                            {
                                DeliveryOptimizationDict.TryAdd(doDownloadStatusCallback.DownloadID, Tuple.Create(doDownload, doDownloadStatusCallback));
                            }

                            int result = doDownload.Start(IntPtr.Zero);
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "Create delivery optimization download failed", e);
                    }
                }
                else if (parameter[0] is "ContinueDownload")
                {
                    Guid downloadID = (Guid)parameter[1];

                    try
                    {
                        lock (deliveryOptimizationLock)
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
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "Continue delivery optimization download failed", e);
                    }
                }
                else if (parameter[0] is "PauseDownload")
                {
                    Guid downloadID = (Guid)parameter[1];

                    try
                    {
                        lock (deliveryOptimizationLock)
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
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "Pause delivery optimization download failed", e);
                    }
                }
                else if (parameter[0] is "DeleteDownload")
                {
                    Guid downloadID = (Guid)parameter[1];

                    try
                    {
                        lock (deliveryOptimizationLock)
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
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LoggingLevel.Error, "Delete delivery optimization download failed", e);
                    }
                }
                else if (parameter[0] is "TerminateDownload")
                {
                    if (GetDownloadCount() > 0)
                    {
                        lock (deliveryOptimizationLock)
                        {
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
            lock (deliveryOptimizationLock)
            {
                count = DeliveryOptimizationDict.Count;
            }
            return count;
        }

        /// <summary>
        /// 使用下载链接创建下载
        /// </summary>
        public static void CreateDownload(string url, string saveFilePath)
        {
            try
            {
                while (backgroundWorker.IsBusy) continue;
                backgroundWorker.RunWorkerAsync(new object[3] { "CreateDownload", url, saveFilePath });
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Create delivery optimization download failed", e);
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
                LogService.WriteLog(LoggingLevel.Error, "Continue delivery optimization download task failed", e);
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
                LogService.WriteLog(LoggingLevel.Error, "Pause delivery optimization download task failed", e);
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
                LogService.WriteLog(LoggingLevel.Error, "Delete delivery optimization download task failed", e);
            }
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
                LogService.WriteLog(LoggingLevel.Error, "Terminate delivery optimization download all task failed", e);
            }
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

                    lock (deliveryOptimizationLock)
                    {
                        if (DeliveryOptimizationDict.ContainsKey(callback.DownloadID))
                        {
                            DeliveryOptimizationDict.Remove(callback.DownloadID);
                        }
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
