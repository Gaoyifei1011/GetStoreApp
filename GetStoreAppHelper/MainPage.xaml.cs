using System;
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

        private async void Button_Click(object sender, RoutedEventArgs args)
        {
            (sender as Button).Content = "Clicked";

            ContentDialog dialog = new ContentDialog();

            // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
            dialog.XamlRoot = XamlRoot;
            dialog.Title = "Save your work?";
            dialog.PrimaryButtonText = "Save";
            dialog.SecondaryButtonText = "Don't Save";
            dialog.CloseButtonText = "Cancel";
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.Content = "Test String";

            await dialog.ShowAsync();
        }
    }
}
