using GetStoreApp.Helpers.Window;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.Services.Window;
using GetStoreApp.Views.Pages;
using GetStoreApp.Views.Window;
using Microsoft.UI.Dispatching;
using Microsoft.Windows.AppNotifications;
using System;
using System.Threading.Tasks;
using Windows.Foundation;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 应用通知服务
    /// </summary>
    public static class AppNotificationService
    {
        public static void Initialize()
        {
            AppNotificationManager.Default.NotificationInvoked += OnNotificationInvoked;

            AppNotificationManager.Default.Register();
        }

        /// <summary>
        /// 处理应用通知后的响应事件
        /// </summary>
        public static void OnNotificationInvoked(AppNotificationManager sender, AppNotificationActivatedEventArgs args)
        {
            HandleAppNotification(args);
        }

        /// <summary>
        /// 处理应用通知
        /// </summary>
        public static void HandleAppNotification(AppNotificationActivatedEventArgs args)
        {
            MainWindow.Current.DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, async () =>
            {
                while (NavigationService.NavigationFrame is null)
                {
                    await Task.Delay(500);
                }

                // 先设置应用窗口的显示方式
                WindowHelper.ShowAppWindow();

                switch (new WwwFormUrlDecoder(args.Argument).GetFirstValueByName("action"))
                {
                    case "OpenApp":
                        {
                            break;
                        }
                    case "CheckNetWorkConnection":
                        {
                            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:network"));
                            break;
                        }
                    case "DownloadingNow":
                        {
                            if (NavigationService.GetCurrentPageType() != typeof(DownloadPage))
                            {
                                NavigationService.NavigateTo(typeof(DownloadPage));
                            }
                            break;
                        }
                    case "NotDownload":
                        {
                            if (NavigationService.GetCurrentPageType() != typeof(DownloadPage))
                            {
                                NavigationService.NavigateTo(typeof(DownloadPage));
                            }
                            break;
                        }
                    case "ViewDownloadPage":
                        {
                            if (NavigationService.GetCurrentPageType() != typeof(DownloadPage))
                            {
                                NavigationService.NavigateTo(typeof(DownloadPage));
                            }
                            break;
                        }
                    case "DownloadCompleted":
                        {
                            if (NavigationService.GetCurrentPageType() != typeof(DownloadPage))
                            {
                                NavigationService.NavigateTo(typeof(DownloadPage));
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            });
        }

        /// <summary>
        /// 显示通知
        /// </summary>
        public static void Show(string notificationKey, params string[] notificationContent)
        {
            if (!NotificationService.AppNotification)
            {
                return;
            }

            AppNotification notification;

            switch (notificationKey)
            {
                case "DownloadAborted":
                    {
                        if (notificationContent.Length == 0)
                        {
                            return;
                        }

                        // 有任务处于正在下载状态时被迫中断显示相应的通知
                        if (notificationContent[0] == "Downloading")
                        {
                            notification = new AppNotification(ResourceService.GetLocalized("/Notification/DownloadingNowOfflineMode"));
                            notification.ExpiresOnReboot = true;
                            AppNotificationManager.Default.Show(notification);
                        }

                        // 没有任务下载时显示相应的通知
                        else if (notificationContent[0] == "NotDownload")
                        {
                            notification = new AppNotification(ResourceService.GetLocalized("/Notification/NotDownloadOfflineMode"));
                            notification.ExpiresOnReboot = true;
                            AppNotificationManager.Default.Show(notification);
                        }
                        break;
                    }

                // 安装应用显示相应的通知
                case "InstallApp":
                    {
                        if (notificationContent.Length == 0)
                        {
                            return;
                        }

                        // 成功安装应用通知
                        if (notificationContent[0] == "Successfully")
                        {
                            notification = new AppNotification(string.Format(ResourceService.GetLocalized("/Notification/InstallSuccessfully"), notificationContent[1]));
                            notification.ExpiresOnReboot = true;
                            AppNotificationManager.Default.Show(notification);
                        }
                        else if (notificationContent[0] == "Error")
                        {
                            notification = new AppNotification(string.Format(ResourceService.GetLocalized("/Notification/InstallError"), notificationContent[1], notificationContent[2]));
                            notification.ExpiresOnReboot = true;
                            AppNotificationManager.Default.Show(notification);
                        }
                        break;
                    }

                // 所有任务下载完成时显示通知
                case "DownloadCompleted":
                    {
                        notification = new AppNotification(ResourceService.GetLocalized("/Notification/DownloadCompleted"));
                        notification.ExpiresOnReboot = true;
                        AppNotificationManager.Default.Show(notification);
                        break;
                    }
                default:
                    break;
            }
        }

        /// <summary>
        /// 注销通知服务
        /// </summary>
        public static void Unregister()
        {
            AppNotificationManager.Default.NotificationInvoked -= OnNotificationInvoked;
            AppNotificationManager.Default.Unregister();
        }
    }
}
