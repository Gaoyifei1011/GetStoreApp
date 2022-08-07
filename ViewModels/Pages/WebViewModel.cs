using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Web;
using GetStoreApp.Contracts.ViewModels;
using GetStoreApp.Helpers;
using Microsoft.Web.WebView2.Core;
using System;
using System.Windows.Input;

namespace GetStoreApp.ViewModels.Pages
{
    public class WebViewModel : ObservableRecipient, INavigationAware
    {
        private const string DefaultUrl = "https://store.rg-adguard.net/";

        private Uri _source;
        private bool _isLoading = true;
        private ICommand _browserBackCommand;
        private ICommand _browserForwardCommand;
        private ICommand _openInBrowserCommand;
        private ICommand _refreshCommand;
        private ICommand _retryCommand;

        public IWebViewService WebViewService { get; } = IOCHelper.GetService<IWebViewService>();

        public Uri Source
        {
            get { return _source; }
            set { SetProperty(ref _source, value); }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand BrowserBackCommand => _browserBackCommand ??= new RelayCommand(
            () => WebViewService?.GoBack(), () => WebViewService?.CanGoBack ?? false);

        public ICommand BrowserForwardCommand => _browserForwardCommand ??= new RelayCommand(
            () => WebViewService?.GoForward(), () => WebViewService?.CanGoForward ?? false);

        public ICommand RefreshCommand => _refreshCommand ??= new RelayCommand(
            () => WebViewService?.Reload());

        public ICommand RetryCommand => _retryCommand ??= new RelayCommand(OnRetry);

        public ICommand OpenInBrowserCommand => _openInBrowserCommand ??= new RelayCommand(async
            () => await Windows.System.Launcher.LaunchUriAsync(Source));

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
            OnPropertyChanged(nameof(BrowserBackCommand));
            OnPropertyChanged(nameof(BrowserForwardCommand));
        }

        private void OnRetry()
        {
            IsLoading = true;
            WebViewService?.Reload();
        }
    }
}
