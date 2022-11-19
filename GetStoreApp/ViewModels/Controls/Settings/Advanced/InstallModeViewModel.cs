using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models.Controls.Settings.Advanced;
using GetStoreApp.Services.Controls.Settings.Advanced;
using GetStoreApp.Services.Window;
using GetStoreApp.ViewModels.Base;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings.Advanced
{
    public sealed class InstallModeViewModel : ViewModelBase
    {
        public List<InstallModeModel> InstallModeList => InstallModeService.InstallModeList;

        private InstallModeModel _installMode;

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

        public InstallModeViewModel()
        {
            InstallMode = InstallModeService.InstallMode;
        }

        /// <summary>
        /// 应用安装方式设置
        /// </summary>
        public async void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            await InstallModeService.SetInstallModeAsync(InstallMode);
        }
    }
}
