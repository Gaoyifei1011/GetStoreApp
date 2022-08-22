using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Helpers;
using GetStoreApp.Models;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public partial class BackdropViewModel : ObservableRecipient
    {
        private IBackdropService BackdropService { get; } = IOCHelper.GetService<IBackdropService>();

        public List<BackdropModel> BackdropList => BackdropService.BackdropList;

        private BackdropModel _backdrop;

        public BackdropModel Backdrop
        {
            get { return _backdrop; }

            set { SetProperty(ref _backdrop, value); }
        }

        public IAsyncRelayCommand BackdropSelectCommand => new AsyncRelayCommand(async () =>
        {
            await BackdropService.SetBackdropAsync(Backdrop);
            await BackdropService.SetAppBackdropAsync();
        });

        public BackdropViewModel()
        {
            Backdrop = BackdropService.AppBackdrop;
        }
    }
}
