using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GetStoreApp.Messages
{
    public class HistoryMessage : ValueChangedMessage<bool>
    {
        public HistoryMessage(bool value) : base(value)
        {
        }
    }
}
