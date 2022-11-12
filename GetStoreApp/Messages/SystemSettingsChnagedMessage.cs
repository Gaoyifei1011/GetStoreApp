using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GetStoreApp.Messages
{
    public class SystemSettingsChnagedMessage : ValueChangedMessage<bool>
    {
        public SystemSettingsChnagedMessage(bool value) : base(value)
        {
        }
    }
}
