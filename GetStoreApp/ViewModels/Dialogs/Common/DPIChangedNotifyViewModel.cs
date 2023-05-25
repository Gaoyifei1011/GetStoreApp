using GetStoreApp.Services.Controls.Download;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppLifecycle;

namespace GetStoreApp.ViewModels.Dialogs.Common
{
    /// <summary>
    /// 屏幕缩放通知对话框视图模型
    /// </summary>
    public sealed class DPIChangedNotifyViewModel
    {
        /// <summary>
        /// 重启应用
        /// </summary>
        public void OnRestartClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
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
