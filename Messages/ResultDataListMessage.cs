using CommunityToolkit.Mvvm.Messaging.Messages;
using GetStoreApp.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GetStoreApp.Messages
{
    /// <summary>
    /// API获取并解析得到的ResultDataList消息
    /// The API gets and parses the resulting ResultsDataList message
    /// </summary>
    public class ResultDataListMessage : ValueChangedMessage<List<ResultDataModel>>
    {
        public ResultDataListMessage(List<ResultDataModel> value) : base(value)
        {
        }
    }
}
