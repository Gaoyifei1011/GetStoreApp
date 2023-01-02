using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.UI.Dialogs.About;
using System;

namespace GetStoreApp.ViewModels.Controls.About
{
    public sealed class PrecautionViewModel
    {
        // 区分传统桌面应用
        public IRelayCommand RecognizeCommand => new RelayCommand(async () =>
        {
            if (!App.Current.IsDialogOpening)
            {
                App.Current.IsDialogOpening = true;
                await new DesktopAppsDialog().ShowAsync();
                App.Current.IsDialogOpening = false;
            }
        });
    }
}
