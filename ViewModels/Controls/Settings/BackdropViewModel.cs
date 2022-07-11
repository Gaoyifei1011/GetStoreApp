using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Models;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class BackdropViewModel : ObservableRecipient
    {
        private readonly IBackdropService _backdropService;

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
            _backdropService = backdropService;

            BackdropList = _backdropService.BackdropList;
            Backdrop = _backdropService.AppBackdrop;

            BackdropSelectCommand = new AsyncRelayCommand(async () =>
            {
                await _backdropService.SetBackdropAsync(Backdrop);
            });
        }
    }
}
