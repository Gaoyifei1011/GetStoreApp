using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Views.CustomControls.DialogsAndFlyouts;
using Microsoft.UI.Xaml;
using System;

namespace GetStoreApp.UI.Dialogs.Settings
{
    /// <summary>
    /// 应用重启对话框视图
    /// </summary>
    public sealed partial class RestartAppsDialog : ExtendedContentDialog
    {
        public ElementTheme DialogTheme { get; } = (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        public RestartAppsDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 取消重启应用
        /// </summary>
        public void OnCancelClicked(object sender, RoutedEventArgs args)
        {
            Hide();
        }

        /// <summary>
        /// 重启应用
        /// </summary>
        public async void OnRestartAppsClicked(object sender, RoutedEventArgs args)
        {
            await ViewModel.RestartAppsAsync();
        }
    }
}
