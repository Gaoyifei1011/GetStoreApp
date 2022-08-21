using CommunityToolkit.Mvvm.Messaging.Messages;
using GetStoreApp.Models;

namespace GetStoreApp.Messages
{
    public class HistoryItemValueMessage : ValueChangedMessage<HistoryItemValueModel>
    {
        public HistoryItemValueMessage(HistoryItemValueModel value) : base(value)
        {
        }
    }
}
