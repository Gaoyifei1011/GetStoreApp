using System.Runtime.InteropServices;

// 抑制 CA1401 警告
#pragma warning disable CA1401

namespace GetStoreAppWebView.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// User32.dll 函数库
    /// </summary>
    internal static partial class User32Library
    {
        private const string User32 = "user32.dll";

        /// <summary>
        /// EnumDisplayMonitors 函数枚举显示监视器 (包括与镜像驱动程序关联的不可见伪监视器) ，这些监视器与指定剪裁矩形的交集与设备上下文的可见区域相交而形成的区域相交。 EnumDisplayMonitors 为枚举的每个监视器调用一次应用程序定义的 MonitorEnumProc 回调函数。 请注意， GetSystemMetrics (SM_CMONITORS) 仅对显示监视器进行计数。
        /// </summary>
        /// <param name="hdc">
        /// 显示设备上下文的句柄，用于定义感兴趣的可见区域。
        /// 如果此参数为 NULL，则传递给回调函数的 hdcMonitor 参数将为 NULL，并且关注的可见区域是包含桌面上所有显示器的虚拟屏幕。
        /// </param>
        /// <param name="lprcClip">
        /// 指向指定剪裁矩形的 RECT 结构的指针。 感兴趣区域是剪裁矩形与 hdc 指定的可见区域的交集。
        /// 如果 hdc 为非 NULL，则剪裁矩形的坐标相对于 hdc 的原点。 如果 hdc 为 NULL，则坐标为虚拟屏幕坐标。
        /// 如果不想剪辑 hdc 指定的区域，此参数可以为 NULL。</param>
        /// <param name="lpfnEnum">指向 MonitorEnumProc 应用程序定义的回调函数的指针。</param>
        /// <param name="dwData">EnumDisplayMonitors 直接传递给 MonitorEnumProc 函数的应用程序定义数据。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [LibraryImport(User32, EntryPoint = "EnumDisplayMonitors", SetLastError = false), PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool EnumDisplayMonitors(nint hdc, nint lprcClip, MONITORENUMPROC lpfnEnum, nint dwData);

        /// <summary>
        /// 返回指定窗口的每英寸点数 (dpi) 值。
        /// </summary>
        /// <param name="hwnd">要获取其相关信息的窗口。</param>
        /// <returns>窗口的 DPI，取决于窗口 DPI_AWARENESS 。 有关详细信息，请参阅备注部分。 无效 的 hwnd 值将导致返回值 0。</returns>
        [LibraryImport(User32, EntryPoint = "GetDpiForWindow", SetLastError = false), PreserveSig]
        internal static partial uint GetDpiForWindow(nint hwnd);

        /// <summary>
        /// 检索指定窗口的显示状态以及还原、最小化和最大化的位置。
        /// </summary>
        /// <param name="hWnd">窗口的句柄。</param>
        /// <param name="lpwndpl">指向接收显示状态和位置信息的 WINDOWPLACEMENT 结构的指针。 在调用 GetWindowPlacement 之前，请将 length 成员设置为 sizeof(WINDOWPLACEMENT)。 如果未正确设置 lpwndpl-length，GetWindowPlacement > 将失败。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [LibraryImport(User32, EntryPoint = "GetWindowPlacement", SetLastError = false), PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool GetWindowPlacement(nint hWnd, ref WINDOWPLACEMENT lpwndpl);

        /// <summary>
        /// 创建从指定文件中提取的图标的句柄数组。
        /// </summary>
        /// <param name="lpszFile">要从中提取图标的文件的路径和名称。</param>
        /// <param name="nIconIndex">要提取的第一个图标的从零开始的索引。 例如，如果此值为零，函数将提取指定文件中的第一个图标。</param>
        /// <param name="cxIcon">所需的水平图标大小。 </param>
        /// <param name="cyIcon">所需的垂直图标大小。</param>
        /// <param name="phicon">指向返回的图标句柄数组的指针。</param>
        /// <param name="piconid">
        /// 指向最适合当前显示设备的图标返回的资源标识符的指针。 如果标识符不可用于此格式，则返回的标识符0xFFFFFFFF。 如果无法获取标识符，则返回的标识符为 0。
        /// </param>
        /// <param name="nIcons">要从文件中提取的图标数。 此参数仅在从 .exe 和 .dll 文件时有效。</param>
        /// <param name="flags">指定控制此函数的标志。 这些标志是 LoadImage 函数使用的 LR_* 标志。</param>
        /// <returns>
        /// 如果 <param name="phicon"> 参数为 NULL 且此函数成功，则返回值为文件中的图标数。 如果函数失败，则返回值为 0。 如果 <param name="phicon"> 参数不是 NULL 且函数成功，则返回值是提取的图标数。 否则，如果未找到文件，则返回值0xFFFFFFFF。
        /// </returns>
        [LibraryImport(User32, EntryPoint = "PrivateExtractIconsW", SetLastError = false, StringMarshalling = StringMarshalling.Utf16), PreserveSig]
        internal static partial int PrivateExtractIcons([MarshalAs(UnmanagedType.LPWStr)] string lpszFile, int nIconIndex, int cxIcon, int cyIcon, [Out, MarshalAs(UnmanagedType.LPArray)] nint[] phicon, [Out, MarshalAs(UnmanagedType.LPArray)] int[] piconid, int nIcons, int flags);

        /// <summary>
        /// 将指定的消息发送到窗口或窗口。 SendMessage 函数调用指定窗口的窗口过程，在窗口过程处理消息之前不会返回。
        /// </summary>
        /// <param name="hWnd">
        /// 窗口过程的句柄将接收消息。 如果此参数 HWND_BROADCAST ( (HWND) 0xffff) ，则会将消息发送到系统中的所有顶级窗口，
        /// 包括已禁用或不可见的未所有者窗口、重叠窗口和弹出窗口;但消息不会发送到子窗口。消息发送受 UIPI 的约束。
        /// 进程的线程只能将消息发送到较低或等于完整性级别的线程的消息队列。
        /// </param>
        /// <param name="wMsg">要发送的消息。</param>
        /// <param name="wParam">其他的消息特定信息。</param>
        /// <param name="lParam">其他的消息特定信息。</param>
        /// <returns>返回值指定消息处理的结果;这取决于发送的消息。</returns>
        [LibraryImport(User32, EntryPoint = "SendMessageW", SetLastError = false), PreserveSig]
        internal static partial nint SendMessage(nint hWnd, WindowMessage wMsg, nuint wParam, nint lParam);

        /// <summary>
        /// 当仅针对当前正在运行的进程创建没有父级或所有者的窗口时，更改默认布局。
        /// </summary>
        /// <param name="dwDefaultLayout">默认进程布局。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [LibraryImport(User32, EntryPoint = "SetProcessDefaultLayout", SetLastError = false), PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool SetProcessDefaultLayout(uint dwDefaultLayout);

        /// <summary>
        /// 设置指定窗口的显示状态和还原、最小化和最大化的位置。
        /// </summary>
        /// <param name="hWnd">窗口的句柄。</param>
        /// <param name="lpwndpl">指向 WINDOWPLACEMENT 结构的指针，该结构指定新的显示状态和窗口位置。在调用 SetWindowPlacement 之前，请将 WINDOWPLACEMENT 结构的长度成员设置为 sizeof (WINDOWPLACEMENT) 。 如果未正确设置长度成员，SetWindowPlacement 将失败。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [LibraryImport(User32, EntryPoint = "SetWindowPlacement", SetLastError = false), PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static partial bool SetWindowPlacement(nint hWnd, WINDOWPLACEMENT lpwndpl);
    }
}
