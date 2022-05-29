using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GetStoreApp.Messages
{
    /// <summary>
    /// 使用说明按钮是否显示消息
    /// Use Instruction button to display a message
    /// </summary>
    public sealed class UseInstructionMessage : ValueChangedMessage<bool>
    {
        public UseInstructionMessage(bool value) : base(value)
        {
        }
    }
}
