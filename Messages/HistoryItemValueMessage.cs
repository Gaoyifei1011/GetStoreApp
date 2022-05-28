using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GetStoreApp.Messages
{
    public class HistoryItemValueMessage : ValueChangedMessage<int>
    {
        public HistoryItemValueMessage(int value) : base(value)
        {
        }
    }
}