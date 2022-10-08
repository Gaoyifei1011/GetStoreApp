using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace GetStoreApp.ViewModels.Controls.About
{
    public class SettingsHelpViewModel : ObservableRecipient
    {
        // 系统信息
        public IRelayCommand SystemInformationCommand => new RelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:about"));
        });

        // 应用信息
        public IRelayCommand AppInformationCommand => new RelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures"));
        });

        // 了解以".e"开头的文件
        public IRelayCommand StartsWithECommand => new RelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://datatypes.net/open-eappx-files#:~:text=EAPPX%20file%20is%20a%20Microsoft%20Windows%20Encrypted%20App,applications%20may%20also%20use%20the%20.eappx%20file%20extension."));
        });

        // 了解包块映射文件
        public IRelayCommand BlockMapCommand => new RelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://docs.microsoft.com/en-us/uwp/schemas/blockmapschema/app-package-block-map"));
        });
    }
}
