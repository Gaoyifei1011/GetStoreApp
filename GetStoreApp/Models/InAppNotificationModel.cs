using GetStoreApp.Models.Base;
using Microsoft.UI.Xaml;

namespace GetStoreApp.Models
{
    public class InAppNotificationModel : ModelBase
    {
        public string NotificationContent
        {
            get { return (string)GetValue(NotificationContentProperty); }
            set { SetValue(NotificationContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NotificationContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NotificationContentProperty =
            DependencyProperty.Register("NotificationContent", typeof(string), typeof(InAppNotificationModel), new PropertyMetadata(string.Empty));

        public object[] NotificationValue
        {
            get { return (object[])GetValue(NotificationValueProperty); }
            set { SetValue(NotificationValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NotificationValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NotificationValueProperty =
            DependencyProperty.Register("NotificationValue", typeof(object[]), typeof(InAppNotificationModel), new PropertyMetadata(null));
    }
}
