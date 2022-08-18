using CommunityToolkit.Mvvm.ComponentModel;
using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.Events;
using GetStoreApp.Helpers;
using GetStoreApp.UI.Dialogs;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;

namespace GetStoreApp.ViewModels.Window
{
    public class MainWindowViewModel : ObservableRecipient
    {
        public IAria2Service Aria2Service { get; } = IOCHelper.GetService<IAria2Service>();

        private INavigationService NavigationService { get; } = IOCHelper.GetService<INavigationService>();

        public async void WindowClosed()
        {
            await Aria2Service.CloseAria2Async();
        }

        public async void WindowClosing(object sender, WindowClosingEventArgs args)
        {
            ClosingWindowDialog contentDialog = new ClosingWindowDialog();
            ContentDialogResult result = await contentDialog.ShowAsync();

            if(result == ContentDialogResult.Primary) { args.TryCloseWindow(); }
            else if (result == ContentDialogResult.Secondary)
            {
                NavigationService.NavigateTo(typeof(DownloadViewModel).FullName, null, new DrillInNavigationTransitionInfo());
            }
        }
    }
}
