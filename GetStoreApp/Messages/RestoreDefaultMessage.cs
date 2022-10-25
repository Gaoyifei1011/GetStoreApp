using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GetStoreApp.Messages
{
    public class RestoreDefaultMessage : ValueChangedMessage<bool>
    {
        public RestoreDefaultMessage(bool value) : base(value)
        {
        }
    }
}
