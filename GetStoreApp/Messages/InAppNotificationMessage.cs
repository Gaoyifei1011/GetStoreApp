using CommunityToolkit.Mvvm.Messaging.Messages;
using GetStoreApp.Models.Notification;

namespace GetStoreApp.Messages
{
    public class InAppNotificationMessage : ValueChangedMessage<InAppNotificationModel>
    {
        public InAppNotificationMessage(InAppNotificationModel value) : base(value)
        {
        }
    }
}
