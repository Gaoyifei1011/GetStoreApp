namespace GetStoreAppWindowsAPI.PInvoke.User32
{
    /// <summary>
    /// Windows 消息
    /// </summary>
    public enum WindowMessage : int
    {
        /// <summary>
        /// WM_CLOSE消息作为窗口或应用程序应终止的信号发送。
        /// </summary>
        WM_CLOSE = 0x0010,

        /// <summary>
        /// 当 SystemParametersInfo 函数更改系统范围设置或策略设置发生更改时，将发送到所有顶级窗口的消息
        /// </summary>
        WM_SETTINGCHANGE = 0x001A,

        /// <summary>
        /// 应用程序将 WM_COPYDATA 消息发送到另一个应用程序。
        /// </summary>
        WM_COPYDATA = 0x004A
    }
}
