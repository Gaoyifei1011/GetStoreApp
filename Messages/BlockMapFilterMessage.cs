using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GetStoreApp.Messages
{
    /// <summary>
    /// BlockMap文件过滤消息
    /// BlockMap file filters messages
    /// </summary>
    public class BlockMapFilterMessage : ValueChangedMessage<bool>
    {
        public BlockMapFilterMessage(bool value) : base(value)
        {
        }
    }
}
