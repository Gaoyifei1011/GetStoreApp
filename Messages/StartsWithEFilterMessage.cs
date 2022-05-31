using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GetStoreApp.Messages
{
    public class StartsWithEFilterMessage : ValueChangedMessage<bool>
    {
        public StartsWithEFilterMessage(bool value) : base(value)
        {
        }
    }
}
