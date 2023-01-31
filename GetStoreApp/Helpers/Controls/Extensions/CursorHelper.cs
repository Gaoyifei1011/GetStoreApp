using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using System.Reflection;

namespace GetStoreApp.Helpers.Controls.Extensions
{
    /// <summary>
    /// 附加属性：设置指针位于控件上时显示的游标
    /// </summary>
    public class CursorHelper
    {
        public static InputSystemCursorShape GetCursor(DependencyObject obj)
        {
            return (InputSystemCursorShape)obj.GetValue(CursorProperty);
        }

        public static void SetCursor(DependencyObject obj, InputSystemCursorShape value)
        {
            obj.SetValue(CursorProperty, value);
        }

        // Using a DependencyProperty as the backing store for CursorProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CursorProperty =
            DependencyProperty.RegisterAttached("Cursor", typeof(InputSystemCursorShape), typeof(CursorHelper), new PropertyMetadata(InputSystemCursorShape.Arrow, OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            FrameworkElement element = d as FrameworkElement;
            if (element is not null)
            {
                element.Loaded += (sender, e) =>
                {
                    FrameworkElement element = sender as FrameworkElement;
                    typeof(FrameworkElement).GetProperty("ProtectedCursor", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(element, InputSystemCursor.Create((InputSystemCursorShape)args.NewValue));
                };
            }
        }
    }
}
