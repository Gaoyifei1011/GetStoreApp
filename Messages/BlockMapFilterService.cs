using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GetStoreApp.Messages
{
    public class BlockMapFilterService : ValueChangedMessage<bool>
    {
        public BlockMapFilterService(bool value) : base(value)
        {
        }
    }
}