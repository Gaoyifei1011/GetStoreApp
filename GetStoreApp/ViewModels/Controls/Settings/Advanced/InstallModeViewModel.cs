using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models.Controls.Settings.Advanced;
using GetStoreApp.Services.Controls.Settings.Advanced;
using GetStoreApp.Services.Window;
using GetStoreApp.ViewModels.Base;
using GetStoreApp.Views.Pages;
using System;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings.Advanced
{
    public sealed class InstallModeViewModel : ViewModelBase
    {
        public List<InstallModeModel> InstallModeList => InstallModeService.InstallModeList;

        private InstallModeModel _installMode = InstallModeService.InstallMode;

        public InstallModeModel InstallMode
        {
            get { return _installMode; }

            set
            {
                _installMode = value;
                OnPropertyChanged();
            }
        }

        // 应用安装方式说明
        public IRelayCommand InstallModeTipCommand => new RelayCommand(() =>
        {
            App.NavigationArgs = AppNaviagtionArgs.SettingsHelp;
            NavigationService.NavigateTo(typeof(AboutPage));
        });

        /// <summary>
        /// 应用安装方式设置
        /// </summary>
        public IRelayCommand InstallModeSelectCommand => new RelayCommand<string>(async (installModeIndex) =>
        {
            InstallMode = InstallModeList[Convert.ToInt32(installModeIndex)];
            await InstallModeService.SetInstallModeAsync(InstallMode);
        });
    }
}
