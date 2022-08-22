using GetStoreApp.Contracts.Services.Web;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;

namespace GetStoreApp.Services.Web
{
    /// <summary>
    /// 网页浏览服务
    /// </summary>
    public class WebViewService : IWebViewService
    {
        private WebView2 WebView { get; set; }

        public bool CanGoBack => WebView.CanGoBack;

        public bool CanGoForward => WebView.CanGoForward;

        public event EventHandler<CoreWebView2WebErrorStatus> NavigationCompleted;

        public void Initialize(WebView2 webView)
        {
            WebView = webView;
            WebView.NavigationCompleted += OnWebViewNavigationCompleted;
        }

        public void UnregisterEvents()
        {
            WebView.NavigationCompleted -= OnWebViewNavigationCompleted;
        }

        private void OnWebViewNavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
        {
            NavigationCompleted?.Invoke(this, args.WebErrorStatus);
        }

        public void GoBack()
        {
            WebView.GoBack();
        }

        public void GoForward()
        {
            WebView.GoForward();
        }

        public void Reload()
        {
            WebView.Reload();
        }
    }
}
