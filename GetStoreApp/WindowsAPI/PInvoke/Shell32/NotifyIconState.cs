namespace GetStoreApp.WindowsAPI.PInvoke.Shell32
{
    public enum NotifyIconState
    {
        /// <summary>
        /// 图标处于显示状态。
        /// </summary>
        NIS_VISABLE = 0x00,

        /// <summary>
        /// 图标处于隐藏状态。
        /// </summary>
        NIS_HIDDEN = 0x01,

        /// <summary>
        /// 图标资源在多个图标之间共享。
        /// </summary>
        NIS_SHAREDICON = 0x02
    }
}
