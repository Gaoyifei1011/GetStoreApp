using GetStoreApp.Contracts.Services.App;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Views
{
    public sealed partial class WebPage : Page
    {
        public IResourceService ResourceService { get; }

        public WebViewModel ViewModel { get; }

        public WebPage()
        {
            ResourceService = IOCHelper.GetService<IResourceService>();
            ViewModel = IOCHelper.GetService<WebViewModel>();
            InitializeComponent();
            ViewModel.WebViewService.Initialize(webView);
        }
    }
}
