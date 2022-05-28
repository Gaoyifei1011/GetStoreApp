using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GetStoreApp.Messages
{
    public class StatePrRingActValueMessage : ValueChangedMessage<bool>
    {
        public StatePrRingActValueMessage(bool value) : base(value)
        {
        }
    }
}