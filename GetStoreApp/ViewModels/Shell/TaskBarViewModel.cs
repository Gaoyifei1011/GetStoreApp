using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Controls.Download;
using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Helpers.Window;
using GetStoreApp.Messages;
using GetStoreApp.UI.Dialogs.ContentDialogs.Common;
using GetStoreApp.ViewModels.Pages;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Shell
{
    public class TaskBarViewModel : ObservableRecipient
    {
        private IAria2Service Aria2Service { get; } = ContainerHelper.GetInstance<IAria2Service>();

        private IDownloadSchedulerService DownloadSchedulerService { get; } = ContainerHelper.GetInstance<IDownloadSchedulerService>();

        private INavigationService NavigationService { get; } = ContainerHelper.GetInstance<INavigationService>();

        // 隐藏 / 显示窗口
        public IRelayCommand ShowOrHideWindowCommand => new RelayCommand(() =>
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
        });

        public IRelayCommand SettingsCommand => new RelayCommand(() =>
        {
            // 窗口置前端
            WindowHelper.ShowAppWindow();

            if (NavigationService.Frame.CurrentSourcePageType != typeof(SettingsPage))
            {
                NavigationService.NavigateTo(typeof(SettingsViewModel).FullName, null, new DrillInNavigationTransitionInfo(), false);
            }
        });

        // 退出应用
        public IRelayCommand ExitCommand => new RelayCommand(async () =>
        {
            // 下载队列存在任务时，弹出对话窗口确认是否要关闭窗口
            if (DownloadSchedulerService.DownloadingList.Count > 0 || DownloadSchedulerService.WaitingList.Count > 0)
            {
                // 窗口置前端
                WindowHelper.ShowAppWindow();

                // 关闭窗口提示对话框是否已经处于打开状态，如果是，不再弹出
                if (!App.IsDialogOpening)
                {
                    App.IsDialogOpening = true;

                    ContentDialogResult result = await new ClosingWindowDialog().ShowAsync();

                    if (result == ContentDialogResult.Primary)
                    {
                        await CloseApp();
                    }
                    else if (result == ContentDialogResult.Secondary)
                    {
                        if (NavigationService.Frame.CurrentSourcePageType != typeof(DownloadPage))
                        {
                            NavigationService.NavigateTo(typeof(DownloadViewModel).FullName, null, new DrillInNavigationTransitionInfo(), false);
                        }
                    }

                    App.IsDialogOpening = false;
                }
            }
            else
            {
                await CloseApp();
            }
        });

        /// <summary>
        /// 关闭应用并释放所有资源
        /// </summary>
        private async Task CloseApp()
        {
            await DownloadSchedulerService.CloseDownloadSchedulerAsync();
            await Aria2Service.CloseAria2Async();
            WeakReferenceMessenger.Default.Send(new TrayIconDisposeMessage(true));
            Environment.Exit(Convert.ToInt32(AppExitCode.Successfully));
        }
    }
}
