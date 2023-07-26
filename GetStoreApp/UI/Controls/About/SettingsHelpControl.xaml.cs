using GetStoreApp.Helpers.Controls.Extensions;
using GetStoreApp.UI.Dialogs.About;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.System;

namespace GetStoreApp.UI.Controls.About
{
    /// <summary>
    /// 关于页面：设置选项说明控件
    /// </summary>
    public sealed partial class SettingsHelpControl : Expander
    {
        public SettingsHelpControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 应用信息
        /// </summary>
        public async void OnAppInformationClicked(object sender, RoutedEventArgs args)
        {
            await ContentDialogHelper.ShowAsync(new AppInformationDialog(), this);
        }

        /// <summary>
        /// 应用设置
        /// </summary>
        public async void OnAppSettingsClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures-app"));
        }

        /// <summary>
        /// 系统信息
        /// </summary>
        public async void OnSystemInformationClicked(object sender, RoutedEventArgs args)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:about"));
        }
    }
}
