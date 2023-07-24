using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.Services.Window;
using GetStoreApp.Views.Pages;
using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.UI.Notifications;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 应用通知服务
    /// </summary>
    public static class AppNotificationService
    {
        private static ToastNotifier AppToastNotifier { get; } = ToastNotificationManager.CreateToastNotifier();

        /// <summary>
        /// 处理应用通知
        /// </summary>
        public static async Task HandleAppNotificationAsync(string content)
        {
            string AppNotificationArguments = new WwwFormUrlDecoder(content).GetFirstValueByName("action");

            if (AppNotificationArguments is "CheckNetWorkConnection")
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:network"));
                Program.IsNeedAppLaunch = Application.Current is not null;
            }
            else if (AppNotificationArguments is "OpenDownloadFolder")
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
            else if (AppNotificationArguments is "OpenSettings")
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures"));
                Program.IsNeedAppLaunch = Application.Current is not null;
            }
            else if (AppNotificationArguments.Contains("InstallWithCommand"))
            {
                string[] splitList = AppNotificationArguments.Split(':');
                if (splitList.Length > 1)
                {
                    string appId = splitList[1];
                    unsafe
                    {
                        Kernel32Library.GetStartupInfo(out STARTUPINFO WinGetProcessStartupInfo);
                        WinGetProcessStartupInfo.lpReserved = null;
                        WinGetProcessStartupInfo.lpDesktop = null;
                        WinGetProcessStartupInfo.lpTitle = null;
                        WinGetProcessStartupInfo.dwX = 0;
                        WinGetProcessStartupInfo.dwY = 0;
                        WinGetProcessStartupInfo.dwXSize = 0;
                        WinGetProcessStartupInfo.dwYSize = 0;
                        WinGetProcessStartupInfo.dwXCountChars = 500;
                        WinGetProcessStartupInfo.dwYCountChars = 500;
                        WinGetProcessStartupInfo.dwFlags = STARTF.STARTF_USESHOWWINDOW;
                        WinGetProcessStartupInfo.wShowWindow = WindowShowStyle.SW_SHOW;
                        WinGetProcessStartupInfo.cbReserved2 = 0;
                        WinGetProcessStartupInfo.lpReserved2 = IntPtr.Zero;
                        WinGetProcessStartupInfo.cb = Marshal.SizeOf(typeof(STARTUPINFO));

                        bool createResult = Kernel32Library.CreateProcess(null, string.Format("winget install {0}", appId), IntPtr.Zero, IntPtr.Zero, false, CreateProcessFlags.CREATE_NEW_CONSOLE, IntPtr.Zero, null, ref WinGetProcessStartupInfo, out PROCESS_INFORMATION WinGetProcessInformation);

                        if (createResult)
                        {
                            if (WinGetProcessInformation.hProcess != IntPtr.Zero) Kernel32Library.CloseHandle(WinGetProcessInformation.hProcess);
                            if (WinGetProcessInformation.hThread != IntPtr.Zero) Kernel32Library.CloseHandle(WinGetProcessInformation.hThread);
                        }
                    }
                }
                Program.IsNeedAppLaunch = Application.Current is not null;
            }
            else
            {
                if (Program.IsAppLaunched)
                {
                    HandleMainThreadNotification(AppNotificationArguments);
                }
            }
        }

        /// <summary>
        /// 处理必须主线程才能处理的应用通知
        /// </summary>
        public static void HandleMainThreadNotification(string args)
        {
            Program.ApplicationRoot.MainWindow.DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, () =>
            {
                // 先设置应用窗口的显示方式
                Program.ApplicationRoot.MainWindow.Show();

                switch (args)
                {
                    // 正常打开应用
                    case "OpenApp":
                        {
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
        public static void Show(NotificationArgs notificationKey, params string[] notificationContent)
        {
            if (NotificationService.AppNotification)
            {
                switch (notificationKey)
                {
                    case NotificationArgs.DownloadAborted:
                        {
                            if (notificationContent.Length is 0) return;

                            // 有任务处于正在下载状态时被迫中断显示相应的通知
                            if (notificationContent[0] is "DownloadingNow")
                            {
                                XmlDocument notificationDocument = new XmlDocument();
                                notificationDocument.LoadXml(ResourceService.GetLocalized("Notification/DownloadingNowOfflineMode"));
                                ToastNotification notificaiton = new ToastNotification(notificationDocument);
                                AppToastNotifier.Show(notificaiton);
                            }

                            // 没有任务下载时显示相应的通知
                            else if (notificationContent[0] is "NotDownload")
                            {
                                XmlDocument notificationDocument = new XmlDocument();
                                notificationDocument.LoadXml(ResourceService.GetLocalized("Notification/NotDownloadOfflineMode"));
                                ToastNotification notificaiton = new ToastNotification(notificationDocument);
                                AppToastNotifier.Show(notificaiton);
                            }
                            break;
                        }

                    // 安装应用显示相应的通知
                    case NotificationArgs.InstallApp:
                        {
                            if (notificationContent.Length is 0) return;

                            // 成功安装应用通知
                            if (notificationContent[0] is "Successfully")
                            {
                                XmlDocument notificationDocument = new XmlDocument();
                                notificationDocument.LoadXml(string.Format(ResourceService.GetLocalized("Notification/InstallSuccessfully"), notificationContent[1]));
                                ToastNotification notificaiton = new ToastNotification(notificationDocument);
                                AppToastNotifier.Show(notificaiton);
                            }
                            else if (notificationContent[0] is "Error")
                            {
                                XmlDocument notificationDocument = new XmlDocument();
                                notificationDocument.LoadXml(string.Format(ResourceService.GetLocalized("Notification/InstallError"), notificationContent[1], notificationContent[2]));
                                ToastNotification notificaiton = new ToastNotification(notificationDocument);
                                AppToastNotifier.Show(notificaiton);
                            }
                            break;
                        }

                    // 所有任务下载完成时显示通知
                    case NotificationArgs.DownloadCompleted:
                        {
                            XmlDocument notificationDocument = new XmlDocument();
                            notificationDocument.LoadXml(ResourceService.GetLocalized("Notification/DownloadCompleted"));
                            ToastNotification notificaiton = new ToastNotification(notificationDocument);
                            AppToastNotifier.Show(notificaiton);
                            break;
                        }
                    // 应用安装成功通知
                    case NotificationArgs.InstallSuccessfully:
                        {
                            XmlDocument notificationDocument = new XmlDocument();
                            notificationDocument.LoadXml(string.Format(ResourceService.GetLocalized("Notification/WinGetInstallSuccessfully"), notificationContent[0]));
                            ToastNotification notificaiton = new ToastNotification(notificationDocument);
                            AppToastNotifier.Show(notificaiton);
                            break;
                        }
                    // 应用安装失败通知
                    case NotificationArgs.InstallFailed:
                        {
                            XmlDocument notificationDocument = new XmlDocument();
                            notificationDocument.LoadXml(string.Format(ResourceService.GetLocalized("Notification/WinGetInstallFailed"), notificationContent[0], notificationContent[1]));
                            ToastNotification notificaiton = new ToastNotification(notificationDocument);
                            AppToastNotifier.Show(notificaiton);
                            break;
                        }
                    // 应用卸载成功通知
                    case NotificationArgs.UnInstallSuccessfully:
                        {
                            XmlDocument notificationDocument = new XmlDocument();
                            notificationDocument.LoadXml(string.Format(ResourceService.GetLocalized("Notification/WinGetUnInstallSuccessfully"), notificationContent[0]));
                            ToastNotification notificaiton = new ToastNotification(notificationDocument);
                            AppToastNotifier.Show(notificaiton);
                            break;
                        }
                    // 应用卸载失败通知
                    case NotificationArgs.UnInstallFailed:
                        {
                            XmlDocument notificationDocument = new XmlDocument();
                            notificationDocument.LoadXml(string.Format(ResourceService.GetLocalized("Notification/WinGetUnInstallFailed"), notificationContent[0]));
                            ToastNotification notificaiton = new ToastNotification(notificationDocument);
                            AppToastNotifier.Show(notificaiton);
                            break;
                        }
                    // 应用升级成功通知
                    case NotificationArgs.UpgradeSuccessfully:
                        {
                            XmlDocument notificationDocument = new XmlDocument();
                            notificationDocument.LoadXml(string.Format(ResourceService.GetLocalized("Notification/WinGetUpgradeSuccessfully"), notificationContent[0]));
                            ToastNotification notificaiton = new ToastNotification(notificationDocument);
                            AppToastNotifier.Show(notificaiton);
                            break;
                        }
                    // 应用升级失败通知
                    case NotificationArgs.UpgradeFailed:
                        {
                            XmlDocument notificationDocument = new XmlDocument();
                            notificationDocument.LoadXml(string.Format(ResourceService.GetLocalized("Notification/WinGetUpgradeFailed"), notificationContent[0], notificationContent[1]));
                            ToastNotification notificaiton = new ToastNotification(notificationDocument);
                            AppToastNotifier.Show(notificaiton);
                            break;
                        }
                    default:
                        break;
                }
            }
        }
    }
}
