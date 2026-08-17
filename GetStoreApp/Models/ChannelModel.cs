using WinRT;

namespace GetStoreApp.Models
{
    /// <summary>
    /// 通道类型数据模型
    /// </summary>
    [GeneratedBindableCustomProperty]
    internal sealed partial class ChannelModel
    {
        /// <summary>
        /// 获取应用通道显示名称
        /// </summary>
        internal string DisplayName { get; set; }

        /// <summary>
        /// 获取应用通道内部名称
        /// </summary>
        internal string InternalName { get; set; }

        /// <summary>
        /// 获取应用通道简短名称（用作参数启动使用）
        /// </summary>
        internal string ShortName { get; set; }
    }
}
