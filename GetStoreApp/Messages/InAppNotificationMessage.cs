using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GetStoreApp.Messages
{
    public class InAppNotificationMessage : ValueChangedMessage<string>
    {
        public InAppNotificationMessage(string value) : base(value)
        {
        }
    }
}
