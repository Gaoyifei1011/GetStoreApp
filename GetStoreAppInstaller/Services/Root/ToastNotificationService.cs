using Microsoft.Windows.AppNotifications;
using System;
using Windows.Data.Xml.Dom;
using Windows.Foundation.Diagnostics;
using Windows.UI.Notifications;

namespace GetStoreAppInstaller.Services.Root
{
    /// <summary>
    /// 应用通知服务
    /// </summary>
    public class ToastNotificationService
    {
        public static ToastNotifier AppToastNotifier { get; } = ToastNotificationManager.CreateToastNotifier();

        public static AppNotificationManager AppNotificationManager { get; } = AppNotificationManager.Default;

        /// <summary>
        /// 显示通知
        /// </summary>
        public static void Show(AppNotification appNotification)
        {
            try
            {
                if (AppNotificationManager.IsSupported())
                {
                    AppNotificationManager.Show(appNotification);
                }
                else
                {
                    XmlDocument notificationDocument = new();
                    notificationDocument.LoadXml(appNotification.Payload);
                    ToastNotification toastNotification = new(notificationDocument);
                    AppToastNotifier.Show(toastNotification);
                }
            }
            catch (Exception e)
            {
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreAppInstaller), nameof(ToastNotificationService), nameof(Show), 1, e);
            }
        }
    }
}
