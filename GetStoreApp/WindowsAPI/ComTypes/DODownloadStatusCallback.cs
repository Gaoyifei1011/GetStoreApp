using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreApp.WindowsAPI.ComTypes
{
    /// <summary>
    /// IDODownloadStatusCallback 接口的实现
    /// </summary>
    [GeneratedComClass]
    public partial class DODownloadStatusCallback : IDODownloadStatusCallback
    {
        public string DownloadID { get; set; } = string.Empty;

        /// <summary>
        /// 下载状态发生变化时触发的事件
        /// </summary>
        public Action<DODownloadStatusCallback, IDODownload, DO_DOWNLOAD_STATUS> StatusChanged;

        public void OnStatusChange([MarshalAs(UnmanagedType.Interface)] IDODownload download, ref DO_DOWNLOAD_STATUS status)
        {
            StatusChanged?.Invoke(this, download, status);
        }
    }
}
