using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace GetStoreApp.Views.CustomControls.BasicInput
{
    /// <summary>
    /// 扩展后的按钮，可以设置指针位于控件上时显示的游标
    /// </summary>
    public sealed partial class ExtendedButton : Button
    {
        public ExtendedButton()
        {
            InitializeComponent();
        }

        public InputSystemCursorShape Cursor
        {
            get { return (InputSystemCursorShape)GetValue(CursorProperty); }
            set { SetValue(CursorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Cursor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CursorProperty =
            DependencyProperty.Register("Cursor", typeof(InputSystemCursorShape), typeof(ExtendedDropDownButton), new PropertyMetadata(InputSystemCursorShape.Arrow));

        protected override void OnPointerEntered(PointerRoutedEventArgs args)
        {
            base.OnPointerEntered(args);

            if (ProtectedCursor == null)
            {
                ProtectedCursor = InputSystemCursor.Create(Cursor);
            }
            else
            {
                if ((ProtectedCursor as InputSystemCursor).CursorShape != Cursor)
                {
                    ProtectedCursor = InputSystemCursor.Create(Cursor);
                }
            }
        }
    }
}
