using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Models.Controls.Settings.Advanced;
using GetStoreApp.Services.Controls.Settings.Advanced;
using GetStoreApp.ViewModels.Base;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings.Advanced
{
    public sealed class AppExitViewModel : ViewModelBase
    {
        public List<AppExitModel> AppExitList { get; } = AppExitService.AppExitList;

        private AppExitModel _appExit = AppExitService.AppExit;

        public AppExitModel AppExit
        {
            get { return _appExit; }

            set
            {
                _appExit = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 应用退出方式设置
        /// </summary>
        public IRelayCommand AppExitSelectCommand => new RelayCommand<string>(async (appExitIndex) =>
        {
            AppExit = AppExitList[Convert.ToInt32(appExitIndex)];
            await AppExitService.SetAppExitAsync(AppExit);
        });
    }
}
