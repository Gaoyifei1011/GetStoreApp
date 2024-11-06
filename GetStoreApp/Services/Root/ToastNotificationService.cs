using GetStoreApp.Views.Pages;
using GetStoreApp.Views.Windows;
using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using GetStoreApp.WindowsAPI.PInvoke.User32;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppNotifications;
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
        public static ToastNotifier AppToastNotifier { get; } = ToastNotificationManager.CreateToastNotifier();

        /// <summary>
        /// 处理应用通知
        /// </summary>
        public static async Task HandleToastNotificationAsync(string content, bool isLaunched)
        {
            string notificationArgs = new WwwFormUrlDecoder(content).GetFirstValueByName("action");

            if (notificationArgs is "CheckNetWorkConnection")
            {
                await Launcher.LaunchUriAsync(new Uri("ms-settings:network"));
                if (!isLaunched)
                {
                    Environment.Exit(0);
                }
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

                if (!isLaunched)
                {
                    Environment.Exit(0);
                }
            }
            else if (notificationArgs is "OpenSettings")
            {
                await Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures"));
                if (!isLaunched)
                {
                    Environment.Exit(0);
                }
            }
            else if (notificationArgs.Contains("InstallWithCommand"))
            {
                string[] splitList = notificationArgs.Split(':');
                if (splitList.Length > 1)
                {
                    string appId = splitList[1];
                    Shell32Library.ShellExecute(IntPtr.Zero, "open", "winget.exe", string.Format("install {0}", appId), null, WindowShowStyle.SW_SHOWNORMAL);
                }
                if (!isLaunched)
                {
                    Environment.Exit(0);
                }
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
        public static void Show(AppNotification appNotification)
        {
            try
            {
                XmlDocument notificationDocument = new();
                notificationDocument.LoadXml(appNotification.Payload);
                ToastNotification notificaiton = new(notificationDocument);
                AppToastNotifier.Show(notificaiton);
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, "Show notification failed", e);
            }
        }
    }
}
