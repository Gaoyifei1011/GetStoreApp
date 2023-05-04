using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Models.Controls.Store
{
    /// <summary>
    /// 已下载完成文件信息数据模型
    /// </summary>
    public class StatusBarStateModel
    {
        /// <summary>
        /// 信息状态栏严重程度值
        /// </summary>
        public InfoBarSeverity InfoBarSeverity { get; set; }

        /// <summary>
        /// 信息状态栏文字内容
        /// </summary>
        public string StateInfoText { get; set; }

        /// <summary>
        /// 信息状态栏进度环显示值
        /// </summary>
        public bool StatePrRingVisValue { get; set; }

        /// <summary>
        /// 信息状态栏进度环激活值
        /// </summary>
        public bool StatePrRingActValue { get; set; }
    }
}
