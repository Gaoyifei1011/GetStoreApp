namespace GetStoreApp.Models
{
    public class LanguageData
    {
        /// <summary>
        /// 设置页面显示的语言名称
        /// Settings page displays the language name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 设置页面显示的语言名称对应内部的语言编码
        /// The language name displayed on the settings page corresponds to the internal language encoding
        /// </summary>
        public string CodeName { get; set; }
    }
}
