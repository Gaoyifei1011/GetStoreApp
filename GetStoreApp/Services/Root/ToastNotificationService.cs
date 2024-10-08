﻿using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Controls.Settings;
using GetStoreApp.Views.Pages;
using GetStoreApp.Views.Windows;
using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using Microsoft.UI.Xaml;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;
using Windows.System;
using Windows.UI.Notifications;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 应用通知服务
    /// </summary>
    public static class ToastNotificationService
    {
        private static readonly ToastNotifier appToastNotifier = ToastNotificationManager.CreateToastNotifier();

        /// <summary>
        /// 处理应用通知
        /// </summary>
        public static async Task HandleToastNotificationAsync(string content)
        {
            string notificationArgs = new WwwFormUrlDecoder(content).GetFirstValueByName("action");

            if (notificationArgs is "CheckNetWorkConnection")
            {
                await Launcher.LaunchUriAsync(new Uri("ms-settings:network"));
                Environment.Exit(0);
            }
            else if (notificationArgs is "OpenDownloadFolder")
            {
                string wingetTempPath = Path.Combine(Path.GetTempPath(), "WinGet");
                if (Directory.Exists(wingetTempPath))
                {
                    await Launcher.LaunchFolderPathAsync(wingetTempPath);
                }
                else
                {
                    await Launcher.LaunchFolderPathAsync(Path.GetTempPath());
                }
                Environment.Exit(0);
            }
            else if (notificationArgs is "OpenSettings")
            {
                await Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures"));
                Environment.Exit(0);
            }
            else if (notificationArgs.Contains("InstallWithCommand"))
            {
                string[] splitList = notificationArgs.Split(':');
                if (splitList.Length > 1)
                {
                    string appId = splitList[1];
                    Shell32Library.ShellExecute(IntPtr.Zero, "open", "winget.exe", string.Format("install {0}", appId), null, WindowShowStyle.SW_SHOWNORMAL);
                }
                Environment.Exit(0);
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
        public static void Show(NotificationKind notificationKind, params string[] notificationContent)
        {
            if (NotificationService.AppNotification)
            {
                try
                {
                    switch (notificationKind)
                    {
                        case NotificationKind.AppUpdate:
                            {
                                XmlDocument notificationDocument = new();
                                notificationDocument.LoadXml(string.Format(ResourceService.GetLocalized("Notification/AppUpdateSuccessfully"), notificationContent[0]));
                                ToastNotification notificaiton = new(notificationDocument);
                                appToastNotifier.Show(notificaiton);
                                break;
                            }

                        // 安装应用显示相应的通知
                        case NotificationKind.InstallApp:
                            {
                                if (notificationContent.Length is 0) return;

                                // 成功安装应用通知
                                if (notificationContent[0] is "Successfully")
                                {
                                    XmlDocument notificationDocument = new();
                                    notificationDocument.LoadXml(string.Format(ResourceService.GetLocalized("Notification/InstallSuccessfully"), notificationContent[1]));
                                    ToastNotification notificaiton = new(notificationDocument);
                                    appToastNotifier.Show(notificaiton);
                                }
                                else if (notificationContent[0] is "Error")
                                {
                                    XmlDocument notificationDocument = new();
                                    notificationDocument.LoadXml(string.Format(ResourceService.GetLocalized("Notification/InstallError"), notificationContent[1], notificationContent[2]));
                                    ToastNotification notificaiton = new(notificationDocument);
                                    appToastNotifier.Show(notificaiton);
                                }
                                break;
                            }

                        // 所有任务下载完成时显示通知
                        case NotificationKind.DownloadCompleted:
                            {
                                XmlDocument notificationDocument = new();
                                notificationDocument.LoadXml(ResourceService.GetLocalized("Notification/DownloadCompleted"));
                                ToastNotification notificaiton = new(notificationDocument);
                                appToastNotifier.Show(notificaiton);
                                break;
                            }
                        // 管理员身份运行应用显示通知
                        case NotificationKind.RunAsAdministrator:
                            {
                                XmlDocument notificationDocument = new();
                                notificationDocument.LoadXml(ResourceService.GetLocalized("Notification/ElevatedRunning"));
                                ToastNotification notificaiton = new(notificationDocument);
                                appToastNotifier.Show(notificaiton);
                                break;
                            }
                        // 网络连接异常通知
                        case NotificationKind.NetworkError:
                            {
                                XmlDocument notificationDocument = new();
                                notificationDocument.LoadXml(ResourceService.GetLocalized("Notification/NetworkError"));
                                ToastNotification notificaiton = new(notificationDocument);
                                appToastNotifier.Show(notificaiton);
                                break;
                            }
                        // UWP 应用卸载成功通知
                        case NotificationKind.UWPUnInstallSuccessfully:
                            {
                                XmlDocument notificationDocument = new();
                                notificationDocument.LoadXml(string.Format(ResourceService.GetLocalized("Notification/UWPUnInstallSuccessfully"), notificationContent[0]));
                                ToastNotification notificaiton = new(notificationDocument);
                                appToastNotifier.Show(notificaiton);
                                break;
                            }
                        // UWP 应用卸载失败通知
                        case NotificationKind.UWPUnInstallFailed:
                            {
                                XmlDocument notificationDocument = new();
                                notificationDocument.LoadXml(string.Format(ResourceService.GetLocalized("Notification/UWPUnInstallFailed"), notificationContent[0], notificationContent[1], notificationContent[2]));
                                ToastNotification notificaiton = new(notificationDocument);
                                appToastNotifier.Show(notificaiton);
                                break;
                            }
                        // WinGet 应用安装成功通知
                        case NotificationKind.WinGetInstallSuccessfully:
                            {
                                XmlDocument notificationDocument = new();
                                notificationDocument.LoadXml(string.Format(ResourceService.GetLocalized("Notification/WinGetInstallSuccessfully"), notificationContent[0]));
                                ToastNotification notificaiton = new(notificationDocument);
                                appToastNotifier.Show(notificaiton);
                                break;
                            }
                        // WinGet 应用安装失败通知
                        case NotificationKind.WinGetInstallFailed:
                            {
                                XmlDocument notificationDocument = new();
                                notificationDocument.LoadXml(string.Format(ResourceService.GetLocalized("Notification/WinGetInstallFailed"), notificationContent[0], notificationContent[1]));
                                ToastNotification notificaiton = new(notificationDocument);
                                appToastNotifier.Show(notificaiton);
                                break;
                            }
                        // WinGet 应用卸载成功通知
                        case NotificationKind.WinGetUnInstallSuccessfully:
                            {
                                XmlDocument notificationDocument = new();
                                notificationDocument.LoadXml(string.Format(ResourceService.GetLocalized("Notification/WinGetUnInstallSuccessfully"), notificationContent[0]));
                                ToastNotification notificaiton = new(notificationDocument);
                                appToastNotifier.Show(notificaiton);
                                break;
                            }
                        // WinGet 应用卸载失败通知
                        case NotificationKind.WinGetUnInstallFailed:
                            {
                                XmlDocument notificationDocument = new();
                                notificationDocument.LoadXml(string.Format(ResourceService.GetLocalized("Notification/WinGetUnInstallFailed"), notificationContent[0]));
                                ToastNotification notificaiton = new(notificationDocument);
                                appToastNotifier.Show(notificaiton);
                                break;
                            }
                        // WinGet 应用升级成功通知
                        case NotificationKind.WinGetUpgradeSuccessfully:
                            {
                                XmlDocument notificationDocument = new();
                                notificationDocument.LoadXml(string.Format(ResourceService.GetLocalized("Notification/WinGetUpgradeSuccessfully"), notificationContent[0]));
                                ToastNotification notificaiton = new(notificationDocument);
                                appToastNotifier.Show(notificaiton);
                                break;
                            }
                        // WinGet 应用升级失败通知
                        case NotificationKind.WinGetUpgradeFailed:
                            {
                                XmlDocument notificationDocument = new();
                                notificationDocument.LoadXml(string.Format(ResourceService.GetLocalized("Notification/WinGetUpgradeFailed"), notificationContent[0], notificationContent[1]));
                                ToastNotification notificaiton = new(notificationDocument);
                                appToastNotifier.Show(notificaiton);
                                break;
                            }
                    }
                }
                catch (Exception e)
                {
                    LogService.WriteLog(LoggingLevel.Error, string.Format("Show notification {0} failed", notificationKind.ToString()), e);
                }
            }
        }
    }
}
