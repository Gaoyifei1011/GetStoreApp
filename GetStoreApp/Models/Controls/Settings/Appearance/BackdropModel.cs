using GetStoreApp.Models.Base;

namespace GetStoreApp.Models.Controls.Settings.Appearance
{
    public class BackdropModel : ModelBase
    {
        /// <summary>
        /// 应用背景色设置显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 应用背景色设置内部名称
        /// </summary>
        public string InternalName { get; set; }
    }
}
