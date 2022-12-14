using GetStoreApp.Models.Base;

namespace GetStoreApp.Models.Controls.Settings.Appearance
{
    public class ThemeModel : ModelBase
    {
        /// <summary>
        /// 应用主题设置显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 应用主题设置内部名称
        /// </summary>
        public string InternalName { get; set; }
    }
}
