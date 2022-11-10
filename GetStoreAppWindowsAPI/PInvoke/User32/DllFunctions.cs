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
    }
}
