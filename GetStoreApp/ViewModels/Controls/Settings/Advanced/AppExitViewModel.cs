using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Settings.Advanced;
using GetStoreApp.Helpers;
using GetStoreApp.Models.Settings;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings.Advanced
{
    public class AppExitViewModel : ObservableRecipient
    {
        private IAppExitService AppExitService { get; } = IOCHelper.GetService<IAppExitService>();

        public List<AppExitModel> AppExitList => AppExitService.AppExitList;

        private AppExitModel _appExit;

        public AppExitModel AppExit
        {
            get { return _appExit; }

            set { SetProperty(ref _appExit, value); }
        }

        public IRelayCommand AppExitSelectCommand => new RelayCommand(async () =>
        {
            await AppExitService.SetAppExitAsync(AppExit);
        });

        public AppExitViewModel()
        {
            AppExit = AppExitService.AppExit;
        }
    }
}
