using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GetStoreApp.Messages
{
    public sealed class UseInstructionMessage : ValueChangedMessage<bool>
    {
        public UseInstructionMessage(bool value) : base(value)
        {
        }
    }
}
