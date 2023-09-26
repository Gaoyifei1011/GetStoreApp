using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace GetStoreApp.Services.Root
{
    /// <summary>
    /// 应用任务栏数字角标提示服务
    /// </summary>
    public static class BadgeNotificationService
    {
        /// <summary>
        /// 设置任务栏数字角标的值，并显示
        /// </summary>
        public static void Show(int value)
        {
            string badgeXmlString = string.Format("<badge value='{0}'/>", value);
            XmlDocument badgeDocument = new XmlDocument();
            badgeDocument.LoadXml(badgeXmlString);

            BadgeNotification badge = new BadgeNotification(badgeDocument);
            BadgeUpdater badgeUpdater = BadgeUpdateManager.CreateBadgeUpdaterForApplication();
            badgeUpdater.Update(badge);
        }
    }
}
