using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Models;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class BackdropViewModel : ObservableRecipient
    {
        private readonly IBackdropService BackdropService;

        private string _backdrop;

        public string Backdrop
        {
            get { return _backdrop; }

            set { SetProperty(ref _backdrop, value); }
        }

        public List<BackdropModel> BackdropList { get; set; }

        public IAsyncRelayCommand BackdropSelectCommand { get; set; }

        public BackdropViewModel(IBackdropService backdropService)
        {
            BackdropService = backdropService;

            BackdropList = BackdropService.BackdropList;
            Backdrop = BackdropService.AppBackdrop;

            BackdropSelectCommand = new AsyncRelayCommand(async () =>
            {
                await BackdropService.SetBackdropAsync(Backdrop);
            });
        }
    }
}
