using System;
using System.Runtime.InteropServices;

// 抑制 CA1401 警告
#pragma warning disable CA1401

namespace GetStoreAppInstaller.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// User32.dll 函数库
    /// </summary>
    public static partial class User32Library
    {
        private const string User32 = "user32.dll";

        /// <summary>
        /// 检索其类名称和窗口名称与指定字符串匹配的窗口的句柄。 该函数搜索子窗口，从指定子窗口后面的子窗口开始。 此函数不执行区分大小写的搜索。
        /// </summary>
        /// <param name="hWndParent">
        /// 要搜索其子窗口的父窗口的句柄。
        /// 如果 hwndParent为 NULL，则该函数使用桌面窗口作为父窗口。 该函数在桌面的子窗口之间搜索。
        /// 如果 hwndParentHWND_MESSAGE，则函数将搜索所有 仅消息窗口。
        /// </param>
        /// <param name="hWndChildAfter">
        /// 子窗口的句柄。 搜索从 Z 顺序中的下一个子窗口开始。 子窗口必须是 hwndParent的直接子窗口，而不仅仅是子窗口。
        /// 如果 hwndChildAfterNULL，则搜索从 hwndParent的第一个子窗口开始。
        /// 请注意，如果 hwndParent 和 hwndChildAfter 都 NULL，则该函数将搜索所有顶级窗口和仅消息窗口。
        /// </param>
        /// <param name="lpszClass">
        /// 上一次调用 RegisterClass 或 RegisterClassEx 函数创建的类名或类原子。 原子必须置于 lpszClass的低序单词中;高序单词必须为零。
        /// 如果 lpszClass 是字符串，则指定窗口类名。 类名称可以是注册到 RegisterClass 或 RegisterClassEx的任何名称，也可以是预定义的控件类名称，也可以 MAKEINTATOM(0x8000)。 在此后一种情况下，0x8000是菜单类的原子。
        /// </param>
        /// <param name="lpszWindow">窗口名称（窗口的标题）。 如果此参数 NULL，则所有窗口名称都匹配。</param>
        /// <returns>如果函数成功，则返回值是具有指定类和窗口名称的窗口的句柄。如果函数失败，则返回值 NULL。</returns>
        [LibraryImport(User32, EntryPoint = "FindWindowExW", SetLastError = false, StringMarshalling = StringMarshalling.Utf16)]
        public static partial IntPtr FindWindowEx(IntPtr hWndParent, IntPtr hWndChildAfter, [MarshalAs(UnmanagedType.LPWStr)] string lpszClass, [MarshalAs(UnmanagedType.LPWStr)] string lpszWindow);

        /// <summary>
        /// 检索有关指定窗口的信息。 该函数还会检索 32 位 (DWORD) 值，该值位于指定偏移量处，并进入额外的窗口内存。
        /// </summary>
        /// <param name="hWnd">窗口的句柄，间接地是窗口所属的类。</param>
        /// <param name="nIndex">要检索的值的从零开始的偏移量。 有效值在 0 到额外窗口内存的字节数中，减去 4 个;例如，如果指定了 12 个或更多字节的额外内存，则值 8 将是第三个 32 位整数的索引。
        /// </param>
        /// <returns>如果函数成功，则返回值是请求的值。如果函数失败，则返回值为零。</returns>
        [LibraryImport(User32, EntryPoint = "GetWindowLongW", SetLastError = false), PreserveSig]
        public static partial int GetWindowLong(IntPtr hWnd, WindowLongIndexFlags nIndex);

        /// <summary>
        /// 检索有关指定窗口的信息。 该函数还会检索 64 位 (DWORD) 值，该值位于指定偏移量处，并进入额外的窗口内存。
        /// </summary>
        /// <param name="hWnd">窗口的句柄，间接地是窗口所属的类。</param>
        /// <param name="nIndex">要检索的值的从零开始的偏移量。 有效值的范围为零到额外窗口内存的字节数，减去 LONG_PTR的大小。
        /// </param>
        /// <returns>如果函数成功，则返回值是请求的值。如果函数失败，则返回值为零。</returns>
        [LibraryImport(User32, EntryPoint = "GetWindowLongPtrW", SetLastError = false), PreserveSig]
        public static partial int GetWindowLongPtr(IntPtr hWnd, WindowLongIndexFlags nIndex);

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
        public static partial int PrivateExtractIcons([MarshalAs(UnmanagedType.LPWStr)] string lpszFile, int nIconIndex, int cxIcon, int cyIcon, [Out, MarshalAs(UnmanagedType.LPArray)] IntPtr[] phicon, [Out, MarshalAs(UnmanagedType.LPArray)] int[] piconid, int nIcons, int flags);

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
        public static partial IntPtr SendMessage(IntPtr hWnd, WindowMessage wMsg, UIntPtr wParam, IntPtr lParam);

        /// <summary>
        /// 更改指定窗口的属性。 该函数还将指定偏移量处的32位（long类型）值设置到额外的窗口内存中。
        /// </summary>
        /// <param name="hWnd">窗口的句柄，间接地是窗口所属的类</param>
        /// <param name="nIndex">要设置的值的从零开始的偏移量。 有效值的范围为零到额外窗口内存的字节数，减去整数的大小。</param>
        /// <param name="newProc">新事件处理函数（回调函数）</param>
        /// <returns>如果函数成功，则返回值是指定 32 位整数的上一个值。如果函数失败，则返回值为零。 </returns>
        [LibraryImport(User32, EntryPoint = "SetWindowLongW", SetLastError = false), PreserveSig]
        public static partial IntPtr SetWindowLong(IntPtr hWnd, WindowLongIndexFlags nIndex, IntPtr dwNewLong);

        /// <summary>
        /// 更改指定窗口的属性。 该函数还将指定偏移量处的64位（long类型）值设置到额外的窗口内存中。
        /// </summary>
        /// <param name="hWnd">窗口的句柄，间接地是窗口所属的类</param>
        /// <param name="nIndex">要设置的值的从零开始的偏移量。 有效值的范围为零到额外窗口内存的字节数，减去整数的大小。</param>
        /// <param name="newProc">新事件处理函数（回调函数）</param>
        /// <returns>如果函数成功，则返回值是指定偏移量的上一个值。如果函数失败，则返回值为零。 </returns>
        [LibraryImport(User32, EntryPoint = "SetWindowLongPtrW", SetLastError = false), PreserveSig]
        public static partial IntPtr SetWindowLongPtr(IntPtr hWnd, WindowLongIndexFlags nIndex, IntPtr dwNewLong);
    }
}
