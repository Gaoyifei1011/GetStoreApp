using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.UI.Dialogs;
using System;

namespace GetStoreApp.ViewModels.Controls.About
{
    public class InstructionsViewModel : ObservableRecipient
    {
        // 了解字体图标库
        public IAsyncRelayCommand AcknowledgeIconCommand => new AsyncRelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://learn.microsoft.com/zh-cn/windows/apps/design/downloads/#fonts"));
        });

        // 下载字体图标库
        public IAsyncRelayCommand InstallIconCommand => new AsyncRelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://aka.ms/SegoeFluentIcons"));
        });

        // 检查网络
        public IAsyncRelayCommand CheckNetWorkCommand => new AsyncRelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:network"));
        });

        // 疑难解答
        public IAsyncRelayCommand TroubleShootCommand => new AsyncRelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:troubleshoot"));
        });

        // Cloudflare 5秒验证信息
        public IAsyncRelayCommand CloudflareValidationCommand => new AsyncRelayCommand(async () =>
        {
            await new CloudflareValidationDialog().ShowAsync();
        });
    }
}
