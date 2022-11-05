using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.Xaml.Interactivity;

namespace GetStoreApp.Behaviors
{
    /// <summary>
    /// 让内容对话框的烟雾层背景（SmokeLayerBackground）覆盖到标题栏中
    /// </summary>
    public class ContentDialogBehavior : Behavior<ContentDialog>
    {
        /// <summary>
        /// 关联事件处理程序，通过AssociatedObject属性访问放置行为的元素
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            if (AssociatedObject != null)
            {
                AssociatedObject.Loaded += OnAssociatedObjectLoaded;
            }
        }

        /// <summary>
        /// 移除事件处理程序
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (AssociatedObject != null)
            {
                AssociatedObject.Loaded -= OnAssociatedObjectLoaded;
            }
        }

        /// <summary>
        /// 重置内容对话框烟雾背景距离顶栏的间隔
        /// </summary>
        private void OnAssociatedObjectLoaded(object sender, RoutedEventArgs args)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(AssociatedObject);

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject current = VisualTreeHelper.GetChild(parent, i);
                if (current is Rectangle { Name: "SmokeLayerBackground" } background)
                {
                    background.Margin = new Thickness(0);
                    background.RegisterPropertyChangedCallback(FrameworkElement.MarginProperty, OnMarginChanged);
                    break;
                }
            }
        }

        private void OnMarginChanged(DependencyObject sender, DependencyProperty property)
        {
            if (property == FrameworkElement.MarginProperty)
            {
                sender.ClearValue(property);
            }
        }
    }
}
