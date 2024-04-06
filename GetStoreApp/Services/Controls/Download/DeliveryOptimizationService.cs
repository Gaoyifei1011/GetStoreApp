using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.ComTypes;
using GetStoreApp.WindowsAPI.PInvoke.Ole32;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Windows.Foundation.Diagnostics;

namespace GetStoreApp.Services.Controls.Download
{
    /// <summary>
    /// 传递优化服务
    /// </summary>
    public static class DeliveryOptimizationService
    {
        private static string displayName = "GetStoreApp";
        private static Guid CLSID_DeliveryOptimization = new Guid("5b99fa76-721c-423c-adac-56d03c8a8007");
        private static Guid IID_DOManager = new Guid("400E2D4A-1431-4C1A-A748-39CA472CFDB1");

        private static Dictionary<string, IDODownload> DeliveryOptimizationDict { get; } = new Dictionary<string, IDODownload>();

        /// <summary>
        /// 使用下载链接创建下载
        /// </summary>
        public static unsafe string CreateDownload(string url, string saveFilePath)
        {
            string downloadID = string.Empty;
            try
            {
                IDOManager doManager = null;
                // 创建 IDoManager
                int createResult = Ole32Library.CoCreateInstance(ref CLSID_DeliveryOptimization, IntPtr.Zero, CLSCTX.CLSCTX_LOCAL_SERVER, ref IID_DOManager, out IntPtr doDownloadPointer);
                if (createResult is 0)
                {
                    StrategyBasedComWrappers strategyBasedComWrappers = new StrategyBasedComWrappers();
                    doManager = (IDOManager)strategyBasedComWrappers.GetOrCreateObjectForComInstance(doDownloadPointer, CreateObjectFlags.None);

                    // 创建下载
                    if (doManager is not null)
                    {
                        doManager.CreateDownload(out IDODownload doDownload);
                        int proxyResult = Ole32Library.CoSetProxyBlanket(doDownloadPointer, uint.MaxValue, uint.MaxValue, unchecked((IntPtr)ulong.MaxValue), 0, 3, IntPtr.Zero, 32);

                        // 添加下载信息
                        ComVariant displayNameVarient = ComVariant.Create(displayName);
                        doDownload.SetProperty(DODownloadProperty.DODownloadProperty_DisplayName, &displayNameVarient);
                        ComVariant urlVarient = ComVariant.Create(url);
                        doDownload.SetProperty(DODownloadProperty.DODownloadProperty_Uri, &urlVarient);
                        ComVariant filePathVarient = ComVariant.Create(saveFilePath);
                        doDownload.SetProperty(DODownloadProperty.DODownloadProperty_LocalPath, &filePathVarient);
                        ComVariant foregroundVarient = ComVariant.Create(true);
                        DODownloadStatusCallback doDownloadStatusCallback = new DODownloadStatusCallback();
                        doDownloadStatusCallback.StatusChanged += OnStatusChanged;

                        // 正在学习 COM 源生成......
                        ComVariant callbackInterfaceVarient = ComVariant.CreateRaw(VarEnum.VT_UNKNOWN, IntPtr.Zero);
                        doDownload.SetProperty(DODownloadProperty.DODownloadProperty_ForegroundPriority, &foregroundVarient);

                        int startResult = doDownload.Start(IntPtr.Zero);

                        // 下载启动成功，获取下载 Id，并添加到下载字典中
                        if (startResult is 0)
                        {
                            ComVariant idVarient = ComVariant.Null;
                            doDownload.GetProperty(DODownloadProperty.DODownloadProperty_Id, &idVarient);
                            downloadID = idVarient.As<string>();
                            doDownloadStatusCallback.DownloadID = downloadID;
                            DeliveryOptimizationDict.TryAdd(downloadID, doDownload);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Create delivery optimization download failed", e);
            }
            return downloadID;
        }

        /// <summary>
        /// 删除下载
        /// </summary>
        public static bool DeleteDownload(string downloadID)
        {
            try
            {
                if (DeliveryOptimizationDict.TryGetValue(downloadID, out IDODownload doDownload))
                {
                    int abortResult = doDownload.Abort();
                    return abortResult is 0;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Delete delivery optimization download failed", e);
                return false;
            }
        }

        /// <summary>
        /// 下载状态发生变化触发的事件
        /// </summary>
        private static void OnStatusChanged(DODownloadStatusCallback callback, IDODownload download, DO_DOWNLOAD_STATUS status)
        {
            switch (status.State)
            {
                case DODownloadState.DODownloadState_Created:
                    {
                        break;
                    }
                case DODownloadState.DODownloadState_Transferring:
                    {
                        break;
                    }
                case DODownloadState.DODownloadState_Transferred:
                    {
                        break;
                    }
                case DODownloadState.DODownloadState_Aborted:
                    {
                        break;
                    }
                case DODownloadState.DODownloadState_Finalized:
                    {
                        try
                        {
                            callback.StatusChanged -= OnStatusChanged;
                        }
                        catch (Exception)
                        {
                        }
                        break;
                    }
            }
        }
    }
}
