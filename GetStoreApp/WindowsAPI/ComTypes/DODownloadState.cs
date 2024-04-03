namespace GetStoreApp.WindowsAPI.ComTypes
{
    /// <summary>
    /// DODownloadState 枚举指定当前下载状态的 ID，这是DO_DOWNLOAD_STATUS结构的一部分。
    /// </summary>
    public enum DODownloadState
    {
        /// <summary>
        /// 已创建下载对象，但尚未启动。
        /// </summary>
        DODownloadState_Created = 0,

        /// <summary>
        /// 正在下载。
        /// </summary>
        DODownloadState_Transferring = 1,

        /// <summary>
        /// 下载已转移，可以通过下载文件的另一部分来重新开始下载。
        /// </summary>
        DODownloadState_Transferred = 2,

        /// <summary>
        /// 下载已完成，无法再次启动。
        /// </summary>
        DODownloadState_Finalized = 3,

        /// <summary>
        /// 下载已中止。
        /// </summary>
        DODownloadState_Aborted = 4,

        /// <summary>
        /// 下载已按需暂停或由于暂时性错误。
        /// </summary>
        DODownloadState_Paused = 5
    }
}
