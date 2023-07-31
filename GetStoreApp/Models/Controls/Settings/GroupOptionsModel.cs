namespace GetStoreApp.Models.Controls.Settings
{
    /// <summary>
    /// 组合选项数据模型
    /// </summary>
    public class GroupOptionsModel
    {
        /// <summary>
        /// 选项显示名称
        /// </summary>
        public string DisplayMember { get; set; }

        /// <summary>
        /// 选项内部对应值
        /// </summary>
        public string SelectedValue { get; set; }
    }
}
