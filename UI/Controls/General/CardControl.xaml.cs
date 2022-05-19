using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace GetStoreApp.UI.Controls.General
{
    [ContentProperty(Name = "MainContent")]
    public sealed partial class CardControl : UserControl
    {
        public CardControl()
        {
            this.InitializeComponent();
        }

        public static DependencyProperty MainContentProperty =
            DependencyProperty.Register("MainContent", typeof(object), typeof(CardControl), null);

        public object MainContent
        {
            get => GetValue(MainContentProperty);
            set => SetValue(MainContentProperty, value);
        }
    }
}
