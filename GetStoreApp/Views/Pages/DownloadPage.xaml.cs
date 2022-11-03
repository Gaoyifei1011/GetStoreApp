using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Views.Pages
{
    public sealed partial class DownloadPage : Page
    {
        public IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public DownloadViewModel ViewModel { get; } = ContainerHelper.GetInstance<DownloadViewModel>();

        public DownloadPage()
        {
            InitializeComponent();
        }

        public void TeachingTipTapped(object sender, RoutedEventArgs args)
        {
            DownloadTeachingTip.IsOpen = true;
        }
    }
}
