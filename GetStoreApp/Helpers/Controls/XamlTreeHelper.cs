using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using System.Runtime.InteropServices.Marshalling;
using WinRT;

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

                    try
                    {
                        if (child is T result && (childName is null || child.As<FrameworkElement>() is FrameworkElement frameworkElement && string.Equals(frameworkElement.Name, childName)))
                        {
                            return result;
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionAsVoidMarshaller.ConvertToUnmanaged(e);
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
