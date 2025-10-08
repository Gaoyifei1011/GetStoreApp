namespace GetStoreApp.Extensions.DataType.Enums
{
    /// <summary>
    /// 应用更新数据类型
    /// </summary>
    public enum UpdateAppResultKind
    {
        Initialize = 0,
        Pending = 1,
        Downloading = 2,
        Canceling = 3,
        Canceled = 4,
        Failed = 5,
        Deploying = 6,
        Successfully = 7
    }
}
