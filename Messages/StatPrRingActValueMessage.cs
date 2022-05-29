using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GetStoreApp.Messages
{
    /// <summary>
    /// StatusBar显示圆环动画是否激活消息
    /// StatusBar displays a message if the ring animation is activated
    /// </summary>
    public class StatePrRingActValueMessage : ValueChangedMessage<bool>
    {
        public StatePrRingActValueMessage(bool value) : base(value)
        {
        }
    }
}
