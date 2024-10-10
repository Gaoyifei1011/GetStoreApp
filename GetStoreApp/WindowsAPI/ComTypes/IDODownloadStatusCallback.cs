using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreApp.WindowsAPI.ComTypes
{
    /// <summary>
    /// IDODownloadStatusCallback 接口用于接收有关下载的通知。
    /// </summary>
    [GeneratedComInterface, Guid("D166E8E3-A90E-4392-8E87-05E996D3747D")]
    public partial interface IDODownloadStatusCallback
    {
        /// <summary>
        /// 每当下载状态发生更改时，传递优化将调用此方法的实现。
        /// </summary>
        /// <param name="download">指向其状态更改的 IDODownload 接口的指针。</param>
        /// <param name="status">指向包含下载状态 的DO_DOWNLOAD_STATUS 结构的指针。</param>
        /// <returns>如果函数成功，则返回 S_OK。 否则，它将返回 HRESULT错误代码。</returns>
        [PreserveSig]
        int OnStatusChange([MarshalAs(UnmanagedType.Interface)] IDODownload download, DO_DOWNLOAD_STATUS status);
    }
}
