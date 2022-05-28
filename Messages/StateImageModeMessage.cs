using CommunityToolkit.Mvvm.Messaging.Messages;
using GetStoreApp.Behaviors;

namespace GetStoreApp.Messages
{
    public class StateImageModeMessage : ValueChangedMessage<StateImageMode>
    {
        public StateImageModeMessage(StateImageMode value) : base(value)
        {
        }
    }
}