using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.UI.Dialogs;
using System;

namespace GetStoreApp.ViewModels.Controls.About
{
    public class InstructionsViewModel : ObservableRecipient
    {
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
            await new CloudflareValidationDialog().ShowAsync();
        });
    }
}
