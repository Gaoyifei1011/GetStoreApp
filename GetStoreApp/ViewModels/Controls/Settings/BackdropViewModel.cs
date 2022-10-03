using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.Extensions.Enum;
using GetStoreApp.Helpers;
using GetStoreApp.Models.Settings;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml.Media.Animation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public partial class BackdropViewModel : ObservableRecipient
    {
        private IBackdropService BackdropService { get; } = IOCHelper.GetService<IBackdropService>();

        private INavigationService NavigationService { get; } = IOCHelper.GetService<INavigationService>();

        public List<BackdropModel> BackdropList => BackdropService.BackdropList;

        private BackdropModel _backdrop;

        public BackdropModel Backdrop
        {
            get { return _backdrop; }

            set { SetProperty(ref _backdrop, value); }
        }

        public bool BackdropIsEnabled { get; }

        // 背景色不可用时具体信息了解
        public IAsyncRelayCommand BackdropTipCommand => new AsyncRelayCommand(async () =>
        {
            App.NavigationArgs = AppNaviagtionArgs.SettingsHelp;
            NavigationService.NavigateTo(typeof(AboutViewModel).FullName, null, new DrillInNavigationTransitionInfo(), false);
            await Task.CompletedTask;
        });

        // 选择背景色
        public IAsyncRelayCommand BackdropSelectCommand => new AsyncRelayCommand(async () =>
        {
            await BackdropService.SetBackdropAsync(Backdrop);
            await BackdropService.SetAppBackdropAsync();
        });

        public BackdropViewModel()
        {
            Backdrop = BackdropService.AppBackdrop;

            ulong BuildNumber = InfoHelper.GetSystemVersion()["BuildNumber"];

            if (BuildNumber < 22000)
            {
                Backdrop = BackdropList[0];
                BackdropIsEnabled = false;
            }
            else
            {
                BackdropIsEnabled = true;
            }
        }
    }
}
