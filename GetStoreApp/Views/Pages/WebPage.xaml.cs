using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 访问网页版页面
    /// </summary>
    public sealed partial class WebPage : Page
    {
        public WebPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs args)
        {
            base.OnNavigatingFrom(args);
            ViewModel.OnNavigatedFrom();
        }

        /// <summary>
        /// 关闭浏览器内核
        /// </summary>
        private void WebPageUnloaded(object sender, RoutedEventArgs args)
        {
            WebView?.Close();
        }
    }
}
