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
        /// 当仅针对当前正在运行的进程创建没有父级或所有者的窗口时，更改默认布局。
        /// </summary>
        /// <param name="dwDefaultLayout">默认进程布局。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [LibraryImport(User32, EntryPoint = "GetDesktopWindow", SetLastError = false), PreserveSig]
        public static partial IntPtr GetDesktopWindow();

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
        /// 设置指定窗口的显示状态。
        /// </summary>
        /// <param name="hWnd">窗口的句柄。</param>
        /// <param name="nCmdShow">控制窗口的显示方式。 如果启动应用程序的程序提供 STARTUPINFO 结构，则应用程序首次调用 ShowWindow 时将忽略此参数。 否则，首次调用 ShowWindow 时，该值应为 WinMain 函数在其 nCmdShow 参数中获取的值。</param>
        /// <returns>如果窗口以前可见，则返回值为非零值。如果以前隐藏窗口，则返回值为零。</returns>
        [LibraryImport(User32, EntryPoint = "ShowWindow", SetLastError = false), PreserveSig]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool ShowWindow(IntPtr hWnd, WindowShowStyle nCmdShow);
    }
}
