using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Navigation;
using GetStoreApp.Contracts.Services.Controls.Settings.Common;
using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using Microsoft.UI.Xaml.Media.Animation;

namespace GetStoreApp.ViewModels.Pages
{
    public class HomeViewModel : ObservableRecipient, INavigationAware
    {
        private IUseInstructionService UseInstructionService { get; } = ContainerHelper.GetInstance<IUseInstructionService>();

        private INavigationService NavigationService { get; } = ContainerHelper.GetInstance<INavigationService>();

        private bool _useInsVisValue;

        public bool UseInsVisValue
        {
            get { return _useInsVisValue; }

            set { SetProperty(ref _useInsVisValue, value); }
        }

        // 了解应用具体的使用说明
        public IRelayCommand UseInstructionCommand => new RelayCommand(() =>
        {
            App.NavigationArgs = AppNaviagtionArgs.Instructions;
            NavigationService.NavigateTo(typeof(AboutViewModel).FullName, null, new DrillInNavigationTransitionInfo(), false);
        });

        public HomeViewModel()
        {
            UseInsVisValue = UseInstructionService.UseInsVisValue;
        }

        public void OnNavigatedTo(object parameter)
        {
            UseInsVisValue = UseInstructionService.UseInsVisValue;
        }

        public void OnNavigatedFrom()
        { }
    }
}
