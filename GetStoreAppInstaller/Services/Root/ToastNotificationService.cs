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
                LogService.WriteLog(LoggingLevel.Error, nameof(GetStoreAppInstaller), nameof(ToastNotificationService), nameof(Show), 1, e);
            }
        }
    }
}
