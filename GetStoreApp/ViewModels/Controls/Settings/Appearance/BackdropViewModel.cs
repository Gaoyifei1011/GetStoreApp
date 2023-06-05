using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.Settings.Appearance;
using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Services.Window;
using GetStoreApp.ViewModels.Base;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings.Appearance
{
    /// <summary>
    /// 设置页面：窗口背景材质设置用户控件视图模型
    /// </summary>
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

        /// <summary>
        /// 背景色不可用时具体信息了解
        /// </summary>
        public void OnBackdropTipClicked(object sender, RoutedEventArgs args)
        {
            NavigationService.NavigateTo(typeof(AboutPage), AppNaviagtionArgs.SettingsHelp);
        }

        /// <summary>
        /// 背景色修改设置
        /// </summary>
        public async void OnBackdropSelectClicked(object sender, RoutedEventArgs args)
        {
            RadioMenuFlyoutItem item = sender as RadioMenuFlyoutItem;
            if (item.Tag is not null)
            {
                Backdrop = BackdropList[Convert.ToInt32(item.Tag)];
                await BackdropService.SetBackdropAsync(Backdrop);
                BackdropService.SetAppBackdrop();
            }
        }

        public BackdropViewModel()
        {
            int BuildNumber = InfoHelper.GetSystemVersion().Build;

            CanUseMicaBackdrop = BuildNumber >= 20000;
        }
    }
}
