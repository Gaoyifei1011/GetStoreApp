namespace GetStoreApp.Models
{
    /// <summary>
    /// 应用启动参数数据模型
    /// </summary>
    internal sealed class StartupArgsModel
    {
        /// <summary>
        /// 应用启动参数名称
        /// </summary>
        internal string ArgumentName { get; set; }

        /// <summary>
        /// 具体的应用启动参数
        /// </summary>
        internal string Argument { get; set; }

        /// <summary>
        /// 应用启动参数是否必需要输入
        /// </summary>
        internal string IsRequired { get; set; }

        /// <summary>
        /// 应用启动参数具体内容
        /// </summary>
        internal string ArgumentContent { get; set; }
    }
}
