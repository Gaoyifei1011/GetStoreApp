namespace GetStoreApp.WindowsAPI.PInvoke.User32
{
    /// <summary>
    /// 设置过滤消息筛选器后返回的结果值
    /// </summary>
    public enum ChangeFilterStatus : uint
    {
        /// <summary>
        /// 适用于 MSGFLT_ALLOW 和 MSGFLT_DISALLOW。
        /// </summary>
        MSGFLTINFO_NONE = 0,

        /// <summary>
        /// 此窗口的消息筛选器已允许该消息，因此该函数成功，不会更改窗口的消息筛选器。 适用于 MSGFLT_ALLOW。
        /// </summary>
        MSGFLTINFO_ALREADYALLOWED_FORWND = 1,

        /// <summary>
        /// 该消息已被此窗口的消息筛选器阻止，因此该函数成功，不会更改窗口的消息筛选器。 适用于 MSGFLT_DISALLOW。
        /// </summary>
        MSGFLTINFO_ALREADYDISALLOWED_FORWND = 2,

        /// <summary>
        /// 在高于窗口的范围允许该消息。 适用于 MSGFLT_DISALLOW。
        /// </summary>
        MSGFLTINFO_ALLOWED_HIGHER = 3
    }
}
