using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Messages;
using System;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class UseInstructionViewModel : ObservableRecipient
    {
        private readonly IUseInstructionService UseInstructionService;

        private bool _useInsVisValue;

        public bool UseInsVisValue
        {
            get { return _useInsVisValue; }

            set { SetProperty(ref _useInsVisValue, value); }
        }

        public IAsyncRelayCommand UseInstructionCommand { get; set; }

        public UseInstructionViewModel(IUseInstructionService useInstructionService)
        {
            UseInstructionService = useInstructionService;

            UseInsVisValue = UseInstructionService.UseInsVisValue;

            UseInstructionCommand = new AsyncRelayCommand<bool>(async (param) =>
            {
                await UseInstructionService.SetUseInsVisValueAsync(param);
                Messenger.Send(new UseInstructionMessage(param));
                UseInsVisValue = param;
            });
        }
    }
}
