using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.UI.Dialogs;
using System;

namespace GetStoreApp.ViewModels.Controls.About
{
    public class InstructionsViewModel : ObservableRecipient
    {
        public IAsyncRelayCommand CheckNetWorkCommand { get; } = new AsyncRelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:network"));
        });

        public IAsyncRelayCommand TroubleShootCommand { get; } = new AsyncRelayCommand(async () =>
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:troubleshoot"));
        });

        public IAsyncRelayCommand CloudflareValidationCommand { get; } = new AsyncRelayCommand(async () =>
        {
            CloudflareValidationDialog dialog = new CloudflareValidationDialog { XamlRoot = App.MainWindow.Content.XamlRoot };
            await dialog.ShowAsync();
        });
    }
}
