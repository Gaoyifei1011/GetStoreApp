using CommunityToolkit.WinUI.Notifications;
using GetStoreApp.Contracts.Services.Root;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.Helpers;
using GetStoreApp.ViewModels.Pages;
using GetStoreApp.Views;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.Windows.AppNotifications;
using System;
using System.Collections.Specialized;
using System.Web;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 应用通知服务
    /// </summary>
    public class AppNotificationService : IAppNotificationService
    {
        public IResourceService ResourceService { get; } = IOCHelper.GetService<IResourceService>();

        public INavigationService NavigationService { get; } = IOCHelper.GetService<INavigationService>();

        public INotificationService NotificationService { get; } = IOCHelper.GetService<INotificationService>();

        ~AppNotificationService()
        {
            Unregister();
        }

        public void Initialize()
        {
            AppNotificationManager.Default.NotificationInvoked += OnNotificationInvoked;

            AppNotificationManager.Default.Register();
        }

        /// <summary>
        /// 处理应用通知后的响应事件
        /// </summary>
        public async void OnNotificationInvoked(AppNotificationManager sender, AppNotificationActivatedEventArgs args)
        {
            // 打开网络设置（不涉及到UI主线程）
            if (ParseArguments(args.Argument)["AppNotifications"] == "CheckNetWorkConnection")
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:network"));
            }
            // 涉及到UI主线程
            else
            {
                App.MainWindow.DispatcherQueue.TryEnqueue(() =>
                {
                    // 先设置应用窗口的显示方式
                    WindowHelper.SetAppWindow();

                    // 根据点击通知获取到的参数来选择相应的操作
                    if (ParseArguments(args.Argument)["AppNotifications"] == "DownloadingNow" && NavigationService.Frame.CurrentSourcePageType != typeof(DownloadPage))
                    {
                        NavigationService.NavigateTo(typeof(DownloadViewModel).FullName, null, new DrillInNavigationTransitionInfo());
                    }

                    if (ParseArguments(args.Argument)["AppNotifications"] == "NotDownload" && NavigationService.Frame.CurrentSourcePageType != typeof(DownloadPage))
                    {
                        NavigationService.NavigateTo(typeof(DownloadViewModel).FullName, null, new DrillInNavigationTransitionInfo());
                    }

                    if (ParseArguments(args.Argument)["AppNotifications"] == "ViewDownloadPage" && NavigationService.Frame.CurrentSourcePageType != typeof(DownloadPage))
                    {
                        NavigationService.NavigateTo(typeof(DownloadViewModel).FullName, null, new DrillInNavigationTransitionInfo());
                    }

                    if (ParseArguments(args.Argument)["AppNotifications"] == "DownloadCompleted")
                    {
                        if (NavigationService.Frame.CurrentSourcePageType != typeof(DownloadPage))
                        {
                            NavigationService.NavigateTo(typeof(DownloadViewModel).FullName, null, new DrillInNavigationTransitionInfo());
                        }

                        //App.NavigationArgs = "DownloadCompleted";
                    }
                });
            }
        }

        /// <summary>
        /// 解析应用通知返回的参数
        /// </summary>
        private NameValueCollection ParseArguments(string arguments)
        {
            return HttpUtility.ParseQueryString(arguments);
        }

        /// <summary>
        /// 显示通知
        /// </summary>
        /// <param name="notificationType">显示通知的类型</param>
        public void Show(params string[] notification)
        {
            if (!NotificationService.AppNotification)
            {
                return;
            }

            if (notification.Length > 0)
            {
                if (notification[0] == "DownloadAborted")
                {
                    if (notification[1] == "DownloadingNow")
                    {
                        new ToastContentBuilder().AddArgument("AppNotifications", notification[1])
                            .AddText(ResourceService.GetLocalized("/Notification/OfflineMode"))

                            .AddText(ResourceService.GetLocalized("/Notification/DownloadingAborted"))

                            .AddButton(new ToastButton()
                                .SetContent(ResourceService.GetLocalized("/Notification/ViewDownloadPage"))
                                .AddArgument("AppNotifications", "ViewDownloadPage")
                                .SetBackgroundActivation())

                            .AddButton(new ToastButton()
                                .SetContent(ResourceService.GetLocalized("/Notification/CheckNetWorkConnection"))
                                .AddArgument("AppNotifications", "CheckNetWorkConnection")
                                .SetBackgroundActivation())
                            .Show();
                    }
                    else if (notification[1] == "NotDownload")
                    {
                        new ToastContentBuilder().AddArgument("AppNotifications", notification[1])
                            .AddText(ResourceService.GetLocalized("/Notification/OfflineMode"))

                            .AddText(ResourceService.GetLocalized("/Notification/NotDownload"))

                            .AddButton(new ToastButton()
                                .SetContent(ResourceService.GetLocalized("/Notification/CheckNetWorkConnection"))
                                .AddArgument("AppNotifications", "CheckNetWorkConnection")
                                .SetBackgroundActivation())
                            .Show();
                    }
                }
                else if (notification[0] == "DownloadCompleted")
                {
                    new ToastContentBuilder().AddArgument("AppNotifications", notification[0])
                        .AddText(ResourceService.GetLocalized("/Notification/DownloadCompleted"))

                        .AddText(ResourceService.GetLocalized("/Notification/DownloadCompletedDescription"))

                        .AddButton(new ToastButton()
                            .SetContent(ResourceService.GetLocalized("/Notification/ViewDownloadPage"))
                            .AddArgument("AppNotifications", "ViewDownloadPage")
                            .SetBackgroundActivation())
                        .Show();
                }
            }
        }

        /// <summary>
        /// 注销通知服务
        /// </summary>
        public void Unregister()
        {
            AppNotificationManager.Default.Unregister();
        }
    }
}
