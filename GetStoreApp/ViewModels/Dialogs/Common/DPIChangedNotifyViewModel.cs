using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.Messaging;
using GetStoreApp.Helpers.Window;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppLifecycle;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Dialogs.Common
{
    /// <summary>
    /// 屏幕缩放通知对话框视图模型
    /// </summary>
    public sealed class DPIChangedNotifyViewModel
    {
        // 重启应用
        public IRelayCommand RestartAppsCommand => new RelayCommand<ContentDialog>(async (dialog) =>
        {
            dialog.Hide();
            await RestartAppsAsync();
        });

        // 关闭对话框
        public IRelayCommand CloseDialogCommand => new RelayCommand<ContentDialog>((dialog) =>
        {
            dialog.Hide();
        });

        /// <summary>
        /// 重启应用，并关闭其他进程
        /// </summary>
        private async Task RestartAppsAsync()
        {
            await DownloadSchedulerService.CloseDownloadSchedulerAsync();
            await Aria2Service.CloseAria2Async();
            BackdropHelper.ReleaseBackdrop();
            Messenger.Default.Send(true, MessageToken.WindowClosed);

            // 重启应用
            AppInstance.Restart(string.Empty);
        }
    }
}
