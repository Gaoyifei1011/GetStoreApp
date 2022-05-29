namespace GetStoreApp.Models
{
    /// <summary>
    /// 主页面通道选择的数据模型
    /// The data model selected channel by the home page channel
    /// </summary>
    public class HomeChannelModel
    {
        /// <summary>
        /// ComboBox列表中显示的条目信息
        /// The entry information displayed in the ComboBox list
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// ComboBox在选择显示的条目后对应内部设定的值
        /// ComboBox corresponds to the value set internally after selecting the displayed entry
        /// </summary>
        public string InternalName { get; set; }

        public HomeChannelModel(string displayName, string internalName)
        {
            DisplayName = displayName;
            InternalName = internalName;
        }
    }
}
