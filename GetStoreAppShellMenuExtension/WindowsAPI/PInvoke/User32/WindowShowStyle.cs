namespace WindowsToolsShellExtension.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 使用 ShowWindow 显示窗口的不同方式的枚举。
    /// </summary>
    public enum WindowShowStyle : uint
    {
        /// <summary>
        /// 隐藏窗口并激活另一个窗口。
        /// </summary>
        SW_HIDE = 0,

        /// <summary>
        /// 激活并显示窗口。 如果窗口最小化或最大化，系统会将其还原到其原始大小和位置。 首次显示窗口时，应用程序应指定此标志。
        /// </summary>
        SW_SHOWNORMAL = 1,

        /// <summary>
        /// 激活窗口并将其显示为最小化窗口。
        /// </summary>
        SW_SHOWMINIMIZED = 2,

        /// <summary>
        /// 激活窗口并显示最大化的窗口。
        /// </summary>
        SW_SHOWMAXIMIZED = 3,

        /// <summary>
        /// 激活窗口并显示最大化的窗口。
        /// </summary>
        SW_MAXIMIZE = SW_SHOWMAXIMIZED,

        /// <summary>
        /// 在其最近的大小和位置显示一个窗口。 此值类似于 SW_SHOWNORMAL，但窗口未激活。
        /// </summary>
        SW_SHOWNOACTIVATE = 4,

        /// <summary>
        /// 激活窗口并以当前大小和位置显示窗口。
        /// </summary>
        SW_SHOW = 5,

        /// <summary>
        /// 最小化指定的窗口，并按 Z 顺序激活下一个顶级窗口。
        /// </summary>
        SW_MINIMIZE = 6,

        /// <summary>
        /// 将窗口显示为最小化窗口。 此值类似于 SW_SHOWMINIMIZED，但窗口未激活。
        /// </summary>
        SW_SHOWMINNOACTIVE = 7,

        /// <summary>
        /// 以当前大小和位置显示窗口。 此值类似于 SW_SHOW，但窗口未激活。
        /// </summary>
        SW_SHOWNA = 8,

        /// <summary>
        /// 激活并显示窗口。 如果窗口最小化或最大化，系统会将其还原到其原始大小和位置。 还原最小化窗口时，应用程序应指定此标志。
        /// </summary>
        SW_RESTORE = 9,

        /// <summary>
        /// 根据启动应用程序的程序传递给 CreateProcess 函数的 STARTUPINFO 结构中指定的值设置显示状态。
        /// </summary>
        SW_SHOWDEFAULT = 10,

        /// <summary>
        /// 即使拥有窗口的线程未响应，也会最小化窗口。 仅当将窗口从不同的线程最小化时，才应使用此标志。
        /// </summary>
        SW_FORCEMINIMIZE = 11
    }
}
