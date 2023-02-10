using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace GetStoreApp.Helpers.Controls.Extensions
{
    /// <summary>
    /// 附加属性：设置指针位于控件上时显示的游标
    /// </summary>
    public static class CursorHelper
    {
        private static object locker = new object();
        private static Action<UIElement, InputCursor> setCursorFunc;
        private static Func<UIElement, InputCursor> getCursorFunc;

        public static InputSystemCursorShape? GetCursor(DependencyObject obj)
        {
            if (obj.GetValue(CursorOverrideStateProperty) is 0)
            {
                EnsureCursorFunctions();
                var cursor = GetFrameworkElementCursor((FrameworkElement)obj);

                obj.SetValue(CursorOverrideStateProperty, 1);

                var shape = cursor switch
                {
                    InputSystemCursor inputSystemCursor => (InputSystemCursorShape?)inputSystemCursor.CursorShape,
                    _ => null
                };

                SetCursor(obj, shape);

                return shape;
            }

            return (InputSystemCursorShape?)obj.GetValue(CursorProperty);
        }

        public static void SetCursor(DependencyObject obj, InputSystemCursorShape? value)
        {
            obj.SetValue(CursorProperty, value);
        }

        public static readonly DependencyProperty CursorProperty =
            DependencyProperty.RegisterAttached("Cursor", typeof(InputSystemCursorShape?), typeof(CursorHelper), new PropertyMetadata(InputSystemCursorShape.Arrow, (s, a) =>
            {
                if (!Equals(a.NewValue, a.OldValue) && s is FrameworkElement sender)
                {
                    EnsureCursorFunctions();

                    if (sender.GetValue(CursorOverrideStateProperty) is int state)
                    {
                        if (state == 0 || state == 1)
                        {
                            sender.SetValue(CursorOverrideStateProperty, 2);
                        }

                        if (state == 0 || state == 2)
                        {
                            var cursor = a.NewValue switch
                            {
                                InputSystemCursorShape shape => InputSystemCursor.Create(shape),
                                _ => null
                            };
                            SetFrameworkElementCursor(sender, cursor);
                        }
                    }
                }
            }));

        public static readonly DependencyProperty CursorOverrideStateProperty =
            DependencyProperty.RegisterAttached("CursorOverrideState", typeof(int), typeof(CursorHelper), new PropertyMetadata(0));

        private static void EnsureCursorFunctions()
        {
            if (getCursorFunc == null || setCursorFunc == null)
            {
                lock (locker)
                {
                    if (getCursorFunc == null || setCursorFunc == null)
                    {
                        var uiElementType = typeof(UIElement);
                        var inputCursorType = typeof(InputCursor);

                        var protectedCursorProp = uiElementType.GetProperty("ProtectedCursor", BindingFlags.Instance | BindingFlags.NonPublic);

                        var p1 = Expression.Parameter(uiElementType, "element");

                        if (getCursorFunc == null)
                        {
                            var body = Expression.Property(p1, protectedCursorProp);

                            getCursorFunc = Expression.Lambda<Func<UIElement, InputCursor>>(body, p1)
                                .Compile();
                        }

                        if (setCursorFunc == null)
                        {
                            var p2 = Expression.Parameter(inputCursorType, "cursor");

                            var body = Expression.Assign(Expression.Property(p1, protectedCursorProp), p2);

                            setCursorFunc = Expression.Lambda<Action<UIElement, InputCursor>>(body, p1, p2)
                                .Compile();
                        }
                    }
                }
            }
        }

        private static void SetFrameworkElementCursor(FrameworkElement element, InputCursor cursor)
        {
            EnsureCursorFunctions();

            if (element.IsLoaded)
            {
                setCursorFunc(element, cursor);
            }
            else
            {
                element.Loaded += OnLoaded;
            }

            void OnLoaded(object sender, RoutedEventArgs e)
            {
                element.Loaded -= OnLoaded;
                setCursorFunc(element, cursor);
            }
        }

        private static InputCursor GetFrameworkElementCursor(FrameworkElement element)
        {
            EnsureCursorFunctions();

            return getCursorFunc(element);
        }
    }
}
