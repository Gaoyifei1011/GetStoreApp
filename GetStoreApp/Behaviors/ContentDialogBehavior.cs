using CommunityToolkit.WinUI.UI.Behaviors;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;

namespace GetStoreApp.Behaviors
{
    /// <summary>
    /// 让内容对话框的烟雾层背景（SmokeLayerBackground）覆盖到标题栏中
    /// </summary>
    public class ContentDialogBehavior : BehaviorBase<FrameworkElement>
    {
        protected override void OnAssociatedObjectLoaded()
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

        private static void OnMarginChanged(DependencyObject sender, DependencyProperty property)
        {
            if (property == FrameworkElement.MarginProperty)
            {
                sender.ClearValue(property);
            }
        }
    }
}
