using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.Helpers;
using GetStoreApp.Models.Settings;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class InstallModeViewModel : ObservableRecipient
    {
        private IInstallModeService InstallModeService { get; } = IOCHelper.GetService<IInstallModeService>();

        private INavigationService NavigationService { get; } = IOCHelper.GetService<INavigationService>();

        public List<InstallModeModel> InstallModeList => InstallModeService.InstallModeList;

        private InstallModeModel _installMode;

        public InstallModeModel InstallMode
        {
            get { return _installMode; }

            set { SetProperty(ref _installMode, value); }
        }

        // 应用安装方式说明
        public IAsyncRelayCommand InstallModeTipCommand => new AsyncRelayCommand(async () =>
        {
            App.NavigationArgs = "SettingsHelp";
            NavigationService.NavigateTo(typeof(AboutViewModel).FullName, null, new DrillInNavigationTransitionInfo());
            await Task.CompletedTask;
        });

        public IAsyncRelayCommand InstallModeSelectCommand => new AsyncRelayCommand(async () =>
        {
            await InstallModeService.SetInstallModeAsync(InstallMode);
        });

        public InstallModeViewModel()
        {
            InstallMode = InstallModeService.InstallMode;
        }
    }
}
