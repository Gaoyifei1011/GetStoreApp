using GetStoreApp.UI.Dialogs.About;
using Microsoft.UI.Xaml;
using System;
using Windows.System;

namespace GetStoreApp.ViewModels.Controls.About
{
    /// <summary>
    /// 关于页面：设置选项说明用户控件视图模型
    /// </summary>
    public sealed class SettingsHelpViewModel
    {
        /// <summary>
        /// 应用信息
        /// </summary>
        public async void OnAppInformationClicked(object sender,RoutedEventArgs args)
        {
            await new AppInformationDialog().ShowAsync();
        }

        /// <summary>
        /// 应用设置
        /// </summary>
        public async void OnAppSettingsClicked(object sender,RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures-app"));
        }

        /// <summary>
        /// 了解包块映射文件
        /// </summary>
        public async void OnBlockMapClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://docs.microsoft.com/en-us/uwp/schemas/blockmapschema/app-package-block-map"));
        }

        /// <summary>
        /// 了解以".e"开头的文件
        /// </summary>
        public async void OnStartWithEClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("https://datatypes.net/open-eappx-files#:~:text=EAPPX%20file%20is%20a%20Microsoft%20Windows%20Encrypted%20App,applications%20may%20also%20use%20the%20.eappx%20file%20extension."));
        }

        /// <summary>
        /// 系统信息
        /// </summary>
        public async void OnSystemInformationClicked(object sender,RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:about"));
        }
    }
}
