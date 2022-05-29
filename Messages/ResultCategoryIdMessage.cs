using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GetStoreApp.Messages
{
    // ResultControl控件显示获取数量消息
    // The ResultControl displays the Get Quantity message
    public class ResultCategoryIdMessage : ValueChangedMessage<string>
    {
        public ResultCategoryIdMessage(string value) : base(value)
        {
        }
    }
}
