using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GetStoreApp.Messages
{
    public class BlockMapFilterMessage : ValueChangedMessage<bool>
    {
        public BlockMapFilterMessage(bool value) : base(value)
        {
        }
    }
}
