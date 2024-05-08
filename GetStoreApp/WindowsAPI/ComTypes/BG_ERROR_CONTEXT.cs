namespace GetStoreApp.WindowsAPI.ComTypes
{
    /// <summary>
    /// 定义指定发生错误的上下文的常量。
    /// </summary>
    public enum BG_ERROR_CONTEXT
    {
        /// <summary>
        /// 未发生错误。
        /// </summary>
        BG_ERROR_CONTEXT_NONE = 0,

        /// <summary>
        /// 错误上下文未知。
        /// </summary>
        BG_ERROR_CONTEXT_UNKNOWN = 1,

        /// <summary>
        /// 传输队列管理器生成了错误。
        /// </summary>
        BG_ERROR_CONTEXT_GENERAL_QUEUE_MANAGER = 2,

        /// <summary>
        /// 错误是在队列管理器通知客户端某个事件时生成的。
        /// </summary>
        BG_ERROR_CONTEXT_QUEUE_MANAGER_NOTIFICATION = 3,

        /// <summary>
        /// 此错误与指定的本地文件相关。 例如，权限被拒绝或卷不可用。
        /// </summary>
        BG_ERROR_CONTEXT_LOCAL_FILE = 4,

        /// <summary>
        /// 此错误与指定的远程文件相关。 例如，URL 不可访问。
        /// </summary>
        BG_ERROR_CONTEXT_REMOTE_FILE = 5,

        /// <summary>
        /// 传输层生成了错误。 这些错误是常规传输失败， (这些错误不特定于远程文件) 。
        /// </summary>
        BG_ERROR_CONTEXT_GENERAL_TRANSPORT = 6,

        /// <summary>
        /// BITS 向其传递上传文件的服务器应用程序在处理上传文件时生成了错误。
        /// </summary>
        BG_ERROR_CONTEXT_REMOTE_APPLICATION = 7
    }
}
