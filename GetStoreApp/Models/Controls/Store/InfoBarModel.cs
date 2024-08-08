using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Models.Controls.Store
{
    /// <summary>
    /// 应用信息状态栏数据模型
    /// </summary>
    public sealed class InfoBarModel
    {
        /// <summary>
        /// 信息状态栏严重程度值
        /// </summary>
        public InfoBarSeverity Severity { get; set; }

        /// <summary>
        /// 信息状态栏文字内容
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 信息状态栏进度环显示值
        /// </summary>
        public bool PrRingVisValue { get; set; }

        /// <summary>
        /// 信息状态栏进度环激活值
        /// </summary>
        public bool PrRingActValue { get; set; }
    }
}
