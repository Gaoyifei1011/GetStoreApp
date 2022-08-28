using CommunityToolkit.Mvvm.Messaging.Messages;
using GetStoreApp.Models;

namespace GetStoreApp.Messages
{
    public class HistoryLiteNumMessage : ValueChangedMessage<HistoryLiteNumModel>
    {
        public HistoryLiteNumMessage(HistoryLiteNumModel value) : base(value)
        {
        }
    }
}
