using CommunityToolkit.Mvvm.Messaging.Messages;
using GetStoreApp.Models;
using System.Collections.Generic;

namespace GetStoreApp.Messages
{
    public class ResultDataListMessage : ValueChangedMessage<List<ResultModel>>
    {
        public ResultDataListMessage(List<ResultModel> value) : base(value)
        {
        }
    }
}
