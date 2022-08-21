using CommunityToolkit.Mvvm.Messaging.Messages;
using GetStoreApp.Models;

namespace GetStoreApp.Messages
{
    public sealed class RegionMessage : ValueChangedMessage<RegionModel>
    {
        public RegionMessage(RegionModel value) : base(value)
        {
        }
    }
}
