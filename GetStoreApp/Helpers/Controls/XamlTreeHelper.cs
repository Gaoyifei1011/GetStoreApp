using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace GetStoreApp.Helpers.Controls
{
    /// <summary>
    /// Xaml 可视化树辅助类
    /// </summary>
    public static class XamlTreeHelper
    {
        /// <summary>
        /// 寻找当前控件的子控件
        /// </summary>
        public static T FindDescendant<T>(DependencyObject parent, string childName = null) where T : DependencyObject
        {
            if (parent is not null)
            {
                for (int index = 0; index < VisualTreeHelper.GetChildrenCount(parent); index++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(parent, index);

                    if (child is T result && (childName is null || child is FrameworkElement frameworkElement && string.Equals(frameworkElement.Name, childName)))
                    {
                        return result;
                    }

                    T foundChild = FindDescendant<T>(child, childName);
                    if (foundChild is not null)
                    {
                        return foundChild;
                    }
                }
            }

            return null;
        }
    }
}
