using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GetStoreApp.Messages
{
    public class CommandMessage : ValueChangedMessage<string[]>
    {
        public CommandMessage(string[] value) : base(value)
        {
        }
    }
}
