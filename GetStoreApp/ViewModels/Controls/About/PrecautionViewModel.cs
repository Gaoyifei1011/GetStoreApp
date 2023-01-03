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
            if (!Program.ApplicationRoot.IsDialogOpening)
            {
                Program.ApplicationRoot.IsDialogOpening = true;
                await new DesktopAppsDialog().ShowAsync();
                Program.ApplicationRoot.IsDialogOpening = false;
            }
        });
    }
}
