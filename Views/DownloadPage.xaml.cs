using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Views
{
    public sealed partial class DownloadPage : Page
    {
        public DownloadViewModel ViewModel { get; set; }

        public DownloadPage()
        {
            ViewModel = App.GetService<DownloadViewModel>();
            InitializeComponent();
        }
    }
}
