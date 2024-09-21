using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GetStoreAppInstaller
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
            => this.InitializeComponent();

        private void ClickHandler(object sender, RoutedEventArgs _)
        {
            (sender as Button).Content = "Clicked";
        }
    }
}
