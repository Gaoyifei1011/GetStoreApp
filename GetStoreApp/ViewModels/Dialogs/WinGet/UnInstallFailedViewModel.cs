using GetStoreApp.Views.CustomControls.DialogsAndFlyouts;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.System;

namespace GetStoreApp.ViewModels.Dialogs.WinGet
{
    /// <summary>
    /// 卸载失败提示对话框视图模型
    /// </summary>
    public sealed class UnInstallFailedViewModel
    {
        public string UnInstallFailedContent { get; set; }

        /// <summary>
        /// 关闭对话框
        /// </summary>
        public void OnCloseDialogClicked(object sender, RoutedEventArgs args)
        {
            Button button = sender as Button;
            if (button.Tag is not null)
            {
                ((ExtendedContentDialog)button.Tag).Hide();
            }
        }

        /// <summary>
        /// 打开应用和功能卸载应用
        /// </summary>
        public async void OnOpenSettingsClicked(object sender, RoutedEventArgs args)
        {
            Button button = sender as Button;
            if (button.Tag is not null)
            {
                await Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures"));
                ((ExtendedContentDialog)button.Tag).Hide();
            }
        }
    }
}
