using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GetStoreApp.Messages
{
    public class PivotSelectionMessage : ValueChangedMessage<int>
    {
        public PivotSelectionMessage(int value) : base(value)
        {
        }
    }
}
