using System;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreApp.WindowsAPI.ComTypes
{
    /// <summary>
    /// IDODownloadStatusCallback 接口的实现
    /// </summary>
    [GeneratedComClass]
    public partial class DODownloadStatusCallback : IDODownloadStatusCallback
    {
        public Guid DownloadID { get; set; } = Guid.Empty;

        /// <summary>
        /// 下载状态发生变化时触发的事件
        /// </summary>
        public event Action<DODownloadStatusCallback, IDODownload, DO_DOWNLOAD_STATUS> StatusChanged;

        public int OnStatusChange(IDODownload download, DO_DOWNLOAD_STATUS status)
        {
            StatusChanged?.Invoke(this, download, status);
            return 0;
        }
    }
}
