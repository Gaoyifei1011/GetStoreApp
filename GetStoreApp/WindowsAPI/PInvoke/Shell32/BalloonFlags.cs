namespace GetStoreApp.WindowsAPI.PInvoke.Shell32
{
    /// <summary>
    /// 定义气球工具提示上显示的图标的标志。
    /// </summary>
    public enum BalloonFlags
    {
        /// <summary>
        /// 不显示任何图标。
        /// </summary>
        None = 0x00,

        /// <summary>
        /// 将显示一个信息图标。
        /// </summary>
        Info = 0x01,

        /// <summary>
        /// 将显示一个警告图标。
        /// </summary>
        Warning = 0x02,

        /// <summary>
        /// 将显示一个错误图标。
        /// </summary>
        Error = 0x03,

        /// <summary>
        /// Windows XP Service Pack 2 （SP2） 及更高版本。
        /// 使用自定义图标作为标题图标。
        /// </summary>
        User = 0x04,

        /// <summary>
        /// Windows XP（Shell32.dll版本 6.0）及更高版本。
        /// 不要播放相关声音。仅适用于气球工具提示。
        /// </summary>
        NoSound = 0x10,

        /// <summary>
        /// Windows Vista（Shell32.dll版本 6.0.6）及更高版本。应将大版本的图标用作气球图标。这对应于尺寸为 SM_CXICON x SM_CYICON 的图标。
        /// 如果未设置此标志，则使用尺寸为 x SM_CYSMICON XM_CXSMICON 的图标。
        /// - 此标志可用于所有库存图标。
        /// - 使用较旧的自定义图标（NIIF_USER带有 hIcon）的应用程序必须在托盘图标 （hIcon） 中提供新的 SM_CXICON x SM_CYICON 版本。
        ///   当这些图标显示在系统托盘或系统控制区域 （SCA） 中时，它们会缩小。
        /// - 新的自定义图标（NIIF_USER带有 hBalloonIcon 的图标）必须在提供的图标 （hBalloonIcon） 中提供 SM_CXICON x SM_CYICON 版本。
        /// </summary>
        LargeIcon = 0x20,

        /// <summary>
        /// Windows 7 及更高版本。
        /// </summary>
        RespectQuietTime = 0x80
    }
}
