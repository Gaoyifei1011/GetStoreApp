using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace GetStoreApp.Views.Pages
{
    public sealed partial class DownloadPage : Page
    {
        public DownloadPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.OnNavigatedTo();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            ViewModel.OnNavigatedFrom();
        }

        public void TeachingTipTapped(object sender, RoutedEventArgs args)
        {
            DownloadTeachingTip.IsOpen = true;
        }
    }
}
