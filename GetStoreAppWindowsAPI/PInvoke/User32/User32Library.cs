using GetStoreAppWindowsAPI.PInvoke.WindowsCore;
using System;
using System.Runtime.InteropServices;

namespace GetStoreAppWindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 从 User32.dll Windows库中导出的函数。
    /// </summary>
    public static class User32Library
    {
        private const string User32 = "User32.dll";

        /// <summary>
        /// 将焦点切换到指定的窗口，并将其带到前台。
        /// </summary>
        /// <param name="hWnd">窗口的句柄。</param>
        /// <param name="fAltTab">此参数的 TRUE 指示窗口正切换到使用 Alt/Ctl+Tab 键序列。 否则此参数应为 FALSE 。</param>
        [DllImport("user32.dll ", SetLastError = true)]
        public static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        /// <summary>
        /// 将消息信息传递给指定的窗口过程。
        /// </summary>
        /// <param name="lpPrevWndFunc">
        /// 上一个窗口过程。 如果通过调用设置为GWL_WNDPROC或DWL_DLGPROC的 nIndex 参数的 GetWindowLong 函数来获取此值，
        /// 则它实际上是窗口或对话框过程的地址，或者仅对 CallWindowProc 有意义的特殊内部值。</param>
        /// <param name="hWnd">用于接收消息的窗口过程的句柄。</param>
        /// <param name="Msg">消息。</param>
        /// <param name="wParam">其他的消息特定信息。 此参数的内容取决于 Msg 参数的值。</param>
        /// <param name="lParam">其他的消息特定信息。 此参数的内容取决于 Msg 参数的值。</param>
        /// <returns>返回值指定消息处理的结果，具体取决于发送的消息。</returns>
        [DllImport(User32)]
        public static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// 返回指定窗口的每英寸点 (dpi) 值。
        /// </summary>
        /// <param name="hwnd">要获取相关信息的窗口。</param>
        /// <returns>窗口的 DPI，无效 的 hwnd 值将导致返回值 0。</returns>
        [DllImport(User32)]
        public static extern int GetDpiForWindow(IntPtr hwnd);

        /// <summary>
        /// 检索顶级窗口的句柄，该窗口的类名称和窗口名称与指定的字符串匹配。 此函数不搜索子窗口。 此函数不执行区分大小写的搜索。
        /// </summary>
        /// <param name="lpClassName">
        /// 类名或上一次对 RegisterClass 或 RegisterClassEx 函数的调用创建的类名或类原子。原子必须位于 lpClassName 的低序单词中;高阶单词必须为零。
        /// 如果 lpClassName 指向字符串，则指定窗口类名。 类名可以是向 RegisterClass 或 RegisterClassEx 注册的任何名称，也可以是预定义控件类名称中的任何名称。
        /// 如果 lpClassName 为 NULL，它将查找其标题与 lpWindowName 参数匹配的任何窗口。
        /// </param>
        /// <param name="lpWindowName">窗口名称 (窗口的标题) 。 如果此参数为 NULL，则所有窗口名称都匹配。</param>
        /// <returns>如果函数成功，则返回值是具有指定类名和窗口名称的窗口的句柄。 如果函数失败，则返回值为 NULL。 </returns>
        [DllImport(User32, EntryPoint = "FindWindow")]
        public static extern int FindWindow(string lpClassName, string lpWindowName);

        /// <summary>检索前台窗口的句柄， (用户当前正在使用的窗口) 。 系统向创建前台窗口的线程分配略高于其他线程的优先级。</summary>
        /// <returns>返回值是前台窗口的句柄。 在某些情况下，前台窗口可以为 NULL ，例如窗口丢失激活时。</returns>
        [DllImport(User32, SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// 检索指定窗口的边界矩形的尺寸。 尺寸以相对于屏幕左上角的屏幕坐标提供。
        /// </summary>
        /// <param name="hWnd">窗口的句柄。</param>
        /// <param name="lpRect">指向 RECT 结构的指针，用于接收窗口左上角和右下角的屏幕坐标。</param>
        /// <returns>如果该函数成功，则返回值为非零值。如果函数失败，则返回值为零。</returns>
        [DllImport(nameof(User32), SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        /// <summary>
        /// 确定窗口是否最大化
        /// </summary>
        /// <param name="hWnd">要测试的窗口的句柄</param>
        /// <returns>如果缩放窗口，则返回值为非零。如果未缩放窗口，则返回值为零。</returns>
        [DllImport(User32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsZoomed(IntPtr hWnd);

        /// <summary>
        /// 显示一个模式对话框，其中包含系统图标、一组按钮和一条简短的应用程序特定消息，例如状态或错误信息。 消息框返回一个整数值，该值指示用户单击的按钮。
        /// </summary>
        /// <param name="hWnd">要创建的消息框的所有者窗口的句柄。 如果此参数为 NULL，则消息框没有所有者窗口。</param>
        /// <param name="lptext">要显示的消息。 如果字符串包含多个行，则可以使用回车符和/或换行符分隔每行之间的行。</param>
        /// <param name="lpcaption">对话框标题。 如果此参数为 NULL，则默认标题为 Error。</param>
        /// <param name="options">对话框的内容和行为。</param>
        /// <returns>
        /// 如果消息框有 “取消 ”按钮，则函数返回 IDCANCEL 值（如果按下 ESC 键或选中 “取消 ”按钮）。
        /// 如果消息框没有 “取消 ”按钮，则按 ESC 将不起作用 -除非存在MB_OK按钮。 如果显示MB_OK按钮，并且用户按 ESC，则返回值为 IDOK。
        /// 如果函数失败，则返回值为零。
        /// 如果函数成功，则返回值为 MessageBoxResult 的枚举值之一。
        /// </returns>
        [DllImport(User32, SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern MessageBoxResult MessageBox(IntPtr hWnd, string lptext, string lpcaption, MessageBoxOptions options);

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
        [DllImport(User32, EntryPoint = "SendMessage")]
        public static extern IntPtr SendMessage(IntPtr hWnd, WindowMessage wMsg, int wParam, ref CopyDataStruct lParam);

        /// <summary>
        /// 更改指定窗口的属性。 该函数还将指定偏移量处的32位（long类型）值设置到额外的窗口内存中。
        /// </summary>
        /// <param name="hWnd">窗口的句柄，间接地是窗口所属的类</param>
        /// <param name="nIndex">要设置的值的从零开始的偏移量。 有效值的范围为零到额外窗口内存的字节数，减去整数的大小。</param>
        /// <param name="newProc">新事件处理函数（回调函数）</param>
        /// <returns>如果函数成功，则返回值是指定 32 位整数的上一个值。如果函数失败，则返回值为零。 </returns>
        [DllImport(User32, EntryPoint = "SetWindowLong")]
        public static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, WindowLongIndexFlags nIndex, WinProc newProc);

        /// <summary>
        /// 更改指定窗口的属性。 该函数还将指定偏移量处的64位（long类型）值设置到额外的窗口内存中。
        /// </summary>
        /// <param name="hWnd">窗口的句柄，间接地是窗口所属的类</param>
        /// <param name="nIndex">要设置的值的从零开始的偏移量。 有效值的范围为零到额外窗口内存的字节数，减去整数的大小。</param>
        /// <param name="newProc">新事件处理函数（回调函数）</param>
        /// <returns>如果函数成功，则返回值是指定偏移量的上一个值。如果函数失败，则返回值为零。 </returns>
        [DllImport(User32, EntryPoint = "SetWindowLongPtr")]
        public static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, WindowLongIndexFlags nIndex, WinProc newProc);

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
        [DllImport(nameof(User32), SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int X,
            int Y,
            int cx,
            int cy,
            SetWindowPosFlags uFlags);

        /// <summary>
        /// 设置指定窗口的显示状态。
        /// </summary>
        /// <param name="hWnd">窗口的句柄。</param>
        /// <param name="nCmdShow">
        /// 控制窗口的显示方式。 如果启动应用程序的程序提供 STARTUPINFO 结构，则首次调用 ShowWindow 时忽略此参数。
        /// 否则，首次调用 ShowWindow 时，该值应该是 WinMain 函数在其 nCmdShow 参数中获取的值。
        /// </param>
        /// <returns>如果窗口以前可见，则返回值为非零。如果窗口之前已隐藏，则返回值为零。</returns>
        [DllImport(nameof(User32))]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd, WindowShowStyle nCmdShow);
    }
}
