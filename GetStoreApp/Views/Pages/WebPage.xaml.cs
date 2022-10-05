using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Views.Pages
{
    public sealed partial class WebPage : Page
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public WebViewModel ViewModel { get; } = IOCHelper.GetService<WebViewModel>();

        public WebPage()
        {
            InitializeComponent();
            ViewModel.WebViewService.Initialize(WebView);
        }
    }
}
