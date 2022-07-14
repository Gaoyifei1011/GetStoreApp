using GetStoreApp.Contracts.Services.App;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Views
{
    // To learn more about WebView2, see https://docs.microsoft.com/microsoft-edge/webview2/
    public sealed partial class WebPage : Page
    {
        public IResourceService ResourceService { get; }

        public WebViewModel ViewModel { get; }

        public WebPage()
        {
            ResourceService = App.GetService<IResourceService>();
            ViewModel = App.GetService<WebViewModel>();
            InitializeComponent();
            ViewModel.WebViewService.Initialize(webView);
        }
    }
}
