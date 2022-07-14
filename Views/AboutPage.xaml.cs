using GetStoreApp.Contracts.Services.App;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Views
{
    public sealed partial class AboutPage : Page
    {
        public IResourceService ResourceService { get; }

        public AboutViewModel ViewModel { get; }

        public AboutPage()
        {
            ResourceService = App.GetService<IResourceService>();
            ViewModel = App.GetService<AboutViewModel>();
            InitializeComponent();
        }
    }
}
