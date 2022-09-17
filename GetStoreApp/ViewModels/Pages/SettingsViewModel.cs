using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.UI.Dialogs;
using System;

namespace GetStoreApp.ViewModels.Pages
{
    public class SettingsViewModel : ObservableRecipient
    {
        // 打开重启应用确认的窗口对话框
        public IAsyncRelayCommand RestartCommand = new AsyncRelayCommand(async () =>
        {
            await new RestartAppsDialog().ShowAsync();
        });
    }
}
