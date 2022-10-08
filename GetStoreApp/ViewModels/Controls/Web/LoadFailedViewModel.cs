using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace GetStoreApp.ViewModels.Controls.Web
{
    public class LoadFailedViewModel : ObservableRecipient
    {
        public IRelayCommand DownloadWebView2Command => new RelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://developer.microsoft.com/zh-cn/microsoft-edge/webview2/"));
        });

        public IRelayCommand OpenWithBrowserCommand => new RelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://store.rg-adguard.net/"));
        });
    }
}
