using CommunityToolkit.Mvvm.ComponentModel;
using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Events;
using GetStoreApp.Helpers;
using GetStoreApp.UI.Dialogs;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.ViewModels.Window
{
    public class MainWindowViewModel : ObservableRecipient
    {
        public IAria2Service Aria2Service { get; } = IOCHelper.GetService<IAria2Service>();

        public async void WindowClosed()
        {
            await Aria2Service.CloseAria2Async();
        }

        public async void WindowClosing(object sender, WindowClosingEventArgs e)
        {
            ClosingWindowDialog contentDialog = new ClosingWindowDialog();
            ContentDialogResult result = await contentDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                // 尝试关闭窗口
                e.TryCloseWindow();
            }
        }
    }
}
