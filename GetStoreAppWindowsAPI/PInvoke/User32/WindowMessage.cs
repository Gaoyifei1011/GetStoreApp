namespace GetStoreAppWindowsAPI.PInvoke.User32
{
    /// <summary>
    /// Windows 消息
    /// </summary>
    public enum WindowMessage : int
    {
        /// <summary>
        /// 当 SystemParametersInfo 函数更改系统范围设置或策略设置发生更改时，将发送到所有顶级窗口的消息
        /// </summary>
        WM_SETTINGCHANGE = 0x001A,

        /// <summary>
        /// 当窗口的大小或位置即将更改时，发送到窗口。 应用程序可以使用此消息替代窗口的默认最大化大小和位置，或者默认的最小或最大跟踪大小。
        /// </summary>
        WM_GETMINMAXINFO = 0x0024,
        /// <summary>
        /// 应用程序将 WM_COPYDATA 消息发送到另一个应用程序。
        /// </summary>
        WM_COPYDATA = 0x004A
    }
}
