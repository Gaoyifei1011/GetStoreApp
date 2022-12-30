using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.UI.Dialogs.Settings;
using System;

namespace GetStoreApp.ViewModels.Pages
{
    public sealed class SettingsViewModel
    {
        // 打开重启应用确认的窗口对话框
        public IRelayCommand RestartCommand = new RelayCommand(async () =>
        {
            if (!App.IsDialogOpening)
            {
                App.IsDialogOpening = true;
                await new RestartAppsDialog().ShowAsync();
                App.IsDialogOpening = false;
            }
        });
    }
}
