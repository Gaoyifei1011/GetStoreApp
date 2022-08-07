using CommunityToolkit.Mvvm.ComponentModel;
using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Helpers;

namespace GetStoreApp.ViewModels.Window
{
    public class MainWindowViewModel : ObservableRecipient
    {
        public IAria2Service Aria2Service { get; } = IOCHelper.GetService<IAria2Service>();

        public async void WindowClosed()
        {
            await Aria2Service.CloseAria2Async();
        }
    }
}
