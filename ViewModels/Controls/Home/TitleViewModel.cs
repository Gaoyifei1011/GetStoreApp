using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services;
using GetStoreApp.Messages;
using GetStoreApp.Services.Settings;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml.Media.Animation;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GetStoreApp.ViewModels.Controls.Home
{
    public class TitleViewModel : ObservableRecipient
    {
        private readonly INavigationService _navigationService;

        private bool _useInsVisValue = UseInstructionService.UseInsVisValue;

        public bool UseInsVisValue
        {
            get { return _useInsVisValue; }

            set { SetProperty(ref _useInsVisValue, value); }
        }

        private ICommand _useInstructionCommand;

        public ICommand UseInstructionCommand
        {
            get { return _useInstructionCommand; }

            set { SetProperty(ref _useInstructionCommand, value); }
        }

        public TitleViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            UseInstructionCommand = new RelayCommand(async () => { await UseIns_ClickedAsync(); });

            Messenger.Register<TitleViewModel, UseInstructionMessage>(this, (titleViewModel, useInstructionMessage) => titleViewModel.UseInsVisValue = useInstructionMessage.Value);
        }

        private async Task UseIns_ClickedAsync()
        {
            _navigationService.NavigateTo(typeof(AboutViewModel).FullName, null, new DrillInNavigationTransitionInfo());
            await Task.CompletedTask;
        }
    }
}
