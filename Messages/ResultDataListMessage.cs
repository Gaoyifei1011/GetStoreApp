using CommunityToolkit.Mvvm.Messaging.Messages;
using GetStoreApp.Models;
using System.Collections.Generic;

namespace GetStoreApp.Messages
{
    public class ResultDataListMessage : ValueChangedMessage<List<ResultData>>
    {
        public ResultDataListMessage(List<ResultData> value) : base(value)
        {
        }
    }
}
