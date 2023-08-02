using System;
using System.Runtime.InteropServices;
using Windows.ApplicationModel.DataTransfer;

namespace GetStoreApp.WindowsAPI.ComTypes
{
    /// <summary>
    /// 允许在管理多个窗口的 Windows 应用商店应用中访问 <see cref="DataTransferManager"> 方法。
    /// </summary>
    [ComImport, Guid("3A3DCD6C-3EAB-43DC-BCDE-45671CE800C8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDataTransferManagerInterop
    {
        /// <summary>
        /// 获取指定窗口的 <see cref="DataTransferManager"> 实例。
        /// </summary>
        /// <param name="appWindow">要检索其 <see cref="DataTransferManager"> 实例的窗口。</param>
        /// <param name="riid"><see cref="DataTransferManager"> 实例的请求接口 ID。</param>
        /// <returns>接收 <see cref="DataTransferManager"> 实例。</returns>
        IntPtr GetForWindow([In] IntPtr appWindow, [In] ref Guid riid);

        /// <summary>
        /// 显示用于共享指定窗口内容的 UI。
        /// </summary>
        /// <param name="appWindow">要为其显示共享 UI 的窗口。</param>
        void ShowShareUIForWindow(IntPtr appWindow);
    }
}
