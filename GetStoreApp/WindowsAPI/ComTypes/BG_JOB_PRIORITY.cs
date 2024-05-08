namespace GetStoreApp.WindowsAPI.ComTypes
{
    /// <summary>
    /// 定义指定作业优先级的常量。
    /// </summary>
    public enum BG_JOB_PRIORITY
    {
        /// <summary>
        /// 在前台传输作业。 前台传输与其他应用程序争用网络带宽，这可能会妨碍用户的网络体验。 这是最高优先级。
        /// </summary>
        BG_JOB_PRIORITY_FOREGROUND = 0,

        /// <summary>
        /// 以高优先级在后台传输作业。 后台传输使用客户端的空闲网络带宽来传输文件。 这是最高后台优先级。
        /// </summary>
        BG_JOB_PRIORITY_HIGH = 1,

        /// <summary>
        /// 以正常优先级在后台传输作业。 后台传输使用客户端的空闲网络带宽来传输文件。 这是默认的优先级。
        /// </summary>
        BG_JOB_PRIORITY_NORMAL = 2,

        /// <summary>
        /// 以低优先级在后台传输作业。 后台传输使用客户端的空闲网络带宽来传输文件。 这是最低的后台优先级别。
        /// </summary>
        BG_JOB_PRIORITY_LOW = 3,
    }
}
