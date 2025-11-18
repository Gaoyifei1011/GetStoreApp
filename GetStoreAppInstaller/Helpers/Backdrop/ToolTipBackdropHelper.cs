using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using WinRT;

// 抑制 IDE0060 警告
#pragma warning disable IDE0060

namespace GetStoreAppInstaller.Helpers.Backdrop
{
    /// <summary>
    /// 工具提示背景色辅助类
    /// </summary>
    public static class ToolTipBackdropHelper
    {
        /// <summary>
        /// 工具提示加载完成后触发的事件
        /// </summary>
        public static void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (sender.As<ToolTip>() is ToolTip toolTip && toolTip.Parent.As<Popup>() is Popup popup)
            {
                popup.RequestedTheme = toolTip.RequestedTheme;
                popup.SystemBackdrop ??= Application.Current.Resources["AcrylicBackgroundFillColorDefaultBackdrop"].As<SystemBackdrop>();
            }
        }
    }
}
