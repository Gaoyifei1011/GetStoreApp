using System;
using System.Runtime.InteropServices;

namespace GetStoreAppWebView.WindowsAPI.ComTypes
{
    /// <summary>
    /// 使应用能够 (与此接口关联的 CoreWindow) 获取窗口的窗口句柄。
    /// </summary>
    [ComImport, Guid("45D64A29-A63E-4CB6-B498-5781D298CB4F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ICoreWindowInterop
    {
        /// <summary>
        /// 获取应用的 CoreWindow (HWND) 句柄。
        /// </summary>
        IntPtr WindowHandle { get; }

        /// <summary>
        /// 设置是否已处理到 CoreWindow 的消息。此属性是只写的。
        /// </summary>
        bool MessageHandled { set; }
    }
}
