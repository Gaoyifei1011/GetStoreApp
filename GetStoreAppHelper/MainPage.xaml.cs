using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GetStoreAppHelper
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs args)
        {
            (sender as Button).Content = "Clicked";
        }
    }
}
