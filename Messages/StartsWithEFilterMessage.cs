using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GetStoreApp.Messages
{
    /// <summary>
    /// 设置扩展名以“e”开头文件过滤的消息
    /// Set the file to filter messages with an extension that begins with "e"
    /// </summary>
    public class StartsWithEFilterMessage : ValueChangedMessage<bool>
    {
        public StartsWithEFilterMessage(bool value) : base(value)
        {
        }
    }
}
