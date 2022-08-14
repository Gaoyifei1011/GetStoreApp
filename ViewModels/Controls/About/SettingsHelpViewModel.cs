using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace GetStoreApp.ViewModels.Controls.About
{
    public class SettingsHelpViewModel : ObservableRecipient
    {
        public IAsyncRelayCommand LaunchSettingsInstalledAppsCommand { get; } = new AsyncRelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures"));
        });

        public IAsyncRelayCommand StartsWithECommand { get; } = new AsyncRelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://datatypes.net/open-eappx-files#:~:text=EAPPX%20file%20is%20a%20Microsoft%20Windows%20Encrypted%20App,applications%20may%20also%20use%20the%20.eappx%20file%20extension."));
        });

        public IAsyncRelayCommand BlockMapCommand { get; } = new AsyncRelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://docs.microsoft.com/en-us/uwp/schemas/blockmapschema/app-package-block-map"));
        });
    }
}
