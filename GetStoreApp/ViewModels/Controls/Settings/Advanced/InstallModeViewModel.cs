using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models.Controls.Settings.Advanced;
using GetStoreApp.Services.Controls.Settings.Advanced;
using GetStoreApp.Services.Window;
using GetStoreApp.ViewModels.Base;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;

namespace GetStoreApp.ViewModels.Controls.Settings.Advanced
{
    /// <summary>
    /// 设置页面：应用安装方式设置用户控件视图模型
    /// </summary>
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

        /// <summary>
        /// 应用安装方式说明
        /// </summary>
        public void OnInstallModeTipClicked(object sender, RoutedEventArgs args)
        {
            NavigationService.NavigateTo(typeof(AboutPage), AppNaviagtionArgs.SettingsHelp);
        }

        /// <summary>
        /// 应用安装方式设置
        /// </summary>
        public async void OnInstallModeSelectClicked(object sender, RoutedEventArgs args)
        {
            RadioMenuFlyoutItem item = sender as RadioMenuFlyoutItem;
            if (item.Tag is not null)
            {
                InstallMode = InstallModeList[Convert.ToInt32(item.Tag)];
                await InstallModeService.SetInstallModeAsync(InstallMode);
            }
        }
    }
}
