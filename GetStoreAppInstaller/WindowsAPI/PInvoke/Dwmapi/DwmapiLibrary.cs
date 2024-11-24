using System;
using System.Runtime.InteropServices;

namespace GetStoreAppInstaller.WindowsAPI.PInvoke.Dwmapi
{
    /// <summary>
    /// Dwmapi.dll 函数库
    /// </summary>
    public static partial class DwmapiLibrary
    {
        private const string Dwmapi = "dwmapi.dll";

        /// <summary>
        /// 获取一个值，该值指示是否启用桌面窗口管理器 (DWM) 组合。 运行 Windows 7 或更早版本的计算机上的应用程序可以通过处理 WM_DWMCOMPOSITIONCHANGED 通知来侦听组合状态更改。
        /// </summary>
        /// <returns>指向一个值的指针，当此函数成功返回时，如果启用 DWM 组合，则接收 TRUE ;否则为 FALSE。</returns>
        [LibraryImport(Dwmapi, EntryPoint = "DwmIsCompositionEnabled", SetLastError = false), PreserveSig]
        public static partial int DwmIsCompositionEnabled([MarshalAs(UnmanagedType.Bool)] out bool enabled);

        /// <summary>
        /// 设置窗口管理器（DWM）非客户端呈现属性的值。 有关编程指南和代码示例，请参阅 控制非客户端区域呈现。
        /// </summary>
        /// <param name="hwnd">要为其设置属性值的窗口的句柄。</param>
        /// <param name="attr">描述要设置的值的标志，指定为 DWMWINDOWATTRIBUTE 枚举的值。 此参数指定要设置的属性，pvAttribute 参数指向包含属性值的对象。</param>
        /// <param name="attrValue">指向包含要设置的属性值的对象的指针。 值集的类型取决于 dwAttribute 参数的值。 DWMWINDOWATTRIBUTE 枚举主题指示，在每个标志的行中，应向 pvAttribute 参数传递指向的值类型。</param>
        /// <param name="attrSize">通过 pvAttribute 参数设置的属性值的大小（以字节为单位）。 值集的类型及其大小（以字节为单位）取决于 dwAttribute 参数的值。</param>
        /// <returns>如果函数成功，则返回 S_OK。 否则，它将返回 HRESULT错误代码。如果桌面合成已被禁用（Windows 7 及更早版本），则此函数将返回 DWM_E_COMPOSITIONDISABLED。</returns>
        [LibraryImport(Dwmapi, EntryPoint = "DwmSetWindowAttribute", SetLastError = false), PreserveSig]
        public static partial int DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attr, ref int attrValue, int attrSize);

        /// <summary>
        /// 将窗口框架扩展到工作区。
        /// </summary>
        /// <param name="hwnd">框架将扩展到工作区的窗口的句柄。</param>
        /// <param name="pMarInset">指向 MARGINS 结构的指针，该结构描述在将帧扩展到工作区时要使用的边距。</param>
        /// <returns>如果此函数成功，则返回 S_OK。 否则，将返回 HRESULT 错误代码。</returns>
        [LibraryImport(Dwmapi, EntryPoint = "DwmExtendFrameIntoClientArea", SetLastError = false), PreserveSig]
        public static partial int DwmExtendFrameIntoClientArea(nint hwnd, ref MARGINS pMarInset);
    }
}
