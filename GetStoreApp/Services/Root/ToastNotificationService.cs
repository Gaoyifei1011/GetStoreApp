using GetStoreApp.Extensions.DataType.Enums;
using Microsoft.Windows.AppNotifications;
using System;
using System.Collections.Generic;
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
        public static async Task HandleToastNotificationAsync(string content)
        {
            string notificationArgs = new WwwFormUrlDecoder(content).GetFirstValueByName("action");

            if (notificationArgs is "CheckNetWorkConnection")
            {
                await Launcher.LaunchUriAsync(new Uri("ms-settings:network"));
                Environment.Exit(Environment.ExitCode);
            }
            else if (notificationArgs.Contains("OpenDownloadFolder"))
            {
                string[] splitList = notificationArgs.Split(':');
                if (splitList.Length > 1)
                {
                    if (Directory.Exists(splitList[1]))
                    {
                        await Launcher.LaunchFolderPathAsync(splitList[1]);
                    }
                    else
                    {
                        await Launcher.LaunchFolderPathAsync(Path.GetTempPath());
                    }
                }

                Environment.Exit(Environment.ExitCode);
            }
            else if (notificationArgs is "OpenSettings")
            {
                await Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures"));
                Environment.Exit(Environment.ExitCode);
            }
            else if (notificationArgs.Contains("OpenApp"))
            {
                List<string> dataList = [];
                dataList.Add("OpenApp");
                ResultService.SaveResult(StorageDataKind.ToastNotification, dataList);
            }
            else if (notificationArgs.Contains("ViewDownloadPage"))
            {
                List<string> dataList = [];
                dataList.Add("ViewDownloadPage");
                ResultService.SaveResult(StorageDataKind.ToastNotification, dataList);
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
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreApp), nameof(ToastNotificationService), nameof(Show), 1, e);
            }
        }
    }
}
