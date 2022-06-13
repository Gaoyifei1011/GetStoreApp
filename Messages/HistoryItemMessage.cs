using CommunityToolkit.Mvvm.Messaging.Messages;
using GetStoreApp.Models;

namespace GetStoreApp.Messages
{
    public class HistoryItemMessage : ValueChangedMessage<HistoryItemData>
    {
        public HistoryItemMessage(HistoryItemData value) : base(value)
        {
        }
    }
}
