using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.UI.Dialogs.ContentDialogs.Settings;
using System;

namespace GetStoreApp.ViewModels.Controls.Settings.Advanced
{
    public class ExperimentalFeaturesViewModel : ObservableRecipient
    {
        // 实验功能设置
        public IRelayCommand ConfigCommand => new RelayCommand(async () =>
        {
            await new ExperimentalConfigDialog().ShowAsync();
        });
    }
}
