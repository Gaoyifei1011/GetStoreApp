using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Controls.Extensions;
using GetStoreApp.Services.Window;
using GetStoreApp.UI.Dialogs.About;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.System;

namespace GetStoreApp.UI.Controls.About
{
    /// <summary>
    /// 关于页面：使用说明控件
    /// </summary>
    public sealed partial class InstructionsControl : Expander
    {
        public InstructionsControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 桌面程序启动参数说明
        /// </summary>
        public async void OnDesktopLaunchClicked(object sender, RoutedEventArgs args)
        {
            await ContentDialogHelper.ShowAsync(new DesktopStartupArgsDialog(), this);
        }

        /// <summary>
        /// 控制台程序启动参数说明
        /// </summary>
        public async void OnConsoleLaunchClicked(object sender, RoutedEventArgs args)
        {
            await ContentDialogHelper.ShowAsync(new ConsoleStartupArgsDialog(), this);
        }

        /// <summary>
        /// 检查网络
        /// </summary>
        public async void OnCheckNetWorkClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:network"));
        }

        /// <summary>
        /// 疑难解答
        /// </summary>
        public async void OnTroubleShootClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:troubleshoot"));
        }

        /// <summary>
        /// 打开下载设置
        /// </summary>
        public void OnDownloadSettingsClicked(object sender, RoutedEventArgs args)
        {
            NavigationService.NavigateTo(typeof(SettingsPage), AppNaviagtionArgs.DownloadOptions);
        }
    }
}
