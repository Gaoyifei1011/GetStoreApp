using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GetStoreApp.Messages
{
    /// <summary>
    /// 设置国家/地区消息
    /// Settings about country and region messages
    /// </summary>
    public sealed class RegionMessage : ValueChangedMessage<string>
    {
        public RegionMessage(string value) : base(value)
        {
        }
    }
}
