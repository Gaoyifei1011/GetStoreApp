using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace GetStoreApp.Views.Pages
{
    public sealed partial class WebPage : Page
    {
        public WebPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            ViewModel.OnNavigatedFrom();
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
