using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.UI.Dialogs;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.ViewModels.Pages
{
    public class SettingsViewModel : ObservableRecipient
    {
        public IAsyncRelayCommand RestartCommand { get; set; }

        public SettingsViewModel()
        {
            RestartCommand = new AsyncRelayCommand(async () =>
            {
                await new RestartAppsDialog().ShowAsync();
            });
        }
    }
}
