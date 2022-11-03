using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Helpers.Root;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GetStoreApp.Views.Pages
{
    public sealed partial class WebPage : Page
    {
        public IResourceService ResourceService { get; } = ContainerHelper.GetInstance<IResourceService>();

        public WebViewModel ViewModel { get; } = ContainerHelper.GetInstance<WebViewModel>();

        public WebPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 关闭浏览器内核
        /// </summary>
        private void WebPageUnloaded(object sender, RoutedEventArgs e)
        {
            if (WebView is not null)
            {
                WebView.Close();
            }
        }
    }
}
