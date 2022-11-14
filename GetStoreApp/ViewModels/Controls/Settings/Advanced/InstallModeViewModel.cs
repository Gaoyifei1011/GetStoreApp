using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Controls.Settings.Advanced;
using GetStoreApp.Contracts.Services.Window;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Settings.Advanced;
using GetStoreApp.Views.Pages;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings.Advanced
{
    public class InstallModeViewModel : ObservableRecipient
    {
        private IInstallModeService InstallModeService { get; } = ContainerHelper.GetInstance<IInstallModeService>();

        private INavigationService NavigationService { get; } = ContainerHelper.GetInstance<INavigationService>();

        public List<InstallModeModel> InstallModeList => InstallModeService.InstallModeList;

        private InstallModeModel _installMode;

        public InstallModeModel InstallMode
        {
            get { return _installMode; }

            set { SetProperty(ref _installMode, value); }
        }

        // 应用安装方式说明
        public IRelayCommand InstallModeTipCommand => new RelayCommand(() =>
        {
            App.NavigationArgs = AppNaviagtionArgs.SettingsHelp;
            NavigationService.NavigateTo(typeof(AboutPage));
        });

        // 应用安装方式设置
        public IRelayCommand InstallModeSelectCommand => new RelayCommand(async () =>
        {
            await InstallModeService.SetInstallModeAsync(InstallMode);
        });

        public InstallModeViewModel()
        {
            InstallMode = InstallModeService.InstallMode;
        }
    }
}
