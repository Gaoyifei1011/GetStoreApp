using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Helpers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Dialogs
{
    public class RestartAppsViewModel : ObservableRecipient
    {
        public IAria2Service Aria2Service { get; } = IOCHelper.GetService<IAria2Service>();

        public IAsyncRelayCommand RestartAppsSureCommand { get; }

        public IAsyncRelayCommand RestartAppsCancelCommand { get; }

        public RestartAppsViewModel()
        {
            RestartAppsSureCommand = new AsyncRelayCommand<ContentDialog>(async (param) =>
            {
                await RestartAppsAsync(param);
            });

            RestartAppsCancelCommand = new AsyncRelayCommand<ContentDialog>(async (param) =>
            {
                param.Hide();
                await Task.CompletedTask;
            });
        }

        /// <summary>
        /// 重启应用，并关闭Aria2下载服务
        /// </summary>
        private async Task RestartAppsAsync(ContentDialog dialog)
        {
            dialog.Hide();

            // 关闭Aria2下载服务
            await Aria2Service.CloseAria2Async();

            // 重启应用
            AppInstance.Restart("");
        }
    }
}
