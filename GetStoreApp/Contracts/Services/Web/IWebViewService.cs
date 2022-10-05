using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;

namespace GetStoreApp.Contracts.Services.Web
{
    public interface IWebViewService
    {
        event EventHandler<CoreWebView2WebErrorStatus> NavigationCompleted;

        WebView2 WebView { get; }

        bool CanGoBack { get; }

        bool CanGoForward { get; }

        void Initialize(WebView2 webView);

        bool CheckEnvironment();

        void UnregisterEvents();

        void GoBack();

        void GoForward();

        void Reload();
    }
}
