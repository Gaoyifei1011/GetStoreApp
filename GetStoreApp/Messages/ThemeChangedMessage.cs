using CommunityToolkit.Mvvm.Messaging.Messages;
using GetStoreApp.Models.Controls.Settings.Appearance;

namespace GetStoreApp.Messages
{
    public class ThemeChangedMessage : ValueChangedMessage<ThemeModel>
    {
        public ThemeChangedMessage(ThemeModel value) : base(value)
        {
        }
    }
}
