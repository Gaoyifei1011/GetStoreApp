using GetStoreApp.Views.Controls;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.Xaml.Interactivity;

namespace GetStoreApp.Behaviors
{
    /// <summary>
    /// 修改主页面ResultControl中下拉按钮的鼠标指针样式
    /// </summary>
    public class ResultDropDownButtonBehavior : Behavior<ResultDropDownButton>
    {
        /// <summary>
        /// 关联事件处理程序，通过AssociatedObject属性访问放置行为的元素
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += OnAssociatedObjectLoaded;
        }

        /// <summary>
        /// 移除事件处理程序
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= OnAssociatedObjectLoaded;
        }

        /// <summary>
        /// 修改ResultControl下拉按钮的鼠标指针样式为点击
        /// </summary>
        private void OnAssociatedObjectLoaded(object sender, RoutedEventArgs e)
        {
            ResultDropDownButton customButton = sender as ResultDropDownButton;
            customButton.SetCursor(InputSystemCursor.Create(InputSystemCursorShape.Hand));
        }
    }
}
