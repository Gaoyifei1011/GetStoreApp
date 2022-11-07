using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GetStoreApp.Messages
{
    public class WindowClosedMessage : ValueChangedMessage<bool>
    {
        public WindowClosedMessage(bool value) : base(value)
        {
        }
    }
}
