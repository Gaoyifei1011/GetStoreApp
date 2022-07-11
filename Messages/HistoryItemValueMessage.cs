using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GetStoreApp.Messages
{
    public class HistoryItemValueMessage : ValueChangedMessage<string>
    {
        public HistoryItemValueMessage(string value) : base(value)
        {
        }
    }
}
