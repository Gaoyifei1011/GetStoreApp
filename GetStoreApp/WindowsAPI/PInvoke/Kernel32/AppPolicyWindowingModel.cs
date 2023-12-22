namespace GetStoreApp.WindowsAPI.PInvoke.Kernel32
{
    /// <summary>
    /// AppPolicyWindowingModel 枚举指示进程是使用基于 CoreWindow 的窗口模型，还是使用基于 HWND 的窗口模型。
    /// </summary>
    public enum AppPolicyWindowingModel
    {
        /// <summary>
        /// 指示进程没有窗口化模型。
        /// </summary>
        AppPolicyWindowingModel_None = 0,

        /// <summary>
        /// 指示进程的窗口化模型是基于 CoreWindow 的。
        /// </summary>
        AppPolicyWindowingModel_Universal = 1,

        /// <summary>
        /// 指示进程的窗口化模型是基于 HWND 的。
        /// </summary>
        AppPolicyWindowingModel_ClassicDesktop = 2,

        /// <summary>
        /// 指示进程的窗口化模型是基于 Silverlight 的，并且不提供窗口状态更改的通知。
        /// </summary>
        AppPolicyWindowingModel_ClassicPhone = 3,
    }
}
