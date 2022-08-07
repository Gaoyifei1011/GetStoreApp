using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
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
            ResourceService = IOCHelper.GetService<IResourceService>();
            ViewModel = IOCHelper.GetService<AboutViewModel>();
            InitializeComponent();
        }
    }
}
