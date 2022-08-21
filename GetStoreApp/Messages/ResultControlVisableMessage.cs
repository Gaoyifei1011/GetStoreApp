using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GetStoreApp.Messages
{
    public class ResultControlVisableMessage : ValueChangedMessage<bool>
    {
        public ResultControlVisableMessage(bool value) : base(value)
        {
        }
    }
}
