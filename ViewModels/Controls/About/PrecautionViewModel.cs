using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.UI.Dialogs;
using System;

namespace GetStoreApp.ViewModels.Controls.About
{
    public class PrecautionViewModel : ObservableRecipient
    {
        public IAsyncRelayCommand RecognizeCommand { get; set; }

        public PrecautionViewModel()
        {
            RecognizeCommand = new AsyncRelayCommand(async () =>
            {
                DesktopAppsDialog dialog = new DesktopAppsDialog { XamlRoot = App.MainWindow.Content.XamlRoot };
                await dialog.ShowAsync();
            });
        }
    }
}
