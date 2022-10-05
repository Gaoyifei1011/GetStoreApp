using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace GetStoreApp.ViewModels.Controls.Web
{
    public class LoadFailedViewModel : ObservableRecipient
    {
        public IAsyncRelayCommand DownloadWebView2Command => new AsyncRelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://developer.microsoft.com/zh-cn/microsoft-edge/webview2/"));
        });

        public IAsyncRelayCommand OpenWithBrowserCommand => new AsyncRelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://store.rg-adguard.net/"));
        });
    }
}
