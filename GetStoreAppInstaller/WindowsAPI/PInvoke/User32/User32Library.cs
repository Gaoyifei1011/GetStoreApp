using System;
using System.Runtime.InteropServices;
using Windows.Graphics;

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
        /// 检索鼠标光标的位置（以屏幕坐标为单位）。
        /// </summary>
        /// <param name="lpPoint">指向接收光标屏幕坐标的 POINT 结构的指针。</param>
        /// <returns>如果成功，则返回非零值，否则返回零。 要获得更多的错误信息，请调用 GetLastError。</returns>
        [LibraryImport(User32, EntryPoint = "GetCursorPos", SetLastError = false), PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool GetCursorPos(out PointInt32 lpPoint);

        /// <summary>
        /// 检索指定的系统指标或系统配置设置。
        /// 请注意， GetSystemMetrics 检索的所有维度都以像素为单位。
        /// </summary>
        /// <param name="nIndex">要检索的系统指标或配置设置。 此参数的取值可为下列值之一： 请注意，所有SM_CX* 值为宽度，所有SM_CY* 值为高度。 另请注意，设计为返回布尔数据的所有设置将 TRUE 表示为任何非零值， FALSE 表示零值。</param>
        /// <returns>如果函数成功，则返回值为请求的系统指标或配置设置。如果函数失败，返回值为 0。</returns>
        [LibraryImport(User32, EntryPoint = "GetSystemMetrics", SetLastError = false), PreserveSig]
        public static partial int GetSystemMetrics(SYSTEMMETRICSINDEX nIndex);

        /// <summary>
        /// 检索指定窗口的边框的尺寸。 尺寸以相对于屏幕左上角的屏幕坐标提供。
        /// </summary>
        /// <param name="handleRef">窗口的句柄。</param>
        /// <param name="rect">指向 RECT 结构的指针，该结构接收窗口左上角和右下角的屏幕坐标。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。 要获得更多的错误信息，请调用 GetLastError。</returns>
        [LibraryImport(User32, EntryPoint = "GetWindowRect", SetLastError = false), PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool GetWindowRect(IntPtr handleRef, out RECT rect);

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
        /// 确定窗口是否最大化。
        /// </summary>
        /// <param name="hWnd">要测试的窗口的句柄。</param>
        /// <returns>如果窗口已缩放，则返回值为非零值。如果未缩放窗口，则返回值为零。</returns>
        [LibraryImport(User32, EntryPoint = "IsZoomed", SetLastError = false), PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool IsZoomed(IntPtr hWnd);

        /// <summary>
        /// MapWindowPoints 函数将 (映射) 一组点从相对于一个窗口的坐标空间转换为相对于另一个窗口的坐标空间。
        /// </summary>
        /// <param name="hWndFrom">从中转换点的窗口的句柄。 如果此参数为 NULL 或HWND_DESKTOP，则假定这些点位于屏幕坐标中。</param>
        /// <param name="hWndTo">指向要向其转换点的窗口的句柄。 如果此参数为 NULL 或HWND_DESKTOP，则点将转换为屏幕坐标。</param>
        /// <param name="lpPoints">指向 POINT 结构的数组的指针，该数组包含要转换的点集。 这些点以设备单位为单位。 此参数还可以指向 RECT 结构，在这种情况下， cPoints 参数应设置为 2。</param>
        /// <param name="cPoints">lpPoints 参数指向的数组中的 POINT 结构数。</param>
        /// <returns>如果函数成功，则返回值的低序字是添加到每个源点的水平坐标以计算每个目标点的水平坐标的像素数。 (除此之外，如果正对 hWndFrom 和 hWndTo 之一进行镜像，则每个生成的水平坐标乘以 -1.) 高序字是添加到每个源点垂直坐标的像素数，以便计算每个目标点的垂直坐标。
        /// 如果函数失败，则返回值为零。 在调用此方法之前调用 SetLastError ，以将错误返回值与合法的“0”返回值区分开来。</returns>
        [LibraryImport(User32, EntryPoint = "MapWindowPoints", SetLastError = false), PreserveSig]
        public static unsafe partial int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, ref PointInt32 lpPoints, uint cPoints);

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

        /// <summary>
        /// 更改子窗口、弹出窗口或顶级窗口的大小、位置和 Z 顺序。 这些窗口根据屏幕上的外观进行排序。 最上面的窗口接收最高排名，是 Z 顺序中的第一个窗口。
        /// </summary>
        /// <param name="hWnd">更改子窗口、弹出窗口或顶级窗口的大小、位置和 Z 顺序。 这些窗口根据屏幕上的外观进行排序。 最上面的窗口接收最高排名，是 Z 顺序中的第一个窗口。</param>
        /// <param name="hWndInsertAfter">在 Z 顺序中定位窗口之前窗口的句柄。 </param>
        /// <param name="X">在 Z 顺序中定位窗口之前窗口的句柄。 </param>
        /// <param name="Y">窗口顶部的新位置，以客户端坐标表示。</param>
        /// <param name="cx">窗口的新宽度（以像素为单位）。</param>
        /// <param name="cy">窗口的新高度（以像素为单位）。</param>
        /// <param name="uFlags">窗口大小调整和定位标志。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。 </returns>
        [LibraryImport(User32, EntryPoint = "SetWindowPos", SetLastError = true), PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);
    }
}
