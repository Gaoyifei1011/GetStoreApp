using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Window;
using GetStoreApp.UI.Dialogs.About;
using GetStoreApp.Views.Pages;
using System;
using Windows.System;

namespace GetStoreApp.ViewModels.Controls.About
{
    /// <summary>
    /// 关于页面：使用说明控件视图模型
    /// </summary>
    public sealed class InstructionsViewModel
    {
        // 桌面程序启动参数说明
        public IRelayCommand DesktopLaunchCommand => new RelayCommand(async () =>
        {
            await new DesktopStartupArgsDialog().ShowAsync();
        });

        // 控制台程序启动参数说明
        public IRelayCommand ConsoleLaunchCommand => new RelayCommand(async () =>
        {
            await new ConsoleStartupArgsDialog().ShowAsync();
        });

        // 检查网络
        public IRelayCommand CheckNetWorkCommand => new RelayCommand(async () =>
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:network"));
        });

        // 疑难解答
        public IRelayCommand TroubleShootCommand => new RelayCommand(async () =>
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:troubleshoot"));
        });

        // Cloudflare 5秒验证信息
        public IRelayCommand CloudflareValidationCommand => new RelayCommand(async () =>
        {
            await new CloudflareValidationDialog().ShowAsync();
        });

        // 下载设置
        public IRelayCommand DownloadSettingsCommand => new RelayCommand(() =>
        {
            NavigationService.NavigateTo(typeof(SettingsPage), AppNaviagtionArgs.DownloadOptions);
        });
    }
}
