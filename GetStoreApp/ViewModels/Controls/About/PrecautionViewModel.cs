using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.UI.Dialogs;
using System;

namespace GetStoreApp.ViewModels.Controls.About
{
    public class PrecautionViewModel : ObservableRecipient
    {
        // 区分传统桌面应用
        public IAsyncRelayCommand RecognizeCommand => new AsyncRelayCommand(async () =>
        {
            await new DesktopAppsDialog().ShowAsync();
        });
    }
}
