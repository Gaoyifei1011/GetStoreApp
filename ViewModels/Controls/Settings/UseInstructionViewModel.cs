using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Messages;
using GetStoreApp.Services.Settings;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class UseInstructionViewModel : ObservableRecipient
    {
        private bool _useInsVisValue = UseInstructionService.UseInsVisValue;

        public bool UseInsVisValue
        {
            get { return _useInsVisValue; }

            set { SetProperty(ref _useInsVisValue, value); }
        }

        public IAsyncRelayCommand UseInstructionCommand { get; set; }

        public UseInstructionViewModel()
        {
            UseInstructionCommand = new AsyncRelayCommand(UseInstructionAsync);
        }

        public async Task UseInstructionAsync()
        {
            UseInstructionService.SetUseInsVisValue(UseInsVisValue);
            Messenger.Send(new UseInstructionMessage(UseInsVisValue));
            await Task.CompletedTask;
        }
    }
}
