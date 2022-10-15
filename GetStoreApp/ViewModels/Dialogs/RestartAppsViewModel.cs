using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Helpers;
using GetStoreApp.Messages;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Dialogs
{
    public class RestartAppsViewModel : ObservableRecipient
    {
        private IAria2Service Aria2Service { get; } = IOCHelper.GetService<IAria2Service>();

        private IDownloadSchedulerService DownloadSchedulerService { get; } = IOCHelper.GetService<IDownloadSchedulerService>();

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
            WeakReferenceMessenger.Default.Send(new TrayIconDisposeMessage(true));

            // 重启应用
            AppInstance.Restart("");
        }
    }
}
