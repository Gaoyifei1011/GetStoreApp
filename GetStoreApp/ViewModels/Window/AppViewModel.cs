using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.Messaging;
using GetStoreApp.Extensions.SystemTray;
using GetStoreApp.Helpers.Window;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Window;
using GetStoreApp.UI.Dialogs.Common;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using Windows.UI.StartScreen;

namespace GetStoreApp.ViewModels.Window
{
    public sealed class AppViewModel
    {
        // 隐藏 / 显示窗口
        public IRelayCommand ShowOrHideWindowCommand => new RelayCommand(() =>
        {
            // 隐藏窗口
            if (Program.ApplicationRoot.MainWindow.Visible)
            {
                WindowHelper.HideAppWindow();
            }
            // 显示窗口
            else
            {
                WindowHelper.ShowAppWindow();
            }
        });

        // 打开设置
        public IRelayCommand SettingsCommand => new RelayCommand(() =>
        {
            // 窗口置前端
            WindowHelper.ShowAppWindow();

            if (NavigationService.GetCurrentPageType() != typeof(SettingsPage))
            {
                NavigationService.NavigateTo(typeof(SettingsPage));
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
                if (!Program.ApplicationRoot.IsDialogOpening)
                {
                    Program.ApplicationRoot.IsDialogOpening = true;

                    ContentDialogResult result = await new ClosingWindowDialog().ShowAsync();

                    if (result == ContentDialogResult.Primary)
                    {
                        await CloseApp();
                    }
                    else if (result == ContentDialogResult.Secondary)
                    {
                        if (NavigationService.GetCurrentPageType() != typeof(DownloadPage))
                        {
                            NavigationService.NavigateTo(typeof(DownloadPage));
                        }
                    }

                    Program.ApplicationRoot.IsDialogOpening = false;
                }
            }
            else
            {
                await CloseApp();
            }
        });

        /// <summary>
        /// 处理应用程序未知异常处理
        /// </summary>
        public async void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs args)
        {
            args.Handled = true;

            await DownloadSchedulerService.CloseDownloadSchedulerAsync();
            await Aria2Service.CloseAria2Async();
            Messenger.Default.Send(true, MessageToken.WindowClosed);
        }

        /// <summary>
        /// 窗口激活后配置其他设置
        /// </summary>
        public async Task StartupAsync()
        {
            // 设置应用主题
            await ThemeService.SetAppThemeAsync();

            // 设置应用背景色
            await BackdropService.SetAppBackdropAsync();

            // 设置应用置顶状态
            await TopMostService.SetAppTopMostAsync();

            // 初始化下载监控服务
            await DownloadSchedulerService.InitializeDownloadSchedulerAsync();

            // 初始化Aria2配置文件信息
            await Aria2Service.InitializeAria2ConfAsync();

            // 启动Aria2下载服务（该服务会在后台长时间运行）
            await Aria2Service.StartAria2Async();
        }

        /// <summary>
        /// 移除用户主动删除的条目
        /// </summary>
        public void RemoveUnusedItems()
        {
            // 如果某一条目被用户主动删除，应用初始化时则自动删除该条目
            for (int index = 0; index < Program.ApplicationRoot.TaskbarJumpList.Items.Count; index++)
            {
                if (Program.ApplicationRoot.TaskbarJumpList.Items[index].RemovedByUser)
                {
                    Program.ApplicationRoot.TaskbarJumpList.Items.RemoveAt(index);
                    index--;
                }
            }
        }

        /// <summary>
        /// 更新跳转列表组名
        /// </summary>
        public void UpdateJumpListGroupName()
        {
            foreach (JumpListItem jumpListItem in Program.ApplicationRoot.TaskbarJumpList.Items)
            {
                jumpListItem.GroupName = AppJumpList.GroupName;
            }
        }

        /// <summary>
        /// 关闭应用并释放所有资源
        /// </summary>
        private async Task CloseApp()
        {
            await DownloadSchedulerService.CloseDownloadSchedulerAsync();
            await Aria2Service.CloseAria2Async();
            Messenger.Default.Send(true, MessageToken.WindowClosed);
            AppNotificationService.Unregister();
            BackdropHelper.ReleaseBackdrop();
            Environment.Exit(Convert.ToInt32(AppExitCode.Successfully));
        }
    }
}
