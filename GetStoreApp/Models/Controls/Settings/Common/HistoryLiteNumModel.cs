using GetStoreApp.Models.Base;

namespace GetStoreApp.Models.Controls.Settings.Common
{
    public class HistoryLiteNumModel : ModelBase
    {
        /// <summary>
        /// 主页面历史记录显示数量设置显示名称
        /// </summary>
        public string HistoryLiteNumName { get; set; }

        /// <summary>
        /// 主页面历史记录显示数量设置内部名称
        /// </summary>
        public int HistoryLiteNumValue { get; set; }
    }
}
