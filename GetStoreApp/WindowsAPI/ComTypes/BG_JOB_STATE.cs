namespace GetStoreApp.WindowsAPI.ComTypes
{
    /// <summary>
    /// 定义指定作业的不同状态的常量。
    /// </summary>
    public enum BG_JOB_STATE
    {
        /// <summary>
        /// 指定作业在队列中并等待运行。 如果用户在传输作业时注销，作业将转换为排队状态。
        /// </summary>
        BG_JOB_STATE_QUEUED = 0,

        /// <summary>
        /// 指定 BITS 正在尝试连接到服务器。 如果连接成功，作业的状态将变为 BG_JOB_STATE_TRANSFERRING;否则，状态变为 BG_JOB_STATE_TRANSIENT_ERROR。
        /// </summary>
        BG_JOB_STATE_CONNECTING = 1,

        /// <summary>
        /// 指定 BITS 正在传输作业的数据。
        /// </summary>
        BG_JOB_STATE_TRANSFERRING = 2,

        /// <summary>
        /// 指定作业暂停 (暂停) 。 若要暂停作业，请调用 IBackgroundCopyJob：：Suspend 方法。 创建作业时，BITS 会自动挂起该作业。 在调用 IBackgroundCopyJob：：Resume 方法、IBackgroundCopyJob：：Complete 方法或 IBackgroundCopyJob：：Cancel 方法之前，作业将保持挂起状态。
        /// </summary>
        BG_JOB_STATE_SUSPENDED = 3,

        /// <summary>
        /// 指定服务无法将文件传输) (发生不可恢复的错误。 如果错误（例如拒绝访问错误）可以更正，请调用 IBackgroundCopyJob：：Resume 方法 修复错误后。 但是，如果错误无法更正，请调用 IBackgroundCopyJob：：Cancel 方法 取消作业，或调用 IBackgroundCopyJob：：Complete 方法 接受已成功传输的下载作业部分。
        /// </summary>
        BG_JOB_STATE_ERROR = 4,

        /// <summary>
        /// 指定发生可恢复的错误。 BITS 将根据指定的重试间隔以暂时性错误状态重试作业 (请参阅 IBackgroundCopyJob：：SetMinimumRetryDelay 方法) 。 如果作业无法取得进展 (请参阅 IBackgroundCopyJob：：SetNoProgressTimeout 方法) ，作业的状态将更改为BG_JOB_STATE_ERROR。
        /// 如果网络断开连接或发生磁盘锁定错误，BITS 不会重试作业，例如chkdsk， (正在运行) ，或者 MaxInternetBandwidth 组策略为零。
        /// </summary>
        BG_JOB_STATE_TRANSIENT_ERROR = 5,

        /// <summary>
        /// 指定已成功处理作业。 必须调用
        /// IBackgroundCopyJob：：Complete 方法 确认作业完成，并使文件可供客户端使用。
        /// </summary>
        BG_JOB_STATE_TRANSFERRED = 6,

        /// <summary>
        /// 指定调用 IBackgroundCopyJob：：Complete 方法来 确认作业已成功完成。
        /// </summary>
        BG_JOB_STATE_ACKNOWLEDGED = 7,

        /// <summary>
        /// 指定调用 IBackgroundCopyJob：：Cancel 方法 取消作业 (即从传输队列中删除作业) 。
        /// </summary>
        BG_JOB_STATE_CANCELLED = 8,
    }
}
