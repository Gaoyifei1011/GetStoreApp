using GetStoreApp.Services.Controls.Settings.Appearance;
using Microsoft.UI.Xaml;
using System;
using Windows.System;

namespace GetStoreApp.ViewModels.Controls.Web
{
    /// <summary>
    /// 访问网页版页面：网页初始化失败用户控件视图模型
    /// </summary>
    public sealed class LoadFailedViewModel
    {
        /// <summary>
        /// 下载Webview2运行时
        /// </summary>
        public async void OnDownloadWebView2Clicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri(string.Format(@"https://developer.microsoft.com/{0}/microsoft-edge/webview2/", LanguageService.AppLanguage.InternalName)));
        }

        /// <summary>
        /// 使用浏览器打开网页
        /// </summary>
        public async void OnOpenWithBrowserClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://store.rg-adguard.net/"));
        }
    }
}
