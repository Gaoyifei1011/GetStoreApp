using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GetStoreApp.Messages
{
    /// <summary>
    /// BlockMap文件过滤消息
    /// BlockMap file filters messages
    /// </summary>
    public class BlockMapFilterService : ValueChangedMessage<bool>
    {
        public BlockMapFilterService(bool value) : base(value)
        {
        }
    }
}
