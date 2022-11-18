using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Models.Controls.Settings.Advanced;
using GetStoreApp.Services.Controls.Settings.Advanced;
using GetStoreApp.ViewModels.Base;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings.Advanced
{
    public sealed class AppExitViewModel : ViewModelBase
    {
        public List<AppExitModel> AppExitList => AppExitService.AppExitList;

        private AppExitModel _appExit;

        public AppExitModel AppExit
        {
            get { return _appExit; }

            set
            {
                _appExit = value;
                OnPropertyChanged();
            }
        }

        public AppExitViewModel()
        {
            AppExit = AppExitService.AppExit;
        }

        /// <summary>
        /// 应用退出方式设置
        /// </summary>
        public async void OnSelectionChanged(object sender,SelectionChangedEventArgs args)
        {
            await AppExitService.SetAppExitAsync(AppExit);
        }
    }
}
