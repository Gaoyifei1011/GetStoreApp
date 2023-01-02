using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.Messaging;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Settings.Appearance;
using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Services.Window;
using GetStoreApp.ViewModels.Base;
using GetStoreApp.Views.Pages;
using System;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings.Appearance
{
    public sealed class BackdropViewModel : ViewModelBase
    {
        public List<BackdropModel> BackdropList { get; } = BackdropService.BackdropList;

        private BackdropModel _backdrop = BackdropService.AppBackdrop;

        public BackdropModel Backdrop
        {
            get { return _backdrop; }

            set
            {
                _backdrop = value;
                OnPropertyChanged();
            }
        }

        public bool CanUseMicaBackdrop { get; set; }

        // 背景色不可用时具体信息了解
        public IRelayCommand BackdropTipCommand => new RelayCommand(() =>
        {
            App.Current.NavigationArgs = AppNaviagtionArgs.SettingsHelp;
            NavigationService.NavigateTo(typeof(SettingsPage));
        });

        /// <summary>
        /// 背景色修改设置
        /// </summary>
        public IRelayCommand BackdropSelectCommand => new RelayCommand<string>(async (backdropIndex) =>
        {
            Backdrop = BackdropList[Convert.ToInt32(backdropIndex)];
            await BackdropService.SetBackdropAsync(Backdrop);
            await BackdropService.SetAppBackdropAsync();
            Messenger.Default.Send(Backdrop, MessageToken.BackdropChanged);
        });

        public BackdropViewModel()
        {
            ulong BuildNumber = InfoHelper.GetSystemVersion()["BuildNumber"];

            CanUseMicaBackdrop = BuildNumber >= 20000;
        }
    }
}
