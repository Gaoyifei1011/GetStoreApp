namespace GetStoreAppWebView.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 控制最小化窗口的位置以及还原窗口的方法的标志。
    /// </summary>
    internal enum WINDOWPLACEMENTFLAGS
    {
        /// <summary>
        /// 可以指定最小化窗口的坐标。如果在 ptMinPosition 成员中设置了坐标，则必须指定此标志。
        /// </summary>
        WPF_SETMINPOSITION = 0x0001,

        /// <summary>
        /// 无论还原的窗口在最小化之前是否最大化，都将最大化还原的窗口。 此设置仅在下次还原窗口时有效。 它不会更改默认还原行为。仅当为 showCmd 成员指定了SW_SHOWMINIMIZED值时，此标志才有效。
        /// </summary>
        WPF_RESTORETOMAXIMIZED = 0x0002,

        /// <summary>
        /// 如果调用线程和拥有窗口的线程附加到不同的输入队列，则系统会将请求发布到拥有窗口的线程。 这可以防止调用线程阻止其执行，而其他线程处理请求。
        /// </summary>
        WPF_ASYNCWINDOWPLACEMENT = 0x0004
    }
}
