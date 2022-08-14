using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Helpers;
using GetStoreApp.Models;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class BackdropViewModel : ObservableRecipient
    {
        private IBackdropService BackdropService { get; } = IOCHelper.GetService<IBackdropService>();

        private BackdropModel _backdrop;

        public BackdropModel Backdrop
        {
            get { return _backdrop; }

            set { SetProperty(ref _backdrop, value); }
        }

        public List<BackdropModel> BackdropList { get; set; }

        public IAsyncRelayCommand BackdropSelectCommand { get; }

        public BackdropViewModel()
        {
            BackdropList = BackdropService.BackdropList;
            Backdrop = BackdropService.AppBackdrop;

            BackdropSelectCommand = new AsyncRelayCommand(async () =>
            {
                await BackdropService.SetBackdropAsync(Backdrop);
                await BackdropService.SetAppBackdropAsync();
            });
        }
    }
}
