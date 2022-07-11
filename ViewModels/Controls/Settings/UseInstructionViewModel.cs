using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Messages;
using System;
using System.Diagnostics;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class UseInstructionViewModel : ObservableRecipient
    {
        private readonly IUseInstructionService _useInstructionService;

        private bool _useInsVisValue;

        public bool UseInsVisValue
        {
            get { return _useInsVisValue; }

            set { SetProperty(ref _useInsVisValue, value); }
        }

        public IAsyncRelayCommand UseInstructionCommand { get; set; }

        public UseInstructionViewModel(IUseInstructionService useInstructionService)
        {
            _useInstructionService = useInstructionService;

            UseInsVisValue = _useInstructionService.UseInsVisValue;

            UseInstructionCommand = new AsyncRelayCommand<bool>(async (param) =>
            {
                await _useInstructionService.SetUseInsVisValueAsync(param);
                Messenger.Send(new UseInstructionMessage(param));
                UseInsVisValue = param;
            });
        }
    }
}
