using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GetStoreApp.Messages
{
    /// <summary>
    /// StatusBar显示圆环动画是否显示消息
    /// StatusBar displays whether the ring animation displays a message
    /// </summary>
    public class StatePrRingVisValueMessage : ValueChangedMessage<bool>
    {
        public StatePrRingVisValueMessage(bool value) : base(value)
        {
        }
    }
}
