using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using System;

namespace GetStoreApp.ViewModels.Controls.Web
{
    /// <summary>
    /// 访问网页版页面：网页初始化失败用户控件视图模型
    /// </summary>
    public sealed class LoadFailedViewModel
    {
        // 下载Webview2运行时
        public IRelayCommand DownloadWebView2Command => new RelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://developer.microsoft.com/zh-cn/microsoft-edge/webview2/"));
        });

        // 使用浏览器打开网页
        public IRelayCommand OpenWithBrowserCommand => new RelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://store.rg-adguard.net/"));
        });
    }
}
