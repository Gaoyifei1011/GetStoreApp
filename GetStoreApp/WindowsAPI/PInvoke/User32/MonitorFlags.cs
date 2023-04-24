namespace GetStoreApp.WindowsAPI.PInvoke.Shell32
{
    /// <summary>
    /// 如果窗口不与任何显示监视器相交，则确定函数的返回值。
    /// </summary>
    public enum MonitorFlags : int
    {
        /// <summary>
        /// 返回 NULL。
        /// </summary>
        MONITOR_DEFAULTTONULL = 0,

        /// <summary>
        /// 返回最接近窗口的显示监视器的句柄。
        /// </summary>
        MONITOR_DEFAULTTONEAREST = 1,

        /// <summary>
        /// 返回主显示监视器的句柄。
        /// </summary>
        MONITOR_DEFAULTTOPRIMARY = 2,
    }
}
