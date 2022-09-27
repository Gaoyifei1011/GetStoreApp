using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Download;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.Extensions.Delegate;
using GetStoreApp.Helpers;
using GetStoreApp.Messages;
using GetStoreApp.UI.Dialogs;
using GetStoreApp.ViewModels.Pages;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;

namespace GetStoreApp.ViewModels.Window
{
    public class MainWindowViewModel : ObservableRecipient
    {
        private IAria2Service Aria2Service { get; } = IOCHelper.GetService<IAria2Service>();

        private IAppExitService AppExitService { get; } = IOCHelper.GetService<IAppExitService>();

        private IDownloadSchedulerService DownloadSchedulerService { get; } = IOCHelper.GetService<IDownloadSchedulerService>();

        private INavigationService NavigationService { get; } = IOCHelper.GetService<INavigationService>();

        /// <summary>
        /// 关闭窗口之后关闭其他服务
        /// </summary>
        public async void WindowClosed()
        {
            await Aria2Service.CloseAria2Async();
            await DownloadSchedulerService.CloseDownloadMonitorAsync();

            WeakReferenceMessenger.Default.Send(new TrayIconDisposeMessage(true));
        }

        /// <summary>
        /// 关闭窗口时，如果还有正在下载的任务，弹出对话框询问
        /// </summary>
        public async void WindowClosing(object sender, WindowClosingEventArgs args)
        {
            if (AppExitService.AppExit.InternalName == AppExitService.AppExitList[0].InternalName)
            {
                WindowHelper.HideAppWindow();
            }
            else
            {
                if (DownloadSchedulerService.DownloadingList.Count > 0 || DownloadSchedulerService.WaitingList.Count > 0)
                {
                    ContentDialogResult result = await new ClosingWindowDialog().ShowAsync();

                    if (result == ContentDialogResult.Primary)
                    {
                        args.TryCloseWindow();
                    }
                    else if (result == ContentDialogResult.Secondary)
                    {
                        if (NavigationService.Frame.CurrentSourcePageType != typeof(DownloadPage))
                        {
                            NavigationService.NavigateTo(typeof(DownloadViewModel).FullName, null, new DrillInNavigationTransitionInfo());
                        }
                    }
                }
                else
                {
                    args.TryCloseWindow();
                }
            }
        }
    }
}
