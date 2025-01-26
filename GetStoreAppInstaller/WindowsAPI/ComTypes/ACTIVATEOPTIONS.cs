namespace GetStoreAppInstaller.WindowsAPI.ComTypes
{
    /// <summary>
    /// 以下一个或多个标志用于支持设计模式、调试和测试方案。
    /// </summary>
    public enum ACTIVATEOPTIONS
    {
        /// <summary>
        /// 不设置任何标志。
        /// </summary>
        AO_NONE = 0x00000000,

        /// <summary>
        /// 应用正在为设计模式激活，因此无法创建其正常窗口。 应用窗口的创建必须由设计工具完成，这些工具通过与通过激活管理器建立的站点链上的设计器指定的服务通信来加载必要的组件。 请注意，这意味着在常规激活期间看不到初始屏幕。
        /// 请注意，必须在应用的包上 启用调试模式 才能成功使用设计模式。
        /// </summary>
        AO_DESIGNMODE = 0x00000001,

        /// <summary>
        /// 如果应用无法激活，则不显示错误对话框。
        /// </summary>
        AO_NOERRORUI = 0x00000002,

        /// <summary>
        /// 激活应用时，不要显示应用的初始屏幕。 使用此标志时，必须在应用的包上 启用调试模式 ;否则，PLM 将在几秒钟后终止应用。
        /// </summary>
        AO_NOSPLASHSCREEN = 0x00000004,

        /// <summary>
        /// 应用程序正在预启动模式下激活。 从 Windows 10 开始支持此值。
        /// </summary>
        AO_PRELAUNCH = 0x2000000
    }
}
