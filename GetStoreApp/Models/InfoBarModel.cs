using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Models
{
    /// <summary>
    /// 应用信息状态栏数据模型
    /// </summary>
    internal sealed class InfoBarModel
    {
        /// <summary>
        /// 信息状态栏严重程度值
        /// </summary>
        internal InfoBarSeverity Severity { get; set; }

        /// <summary>
        /// 信息状态栏文字内容
        /// </summary>
        internal string Message { get; set; }

        /// <summary>
        /// 信息状态栏进度环显示值
        /// </summary>
        internal bool PrRingVisValue { get; set; }

        /// <summary>
        /// 信息状态栏进度环激活值
        /// </summary>
        internal bool PrRingActValue { get; set; }
    }
}
