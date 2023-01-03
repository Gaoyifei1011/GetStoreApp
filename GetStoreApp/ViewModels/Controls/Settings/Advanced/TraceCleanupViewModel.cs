using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Window;
using GetStoreApp.UI.Dialogs.Settings;
using GetStoreApp.Views.Pages;
using System;

namespace GetStoreApp.ViewModels.Controls.Settings.Advanced
{
    public sealed class TraceCleanupViewModel
    {
        // 痕迹清理说明
        public IRelayCommand TraceCleanupTipCommand => new RelayCommand(() =>
        {
            App.Current.NavigationArgs = AppNaviagtionArgs.SettingsHelp;
            NavigationService.NavigateTo(typeof(AboutPage));
        });

        // 清理应用内使用的所有痕迹
        public IRelayCommand TraceCleanupCommand = new RelayCommand(async () =>
        {
            if (!App.Current.IsDialogOpening)
            {
                App.Current.IsDialogOpening = true;
                await new TraceCleanupPromptDialog().ShowAsync();
                App.Current.IsDialogOpening = false;
            }
        });
    }
}
