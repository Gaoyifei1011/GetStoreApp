using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GetStoreApp.Messages
{
    public class TrayIconDisposeMessage : ValueChangedMessage<bool>
    {
        public TrayIconDisposeMessage(bool value) : base(value)
        {
        }
    }
}
