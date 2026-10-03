using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Models
{
    /// <summary>
    /// 单选按钮数据模型
    /// </summary>
    internal class RadioButtonItemModel
    {
        /// <summary>
        /// 选择栏项图标
        /// </summary>
        internal IconElement SelectorIcon { get; set; }

        /// <summary>
        /// 选择栏项名称
        /// </summary>
        internal string SelectorName { get; set; }
    }
}
