using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Views.Controls
{
    /// <summary>
    /// 修改后的DropButton按钮，拥有HyperlinkButton的样式
    /// </summary>
    public partial class HyperlinkDPButton : DropDownButton
    {
        public HyperlinkDPButton()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        ~HyperlinkDPButton()
        {
            Loaded -= OnLoaded;
        }

        public InputSystemCursorShape Cursor
        {
            get { return (InputSystemCursorShape)GetValue(CursorProperty); }
            set { SetValue(CursorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Cursor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CursorProperty =
            DependencyProperty.Register("Cursor", typeof(InputSystemCursorShape), typeof(HyperlinkDPButton), new PropertyMetadata(InputSystemCursorShape.Arrow));

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            ProtectedCursor = InputSystemCursor.Create(Cursor);
        }
    }
}
