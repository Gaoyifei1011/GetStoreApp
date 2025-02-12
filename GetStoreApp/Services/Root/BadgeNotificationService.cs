using Microsoft.Windows.BadgeNotifications;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 应用任务栏角标通知提示服务
    /// </summary>
    public static class BadgeNotificationService
    {
        private static readonly BadgeNotificationManager badgeNotificationManager = BadgeNotificationManager.Current;

        /// <summary>
        /// 设置任务栏数字角标的值，并显示
        /// </summary>
        public static void Show(int value)
        {
            badgeNotificationManager.SetBadgeAsCount((uint)value);
        }
    }
}
