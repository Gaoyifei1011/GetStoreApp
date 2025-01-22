using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreAppInstaller.Helpers.Controls.Extensions
{
    /// <summary>
    /// 扩展后的教学提示控件辅助类
    /// </summary>
    public static class TeachingTipHelper
    {
        /// <summary>
        /// 使用教学提示显示应用内通知
        /// </summary>
        public static async Task ShowAsync(TeachingTip teachingTip, int duration = 2000)
        {
            teachingTip.Name = "TeachingTip" + Guid.NewGuid().ToString();

            ((Window.Current.Content as Page).Content as Grid).Children.Add(teachingTip);

            teachingTip.IsOpen = true;
            teachingTip.Closed += (sender, args) =>
            {
                try
                {
                    foreach (UIElement uiElement in ((Window.Current.Content as Page).Content as Grid).Children)
                    {
                        if ((uiElement as FrameworkElement).Name.Equals(teachingTip.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            ((Window.Current.Content as Page).Content as Grid).Children.Remove(uiElement);
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
                }
            };
            await Task.Delay(duration);
            teachingTip.IsOpen = false;
        }
    }
}
