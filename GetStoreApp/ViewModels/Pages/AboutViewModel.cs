using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.UI.Dialogs;
using System;

namespace GetStoreApp.ViewModels.Pages
{
    public class AboutViewModel : ObservableRecipient
    {
        // 查看更新日志
        public IRelayCommand ShowReleaseNotesCommand => new RelayCommand(async () =>
        {
            //await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/GetStoreApp/releases"));
            await new ReleaseNotesDialog().ShowAsync();
        });

        // 查看许可证
        public IRelayCommand ShowLicenseCommand => new RelayCommand(async () =>
        {
            await new LicenseDialog().ShowAsync();
        });
    }
}
