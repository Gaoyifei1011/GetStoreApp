namespace GetStoreApp.Models.Controls.Home
{
    /// <summary>
    /// 通道类型数据模型
    /// </summary>
    public class ChannelModel
    {
        /// <summary>
        /// 获取应用通道显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 获取应用通道内部名称
        /// </summary>
        public string InternalName { get; set; }

        /// <summary>
        /// 获取应用通道简短名称（用作参数启动使用）
        /// </summary>
        public string ShortName { get; set; }
    }
}
