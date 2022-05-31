using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GetStoreApp.Messages
{
    public sealed class RegionMessage : ValueChangedMessage<string>
    {
        public RegionMessage(string value) : base(value)
        {
        }
    }
}
