using CommunityToolkit.Mvvm.Messaging.Messages;
using GetStoreApp.Behaviors;

namespace GetStoreApp.Messages
{
    /// <summary>
    /// StatusBar图标显示消息
    /// The StatusBar icon displays the message
    /// </summary>
    public class StateImageModeMessage : ValueChangedMessage<StateImageMode>
    {
        public StateImageModeMessage(StateImageMode value) : base(value)
        {
        }
    }
}
