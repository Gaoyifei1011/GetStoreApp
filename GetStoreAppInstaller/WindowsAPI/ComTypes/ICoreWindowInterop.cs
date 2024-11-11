using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace GetStoreAppInstaller.WindowsAPI.ComTypes
{
    /// <summary>
    /// 使应用能够 (与此接口关联的 CoreWindow) 获取窗口的窗口句柄。
    /// </summary>
    [GeneratedComInterface, Guid("45D64A29-A63E-4CB6-B498-5781D298CB4F")]
    public partial interface ICoreWindowInterop
    {
        /// <summary>
        /// 获取应用的 CoreWindow (HWND) 句柄。
        /// </summary>
        /// <param name="hwnd">CoreWindow 的窗口句柄</param>
        /// <returns>此方法返回 HRESULT 成功或错误代码。</returns>
        [PreserveSig]
        int GetWindowHandle(out IntPtr hwnd);

        /// <summary>
        /// 设置是否已处理到 CoreWindow 的消息。此属性是只写的。
        /// </summary>
        /// <param name="value">标志该消息是否已被处理</param>
        /// <returns>此方法返回 HRESULT 成功或错误代码。</returns>
        [PreserveSig]
        int SetMessageHandled([MarshalAs(UnmanagedType.Bool)] bool value);
    }
}
