using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.ComTypes;
using GetStoreApp.WindowsAPI.PInvoke.Ole32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;

namespace GetStoreApp.Services.Controls.Download
{
    /// <summary>
    /// 传递优化服务
    /// </summary>
    public static class DeliveryOptimizationService
    {
        private static string displayName = "GetStoreApp";
        private static object deliveryOptimizationLock = new object();
        private static Guid CLSID_DeliveryOptimization = new Guid("5b99fa76-721c-423c-adac-56d03c8a8007");
        private static Guid IID_DOManager = new Guid("400E2D4A-1431-4C1A-A748-39CA472CFDB1");
        private static StrategyBasedComWrappers comWrappers = new StrategyBasedComWrappers();

        private static Dictionary<string, IDODownload> DeliveryOptimizationDict { get; } = new Dictionary<string, IDODownload>();

        public static event Action<string, string, string, string, double> DownloadCreated;

        public static event Action<string> DownloadContinued;

        public static event Action<string> DownloadPaused;

        public static event Action<string> DownloadAborted;

        public static event Action<string, DO_DOWNLOAD_STATUS> DownloadProgressing;

        public static event Action<string, DO_DOWNLOAD_STATUS> DownloadCompleted;

        /// <summary>
        /// 获取下载任务的数量
        /// </summary>
        public static int GetDownloadCount()
        {
            lock (deliveryOptimizationLock)
            {
                return DeliveryOptimizationDict.Count;
            }
        }

        /// <summary>
        /// 终止所有下载任务，仅用于应用关闭时
        /// </summary>
        public static void TerminateDownload()
        {
            if (GetDownloadCount() > 0)
            {
                lock (deliveryOptimizationLock)
                {
                    foreach (KeyValuePair<string, IDODownload> deliveryOptimizationKeyValue in DeliveryOptimizationDict)
                    {
                        deliveryOptimizationKeyValue.Value.Abort();
                    }
                }
            }
        }

        /// <summary>
        /// 使用下载链接创建下载
        /// </summary>
        public static unsafe void CreateDownload(string url, string saveFilePath)
        {
            string downloadID = string.Empty;

            try
            {
                IDOManager doManager = null;
                IDODownload doDownload = null;

                // 创建 IDoManager
                int createResult = Ole32Library.CoCreateInstance(ref CLSID_DeliveryOptimization, IntPtr.Zero, CLSCTX.CLSCTX_LOCAL_SERVER, ref IID_DOManager, out IntPtr doManagerPointer);

                if (createResult is 0)
                {
                    doManager = (IDOManager)comWrappers.GetOrCreateObjectForComInstance(doManagerPointer, CreateObjectFlags.None);

                    // 创建下载
                    if (doManager is not null)
                    {
                        doManager.CreateDownload(out doDownload);
                        ComWrappers.TryGetComInstance(doDownload, out IntPtr doDownloadPointer);
                        int proxyResult = Ole32Library.CoSetProxyBlanket(doDownloadPointer, uint.MaxValue, uint.MaxValue, unchecked((IntPtr)ulong.MaxValue), 0, 3, IntPtr.Zero, 32);

                        // 添加下载信息
                        ComVariant displayNameVarient = ComVariant.Create(displayName);
                        doDownload.SetProperty(DODownloadProperty.DODownloadProperty_DisplayName, &displayNameVarient);
                        ComVariant urlVarient = ComVariant.Create(url);
                        doDownload.SetProperty(DODownloadProperty.DODownloadProperty_Uri, &urlVarient);
                        ComVariant filePathVarient = ComVariant.Create(saveFilePath);
                        doDownload.SetProperty(DODownloadProperty.DODownloadProperty_LocalPath, &filePathVarient);

                        DODownloadStatusCallback doDownloadStatusCallback = new DODownloadStatusCallback();
                        doDownloadStatusCallback.StatusChanged += OnStatusChanged;
                        IntPtr callbackPointer = comWrappers.GetOrCreateComInterfaceForObject(new UnknownWrapper(doDownloadStatusCallback).WrappedObject, CreateComInterfaceFlags.None);
                        ComVariant callbackInterfaceVarient = ComVariant.CreateRaw(VarEnum.VT_UNKNOWN, callbackPointer);
                        ComVariant foregroundVarient = ComVariant.Create(true);
                        doDownload.SetProperty(DODownloadProperty.DODownloadProperty_ForegroundPriority, &foregroundVarient);

                        ComVariant idVarient = ComVariant.Null;
                        doDownload.GetProperty(DODownloadProperty.DODownloadProperty_Id, &idVarient);
                        ComVariant totalSizeVarient = ComVariant.Null;
                        doDownload.GetProperty(DODownloadProperty.DODownloadProperty_TotalSizeBytes, &totalSizeVarient);
                        downloadID = idVarient.As<string>();
                        doDownloadStatusCallback.DownloadID = downloadID;
                        double size = Convert.ToDouble(totalSizeVarient.As<ulong>());
                        DownloadCreated?.Invoke(downloadID, Path.GetFileName(saveFilePath), saveFilePath, url, Convert.ToDouble(totalSizeVarient.As<ulong>()));

                        lock (deliveryOptimizationLock)
                        {
                            DeliveryOptimizationDict.TryAdd(downloadID, doDownload);
                        }

                        int result = doDownload.Start(IntPtr.Zero);
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Create delivery optimization download failed", e);
            }
        }

        /// <summary>
        /// 删除下载
        /// </summary>
        public static void DeleteDownload(string downloadID)
        {
            Task.Run(() =>
            {
                try
                {
                    if (DeliveryOptimizationDict.TryGetValue(downloadID, out IDODownload doDownload))
                    {
                        int abortResult = doDownload.Abort();
                        if (abortResult is 0)
                        {
                            DownloadAborted?.Invoke(downloadID);

                            DeliveryOptimizationDict.Remove(downloadID);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, "Delete delivery optimization download failed", e);
                }
            });
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
                    LogService.WriteLog(LoggingLevel.Warning, "Finalize download task failed", e);
                }
            }
        }
    }
}
