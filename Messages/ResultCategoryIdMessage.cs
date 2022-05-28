using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GetStoreApp.Messages
{
    public class ResultCategoryIdMessage : ValueChangedMessage<string>
    {
        public ResultCategoryIdMessage(string value) : base(value)
        {
        }
    }
}