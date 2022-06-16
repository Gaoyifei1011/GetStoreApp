using CommunityToolkit.Mvvm.Messaging.Messages;
using GetStoreApp.Models;

namespace GetStoreApp.Messages
{
    public class HistoryMessage : ValueChangedMessage<bool>
    {
        public HistoryMessage(bool value) : base(value)
        {
        }
    }
}
