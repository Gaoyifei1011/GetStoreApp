using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 应用任务栏数字角标提示服务
    /// </summary>
    public static class BadgeNotificationService
    {
        private static readonly string badgeXmlString = "<badge value='{0}'/>";
        private static readonly BadgeUpdater badgeUpdater = BadgeUpdateManager.CreateBadgeUpdaterForApplication();
        private static readonly XmlDocument badgeDocument = new();

        /// <summary>
        /// 设置任务栏数字角标的值，并显示
        /// </summary>
        public static void Show(int value)
        {
            badgeDocument.LoadXml(string.Format(badgeXmlString, value));
            BadgeNotification badge = new(badgeDocument);
            badgeUpdater.Update(badge);
        }
    }
}
