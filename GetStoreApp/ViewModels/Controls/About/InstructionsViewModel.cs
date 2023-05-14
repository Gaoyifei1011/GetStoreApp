using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Window;
using GetStoreApp.UI.Dialogs.About;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Xaml;
using System;
using Windows.System;

namespace GetStoreApp.ViewModels.Controls.About
{
    /// <summary>
    /// 关于页面：使用说明控件视图模型
    /// </summary>
    public sealed class InstructionsViewModel
    {
        /// <summary>
        /// 桌面程序启动参数说明
        /// </summary>
        public async void OnDesktopLaunchClicked(object sender,RoutedEventArgs args)
        {
            await new DesktopStartupArgsDialog().ShowAsync();
        }

        /// <summary>
        /// 控制台程序启动参数说明
        /// </summary>
        public async void OnConsoleLaunchClicked(object sender,RoutedEventArgs args)
        {
            await new ConsoleStartupArgsDialog().ShowAsync();
        }

        /// <summary>
        /// 检查网络
        /// </summary>
        public async void OnCheckNetWorkClicked(object sender,RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:network"));
        }

        /// <summary>
        /// 疑难解答
        /// </summary>
        public async void OnTroubleShootClicked(object sender,RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:troubleshoot"));
        }

        /// <summary>
        /// Cloudflare 5秒验证信息
        /// </summary>
        public async void OnCloudflareValidationClicked(object sender,RoutedEventArgs args)
        {
            await new CloudflareValidationDialog().ShowAsync();
        }

        /// <summary>
        /// 打开下载设置
        /// </summary>
        public void OnDownloadSettingsClicked(object sender,RoutedEventArgs args)
        {
            NavigationService.NavigateTo(typeof(SettingsPage), AppNaviagtionArgs.DownloadOptions);
        }
    }
}
