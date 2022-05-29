using CommunityToolkit.Mvvm.Messaging.Messages;
using GetStoreApp.Models;
using System.Collections.Generic;

namespace GetStoreApp.Messages
{
    /// <summary>
    /// API获取并解析得到的ResultDataList消息
    /// The API gets and parses the resulting ResultsDataList message
    /// </summary>
    public class ResultDataListMessage : ValueChangedMessage<List<ResultData>>
    {
        public ResultDataListMessage(List<ResultData> value) : base(value)
        {
        }
    }
}
