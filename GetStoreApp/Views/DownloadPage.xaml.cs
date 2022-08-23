using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Views
{
    public sealed partial class DownloadPage : Page
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public DownloadViewModel ViewModel { get; } = IOCHelper.GetService<DownloadViewModel>();

        public DownloadPage()
        {
            InitializeComponent();
        }
    }
}
