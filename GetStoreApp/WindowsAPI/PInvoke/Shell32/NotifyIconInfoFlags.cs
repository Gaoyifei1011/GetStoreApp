namespace GetStoreApp.WindowsAPI.PInvoke.Shell32
{
    /// <summary>
    /// 气球通知的行为和外观的标志
    /// </summary>
    public enum NotifyIconInfoFlags
    {
        /// <summary>
        /// 无图标。
        /// </summary>
        NIIF_NONE = 0x00,

        /// <summary>
        /// 信息图标。
        /// </summary>
        NIIF_INFO = 0x01,

        /// <summary>
        /// 警告图标。
        /// </summary>
        NIIF_WARNING = 0x02,

        /// <summary>
        /// 错误图标。
        /// </summary>
        NIIF_ERROR = 0x03,

        /// <summary>
        /// Windows XP：使用 hIcon 中标识的图标作为通知气球的标题图标。
        /// Windows Vista 及更高版本：使用 hBalloonIcon 中标识的图标作为通知气球的标题图标。
        /// </summary>
        NIIF_USER = 0x04,

        /// <summary>
        /// Windows XP 及更高版本。不要播放关联的声音。仅适用于通知。
        /// </summary>
        NIIF_NOSOUND = 0x10,

        /// <summary>
        /// Windows Vista 及更高版本。 图标的大版本应用作通知图标。 这对应于尺寸SM_CXICON x SM_CYICON的图标。 如果未设置此标志，则使用尺寸为 SM_CXSMICON x SM_CYSMICON 的图标。
        /// 此标志可以与所有 股票图标一起使用。
        /// 使用 hIcon) (NIIF_USER较旧的自定义图标的应用程序必须在托盘图标中提供新的 SM_CXICON x SM_CYICON 版本， (hIcon) 。 当这些图标显示在“系统托盘”或“系统控制区” (SCA) 时，它们会纵向缩减。
        /// 使用 hBalloonIcon 的新自定义图标 (NIIF_USER必须在提供的图标中提供SM_CXICON x SM_CYICON 版本 (hBalloonIcon) 。
        /// </summary>
        NIIF_LARGE_ICON = 0x20,

        /// <summary>
        ///  Windows 7 及更高版本。 如果当前用户处于“安静时间”，则不显示气球通知，这是新用户首次登录其帐户后的第一小时。 在此期间，不应发送或显示大多数通知。 这使用户习惯于新的计算机系统，而不会出现这些干扰。 在操作系统升级或干净安装后，每个用户也会有静默时间。 在静默时间使用此标志发送的通知不会排队;它只是被不屑一顾。 如果通知在当时仍然有效，则应用程序可以稍后重新发送通知。
        /// 由于应用程序无法预测何时可能会遇到静默时间，因此我们建议始终由任何应用程序在所有适当的通知上设置此标志，以便遵守静默时间。
        /// 在静默期间，仍应发送某些通知，因为用户期望它们作为响应用户操作的反馈，例如，当用户插入 USB 设备或打印文档时。
        /// 如果当前用户不在安静时间，则此标志不起作用。
        /// </summary>
        NIIF_RESPECT_QUIET_TIME = 0x80,

        /// <summary>
        ///  Windows XP 及更高版本。 保留。
        /// </summary>
        NIIF_ICON_MASK = 0x0F
    }
}
