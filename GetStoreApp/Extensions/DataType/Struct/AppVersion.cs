namespace GetStoreApp.Extensions.DataType.Struct
{
    /// <summary>
    /// 应用版本信息
    /// </summary>
    public struct AppVersion
    {
        // 主版本号
        public ushort MajorVersion { get; set; }

        // 次版本号
        public ushort MinorVersion { get; set; }

        // 构建版本号
        public ushort BuildVersion { get; set; }

        // 修订版本号
        public ushort RevisionVersion { get; set; }
    }
}
