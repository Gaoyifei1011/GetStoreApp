using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GetStoreApp.Messages
{
    public class StateInfoTextMessage : ValueChangedMessage<string>
    {
        public StateInfoTextMessage(string value) : base(value)
        {
        }
    }
}