using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GetStoreApp.UI.Controls.General
{
    [ContentProperty(Name = "CardContent")]
    public sealed partial class CardControl : UserControl
    {
        public CardControl()
        {
            this.InitializeComponent();
        }

        public object CardContent
        {
            get => GetValue(MainContentProperty);
            set => SetValue(MainContentProperty, value);
        }

        public static DependencyProperty MainContentProperty = DependencyProperty.Register("CardContent", typeof(object), typeof(CardControl), null);
    }
}
