using System;
using System.Runtime.InteropServices;

namespace GetStoreApp.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// User32.dll 函数库
    /// </summary>
    public static partial class User32Library
    {
        private const string User32 = "user32.dll";

        /// <summary>
        /// 将消息信息传递给指定的窗口过程。
        /// </summary>
        /// <param name="lpPrevWndFunc">
        /// 上一个窗口过程。 如果通过调用设置为 GWL_WNDPROC 或 DWL_DLGPROC 的 nIndex 参数的 GetWindowLong 函数来获取此值，
        /// 则它实际上是窗口或对话框过程的地址，或者仅对 CallWindowProc 有意义的特殊内部值。</param>
        /// <param name="hWnd">用于接收消息的窗口过程的句柄。</param>
        /// <param name="Msg">消息。</param>
        /// <param name="wParam">其他的消息特定信息。 此参数的内容取决于 Msg 参数的值。</param>
        /// <param name="lParam">其他的消息特定信息。 此参数的内容取决于 Msg 参数的值。</param>
        /// <returns>返回值指定消息处理的结果，具体取决于发送的消息。</returns>
        [LibraryImport(User32, EntryPoint = "CallWindowProcW", SetLastError = false)]
        public static partial IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// 修改指定窗口的用户界面特权隔离 (UIPI) 消息筛选器。
        /// </summary>
        /// <param name="hWnd">要修改其 UIPI 消息筛选器的窗口的句柄。</param>
        /// <param name="message">消息筛选器允许通过或阻止的消息。</param>
        /// <param name="action">要执行的操作，可以执行以下值</param>
        /// <param name="pChangeFilterStruct">指向 CHANGEFILTERSTRUCT 结构的可选指针。</param>
        /// <returns>如果函数成功，则返回 TRUE;否则，它将返回 FALSE。</returns>
        [LibraryImport(User32, EntryPoint = "ChangeWindowMessageFilterEx", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool ChangeWindowMessageFilterEx(IntPtr hWnd, WindowMessage message, ChangeFilterAction action, in CHANGEFILTERSTRUCT pChangeFilterStruct);

        /// <summary>
        /// 检索一个窗口的句柄，该窗口的类名和窗口名称与指定的字符串匹配。 该函数搜索子窗口，从指定子窗口后面的子窗口开始。 此函数不执行区分大小写的搜索。
        /// </summary>
        /// <param name="hWndParent">要搜索其子窗口的父窗口的句柄。如果 hwndParent 为 NULL，则该函数使用桌面窗口作为父窗口。 函数在桌面的子窗口之间搜索。 如果 hwndParent 为HWND_MESSAGE，则函数将搜索所有 仅消息窗口。</param>
        /// <param name="hWndChildAfter">子窗口的句柄。 搜索从 Z 顺序中的下一个子窗口开始。 子窗口必须是 hwndParent 的直接子窗口，而不仅仅是子窗口。 如果 hwndChildAfter 为 NULL，则搜索从 hwndParent 的第一个子窗口开始。请注意，如果 hwndParent 和 hwndChildAfter 均为 NULL，则该函数将搜索所有顶级窗口和仅消息窗口。</param>
        /// <param name="lpszClass">类名或上一次对 RegisterClass 或 RegisterClassEx 函数的调用创建的类名或类原子。 原子必须置于 lpszClass 的低序单词中;高阶单词必须为零。如果 lpszClass 是字符串，则指定窗口类名。 类名可以是注册到 RegisterClass 或 RegisterClassEx 的任何名称，也可以是预定义的控件类名称，也可以是 MAKEINTATOM(0x8000)。 在此后一种情况下，0x8000是菜单类的原子。 </param>
        /// <param name="lpszWindow">窗口名称 (窗口的标题) 。 如果此参数为 NULL，则所有窗口名称都匹配。</param>
        /// <returns>如果函数成功，则返回值是具有指定类和窗口名称的窗口的句柄。如果函数失败，则返回值为 NULL。 要获得更多的错误信息，请调用 GetLastError。</returns>
        [LibraryImport(User32, EntryPoint = "FindWindowExW", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
        public static partial IntPtr FindWindowEx(IntPtr hWndParent, IntPtr hWndChildAfter, string lpszClass, string lpszWindow);

        /// <summary>
        /// 检索有关指定窗口的信息。 该函数还会检索 32 位 (DWORD) 值，该值位于指定偏移量处，并进入额外的窗口内存。
        /// </summary>
        /// <param name="hWnd">窗口的句柄，间接地是窗口所属的类。</param>
        /// <param name="nIndex">要检索的值的从零开始的偏移量。 有效值在 0 到额外窗口内存的字节数中，减去 4 个;例如，如果指定了 12 个或更多字节的额外内存，则值 8 将是第三个 32 位整数的索引。
        /// </param>
        /// <returns>如果函数成功，则返回值是请求的值。如果函数失败，则返回值为零。</returns>
        [LibraryImport(User32, EntryPoint = "GetWindowLongW", SetLastError = false)]
        public static partial int GetWindowLong(IntPtr hWnd, WindowLongIndexFlags nIndex);

        /// <summary>
        /// 检索有关指定窗口的信息。 该函数还会检索 64 位 (DWORD) 值，该值位于指定偏移量处，并进入额外的窗口内存。
        /// </summary>
        /// <param name="hWnd">窗口的句柄，间接地是窗口所属的类。</param>
        /// <param name="nIndex">要检索的值的从零开始的偏移量。 有效值的范围为零到额外窗口内存的字节数，减去 LONG_PTR的大小。
        /// </param>
        /// <returns>如果函数成功，则返回值是请求的值。如果函数失败，则返回值为零。</returns>
        [LibraryImport(User32, EntryPoint = "GetWindowLongPtrW", SetLastError = false)]
        public static partial int GetWindowLongPtr(IntPtr hWnd, WindowLongIndexFlags nIndex);

        /// <summary>
        /// 检索创建指定窗口的线程的标识符，以及（可选）创建窗口的进程的标识符。
        /// </summary>
        /// <param name="hwnd">窗口的句柄。</param>
        /// <param name="ID">指向接收进程标识符的变量的指针。如果此参数不为 NULL，则 GetWindowThreadProcessId 将进程的标识符复制到变量;否则，它不会。</param>
        /// <returns>返回值是创建窗口的线程的标识符。</returns>
        [LibraryImport(User32, EntryPoint = "GetWindowThreadProcessId", SetLastError = true)]
        public static partial int GetWindowThreadProcessId(IntPtr hwnd, out uint lpdwProcessId);

        /// <summary>
        /// 将创建指定窗口的线程引入前台并激活窗口。 键盘输入将定向到窗口，并为用户更改各种视觉提示。 系统为创建前台窗口的线程分配的优先级略高于其他线程的优先级。
        /// </summary>
        /// <param name="hWnd">应激活并带到前台的窗口的句柄。</param>
        /// <returns>如果将窗口带到前台，则返回值为非零值。如果未将窗口带到前台，则返回值为零。</returns>
        [LibraryImport(User32, EntryPoint = "SetForegroundWindow", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool SetForegroundWindow(IntPtr hWnd);

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
        [LibraryImport(User32, EntryPoint = "SendMessageW", SetLastError = false)]
        public static partial IntPtr SendMessage(IntPtr hWnd, WindowMessage wMsg, int wParam, IntPtr lParam);

        /// <summary>
        /// 更改指定子窗口的父窗口。
        /// </summary>
        /// <param name="hWndChild">子窗口的句柄。</param>
        /// <param name="hWndNewParent">新父窗口的句柄。 如果此参数为 NULL，桌面窗口将成为新的父窗口。 如果此参数 HWND_MESSAGE，则子窗口将成为 仅消息窗口。</param>
        /// <returns>如果函数成功，则返回值是上一个父窗口的句柄。如果函数失败，则返回值为 NULL。</returns>
        [LibraryImport(User32, EntryPoint = "SetParent", SetLastError = false)]
        public static partial IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        /// <summary>
        /// 更改指定窗口的属性。 该函数还将指定偏移量处的32位（long类型）值设置到额外的窗口内存中。
        /// </summary>
        /// <param name="hWnd">窗口的句柄，间接地是窗口所属的类</param>
        /// <param name="nIndex">要设置的值的从零开始的偏移量。 有效值的范围为零到额外窗口内存的字节数，减去整数的大小。</param>
        /// <param name="newProc">新事件处理函数（回调函数）</param>
        /// <returns>如果函数成功，则返回值是指定 32 位整数的上一个值。如果函数失败，则返回值为零。 </returns>
        [LibraryImport(User32, EntryPoint = "SetWindowLongW", SetLastError = false)]
        public static partial IntPtr SetWindowLong(IntPtr hWnd, WindowLongIndexFlags nIndex, IntPtr dwNewLong);

        /// <summary>
        /// 更改指定窗口的属性。 该函数还将指定偏移量处的64位（long类型）值设置到额外的窗口内存中。
        /// </summary>
        /// <param name="hWnd">窗口的句柄，间接地是窗口所属的类</param>
        /// <param name="nIndex">要设置的值的从零开始的偏移量。 有效值的范围为零到额外窗口内存的字节数，减去整数的大小。</param>
        /// <param name="newProc">新事件处理函数（回调函数）</param>
        /// <returns>如果函数成功，则返回值是指定偏移量的上一个值。如果函数失败，则返回值为零。 </returns>
        [LibraryImport(User32, EntryPoint = "SetWindowLongPtrW", SetLastError = false)]
        public static partial IntPtr SetWindowLongPtr(IntPtr hWnd, WindowLongIndexFlags nIndex, IntPtr dwNewLong);
    }
}
