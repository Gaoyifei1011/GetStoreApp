using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.Helpers;
using GetStoreApp.UI.Dialogs;
using GetStoreApp.ViewModels.Pages;
using GetStoreApp.Views.Pages;
using H.NotifyIcon;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Tray
{
    public class TaskBarViewModel : ObservableRecipient
    {
        private IAria2Service Aria2Service { get; } = IOCHelper.GetService<IAria2Service>();

        private IDownloadSchedulerService DownloadSchedulerService { get; } = IOCHelper.GetService<IDownloadSchedulerService>();

        private INavigationService NavigationService { get; } = IOCHelper.GetService<INavigationService>();

        // 隐藏 / 显示窗口
        public IAsyncRelayCommand ShowOrHideWindowCommand => new AsyncRelayCommand(async () =>
        {
            // 隐藏窗口
            if (App.MainWindow.Visible)
            {
                WindowHelper.HideAppWindow();
            }
            // 显示窗口
            else
            {
                WindowHelper.ShowAppWindow();
            }
            await Task.CompletedTask;
        });

        public IAsyncRelayCommand SettingsCommand => new AsyncRelayCommand(async () =>
        {
            // 窗口置前端
            WindowHelper.ShowAppWindow();

            if (NavigationService.Frame.CurrentSourcePageType != typeof(SettingsPage))
            {
                NavigationService.NavigateTo(typeof(SettingsViewModel).FullName, null, new DrillInNavigationTransitionInfo());
            }
            await Task.CompletedTask;
        });

        // 退出应用
        public IAsyncRelayCommand ExitCommand => new AsyncRelayCommand<TaskbarIcon>(async (param) =>
        {
            if (DownloadSchedulerService.DownloadingList.Count > 0 || DownloadSchedulerService.WaitingList.Count > 0)
            {
                // 窗口置前端
                WindowHelper.ShowAppWindow();

                ContentDialogResult result = await new ClosingWindowDialog().ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    await CloseApp(param);
                }
                if (result == ContentDialogResult.Secondary)
                {
                    if (NavigationService.Frame.CurrentSourcePageType != typeof(DownloadPage))
                    {
                        NavigationService.NavigateTo(typeof(DownloadViewModel).FullName, null, new DrillInNavigationTransitionInfo());
                    }
                    return;
                }
            }
            else
            {
                await CloseApp(param);
            }
        });

        /// <summary>
        /// 关闭应用并释放所有资源
        /// </summary>
        private async Task CloseApp(TaskbarIcon appTaskbarIcon)
        {
            App.MainWindow.Close();
            appTaskbarIcon.Dispose();
            await Aria2Service.CloseAria2Async();
            await DownloadSchedulerService.CloseDownloadMonitorAsync();
        }
    }
}
