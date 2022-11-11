using System;
using System.Runtime.InteropServices;

namespace GetStoreAppWindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 从 User32.dll Windows库中导出的函数。
    /// </summary>
    public static class DllFunctions
    {
        private const string User32 = "User32.dll";

        /// <summary>检索前台窗口的句柄， (用户当前正在使用的窗口) 。 系统向创建前台窗口的线程分配略高于其他线程的优先级。</summary>
        /// <returns>返回值是前台窗口的句柄。 在某些情况下，前台窗口可以为 NULL ，例如窗口丢失激活时。</returns>
        [DllImport(User32, SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr GetForegroundWindow();

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
    }
}
