using CommunityToolkit.Mvvm.Messaging.Messages;
using GetStoreApp.Models.Controls.Settings.Appearance;

namespace GetStoreApp.Messages
{
    public class BackdropChangedMessage : ValueChangedMessage<BackdropModel>
    {
        public BackdropChangedMessage(BackdropModel value) : base(value)
        {
        }
    }
}
