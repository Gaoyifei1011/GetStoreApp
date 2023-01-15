using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Models.Controls.Settings.Advanced;
using GetStoreApp.Services.Controls.Settings.Advanced;
using GetStoreApp.ViewModels.Base;
using System;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings.Advanced
{
    /// <summary>
    /// 设置页面：应用关闭方式设置用户控件视图模型
    /// </summary>
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

        // 应用退出方式设置
        public IRelayCommand AppExitSelectCommand => new RelayCommand<string>(async (appExitIndex) =>
        {
            AppExit = AppExitList[Convert.ToInt32(appExitIndex)];
            await AppExitService.SetAppExitAsync(AppExit);
        });
    }
}
