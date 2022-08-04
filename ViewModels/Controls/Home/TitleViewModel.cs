using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.Messages;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml.Media.Animation;
using System.Threading.Tasks;

namespace GetStoreApp.ViewModels.Controls.Home
{
    public class TitleViewModel : ObservableRecipient
    {
        private IUseInstructionService UseInstructionService { get; } = App.GetService<IUseInstructionService>();

        private INavigationService NavigationService { get; } = App.GetService<INavigationService>();

        private bool _useInsVisValue;

        public bool UseInsVisValue
        {
            get { return _useInsVisValue; }

            set { SetProperty(ref _useInsVisValue, value); }
        }

        public IAsyncRelayCommand UseInstructionCommand { get; set; }

        public TitleViewModel()
        {
            UseInsVisValue = UseInstructionService.UseInsVisValue;

            UseInstructionCommand = new AsyncRelayCommand(async () =>
            {
                NavigationService.NavigateTo(typeof(AboutViewModel).FullName, null, new DrillInNavigationTransitionInfo());
                await Task.CompletedTask;
            });

            Messenger.Register<TitleViewModel, UseInstructionMessage>(this, (titleViewModel, useInstructionMessage) => titleViewModel.UseInsVisValue = useInstructionMessage.Value);
        }
    }
}
