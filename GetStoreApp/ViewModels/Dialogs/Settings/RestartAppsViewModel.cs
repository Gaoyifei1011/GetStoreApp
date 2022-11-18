using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Messages;
using GetStoreApp.Services.Controls.Download;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Dialogs.Settings
{
    public sealed class RestartAppsViewModel
    {
        // 重启应用
        public IRelayCommand RestartAppsCommand => new RelayCommand<ContentDialog>(async (dialog) =>
        {
            await RestartAppsAsync(dialog);
        });

        // 取消重启应用
        public IRelayCommand CloswWindowCommand => new RelayCommand<ContentDialog>((dialog) =>
        {
            dialog.Hide();
        });

        /// <summary>
        /// 重启应用，并关闭Aria2下载服务
        /// </summary>
        private async Task RestartAppsAsync(ContentDialog dialog)
        {
            dialog.Hide();

            await DownloadSchedulerService.CloseDownloadSchedulerAsync();
            await Aria2Service.CloseAria2Async();
            WeakReferenceMessenger.Default.Send(new WindowClosedMessage(true));

            // 重启应用
            AppInstance.Restart("");
        }
    }
}
