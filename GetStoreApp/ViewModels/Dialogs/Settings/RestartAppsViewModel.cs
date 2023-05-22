using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Views.CustomControls.DialogsAndFlyouts;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppLifecycle;

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
        public void OnRestartAppsClicked(object sender, RoutedEventArgs args)
        {
            Button button = sender as Button;
            if (button.Tag is not null)
            {
                ((ExtendedContentDialog)button.Tag).Hide();
            }
            RestartApps();
        }

        /// <summary>
        /// 重启应用，并关闭其他进程
        /// </summary>
        private void RestartApps()
        {
            DownloadSchedulerService.CloseDownloadScheduler();
            Aria2Service.CloseAria2();

            // 重启应用
            AppInstance.Restart(string.Empty);
        }
    }
}
