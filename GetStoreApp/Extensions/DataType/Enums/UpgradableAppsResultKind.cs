namespace GetStoreApp.Extensions.DataType.Enums
{
    /// <summary>
    /// 可更新应用结果类型
    /// </summary>
    internal enum UpgradableAppsResultKind
    {
        NotCheckUpdate = 0,
        Querying = 1,
        Failed = 2,
        HasResult = 3,
        AllUpdateToDate = 4
    }
}
