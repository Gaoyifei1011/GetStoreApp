using CommunityToolkit.Mvvm.Messaging.Messages;
using GetStoreApp.Models;

namespace GetStoreApp.Messages
{
    public class HistoryItemMessage : ValueChangedMessage<HistoryModel>
    {
        public HistoryItemMessage(HistoryModel value) : base(value)
        {
        }
    }
}
