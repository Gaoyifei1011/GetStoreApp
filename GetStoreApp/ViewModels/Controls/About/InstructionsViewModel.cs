using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Window;
using GetStoreApp.UI.Dialogs.About;
using GetStoreApp.Views.Pages;
using System;

namespace GetStoreApp.ViewModels.Controls.About
{
    public sealed class InstructionsViewModel
    {
        // 桌面程序启动参数说明
        public IRelayCommand DesktopLaunchCommand => new RelayCommand(async () =>
        {
            if (!App.Current.IsDialogOpening)
            {
                App.Current.IsDialogOpening = true;
                await new DesktopStartupArgsDialog().ShowAsync();
                App.Current.IsDialogOpening = false;
            }
        });

        // 控制台程序启动参数说明
        public IRelayCommand ConsoleLaunchCommand => new RelayCommand(async () =>
        {
            if (!App.Current.IsDialogOpening)
            {
                App.Current.IsDialogOpening = true;
                await new ConsoleStartupArgsDialog().ShowAsync();
                App.Current.IsDialogOpening = false;
            }
        });

        // 检查网络
        public IRelayCommand CheckNetWorkCommand => new RelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:network"));
        });

        // 疑难解答
        public IRelayCommand TroubleShootCommand => new RelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:troubleshoot"));
        });

        // Cloudflare 5秒验证信息
        public IRelayCommand CloudflareValidationCommand => new RelayCommand(async () =>
        {
            if (!App.Current.IsDialogOpening)
            {
                App.Current.IsDialogOpening = true;
                await new CloudflareValidationDialog().ShowAsync();
                App.Current.IsDialogOpening = false;
            }
        });

        // 下载设置
        public IRelayCommand DownloadSettingsCommand => new RelayCommand(() =>
        {
            App.Current.NavigationArgs = AppNaviagtionArgs.DownloadOptions;
            NavigationService.NavigateTo(typeof(SettingsPage));
        });
    }
}
