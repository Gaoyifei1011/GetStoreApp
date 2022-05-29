using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GetStoreApp.Messages
{
    /// <summary>
    /// ResultControl控件是否显示消息
    /// Whether the ResultControl control displays a message
    /// </summary>
    public class ResultControlVisableMessage : ValueChangedMessage<bool>
    {
        public ResultControlVisableMessage(bool value) : base(value)
        {
        }
    }
}
