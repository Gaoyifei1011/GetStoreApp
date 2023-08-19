namespace GetStoreApp.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 过滤消息时指定的操作
    /// </summary>
    public enum ChangeFilterAction : uint
    {
        /// <summary>
        /// 将 hWnd 的窗口消息筛选器重置为默认值。 允许全局或进程范围的任何消息都将通过，但这两个类别中不包含的任何消息（来自较低特权进程）将被阻止。
        /// </summary>
        MSGFLT_RESET = 0,

        /// <summary>
        /// 允许通过筛选器的消息。 这允许 hWnd 接收消息，而不管消息的来源如何，即使消息来自较低特权进程也是如此。
        /// </summary>
        MSGFLT_ALLOW = 1,

        /// <summary>
        /// 如果消息来自较低特权进程，则阻止将消息传递到 hWnd ，除非使用 ChangeWindowMessageFilter 函数或全局允许该消息进程范围。
        /// </summary>
        MSGFLT_DISALLOW = 2
    }
}
