using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.WindowsUI
{
    public static partial class WindowsUILibrary
    {
        private const string WindowsUI = "Windows.UI.dll";

        /// <summary>
        /// 在桌面应用中创建 CoreWindow
        /// </summary>
        /// <param name="WindowType">创建的 CoreWindow 窗口类型</param>
        /// <param name="pWindowTitle">CoreWindow 窗口标题</param>
        /// <param name="X">CoreWindow 窗口 X 轴坐标位置</param>
        /// <param name="Y">CoreWindow 窗口 Y 轴坐标位置</param>
        /// <param name="uWidth">CoreWindow 窗口宽度</param>
        /// <param name="uHeight">CoreWindow 窗口高度</param>
        /// <param name="dwAttributes">CoreWindow 窗口属性</param>
        /// <param name="hOwnerWindow">CoreWindow 窗口所有者的窗口句柄</param>
        /// <param name="riid">CoreWindow 类对应 COM 组件中的接口</param>
        /// <param name="ppv">创建 CoreWindow 返回的对象</param>
        /// <returns>创建 CoreWindow 的结果</returns>
        [LibraryImport(WindowsUI, EntryPoint = "#1500", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        public static partial int PrivateCreateCoreWindow(WINDOW_TYPE WindowType, string pWindowTitle, int X, int Y, int uWidth, int uHeight, int dwAttributes, IntPtr hOwnerWindow, Guid riid, out IntPtr ppv);
    }
}
