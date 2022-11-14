using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Controls.Settings.Appearance;
using GetStoreApp.Contracts.Services.Window;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Settings.Appearance;
using GetStoreApp.Views.Pages;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings.Appearance
{
    public partial class BackdropViewModel : ObservableRecipient
    {
        private IBackdropService BackdropService { get; } = ContainerHelper.GetInstance<IBackdropService>();

        private INavigationService NavigationService { get; } = ContainerHelper.GetInstance<INavigationService>();

        public List<BackdropModel> BackdropList = new List<BackdropModel>();

        private BackdropModel _backdrop;

        public BackdropModel Backdrop
        {
            get { return _backdrop; }

            set { SetProperty(ref _backdrop, value); }
        }

        public bool BackdropHelp { get; }

        // 背景色不可用时具体信息了解
        public IRelayCommand BackdropTipCommand => new RelayCommand(() =>
        {
            App.NavigationArgs = AppNaviagtionArgs.SettingsHelp;
            NavigationService.NavigateTo(typeof(SettingsPage));
        });

        // 背景色修改设置
        public IRelayCommand BackdropSelectCommand => new RelayCommand(async () =>
        {
            await BackdropService.SetBackdropAsync(Backdrop);
            await BackdropService.SetAppBackdropAsync();
        });

        public BackdropViewModel()
        {
            ulong BuildNumber = InfoHelper.GetSystemVersion()["BuildNumber"];

            if (BuildNumber < 22000)
            {
                BackdropHelp = true;
                BackdropList.Add(BackdropService.BackdropList[0]);
                BackdropList.Add(BackdropService.BackdropList[3]);
            }
            else
            {
                BackdropHelp = false;
                BackdropList = BackdropService.BackdropList;
            }

            Backdrop = BackdropService.AppBackdrop;
        }
    }
}
