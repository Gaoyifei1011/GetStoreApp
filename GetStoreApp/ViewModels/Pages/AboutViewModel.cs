using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.UI.Dialogs.About;
using System;

namespace GetStoreApp.ViewModels.Pages
{
    public sealed class AboutViewModel
    {
        // 查看更新日志
        public IRelayCommand ShowReleaseNotesCommand => new RelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/Gaoyifei1011/GetStoreApp/releases"));
        });

        // 查看许可证
        public IRelayCommand ShowLicenseCommand => new RelayCommand(async () =>
        {
            if (!App.IsDialogOpening)
            {
                App.IsDialogOpening = true;
                await new LicenseDialog().ShowAsync();
                App.IsDialogOpening = false;
            }
        });
    }
}
