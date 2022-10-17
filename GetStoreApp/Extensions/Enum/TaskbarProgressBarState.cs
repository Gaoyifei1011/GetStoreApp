namespace GetStoreApp.Extensions.Enum
{
    /// <summary>
    /// 表示缩略图进度条状态。
    /// </summary>
    public enum TaskbarProgressBarState
    {
        /// <summary>
        /// 不显示任何进度。
        /// </summary>
        NoProgress = 0,

        /// <summary>
        /// 进度不确定（选取框）。
        /// </summary>
        Indeterminate = 0x1,

        /// <summary>
        /// 显示正常进度。
        /// </summary>
        Normal = 0x2,

        /// <summary>
        /// 发生错误（红色）。
        /// </summary>
        Error = 0x4,

        /// <summary>
        /// 操作已暂停（黄色）。
        /// </summary>
        Paused = 0x8
    }
}
