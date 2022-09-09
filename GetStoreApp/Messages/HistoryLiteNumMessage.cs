using CommunityToolkit.Mvvm.Messaging.Messages;
using GetStoreApp.Models.Settings;

namespace GetStoreApp.Messages
{
    public class HistoryLiteNumMessage : ValueChangedMessage<HistoryLiteNumModel>
    {
        public HistoryLiteNumMessage(HistoryLiteNumModel value) : base(value)
        {
        }
    }
}
