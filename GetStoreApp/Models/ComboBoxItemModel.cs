using WinRT;

namespace GetStoreApp.Models
{
    /// <summary>
    /// 下拉框数据模型
    /// </summary>
    [GeneratedBindableCustomProperty]
    public partial class ComboBoxItemModel
    {
        /// <summary>
        /// 选中值
        /// </summary>
        public object SelectedValue { get; set; }

        /// <summary>
        /// 显示值
        /// </summary>
        public string DisplayMember { get; set; }
    }
}
