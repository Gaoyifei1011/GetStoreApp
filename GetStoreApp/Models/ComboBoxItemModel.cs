using WinRT;

namespace GetStoreApp.Models
{
    /// <summary>
    /// 下拉框数据模型
    /// </summary>
    [GeneratedBindableCustomProperty]
    internal sealed partial class ComboBoxItemModel
    {
        /// <summary>
        /// 选中值
        /// </summary>
        internal object SelectedValue { get; set; }

        /// <summary>
        /// 显示值
        /// </summary>
        internal string DisplayMember { get; set; }
    }
}
