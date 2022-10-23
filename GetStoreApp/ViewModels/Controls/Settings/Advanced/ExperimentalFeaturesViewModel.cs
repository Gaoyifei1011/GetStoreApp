using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.UI.Dialogs;
using System;

namespace GetStoreApp.ViewModels.Controls.Settings.Advanced
{
    public class ExperimentalFeaturesViewModel : ObservableRecipient
    {
        public IRelayCommand ConfigCommand => new RelayCommand(async () =>
        {
            await new ExperimentalConfigDialog().ShowAsync();
        });
    }
}
