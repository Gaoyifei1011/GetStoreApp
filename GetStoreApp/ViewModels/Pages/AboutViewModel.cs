using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.UI.Dialogs;
using Microsoft.UI.Dispatching;
using System;

namespace GetStoreApp.ViewModels.Pages
{
    public class AboutViewModel : ObservableRecipient
    {
        // 查看更新日志
        public IAsyncRelayCommand ShowReleaseNotesCommand => new AsyncRelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/GetStoreApp/releases"));
        });

        // 查看许可证
        public IAsyncRelayCommand ShowLicenseCommand => new AsyncRelayCommand(async () =>
        {
            await new LicenseDialog().ShowAsync();
        });
    }
}
