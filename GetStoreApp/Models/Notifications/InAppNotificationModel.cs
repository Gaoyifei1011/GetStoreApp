using GetStoreApp.Extensions.DataType.Enum;
using GetStoreApp.Models.Base;
using Microsoft.UI.Xaml;

namespace GetStoreApp.Models.Notifications
{
    public class InAppNotificationModel : ModelBase
    {
        public InAppNotificationArgs NotificationArgs
        {
            get { return (InAppNotificationArgs)GetValue(NotificationArgsProperty); }
            set { SetValue(NotificationArgsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NotificationArgs.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NotificationArgsProperty =
            DependencyProperty.Register("NotificationArgs", typeof(InAppNotificationArgs), typeof(InAppNotificationModel), new PropertyMetadata(null));

        public object[] NotificationValue
        {
            get { return (object[])GetValue(NotificationValueProperty); }
            set { SetValue(NotificationValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NotificationValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NotificationValueProperty =
            DependencyProperty.Register("NotificationValue", typeof(object[]), typeof(InAppNotificationModel), new PropertyMetadata(new object()));
    }
}
