namespace GetStoreAppHelper.Extensions.DataType.Struct
{
    /// <summary>
    /// 系统版本信息
    /// </summary>
    public struct SystemVersion
    {
        // 主版本号
        public ulong MajorVersion { get; set; }

        // 次版本号
        public ulong MinorVersion { get; set; }

        // 构建版本号
        public ulong BuildNumber { get; set; }

        // 修订版本号
        public ulong BuildRevision { get; set; }
    }
}
