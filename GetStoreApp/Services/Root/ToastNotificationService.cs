using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.WindowsAPI.PInvoke.Shell32;
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
            else if (notificationArgs.Contains("DownloadWithCommand"))
            {
                string[] splitList = notificationArgs.Split(':');
                if (splitList.Length is 3)
                {
                    string appId = splitList[1];
                    string path = splitList[2];
                    Shell32Library.ShellExecute(IntPtr.Zero, "open", "winget.exe", string.Format(@"download {0} -d ""{1}""", appId, path), null, WindowShowStyle.SW_SHOWNORMAL);
                }
                else if (splitList.Length is 4)
                {
                    string appId = splitList[1];
                    string source = splitList[2];
                    string path = splitList[3];
                    Shell32Library.ShellExecute(IntPtr.Zero, "open", "winget.exe", string.Format(@"download {0} -s ""{1}"" -d ""{2}""", appId, source, path), null, WindowShowStyle.SW_SHOWNORMAL);
                }
                Environment.Exit(Environment.ExitCode);
            }
            else if (notificationArgs.Contains("InstallWithCommand"))
            {
                string[] splitList = notificationArgs.Split(':');
                if (splitList.Length is 2)
                {
                    string appId = splitList[1];
                    Shell32Library.ShellExecute(IntPtr.Zero, "open", "winget.exe", string.Format("install {0}", appId), null, WindowShowStyle.SW_SHOWNORMAL);
                }
                else if (splitList.Length is 3)
                {
                    string appId = splitList[1];
                    string path = splitList[2];
                    Shell32Library.ShellExecute(IntPtr.Zero, "open", "winget.exe", string.Format(@"install {0} -s ""{1}""", appId, path), null, WindowShowStyle.SW_SHOWNORMAL);
                }
                Environment.Exit(Environment.ExitCode);
            }
            else if (notificationArgs.Contains("UpgradeWithCommand"))
            {
                string[] splitList = notificationArgs.Split(':');
                if (splitList.Length is 2)
                {
                    string appId = splitList[1];
                    Shell32Library.ShellExecute(IntPtr.Zero, "open", "winget.exe", string.Format("upgrade {0}", appId), null, WindowShowStyle.SW_SHOWNORMAL);
                }
                else if (splitList.Length is 3)
                {
                    string appId = splitList[1];
                    string path = splitList[2];
                    Shell32Library.ShellExecute(IntPtr.Zero, "open", "winget.exe", string.Format(@"upgrade {0} -s ""{1}""", appId, path), null, WindowShowStyle.SW_SHOWNORMAL);
                }
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
                LogService.WriteLog(LoggingLevel.Error, "Show notification failed", e);
            }
        }
    }
}
