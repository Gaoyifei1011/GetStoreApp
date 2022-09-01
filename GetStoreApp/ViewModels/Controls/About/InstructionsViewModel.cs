using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.UI.Dialogs;
using System;

namespace GetStoreApp.ViewModels.Controls.About
{
    public class InstructionsViewModel : ObservableRecipient
    {
        public IAsyncRelayCommand CheckNetWorkCommand => new AsyncRelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:network"));
        });

        public IAsyncRelayCommand TroubleShootCommand => new AsyncRelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:troubleshoot"));
        });

        public IAsyncRelayCommand CloudflareValidationCommand => new AsyncRelayCommand(async () =>
        {
            await new CloudflareValidationDialog().ShowAsync();
        });
    }
}
