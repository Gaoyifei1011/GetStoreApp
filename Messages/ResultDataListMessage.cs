using CommunityToolkit.Mvvm.Messaging.Messages;
using GetStoreApp.Models;
using System.Collections.ObjectModel;

namespace GetStoreApp.Messages
{
    public class ResultDataListMessage : ValueChangedMessage<ObservableCollection<ResultDataModel>>
    {
        public ResultDataListMessage(ObservableCollection<ResultDataModel> value) : base(value)
        {
        }
    }
}