using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GetStoreApp.Messages
{
    /// <summary>
    /// 设置历史记录显示条目数量消息
    /// Settings about history to display the number of entries message
    /// </summary>
    public class HistoryItemValueMessage : ValueChangedMessage<int>
    {
        public HistoryItemValueMessage(int value) : base(value)
        {
        }
    }
}
