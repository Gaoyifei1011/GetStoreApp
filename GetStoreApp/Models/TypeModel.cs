namespace GetStoreApp.Models
{
    /// <summary>
    /// 应用类型数据模型
    /// </summary>
    public sealed class TypeModel
    {
        /// <summary>
        /// 获取应用类型显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 获取应用类型内部名称
        /// </summary>
        public string InternalName { get; set; }

        /// <summary>
        /// 获取应用类型简短名称（用作参数启动使用）
        /// </summary>
        public string ShortName { get; set; }
    }
}
