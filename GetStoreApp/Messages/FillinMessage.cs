using CommunityToolkit.Mvvm.Messaging.Messages;
using GetStoreApp.Models.Controls.History;

namespace GetStoreApp.Messages
{
    public class FillinMessage : ValueChangedMessage<HistoryModel>
    {
        public FillinMessage(HistoryModel value) : base(value)
        {
        }
    }
}
