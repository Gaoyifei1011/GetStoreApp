namespace GetStoreAppInstaller.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 过滤消息时指定的操作
    /// </summary>
    public enum ChangeFilterFlags : uint
    {
        /// <summary>
        /// 将消息添加到筛选器。这具有允许接收消息的效果。
        /// </summary>
        MSGFLT_ADD = 1,

        /// <summary>
        /// 从筛选器中删除消息。这具有阻止消息的效果。
        /// </summary>
        MSGFLT_REMOVE = 2
    }
}
