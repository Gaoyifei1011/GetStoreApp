using CommunityToolkit.Mvvm.Messaging.Messages;
using GetStoreApp.Models;

namespace GetStoreApp.Messages
{
    public class HistoryItemValueMessage : ValueChangedMessage<HistoryLiteNumModel>
    {
        public HistoryItemValueMessage(HistoryLiteNumModel value) : base(value)
        {
        }
    }
}
