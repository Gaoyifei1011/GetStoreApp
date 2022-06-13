using CommunityToolkit.Mvvm.Messaging.Messages;
using GetStoreApp.Models;

namespace GetStoreApp.Messages
{
    public class FillinMessage : ValueChangedMessage<HistoryItemData>
    {
        public FillinMessage(HistoryItemData value) : base(value)
        {
        }
    }
}
