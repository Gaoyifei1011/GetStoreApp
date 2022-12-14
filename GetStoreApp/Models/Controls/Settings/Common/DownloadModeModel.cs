using GetStoreApp.Models.Base;

namespace GetStoreApp.Models.Controls.Settings.Common
{
    public class DownloadModeModel : ModelBase
    {
        /// <summary>
        /// 下载模式设置显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 下载模式设置内部名称
        /// </summary>
        public string InternalName { get; set; }
    }
}
