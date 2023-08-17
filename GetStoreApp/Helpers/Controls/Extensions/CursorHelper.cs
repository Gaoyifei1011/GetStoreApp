using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace GetStoreApp.Helpers.Controls.Extensions
{
    /// <summary>
    /// 附加属性：设置指针位于控件上时显示的游标
    /// </summary>
    public static class CursorHelper
    {
        private static readonly object _cursorLock = new object();
        private static readonly InputCursor _defaultCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);

        private static readonly Dictionary<InputSystemCursorShape, InputCursor> _cursors =
            new Dictionary<InputSystemCursorShape, InputCursor> { { InputSystemCursorShape.Arrow, _defaultCursor } };

        public static readonly DependencyProperty CursorProperty =
            DependencyProperty.RegisterAttached("Cursor", typeof(InputSystemCursorShape), typeof(CursorHelper), new PropertyMetadata(InputSystemCursorShape.Arrow, CursorChanged));

        public static void SetCursor(FrameworkElement element, InputSystemCursorShape value)
        {
            element.SetValue(CursorProperty, value);
        }

        public static InputSystemCursorShape GetCursor(FrameworkElement element)
        {
            return (InputSystemCursorShape)element.GetValue(CursorProperty);
        }

        private static void CursorChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (d is not FrameworkElement element)
            {
                throw new NullReferenceException(nameof(element));
            }

            InputSystemCursorShape value = (InputSystemCursorShape)args.NewValue;

            lock (_cursorLock)
            {
                if (!_cursors.ContainsKey(value))
                {
                    _cursors[value] = InputSystemCursor.Create(value);
                }

                element.PointerEntered -= Element_PointerEntered;
                element.PointerEntered += Element_PointerEntered;
                element.PointerExited -= Element_PointerExited;
                element.PointerExited += Element_PointerExited;
                element.Unloaded -= ElementOnUnloaded;
                element.Unloaded += ElementOnUnloaded;
            }
        }

        private static void Element_PointerEntered(object sender, PointerRoutedEventArgs args)
        {
            InputSystemCursorShape cursor = GetCursor(sender as FrameworkElement);
            FrameworkElement element = sender as FrameworkElement;
            typeof(FrameworkElement).GetProperty("ProtectedCursor", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(element, InputSystemCursor.Create(cursor));
        }

        private static void Element_PointerExited(object sender, PointerRoutedEventArgs args)
        {
            InputCursor cursor;
            if (sender != args.OriginalSource && args.OriginalSource is FrameworkElement newElement)
            {
                cursor = _cursors[GetCursor(newElement)];
            }
            else
            {
                cursor = _defaultCursor;
            }

            FrameworkElement element = sender as FrameworkElement;
            typeof(FrameworkElement).GetProperty("ProtectedCursor", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(element, cursor);
        }

        private static void ElementOnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            FrameworkElement element = sender as FrameworkElement;
            typeof(FrameworkElement).GetProperty("ProtectedCursor", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(element, _defaultCursor);
        }
    }
}
