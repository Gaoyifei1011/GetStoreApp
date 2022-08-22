using CommunityToolkit.Mvvm.ComponentModel;
using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.Helpers;
using GetStoreApp.Views;
using Microsoft.UI.Xaml.Navigation;

namespace GetStoreApp.ViewModels.Pages
{
    public class ShellViewModel : ObservableRecipient
    {
        private bool _isBackEnabled;
        private object _selected;

        public INavigationService NavigationService { get; } = IOCHelper.GetService<INavigationService>();

        public INavigationViewService NavigationViewService { get; } = IOCHelper.GetService<INavigationViewService>();

        public bool IsBackEnabled
        {
            get { return _isBackEnabled; }

            set { SetProperty(ref _isBackEnabled, value); }
        }

        public object Selected
        {
            get { return _selected; }

            set { SetProperty(ref _selected, value); }
        }

        public ShellViewModel()
        {
            NavigationService.Navigated += OnNavigated;
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            IsBackEnabled = NavigationService.CanGoBack;

            if (e.SourcePageType == typeof(SettingsPage))
            {
                Selected = NavigationViewService.SettingsItem;
                return;
            }

            var selectedItem = NavigationViewService.GetSelectedItem(e.SourcePageType);
            if (selectedItem != null)
            {
                Selected = selectedItem;
            }
        }
    }
}
