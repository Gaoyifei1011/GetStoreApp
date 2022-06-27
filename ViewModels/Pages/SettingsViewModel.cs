using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.UI.Dialogs;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;

namespace GetStoreApp.ViewModels.Pages
{
    public class SettingsViewModel : ObservableRecipient
    {
        public IAsyncRelayCommand RestartCommand { get; set; }

        public SettingsViewModel()
        {
            RestartCommand = new AsyncRelayCommand(async async =>
            {
                ContentDialogResult result = await ShowRestartPromptDialogAsync();

                if(result == ContentDialogResult.Primary) Microsoft.Windows.AppLifecycle.AppInstance.Restart("");
            });
        }

        /// <summary>
        /// 应用重启时显示重启提示对话框
        /// </summary>
        public async Task<ContentDialogResult> ShowRestartPromptDialogAsync()
        {
            RestartPromptDialog dialog = new RestartPromptDialog();
            dialog.XamlRoot = App.MainWindow.Content.XamlRoot;
            return await dialog.ShowAsync();
        }
    }
}
