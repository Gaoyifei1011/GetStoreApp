using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Controls.Settings.Advanced;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Settings.Advanced;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings.Advanced
{
    public class AppExitViewModel : ObservableRecipient
    {
        private IAppExitService AppExitService { get; } = ContainerHelper.GetInstance<IAppExitService>();

        public List<AppExitModel> AppExitList => AppExitService.AppExitList;

        private AppExitModel _appExit;

        public AppExitModel AppExit
        {
            get { return _appExit; }

            set { SetProperty(ref _appExit, value); }
        }

        // 应用退出方式设置
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
