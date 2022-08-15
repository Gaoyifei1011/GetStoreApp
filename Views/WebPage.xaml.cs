using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Views
{
    public sealed partial class WebPage : Page
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public WebViewModel ViewModel { get; } = IOCHelper.GetService<WebViewModel>();

        public WebPage()
        {
            InitializeComponent();

            ViewModel.WebViewService.Initialize(webView);
        }
    }
}
