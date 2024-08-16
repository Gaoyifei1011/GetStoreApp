using Microsoft.UI.Xaml;

namespace GetStoreApp.Helpers.Controls.Extensions
{
    /// <summary>
    /// 扩展后的切换开关控件辅助类
    /// </summary>
    public static class ToggleSwitchHelper
    {
        /// <summary>
        /// 获取 ToggleSwitch 的文字转向
        /// </summary>
        public static FlowDirection GetFlowDirection(FlowDirection flowDirection)
        {
            return flowDirection is FlowDirection.LeftToRight ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
        }
    }
}
