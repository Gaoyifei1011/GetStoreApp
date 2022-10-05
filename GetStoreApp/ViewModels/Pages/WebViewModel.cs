using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Web;
using GetStoreApp.Contracts.ViewModels;
using GetStoreApp.Helpers;
using Microsoft.Web.WebView2.Core;
using System;
using System.Data;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Pages
{
    public class WebViewModel : ObservableRecipient, INavigationAware
    {
        private const string DefaultUrl = "https://store.rg-adguard.net/";

        public IWebViewService WebViewService { get; } = IOCHelper.GetService<IWebViewService>();

        public bool IsWebView2Exist => WebViewService.CheckEnvironment();

        private bool _canGoBack = false;

        public bool CanGoBack
        {
            get { return _canGoBack; }

            set { SetProperty(ref _canGoBack, value); }
        }

        private bool _canGoForward = false;

        public bool CanGoForward
        {
            get { return _canGoForward; }

            set { SetProperty(ref _canGoForward, value); }
        }

        private Uri _source;

        public Uri Source
        {
            get { return _source; }

            set { SetProperty(ref _source, value); }
        }

        private bool _isLoading = true;

        public bool IsLoading
        {
            get { return _isLoading; }

            set { SetProperty(ref _isLoading, value); }
        }

        // 网页后退
        public IAsyncRelayCommand BrowserBackCommand => new AsyncRelayCommand(
            async () =>
            {
                if (WebViewService.WebView.CanGoBack)
                {
                    WebViewService.GoBack();
                }
                else
                {
                    CanGoBack = WebViewService.WebView.CanGoBack;
                }
                await Task.CompletedTask;
            });

        // 网页前进
        public IAsyncRelayCommand BrowserForwardCommand => new AsyncRelayCommand(
            async () =>
            {
                if (WebViewService.WebView.CanGoForward)
                {
                    WebViewService.GoForward();
                }
                else
                {
                    CanGoForward = WebViewService.WebView.CanGoForward;
                }
                await Task.CompletedTask;
            });

        // 网页刷新
        public IAsyncRelayCommand RefreshCommand => new AsyncRelayCommand(async () =>
        {
            WebViewService.Reload();
            await Task.CompletedTask;
        });

        // 在浏览器中打开
        public IAsyncRelayCommand OpenWithBrowserCommand => new AsyncRelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(Source);
        });

        public void OnNavigatedTo(object parameter)
        {
            WebViewService.NavigationCompleted += OnNavigationCompleted;
            Source = new Uri(DefaultUrl);
        }

        public void OnNavigatedFrom()
        {
            WebViewService.UnregisterEvents();
            WebViewService.NavigationCompleted -= OnNavigationCompleted;
            
            
        }

        private void OnNavigationCompleted(object sender, CoreWebView2WebErrorStatus webErrorStatus)
        {
            IsLoading = false;
            CanGoBack = WebViewService.CanGoBack;
            CanGoForward = WebViewService.CanGoForward;
        }
    }
}
