using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GetStoreApp.Messages
{
    public class StatePrRingVisValueMessage : ValueChangedMessage<bool>
    {
        public StatePrRingVisValueMessage(bool value) : base(value)
        {
        }
    }
}