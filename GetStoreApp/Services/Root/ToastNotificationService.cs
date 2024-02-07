using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Views.Pages;
using GetStoreApp.Views.Windows;
using Microsoft.UI.Content;
using Microsoft.UI.Xaml;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.UI.Notifications;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 应用通知服务
    /// </summary>
    public static class ToastNotificationService
    {
        private static ToastNotifier appToastNotifier = ToastNotificationManager.CreateToastNotifier();

        /// <summary>
        /// 处理应用通知
        /// </summary>
        public static async Task HandleToastNotificationAsync(string content)
        {
            string notificationArgs = new WwwFormUrlDecoder(content).GetFirstValueByName("action");

            if (notificationArgs is "CheckNetWorkConnection")
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:network"));
                Program.IsNeedAppLaunch = Application.Current is not null;
            }
            else if (notificationArgs is "OpenDownloadFolder")
            {
                string wingetTempPath = Path.Combine(Path.GetTempPath(), "WinGet");
                if (Directory.Exists(wingetTempPath))
                {
                    await Windows.System.Launcher.LaunchFolderPathAsync(wingetTempPath);
                }
                else
                {
                    await Windows.System.Launcher.LaunchFolderPathAsync(Path.GetTempPath());
                }
                Program.IsNeedAppLaunch = Application.Current is not null;
            }
            else if (notificationArgs is "OpenSettings")
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures"));
                Program.IsNeedAppLaunch = Application.Current is not null;
            }
            else if (notificationArgs.Contains("InstallWithCommand"))
            {
                string[] splitList = notificationArgs.Split(':');
                if (splitList.Length > 1)
                {
                    string appId = splitList[1];
                    ProcessStarter.StartProcess("winget.exe", string.Format("install {0}", appId), out _);
                }
                Program.IsNeedAppLaunch = Application.Current is not null;
            }
            else if (notificationArgs.Contains("OpenApp"))
            {
                if (Application.Current is null)
                {
                    DesktopLaunchService.InitializePage = typeof(StorePage);
                }
                else
                {
                    MainWindow.Current.DispatcherQueue.TryEnqueue(() =>
                    {
                        if (MainWindow.Current.GetCurrentPageType() != typeof(StorePage))
                        {
                            MainWindow.Current.NavigateTo(typeof(StorePage));
                        }
                    });
                }
            }
            else if (notificationArgs.Contains("ViewDownloadPage"))
            {
                if (Application.Current is null)
                {
                    DesktopLaunchService.InitializePage = typeof(DownloadPage);
                }
                else
                {
                    MainWindow.Current.DispatcherQueue.TryEnqueue(() =>
                    {
                        if (MainWindow.Current.GetCurrentPageType() != typeof(DownloadPage))
                        {
                            MainWindow.Current.NavigateTo(typeof(DownloadPage));
                        }
                    });
                }
            }
        }

        /// <summary>
        /// 显示通知
        /// </summary>
        public static void Show(NotificationKind notificationKey, params string[] notificationContent)
        {
            if (NotificationService.AppNotification)
            {
                switch (notificationKey)
                {
                    // 管理员身份运行应用显示通知
                    case NotificationKind.RunAsAdministrator:
                        {
                            XmlDocument notificationDocument = new XmlDocument();
                            notificationDocument.LoadXml(ResourceService.GetLocalized("Notification/ElevatedRunning"));
                            ToastNotification notificaiton = new ToastNotification(notificationDocument);
                            appToastNotifier.Show(notificaiton);
                            break;
                        }
                    case NotificationKind.DownloadAborted:
                        {
                            if (notificationContent.Length is 0) return;

                            // 有任务处于正在下载状态时被迫中断显示相应的通知
                            if (notificationContent[0] is "DownloadingNow")
                            {
                                XmlDocument notificationDocument = new XmlDocument();
                                notificationDocument.LoadXml(ResourceService.GetLocalized("Notification/DownloadingNowOfflineMode"));
                                ToastNotification notificaiton = new ToastNotification(notificationDocument);
                                appToastNotifier.Show(notificaiton);
                            }

                            // 没有任务下载时显示相应的通知
                            else if (notificationContent[0] is "NotDownload")
                            {
                                XmlDocument notificationDocument = new XmlDocument();
                                notificationDocument.LoadXml(ResourceService.GetLocalized("Notification/NotDownloadOfflineMode"));
                                ToastNotification notificaiton = new ToastNotification(notificationDocument);
                                appToastNotifier.Show(notificaiton);
                            }
                            break;
                        }

                    // 安装应用显示相应的通知
                    case NotificationKind.InstallApp:
                        {
                            if (notificationContent.Length is 0) return;

                            // 成功安装应用通知
                            if (notificationContent[0] is "Successfully")
                            {
                                XmlDocument notificationDocument = new XmlDocument();
                                notificationDocument.LoadXml(string.Format(ResourceService.GetLocalized("Notification/InstallSuccessfully"), notificationContent[1]));
                                ToastNotification notificaiton = new ToastNotification(notificationDocument);
                                appToastNotifier.Show(notificaiton);
                            }
                            else if (notificationContent[0] is "Error")
                            {
                                XmlDocument notificationDocument = new XmlDocument();
                                notificationDocument.LoadXml(string.Format(ResourceService.GetLocalized("Notification/InstallError"), notificationContent[1], notificationContent[2]));
                                ToastNotification notificaiton = new ToastNotification(notificationDocument);
                                appToastNotifier.Show(notificaiton);
                            }
                            break;
                        }

                    // 所有任务下载完成时显示通知
                    case NotificationKind.DownloadCompleted:
                        {
                            XmlDocument notificationDocument = new XmlDocument();
                            notificationDocument.LoadXml(ResourceService.GetLocalized("Notification/DownloadCompleted"));
                            ToastNotification notificaiton = new ToastNotification(notificationDocument);
                            appToastNotifier.Show(notificaiton);
                            break;
                        }
                    // UWP 应用卸载成功通知
                    case NotificationKind.UWPUnInstallSuccessfully:
                        {
                            XmlDocument notificationDocument = new XmlDocument();
                            notificationDocument.LoadXml(string.Format(ResourceService.GetLocalized("Notification/UWPUnInstallSuccessfully"), notificationContent[0]));
                            ToastNotification notificaiton = new ToastNotification(notificationDocument);
                            appToastNotifier.Show(notificaiton);
                            break;
                        }
                    // UWP 应用卸载失败通知
                    case NotificationKind.UWPUnInstallFailed:
                        {
                            XmlDocument notificationDocument = new XmlDocument();
                            notificationDocument.LoadXml(string.Format(ResourceService.GetLocalized("Notification/UWPUnInstallFailed"), notificationContent[0], notificationContent[1], notificationContent[2]));
                            ToastNotification notificaiton = new ToastNotification(notificationDocument);
                            appToastNotifier.Show(notificaiton);
                            break;
                        }
                    // WinGet 应用安装成功通知
                    case NotificationKind.WinGetInstallSuccessfully:
                        {
                            XmlDocument notificationDocument = new XmlDocument();
                            notificationDocument.LoadXml(string.Format(ResourceService.GetLocalized("Notification/WinGetInstallSuccessfully"), notificationContent[0]));
                            ToastNotification notificaiton = new ToastNotification(notificationDocument);
                            appToastNotifier.Show(notificaiton);
                            break;
                        }
                    // WinGet 应用安装失败通知
                    case NotificationKind.WinGetInstallFailed:
                        {
                            XmlDocument notificationDocument = new XmlDocument();
                            notificationDocument.LoadXml(string.Format(ResourceService.GetLocalized("Notification/WinGetInstallFailed"), notificationContent[0], notificationContent[1]));
                            ToastNotification notificaiton = new ToastNotification(notificationDocument);
                            appToastNotifier.Show(notificaiton);
                            break;
                        }
                    // WinGet 应用卸载成功通知
                    case NotificationKind.WinGetUnInstallSuccessfully:
                        {
                            XmlDocument notificationDocument = new XmlDocument();
                            notificationDocument.LoadXml(string.Format(ResourceService.GetLocalized("Notification/WinGetUnInstallSuccessfully"), notificationContent[0]));
                            ToastNotification notificaiton = new ToastNotification(notificationDocument);
                            appToastNotifier.Show(notificaiton);
                            break;
                        }
                    // WinGet 应用卸载失败通知
                    case NotificationKind.WinGetUnInstallFailed:
                        {
                            XmlDocument notificationDocument = new XmlDocument();
                            notificationDocument.LoadXml(string.Format(ResourceService.GetLocalized("Notification/WinGetUnInstallFailed"), notificationContent[0]));
                            ToastNotification notificaiton = new ToastNotification(notificationDocument);
                            appToastNotifier.Show(notificaiton);
                            break;
                        }
                    // WinGet 应用升级成功通知
                    case NotificationKind.WinGetUpgradeSuccessfully:
                        {
                            XmlDocument notificationDocument = new XmlDocument();
                            notificationDocument.LoadXml(string.Format(ResourceService.GetLocalized("Notification/WinGetUpgradeSuccessfully"), notificationContent[0]));
                            ToastNotification notificaiton = new ToastNotification(notificationDocument);
                            appToastNotifier.Show(notificaiton);
                            break;
                        }
                    // WinGet 应用升级失败通知
                    case NotificationKind.WinGetUpgradeFailed:
                        {
                            XmlDocument notificationDocument = new XmlDocument();
                            notificationDocument.LoadXml(string.Format(ResourceService.GetLocalized("Notification/WinGetUpgradeFailed"), notificationContent[0], notificationContent[1]));
                            ToastNotification notificaiton = new ToastNotification(notificationDocument);
                            appToastNotifier.Show(notificaiton);
                            break;
                        }
                    case NotificationKind.AppUpdate:
                        {
                            XmlDocument notificationDocument = new XmlDocument();
                            notificationDocument.LoadXml(string.Format(ResourceService.GetLocalized("Notification/AppUpdateSuccessfully"), notificationContent[0]));
                            ToastNotification notificaiton = new ToastNotification(notificationDocument);
                            appToastNotifier.Show(notificaiton);
                            break;
                        }
                    default:
                        break;
                }
            }
        }
    }
}
