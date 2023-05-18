using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.Messaging;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Views.CustomControls.DialogsAndFlyouts;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Dialogs.Settings
{
    /// <summary>
    /// 应用重启对话框视图模型
    /// </summary>
    public sealed class RestartAppsViewModel
    {
        /// <summary>
        /// 取消重启应用
        /// </summary>
        public void OnCancelClicked(object sender, RoutedEventArgs args)
        {
            Button button = sender as Button;
            if (button.Tag is not null)
            {
                ((ExtendedContentDialog)button.Tag).Hide();
            }
        }

        /// <summary>
        /// 重启应用
        /// </summary>
        public async void OnRestartAppsClicked(object sender, RoutedEventArgs args)
        {
            Button button = sender as Button;
            if (button.Tag is not null)
            {
                ((ExtendedContentDialog)button.Tag).Hide();
            }
            await RestartAppsAsync();
        }

        /// <summary>
        /// 重启应用，并关闭其他进程
        /// </summary>
        private async Task RestartAppsAsync()
        {
            await DownloadSchedulerService.CloseDownloadSchedulerAsync();
            await Aria2Service.CloseAria2Async();
            Messenger.Default.Send(true, MessageToken.WindowClosed);

            // 重启应用
            AppInstance.Restart(string.Empty);
        }
    }
}
