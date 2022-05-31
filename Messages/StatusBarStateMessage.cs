using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GetStoreApp.Messages
{
    public class StatusBarStateMessage : ValueChangedMessage<int>
    {
        public StatusBarStateMessage(int value) : base(value)
        {
        }
    }
}
