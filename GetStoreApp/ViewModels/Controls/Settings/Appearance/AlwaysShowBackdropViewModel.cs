using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Controls.Settings.Appearance;
using GetStoreApp.Helpers.Root;

namespace GetStoreApp.ViewModels.Controls.Settings.Appearance
{
    public class AlwaysShowBackdropViewModel : ObservableRecipient
    {
        private IAlwaysShowBackdropService AlwaysShowBackdropService { get; } = ContainerHelper.GetInstance<IAlwaysShowBackdropService>();

        private bool _alwaysShowBackdropValue;

        public bool AlwaysShowBackdropValue
        {
            get { return _alwaysShowBackdropValue; }

            set { SetProperty(ref _alwaysShowBackdropValue, value); }
        }

        public IRelayCommand AlwaysShowBackdropCommand => new RelayCommand<bool>(async (alwaysShowBackdropValue) =>
        {
            await AlwaysShowBackdropService.SetAlwaysShowBackdropAsync(alwaysShowBackdropValue);
            AlwaysShowBackdropValue = alwaysShowBackdropValue;
        });

        public AlwaysShowBackdropViewModel()
        {
            AlwaysShowBackdropValue = AlwaysShowBackdropService.AlwaysShowBackdropValue;
        }
    }
}
