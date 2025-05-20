namespace GetStoreApp.Extensions.DataType.Enums
{
    /// <summary>
    /// 下载进度状态
    /// </summary>
    public enum DownloadProgressState
    {
        Queued = 0,
        Downloading = 1,
        Paused = 2,
        Failed = 3,
        Finished = 4,
        Deleted = 5,
    }
}
