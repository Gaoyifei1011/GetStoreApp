using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.UI.Dialogs.About;
using System;
using Windows.System;

namespace GetStoreApp.ViewModels.Controls.About
{
    /// <summary>
    /// 关于页面：设置选项说明用户控件视图模型
    /// </summary>
    public sealed class SettingsHelpViewModel
    {
        // 系统信息
        public IRelayCommand SystemInformationCommand => new RelayCommand(async () =>
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:about"));
        });

        // 应用信息
        public IRelayCommand AppInformationCommand => new RelayCommand(async () =>
        {
            await new AppInformationDialog().ShowAsync();
        });

        // 应用设置
        public IRelayCommand AppSettingsCommand => new RelayCommand(async () =>
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures-app"));
        });

        // 了解以".e"开头的文件
        public IRelayCommand StartsWithECommand => new RelayCommand(async () =>
        {
            await Launcher.LaunchUriAsync(new Uri("https://datatypes.net/open-eappx-files#:~:text=EAPPX%20file%20is%20a%20Microsoft%20Windows%20Encrypted%20App,applications%20may%20also%20use%20the%20.eappx%20file%20extension."));
        });

        // 了解包块映射文件
        public IRelayCommand BlockMapCommand => new RelayCommand(async () =>
        {
            await Launcher.LaunchUriAsync(new Uri("https://docs.microsoft.com/en-us/uwp/schemas/blockmapschema/app-package-block-map"));
        });
    }
}
