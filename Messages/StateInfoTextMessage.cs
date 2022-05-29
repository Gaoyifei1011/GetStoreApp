using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GetStoreApp.Messages
{
    /// <summary>
    /// StatusBar显示的文本消息
    /// StatusBar displays the text message
    /// </summary>
    public class StateInfoTextMessage : ValueChangedMessage<string>
    {
        public StateInfoTextMessage(string value) : base(value)
        {
        }
    }
}
