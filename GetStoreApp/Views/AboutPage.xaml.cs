using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Views
{
    public sealed partial class AboutPage : Page
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public AboutViewModel ViewModel { get; } = IOCHelper.GetService<AboutViewModel>();

        public AboutPage()
        {
            InitializeComponent();
        }
    }
}
