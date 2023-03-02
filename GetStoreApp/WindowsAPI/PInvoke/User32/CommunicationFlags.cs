namespace GetStoreApp.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 自定义消息：进程通信时对应的标志
    /// </summary>
    public enum CommunicationFlags : int
    {
        /// <summary>
        /// 打开应用“关于”页面
        /// </summary>
        AboutApp = 0,

        /// <summary>
        /// 显示 / 隐藏窗口
        /// </summary>
        ShowOrHideWindow = 1,

        /// <summary>
        /// 打开设置
        /// </summary>
        Settings = 2,

        /// <summary>
        /// 退出应用
        /// </summary>
        Exit = 3,
    }
}
