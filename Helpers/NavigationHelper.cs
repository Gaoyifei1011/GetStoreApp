using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Helpers
{
    /// <summary>
    /// 应用导航切换
    /// </summary>
    public class NavigationHelper
    {
        public static string GetNavigateTo(NavigationViewItem item)
        {
            return (string)item.GetValue(NavigateToProperty);
        }

        public static void SetNavigateTo(NavigationViewItem item, string value)
        {
            item.SetValue(NavigateToProperty, value);
        }

        public static readonly DependencyProperty NavigateToProperty =
            DependencyProperty.RegisterAttached("NavigateTo", typeof(string), typeof(NavigationHelper), new PropertyMetadata(null));
    }
}
