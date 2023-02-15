namespace GetStoreApp.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 更改窗口大小后具体的行为
    /// </summary>
    public enum SizeMode : int
    {
        /// <summary>
        /// 窗口已调整大小，但 <see cref="SIZE_MINIMIZED"> 和 <see cref="SIZE_MAXIMIZED"> 值均未应用。
        /// </summary>
        SIZE_RESTORED = 0,

        /// <summary>
        /// 窗口已最小化。
        /// </summary>
        SIZE_MINIMIZED = 1,

        /// <summary>
        /// 窗口已最大化。
        /// </summary>
        SIZE_MAXIMIZED = 2,

        /// <summary>
        /// 当其他一些窗口还原到其以前的大小时，消息将发送到所有弹出窗口。
        /// </summary>
        SIZE_MAXSHOW = 3,

        /// <summary>
        /// 当其他一些窗口最大化时，消息将发送到所有弹出窗口。
        /// </summary>
        SIZE_MAXHIDE = 4,
    }
}
