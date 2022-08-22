using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.UI.Dialogs;
using System;

namespace GetStoreApp.ViewModels.Pages
{
    public class SettingsViewModel : ObservableRecipient
    {
        public IAsyncRelayCommand RestartCommand = new AsyncRelayCommand(async () =>
        {
            await new RestartAppsDialog().ShowAsync();
        });
    }
}
