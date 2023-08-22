using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using System.Reflection;

namespace GetStoreApp.Helpers.Controls.Extensions
{
    /// <summary>
    /// 附加属性：设置指针位于控件上时显示的游标
    /// </summary>
    public static class CursorHelper
    {
        public static InputSystemCursorShape GetCursor(DependencyObject obj)
        {
            return (InputSystemCursorShape)obj.GetValue(CursorProperty);
        }

        public static void SetCursor(DependencyObject obj, InputSystemCursorShape value)
        {
            obj.SetValue(CursorProperty, value);
        }

        public static readonly DependencyProperty CursorProperty =
            DependencyProperty.RegisterAttached("Cursor", typeof(InputSystemCursorShape), typeof(CursorHelper), new PropertyMetadata(InputSystemCursorShape.Arrow, OnCursorChanged));

        public static void OnCursorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is FrameworkElement element)
            {
                element.Loaded += (sender, args) =>
                {
                    typeof(FrameworkElement).GetProperty("ProtectedCursor", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(element, InputSystemCursor.Create(GetCursor(sender as FrameworkElement)));
                };
            }
        }
    }
}
